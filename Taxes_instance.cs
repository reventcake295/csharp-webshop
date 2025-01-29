namespace Store;

internal partial class Taxes
{
    // this class is dived in two;
    // The Taxes instance data and the creation and storage of the instances 
    
    // instance: Taxes_instance.cs
    // creation and storage: Taxes_store.cs extends Collectible<Taxes>
    
    // instance data and functions
    internal int Id { get; }
    internal int TaxRate { get; }
    internal string TaxName { get; } = string.Empty;
    
    private Taxes(int id, int taxRate, string taxName)
    {
        Id = id;
        TaxRate = taxRate;
        TaxName = taxName;
    }
    public decimal CalculateTax(decimal price) => price * (Convert.ToDecimal(TaxRate) / 100);

    public decimal CalculateTotal(decimal price) => price * (Convert.ToDecimal(TaxRate) / 100) + price;
    
}