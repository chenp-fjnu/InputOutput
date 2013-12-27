using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using log4net;
using System.Data;

namespace InputOutput.DB
{
	[SQLiteFunction(Name = "TABLENOTIFY", FuncType = FunctionType.Scalar, Arguments = 1)]
	public class TABLENOTIFY : System.Data.SQLite.SQLiteFunction
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(TABLENOTIFY));
		public override object Invoke(object[] args)
		{
			args.ToList().ForEach(k => { Logger.DebugFormat("TABLENOTIFY: Arg = {0}", k.ToString()); });

			return null;
		}
	}
	public class SQLiteDAL
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(SQLiteDAL));
		private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

		private static object connectionlocker = new object();
		protected static SQLiteConnection _memConnection;
		private char[] delimiter = new char[] { '|' };

		internal static SQLiteConnection MemConnection
		{
			get
			{
				if (_memConnection == null)
				{
					lock (connectionlocker)
					{
						if (_memConnection == null)
						{
							_memConnection = new SQLiteConnection("Data Source = :memory:");
							_memConnection.Open();
						}
					}
				}
				return _memConnection;
			}
		}

		public SQLiteConnection Connection { get; set; }

		public SQLiteDAL(SQLiteConnection conn, bool TABLENOTIFY = false)
		{
			Connection = conn;
			if (TABLENOTIFY)
			{
				SQLiteFunction.RegisterFunction(typeof(TABLENOTIFY));
			}
		}

		public void CreateTable(string tableName, string tableSchema, string primarycolstring, string indexcolstring = "", bool setupTrigger = false)
		{
			ExecuteNonQuery(string.Format("CREATE TABLE IF NOT EXISTS {0} ({1}{2})", tableName, tableSchema, (string.IsNullOrEmpty(primarycolstring) ? "" : string.Format(", PRIMARY KEY({0})", primarycolstring))));

			if (!string.IsNullOrEmpty(indexcolstring))
			{
				string[] indexcols = indexcolstring.Split(delimiter);
				foreach (var indexcol in indexcols)
				{
					ExecuteNonQuery(string.Format("CREATE INDEX IF NOT EXISTS IDX_{0}_{2} ON {0} ({1})", tableName, indexcol, indexcol.Replace(',', '_')));
				}
			}
			if (setupTrigger)
			{
				SetupTableTrigger(tableName);
			}
		}

		public void DropTable(string tableName)
		{
			ExecuteNonQuery("DROP TABLE IF EXISTS " + tableName);
		}

		public void SetupTableTrigger(string tableName)
		{
			ExecuteNonQuery(string.Format("CREATE TRIGGER IF NOT EXISTS {0}_INSERT AFTER INSERT ON {0} BEGIN SELECT TABLENOTIFY('{0}'); END", tableName));
			ExecuteNonQuery(string.Format("CREATE TRIGGER IF NOT EXISTS {0}_DELETE AFTER DELETE ON {0} BEGIN SELECT TABLENOTIFY('{0}'); END", tableName));
			ExecuteNonQuery(string.Format("CREATE TRIGGER IF NOT EXISTS {0}_UPDATE AFTER UPDATE ON {0} BEGIN SELECT TABLENOTIFY('{0}'); END", tableName));
		}

		public DataTable GetSchema()
		{
			DataTable data = Connection.GetSchema("TABLES");
			return data;
		}

		public int ExecuteNonQuery(string cmdString, SQLiteParameter[] parameters = null, bool reThrowException = false)
		{
			int rowsUpdated = 0;

			using (var cmd = new SQLiteCommand(cmdString, Connection))
			{
				rowsUpdated = ExecuteNonQuery(cmd, parameters, reThrowException);
			}

			return rowsUpdated;
		}

		private int ExecuteNonQuery(SQLiteCommand cmd, SQLiteParameter[] parameters = null, bool reThrowException = false)
		{
			int rowsUpdated = 0;
			watch.Start();
			try
			{
				// can not use transaction for attach detach db.
				if (cmd.CommandText.Contains("ATTACH DATABASE") || cmd.CommandText.Contains("DETACH DATABASE"))
				{
					cmd.CommandTimeout = 180;
					rowsUpdated = cmd.ExecuteNonQuery();
				}
				else
				{
					using (SQLiteTransaction txn = cmd.Connection.BeginTransaction())
					{
						cmd.CommandTimeout = 180;
						if (parameters != null)
						{
							cmd.Parameters.AddRange(parameters);
						}
						rowsUpdated = cmd.ExecuteNonQuery();
						txn.Commit();
					}
				}
				Logger.DebugFormat("ExecuteNonQuery SQL={0}, Elapsed: {1}ms", cmd.CommandText, watch.Elapsed);
			}
			catch (Exception e)
			{
				Logger.Error(String.Format("ExecuteNonQuery: {0}\n{1}", cmd.CommandText, e.GetExceptionStackTrace()));
				if (reThrowException)
					throw e;
			}
			finally
			{
				watch.Reset();
			}
			return rowsUpdated;
		}

		public SQLiteDataReader ExecuteReader(string sql, SQLiteParameter[] parameters = null)
		{
			watch.Start();
			try
			{
				SQLiteCommand command = new SQLiteCommand(sql, Connection);
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters);
				}
				var reader = command.ExecuteReader();
				Logger.DebugFormat("ExecuteReader SQL={0}, Elapsed: {1}ms", sql, watch.Elapsed);
				return reader;
			}
			catch (Exception e)
			{
				Logger.Error(String.Format("ExecuteReader: {0}\n{1}", sql, e.GetExceptionStackTrace()));
				return null;
			}
			finally
			{
				watch.Reset();
			}

		}
		public DataTable ExecuteDataTable(string sql, SQLiteParameter[] parameters = null)
		{
			watch.Start();
			try
			{
				using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
				{
					if (parameters != null)
					{
						command.Parameters.AddRange(parameters);
					}
					SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
					DataTable data = new DataTable();
					adapter.Fill(data);
					Logger.DebugFormat("ExecuteDataTable SQL={0}, Elapsed: {1}ms", sql, watch.Elapsed);
					return data;
				}
			}
			catch (Exception e)
			{
				Logger.Error(String.Format("ExecuteDataTable: {0}\n{1}", sql, e.GetExceptionStackTrace()));
				return null;
			}
			finally
			{
				watch.Reset();
			}
			
		}
		public void AddOrUpdate(string tablename, string columns, string values)
		{
			ExecuteNonQuery(string.Format("INSERT OR REPLACE INTO {0} ({1}) VALUES ({2})", tablename, columns, values));
		}
	}
}
