using System;
using System.Configuration;

namespace Known.QLite.Helpers
{
    public static class DbHelperFactory
    {
        public static DbHelper Create()
        {
            return Create("Default");
        }

        public static DbHelper Create(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");

            var settings = ConfigurationManager.ConnectionStrings[connectionName];
            return Create(settings.ProviderName, settings.ConnectionString);
        }

        public static DbHelper Create(string providerName, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return new SqlDbHelper(connectionString);
                case "System.Data.OracleClient":
                    return new OracleDbHelper(connectionString);
                default:
                    return new SqlDbHelper(connectionString);
            }
        }
    }
}
