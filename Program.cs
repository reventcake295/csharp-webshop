using Store.Ui;

namespace Store;

class Program
{
    public static void Main(string[] args)
    {
        // this is the only line in the project that has a direct string placed inside an WriteLine() function
        Console.WriteLine("The store is loading.");
        
        // load the standard data
        Taxes.LoadTaxes();
        Money.LoadMoney();
        Products.GetInstance();
        
        Console.WriteLine(Lang.GetLangString("programLoaded"));
        new Ui.Ui().Run();
    }
}