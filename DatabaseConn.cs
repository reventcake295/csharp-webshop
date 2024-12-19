namespace Store;

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
        
    }
    
    private readonly MySqlConnection _connection = new MySqlConnection("");

    internal Task<int> ExecuteCommandAsync(MySqlCommand command)
    {
        _connection.Open();
        command.Connection = _connection;
        Task<int> result = command.ExecuteNonQueryAsync();
        _connection.Close();
        return result;
    }

    internal Task<MySqlDataReader> ExecuteReaderAsync(MySqlCommand command)
    {
        _connection.Open();
        command.Connection = _connection;
        Task<MySqlDataReader> result = command.ExecuteReaderAsync();
        _connection.Close();
        return result;
    }
}