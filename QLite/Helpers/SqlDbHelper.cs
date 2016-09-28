using System.Data.Common;
using System.Data.SqlClient;

namespace Known.QLite.Helpers
{
    internal sealed class SqlDbHelper : DbHelper
    {
        internal SqlDbHelper(string connectionName)
            : base(connectionName)
        {
        }

        protected override string ParameterPrefix
        {
            get { return "@"; }
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        //protected override DbDataAdapter CreateDataAdapter()
        //{
        //    return new SqlDataAdapter();
        //}

        protected override void DeriveParameters(DbCommand command)
        {
            SqlCommandBuilder.DeriveParameters((SqlCommand)command);
        }
    }
}
