using Store.Common.Model;

namespace Store.Cli;

public class UiState
{
    internal static UserModel? CurrentUser { get; set; }
    internal static ProductModel? CurrentProduct { get; set; }
    internal static OrderModel? CurrentOrder { get; set; }
    
    internal static Action? UpdateCatalog { get; set; }
    internal static Action? UpdateUsers { get; set; }
    internal static Action? UpdateOrders { get; set; }
    internal static Action? UpdateShoppingCart { get; set; }
    
    internal static bool MenuBack { get; set; }
}