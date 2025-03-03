using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Catalog;

internal class UiAddProd : UiItem
{
    internal UiAddProd() => NameId = "Menu_AddProd_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        Console.WriteLine(LangHandler.GetLangGroupString("prodAdd", LangHandler.StringType.Header));
        // ask the name and description of the product
        if (!UiHelper.AskQuestion("prod_name", out string productName, "")) return;
        if (!UiHelper.AskQuestion("prod_decs", out string productDescription, "")) return;
        
        // ask the money type to be used after displaying the options
        Console.WriteLine(LangHandler.GetLangString("prod_MoneyType"));
        Console.WriteLine();
        IEnumerable<MoneyModel> moneyModels = App.GetDataWorker().Money.GetAll().ToList();
        foreach (MoneyModel money in moneyModels)
            Console.WriteLine($"    {money.Id}: {money.Name}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_money", out int moneyId, App.GetSettings().DefaultMoney.Id, 
                                  optional:true, choices:moneyModels.Select(m => m.Id).ToList())) return;
        
        // ask the price of the product
        if (!UiHelper.AskQuestion("prod_price", out decimal productPrice, 0)) return;
        
        // ask the type of tax that is to be used after displaying the options
        Console.WriteLine(LangHandler.GetLangString("prod_TaxType"));
        Console.WriteLine();
        IEnumerable<TaxModel> taxModels = App.GetDataWorker().Tax.GetAll().ToList();
        foreach (TaxModel tax in taxModels)
            Console.WriteLine($"    {tax.Id}: {tax.Name}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_taxes", out int taxesId, App.GetSettings().DefaultTax.Id, 
                                  optional:true, choices:taxModels.Select(m => m.Id).ToList())) return;
        
        ProductModel productModel = new(0, productName, productDescription, productPrice, taxesId, moneyId);
        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.Product.Add(productModel);
        Task result = dataWorker.FinishSessionAsync();
        result.Wait();
        if (!result.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("prodAdd", LangHandler.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(LangHandler.GetLangGroupString("prodAdd", LangHandler.StringType.ResultSuccess));
        
        // update the product catalog
        UiState.UpdateCatalog?.Invoke();
    }

    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
    
}