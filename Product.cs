namespace Store;

internal class Product(int productId, string productName, string productDescription, decimal productPrice, Taxes taxes, Money money) : SqlBuilder
{
    // this field is the only one with no set of any type because it cannot logically change without violation of best practices in several fields.
    internal int ProductId { get; } = productId;
    
    internal Money MoneyType { get; private set; } = money;
    
    internal Taxes Taxes { get; private set; } = taxes;
    
    internal string ProductName { get; private set; } = productName;
    
    internal string ProductDescription { get; private set; } = productDescription;
    
    internal decimal ProductPrice { get; private set; } = productPrice;
    
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
        // yes, the field updating is done inside an if statement and then afterward the result is returned whether the product is updated or not
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
        StartStmt("DELETE FROM Products WHERE product_id = @productId AND NOT EXISTS( SELECT order_product_id FROM orderProducts WHERE product_id = @productId);");
        AddArg("@productId", ProductId);
        EndStmt();
        int result = ExecCmdAsync().Result;
        return result > 0;
    }
}