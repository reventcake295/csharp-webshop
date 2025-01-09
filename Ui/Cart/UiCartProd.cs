using Store.Ui.Util;

namespace Store.Ui.Cart;

internal class UiCartProd : UiItem
{
    internal UiCartProd()
    {
        NameId = "Menu_cartProd_option";
        SubMenu.Add("1", new UiCartRem());
        SubMenu.Add("2", new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    internal override bool DisplayItem(string key)
    {
        throw new NotImplementedException();
        return true;
    }
}