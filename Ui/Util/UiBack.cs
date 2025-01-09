namespace Store.Ui.Util;

internal class UiBack : UiItem
{
    internal UiBack()
    {
        NameId = "Menu_back_option";
    }
    internal override void Execute()
    {
        StateHolder.MenuBack = true;
    }
}