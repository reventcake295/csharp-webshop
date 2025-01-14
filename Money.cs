using System.Globalization;
using MySqlConnector;

namespace Store;

internal class Money : SqlBuilder
{
    public static Dictionary<int, Money> MoneyTypes { get; private set; } = [];
    
    [Mapping(ColumnName = "money_id")]
    public int Id { get; set; }
    [Mapping(ColumnName = "name")]
    public string Currency { get; set; } = string.Empty;
    [Mapping(ColumnName = "displayFormat")]
    private string Format { get; set; } =  string.Empty;

    private Money(int id, string currency, string format)
    {
        Id = id;
        Currency = currency;
        Format = format;
    }

    internal Money() { }


    internal static void LoadMoney()
    {
        Money taxes = new();
        taxes.StartStmt("select * from Money");
        MySqlDataReader result = taxes.ExecQueryAsync().Result;
        while (result.Read())
        {
            Money tax = new(result.GetInt32("money_id"),
                            result.GetString("name"),
                            result.GetString("displayFormat"));
            MoneyTypes.Add(tax.Id, tax);
        }
        taxes.CloseConnection();
    }

    internal string FormatPrice(decimal price)
    {
        string[] parts = Format.Split(';');
        CultureInfo culture = new(parts[0])
        {
            NumberFormat =
            {
                CurrencyPositivePattern = Convert.ToInt32(parts[1]),
                CurrencySymbol = parts[2]
            }
        };
        return price.ToString("C2", culture);
    }
}