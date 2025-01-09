using Store.Ui.Util;

namespace Store.Ui.Catalog;

internal class UiCatalog : UiItem
{
    /// <summary>
    /// The items that are always present in the menu but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();
    internal UiCatalog()
    {
        NameId = "Menu_Catalog_option";
        _fixedItems.Add(1, new UiAddProd());
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
}