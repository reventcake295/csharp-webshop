using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Common;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli;

public class ServerCliLayer : IAppLayer
{
    public override IHostApplicationBuilder Load(string[] args, IHostApplicationBuilder host)
    {
        // this is added as a Singleton as opposed to a scoped due to the fact that the server Cli only a single user has, no matter what.
        // if the user changes, then that means that the previous user has already logged out or rebooted the application
        host.Services.AddSingleton<IUserSession, ServerUserSession>();

//        host.ConfigureServices((context, collection) => collection.AddHostedService<ConsoleService>());
        return host;
    }

    public override IHost PostLoad(IHost host)
    {
        SettingModel? settingModel = host.Services.GetRequiredService<IDataWorker>().Setting.GetByKey(1);
        if (settingModel == null) throw new ApplicationException("No settings were found");
        App.SetSettings(settingModel);
        return host;
    }
}