using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Data.Attributes;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Data.Models;

[Table("Money")]
internal class MoneyData : IEntityModel<MoneyModel, MoneyData>
{
    [ModelMapping(Name = "Id")]
    [Key, Column("money_id")] [Required] 
    public required int money_id { get; set; }

    public int GetId() => money_id;
    
    [ModelMapping(Name = "DisplayFormat")]
    [Required, Column("displayFormat"), MaxLength(255)] 
    public required string displayFormat { get; set; }
    
    [ModelMapping(Name = "Name")]
    [Required, Column("name"), MaxLength(255)]
    public required string name { get; set; }

    internal MoneyData(MoneyModel moneyModel)
    {
        money_id = moneyModel.Id;
        displayFormat = moneyModel.DisplayFormat;
        name = moneyModel.Name;
    }

    internal MoneyData(int money_id, string name, string displayFormat)
    {
        this.money_id = money_id;
        this.displayFormat = displayFormat;
        this.name = name;
    }
    
    public MoneyModel ToCommonModel() => new(
            money_id,
            displayFormat,
            name
        );
}