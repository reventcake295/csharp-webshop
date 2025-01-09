using System.Data;
using System.Security;

namespace Store.Ui.User;

internal class UiAddUser : UiItem
{
    internal UiAddUser()
    {
        NameId = "Menu_AddUser_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        Console.WriteLine(Lang.GetLangString("userAdd_header"));
        if (!UiHelper.AskQuestion("userAdd_user", out string username, "")) return;
        if (!string.IsNullOrWhiteSpace(username) && Store.User.UserExists(username))
        {
            Console.WriteLine(Lang.GetLangGroupString("userAdd_user", Lang.StringType.QuestionWrong));
            return;
        }
        if (!UiHelper.AskQuestion("userAdd_password", out string password, "", hidden: true)) return;
        if (!UiHelper.AskQuestion("userAdd_permission", out Perm permissionRank, Perm.None, choices:[Perm.Customer, Perm.Admin])) return;
        if (!UiHelper.AskQuestion("userAdd_addressStreet", out string adresStreet, "")) return;
        if (!UiHelper.AskQuestion("userAdd_addressNumber", out int adresNumber, 0)) return;
        if (!UiHelper.AskQuestion("userAdd_addressAdd", out string adresAdd, "", optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressPostal", out string adresPostal, "", min: 6, max: 6)) return;
        if (!UiHelper.AskQuestion("userAdd_addressCity", out string adresCity, "")) return;
        if (!UiHelper.AskQuestion("userAdd_email", out string email, "")) return;
        // I know how I have written the AskQuestion functions and know that this if statement is unnecessary. However, the compiler disagrees with that,
        // so it is placed here to close that line of complaints, and yes, addressAdd is left out of this if statement because it is optional
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(adresStreet)
            || string.IsNullOrWhiteSpace(adresPostal) || string.IsNullOrWhiteSpace(adresCity) || string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine(Lang.GetLangString("userAdd_emptyAny"));
            return;
        }
        // because adresAdd is optional, ensure that it has at least an empty string instead of null to ensure that it will not cause issues later on
        adresAdd = string.IsNullOrEmpty(adresAdd) ? "" : adresAdd;
        // don't know how to trigger this if case due to the way the AskQuestion itself is written,
        // but I place it here to catch problems before it really is problematic
        
        // due to the way this is handled by the user class to add the user, there are cases that will throw exceptions for us to handle
        try
        {
            if (!Store.User.AddUser(username, password, permissionRank, adresStreet, adresNumber, adresAdd, adresPostal,
                                    adresCity,
                                    email))
            {
                Console.WriteLine(Lang.GetLangGroupString("userAdd_user", Lang.StringType.ResultFailure));
                return;
            }
            // update the users so that when the menu loads the new users is also present
            StateHolder.UpdateUsers?.Invoke();
            Console.WriteLine(Lang.GetLangGroupString("userAdd_user", Lang.StringType.ResultSuccess));
            
        }
        catch (SecurityException e)
        { // we already checked this at the start, but since the AddUser has another one embedded into it, we catch if here and return 
            return;
        }
        catch (ConstraintException e)
        { // the user already exits, although it is weird, it comes up here instead of at the beginning after the username was supplied
            Console.WriteLine(Lang.GetLangGroupString("userAdd_username", Lang.StringType.QuestionWrong));
        }
        
    } 
    
    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}