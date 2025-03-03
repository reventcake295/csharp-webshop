using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Attributes;

namespace Store.Data.Models;

[Table("Taxes")]
internal class TaxData : IEntityModel<TaxModel, TaxData>
{
    internal TaxData(TaxModel model)
    {
        taxes_id = model.Id;
        name = model.Name;
        percent = model.Rate;
    }

    internal TaxData(int taxes_id, string name, int percent)
    {
        this.taxes_id = taxes_id;
        this.name = name;
        this.percent = percent;
    }
    
    [ModelMapping(Name = "Id")]
    [Key, Required] 
    public required int taxes_id { get; set; }
    public int GetId() => taxes_id;
    
    [ModelMapping(Name = "Name")]
    [Required, MaxLength(255)] 
    public required string name { get; set; }
    
    [ModelMapping(Name = "Rate")]
    [Required]
    public required int percent { get; set; }

    public TaxModel ToCommonModel() => new(
        taxes_id,
        name,
        percent
    );
}