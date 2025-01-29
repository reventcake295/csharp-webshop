using MySqlConnector;

namespace Store;

internal class Order(int orderId, OrderStatus status, DateTime orderDate, decimal orderTotal, Money moneyType, int userId) : SqlBuilder
{
    internal int OrderId { get; } = orderId;
    
    internal DateTime OrderDate { get; } = orderDate;
    
    internal int CustomerId { get; set; } = userId;
    
    internal decimal OrderTotal { get; } = orderTotal;
    
    internal Money MoneyType { get; } = moneyType;
    
    internal OrderStatus Status { get; private set; } = status;

    private readonly List<OrderProduct> _products = [];
    internal List<OrderProduct> Products
    {
        get
        {
            // this may seem weird, but this is placed here to ensure that when the products get accessed,
            // the products are loaded in; this is not done via null check because later on during runtime this may change for some reason.
            if (_products.Count == 0) LoadData();
            // if the product list is still empty, then forward the empty list
            // either it is by design or something went wrong
            return _products;
        }
    }

    protected override void LoadData()
    {
        StartStmt("SELECT order_product_id, product_id, pcsPrice, count, total, money_id, taxes_id FROM orderProducts WHERE order_id = @orderId;");
        AddArg("@orderId", OrderId);
        EndStmt();
        MySqlDataReader result = ExecQueryAsync().Result;

        while (result.Read())
        {
            OrderProduct orderProduct = new(
                result.GetInt32("order_product_id"),
                OrderId,
                result.GetInt32("count"),
                result.GetInt32("product_id"),
                result.GetDecimal("pcsPrice"), Taxes.Instance.Get(result.GetInt32("taxes_id")), Money.Instance.Get(result.GetInt32("money_id"))
            );
            _products.Add(orderProduct);
        }
        CloseConnection();
    }

    public bool UpdateStatus(OrderStatus newStatus)
    {
        StartStmt("UPDATE Orders SET statusId=@newStatusId WHERE order_id = @orderId;");
        AddArg("@newStatusId", (int)newStatus);
        AddArg("@orderId", OrderId);
        EndStmt();
        // don't bother getting the result into an int;
        // it is just to see if the update was successful or not
        bool result = ExecCmdAsync().Result > 0;
        if (result) Status = newStatus;
        return result;
    }
}