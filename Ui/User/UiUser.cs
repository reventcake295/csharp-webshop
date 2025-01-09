using Store.Ui.Util;

namespace Store.Ui.User;

internal class UiUser : UiItem
{
    private readonly Store.User _user;

    internal UiUser(Store.User user)
    {
        NameId = "Menu_User_option";
        SubMenu.Add("1", new UiChangeUser());
        SubMenu.Add("2", new UiRemUser());
        SubMenu.Add("3", new UiBack());
        _user = user;
    }

    internal override bool DisplayItem(string key)
    {
        // ensure that the user may access this option
        if (!Accessible()) return false;
        UiHelper.DisplayCustomOption(key, $"{_user.PermissionRank.ToString()}: '{_user.Username}'");
        return true;
    }

    internal override void Execute()
    {
        // set the user set to this instance as the current user for usage by other classes where needed
        StateHolder.CurrentUser = _user;
        Console.WriteLine(Lang.GetLangString("user_info_header"));
        Console.WriteLine($"{Lang.GetLangString("user_info_name")}: {_user.Username}");
        Console.WriteLine($"{Lang.GetLangString("user_info_email")}: {_user.Email}");
        Console.WriteLine($"{Lang.GetLangString("user_info_perm")}: {_user.PermissionRank.ToString()}");
        Console.WriteLine($"{Lang.GetLangString("user_info_address")}:");
        Console.WriteLine($"{_user.AdresStreet} {_user.AdresNumber}{_user.AdresAdd}");
        Console.WriteLine($"{_user.AdresPostal} {_user.AdresCity}");
        Console.WriteLine();
        DisplayMenu("userMenu");
        // the funny thing about the way this is implemented
        // is that this can be done without a problem because everything that can happen with a selected user happens via the displayed menu referenced above
        StateHolder.CurrentUser = null;
    }
    
    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}