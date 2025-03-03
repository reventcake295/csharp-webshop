using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.Order;

internal class UiOrders : UiMenu
{
    /// <summary>
    /// The items that are always present in the menu,
    /// but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();

    private Perm? _currentUserType;
    
    private int? _currentUserId;
    internal UiOrders()
    {
        NameId = "Menu_Orders_option";
        _fixedItems.Add(1, new UiBack());
    }

    internal override void Execute()
    {
        if (!Accessible(out Perm menuType)) return;
        if (menuType == Perm.None) return;
        UiState.UpdateOrders = LoadMenu;

        // ensure that the menu is reloaded if either the menu type or the userId has changed,
        // although if the user just switches between Admin accounts, it does not matter,
        // but for Customer accounts it does.
        // if the user comes back to the menu and it is still the same user,
        // then we need to ensure that the menu is correctly loaded if needed 
        if (_currentUserType != menuType || _currentUserId != App.GetScopedService<IUserSession>().UserModel?.Id) LoadMenu();
        else if (SubMenu.Count == 0 || SubMenu.Count == _fixedItems.Count) LoadMenu();
        
        // if the menu is still empty beyond the fixed Items then return the previous menu
        // because there is nothing to do here 
        if (SubMenu.Count == _fixedItems.Count) {
            Console.WriteLine(LangHandler.GetLangGroupString($"ordersMenu_{menuType.ToString()}", LangHandler.StringType.ResultNoMatch));
            return;
        }
        
        // assign the current menuType and userId internally;
        // for when the user comes back to this menu, the menu can be reloaded if need be
        _currentUserType = menuType;
        _currentUserId = App.GetScopedService<IUserSession>().UserModel?.Id;
        DisplayMenu($"ordersMenu_{menuType.ToString()}");
    }

    private void LoadMenu()
    {
        if (!Accessible(out Perm menuType)) return;
        if (menuType == Perm.None) return;
        SubMenu.Clear();
        
        // my IDE says that this switch does not have the Perm.None arm, but that option never hits the switch case,
        // and there is a throwaway arm present, so it's a moot point
        
        IEnumerable<OrderModel> orders = menuType switch
        {
            Perm.Admin    => OrderModel.GetOrders(OrderStatus.Incoming),
            Perm.Customer => OrderModel.GetOrders(App.GetScopedService<IUserSession>().UserModel.Id),
            _             => []
        };
        int key = 0;
        foreach (OrderModel order in orders)
            SubMenu.Add((++key).ToString(), new UiOrder(order));

        foreach (KeyValuePair<int,UiItem> fixedItem in _fixedItems)
            SubMenu.Add((key + fixedItem.Key).ToString(), fixedItem.Value);
    }
    
    protected override bool Accessible()
    {
        Perm perm = App.GetScopedService<IUserSession>().Permission;
        // disable this if statement and add a throwaway case in the switch statement that catches what the if statement checks and more
//        if (perm is not (Perm.Admin or Perm.Customer)) return false;
        return perm switch
        {
            Perm.Customer => OrderModel.GetOrders(App.GetScopedService<IUserSession>().UserModel.Id).Any(),
            Perm.Admin    => OrderModel.GetOrders(OrderStatus.Incoming).Any(),
            _             => false
        };
    }

    protected override bool Accessible(out Perm key)
    {
        key = App.GetScopedService<IUserSession>().Permission;
        return App.GetScopedService<IUserSession>().Permission is Perm.Admin or Perm.Customer;
    }
}