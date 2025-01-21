namespace Store;

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
    protected void StartStmt(string statement)
    {
        _stmt.CommandText = statement;
    }
    protected void AddArg(string key, object value)
    {
        _stmt.Parameters.AddWithValue(key, value);
    }

    protected void EndStmt()
    {
        AddBatch(_stmt);
        _stmt = new MySqlBatchCommand();
    }

    private void AddBatch(MySqlBatchCommand stmt)
    {
        _batch.BatchCommands.Add(stmt);
    }
    
    protected Task<int> ExecCmdAsync()
    { // return the task with the int of affected rows
        return DatabaseConn.ExecuteCommandAsync(_batch).ContinueWith(task =>
        {
            int result = task.Result;
            CloseConnection();
            return result;
        });
    }

    protected Task<MySqlDataReader> ExecQueryAsync() => DatabaseConn.ExecuteReaderAsync(_batch);

    /// <summary>
    /// Close the connection and clear the commands currently stored in the _batch variable
    /// </summary>
    protected void CloseConnection()
    {
        _batch.Connection?.CloseAsync();
        _batch.BatchCommands.Clear();
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