using Store.Ui.Util;

namespace Store.Ui.Cart;

internal class UiCart : UiItem
{
    /// <summary>
    /// The items that are always present in the menu but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();
    
    internal UiCart()
    {
        NameId = "Menu_Cart_option";
        _fixedItems.Add(1, new UiCartOrder());
        _fixedItems.Add(2, new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

//    internal override void DisplayMenu()
//    {
//        throw new NotImplementedException();
//    }

    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Customer;
    }
}