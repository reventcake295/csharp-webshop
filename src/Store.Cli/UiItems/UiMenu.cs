using Store.Cli.Lang;

namespace Store.Cli.UiItems;

internal abstract class UiMenu : UiItem
{
    protected readonly Dictionary<string, UiItem> SubMenu = new();
    
    internal virtual void DisplayMenu(string menuId = "default")
    {
        while (!UiState.MenuBack && !Ui.ShuttingDown)
        {
            // place this here to ensure that the menu remains accessible between menu displays;
            // this is because it is liable to change for certain menu's (Cart, Orders)
            if (!Accessible())
            {
                Console.WriteLine(LangHandler.GetLangGroupString(menuId, LangHandler.StringType.ResultNoMatch));
                break;
            }
            UiHelper.DisplayMenu(SubMenu, menuId);
        }
        UiState.MenuBack = false;
    }
}