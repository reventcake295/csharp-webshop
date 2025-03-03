using Store.Common.Enums;
using Store.Common.Interfaces;

namespace Store.Common.Model;

public class OrderModel(int id, int? customerId, DateTime orderDate, decimal total, OrderStatus status, int moneyId) : CommonModel(id)
{
    public DateTime Date { get; private set; } = orderDate;
    
    public decimal Total { get; private set; } = total;

    public OrderStatus OrderStatus { get; set; } = status;

    private List<OrderProductModel>? _orderProducts;
    public List<OrderProductModel> OrderProducts => _orderProducts ??= OrderProductModel.GetByOrderId(Id);
    
    private int? _customerId = customerId;
    private UserModel? _customer;
    public UserModel? Customer
    {
        get
        {
            if (_customerId == null) return null;
            _customer ??= UserModel.GetById((int)_customerId);
            if (_customer == null) throw new NullReferenceException("User not found in relation to Order");
            return _customer;
        }
        set
        {
            _customer = value;
            _customerId = value?.Id;
        }
    }

    private int _moneyId = moneyId;
    private MoneyModel? _moneyModel;
    public MoneyModel Money
    {
        get
        {
            _moneyModel ??= MoneyModel.GetById(_moneyId);
            if (_moneyModel == null) throw new NullReferenceException("Money not found in relation to Order");
            return _moneyModel;
        }
        private set
        {
            _moneyModel = value;
            _moneyId = value.Id;


        }
    }

    public static OrderModel? GetById(int id) => App.GetDataWorker().Order.GetByKey(id);

    public static IEnumerable<OrderModel> GetOrders() => App.GetDataWorker().Order.GetAll();
    public static IEnumerable<OrderModel> GetOrders(int customerId)
    {
        return App.GetDataWorker().Order.FindWhere(model => model.Customer.Id == customerId).ToList();
    }

    public static IEnumerable<OrderModel> GetOrders(OrderStatus status) => App.GetDataWorker().Order.FindWhere(model => model.OrderStatus == status).ToList();
    public static List<OrderModel> GetOrders(int customerId, OrderStatus status) 
        => App.GetDataWorker().Order.FindWhere(model => model.OrderStatus == status 
                                                                && model.Customer.Id == customerId).ToList();
    
    /// <summary>
    /// Calculate the total of the Order, begins with getting the OrderProducts due to unknown current data status, pass false to use the current set within the Order  
    /// </summary>
    /// <param name="regatherProducts"></param>
    /// <returns></returns>
    public decimal CalculateTotal(bool regatherProducts = true)
    {
        // regather the OrderProducts because it is not certain that all the data is already available
        if (regatherProducts) _orderProducts = OrderProductModel.GetByOrderId(Id);
        return Total = OrderProducts.Sum(orderProduct => orderProduct.Total);
    }
}