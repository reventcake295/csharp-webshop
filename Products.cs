using MySqlConnector;

namespace Store;

internal class Products : SqlBuilder
{
    private Products() => LoadData();
    
    private static Products? _instance;
    internal static Products GetInstance() => _instance ??= new Products();

    private readonly List<Product> _products = [];
    protected sealed override void LoadData()
    {
        SingleStmt("SELECT product_id, name, description, price, money_id, taxes_id FROM Products;");
        MySqlDataReader result = ExecQueryAsync().Result;
        if (!result.HasRows) {
            CloseConnection();
            return;
        }
        while (result.Read())
        {
            Product product = new(
                result.GetInt32("product_id"),
                result.GetString("name"),
                result.GetString("description"),
                result.GetDecimal("price"),
                Taxes.TaxTypes[result.GetInt32("taxes_id")],
                Money.MoneyTypes[result.GetInt32("money_id")]
            );
            _products.Add(product);
        }
        CloseConnection();
    }

    protected override void LoadData(int id)
    {
        StartStmt("SELECT product_id, name, description, price, money_id, taxes_id FROM Products WHERE product_id = @id;");
        AddArg("@id", id);
        EndStmt();
        MySqlDataReader result = ExecQueryAsync().Result;
        if (!result.HasRows) {
            CloseConnection();
            return;
        }

        Product product = new(
            result.GetInt32("product_id"),
            result.GetString("name"),
            result.GetString("description"),
            result.GetDecimal("price"),
            Taxes.TaxTypes[result.GetInt32("taxes_id")],
            Money.MoneyTypes[result.GetInt32("money_id")]
            );
        _products.Add(product);
        CloseConnection();
    }

    internal List<Product> GetAllProducts()
    {
        return _products;
    }
    
    internal Product? GetProductById(int id) => _products.Find(product => product.ProductId == id);
    
    internal bool AddProduct(string productName, string productDescription, Money moneyType, decimal productPrice, Taxes taxes)
    {
        StartStmt("INSERT INTO Products (name, description, money_id, taxes_id, price) VALUES (@productName, @productDescription, @productMoneyId, @productTaxesId, @productPrice) RETURNING product_id;");
        AddArg("@productName", productName);
        AddArg("@productDescription", productDescription);
        AddArg("@productMoneyId", moneyType.Id);
        AddArg("@productTaxesId", taxes.Id);
        AddArg("@productPrice", productPrice);
        EndStmt();
        MySqlDataReader result = ExecQueryAsync().Result;
        // get the wanted results from the query
        bool hasRows = result.HasRows;
        int productId = result.GetInt32("product_id");
        CloseConnection();
        // create and add the product to the stored array
        _products.Add(new Product(productId, productName, productDescription, productPrice, taxes, moneyType));
        return hasRows;
    }

    internal bool RemoveProduct(int productId)
    {
        Product? product = GetProductById(productId);
        if (product == null) return false;
        if (!product.Delete()) return false;
        _products.Remove(product);
        return true;
    }
}

internal class Product : SqlBuilder
{
    internal int ProductId { get; set; } // this may say its unused, but it is necessary for the SQlHelper Class mapper 
    
    internal Money MoneyType { get; private set; }
    
    internal Taxes Taxes { get; private set; }
    
    internal string ProductName { get; private set; }
    
    internal string ProductDescription { get; private set; }
    
    internal decimal ProductPrice { get; private set; }

    internal Product(int productId, string productName, string productDescription, decimal productPrice, Taxes taxes, Money money)
    {
        ProductId = productId;
        MoneyType = money;
        Taxes = taxes;
        ProductName = productName;
        ProductDescription = productDescription;
        ProductPrice = productPrice;
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
        EndStmt();
        bool result = ExecCmdAsync().Result > 0;
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
        SingleStmt("DELETE FROM Products WHERE product_id = @productId;");
        int result = ExecCmdAsync().Result;
        return result > 0;
    }
    
    internal OrderProduct CreateOrderProduct(int count) => new(this, count);

}