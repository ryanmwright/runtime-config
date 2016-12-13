using Microsoft.Extensions.Configuration;

namespace RuntimeConfig.SqlServer
{
    public static class Extensions
    {
        public static IConfigurationBuilder AddSqlServerDatabase(this IConfigurationBuilder builder, string connectionString, string databaseName, string tableSchema = "dbo", string tableName = "Setting", int pollingIntervalSeconds = 30)
        {
            return builder.Add(new SqlServerDatabaseConfigurationSource(connectionString, databaseName, tableSchema, tableName, pollingIntervalSeconds));
        }
    }
}
