using MySqlConnector;

namespace Store;

internal class Taxes : SqlBuilder
{
    public static Dictionary<int, Taxes> TaxTypes { get; private set; } = [];
    
    [Mapping(ColumnName = "taxes_id")]
    public int Id { get; set; }
    [Mapping(ColumnName = "percent")]
    public int TaxRate { get; set; }
    [Mapping(ColumnName = "name")]
    public string TaxName { get; set; } = "";
    
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
        taxes.StartStmt("select * from Taxes");
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