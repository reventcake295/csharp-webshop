using Store.Ui.Util;

namespace Store.Ui.Order;

internal class UiOrders : UiItem
{
    /// <summary>
    /// The items that are always present in the menu but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();
    
    internal UiOrders()
    {
        NameId = "Menu_Orders_option";
        _fixedItems.Add(1, new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

//    internal override void DisplayMenu()
//    {
//        throw new NotImplementedException();
//    }

    // why did I write this here again?
//    internal override void DisplayItem(string key)
//    {
//        throw new NotImplementedException();
//    }
    
    protected override bool Accessible()
    {
        return Session.PermissionRank is Perm.Admin or Perm.Customer;
    }

    protected override bool Accessible(out Perm key)
    {
        throw new NotImplementedException();
    }
}