using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.User;

internal class UiChangeUser : UiItem
{
    internal UiChangeUser() => NameId = "Menu_ChangeUser_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (UiState.CurrentUser is null)
        {
            Console.WriteLine(LangHandler.GetLangString("userChange_noUserSelected"));
            return;
        }
        UserModel changeUser = UiState.CurrentUser;
        bool userUpdated = false;
        bool updatePerm = false;
        Console.WriteLine(LangHandler.GetLangString("userChange_header"));
        Perm permissionRank;
        if (changeUser.Id == App.GetScopedService<IUserSession>().UserModel?.Id)
        { // if the user is editing their own user account, then we skip the asking of the permission settings, otherwise we just ask the info
            Console.WriteLine(LangHandler.GetLangString("userChange_permCurrentUser"));
            permissionRank = App.GetScopedService<IUserSession>().Permission;
        } else if (!UiHelper.AskQuestion("userAdd_permission", out permissionRank, changeUser.Perm, choices:[Perm.Customer, Perm.Admin], optional:true)) return;

        if (permissionRank != changeUser.Perm)
        {
            changeUser.Perm = permissionRank;
            userUpdated = true;
            updatePerm = true;
        }
        if (!UiHelper.AskQuestion("userAdd_addressStreet", out string adresStreet, changeUser.AdresStreet, optional:true)) return;
        if (changeUser.AdresStreet != adresStreet)
        {
            changeUser.AdresStreet = adresStreet;
            userUpdated = true;
        }
        if (!UiHelper.AskQuestion("userAdd_addressNumber", out int adresNumber, changeUser.AdresNumber, optional:true)) return;
        if (changeUser.AdresNumber != adresNumber)
        {
            changeUser.AdresNumber = adresNumber;
            userUpdated = true;
        }
        if (!UiHelper.AskQuestion("userAdd_addressAdd", out string adresAdd, changeUser.AdresAdd, optional:true)) return;
        if (changeUser.AdresAdd != adresAdd)
        {
            changeUser.AdresAdd = adresAdd;
            userUpdated = true;
        }
        if (!UiHelper.AskQuestion("userAdd_addressPostal", out string adresPostal, changeUser.AdresZip, min: 6, max: 6, optional:true)) return;
        if (changeUser.AdresZip != adresPostal)
        {
            changeUser.AdresZip = adresPostal;
            userUpdated = true;
        }
        if (!UiHelper.AskQuestion("userAdd_addressCity", out string adresCity, changeUser.AdresCity, optional:true)) return;
        if (changeUser.AdresCity != adresCity)
        {
            changeUser.AdresCity = adresCity;
            userUpdated = true;
        }
        if (!UiHelper.AskQuestion("userAdd_email", out string email, changeUser.Email, optional:true)) return;
        if (changeUser.Email != email)
        {
            changeUser.Email = email;
            userUpdated = true;
        }
        
        // this may seem a bit of a weird thing to include,
        // but the option is there to do this, so an if statement is included to catch it when it happens
        if (!userUpdated)
        { // because if this is the case, then we don't need to talk to anybody and just return
            Console.WriteLine(LangHandler.GetLangString("userChange_noChange"));
            return;
        }

        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.User.Update(changeUser);
        Task result = dataWorker.FinishSessionAsync();
        result.Wait();
        
        // update the user and then display a message of the result
        Console.WriteLine(result.IsCompletedSuccessfully
                              ? LangHandler.GetLangString("userChange_success")
                              : LangHandler.GetLangString("userChange_fail"));
        // if the permission of the user has been updated, then the list needs also to be updated to reflect the changes
        if (updatePerm) UiState.UpdateUsers?.Invoke();
    }
    
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
    
}