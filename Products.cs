using MySqlConnector;

namespace Store;

internal class Products : Collectible<Product>
{
    private Products() => LoadData();
    
    private static Products? _instance;

    internal static Products Instance => _instance ??= new Products();
    
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
                Taxes.Instance.Get(result.GetInt32("taxes_id")),
                Money.Instance.Get(result.GetInt32("money_id"))
            );
            Collectibles.Add(product.ProductId, product);
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
            Taxes.Instance.Get(result.GetInt32("taxes_id")),
            Money.Instance.Get(result.GetInt32("money_id"))
            );
        Collectibles.Add(product.ProductId, product);
        CloseConnection();
    }
    
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
        if (!hasRows) return false;
        result.Read();
        int productId = result.GetInt32("product_id");
        CloseConnection();
        // create and add the product to the stored array
        Collectibles.Add(productId, new Product(productId, productName, productDescription, productPrice, taxes, moneyType));
        return hasRows;
    }

    internal bool RemoveProduct(int productId)
    {
        Product product = Get(productId);
        if (!product.Delete()) return false;
        Collectibles.Remove(productId);
        return true;
    }
}