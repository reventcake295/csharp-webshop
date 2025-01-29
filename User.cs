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
