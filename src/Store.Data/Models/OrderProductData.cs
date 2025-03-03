using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Attributes;
using Store.Data.Enums;

namespace Store.Data.Models;

[Table("orderProducts")]
internal class OrderProductData : IDataChildModel<OrderProductModel, OrderProductData, OrderModel, OrderData>
{
    internal OrderProductData(OrderProductModel model, OrderData parent)
    {
        order_product_id = model.Id;
        order_id = parent.order_id;
        product_id = model.Product.Id;
        count = model.Quantity;
        taxes_id = model.Tax.Id;
        money_id = model.Money.Id;
        pcsPrice = model.PscPrice;
        total = model.Total;
        
        Parent = parent;
    }

    internal OrderProductData(int order_product_id, int order_id, int product_id, int count, int taxes_id, int money_id, decimal pcsPrice, decimal total)
    {
        this.order_product_id = order_product_id;
        this.order_id = order_id;
        this.product_id = product_id;
        this.count = count;
        this.taxes_id = taxes_id;
        this.money_id = money_id;
        this.pcsPrice = pcsPrice;
        this.total = total;
    }
    
    [ModelMapping(Name = "Id")]
    [Key, Required]
    public required int order_product_id { get; set; }
    public int GetId() => order_product_id;

    [ModelMapping(Name = "Order", Accessor = "OrderModel.Id",  PropertyType = PropertyType.Model)]
    [Required, Column("order_id")]
    public required int order_id { get; set; }
    
    [ModelMapping(Name = "Product", Accessor = "ProductModel.Id",  PropertyType = PropertyType.Model)]
    [Required]
    public required int product_id { get; set; }

    [ModelMapping(Name = "Quantity")]
    [Required] 
    public required int count { get; set; }
    
    [ModelMapping(Name = "Tax", Accessor = "TaxModel.Id",  PropertyType = PropertyType.Model)]
    [Required, ForeignKey("taxes_id")]
    public required int taxes_id { get; set; }
    
    [ModelMapping(Name = "Money", Accessor = "MoneyModel.Id",  PropertyType = PropertyType.Model)]
    [Required, ForeignKey("money_id")]
    public required int money_id { get; set; }
    
    [ModelMapping(Name = "PscPrice")]
    [Required][Column(TypeName = "Double")]
    public required decimal pcsPrice { get; set; }
    
    [ModelMapping(Name = "Total")]
    [Required][Column(TypeName = "Double")]
    public required decimal total { get; set; }

    public OrderProductModel ToCommonModel() => new(
        order_product_id,
        order_id,
        product_id,
        count,
        pcsPrice,
        total,
        taxes_id,
        money_id
    );

    [ForeignKey("order_id")]
    public required OrderData Parent { get; set; }
}