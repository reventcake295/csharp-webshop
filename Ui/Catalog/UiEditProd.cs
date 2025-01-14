namespace Store.Ui.Catalog;

internal class UiEditProd : UiItem
{
    internal UiEditProd()
    {
        NameId = "Menu_EditProd_option";
    }
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (StateHolder.CurrentProduct == null)
        {
            Console.WriteLine(Lang.GetLangString("prodEdit_noProductSelected"));
            return;
        }
        Product currentProduct = StateHolder.CurrentProduct;
        Console.WriteLine(Lang.GetLangGroupString("prodEdit", Lang.StringType.Header));
        // ask the name and description of the product
        if (!UiHelper.AskQuestion("prod_name", out string productName, currentProduct.ProductName, optional:true)) return;
        if (!UiHelper.AskQuestion("prod_decs", out string productDescription, currentProduct.ProductDescription, optional:true)) return;
        
        // ask the money type to be used after displaying the options
        Console.WriteLine(Lang.GetLangString("prod_MoneyType"));
        Console.WriteLine();
        foreach (KeyValuePair<int,Money> money in Money.MoneyTypes)
            Console.WriteLine($"    {money.Key}: {money.Value.Currency}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_money", out int moneyId, currentProduct.MoneyId, 
                                  optional:true, choices:Money.MoneyTypes.Keys.ToList())) return;
        Money moneyType = Money.MoneyTypes[moneyId];
        
        // ask the price of the product
        if (!UiHelper.AskQuestion("prod_price", out decimal productPrice, currentProduct.ProductPrice, optional:true)) return;
        
        // ask the type of tax that is to be used after displaying the options
        Console.WriteLine(Lang.GetLangString("prod_TaxType"));
        Console.WriteLine();
        foreach (KeyValuePair<int,Taxes> tax in Taxes.TaxTypes)
            Console.WriteLine($"    {tax.Key}: {tax.Value.TaxName}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_taxes", out int taxesId, currentProduct.TaxesId, 
                                  optional:true, choices:Taxes.TaxTypes.Keys.ToList())) return;
        Taxes taxes = Taxes.TaxTypes[taxesId];
        
        // ensure that at least one field is to be updated
        if (currentProduct.ProductName == productName && currentProduct.ProductDescription == productDescription &&
            currentProduct.ProductPrice == productPrice && currentProduct.TaxesId == taxesId &&
            currentProduct.MoneyId == moneyId)
        {
            Console.WriteLine(Lang.GetLangGroupString("prodEdit", Lang.StringType.ResultNoMatch));
            return;
        }
        // to update the product with the new data,
        // as we don't really know the actual field(s) that are to be updated we just do all fields
        if (!currentProduct.Edit(productName, productDescription, moneyType, productPrice, taxes))
        {
            Console.WriteLine(Lang.GetLangGroupString("prodEdit", Lang.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(Lang.GetLangGroupString("prodEdit", Lang.StringType.ResultSuccess));
        
        // do not update the product catalog because the current product is already updated
    }
    
    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}