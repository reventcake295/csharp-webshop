namespace Store.Ui;

internal static class StateHolder
{
    internal static Action? UpdateCatalog { get; set; }
    internal static Action? UpdateUsers { get; set; }
    internal static Action? UpdateOrders { get; set; }
    internal static Action? UpdateShoppingCart { get; set; }
    
    internal static bool MenuBack { get; set; }
    
    internal static Store.User? CurrentUser { get; set; }
    
    internal static Product? CurrentProduct { get; set; }
    
    internal static Store.Order? CurrentOrder { get; set; }
    internal static OrderProduct? CurrentOrderProduct { get; set; }
}