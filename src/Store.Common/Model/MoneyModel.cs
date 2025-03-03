using System.Globalization;
using Store.Common.Interfaces;

namespace Store.Common.Model;

public class MoneyModel(int id, string displayFormat, string name) : CommonModel(id)
{
    public string DisplayFormat { get; private set; } = displayFormat;
    
    public string Name { get; private set; } = name;

    private CultureInfo? _cultureBack;
    private CultureInfo Culture
    {
        get
        {
            // first detect if it is not already set, if yes, then return it, otherwise create the Culture, store it and then return that 
            if (_cultureBack != null) return _cultureBack;
            string[] parts = DisplayFormat.Split(';');
            _cultureBack = new CultureInfo(parts[0])
            {
                NumberFormat =
                {
                    CurrencyPositivePattern = Convert.ToInt32(parts[1]),
                    CurrencySymbol = parts[2]
                }
            };
            return _cultureBack;
        }
    }
    
    public static MoneyModel? GetById(int id) => App.GetDataWorker().Money.GetByKey(id);
    
    public string FormatPrice(decimal price) => price.ToString("C2", Culture);
}