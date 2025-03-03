using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace Store.Data;

public class DataLayer : IAppLayer
{
    public override void Load(string[] args, IHostBuilder host)
    {
        host.ConfigureServices((context, collection) =>
        {
            collection.AddDbContextPool<StoreContext>(options =>
            {
                    string? connectionString = context.Configuration.GetConnectionString("StoreContext");
                    if (string.IsNullOrEmpty(connectionString)) throw new NullReferenceException("Connection string is null");
                    options.UseMySQL(connectionString);
                
                    options.EnableDetailedErrors(true);
            });
            
            collection.AddTransient<IDataStorage, StoreContext>();
            
            collection.AddTransient<IDataWorker, DbDataWorker>();
        });
        
    }
}