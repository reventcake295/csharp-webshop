using MySqlConnector;

namespace Store;

internal class Products : SqlBuilder
{
    private Products() { }
    
    internal static Product[] GetAllProducts()
    {
        Products productsInstance = new();
        productsInstance.StartStmt("SELECT product_id, name, description, price, money_id, taxes_id FROM Products;");
        MySqlDataReader result = productsInstance.ExecQueryAsync().Result;
        if (!result.HasRows) {
            productsInstance.CloseConnection();
            return [];
        }
        Product[] products = SqlHelper.MapToClassArray<Product>(result);
        productsInstance.CloseConnection();
        return products;
    }
}

internal class Product : SqlBuilder
{
    [Mapping(ColumnName = "product_id")]
    public int ProductId { get; set; }

    private int _moneyId;
    [Mapping(ColumnName = "money_id")]
    public int MoneyId
    {
        get => _moneyId;
        set
        {
            _moneyId = value;
            MoneyType = Money.MoneyTypes[value];
        }
    }
    
    public Money MoneyType { get; private set; } = Settings.DefaultMoney;
//    [Mapping(ColumnName = "money_id")]
//    public int MoneyId { get; set; }
//    [Mapping(ColumnName = "Money.displayFormat")]
//    public string MoneyFormat { get; set; }
//    [Mapping(ColumnName = "Money.name")]
//    public string MoneyName { get; set; }
    private int _taxesId;
    
    [Mapping(ColumnName = "taxes_id")]
    public int TaxesId {
        get => _taxesId;
        private set // this may say unused but it is by the SqlHelper Class mapper to assign the Taxes  
        {
            _taxesId = value;
            Taxes = Taxes.TaxTypes[value];
        }
    }
    
    public Taxes Taxes { get; private set; } = Settings.DefaultTaxes;
//    public int TaxesId { get; set; }
//    [Mapping(ColumnName = "Taxes.name")]
//    public string TaxesName { get; set; }
//    [Mapping(ColumnName = "Taxes.percent")]
//    public int TaxesPercent { get; set; }
    
    [Mapping(ColumnName = "name")]
    public string ProductName { get; set; } = string.Empty;
    [Mapping(ColumnName = "description")]
    public string ProductDescription { get; set; } = string.Empty;
    [Mapping(ColumnName = "price")]
    public decimal ProductPrice { get; set; }
    
    internal static bool Add(string productName, string productDescription, Money moneyType, decimal productPrice, Taxes taxes)
    {
        Product product = new();
        product.StartStmt("INSERT INTO Products (name, description, money_id, taxes_id, price) VALUES (@productName, @productDescription, @productMoneyId, @productTaxesId, @productPrice);");
        product.AddArg("@productName", productName);
        product.AddArg("@productDescription", productDescription);
        product.AddArg("@productMoneyId", moneyType.Id);
        product.AddArg("@productTaxesId", taxes.Id);
        product.AddArg("@productPrice", productPrice);
        int result = product.ExecCmdAsync().Result;
        product.CloseConnection();
        return result > 0;
    }
    
    internal bool Edit(string productName, string productDescription, Money moneyType, decimal productPrice, Taxes taxes)
    {
        StartStmt("UPDATE Products SET money_id=@productMoneyId, taxes_id=@productTaxesId, name=@productName, description=@productDescription, price=@productPrice WHERE product_id = @productId;");
        AddArg("@productName", productName);
        AddArg("@productDescription", productDescription);
        AddArg("@productMoneyId", moneyType.Id);
        AddArg("@productTaxesId", taxes.Id);
        AddArg("@productPrice", productPrice);
        AddArg("@productId", ProductId);
        bool result = ExecCmdAsync().Result > 0;
        CloseConnection();
        if (result)
        {
            ProductName = productName;
            ProductDescription = productDescription;
            ProductPrice = productPrice;
            Taxes = taxes;
            MoneyType = moneyType;
        }
        return result;
    }
    
    internal bool Delete()
    {
        StartStmt("DELETE FROM Products WHERE product_id = @productId;");
        int result = ExecCmdAsync().Result;
        CloseConnection();
        return result > 0;
    }


    internal void CreateOrderProduct(int count)
    {
        throw new NotImplementedException();
    }

}