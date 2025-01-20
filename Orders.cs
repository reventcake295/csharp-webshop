using MySqlConnector;

namespace Store;

internal class Orders : SqlBuilder
{
    private readonly List<Order> _orders = [];

    // do a direct method call instead of bothering with a full function body, it's only a single line anyway
    internal IEnumerable<Order> GetUserOrders(int userId) => _orders.Where(o => o.CustomerId == userId);
    
    internal IEnumerable<Order> GetIncomingOrders() => _orders.Where(o => o.Status == OrderStatus.Incoming);
    
    protected sealed override void LoadData()
    {
        SingleStmt("SELECT order_id, statusId, order_date, orderTotal, money_id, user_id FROM Orders");
        MySqlDataReader result = ExecQueryAsync().Result;
        while (result.Read())
        {
            // it's possible that the user_id is null because the user has been deleted,
            // if so, then we give value 0 to the Order
            int userId = result.IsDBNull(result.GetOrdinal("user_id")) ? 0 : result.GetInt32("user_id");
            Order o = new(
                result.GetFieldValue<int>(result.GetOrdinal("order_id")),
                (OrderStatus)result.GetInt32("statusId"),
                result.GetDateTime("order_date"),
                result.GetDecimal("orderTotal"),
                Money.MoneyTypes[result.GetInt32("money_id")],
                userId
                );
            _orders.Add(o);
        }
        CloseConnection();
    }

    internal static Orders GetInstance() => _instance ??= new Orders();
    
    private static Orders? _instance;

    private Orders() => LoadData();
    
    internal bool UserHasOrders(int userId, OrderStatus? status = null) => status.HasValue ? 
               _orders.Any(order => order.Status == status && order.CustomerId == userId) 
               : _orders.Any(order => order.CustomerId == userId);
    // ensure that only when the order status is set that the statusId param is also given
    // and then use LINQ to see if there is an order with that

    internal Order? GenerateOrder(List<OrderProduct> products, decimal total, Money money)
    {
        // create the order insertion stmt
        StartStmt("INSERT INTO Orders (user_id, order_date, statusId, orderTotal, money_id) VALUES (@userId, @orderDate, @orderStatusId, @orderTotal, @orderMoneyId);");
        AddArg("@userId", Session.Id);
        AddArg("@orderDate", DateTime.Now);
        AddArg("@orderStatusId", (int)OrderStatus.Incoming);
        AddArg("@orderTotal", total);
        AddArg("@orderMoneyId", money.Id);
        EndStmt();
        SingleStmt("SELECT LAST_INSERT_ID() INTO @_process_orderId;");
        // create the OrderProducts insertion stmt dynamically
        // I know that my IDE states here that I don't have any values being given to the stmt,
        // but those are dynamically added below that 
        string sqlStmt = "INSERT INTO orderProducts (order_id, product_id, taxes_id, money_id, count, pcsPrice, total) VALUES ";
        int prodCount = products.Count;
        // detect if there is more than one product in the list
        if (prodCount > 1)
            for (int i = 1; i < prodCount; i++)
                sqlStmt // add the rows for the products except the last one because of the final char that is used at the end of the string
                    += $"(@_process_orderId, @orderProductId{i}, @productTaxesId{i}, @productMoneyId{i}, @productCount{i}, @productPcsPrice{i}, @productTotal{i}),";
        // add the final row and the closing semicolon
        sqlStmt += $"(@_process_orderId, @orderProductId{prodCount}, @productTaxesId{prodCount}, @productMoneyId{prodCount}, @productCount{prodCount}, @productPcsPrice{prodCount}, @productTotal{prodCount}) RETURNING @_process_OrderId;";
        StartStmt(sqlStmt);
        for (int i = 0; i < prodCount; i++)
        {
            AddArg($"@orderProductId{i+1}", products[i].Product.ProductId);
            AddArg($"@productTaxesId{i+1}", products[i].Taxes.Id);
            AddArg($"@productMoneyId{i+1}", products[i].Money.Id);
            AddArg($"@productCount{i+1}", products[i].Count); 
            AddArg($"@productPcsPrice{i+1}", products[i].ProductPrice);
            AddArg($"@productTotal{i+1}", products[i].PriceTotal);
        }
        EndStmt();
        SingleStmt("SELECT @_process_orderId;");
        MySqlDataReader result = ExecQueryAsync().Result;
        if (!result.NextResult() || !result.Read())
        {
            CloseConnection();
            return null;
        }
        int orderId= result.GetInt32("@_process_orderId");
        CloseConnection();
        
        Order order = new(orderId, OrderStatus.Incoming, DateTime.Now, total, money, Session.Id);
        _orders.Add(order);
        return order;
    }

    public bool HasOrders(int id) => Session.PermissionRank switch
    {
        Perm.None     => false,
        Perm.Admin    => GetIncomingOrders().Any(),
        Perm.Customer => GetUserOrders(id).Any(),
        _ => false
    };
}

internal class Order : SqlBuilder
{
    internal int OrderId { get; set; }
    
    internal DateTime OrderDate { get; set; }
    
    internal int CustomerId { get; set; }
    
    internal decimal OrderTotal { get; set; }
    
    internal Money MoneyType { get; set; }
    
    internal OrderStatus Status { get; private set; }

    private readonly List<OrderProduct> _products = [];
    internal List<OrderProduct> Products
    {
        get
        {
            // this may seem weird, but this is placed here to ensure that when the products get accessed,
            // the products are loaded in
            if (_products.Count == 0) LoadData();
            // if the products list is still empty, then forward the empty list
            // either it is by design or something went wrong
            return _products;
        }
    }
    internal Order(int orderId, OrderStatus status, DateTime orderDate, decimal orderTotal, Money moneyType, int userId)
    {
        OrderId = orderId;
        OrderDate = orderDate;
        OrderTotal = orderTotal;
        MoneyType = moneyType;
        Status = status;
        CustomerId = userId;
    }

    protected override void LoadData()
    {
        StartStmt("SELECT order_product_id, product_id, pcsPrice, count, total, money_id, taxes_id FROM orderProducts oP WHERE oP.order_id = @orderId;");
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
                result.GetDecimal("pcsPrice"), Taxes.TaxTypes[result.GetInt32("taxes_id")], Money.MoneyTypes[result.GetInt32("money_id")]
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

internal enum OrderStatus
{
    Incoming = 0,
    Accepted = 1,
    Rejected = 2,
}
