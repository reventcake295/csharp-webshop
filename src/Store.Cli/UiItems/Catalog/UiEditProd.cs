using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Catalog;

internal class UiEditProd : UiItem
{
    internal UiEditProd()
    {
        NameId = "Menu_EditProd_option";
    }
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (UiState.CurrentProduct == null)
        {
            Console.WriteLine(LangHandler.GetLangString("prodEdit_noProductSelected"));
            return;
        }
        ProductModel currentProduct = UiState.CurrentProduct;
        bool productUpdated = false;
        
        Console.WriteLine(LangHandler.GetLangGroupString("prodEdit", LangHandler.StringType.Header));
        // ask the name and description of the product
        if (!UiHelper.AskQuestion("prod_name", out string productName, currentProduct.Name, optional:true)) return;
        if (productName != currentProduct.Name)
        {
            currentProduct.Name = productName;
            productUpdated = true;
        }
        
        if (!UiHelper.AskQuestion("prod_decs", out string productDescription, currentProduct.Description, optional:true)) return;
        if (productDescription != currentProduct.Description)
        {
            currentProduct.Description = productDescription;
            productUpdated = true;
        }
        
        // ask the money type to be used after displaying the options
        Console.WriteLine(LangHandler.GetLangString("prod_MoneyType"));
        Console.WriteLine();
        
        IEnumerable<MoneyModel> moneyModels = App.GetDataWorker().Money.GetAll().ToList();
        foreach (MoneyModel money in moneyModels)
            Console.WriteLine($"    {money.Id}: {money.Name}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_money", out int moneyId, currentProduct.Money.Id, 
                                  optional:true, choices:moneyModels.Select(m => m.Id).ToList())) return;
        if (moneyId != currentProduct.Money.Id)
        {
            currentProduct.Money = moneyModels.First(m => m.Id == moneyId);
            productUpdated = true;
        }
        
        // ask the price of the product
        if (!UiHelper.AskQuestion("prod_price", out decimal productPrice, currentProduct.Price, optional:true)) return;
        if (productPrice != currentProduct.Price)
        {
            currentProduct.Price = productPrice;
            productUpdated = true;
        }
        
        // ask the type of tax that is to be used after displaying the options
        Console.WriteLine(LangHandler.GetLangString("prod_TaxType"));
        Console.WriteLine();
        IEnumerable<TaxModel> taxModels = App.GetDataWorker().Tax.GetAll().ToList();
        foreach (TaxModel tax in taxModels)
            Console.WriteLine($"    {tax.Id}: {tax.Name}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_taxes", out int taxesId, currentProduct.Tax.Id, 
                                  optional:true, choices:taxModels.Select(m => m.Id).ToList())) return;
        if (taxesId != currentProduct.Tax.Id)
        {
            currentProduct.Tax = taxModels.First(t => t.Id == taxesId);
            productUpdated = true;
        }
        
        // ensure that at least one field is to be updated
        if (!productUpdated)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("prodEdit", LangHandler.StringType.ResultNoMatch));
            return;
        }
        
        // to update the product with the new data,
        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.Product.Update(currentProduct);
        Task result = dataWorker.FinishSessionAsync();
        result.Wait();
        if (!result.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("prodEdit", LangHandler.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(LangHandler.GetLangGroupString("prodEdit", LangHandler.StringType.ResultSuccess));
        
        // do not update the product catalog because the current product is already updated
    }
    
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
}