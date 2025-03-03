using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Order;

internal class UiOrder : UiMenu
{
    private readonly OrderModel _order;
    
    internal UiOrder(OrderModel order)
    {
        _order = order;
        NameId = "Menu_Order_option";
        SubMenu.Add("1", new UiBack());
        SubMenu.Add("2", new UiOrderAccept());
        SubMenu.Add("3", new UiOrderReject());
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        UiState.CurrentOrder = _order;
        if (_order.Customer.Id != 0)
        {
            UserModel orderUser = _order.Customer;
            Console.WriteLine($"{LangHandler.GetLangString("orderDis_user")}: {orderUser.Username}");
        }
        else Console.WriteLine($"{LangHandler.GetLangString("orderDis_noUser")}");
        
        Console.WriteLine($"{LangHandler.GetLangString("orderDis_total")}: {_order.CalculateTotal()}");
        Console.WriteLine($"{LangHandler.GetLangString("orderDis_date")}: {_order.Date}");
        Console.WriteLine($"{LangHandler.GetLangString("orderDis_products")}:");
        foreach (OrderProductModel product in _order.OrderProducts)
            Console.WriteLine($"{product.Product.Name}: {product.Money.FormatPrice(product.PscPrice)} x{product.Quantity} = {product.Money.FormatPrice(product.Total)}");
        
        // If the user is a customer, then they have nothing to do here, so we return to the previous menu.
        // We also do not have to give an explanation because there is no case where a Customer can do anything here
        if (App.GetScopedService<IUserSession>().Permission == Perm.Customer) return;
        DisplayMenu("orderMenu");
    }

    internal override bool DisplayItem(string key)
    {
        if (!Accessible()) return false;
        
        UiHelper.DisplayCustomOption(key, $"{_order.OrderStatus}: {_order.Date} {_order.Total}");

        return true;
    }
    
    /// <summary>
    /// Checks to see if the current user has access to the UiItem.
    /// In the case of a customer, a further check if performed to see if the order has the same userId
    /// </summary>
    /// <returns></returns>
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission switch
        {
            Perm.Admin    => true,
            Perm.Customer => _order.Customer.Id == App.GetScopedService<IUserSession>().UserModel?.Id,
            _             => false
        };
}