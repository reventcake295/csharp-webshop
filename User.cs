using System.Data;
using System.Security;
using System.Security.Authentication;
using MySqlConnector;

namespace Store;

/// <summary>
/// The backing class for the table Users, do note that certain columns present in the table or not present here
/// </summary>
internal class User(
    int id,
    string username,
    string email,
    string adresStreet,
    int adresNumber,
    string adresAdd,
    string adresPostal,
    string adresCity,
    Perm perm)
    : SqlBuilder
{
    internal int Id { get; } = id;
    
    internal string Username { get; } = username;

    internal Perm PermissionRank { get; private set; } = perm;

    internal string AdresStreet { get; private set; } = adresStreet;

    internal int AdresNumber { get; private set; } = adresNumber;

    internal string AdresAdd { get; private set; } = adresAdd;

    internal string AdresPostal { get; private set; } = adresPostal;

    internal string AdresCity { get; private set; } = adresCity;

    internal string Email { get; private set; } = email;

    internal bool EditUser(Perm permissionRank, string adresStreet, int adresNumber, string adresAdd, string adresPostal, string adresCity, string email)
    {
        // cast the Perm into an int so the database can handle it and for later usage when settings the fields in the instance
        int authId= (int)permissionRank;
        StartStmt("UPDATE users SET email=@email, adres_street=@adresStreet, adres_number=@adresNumber, adres_add=@adresAdd, adres_postal=@adresPostal, adres_city=@adresCity, auth_id=@authId WHERE user_id = @userId;");
        AddArg("@userId", Id);
        AddArg("@email", email);
        AddArg("@adresStreet", adresStreet);
        AddArg("@adresNumber", adresNumber);
        AddArg("@adresAdd", adresAdd);
        AddArg("@adresPostal", adresPostal);
        AddArg("@adresCity", adresCity);
        AddArg("@authId", authId);
        EndStmt();
        int updateUser = ExecCmdAsync().Result;
        CloseConnection();
        if (!(updateUser > 0)) return false;
        
        // update the fields in the instance too so it does not have to grab the whole list from the database again
        PermissionRank = permissionRank;
        AdresStreet = adresStreet;
        AdresNumber = adresNumber;
        AdresAdd = adresAdd;
        AdresPostal = adresPostal;
        AdresCity = adresCity;
        Email = email;
        return true;
    }
    
    internal bool RemoveUser()
    {
        if (Orders.Instance.UserHasOrders(Id))
        {
            // remove the user_id from all orders where they are set to the current user to delete
            StartStmt("UPDATE Orders SET user_id = NULL WHERE user_id = @userId;");
            AddArg("@userId", Id);
            EndStmt();
            int orderResult = ExecCmdAsync().Result;
            if (!(orderResult > 0))
            {
                CloseConnection();
                return false;
            }
            // remove the user id from the locally stored orders too
            foreach (Order userOrder in Orders.Instance.GetUserOrders(Id))
                userOrder.CustomerId = 0;
        }
        // Delete the user itself from the database
        StartStmt("DELETE FROM users WHERE `user_id` = @userId");
        AddArg("@userId", Id);
        EndStmt();
        int userResult = ExecCmdAsync().Result;
        bool deleted = userResult > 0;
        CloseConnection();
        
        Users.Instance.RemoveUser(this);
        
        return deleted;
        // we do not update anything else here that is dependent on the calling function to do
    }
}
internal enum Perm
{
    None = 0,
    Customer = 1,
    Admin = 2
}

internal class Users : SqlBuilder
{
    private Users() => LoadData();
    
    private static Users? _instance;

    internal static Users Instance => _instance ??= new Users();
    
    private readonly List<User> _users = [];

    protected sealed override void LoadData()
    {
        SingleStmt("SELECT user_id, username, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id FROM users;");
        MySqlDataReader result = ExecQueryAsync().Result;
        while (result.Read())
        {
            User user = new(
                result.GetInt32("user_id"),
                result.GetString("username"),
                result.GetString("email"),
                result.GetString("adres_street"),
                result.GetInt32("adres_number"),
                result.GetString("adres_add"),
                result.GetString("adres_postal"),
                result.GetString("adres_city"),
                (Perm)result.GetInt32("auth_id")
            );
            _users.Add(user);
        }
        CloseConnection();
    }

    internal List<User> GetAllUsers()
    {
        if (Session.PermissionRank != Perm.Admin) throw new SecurityException("You must be Admin to use this command");
        return _users;
    }
    
    internal bool AddUser(string username, string password, Perm permissionRank, string adresStreet, int adresNumber, string adresAdd, string adresPostal, string adresCity, string email)
    {
        // ensure that the current user is allowed to do this,
        // high chance that the calling method has also already done this, but we do it anyway
        if (Session.PermissionRank != Perm.Admin) throw new AuthenticationException("You must be Admin to use this command");
        
        if (UserExists(username)) throw new ConstraintException("User already exists");
        
        // insert the user into the database
        // yes, I know that this line is long,
        // but if I wrap it to the next line,
        // it will throw an error and otherwise not check the syntax of the SQL
        StartStmt("INSERT INTO users (username, password, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id) VALUES (@username, @password, @email, @adresStreet, @adresNumber, @adresAdd, @adresPostal, @adresCity, @authId) RETURNING user_id;");
        AddArg("@username", username);
        AddArg("@password", password);
        AddArg("@email", email);
        AddArg("@adresStreet", adresStreet);
        AddArg("@adresNumber", adresNumber);
        AddArg("@adresAdd", adresAdd);
        AddArg("@adresPostal", adresPostal);
        AddArg("@adresCity", adresCity);
        AddArg("@authId", (int)permissionRank); // the value is stored as an int in the database,
                                                // and I don't like to take chances that the system my try
                                                // to convert it differently during execution
        EndStmt();
        MySqlDataReader result = ExecQueryAsync().Result;
        if (!result.Read()) return false;
        int userId = result.GetInt32("user_id");
        _users.Add(new User(
                            userId, 
                            username,
                            email,
                            adresStreet,
                            adresNumber,
                            adresAdd,
                            adresPostal,
                            adresCity,
                            permissionRank
                       ));
        // we don't have to close the connection because that is already done in the called function
        return true; // we expect one row to be affected and that's it
    }
    
    internal bool UserExists(string username) => _users.Any(user => user.Username == username);
    
    internal bool UserExists(int userId) => _users.Any(user => user.Id == userId);

    internal void RemoveUser(User user) => _users.Remove(user);

    public User GetUserById(int id)
    {
        User? user = _users.Find(user => user.Id == id);
        if (user == null) throw new ConstraintException("User not found");
        return user;
    }
}