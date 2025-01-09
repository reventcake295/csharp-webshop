using MySqlConnector;

namespace Store;

internal class Orders : SqlBuilder
{
    private Order[] _orders = [];

    internal void ListUserOrders()
    {
        throw new NotImplementedException();
    }

    internal void ListIncomingOrders()
    {
        throw new NotImplementedException();
    }

    protected override void LoadData(int orderId)
    {
        throw new NotImplementedException();
    }

    internal static bool UserHasOrders(int userId, OrderStatus? status = null)
    {
        Orders orders = new();
        // ensure that only when the order status is set that the statusId param is also given
        if (status.HasValue)
        {
            orders.StartStmt("SELECT COUNT(*) AS orderCount FROM Orders O WHERE O.user_id=@userId AND O.statusId=@statusId;");
            orders.AddArg("@statusId", (int)status);
        } else orders.StartStmt("SELECT COUNT(*) AS orderCount FROM Orders O WHERE O.user_id=@userId;");
        orders.AddArg("@userId", userId);
        MySqlDataReader result = orders.ExecQueryAsync().Result;
        if (!result.Read()) return false;
        bool hasOrders = result.GetInt32(0) > 0;
        orders.CloseConnection();
        return hasOrders;
    }
}

internal class Order : SqlBuilder
{
    [Mapping(ColumnName = "order_id")]
    internal int OrderId { get; set; }
    [Mapping(ColumnName = "order_date")]
    internal DateTime OrderDate { get; set; }
    [Mapping(ColumnName = "user_id")]
    internal int CustomerId { get; set; }
    
    [Mapping(ColumnName = "orderTotal")]
    internal decimal OrderTotal { get; set; }
    
    [Mapping(ColumnName = "M.displayFormat")]
    internal string DisplayFormat { get; set; }
    
    private int _statusId;
    [Mapping(ColumnName = "status_id")]
    internal int StatusId
    {
        get => _statusId;
        set
        {
            _statusId = value;
            Status = (OrderStatus) value;
        }
    }
    internal OrderStatus Status { get; private set; }


    protected override void LoadData(int id)
    {
        throw new NotImplementedException();

    }
}
internal enum OrderStatus
{
    Incoming = 0,
    Accepted = 1,
    Rejected = 2,
}
