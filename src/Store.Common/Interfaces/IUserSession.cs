using Store.Common.Enums;
using Store.Common.Factory;
using Store.Common.Model;

namespace Store.Common.Interfaces;

public interface IUserSession
{
    public Perm Permission { get; protected set; }
    public UserModel? UserModel { get; protected set; }
    
    /// <summary>
    /// In the session we implement our Shopping cart because one shopping cart belongs to one User session
    /// </summary>
    public OrderFactory ShoppingCart { get; protected set; }
    
    public bool IsAuthenticated(Perm requiredPermission);
    
    public bool Login(string username, string password);
    
    public bool Logout();
}