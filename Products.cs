namespace Store;

internal class Products : SqlBuilder
{
    internal Product[] ProductListing { get; set; }
    
    protected override void LoadData()
    {
        throw new NotImplementedException();
    }

    internal bool Add()
    {
        throw new NotImplementedException();
    }

    internal void List()
    {
        throw new NotImplementedException();
    }
}

internal class Product
{
    [Mapping(ColumnName = "product_id")]
    internal int ProductId { get; set; }
    [Mapping(ColumnName = "money_id")]
    internal int MoneyId { get; set; }
    [Mapping(ColumnName = "Money.displayFormat")]
    internal string MoneyFormat { get; set; }
    [Mapping(ColumnName = "Money.name")]
    internal string MoneyName { get; set; }
    [Mapping(ColumnName = "taxes_id")]
    internal int TaxesId { get; set; }
    [Mapping(ColumnName = "Taxes.name")]
    internal string TaxesName { get; set; }
    [Mapping(ColumnName = "Taxes.percent")]
    internal int TaxesPercent { get; set; }
    [Mapping(ColumnName = "Products.name")]
    internal string ProductName { get; set; }
    [Mapping(ColumnName = "description")]
    internal string ProductDescription { get; set; }
    [Mapping(ColumnName = "price")]
    internal decimal ProductPrice { get; set; }

    internal bool Edit()
    {
        throw new NotImplementedException();
    }

    internal bool Delete()
    {
        throw new NotImplementedException();
    }

    internal void Display()
    {
        throw new NotImplementedException();
    }

    internal virtual void LoadData(int productId)
    {
        throw new NotImplementedException();
    }
}