namespace Store;

internal class OrderProduct
{
    internal int? OrderProductId { get; set; }
    
    internal int? OrderId { get; set; }
    
    internal int Count { get; set; }
    
    internal Product Product { get; private set; }

    internal decimal ProductPrice { get; }

    internal decimal PriceTotal => Taxes.CalculateTotal(ProductPrice) * Count;
    internal Taxes Taxes { get; }
    
    internal Money Money { get; }
    
    internal OrderProduct(int orderProductId, int orderId, int count, int productId, decimal productPrice, Taxes taxes, Money money)
    {
        OrderProductId = orderProductId;
        OrderId = orderId;
        Count = count;
        ProductPrice = productPrice;
        Taxes = taxes;
        Money = money;
        Product? product = Products.Instance.Get(productId);
        Product = product ?? throw new NullReferenceException("Product not found");
    }

    internal OrderProduct(Product product, int count)
    {
        Count = count;
        Product = product;
        ProductPrice = product.ProductPrice;
        Taxes = product.Taxes;
        Money = product.MoneyType;
    }
}