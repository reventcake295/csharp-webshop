namespace Store;

internal class ShoppingCart : Collectible<OrderProduct>
{
    private ShoppingCart() { }
    private static ShoppingCart? _instance;

    internal static ShoppingCart Instance => _instance ??= new ShoppingCart();

    
//    private readonly List<OrderProduct> _orderProducts = [];

//    internal List<OrderProduct> GetList() => _orderProducts;
    
    internal Money DisplayFormat { get; private set; } = Settings.DefaultMoney;

    internal decimal TotalPrice { get; private set; }


    internal void AddProduct(OrderProduct product)
    {
        Collectibles.Add(product.Product.ProductId, product);
        TotalPrice += product.Taxes.CalculateTotal(product.ProductPrice) * product.Count;
    }

    internal void RemoveProduct(OrderProduct product)
    {
        Collectibles.Remove(product.Product.ProductId);
        TotalPrice -= product.Taxes.CalculateTotal(product.ProductPrice) * product.Count;
    }

    private void ClearCart()
    {
        Collectibles.Clear();
        TotalPrice = 0;
        DisplayFormat = Settings.DefaultMoney;
    }
    
    internal bool CreateOrder(out Order? order)
    {
        order = null;
        Order? possibleOrder = Orders.Instance.GenerateOrder(GetValues(), TotalPrice, DisplayFormat);
        if (possibleOrder == null) return false;
        order = possibleOrder;
        // order successfully created clearing cart and returning true
        ClearCart();
        return true;
    }

    internal bool HasItems() => Collectibles.Count > 0;

    internal bool HasItem(Product currentProduct) => Collectibles.ContainsKey(currentProduct.ProductId);

}