using System.Globalization;
using MySqlConnector;

namespace Store;

internal partial class Money : Collectible<Money>
{
    // this class is dived in two;
    // The Money instance data and the creation and storage of the instances 
    
    // instance: Money_instance.cs
    // creation and storage: Money_store.cs extends Collectible<Money>
    
    // instance creation and storage
    
    private static Money? _instance;
    
    internal static Money Instance => _instance ??= new Money();
    internal Money() => LoadData();
    protected sealed override void LoadData()
    {
        SingleStmt("select * from Money");
        MySqlDataReader result = ExecQueryAsync().Result;
        while (result.Read())
        {
            Money tax = new(result.GetInt32("money_id"),
                            result.GetString("name"),
                            result.GetString("displayFormat"));
            Collectibles.Add(tax.Id, tax);
        }
        CloseConnection();
    }
    
    // instance data and function
    
    
}