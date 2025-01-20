namespace Store;

internal class ShoppingCart : SqlBuilder
{
    private ShoppingCart() { }
    private static ShoppingCart? _instance;
    internal static ShoppingCart GetInstance() => _instance ??= new ShoppingCart();
    
    private readonly List<OrderProduct> _orderProducts = [];

    internal Money DisplayFormat { get; set; } = Settings.DefaultMoney;

    internal decimal TotalPrice { get; private set; } = 0;

    internal List<OrderProduct> GetList() => _orderProducts;

    internal void AddProduct(OrderProduct product)
    {
        _orderProducts.Add(product);
        TotalPrice += product.Taxes.CalculateTotal(product.ProductPrice) * product.Count;
    }

    internal void RemoveProduct(OrderProduct product)
    {
        _orderProducts.Remove(product);
        TotalPrice -= product.Taxes.CalculateTotal(product.ProductPrice) * product.Count;
    }

    private void ClearCart()
    {
        _orderProducts.Clear();
        TotalPrice = 0;
        DisplayFormat = Settings.DefaultMoney;
    }
    
    internal bool CreateOrder(out Order? order)
    {
        order = null;
        Order? possibleOrder = Orders.GetInstance().GenerateOrder(_orderProducts, TotalPrice, DisplayFormat);
        if (possibleOrder == null) return false;
        order = possibleOrder;
        // order successfully created clearing cart and returning true
        ClearCart();
        return true;
    }

    internal bool HasItems() => _orderProducts.Count > 0;

    internal bool HasItem(Product currentProduct) => _orderProducts.Exists(order => order.Product == currentProduct);

}