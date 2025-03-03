using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common.Model;

namespace Store.Cli.UiItems.Catalog;

internal class UiCatalogProd : UiMenu
{
    private readonly ProductModel _product;
    
    internal UiCatalogProd(ProductModel product)
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
        UiState.CurrentProduct = _product;
        Console.WriteLine(LangHandler.GetLangString("catProd_info_header"));
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_name")}: {_product.Name}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_priceExcl")}: {_product.Money.FormatPrice(_product.Price)}");
        Console.WriteLine($"{_product.Tax.Name}: {_product.Money.FormatPrice(_product.Tax.CalculateTax(_product.Price))}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_priceTotal")}: {_product.Money.FormatPrice(_product.Tax.CalculateTotal(_product.Price))}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_description")}:");
        Console.WriteLine($"{_product.Description}");
        DisplayMenu();
    }
    
    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        UiHelper.DisplayCustomOption(key, $"{_product.Name} {_product.Money.FormatPrice(_product.Price)}");
        return true;
    }
}