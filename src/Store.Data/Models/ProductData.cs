using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Attributes;
using Store.Data.Enums;

namespace Store.Data.Models;

[Table("Products")]
internal class ProductData : IEntityModel<ProductModel, ProductData>
{
    internal ProductData(ProductModel model)
    {
        product_id = model.Id;
        name = model.Name;
        description = model.Description;
        price = model.Price;
        taxes_id = model.Tax.Id;
        money_id = model.Money.Id;
    }

    internal ProductData(int product_id, string name, string description, decimal price, int taxes_id, int money_id)
    {
        this.product_id = product_id;
        this.name = name;
        this.description = description;
        this.price = price;
        this.taxes_id = taxes_id;
        this.money_id = money_id;
    }
    
    [ModelMapping(Name = "Id")]
    [Key, Required] 
    public required int product_id { get; set; }
    public int GetId() => product_id;

    [ModelMapping(Name = "Name")]
    [Required, Column("name"), MaxLength(255)]
    public required string name { get; set; }
    
    [ModelMapping(Name = "Description")]
    [Required, MaxLength(255)]
    public required string description { get; set; }
    
    [ModelMapping(Name = "Price")]
    [Required][Column(TypeName = "Double")]
    public required decimal price { get; set; }
    
    [ModelMapping(Name = "Tax", Accessor = "TaxModel.Id",  PropertyType = PropertyType.Model)]
    [Required, ForeignKey("taxes_id")]
    public required int taxes_id { get; set; }
//    [ForeignKey("TaxId")]
//    public Tax Tax { get; set; }
    
    [ModelMapping(Name = "Money", Accessor = "MoneyModel.Id",  PropertyType = PropertyType.Model)]
    [Required]
    public required int money_id { get; set; }
    
//    [ForeignKey("MoneyId")]
//    public Money Money { get; set; }
    public ProductModel ToCommonModel() => new(
        product_id,
        name,
        description,
        price,
        taxes_id,
        money_id
    );
}