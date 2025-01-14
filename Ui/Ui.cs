using Store.Ui.Cart;
using Store.Ui.Catalog;
using Store.Ui.Order;
using Store.Ui.User;
using Store.Ui.Util;

namespace Store.Ui;

internal class Ui
{
    private readonly Dictionary<string, UiItem> _menuMap = new();
    
    internal static bool ShuttingDown { get; private set; }

    internal Ui()
    {
        // somehow the chaining of the .Add methods doesn't work,
        // so we do the adding in sequence and reference the map each time
        _menuMap.Add("1", new UiCatalog());
        _menuMap.Add("2", new UiCart());
        _menuMap.Add("3", new UiOrders());
        _menuMap.Add("4", new UiUsers());
        _menuMap.Add("7", new UiLoginLogout());
        _menuMap.Add("8", new UiChangeLang());
        _menuMap.Add("9", new UiExit());
    }

    internal void Run()
    {
        while (!ShuttingDown)
        {
            UiHelper.DisplayMenu(_menuMap, "mainMenu");
        }
    }

    internal static void Shutdown()
    {
        ShuttingDown = true;
    }
    
}