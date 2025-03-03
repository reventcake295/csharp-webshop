using Store.Common.Enums;
using Store.Common.Interfaces;

namespace Store.Common.Model;

public class UserModel(int id, string username, string email, string adresStreet, int adresNumber, string adresAdd, string adresCity, string adresZip, Perm perm) : CommonModel(id)
{
    public string Username { get; set; } = username;
    
    public string Email { get; set; } = email;
    
    public string AdresStreet { get; set; } = adresStreet;
    
    public int AdresNumber { get; set; } = adresNumber;

    public string AdresAdd { get; set; } = adresAdd;
    
    public string AdresZip { get; set; } = adresZip;
    
    public string AdresCity { get; set; } = adresCity;
    
    public Perm Perm { get; set; } = perm;

    public IEnumerable<OrderModel> Orders => OrderModel.GetOrders(Id);

    public static UserModel? GetById(int id) => App.GetDataWorker().User.GetByKey(id);
}