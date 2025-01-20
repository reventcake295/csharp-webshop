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
        StateHolder.UpdateShoppingCart = LoadCart;
        SubMenu.Clear();
        if (SubMenu.Count == 0 || _fixedItems.Count == SubMenu.Count)
            LoadCart();

        if (SubMenu.Count == _fixedItems.Count)
        {
            Console.WriteLine(Lang.GetLangGroupString("cartMenu", Lang.StringType.ResultNoMatch));
            return;
        }
        DisplayMenu("cartMenu");
    }

    // we override the menu here
    // because there is a special header that is attached to the menu that would otherwise not be present after the first load
    internal override void DisplayMenu(string menuName = "default")
    {
        Console.WriteLine(Lang.GetLangString("cartMenu_head"));
        Console.WriteLine($"{Lang.GetLangString("cartMenu_total")}: {ShoppingCart.GetInstance().DisplayFormat.FormatPrice(ShoppingCart.GetInstance().TotalPrice)}");
        base.DisplayMenu(menuName);
    }

    protected override bool Accessible() => Session.PermissionRank == Perm.Customer 
                                            && ShoppingCart.GetInstance().HasItems();
    
    private void LoadCart()
    {
        SubMenu.Clear();
        List<OrderProduct> products = ShoppingCart.GetInstance().GetList();
        for (int i = 0; i < products.Count; i++)
            SubMenu.Add((i + 1).ToString(), new UiCartProd(products[i]));
        
        // add the fixed items at the end
        foreach (KeyValuePair<int, UiItem> fixedItem in _fixedItems)
            SubMenu.Add((fixedItem.Key + products.Count).ToString(), fixedItem.Value);
    }
}