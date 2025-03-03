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
        
        App.LoadApp(args);
        
        new Ui().Run();
        
        App.UnloadApp();
    }
}

