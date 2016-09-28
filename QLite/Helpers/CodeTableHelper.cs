using System.Data;

namespace Known.QLite.Helpers
{
    internal static class CodeTableHelper
    {
        public static DataTable GetCodeTables()
        {
            string commandText = "SELECT CATEGORY,CODE,TEXT FROM T_CODE_TABLE WHERE ENABLE=1 ORDER BY CATEGORY,SEQUENCE";
            var dataTable = new DataTable();
            var helper = DbHelperFactory.Create();
            helper.Fill(dataTable, commandText, helper.EmptyParameter);
            return dataTable;
        }
    }
}
