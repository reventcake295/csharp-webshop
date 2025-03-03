using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Catalog;

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
        if (UiState.CurrentProduct == null)
        {
            Console.WriteLine(LangHandler.GetLangString("cartProd_noProductSelected"));
            return;
        }

        OrderFactory orderFactory = App.GetScopedService<IUserSession>().ShoppingCart;
        
        bool prodInCart = orderFactory.HasProduct(UiState.CurrentProduct.Id);
        if (prodInCart) Console.WriteLine(LangHandler.GetLangString("cartProd_alreadyExist"));
        
        // ask the user how much of the product needs to be added to the cart, with at least one
        if (!UiHelper.AskQuestion("addCartProd_count", out int productCount, 0, min: 1)) return;

        if (prodInCart)
        {
            orderFactory.AddProduct(UiState.CurrentProduct, productCount);
            Console.WriteLine(LangHandler.GetLangString("cartProd_addedToExisting"));
            return;
        }
        
        // Add the product as an OrderProduct to the shopping cart
        orderFactory.AddProduct(UiState.CurrentProduct, productCount);
        Console.WriteLine(LangHandler.GetLangString("cartProd_addedToCart"));
        // update the shopping cart if necessary
        // to ensure that when the user visits it again, the new product is visible
        UiState.UpdateShoppingCart?.Invoke();
    }
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Customer;
}