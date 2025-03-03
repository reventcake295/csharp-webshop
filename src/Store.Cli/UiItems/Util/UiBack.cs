namespace Store.Cli.UiItems.Util;

internal class UiBack : UiItem
{
    internal UiBack() => NameId = "Menu_back_option";
    
    internal override void Execute() => UiState.MenuBack = true;
    
}