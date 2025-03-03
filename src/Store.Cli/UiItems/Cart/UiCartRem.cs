using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;

namespace Store.Cli.UiItems.Cart;

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
        if (UiState.CurrentProduct == null)
        {
            Console.WriteLine(LangHandler.GetLangString("cartProd_noProductSelected"));
            return;
        }
        // ask the user how much of the product needs to be added to the cart, with at least one
        if (!UiHelper.AskQuestion("remCartProd", out bool productRemove, false, choices:[false, true])) return;
        if (!productRemove)
        { // ensure that the user has accepted the removal of the product
            Console.WriteLine(LangHandler.GetLangGroupString("remCartProd", LangHandler.StringType.ResultFailure));
            return;
        }
        
        // Add the product as an OrderProduct to the shopping cart
        App.GetScopedService<IUserSession>().ShoppingCart.RemoveProduct(UiState.CurrentProduct.Id);
        // update the shopping cart if necessary
        // to ensure that when the user visits it again, the product is no longer visible
        UiState.UpdateShoppingCart?.Invoke();
        UiState.MenuBack = true;
        UiState.CurrentProduct = null;
    }

    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Customer;
}