namespace Store.Ui.Cart;

internal class UiCartRem : UiItem
{
    internal UiCartRem()
    {
        NameId = "Menu_CartRem_option";
    }
    internal override void Execute()
    {
        
        if (!Accessible()) return;
        // don't know how it is possible, but it is a required thing for later usage
        if (StateHolder.CurrentOrderProduct == null)
        {
            Console.WriteLine(Lang.GetLangString("cartProd_noProductSelected"));
            return;
        }
        // ask the user how much of the product needs to be added to the cart, with at least one
        if (!UiHelper.AskQuestion("remCartProd", out bool productRemove, false, choices:[false, true])) return;
        if (!productRemove)
        { // ensure that the user has accepted the removal of the product
            Console.WriteLine(Lang.GetLangGroupString("remCartProd", Lang.StringType.ResultFailure));
            return;
        }
        // Add the product as an OrderProduct to the shopping cart
        ShoppingCart.GetInstance().RemoveProduct(StateHolder.CurrentOrderProduct);
        // update the shopping cart if necessary
        // to ensure that when the user visits it again, the product is no longer visible
        StateHolder.UpdateShoppingCart?.Invoke();
        StateHolder.MenuBack = true;
        StateHolder.CurrentOrderProduct = null;
    }

    protected override bool Accessible() => Session.PermissionRank == Perm.Customer;
}