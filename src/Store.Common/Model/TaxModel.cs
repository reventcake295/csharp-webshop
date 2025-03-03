
using Store.Common.Interfaces;

namespace Store.Common.Model;

public class TaxModel(int id, string name, int rate) : CommonModel(id)
{
    public string Name { get; set; } = name;

    public int Rate { get; set; } = rate;

    public static TaxModel? GetById(int id) => App.GetDataWorker().Tax.GetByKey(id);
    
    public decimal CalculateTax(decimal price) => price * (Convert.ToDecimal(Rate) / 100);

    public decimal CalculateTotal(decimal price) => price * (Convert.ToDecimal(Rate) / 100) + price;
}