using Store.Ui.Util;

namespace Store.Ui.Order;

internal class UiOrder : UiItem
{
    private readonly Store.Order _order;
    
    internal UiOrder(Store.Order order)
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
        StateHolder.CurrentOrder = _order;
        if (_order.CustomerId != 0)
        {
            Store.User orderUser = Users.GetInstance().GetUserById(_order.CustomerId);
            Console.WriteLine($"{Lang.GetLangString("orderDis_user")}: {orderUser.Username}");
        }
        else Console.WriteLine($"{Lang.GetLangString("orderDis_noUser")}");
        
        Console.WriteLine($"{Lang.GetLangString("orderDis_total")}: {_order.MoneyType.FormatPrice(_order.OrderTotal)}");
        Console.WriteLine($"{Lang.GetLangString("orderDis_date")}: {_order.OrderDate}");
        Console.WriteLine($"{Lang.GetLangString("orderDis_products")}:");
        foreach (OrderProduct product in _order.Products)
            Console.WriteLine($"{product.Product.ProductName}: {product.Money.FormatPrice(product.ProductPrice)} x{product.Count} = {product.Money.FormatPrice(product.ProductPrice*product.Count)}");
        
        // If the user is a customer, then they have nothing to do here, so we return to the previous menu.
        // We also do not have to give an explanation because there is no case where a Customer can do anything here
        if (Session.PermissionRank == Perm.Customer) return;
        DisplayMenu("orderMenu");
    }

    internal override bool DisplayItem(string key)
    {
        if (!Accessible()) return false;
        
        UiHelper.DisplayCustomOption(key, $"{_order.Status}: {_order.OrderDate} {_order.MoneyType.FormatPrice(_order.OrderTotal)}");

        return true;
    }
    
    /// <summary>
    /// Checks to see if the current user has access to the UiItem.
    /// In case of a customer a further check if performed to see if the order has the same userId
    /// </summary>
    /// <returns></returns>
    protected override bool Accessible() => Session.PermissionRank switch
        {
            Perm.Admin    => true,
            Perm.Customer => _order.CustomerId == Session.Id,
            _             => false
        };
}