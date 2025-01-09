namespace Store.Ui.Catalog;

internal class UiAddProd : UiItem
{
    internal UiAddProd()
    {
        NameId = "Menu_AddProd_option";
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