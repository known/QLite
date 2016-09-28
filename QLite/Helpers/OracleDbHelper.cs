using System.Data.Common;
using System.Data.OracleClient;

namespace Known.QLite.Helpers
{
    internal sealed class OracleDbHelper : DbHelper
    {
        internal OracleDbHelper(string connectionName)
            : base(connectionName)
        {
        }

        protected override string ParameterPrefix
        {
            get { return ":"; }
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        //protected override DbDataAdapter CreateDataAdapter()
        //{
        //    return new OracleDataAdapter();
        //}

        protected override void DeriveParameters(DbCommand command)
        {
            OracleCommandBuilder.DeriveParameters((OracleCommand)command);
        }
    }
}
