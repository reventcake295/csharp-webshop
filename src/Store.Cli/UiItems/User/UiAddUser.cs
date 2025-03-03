using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;

namespace Store.Cli.UiItems.User;

internal class UiAddUser : UiItem
{
    internal UiAddUser() => NameId = "Menu_AddUser_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        Console.WriteLine(LangHandler.GetLangString("userAdd_header"));
        if (!UiHelper.AskQuestion("userAdd_user", out string username, "")) return;
        if (string.IsNullOrWhiteSpace(username) && App.GetDataWorker().User.UserExists(username))
        {
            Console.WriteLine(LangHandler.GetLangGroupString("userAdd_user", LangHandler.StringType.QuestionWrong));
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
            Console.WriteLine(LangHandler.GetLangString("userAdd_emptyAny"));
            return;
        }
        // because adresAdd is optional, ensure that it has at least an empty string instead of null to ensure that it will not cause issues later on
        adresAdd = string.IsNullOrEmpty(adresAdd) ? "" : adresAdd;
        // don't know how to trigger this if case due to the way the AskQuestion itself is written,
        // but I place it here to catch problems before it really is problematic
        
        // due to the way this is handled by the user class to add the user, there are cases that will throw exceptions for us to handle
        UserModel userModel = new(
            0,
            username,
            email,
            adresStreet,
            adresNumber,
            adresAdd,
            adresPostal,
            adresCity,
            permissionRank
        );
        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.User.Register(userModel, password);
        Task result = dataWorker.FinishSessionAsync();
        if (!result.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangGroupString("userAdd_user", LangHandler.StringType.ResultFailure));
            return;
        }
        UiState.UpdateUsers?.Invoke();
        Console.WriteLine(LangHandler.GetLangGroupString("userAdd_user", LangHandler.StringType.ResultSuccess));
    } 
    
    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
}