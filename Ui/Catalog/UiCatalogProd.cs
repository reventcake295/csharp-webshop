using Store.Ui.Cart;
using Store.Ui.Util;

namespace Store.Ui.Catalog;

internal class UiCatalogProd : UiItem
{
    internal UiCatalogProd()
    {
        NameId = "Menu_catalogProd_option";
        SubMenu.Add("1", new UiAddCartProd());
        SubMenu.Add("2", new UiEditProd());
        SubMenu.Add("3", new UiRemProd());
        SubMenu.Add("4", new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    internal override bool DisplayItem(string key)
    {
        throw new NotImplementedException();
    }
}