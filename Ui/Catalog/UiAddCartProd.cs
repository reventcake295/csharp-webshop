namespace Store.Ui.Catalog;

internal class UiAddCartProd : UiItem
{
    internal UiAddCartProd()
    {
        NameId = "Menu_AddCartProd_option";
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