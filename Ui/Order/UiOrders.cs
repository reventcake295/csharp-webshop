using Store.Ui.Util;

namespace Store.Ui.Order;

internal class UiOrders : UiItem
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
        StateHolder.UpdateOrders = LoadMenu;

        // ensure that the menu is reloaded if either the menu type or the userId has changed,
        // although if the user just switches between Admin accounts, it does not matter,
        // but for Customer accounts it does.
        // if the user comes back to the menu and it is still the same user,
        // then we need to ensure that the menu is correctly loaded if needed 
        if (_currentUserType != menuType || _currentUserId != Session.Id) LoadMenu();
        else if (SubMenu.Count == 0 || SubMenu.Count == _fixedItems.Count) LoadMenu();
        
        // if the menu is still empty beyond the fixed Items then return the previous menu
        // because there is nothing to do here 
        if (SubMenu.Count == _fixedItems.Count) {
            Console.WriteLine(Lang.GetLangGroupString($"ordersMenu_{menuType.ToString()}", Lang.StringType.ResultNoMatch));
            return;
        }
        
        // assign the current menuType and userId internally;
        // for when the user comes back to this menu, the menu can be reloaded if need be
        _currentUserType = menuType;
        _currentUserId = Session.Id;
        DisplayMenu($"ordersMenu_{menuType.ToString()}");
    }

    private void LoadMenu()
    {
        if (!Accessible(out Perm menuType)) return;
        if (menuType == Perm.None) return;
        SubMenu.Clear();
        // my IDE says that this switch does not have the Perm.None arm, but that option never hits the switch case,
        // and there is a throwaway arm present, so it's a moot point
        IEnumerable<Store.Order> orders = menuType switch
        {
            Perm.Admin    => Orders.Instance.GetIncomingOrders(),
            Perm.Customer => Orders.Instance.GetUserOrders(Session.Id),
            _             => []
        };
        int key = 0;
        foreach (Store.Order order in orders)
            SubMenu.Add((++key).ToString(), new UiOrder(order));

        foreach (KeyValuePair<int,UiItem> fixedItem in _fixedItems)
            SubMenu.Add((key + fixedItem.Key).ToString(), fixedItem.Value);
    }
    
    protected override bool Accessible() => Session.PermissionRank is Perm.Admin or Perm.Customer 
                                            && Orders.Instance.HasOrders(Session.Id);

    protected override bool Accessible(out Perm key)
    {
        key = Session.PermissionRank;
        return Session.PermissionRank is Perm.Admin or Perm.Customer;
    }
}