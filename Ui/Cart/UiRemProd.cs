namespace Store.Ui.Cart;

internal class UiRemProd : UiItem
{
    internal UiRemProd()
    {
        NameId = "menu_RemProd_option";
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