using Microsoft.Extensions.Hosting;
using Store.Common;
using Store.Common.Enums;
using Store.Data;

namespace Store.Cli;

public class Program
{
    public static void Main(string[] args)
    {
        App.AddAppLayer(new CommonLayer());
        App.AddAppLayer(new DataLayer());
        App.AddAppLayer(new ServerCliLayer());
        
        HostApplicationBuilder builder = (HostApplicationBuilder)App.LoadApp(new HostApplicationBuilder(), args);
        IHost host = builder.Build();
        App.PostLoad(host);
        
        new Ui().Run();
        
        App.UnloadApp();
    }
}

