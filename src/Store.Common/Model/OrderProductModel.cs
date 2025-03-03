using Store.Common.Interfaces;

namespace Store.Common.Model;

public class OrderProductModel(int id, int orderId, int productId, int quantity, decimal pcsPrice, decimal total, int taxId, int moneyId) : CommonModel(id)
{
    /// <summary>
    /// Create an initial version or the OrderProductModel based on a ProductModel and a quantity to allow for easier conversion to an OrderProduct
    /// </summary>
    /// <param name="product">The Product to create the OrderProduct from</param>
    /// <param name="quantity">The quantity to use for this OrderProduct</param>
    public OrderProductModel(ProductModel product, int quantity) :
        this(0, 0, product.Id, quantity, product.Price, product.Price * quantity, product.Tax.Id, product.Money.Id)
    { }
    
    public int Quantity { get; private set; } = quantity;

    public decimal PscPrice { get; private set; } = pcsPrice;

    public decimal Total { get; private set; } = total;
    
    // We use this to ensure that the Models are only loaded in if needed
    // and that not more than needed are created by getting the reference instead of having a new one created for each
    private int _orderId = orderId;
    private OrderModel? _order;
    public OrderModel Order
    {
        get
        {
            if (_orderId == 0) throw new NullReferenceException();
            return _order ??= OrderModel.GetById(_orderId);
        }
        private set
        {
            _order = value;
            _orderId = value.Id;
        }
    }

    private int _productId = productId;
    private ProductModel? _product;
    public ProductModel Product
    {
        get // => _product ??= ProductModel.GetById(_productId);
        {
            _product ??= ProductModel.GetById(_productId);
            if (_product == null) throw new NullReferenceException("Product not found in relation to OrderProduct");
            return _product;
        }
        private set
        {
            _product = value;
            _productId = value.Id;
        }
    }
    
    
    private int _taxId = taxId;
    private TaxModel? _taxModel;
    public TaxModel Tax
    {
        get// => _taxModel ??= TaxModel.GetById(_taxId);
        {
            _taxModel ??= TaxModel.GetById(_taxId);
            if (_taxModel == null) throw new NullReferenceException("Tax not found in relation to OrderProduct");
            return _taxModel;
        }
        private set 
        {
            _taxModel = value;
            _taxId = value.Id;
        }
    }
    
    private int _moneyId = moneyId;
    private MoneyModel? _moneyModel;
    public MoneyModel Money
    {
        get
        {
            _moneyModel ??= MoneyModel.GetById(_moneyId);
            if (_moneyModel == null) throw new NullReferenceException("Money not found in relation to OrderProduct");
            return _moneyModel;
        }
        private set
        {
            _moneyModel = value;
            _moneyId = value.Id;
        }
    }

    public static OrderProductModel? GetById(int id) => App.GetDataWorker().OrderProduct.GetByKey(id);
    public static List<OrderProductModel> GetByOrderId(int orderId) => App.GetDataWorker().OrderProduct.FindWhere(model => model.Order.Id == orderId).ToList();

    public void ChangeQuantity(int quantity)
    {
        Quantity = quantity;
        Total = Quantity * PscPrice;
    }
}