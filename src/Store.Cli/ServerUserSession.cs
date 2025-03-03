using Store.Common;
using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli;

public class ServerUserSession : IUserSession
{
    public Perm Permission { get; set; } = Perm.None;
    public UserModel? UserModel { get; set; }
    public OrderFactory ShoppingCart { get; set; } = new();
    
    public bool IsAuthenticated(Perm requiredPermission) => requiredPermission == Permission;
    
    public bool Login(string username, string password)
    {
        UserModel? userModel = App.GetDataWorker().User.Login(username, password);
        if (userModel == null) return false;
        UserModel = userModel;
        Permission = userModel.Perm;
        return true;
    }
    
    public bool Logout()
    {
        UserModel = null;
        Permission = Perm.None;
        return true;
    }
}