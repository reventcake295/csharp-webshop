using MySqlConnector;

namespace Store;

internal class Taxes : SqlBuilder
{
    internal static Dictionary<int, Taxes> TaxTypes { get; private set; } = [];
    
    internal int Id { get; set; }
    internal int TaxRate { get; set; }
    internal string TaxName { get; set; } = "";
    
    public decimal CalculateTax(decimal price)
    {
        return price * (Convert.ToDecimal(TaxRate) / 100);
    }

    public decimal CalculateTotal(decimal price)
    {
        return price * (Convert.ToDecimal(TaxRate) / 100) + price;
    }

    internal static void LoadTaxes()
    {
        Taxes taxes = new Taxes();
        taxes.SingleStmt("select * from Taxes");
        MySqlDataReader result = taxes.ExecQueryAsync().Result;
        while (result.Read())
        {
            Taxes tax = new(result.GetInt32("taxes_id"),
                            result.GetInt32("percent"), 
                            result.GetString("name"));
            TaxTypes.Add(tax.Id, tax);
        }
        taxes.CloseConnection();
    }
    private Taxes(int id, int taxRate, string taxName)
    {
        Id = id;
        TaxRate = taxRate;
        TaxName = taxName;
    }
    
    internal Taxes() { }
    
}