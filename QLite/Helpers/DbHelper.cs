using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Known.QLite.Helpers
{
    public abstract class DbHelper
    {
        private string _connectionString;

        public DbHelper(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            _connectionString = connectionString;
        }

        public Dictionary<string, object> EmptyParameter
        {
            get { return null; }
        }

        public Dictionary<string, object> CreateParameters()
        {
            return new Dictionary<string, object>();
        }

        public int ExecuteNonQuery(string commandText, Dictionary<string, object> parameters)
        {
            using (var connection = CreateConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, commandText, parameters);
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string procedureName, params object[] parameterValues)
        {
            using (var connection = CreateConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, procedureName, parameterValues);
                return command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string commandText, Dictionary<string, object> parameters)
        {
            using (var connection = CreateConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, commandText, parameters);
                return (T)command.ExecuteScalar();
            }
        }

        public T ExecuteScalar<T>(string procedureName, params object[] parameterValues)
        {
            using (var connection = CreateConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, procedureName, parameterValues);
                return (T)command.ExecuteScalar();
            }
        }

        public DbDataReader ExecuteReader(string commandText, Dictionary<string, object> parameters)
        {
            var connection = CreateConnection(_connectionString);
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, commandText, parameters);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public DbDataReader ExecuteReader(string procedureName, params object[] parameterValues)
        {
            var connection = CreateConnection(_connectionString);
            using (var command = connection.CreateCommand())
            {
                PrepareCommand(command, procedureName, parameterValues);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public void Fill(DataTable dataTable, string commandText, Dictionary<string, object> parameters)
        {
            using (var reader = ExecuteReader(commandText, parameters))
            {
                dataTable.Load(reader);
            }
        }

        public void Fill(DataTable dataTable, string procedureName, params object[] parameterValues)
        {
            using (var reader = ExecuteReader(procedureName, parameterValues))
            {
                dataTable.Load(reader);
            }
        }

        //public void Fill(DataTable dataTable, int startRecord, int maxRecords, string commandText, Dictionary<string, object> parameters)
        //{
        //    using (var connection = CreateConnection(_connectionString))
        //    using (var command = connection.CreateCommand())
        //    {
        //        var adapter = CreateDataAdapter();
        //        PrepareCommand(command, commandText, parameters);
        //        adapter.SelectCommand = command;
        //        adapter.Fill(startRecord, maxRecords, dataTable);
        //    }
        //}

        //public void Fill(DataTable dataTable, int startRecord, int maxRecords, string procedureName, params object[] parameterValues)
        //{
        //    using (var connection = CreateConnection(_connectionString))
        //    using (var command = connection.CreateCommand())
        //    {
        //        var adapter = CreateDataAdapter();
        //        PrepareCommand(command, procedureName, parameterValues);
        //        adapter.SelectCommand = command;
        //        adapter.Fill(startRecord, maxRecords, dataTable);
        //    }
        //}

        protected abstract string ParameterPrefix { get; }
        protected abstract DbConnection CreateConnection(string connectionString);
        //protected abstract DbDataAdapter CreateDataAdapter();
        protected abstract void DeriveParameters(DbCommand command);
        
        private void PrepareCommand(DbCommand command, string commandText, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            command.CommandText = commandText.Trim();

            if (parameters != null && parameters.Count > 0)
            {
                command.CommandText = command.CommandText.Replace("?", ParameterPrefix);

                foreach (string key in parameters.Keys)
                {
                    DbParameter parameter = command.CreateParameter();
                    parameter.ParameterName = ParameterPrefix + key;
                    parameter.Value = FormatParameterValue(parameters[key]);
                    command.Parameters.Add(parameter);
                }
            }

            OpenConnection(command);
        }

        private void PrepareCommand(DbCommand command, string procedureName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(procedureName))
                throw new ArgumentNullException("procedureName");

            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            OpenConnection(command);

            if (parameterValues != null && parameterValues.Length > 0)
            {
                DeriveParameters(command);
                int parameterIndex = 0;

                foreach (DbParameter parameter in command.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Input)
                    {
                        if (parameterIndex >= parameterValues.Length)
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            parameter.Value = FormatParameterValue(parameterValues[parameterIndex++]);
                        }
                    }
                }
            }
        }

        private static void OpenConnection(DbCommand command)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }
        }

        private static object FormatParameterValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            string valueString = value.ToString().Trim();

            if (valueString == string.Empty)
            {
                return DBNull.Value;
            }

            return valueString;
        }
    }
}
