using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Common;

public static class App
{
    private static readonly List<IAppLayer> _AppLayers = [];
    
    public static IHost? AppHost { get; private set; }
    
    public static bool AppRunning { get; private set; }
    
    private static SettingModel? SettingModel { get; set; }

    public static void SetSettings(SettingModel settingModel)
    {
        if (SettingModel != null) throw new InvalidOperationException("Settings are already set");
        SettingModel = settingModel;
    }
    
    public static void AddAppLayer(IAppLayer appLayer)
    {
        if (AppRunning) throw new InvalidOperationException("App already running");
        _AppLayers.Add(appLayer);
    }

    public static IHostApplicationBuilder LoadApp(IHostApplicationBuilder builder, string[] args)
    {
        if (AppRunning) throw new InvalidOperationException("App already running");
        
        foreach (IAppLayer appLayer in _AppLayers)
            builder = appLayer.PreLoad(args, builder);
        
        foreach (IAppLayer appLayer in _AppLayers)
            builder = appLayer.Load(args, builder);
        
        return builder;
    }

    public static void PostLoad(IHost host)
    {
        if (AppRunning) throw new InvalidOperationException("App already running");
        foreach (IAppLayer appLayer in _AppLayers)
            host = appLayer.PostLoad(host);
        
        AppHost = host;
        
        AppRunning = true;
    }
    
    public static void UnloadApp()
    {
        if (!AppRunning) throw new InvalidOperationException("App is not running");
        foreach (IAppLayer appLayer in _AppLayers)
            appLayer.Unload();
        AppRunning = false;
    }

    public static IDataWorker GetDataWorker()
    {
        if (!AppRunning || AppHost == null) throw new InvalidOperationException("App is not running");
        if (AppHost.Services.GetService(typeof(IDataWorker)) is IDataWorker dataWorker)
            return dataWorker
                ?? throw new NullReferenceException("IDataWorker is not registered");
        throw new InvalidOperationException("App is not running");
    }

    public static SettingModel GetSettings()
    {
        if (SettingModel != null) return SettingModel;
        throw new NullReferenceException("Settings is not registered");
    }

    public static IHost GetAppHost()
    {
        if (!AppRunning || AppHost == null) throw new InvalidOperationException("App is not running");
        return AppHost;
    }

    public static TService GetScopedService<TService>() where TService : notnull
    {
        if (!AppRunning || AppHost == null) throw new InvalidOperationException("App is not running");
        return AppHost.Services.CreateScope().ServiceProvider.GetRequiredService<TService>();
    }
}