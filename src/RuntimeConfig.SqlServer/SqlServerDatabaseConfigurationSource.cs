using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuntimeConfig.SqlServer
{
    public class SqlServerDatabaseConfigurationSource : IConfigurationSource
    {
        private readonly string _tableName;
        private readonly string _tableSchema;
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly int _pollingIntervalSeconds;

        public SqlServerDatabaseConfigurationSource(string connectionString, string databaseName, string tableSchema, string tableName, int pollingIntervalSeconds)
        {
            _tableName = tableName;
            _tableSchema = tableSchema;
            _connectionString = connectionString;
            _databaseName = databaseName;
            _pollingIntervalSeconds = pollingIntervalSeconds;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SqlServerConfigurationProvider(_connectionString, _databaseName, _tableSchema, _tableName, _pollingIntervalSeconds);
        }
    }
}
