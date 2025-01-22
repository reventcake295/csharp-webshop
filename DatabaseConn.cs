namespace Store;

using Microsoft.Extensions.Configuration;
using MySqlConnector;

internal class DatabaseConn
{
    // create the standard boilerplate code for a Singleton class
    
    /// <summary>
    /// The instance of the class
    /// </summary>
    private static DatabaseConn? _instance;

    /// <summary>
    /// Get the instance of the class or create the instance if not done before
    /// </summary>
    /// <returns>The class instance</returns>
    private static DatabaseConn GetInstance() => _instance ??= new DatabaseConn();

    private DatabaseConn()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
                                       .SetBasePath(Directory.GetCurrentDirectory())
                                       .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();
        IConfigurationSection dbSection = config.GetRequiredSection("Settings");
        string connectionString = $"Server={dbSection.GetValue<string>("database")}; "+
                                  $"Username={dbSection.GetValue<string>("user")}; "+
                                  $"Password={dbSection.GetValue<string>("password")}; "+
                                  $"Database={dbSection.GetValue<string>("schema")}; "+
                                  "AllowUserVariables=true;";
        _connection = new MySqlConnection(connectionString);
    }
    
    private readonly MySqlConnection _connection;
    
    internal static Task<int> ExecuteCommandAsync(MySqlBatch command)
    {
        Task<int> result = GetInstance()._connection.OpenAsync().ContinueWith(_ =>
        {
            command.Connection = GetInstance()._connection;
            Task<int> result1 = command.ExecuteNonQueryAsync().ContinueWith(task1 => task1.Result);
            return result1.Result;
        });
        return result;
    }

    internal static Task<MySqlDataReader> ExecuteReaderAsync(MySqlBatch command)
    {
        Task<Task<MySqlDataReader>> result = GetInstance()._connection.OpenAsync().ContinueWith(_ =>
        {
            command.Connection = GetInstance()._connection;
            Task<MySqlDataReader> result = command.ExecuteReaderAsync();
            return result;
        });
        return result.Result;
    }
}