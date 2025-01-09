namespace Store.Ui.Catalog;

internal class UiEditProd : UiItem
{
    internal UiEditProd()
    {
        NameId = "Menu_EditProd_option";
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