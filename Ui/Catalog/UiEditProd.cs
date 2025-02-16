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
        foreach (KeyValuePair<int,Money> money in Money.Instance.GetDictionary())
            Console.WriteLine($"    {money.Key}: {money.Value.Currency}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_money", out int moneyId, currentProduct.MoneyType.Id, 
                                  optional:true, choices:Money.Instance.GetIds())) return;
        Money moneyType = Money.Instance.Get(moneyId);
        
        // ask the price of the product
        if (!UiHelper.AskQuestion("prod_price", out decimal productPrice, currentProduct.ProductPrice, optional:true)) return;
        
        // ask the type of tax that is to be used after displaying the options
        Console.WriteLine(Lang.GetLangString("prod_TaxType"));
        Console.WriteLine();
        foreach (KeyValuePair<int,Taxes> tax in Taxes.Instance.GetDictionary())
            Console.WriteLine($"    {tax.Key}: {tax.Value.TaxName}");
        Console.WriteLine();
        if (!UiHelper.AskQuestion("prod_taxes", out int taxesId, currentProduct.Taxes.Id, 
                                  optional:true, choices:Taxes.Instance.GetIds())) return;
        Taxes taxes = Taxes.Instance.Get(taxesId);
        
        // ensure that at least one field is to be updated
        if (currentProduct.ProductName == productName && currentProduct.ProductDescription == productDescription &&
            currentProduct.ProductPrice == productPrice && currentProduct.Taxes.Id == taxesId &&
            currentProduct.MoneyType.Id == moneyId)
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
    
    protected override bool Accessible() => Session.PermissionRank == Perm.Admin;
}