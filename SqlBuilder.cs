namespace Store;

using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MySqlConnector;

internal abstract class SqlBuilder
{
    private readonly MySqlBatch _batch = new();
    private MySqlBatchCommand _stmt = new();

    protected void SingleStmt(string sql)
    {
        StartStmt(sql);
        EndStmt();
    }
    protected void StartStmt(string statement) => _stmt.CommandText = statement;
    
    protected void AddArg(string key, object value) => _stmt.Parameters.AddWithValue(key, value);
    

    protected void EndStmt()
    {
        AddBatch(_stmt);
        _stmt = new MySqlBatchCommand();
    }

    private void AddBatch(MySqlBatchCommand stmt) => _batch.BatchCommands.Add(stmt);
    
    protected Task<int> ExecCmdAsync()
    { // return the task with the int of affected rows
        return new DatabaseConn().ExecuteCommandAsync(_batch).ContinueWith(task =>
        {
            int result = task.Result;
            CloseConnection();
            return result;
        });
    }

    protected Task<MySqlDataReader> ExecQueryAsync() => new DatabaseConn().ExecuteReaderAsync(_batch);

    /// <summary>
    /// Close the connection and clear the commands currently stored in the _batch variable
    /// </summary>
    protected void CloseConnection()
    {
        _batch.Connection?.CloseAsync();
        _batch.BatchCommands.Clear();
    }
    protected virtual void LoadData() => throw new MissingMethodException();
    
    protected virtual void LoadData(int id) => throw new MissingMethodException();
    
    private class DatabaseConn
    {
        // set the connection string to a private static and to only allow it to be used in this class 
        private static string ConnectionString { get; }

        static DatabaseConn()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                           .SetBasePath(Directory.GetCurrentDirectory())
                                           .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            IConfigurationSection dbSection = config.GetRequiredSection("Settings");
            ConnectionString = $"Server={dbSection.GetValue<string>("database")}; "+
                                      $"Username={dbSection.GetValue<string>("user")}; "+
                                      $"Password={dbSection.GetValue<string>("password")}; "+
                                      $"Database={dbSection.GetValue<string>("schema")}; "+
                                      "AllowUserVariables=true;";
        }
        internal DatabaseConn() => _connection = new MySqlConnection(ConnectionString);
        
        private readonly MySqlConnection _connection;
        
        internal Task<int>  ExecuteCommandAsync(MySqlBatch command) =>
            _connection.OpenAsync().ContinueWith(_ =>
            {
                command.Connection = _connection;
                return command.ExecuteNonQueryAsync().Result;
            });

        internal Task<MySqlDataReader> ExecuteReaderAsync(MySqlBatch command) => 
            _connection.OpenAsync().ContinueWith(_ =>
            {
                command.Connection = _connection;
                return command.ExecuteReaderAsync().Result;
            });
        
        /// <summary>
        /// Always try to close the _connection if possible
        /// </summary>
        ~DatabaseConn() => _connection.CloseAsync();
    }
}