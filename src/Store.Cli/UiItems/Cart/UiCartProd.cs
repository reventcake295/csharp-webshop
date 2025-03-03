using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common.Model;

namespace Store.Cli.UiItems.Cart;

internal class UiCartProd : UiMenu
{
    private readonly ProductModel _product;
    private readonly int _quantity;
    
    internal UiCartProd(ProductModel product, int count)
    {
        _product = product;
        _quantity = count;
        NameId = "Menu_cartProd_option";
        SubMenu.Add("1", new UiCartProdUpdate());
        SubMenu.Add("2", new UiCartRem());
        SubMenu.Add("3", new UiBack());
    }
    internal override void Execute()
    {
        UiState.CurrentProduct = _product;
        
        // separate the total price calc due to the lenght some lines would be otherwise
        decimal totalPrice = _product.Tax.CalculateTotal(_product.Price);
        
        Console.WriteLine(LangHandler.GetLangString("catProd_info_header"));
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_name")}: {_product.Name}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_priceExcl")}: {_product.Money.FormatPrice(_product.Price)}");
        Console.WriteLine($"{_product.Tax.Name}: {_product.Money.FormatPrice(_product.Tax.CalculateTax(_product.Price))}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_priceTotal")}: {_product.Money.FormatPrice(totalPrice)}");
        Console.WriteLine($"{LangHandler.GetLangString("cartProd_info_count")}: {_quantity}x");
        Console.WriteLine($"{LangHandler.GetLangString("cartProd_info_total")}: {_product.Money.FormatPrice(_product.Price * _quantity)}");
        Console.WriteLine($"{LangHandler.GetLangString("catProd_info_description")}:");
        Console.WriteLine($"{_product.Description}");
        
        DisplayMenu();
    }

    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        UiHelper.DisplayCustomOption(key, $"{_product.Name} x{_quantity} {_product.Money.FormatPrice(_product.Price * _quantity)}");
        return true;
    }
}