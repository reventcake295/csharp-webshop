namespace Store.Ui.Cart;

internal class UiCartRem : UiItem
{
    internal UiCartRem()
    {
        NameId = "Menu_CartRem_option";
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