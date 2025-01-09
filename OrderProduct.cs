namespace Store;

internal class OrderProduct : Product
{
    [Mapping(ColumnName = "order_product_id")]
    internal int OrderProductId { get; set; }
    [Mapping(ColumnName = "order_id")]
    internal int OrderId { get; set; }
    [Mapping(ColumnName = "count")]
    internal int Count { get; set; }
    
    internal override void LoadData(int productId)
    {
        throw new NotImplementedException();
    }
    
}