using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Cart;

internal class UiCartOrder : UiItem
{
    internal UiCartOrder() => NameId = "Menu_CartOrder_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        Task<OrderModel> orderModel = App.GetScopedService<IUserSession>().ShoppingCart.GenerateOrder();
        if (!orderModel.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangString("cartOrder_error"));
            return;
        }
        Console.WriteLine(LangHandler.GetLangString("cartOrder_success"));
        UiState.MenuBack = true;
        // empty the shopping cart subMenu so that when the user tries to access it again,
        // it is not displaying old information
        UiState.UpdateShoppingCart?.Invoke();
        UiState.UpdateOrders?.Invoke();
    }

    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Customer;
}