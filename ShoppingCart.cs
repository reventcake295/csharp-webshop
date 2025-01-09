namespace Store;

internal class ShoppingCart : SqlBuilder
{
    internal OrderProduct[] OrderProducts { get; set; }

    internal int MoneyId { get; set; }
    
    internal string DisplayFormat { get; set; }
    
    internal decimal TotalPrice { get; set; }

    internal void List()
    {
        throw new NotImplementedException();
    }

    internal void AddProduct(Product product)
    {
        throw new NotImplementedException();
    }

    internal void RemoveProduct(Product product)
    {
        throw new NotImplementedException();
    }

    internal void CreateOrder()
    {
        throw new NotImplementedException();
    }
}