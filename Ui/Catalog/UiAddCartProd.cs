namespace Store.Ui.Catalog;

internal class UiAddCartProd : UiItem
{
    internal UiAddCartProd()
    {
        NameId = "Menu_AddCartProd_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        // don't know how it is possible, but it is a required thing for later usage
        if (StateHolder.CurrentProduct == null)
        {
            Console.WriteLine(Lang.GetLangString("cartProd_noProductSelected"));
            return;
        }

        bool prodInCart = ShoppingCart.GetInstance().HasItem(StateHolder.CurrentProduct);
        if (prodInCart) Console.WriteLine(Lang.GetLangString("cartProd_alreadyExist"));
        
        // ask the user how much of the product needs to be added to the cart, with at least one
        if (!UiHelper.AskQuestion("addCartProd_count", out int productCount, 0, min: 1)) return;

        if (prodInCart)
        {
            OrderProduct? oldProduct = ShoppingCart.GetInstance().GetList()
                                                        .Find(product => product.Product == StateHolder.CurrentProduct);
            if (oldProduct != null)
            {
                oldProduct.Count += productCount;
                Console.WriteLine(Lang.GetLangString("cartProd_addedToExisting"));
                return;
            }
        }
        // Add the product as an OrderProduct to the shopping cart
        ShoppingCart.GetInstance().AddProduct(StateHolder.CurrentProduct.CreateOrderProduct(productCount));
        Console.WriteLine(Lang.GetLangString("cartProd_addedToCart"));
        // update the shopping cart if necessary
        // to ensure that when the user visits it again, the new product is visible
        StateHolder.UpdateShoppingCart?.Invoke();
    }
    protected override bool Accessible() => Session.PermissionRank == Perm.Customer;
}