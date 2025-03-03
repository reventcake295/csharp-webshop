using Microsoft.EntityFrameworkCore;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Models;

namespace Store.Data;

internal class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options), IDataStorage
{
    internal DbSet<ProductData> Product { get; set; }
    internal DbSet<OrderData> Order { get; set; }
    internal DbSet<MoneyData> Money { get; set; }
    internal DbSet<OrderProductData> OrderProduct { get; set; }
    internal DbSet<TaxData> Tax { get; set; }
    internal DbSet<UserData> User { get; set; }
    internal DbSet<SettingData> Setting { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaxData>().ToTable("Taxes");
        modelBuilder.Entity<MoneyData>().ToTable("Money");
        modelBuilder.Entity<SettingData>().ToTable("Settings");
        modelBuilder.Entity<UserData>().ToTable("users");
        modelBuilder.Entity<ProductData>().ToTable("Products"); 
        modelBuilder.Entity<OrderData>().ToTable("Orders");
        modelBuilder.Entity<OrderProductData>().ToTable("orderProducts");
    }

    public bool EnsureCreated() => Database.EnsureCreated();
    
    public async Task<bool> EnsureCreatedAsync() => await Database.EnsureCreatedAsync();

    public DataSessionState SessionState = DataSessionState.Initialized;
    
    public int SaveDataChanges() => SaveChanges();
    
    public async Task<int> SaveDataChangesAsync() => await SaveChangesAsync();

    public StoredIds UpdatedIds { get; set; } = new();
    
    
    public class StoredIds
    {
        public List<(CommonModel commonModel, IId entityModel)> Ids { get; set; } = [];
        
        public void UpdateIds()
        {
            foreach ((CommonModel commonModel, IId entityModel) x in Ids)
            {
                x.commonModel.Id = x.entityModel.GetId();
            }
        }

        public void Empty()
        {
            Ids = [];
        }
        
        public void AddModel(CommonModel model, IId entity)
        {
            Ids.Add((model, entity));
        }
        
    }
}