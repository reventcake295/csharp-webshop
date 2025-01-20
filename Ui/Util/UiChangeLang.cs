namespace Store.Ui.Util;

internal class UiChangeLang : UiItem
{
    internal UiChangeLang()
    {
        NameId = "Menu_ChangeLang_option";
    }
    internal override void Execute()
    {
        Console.WriteLine(Lang.GetLangGroupString("changeLang", Lang.StringType.Header));
        int key = 0;
        Dictionary<int, string> availableLangs = new();
        Console.WriteLine();
        foreach (string availableLang in Settings.AvailableLangs)
        {
            UiHelper.DisplayOption((++key).ToString(), availableLang);
            availableLangs.Add(key, availableLang);
        }
        Console.WriteLine();
        if (!UiHelper.AskQuestion("changeLang", out int question, 0, availableLangs.Keys.ToList())) return;

        if (!Lang.ChangeLang(availableLangs[question]))
        {
            Console.WriteLine(Lang.GetLangGroupString("changeLang", Lang.StringType.ResultFailure));
        }
        Console.WriteLine(Lang.GetLangGroupString("changeLang", Lang.StringType.ResultSuccess));
        
    }
}