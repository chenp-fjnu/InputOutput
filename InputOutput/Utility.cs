using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputOutput
{
    public static class ExtensionMethod
    {
        public static string GetExceptionStackTrace(this Exception e)
        {
            var stackTrace = "Exception Message : " + e.Message + "\nInnerException Message: " + ((e.InnerException != null) ? e.InnerException.Message : "") + "\nStackTrace: " + e.StackTrace + "\nInnerException StackTrace : " + ((e.InnerException != null) ? e.InnerException.StackTrace : "");
            return stackTrace;
        }
    }
    public static class Utility
    {
        public static string CreateTableSchema(string columns, string delimiter)
        {
            return CreateTableSchema(columns.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries), null);
        }
        public static string CreateTableSchema(IEnumerable<string> columns)
        {
            return CreateTableSchema(columns, null);
        }
        public static string CreateTableSchema(IEnumerable<string> columns, IEnumerable<string> types)
        {
            if (types != null && types.Count() != 0 && columns.Count() != types.Count()) return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < columns.Count(); index++)
            {
                sb.Append(",'").Append(columns.ElementAt(index)).Append("' ").Append(types == null ? "varchar" : types.ElementAt(index));
            }
            return sb.Length > 0 ? sb.ToString().Substring(1) : sb.ToString();
        }

    }
}
