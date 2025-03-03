using Store.Common.Enums;
using Store.Common.Interfaces.Repository;
using Store.Common.Model;

namespace Store.Common.Interfaces;

public interface IDataWorker : IDisposable, IAsyncDisposable
{
    
    public bool EnsureCreated();
    public Task<bool> EnsureCreatedAsync();

    public DataSessionState SessionState { get; protected set; }
    public void StartSession();
    public Task FinishSessionAsync();

    public void UpdateIds();
    // this is not allowed to be used outside the DataWorker that is to be implemented
//    public int SaveChanges();
//    public Task<int> SaveChangesAsync();
    
    public IRangeGenericRepository<ProductModel> Product { get; set; }
    public IRangeGenericRepository<OrderModel> Order { get; set; }
    public IRangeGenericRepository<MoneyModel> Money { get; set; }
    public IChildGenericRepository<OrderProductModel, OrderModel> OrderProduct { get; set; }
    public IRangeGenericRepository<TaxModel> Tax { get; set; }
    public IUserRepository User { get; set; }
    public IGenericRepository<SettingModel> Setting { get; set; }


}