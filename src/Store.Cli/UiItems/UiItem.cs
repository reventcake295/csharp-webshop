using Store.Common.Enums;

namespace Store.Cli.UiItems;

internal abstract class UiItem
{
    /// <summary>
    /// The id of the name for this UiItem is required to be implemented by all subitems because all items have a name 
    /// </summary>
    internal string NameId { get; init; } = "default";
    
    internal virtual bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        if (!Accessible()) return false;
        // this is disabled here because this is better placed in the specific item class Accessible method to ensure that it is only applicable for that
//        if (permRank != Perm.None) 
//            if (User.GetInstance().PermissionRank != permRank) return;
        UiHelper.DisplayOption(key, NameId);
        return true;
    }

    // return the default state so that it does not have to be overriden each time
    protected virtual bool Accessible() => true;

    protected virtual bool Accessible(out Perm key)
    {
        // set the default perm for this so that the method does not have to be overriden each time
        key = Perm.None;
        return Accessible();
    }

    internal abstract void Execute();
}