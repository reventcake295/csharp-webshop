using MySqlConnector;

namespace Store;

internal class Settings : SqlBuilder
{
    [Mapping(ColumnName = "MaxInputLoop")] internal static int MaxInputLoop { get; private set; } = 1;
    [Mapping(ColumnName = "Lang")] internal static string Lang { get; private set; } = "en";

    [Mapping(ColumnName = "AvailableLangs")] internal static List<string> AvailableLangs { get; private set; } = [ "en" ];

    static Settings()
    {
        // this is required to get the process completely loaded but once it is loaded it is no longer needed
        new Settings().LoadData();
    }

    protected override void LoadData()
    {
        StartStmt("SELECT DefaultLang, AvailableLangs, MaxInputLoop FROM capstoneStore.Settings;");
        Task<MySqlDataReader> settingsLoad = ExecQueryAsync();
        settingsLoad.Wait();
        if (settingsLoad.Result == null) throw new FileLoadException("Failed to load settings");
        MySqlDataReader settings = settingsLoad.Result;
        settings.Read();
        Lang = settings.GetString("DefaultLang");
        MaxInputLoop = settings.GetInt32("MaxInputLoop");
        AvailableLangs = settings.GetString("AvailableLangs").Split(',').ToList();
        settings.CloseAsync();
    }
}