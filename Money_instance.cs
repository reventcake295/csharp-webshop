using System.Globalization;

namespace Store;

internal partial class Money
{
    // this class is dived in two;
    // The Money instance data and the creation and storage of the instances 
    
    // instance: Money_instance.cs
    // creation and storage: Money_store.cs extends Collectible<Money>
    
    // instance data and function
    public int Id { get; set; }
    internal string Currency { get; set; } = string.Empty;
    private string Format { get; set; } = string.Empty;

    private CultureInfo? _cultureBack;
    private CultureInfo Culture
    {
        get
        {
            // first detect if it is not already set, if yes, then return it, otherwise create the Culture, store it and then return that 
            if (_cultureBack != null) return _cultureBack;
            string[] parts = Format.Split(';');
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
    private Money(int id, string currency, string format)
    {
        Id = id;
        Currency = currency;
        // we do not create the culture here but leave it for when it gets called and if it gets called
        Format = format;
    }
    
    internal string FormatPrice(decimal price) => price.ToString("C2", Culture);
}