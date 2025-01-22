using Store.Ui.Util;

namespace Store.Ui.Catalog;

internal class UiCatalog : UiItem
{
    /// <summary>
    /// The items that are always present in the menu,
    /// but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();
    internal UiCatalog()
    {
        NameId = "Menu_Catalog_option";
        _fixedItems.Add(1, new UiBack());
        _fixedItems.Add(2, new UiAddProd());
    }
    
    internal override void Execute()
    {
        // set the method of the current class instance as the Action to execute when the user's menu is to be reloaded
        StateHolder.UpdateCatalog = LoadProducts;
        // see if the user's list needs to be updated or generated
        if (SubMenu.Count == 0 || SubMenu.Count == _fixedItems.Count)
            LoadProducts();
        // trigger the display menu function as normal
        if (SubMenu.Count == _fixedItems.Count)
        {
            // if th user is no admin, then there is no use coming here currently, so we return to the previous menu
            if (Session.PermissionRank != Perm.Admin)
            {
                Console.WriteLine(Lang.GetLangString("catalogMenu_noProducts"));
                return;
            }
            Console.WriteLine(Lang.GetLangString("catalogMenu_noProducts_admin"));
        }
        DisplayMenu("catalogMenu");
    }

    private void LoadProducts()
    {
        // clear the current items list and gather the product's list anew
        SubMenu.Clear();
        List<Product> products = Products.Instance.GetAllProducts();
        // add the products to the SubMenu list
        for (int i = 0; i < products.Count; i++)
            SubMenu.Add((i + 1).ToString(), new UiCatalogProd(products[i]));
        
        // add the fixed items at the end
        foreach (KeyValuePair<int, UiItem> fixedItem in _fixedItems)
            SubMenu.Add((fixedItem.Key + products.Count).ToString(), fixedItem.Value);
    }
}