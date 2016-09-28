using System;
using System.Data;
using System.Web;
using Known.QLite.Helpers;

namespace Known.QLite.WebControls
{
    internal static class CacheHelper
    {
        public static T GetWebCache<T>(string key, Func<T> setValue)
        {
            if (HttpContext.Current.Cache[key] == null)
            {
                HttpContext.Current.Cache[key] = setValue();
            }

            return (T)HttpContext.Current.Cache[key];
        }

        public static DataTable GetCodeTables(string category)
        {
            var dataTable = CodeTables.Clone();
            var rows = CodeTables.Select(string.Format("CATEGORY='{0}'", category));
            //rows.CopyToDataTable(dataTable, LoadOption.OverwriteChanges);
            Array.ForEach<DataRow>(rows, r => dataTable.ImportRow(r));
            return dataTable;
        }

        public static string GetCodeText(string category, string code)
        {
            var codeTable = GetCodeTables(category);
            var rows = codeTable.Select(string.Format("CODE='{0}'", code));

            if (rows.Length > 0)
            {
                return rows[0].Field<string>("TEXT");
            }

            return string.Empty;
        }

        private static DataTable CodeTables
        {
            get { return GetWebCache<DataTable>("CodeTables", () => CodeTableHelper.GetCodeTables()); }
        }
    }
}
