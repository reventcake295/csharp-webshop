namespace Store.Ui;

internal abstract class UiMenu : UiItem
{
    protected readonly Dictionary<string, UiItem> SubMenu = new();
    
    internal virtual void DisplayMenu(string menuId = "default")
    {
        while (!StateHolder.MenuBack && !Ui.ShuttingDown)
        {
            // place this here to ensure that the menu remains accessible between menu displays;
            // this is because it is liable to change for certain menu's (Cart, Orders)
            if (!Accessible())
            {
                Console.WriteLine(Lang.GetLangGroupString(menuId, Lang.StringType.ResultNoMatch));
                break;
            }
            UiHelper.DisplayMenu(SubMenu, menuId);
        }
        StateHolder.MenuBack = false;
    }
}