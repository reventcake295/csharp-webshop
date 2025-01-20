namespace Store.Ui.Catalog;

internal class UiRemProd : UiItem
{
    internal UiRemProd()
    {
        NameId = "menu_RemProd_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (StateHolder.CurrentProduct == null)
        {
            Console.WriteLine(Lang.GetLangGroupString("prodRem", Lang.StringType.ResultNoMatch));  
            return;
        }
        
        if (!UiHelper.AskQuestion("prodRem", out bool actionable, false, [true, false])) return;

        if (!actionable)
        {
            Console.WriteLine(Lang.GetLangString("prodRem_cancel"));
            return;
        }

        if (!Products.GetInstance().RemoveProduct(StateHolder.CurrentProduct.ProductId))
        {
            Console.WriteLine(Lang.GetLangGroupString("prodRem", Lang.StringType.ResultFailure));
            return;
        }
        Console.WriteLine(Lang.GetLangGroupString("prodRem", Lang.StringType.ResultSuccess));
        
        StateHolder.CurrentProduct = null;
        StateHolder.MenuBack = true;
        StateHolder.UpdateCatalog?.Invoke();
    }
    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}