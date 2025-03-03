using Store.Cli.UiItems;
using Store.Cli.UiItems.Cart;
using Store.Cli.UiItems.Catalog;
using Store.Cli.UiItems.Order;
using Store.Cli.UiItems.User;
using Store.Cli.UiItems.Util;

namespace Store.Cli;

public class Ui
{
    private readonly Dictionary<string, UiItem> _menuMap = new()
    { 
        {"1", new UiCatalog()},
        {"2", new UiCart()},
        {"3", new UiOrders()},
        {"4", new UiUsers()},
        {"7", new UiLoginLogout()},
        {"8", new UiChangeLang()},
        {"9", new UiExit()},
    };
    
    internal static bool ShuttingDown { get; private set; }

    internal void Run()
    {
        while (!ShuttingDown)
            UiHelper.DisplayMenu(_menuMap, "mainMenu");
    }

    internal static void Shutdown() => ShuttingDown = true;
}