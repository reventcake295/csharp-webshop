using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Interfaces.Repository;
using Store.Common.Model;
using Store.Data.Models;
using Store.Data.Repository;

namespace Store.Data;

public class DbDataWorker : IDataWorker
{
    private readonly StoreContext _context;
    
    public DbDataWorker(IDataStorage context)
    {
        _context = (StoreContext)context;
        Product = new DbRangeRepository<ProductModel, ProductData>(_context);
        Order = new DbRangeRepository<OrderModel, OrderData>(_context);
        Money = new DbRangeRepository<MoneyModel, MoneyData>(_context);
        OrderProduct = new DbChildRepository<OrderProductModel, OrderProductData, OrderModel, OrderData>(_context);
        Tax = new DbRangeRepository<TaxModel, TaxData>(_context);
        Setting = new DbGenericRepository<SettingModel, SettingData>(_context);
        User = new DbUserRepository<UserData>(_context);
    }

    public bool EnsureCreated() => _context.EnsureCreated();
    
    public async Task<bool> EnsureCreatedAsync() => await _context.EnsureCreatedAsync();
    
    public DataSessionState SessionState { get; set; }
    public void StartSession()
    {
        _context.UpdatedIds.Empty();
        if (SessionState is not (DataSessionState.Initialized or DataSessionState.Finished)) throw new InvalidOperationException("Session has already been activated");
        _context.SessionState = DataSessionState.Started;
        SessionState = DataSessionState.Started;
    }
    
    public async Task FinishSessionAsync()
    {
        if (SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        _context.SessionState = DataSessionState.Finishing;
        SessionState = DataSessionState.Finishing;
        
        await SaveChangesAsync();
        
        // we first update the internal Session state and then update the ids before declaring to the application that we are finished
        _context.SessionState = DataSessionState.Finished;
        
        UpdateIds();
        
        SessionState = DataSessionState.Finished;
    }
    
    public void UpdateIds()
    {
        if (_context.SessionState != DataSessionState.Finished) throw new InvalidOperationException("Session is not finished");
        _context.UpdatedIds.UpdateIds();
    }
    
    private async Task<int> SaveChangesAsync() => await _context.SaveDataChangesAsync();
    
    public IRangeGenericRepository<ProductModel> Product { get; set; }
    public IRangeGenericRepository<OrderModel> Order { get; set; }
    public IRangeGenericRepository<MoneyModel> Money { get; set; }
    public IChildGenericRepository<OrderProductModel, OrderModel> OrderProduct { get; set; }
    public IRangeGenericRepository<TaxModel> Tax { get; set; }
    public IUserRepository User { get; set; }
    public IGenericRepository<SettingModel> Setting { get; set; }
    
    public void Dispose() => _context.Dispose();

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}