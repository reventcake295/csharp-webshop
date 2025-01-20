using MySqlConnector;

namespace Store;

internal class Settings : SqlBuilder
{
    internal static int MaxInputLoop { get; private set; } = 1;
    internal static string Lang { get; private set; } = "en";
    internal static List<string> AvailableLangs { get; private set; } = [ "en" ];

    internal static Money DefaultMoney { get; private set; } = new();
    internal static Taxes DefaultTaxes { get; private set; } = new();
    
    static Settings()
    {
        // this is required to get the process completely loaded, but once it is loaded, it is no longer necessary
        new Settings().LoadData();
    }


    protected override void LoadData()
    {
        SingleStmt("SELECT DefaultLang, AvailableLangs, MaxInputLoop, DefaultMoney, DefaultTaxes FROM capstoneStore.Settings;");
        Task<MySqlDataReader> settingsLoad = ExecQueryAsync();
        settingsLoad.Wait();
        if (settingsLoad.Result == null) throw new FileLoadException("Failed to load settings");
        MySqlDataReader settings = settingsLoad.Result;
        settings.Read();
        Lang = settings.GetString("DefaultLang");
        MaxInputLoop = settings.GetInt32("MaxInputLoop");
        AvailableLangs = settings.GetString("AvailableLangs").Split(',').ToList();
        DefaultMoney = Money.MoneyTypes[settings.GetInt32("DefaultMoney")];
        DefaultTaxes = Taxes.TaxTypes[settings.GetInt32("DefaultTaxes")];
        settings.CloseAsync();
    }
}