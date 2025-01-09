namespace Store.Ui.User;

internal class UiLoginLogout : UiItem
{
    internal UiLoginLogout()
    {
        NameId = "Menu_LoginLogout_option";
    }
    internal override void Execute()
    {
        Accessible(out Perm permission);
        switch (permission)
        {
            case Perm.None:
                _login();
                break;
            case Perm.Customer:
            case Perm.Admin:_logout();
                break;
            default:
                throw new ArgumentOutOfRangeException(permission.ToString(), "Given permission value is invalid");
        }
    }

    internal override bool DisplayItem(string key)
    {
        Accessible(out Perm permission);
        switch (permission)
        {
            case Perm.None:
                UiHelper.DisplayOption(key, "Menu_Login_option");
                break;
            case Perm.Customer:
            case Perm.Admin:
                UiHelper.DisplayOption(key, "Menu_Logout_option");
                break;
            default:
                throw new ArgumentOutOfRangeException(permission.ToString(), "Given permission value is invalid");
        }
        return true;
    }

    protected override bool Accessible(out Perm permRank)
    {
        permRank = Session.PermissionRank;
        return true;
    }

    private static void _login()
    {
        Console.WriteLine(Lang.GetLangString("login_header"));
        // retrieve the required information from the user, if either turn out false then we just straight up say return no chance for recovery 
        if (!UiHelper.AskQuestion("login_username", out string username, ""))
            return;
        if (!UiHelper.AskQuestion("login_password", out string password, "", hidden: true))
            return;
        // forward the information to the User.login() method for actual processing
        bool result = Session.Login(username, password);
        // I know I use the _username version here and it does not matter which of the two is used here both should have the success and failure text stored
        UiHelper.DisplayResult("login_username", result ? Lang.StringType.ResultSuccess : Lang.StringType.ResultFailure);
    }

    private static void _logout()
    {
        bool result = Session.Logout();
        UiHelper.DisplayResult("logout", result ? Lang.StringType.ResultSuccess : Lang.StringType.ResultFailure);
    }
}