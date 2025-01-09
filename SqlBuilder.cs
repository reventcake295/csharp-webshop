namespace Store;

using System.Threading.Tasks;
using MySqlConnector;

internal abstract class SqlBuilder
{
    private readonly MySqlCommand _stmt = new();

    protected void StartStmt(string statement)
    {
        _stmt.CommandText = statement;
    }

    protected void AddArg(string key, object value)
    {
        _stmt.Parameters.AddWithValue(key, value);
    }

    protected Task<int> ExecCmdAsync()
    { // return the task with the int of affected rows
        return DatabaseConn.GetInstance().ExecuteCommandAsync(_stmt).ContinueWith((task =>
                                                                               {
                                                                                   CloseConnection();
                                                                                   return task.Result;
                                                                               }));
    }

    protected Task<MySqlDataReader> ExecQueryAsync()
    {
        return DatabaseConn.GetInstance().ExecuteReaderAsync(_stmt);
    }

    /// <summary>
    /// Close the connection and clear the parameters currently stored in the _stmt variable
    /// </summary>
    protected void CloseConnection()
    {
        _stmt.Connection?.CloseAsync();
        _stmt.Parameters.Clear();
    }
    protected virtual void LoadData()
    {
        throw new MissingMethodException();
    }
    
    protected virtual void LoadData(int id)
    {
        throw new MissingMethodException();
    }
}