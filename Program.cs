using Store.Ui;

namespace Store;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(Lang.GetLangString("programLoading"));
        Console.WriteLine(Lang.GetLangString("programLoaded"));
        new Ui.Ui().Run();
    }
}