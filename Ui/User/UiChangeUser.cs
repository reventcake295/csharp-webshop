namespace Store.Ui.User;

internal class UiChangeUser : UiItem
{
    internal UiChangeUser()
    {
        NameId = "Menu_ChangeUser_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (StateHolder.CurrentUser is null)
        {
            Console.WriteLine(Lang.GetLangString("userChange_noUserSelected"));
            return;
        }
        Store.User changeUser = StateHolder.CurrentUser;
        Console.WriteLine(Lang.GetLangString("userChange_header"));
        Perm permissionRank;
        if (changeUser.Id == Session.Id)
        { // if the user is editing their own user account, then we skip the asking of the permission settings, otherwise we just ask the info
            Console.WriteLine(Lang.GetLangString("userChange_permCurrentUser"));
            permissionRank = Session.PermissionRank;
        } else if (!UiHelper.AskQuestion("userAdd_permission", out permissionRank, changeUser.PermissionRank, choices:[Perm.Customer, Perm.Admin], optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressStreet", out string adresStreet, changeUser.AdresStreet, optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressNumber", out int adresNumber, changeUser.AdresNumber, optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressAdd", out string adresAdd, changeUser.AdresAdd, optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressPostal", out string adresPostal, changeUser.AdresPostal, min: 6, max: 6, optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_addressCity", out string adresCity, changeUser.AdresCity, optional:true)) return;
        if (!UiHelper.AskQuestion("userAdd_email", out string email, changeUser.Email, optional:true)) return;
        
        // this may seem a bit of a weird thing to include,
        // but the option is there to do this, so an if statement is included to catch it when it happens
        if (permissionRank == changeUser.PermissionRank
            && adresStreet == changeUser.AdresStreet
            && adresNumber == changeUser.AdresNumber
            && adresPostal == changeUser.AdresPostal
            && adresCity == changeUser.AdresCity
            && email == changeUser.Email)
        { // because if this is the case, then we don't need to talk to anybody and just return
            Console.WriteLine(Lang.GetLangString("userChange_noChange"));
            return;
        }
        // this is for later usage in regard to the user's list
        bool updatePerm = changeUser.PermissionRank != permissionRank;
        
        // update the user and then display a message of the result
        Console.WriteLine(
            changeUser.EditUser(permissionRank, adresStreet, adresNumber, adresAdd, adresPostal, adresCity, email)
                ? Lang.GetLangString("userChange_success")
                : Lang.GetLangString("userChange_fail"));
        // if the permission of the user has been updated, then the list needs also to be updated to reflect the changes
        if (updatePerm) StateHolder.UpdateUsers?.Invoke();
    }
    
    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}