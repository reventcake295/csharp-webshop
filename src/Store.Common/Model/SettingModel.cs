using Store.Common.Interfaces;

namespace Store.Common.Model;

public class SettingModel(int id, string defaultLang, List<string> languages, int maxInputLoop, int defaultTaxId, int defaultMoneyId) : CommonModel(id)
{
    public string DefaultLang { get; set; } = defaultLang;

    public List<string> Languages { get; set; } = languages;

    public int MaxInputLoop { get; set; } = maxInputLoop;

    
    // We use this to ensure that the Models are only loaded in if needed
    // and that not more than needed are created by getting the reference instead of having a new one created for each
    private int _defaultTaxId = defaultTaxId;
    private TaxModel? _defaultTaxModel;
    public TaxModel DefaultTax
    {
        get// => _defaultTaxModel ??= TaxModel.GetById(_defaultTaxId);
        {
            _defaultTaxModel ??= TaxModel.GetById(_defaultTaxId);
            if (_defaultTaxModel == null) throw new NullReferenceException("Tax not found in relation to Settings");
            return _defaultTaxModel;
        }
        set 
        {
            _defaultTaxModel = value;
            _defaultTaxId = value.Id;
        }
    }
    
    private int _defaultMoneyId = defaultMoneyId;
    private MoneyModel? _defaultMoneyModel;
    public MoneyModel DefaultMoney
    {
        get// => _defaultMoneyModel ??= MoneyModel.GetById(_defaultMoneyId);
        {
            _defaultMoneyModel ??= MoneyModel.GetById(_defaultMoneyId);
            if (_defaultMoneyModel == null) throw new NullReferenceException("Money not found in relation to Settings");
            return _defaultMoneyModel;
        }
        set
        {
            _defaultMoneyModel = value;
            _defaultMoneyId = value.Id;
        }
    }
    
}