using MySqlConnector;

namespace Store;

internal class Session : SqlBuilder
{
    internal static int Id { get; private set; }
    internal static string? Username { get; private set; }
    
    internal static Perm PermissionRank { get; private set; }

    private static User? _user;
    internal static User User
    {
        get
        {
            return _user ??= Users.Instance.GetUserById(Id);
        }
    }
    private Session()
    {
        // this is placed here for the login method that is placed here because it is in regards to the session that this is handled
    }
    
    /// <summary>
    /// Try to log in the user based on the supplied username and password, and if successful, sets the internal variables to the required data
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <param name="password">The password that was given by the user</param>
    /// <returns></returns>
    internal static bool Login(string username, string password)
    {
        Session session = new();
        // get the User row from the database
        session.StartStmt("SELECT count(*) AS userCount, password, user_id, auth_id FROM users WHERE username=@username;");
        session.AddArg("@username", username);
        session.EndStmt();
        MySqlDataReader result = session.ExecQueryAsync().Result;
        // check that there was a result and then check if it was a singular result can be combined because the handling that happens is the same
        if (!result.Read() || result.GetInt32("userCount") != 1)
        {
            session.CloseConnection();
            return false;
        }
        // retrieve the results from the line and then close the connection
        int userId = result.GetInt32("user_id");
        int authId = result.GetInt32("auth_id");
        string dbPassword = result.GetString("password");
        session.CloseConnection();
        
        // we do nothing complicated here because this is just a portfolio project,
        // otherwise there would be a separate function called to process the password to match the one stored in the database,
        // along with the standard stored info inside the password string required for the processing.
        // think password version, salt, iterations, etc.
        if (dbPassword != password)
        {
            // let the calling function deal with the fallout of this because they should know how to deal with it in regard to the actual interface that's in use currently
            return false;
        }
        
        Username = username;
        PermissionRank = (Perm)authId;
        Id = userId;
        
//        GetInstance().PermissionRank = Perm.Admin;
        return true;
    }

    internal static bool Logout()
    {
        PermissionRank = Perm.None;
        return true;
    }
}