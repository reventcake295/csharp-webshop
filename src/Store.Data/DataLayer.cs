using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Store.Data;

public class DataLayer : IAppLayer
{
    public override IHostApplicationBuilder Load(string[] args, IHostApplicationBuilder host)
    {
        host.Services.AddDbContextPool<StoreContext>(options =>
        {
            string? connectionString = host.Configuration.GetConnectionString("StoreContext");
            if (string.IsNullOrEmpty(connectionString)) throw new NullReferenceException("Connection string is null");
            options.UseMySQL(connectionString);
            
            // depending on the setting in the environ display detailed errors or not
            options.EnableDetailedErrors(host.Environment.IsDevelopment());
        });
        
        host.Services.AddTransient<IDataStorage, StoreContext>();
        
        host.Services.AddTransient<IDataWorker, DbDataWorker>();
        
        return host;
    }
}