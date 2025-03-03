using Store.Cli.Lang;
using Store.Common;

namespace Store.Cli.UiItems.Util;

internal class UiChangeLang : UiItem
{
    internal UiChangeLang() => NameId = "Menu_ChangeLang_option";
    
    internal override void Execute()
    {
        Console.WriteLine(LangHandler.GetLangGroupString("changeLang", LangHandler.StringType.Header));
        int key = 0;
        Dictionary<int, string> availableLangs = new();
        Console.WriteLine();
        foreach (string availableLang in App.GetSettings().Languages)
        {
            UiHelper.DisplayOption((++key).ToString(), availableLang);
            availableLangs.Add(key, availableLang);
        }
        Console.WriteLine();
        if (!UiHelper.AskQuestion("changeLang", out int question, 0, availableLangs.Keys.ToList())) return;

        if (!LangHandler.ChangeLang(availableLangs[question]))
        {
            Console.WriteLine(LangHandler.GetLangGroupString("changeLang", LangHandler.StringType.ResultFailure));
        }
        Console.WriteLine(LangHandler.GetLangGroupString("changeLang", LangHandler.StringType.ResultSuccess));
        
    }
}