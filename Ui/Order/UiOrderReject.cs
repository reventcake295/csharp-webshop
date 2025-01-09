namespace Store.Ui.Order;

internal class UiOrderReject : UiItem
{
    internal UiOrderReject()
    {
        NameId = "Menu_OrderReject_option";
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}