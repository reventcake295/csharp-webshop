using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Cart;

internal class UiCart : UiMenu
{
    /// <summary>
    /// The items that are always present in the menu but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new()
    {
        {1, new UiCartOrder()},
        {2, new UiBack()}
    };
    internal UiCart() => NameId = "Menu_Cart_option";
    
    internal override void Execute()
    {
        UiState.UpdateShoppingCart = LoadCart;
        SubMenu.Clear();
        if (SubMenu.Count == 0 || _fixedItems.Count == SubMenu.Count)
            LoadCart();

        if (SubMenu.Count == _fixedItems.Count)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("cartMenu", LangHandler.StringType.ResultNoMatch));
            return;
        }
        DisplayMenu("cartMenu");
    }

    // we override the menu here
    // because there is a special header that is attached to the menu that would otherwise not be present after the first load
    internal override void DisplayMenu(string menuName = "default")
    {
        OrderFactory orderFactory = App.GetScopedService<IUserSession>().ShoppingCart;
        Console.WriteLine(LangHandler.GetLangString("cartMenu_head"));
        Console.WriteLine($"{LangHandler.GetLangString("cartMenu_total")}: {orderFactory.MoneyModel?.FormatPrice(orderFactory.Total)}");
        base.DisplayMenu(menuName);
    }

    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Customer 
                                            && App.GetScopedService<IUserSession>().ShoppingCart.HasItems();
    
    private void LoadCart()
    {
        SubMenu.Clear();
        Dictionary<int, (ProductModel product, int quantity)> products = App.GetScopedService<IUserSession>().ShoppingCart.GetProducts();
        int key = 0;
        
        foreach (KeyValuePair<int, (ProductModel product, int quantity)> product in products)
            SubMenu.Add((++key).ToString(), new UiCartProd(product.Value.product, product.Value.quantity));
        
        // add the fixed items at the end
        foreach (KeyValuePair<int, UiItem> fixedItem in _fixedItems)
            SubMenu.Add((fixedItem.Key + key).ToString(), fixedItem.Value);
    }
}