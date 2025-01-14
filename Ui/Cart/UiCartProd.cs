using Store.Ui.Util;

namespace Store.Ui.Cart;

internal class UiCartProd : UiItem
{
    private readonly OrderProduct _orderProduct;
    internal UiCartProd(OrderProduct orderProduct)
    {
        _orderProduct = orderProduct;
        NameId = "Menu_cartProd_option";
        SubMenu.Add("1", new UiCartRem());
        SubMenu.Add("2", new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        if (!Accessible()) return false;
        UiHelper.DisplayCustomOption(key, $"{_orderProduct.ProductName} x{_orderProduct.Count} {_orderProduct.Count * _orderProduct.ProductPrice}");
        return true;
    }
}