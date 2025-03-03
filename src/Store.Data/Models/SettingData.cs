using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Attributes;
using Store.Data.Enums;

namespace Store.Data.Models;

[Table("Settings")]
internal class SettingData : IEntityModel<SettingModel, SettingData>
{
    internal SettingData(SettingModel model)
    {
        settingsId = model.Id;
        DefaultLang = model.DefaultLang;
        AvailableLangs = string.Join(";", model.Languages);
        MaxInputLoop = model.MaxInputLoop;
        DefaultTaxes = model.DefaultTax.Id;
        DefaultMoney = model.DefaultMoney.Id;
    }
    
    internal SettingData() { }
    
    [ModelMapping(Name = "Id")]
    [Key, Column("settingsId")]
    public required int settingsId { get; set; }
    public int GetId() => settingsId;

    [ModelMapping(Name = "DefaultLang")]
    [Required, Column("DefaultLang"), Length(5, 5), MaxLength(5), MinLength(5)]
    public required string DefaultLang { get; set; }
    
    [ModelMapping(Name = "Languages", PropertyType = PropertyType.Disabled)]
    [Required, Column("AvailableLangs"), MaxLength(255)]
    public required string AvailableLangs { get; set; }
    
    [ModelMapping(Name = "MaxInputLoop")]
    [Required, Column("MaxInputLoop")]
    public required int MaxInputLoop { get; set; }
    
    [ModelMapping(Name = "DefaultTax", Accessor = "TaxModel.Id",  PropertyType = PropertyType.Model)]
    [Required, ForeignKey("taxes_id")]
    public required int DefaultTaxes { get; set; }
    
    [ModelMapping(Name = "DefaultMoney", Accessor = "MoneyModel.Id",  PropertyType = PropertyType.Model)]
    [Required, ForeignKey("money_id")]
    public required int DefaultMoney { get; set; }

    public SettingModel ToCommonModel() => new(
        settingsId,
        DefaultLang,
        AvailableLangs.Split(',').ToList(),
        MaxInputLoop,
        DefaultTaxes,
        DefaultMoney
    );
}