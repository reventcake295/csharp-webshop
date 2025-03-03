using Store.Common.Interfaces;

namespace Store.Common.Model;

public class ProductModel(int id, string name, string description, decimal price, int taxId, int moneyId) : CommonModel(id)
{
    public string Name { get; set; } = name;

    public string Description { get; set; } = description;
    
    public decimal Price { get; set; } = price;
    
    
    // We use this to ensure that the Models are only loaded in if needed
    // and that not more than needed are created by getting the reference instead of having a new one created for each
    private int _taxId = taxId;
    private TaxModel? _taxModel;
    public TaxModel Tax
    {
        get
        {
            _taxModel ??= TaxModel.GetById(_taxId);
            if (_taxModel == null) throw new NullReferenceException("Tax not found in relation to Product");
            return _taxModel;
        }
        set 
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
            if (_moneyModel == null) throw new NullReferenceException("Money not found in relation to Product");
            return _moneyModel;
        }
        set
        {
            _moneyModel = value;
            _moneyId = value.Id;
        }
    }

    public static ProductModel? GetById(int id) => App.GetDataWorker().Product.GetByKey(id);

    public decimal CalculateTax() => Tax.CalculateTax(Price);
    
    public decimal CalculateTotal() => Tax.CalculateTotal(Price);
}