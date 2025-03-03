using Microsoft.Extensions.DependencyInjection;
using Store.Cli.Lang;
using Store.Cli.UiItems.Util;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.User;

internal class UiUser : UiMenu
{
    private readonly UserModel _user;

    internal UiUser(UserModel user)
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
        UiHelper.DisplayCustomOption(key, $"{_user.Perm.ToString()}: '{_user.Username}'");
        return true;
    }

    internal override void Execute()
    {
        // set the user set to this instance as the current user for usage by other classes where needed
        UiState.CurrentUser = _user;
        Console.WriteLine(LangHandler.GetLangString("user_info_header"));
        Console.WriteLine($"{LangHandler.GetLangString("user_info_name")}: {_user.Username}");
        Console.WriteLine($"{LangHandler.GetLangString("user_info_email")}: {_user.Email}");
        Console.WriteLine($"{LangHandler.GetLangString("user_info_perm")}: {_user.Perm.ToString()}");
        Console.WriteLine($"{LangHandler.GetLangString("user_info_address")}:");
        Console.WriteLine($"{_user.AdresStreet} {_user.AdresNumber}{_user.AdresAdd}");
        Console.WriteLine($"{_user.AdresZip} {_user.AdresCity}");
        Console.WriteLine();
        DisplayMenu();
        // the funny thing about the way this is implemented
        // is that this can be done without a problem because everything that can happen with a selected user happens via the displayed menu referenced above
        UiState.CurrentUser = null;
    }
    
    protected override bool Accessible() => 
        App.GetScopedService<IUserSession>().Permission == Perm.Admin;
}