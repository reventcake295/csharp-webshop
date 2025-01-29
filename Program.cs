using Store.Ui;

namespace Store;

class Program
{
    public static void Main(string[] args)
    {
        // this is the only line in the project that has a direct string placed inside an WriteLine() function
        Console.WriteLine("The store is loading.");

        // load the settings data,
        // this is vital that it is done now and is waited for because otherwise it will cause issues later on
        if (Settings.SettingsLoaded?.Result == false)
        {
            Console.WriteLine("The store could not be loaded, excuses for the inconvenience.");
            return;
        }
        
        Console.WriteLine(Lang.GetLangString("programLoaded"));
        new Ui.Ui().Run();
    }
}