namespace Store.Ui.Order;

internal class UiOrderAccept : UiItem
{
    internal UiOrderAccept()
    {
        NameId = "Menu_OrderAccept_option";
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