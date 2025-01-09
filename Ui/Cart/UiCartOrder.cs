namespace Store.Ui.Cart;

internal class UiCartOrder : UiItem
{
    internal UiCartOrder()
    {
        NameId = "Menu_CartOrder_option";
    }
    internal override void Execute()
    {
        throw new NotImplementedException();
    }

    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Customer;
    }
}