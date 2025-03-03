namespace Store.Common.Interfaces;

public interface IDataStorage : IDisposable, IAsyncDisposable
{
    public bool EnsureCreated();

    public Task<bool> EnsureCreatedAsync();
    
    public int SaveDataChanges();

    public Task<int> SaveDataChangesAsync();
}