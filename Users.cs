using System.Data;
using System.Security;
using System.Security.Authentication;
using MySqlConnector;

namespace Store;

internal class Users : Collectible<User>
{
    private Users() => LoadData();
    
    private static Users? _instance;

    internal static Users Instance => _instance ??= new Users();

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
            Collectibles.Add(user.Id, user);
        }
        CloseConnection();
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
        Collectibles.Add(userId, new User(
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
    
    internal bool UserExists(string username) => GetValues().Any(user => user.Username == username);
    
    internal bool UserExists(int userId) => GetValues().Any(user => user.Id == userId);

    internal void RemoveUser(User user) => Collectibles.Remove(user.Id);
}