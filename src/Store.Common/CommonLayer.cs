using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Common;

public class CommonLayer : IAppLayer
{
    public override void Load(string[] args, IHostBuilder host)
    {
        host.ConfigureServices(collection => collection.AddScoped<OrderFactory>());
    }

    public override void PostLoad(string[] args, IHost host)
    {
        // start seeding the database, should be possible now
        Initialize(host.Services.GetRequiredService<IDataWorker>());
    }
    
    private static void Initialize(IDataWorker context)
    {
        context.EnsureCreated();

        // Look for any user.
        if (context.User.FindWhere(model => model.Perm == Perm.Admin).Any())
            return; // DB has been seeded
        
        // if there is no User then create at least one user with admin permissions to ensure that the Application remains usable
        context.StartSession();
        context.User.Register(new UserModel(
              1,
              "admin", 
              // password: "admin",
              "admin@admin.com",
              "",
              0,
              "",
              "",
              "",
              Perm.Admin
        ), "admin");
        
        context.FinishSessionAsync().GetAwaiter().GetResult();
    }
    
}