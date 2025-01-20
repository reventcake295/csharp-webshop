namespace Store.Ui.User;

internal class UiRemUser: UiItem
{
    internal UiRemUser()
    {
        NameId = "Menu_RemUser_option";
    }
    internal override void Execute()
    {
        // check that the user may access the action and is not the user this action is getting called on
        if (!Accessible()) return;
        // ensure that the CurrentUser is not null because otherwise it will throw errors when we try to do things with it
        if (StateHolder.CurrentUser is null)
        { // probably use to do this here, but it will prevent problems from being sent to me by the ide
            Console.WriteLine(Lang.GetLangString("userRem_noUserSelected"));
            return;
        }
        if (!UiHelper.AskQuestion("userRem", out bool confirmAnswer, false)) return;
        if (!confirmAnswer)
        { // if it not either of those two, it means that it is 'no' or 'n' so we display the canceling failure message and return 
            Console.WriteLine(Lang.GetLangGroupString("userRem", Lang.StringType.ResultFailure));
            return;
        }

        // if the user that is to be deleted has incoming orders, then we need to have confirmation that the user needs to be deleted
        if (Orders.GetInstance().UserHasOrders(StateHolder.CurrentUser.Id, OrderStatus.Incoming))
        { // the user has at least one incoming order attached, so we need to ensure that the admin is certain that they wish to delete it,
            // and yes, the option needs to be available due to AVG and other such laws
            if (!UiHelper.AskQuestion("userRem_userIncomingOrders", out bool orderAnswer, false)) return;
            if (!orderAnswer)
            { // the user does not wish to delete the account, so we display a cancel message and return
                Console.WriteLine(Lang.GetLangGroupString("userRem", Lang.StringType.ResultFailure));
                return;
            }
        }
        // remove the user itself, and the function also deletes the user_id from the Orders table where needed
        if (!StateHolder.CurrentUser.RemoveUser())
        {
            Console.WriteLine(Lang.GetLangString("userRem_CouldNotRemoveUser"));
            return;
        }
        Console.WriteLine(Lang.GetLangGroupString("userRem", Lang.StringType.ResultSuccess));
        // remove the user from the StateHolder because the user is no more
        StateHolder.CurrentUser = null;
        // Reload the list of users so the user is no longer present in the list,
        // the User.RemoveUser() function does not do this because this is purely an Ui function 
        StateHolder.UpdateUsers?.Invoke();
        // because I know where this action is getting called from,
        // we send the user back a menu because it will otherwise become problematic when the user does something with the user selected
        StateHolder.MenuBack = true;
    }

    protected override bool Accessible()
    {
        // this action requires the existence of a selected user, so if it is not present, then it is useless to do this.
        // if the user that is currently logged in, we do not want to allow the option of deletion
        if (StateHolder.CurrentUser is null || StateHolder.CurrentUser.Id == Session.Id) return false;
        return Session.PermissionRank == Perm.Admin;
    }
}