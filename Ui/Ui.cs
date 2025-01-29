using Store.Ui.Cart;
using Store.Ui.Catalog;
using Store.Ui.Order;
using Store.Ui.User;
using Store.Ui.Util;

namespace Store.Ui;

internal class Ui
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