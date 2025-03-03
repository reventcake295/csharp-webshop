using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Store.Common.Enums;
using Store.Data.Attributes;
using Store.Data.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Data.Models;

[Table("Orders")]
internal class OrderData : IDataParentModel<OrderModel, OrderData, OrderProductModel, OrderProductData>
{
    internal OrderData(OrderModel model)
    {
        order_id = model.Id;
        user_id = model.Customer?.Id;
        money_id = model.Money.Id;
        order_date = model.Date;
        orderTotal = model.Total;
        statusId = (int)model.OrderStatus;
    }

    internal OrderData(int order_id, int money_id, DateTime order_date, decimal orderTotal, int statusId, int? user_id)
    {
        this.order_id = order_id;
        this.user_id = user_id;
        this.money_id = money_id;
        this.order_date = order_date;
        this.orderTotal = orderTotal;
        this.statusId = statusId;
    }
    
    [ModelMapping(Name = "Id")]
    [Key, Required] 
    public required int order_id { get; set; }
    public int GetId() => order_id;
    
    [ModelMapping(Name = "Customer", Accessor = "UserModel.Id", PropertyType = PropertyType.Model)]
    public int? user_id { get; set; }
    
    [ModelMapping(Name = "Money", Accessor = "MoneyModel.Id", PropertyType = PropertyType.Model)]
    [Required] 
    public required int money_id { get; set; }
    
    [ModelMapping(Name = "Date")]
    [Required]
    public required DateTime order_date { get; set; }
    
    [ModelMapping(Name = "Total")]
    [Required][Column(TypeName = "Double")]
    public required decimal orderTotal { get; set; }
    
    [ModelMapping(Name = "OrderStatus", PropertyType = PropertyType.Enum)]
    [Required]
    public required int statusId { get; set; }
    
    
    public OrderModel ToCommonModel() => new(
        order_id,
        user_id,
        order_date,
        orderTotal,
        (OrderStatus) statusId,
        money_id
    );

//    [InverseProperty("order_id")] 
    public ICollection<OrderProductData> Children { get; set; } = [];
}