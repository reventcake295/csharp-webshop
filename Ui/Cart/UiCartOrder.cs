namespace Store.Ui.Cart;

internal class UiCartOrder : UiItem
{
    internal UiCartOrder()
    {
        NameId = "Menu_CartOrder_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (!ShoppingCart.Instance.CreateOrder(out _))
        {
            Console.WriteLine(Lang.GetLangString("cartOrder_error"));
            return;
        }
        Console.WriteLine(Lang.GetLangString("cartOrder_success"));
        StateHolder.MenuBack = true;
        // empty the shopping cart subMenu so that when the user tries to access it again,
        // it is not displaying old information
        StateHolder.UpdateShoppingCart?.Invoke();
        StateHolder.UpdateOrders?.Invoke();
    }

    protected override bool Accessible() => Session.PermissionRank == Perm.Customer;
}