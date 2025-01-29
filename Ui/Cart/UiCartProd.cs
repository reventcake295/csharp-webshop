using Store.Ui.Util;

namespace Store.Ui.Cart;

internal class UiCartProd : UiMenu
{
    private readonly OrderProduct _orderProduct;
    internal UiCartProd(OrderProduct orderProduct)
    {
        _orderProduct = orderProduct;
        NameId = "Menu_cartProd_option";
        SubMenu.Add("1", new UiCartProdUpdate());
        SubMenu.Add("2", new UiCartRem());
        SubMenu.Add("3", new UiBack());
    }
    internal override void Execute()
    {
        StateHolder.CurrentOrderProduct = _orderProduct;
        
        // separate the total price calc due to the lenght some lines would be otherwise
        decimal totalPrice = _orderProduct.Taxes.CalculateTotal(_orderProduct.ProductPrice);
        
        Console.WriteLine(Lang.GetLangString("catProd_info_header"));
        Console.WriteLine($"{Lang.GetLangString("catProd_info_name")}: {_orderProduct.Product.ProductName}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_priceExcl")}: {_orderProduct.Money.FormatPrice(_orderProduct.ProductPrice)}");
        Console.WriteLine($"{_orderProduct.Taxes.TaxName}: {_orderProduct.Money.FormatPrice(_orderProduct.Taxes.CalculateTax(_orderProduct.ProductPrice))}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_priceTotal")}: {_orderProduct.Money.FormatPrice(totalPrice)}");
        Console.WriteLine($"{Lang.GetLangString("cartProd_info_count")}: {_orderProduct.Count}x");
        Console.WriteLine($"{Lang.GetLangString("cartProd_info_total")}: {_orderProduct.Money.FormatPrice(_orderProduct.PriceTotal)}");
        Console.WriteLine($"{Lang.GetLangString("catProd_info_description")}:");
        Console.WriteLine($"{_orderProduct.Product.ProductDescription}");
        
        DisplayMenu();
    }

    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        if (!Accessible()) return false;
        UiHelper.DisplayCustomOption(key, $"{_orderProduct.Product.ProductName} x{_orderProduct.Count} {_orderProduct.Money.FormatPrice(_orderProduct.PriceTotal)}");
        return true;
    }
}