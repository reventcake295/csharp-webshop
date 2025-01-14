using Store.Ui.Util;

namespace Store.Ui.Catalog;

internal class UiCatalogProd : UiItem
{
    private readonly Product _product;
    
    internal UiCatalogProd(Product product)
    {
        _product = product;
        NameId = "Menu_catalogProd_option";
        SubMenu.Add("1", new UiAddCartProd());
        SubMenu.Add("2", new UiEditProd());
        SubMenu.Add("3", new UiRemProd());
        SubMenu.Add("4", new UiBack());
    }
    internal override void Execute()
    {
        StateHolder.CurrentProduct = _product;
        Console.WriteLine(Lang.GetLangString("catProd_info_header"));
        Console.WriteLine($"{Lang.GetLangString("catProd_info_name")}: {_product.ProductName}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_priceExcl")}: {_product.MoneyType.FormatPrice(_product.ProductPrice)}");
        Console.WriteLine($"{_product.Taxes.TaxName}: {_product.MoneyType.FormatPrice(_product.Taxes.CalculateTax(_product.ProductPrice))}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_priceTotal")}: {_product.MoneyType.FormatPrice(_product.Taxes.CalculateTotal(_product.ProductPrice))}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_description")}:");
        Console.WriteLine($"{_product.ProductDescription}");
        DisplayMenu();
    }
    
    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        if (!Accessible()) return false;
        UiHelper.DisplayCustomOption(key, $"{_product.ProductName} {_product.MoneyType.FormatPrice(_product.ProductPrice)}");
        return true;
    }
}