using MySqlConnector;

namespace Store;

internal partial class Taxes : Collectible<Taxes>
{
    // this class is dived in two;
    // The Taxes instance data and the creation and storage of the instances 
    
    // instance: Taxes_instance.cs
    // creation and storage: Taxes_store.cs extends Collectible<Taxes>
    
    // instance creation and storage
    
    private static Taxes? _instance;
    
    internal static Taxes Instance => _instance ??= new Taxes();
    internal Taxes() => LoadData();
    protected sealed override void LoadData()
    {
        SingleStmt("select * from Taxes");
        MySqlDataReader result = ExecQueryAsync().Result;
        while (result.Read())
        {
            Taxes tax = new(result.GetInt32("taxes_id"),
                            result.GetInt32("percent"), 
                            result.GetString("name"));
            Collectibles.Add(tax.Id, tax);
        }
        CloseConnection();
    }
}