using Store.Ui.Util;

namespace Store.Ui.Order;

internal class UiOrder : UiItem
{
    internal UiOrder()
    {
        NameId = "Menu_Order_option";
        SubMenu.Add("1", new UiOrderAccept());
        SubMenu.Add("2", new UiOrderReject());
        SubMenu.Add("3", new UiBack());
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    internal override bool DisplayItem(string key)
    {
        throw new NotImplementedException();
    }
    
    protected override bool Accessible()
    {
        return Session.PermissionRank is Perm.Admin or Perm.Customer;
    }

    protected override bool Accessible(out Perm key)
    {
        throw new NotImplementedException();
    }
}