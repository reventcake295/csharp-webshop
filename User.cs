using System.Data;
using System.Security;
using System.Security.Authentication;
using MySqlConnector;
using Store.Ui;

namespace Store;

/// <summary>
/// The backing class for the table Users, do note that certain columns present in the table or not present here
/// </summary>
internal class User : SqlBuilder
{
    [Mapping(ColumnName = "user_id")]
    public int Id { get; private set; }
    [Mapping(ColumnName = "username")]
    public string Username { get; private set; }
    
    private int _authLevelId;
    [Mapping(ColumnName = "auth_id")]
    public int AuthLevelId
    {
        get => _authLevelId;
        private set
        {
            _authLevelId = value;
            PermissionRank = (Perm)value;
        }
    }
    public Perm PermissionRank { get; private set; }
    
    [Mapping(ColumnName = "adres_street")]
    public string AdresStreet { get; private set; }
    
    [Mapping(ColumnName = "adres_number")]
    public int AdresNumber { get; private set; }
    
    [Mapping(ColumnName = "adres_add")]
    public string AdresAdd { get; private set; }
    
    [Mapping(ColumnName = "adres_postal")]
    public string AdresPostal { get; private set; }
    
    [Mapping(ColumnName = "adres_city")]
    public string AdresCity { get; private set; }
    
    [Mapping(ColumnName = "email")]
    public string Email { get; private set; }

    protected override void LoadData(int id)
    {
        throw new NotImplementedException("LoadData, not implemented comes later when building the user login handling");
    }

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
        int updateUser = ExecCmdAsync().Result;
        CloseConnection();
        if (!(updateUser > 0)) return false;
        
        // update the fields in the instance too so it does not have to grab the whole list from the database again
        _authLevelId = authId;
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
        if (Orders.UserHasOrders(Id))
        {
            // remove the user_id from all orders where they are set to the current user to delete
            StartStmt("UPDATE Orders SET user_id = NULL WHERE user_id = @userId");
            AddArg("@userId", Id);
            int orderResult = ExecCmdAsync().Result;
            if (!(orderResult > 0))
            {
                CloseConnection();
                return false;
            }
        }
        // Delete the user itself from the database
        StartStmt("DELETE FROM users WHERE `user_id` = @userId");
        AddArg("@userId", Id);
        int userResult = ExecCmdAsync().Result;
        bool deleted = userResult > 0;
        CloseConnection();
        return deleted;
        // we do not update anything else here that is dependent on the calling function to do
    }

    internal static bool UserExists(int userId)
    {
        User user = new();
        user.StartStmt("SELECT count(*) AS userCount FROM users WHERE user_id=@userId;");
        user.AddArg("@user_id", userId);
        MySqlDataReader result = user.ExecQueryAsync().Result;
        if (result.Read())
        {
            if (result.GetInt32("userCount") > 0)
            {
                user.CloseConnection();
                return true;
            }
        }
        user.CloseConnection();
        return false;
    }
    
    internal static bool UserExists(string username)
    {
        User user = new();
        user.StartStmt("SELECT count(*) AS userCount FROM users WHERE username=@username;");
        user.AddArg("@username", username);
        MySqlDataReader result = user.ExecQueryAsync().Result;
        if (result.Read())
        {
            if (result.GetInt32("userCount") > 0)
            {
                user.CloseConnection();
                return true;
            }
        }
        user.CloseConnection();
        return false;
    }
    
    internal static bool AddUser(string username, string password, Perm permissionRank, string adresStreet, int adresNumber, string adresAdd, string adresPostal, string adresCity, string email)
    {
        // ensure that the current user is allowed to do this, high chance that the calling method has also already done this, but we do it anyway
        if (Session.PermissionRank != Perm.Admin) throw new AuthenticationException("You must be Admin to use this command");
        
        if (UserExists(username)) throw new ConstraintException("User already exists");
        
        User user = new();
        // insert the user into the database
        // yes, I know that this line is long but if I wrap it to the next line, it will throw an error and otherwise not check the syntax of the SQL
        user.StartStmt("INSERT INTO users (username, password, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id) VALUES (@username, @password, @email, @adresStreet, @adresNumber, @adresAdd, @adresPostal, @adresCity, @authId)");
        
        user.AddArg("@username", username);
        user.AddArg("@password", password);
        user.AddArg("@email", email);
        user.AddArg("@adresStreet", adresStreet);
        user.AddArg("@adresNumber", adresNumber);
        user.AddArg("@adresAdd", adresAdd);
        user.AddArg("@adresPostal", adresPostal);
        user.AddArg("@adresCity", adresCity);
        user.AddArg("@authId", (int)permissionRank); // the value is stored as an int in the database, and I don't like to take chances that the system my try to convert it differently during execution
        int result = user.ExecCmdAsync().Result;
        // we don't have to close the connection because that is already done in the called function
        return result == 1; // we expect one row to be affected and that's it
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
    private Users() { }
    
    internal static User[] GetAllUsers()
    {
        if (Session.PermissionRank != Perm.Admin) throw new SecurityException("You must be Admin to use this command");
        Users usersInstance = new();
        usersInstance.StartStmt("SELECT user_id, username, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id FROM users;");
        MySqlDataReader result = usersInstance.ExecQueryAsync().Result;
        if (!result.HasRows) {
            usersInstance.CloseConnection();
            return []; // doubt this will ever be the case, but it must be here to prevent edge cases
        }
        User[] users = SqlHelper.MapToClassArray<User>(result);
        usersInstance.CloseConnection();
        return users;
    }
}