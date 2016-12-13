using System.Collections.Generic;
using System.Data.SqlClient;

namespace RuntimeConfig.SqlServer
{
    public class SqlServerConfigurationProvider : PollingConfigurationProvider
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _tableSchema;
        private readonly string _tableName;
        private readonly string _selectStatement;
        private readonly string _keyFieldName;
        private readonly string _valueFieldName;

        public SqlServerConfigurationProvider(string connectionString, string databaseName, string tableSchema, string tableName, int pollingIntervalSeconds)
            : base(pollingIntervalSeconds)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _tableSchema = tableSchema;
            _tableName = tableName;
            _keyFieldName = "SettingKey";
            _valueFieldName = "SettingValue";

            _selectStatement = $"SELECT [{_keyFieldName}], [{_valueFieldName}] FROM [{tableSchema}].[{tableName}]";
        }

        protected override Dictionary<string, string> GetConfiguration()
        {
            var data = new Dictionary<string, string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(_databaseName);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = _selectStatement;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var key = reader.GetString(reader.GetOrdinal(_keyFieldName));
                        var value = reader.GetString(reader.GetOrdinal(_valueFieldName));
                        data.Add(key, value);
                    }
                }
            }

            return data;
        }
    }
}
