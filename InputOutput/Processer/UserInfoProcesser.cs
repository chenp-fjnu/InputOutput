﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using InputOutput.DB;
using log4net;
using System.Data;

namespace InputOutput.Processer
{
    public class UserInfoProcesser : BaseProcesser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserInfoProcesser));
        private char[] delimiter = new char[] { ',', ' ' };

        private static object dallocker = new object();
        protected static SQLiteDAL _DAL;


        protected static SQLiteDAL DAL
        {
            get
            {
                if (_DAL == null)
                {
                    lock (dallocker)
                    {
                        if (_DAL == null)
                        {
                            var _Connection = new SQLiteConnection("Data Source=InputOutput.s3db;Version=3;");
                            _Connection.Open();
                            _DAL = new SQLiteDAL(_Connection, true);
                            _DAL.CreateTable(Constant.UserInfo_TableName, Utility.CreateTableSchema(Constant.UserInfo_Columns, ","), Constant.UserInfo_PrimaryKey, Constant.UserInfo_PrimaryKey, true);
                        }
                    }
                }
                return _DAL;
            }
        }
        public override string Process(string user, string input)
        {
            if (input.ToLower() == "table")
            {
                #region Show TableSchema

                StringBuilder strBuilder = new StringBuilder();
                foreach (DataRow row in DAL.GetSchema().Rows)
                {
                    row.ItemArray.ToList().ForEach((k) =>
                    {
                        strBuilder.Append(k.ToString());
                        strBuilder.Append(";");
                    });
                    strBuilder.AppendLine();
                }
                return strBuilder.ToString();
                #endregion
            }
            else if (input.ToLower() == "data")
            {
                #region Show AllData

                SQLiteParameter[] parameters = new SQLiteParameter[]{  
                        new SQLiteParameter("@id",user.ToLower())
                    };
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine(string.Format("{0} {1}", "Key".PadRight(Constant.UserInfo_KeyLength), "Value".PadRight(Constant.UserInfo_ValueLength)));
                (from t in DAL.ExecuteDataTable("select key,value from user_info where id=@id", parameters).Rows.Cast<DataRow>()
                 select new
                 {
                     key = t.ItemArray[0].ToString(),
                     value = t.ItemArray[1].ToString()
                 }).ToList().ForEach((k) =>
                 {
                     strBuilder.Append(k.key.PadRight(Constant.UserInfo_KeyLength));
                     strBuilder.Append(" ");
                     strBuilder.Append(k.value.PadRight(Constant.UserInfo_ValueLength));
                     strBuilder.AppendLine();
                 });

                return strBuilder.ToString();
                #endregion
            }
            else
            {
                #region Insert Data

                var kvp = input.Split(delimiter);
                if (kvp.Length != 2) return string.Empty;
                var key = kvp[0];
                var value = kvp[1];
                DAL.AddOrUpdate(Constant.UserInfo_TableName, Constant.UserInfo_Columns, string.Format(Constant.UserInfo_ValueFormat, user, key, value));
                Logger.InfoFormat("AddOrUpdate, (id,key,value) {0}", string.Format(Constant.UserInfo_ValueFormat, user, key, value));

                return string.Format(Constant.UserInfo_ProcessResultFormat, key, value);
                #endregion
            }
        }

        public override string ToString()
        {
            return "User Info Processer, Format 'key value' or 'key,value'";
        }
    }
    public partial class Constant
    {
        public const string UserInfo_TableName = "user_info";
        public const string UserInfo_Columns = "id,key,value";
        public const string UserInfo_PrimaryKey = "id,key";
        public const string UserInfo_ValueFormat = "'{0}','{1}','{2}'";
        public const string UserInfo_ProcessResultFormat = "Process new info:{0}={1}";
        public const int UserInfo_IDLength = 10;
        public const int UserInfo_KeyLength = 10;
        public const int UserInfo_ValueLength = 15;
    }
}
