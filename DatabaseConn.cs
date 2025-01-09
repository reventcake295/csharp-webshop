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
    internal static DatabaseConn GetInstance()
    {
        return _instance ??= new DatabaseConn();
    }

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
                                  $"Database={dbSection.GetValue<string>("schema")};";
        _connection = new MySqlConnection(connectionString);
    }
    
    private readonly MySqlConnection _connection;

    internal Task<int> ExecuteCommandAsync(MySqlCommand command)
    {
        Task<int> result = _connection.OpenAsync().ContinueWith(task =>
        {
            command.Connection = _connection;
            Task<int> result1 = command.ExecuteNonQueryAsync().ContinueWith(task1 =>
            {
                _connection.CloseAsync();
                return task1.Result;
            });
            return result1.Result;
        });
        return result;
    }

    internal Task<MySqlDataReader> ExecuteReaderAsync(MySqlCommand command)
    {
        Task<Task<MySqlDataReader>> result = _connection.OpenAsync().ContinueWith(task =>
        {
            command.Connection = _connection;
            Task<MySqlDataReader> result = command.ExecuteReaderAsync();
            return result;
        });
        return result.Result;
    }
}