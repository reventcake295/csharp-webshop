using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;

namespace Store.Cli.UiItems.Catalog;

internal class UiRemProd : UiItem
{
    internal UiRemProd() => NameId = "menu_RemProd_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (UiState.CurrentProduct == null)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("prodRem", LangHandler.StringType.ResultNoMatch));  
            return;
        }
        
        if (!UiHelper.AskQuestion("prodRem", out bool actionable, false, [true, false])) return;

        if (!actionable)
        {
            Console.WriteLine(LangHandler.GetLangString("prodRem_cancel"));
            return;
        }
        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.Product.Remove(UiState.CurrentProduct);
        Task result = dataWorker.FinishSessionAsync();
        result.Wait();
        if (!result.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("prodRem", LangHandler.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(LangHandler.GetLangGroupString("prodRem", LangHandler.StringType.ResultSuccess));
        
        UiState.CurrentProduct = null;
        UiState.MenuBack = true;
        UiState.UpdateCatalog?.Invoke();
    }
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
}