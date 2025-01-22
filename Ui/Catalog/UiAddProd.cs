namespace Store.Ui.Catalog;

internal class UiAddProd : UiItem
{
    internal UiAddProd()
    {
        NameId = "Menu_AddProd_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        Console.WriteLine(Lang.GetLangGroupString("prodAdd", Lang.StringType.Header));
        // ask the name and description of the product
        if (!UiHelper.AskQuestion("prod_name", out string productName, "")) return;
        if (!UiHelper.AskQuestion("prod_decs", out string productDescription, "")) return;
        
        // ask the money type to be used after displaying the options
        Console.WriteLine(Lang.GetLangString("prod_MoneyType"));
        Console.WriteLine();
        foreach (KeyValuePair<int,Money> money in Money.MoneyTypes)
            Console.WriteLine($"    {money.Key}: {money.Value.Currency}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_money", out int moneyId, Settings.DefaultMoney.Id, 
                                  optional:true, choices:Money.MoneyTypes.Keys.ToList())) return;
        Money moneyType = Money.MoneyTypes[moneyId];
        
        // ask the price of the product
        if (!UiHelper.AskQuestion("prod_price", out decimal productPrice, 0)) return;
        
        // ask the type of tax that is to be used after displaying the options
        Console.WriteLine(Lang.GetLangString("prod_TaxType"));
        Console.WriteLine();
        foreach (KeyValuePair<int,Taxes> tax in Taxes.TaxTypes)
            Console.WriteLine($"    {tax.Key}: {tax.Value.TaxName}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_taxes", out int taxesId, Settings.DefaultTaxes.Id, 
                                  optional:true, choices:Taxes.TaxTypes.Keys.ToList())) return;
        Taxes taxes = Taxes.TaxTypes[taxesId];
        
        if (!Products.Instance.AddProduct(productName, productDescription, moneyType, productPrice, taxes))
        {
            Console.WriteLine(Lang.GetLangGroupString("prodAdd", Lang.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(Lang.GetLangGroupString("prodAdd", Lang.StringType.ResultSuccess));
        
        // update the product catalog
        StateHolder.UpdateCatalog?.Invoke();
    }

    protected override bool Accessible() => Session.PermissionRank == Perm.Admin;
    
}