namespace Store.Cli.UiItems.Util;

internal class UiExit : UiItem
{
    internal UiExit() => NameId = "Menu_exit_option";
    
    internal override void Execute() => Ui.Shutdown();
    
}