using Store.Common.Model;

namespace Store.Common.Interfaces.Repository;

public interface IUserRepository : IRangeGenericRepository<UserModel>
{
    public UserModel? GetByUsername(string username);
    
    public bool UserExists(string username);
    
    public UserModel? Login(string username, string password);
    
    public void Register(UserModel userModel, string password);
    
    
}