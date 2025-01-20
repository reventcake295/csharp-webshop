namespace Store.Ui.Cart;

internal class UiCartProdUpdate : UiItem
{
    internal UiCartProdUpdate()
    {
        NameId = "Menu_CartProdUpdate_option";
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
        if (!UiHelper.AskQuestion("updateCartProd_count", out int productCount, 0, min: 1)) return;
        // temporarily remove the product from the shopping cart; this ensures that the total price remains consistent
        ShoppingCart.GetInstance().RemoveProduct(StateHolder.CurrentOrderProduct);
        // update the product count
        StateHolder.CurrentOrderProduct.Count = productCount;
        // re-add the OrderProduct back to the shopping cart
        ShoppingCart.GetInstance().AddProduct(StateHolder.CurrentOrderProduct);
        
        // do not update the shopping cart because the count and prices and such are dynamically built upon menu display
    }
    
    protected override bool Accessible() => Session.PermissionRank == Perm.Customer;
}