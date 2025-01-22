using Store.Ui.Util;

namespace Store.Ui.User;

internal class UiUsers : UiItem
{
    /// <summary>
    /// The items that are always present in the menu, but the number changes depending on what the dynamic list of items is currently
    /// </summary>
    private readonly Dictionary<int, UiItem> _fixedItems = new();
    
    internal UiUsers()
    {
        NameId = "Menu_Users_option";
        _fixedItems.Add(1, new UiBack());
        _fixedItems.Add(2, new UiAddUser());
    }
    internal override void Execute()
    {
        // set the method of the current class instance as the Action to execute when the user's menu is to be reloaded
        StateHolder.UpdateUsers = LoadUsers;
        // see if the user's list needs to be updated or generated
        if (SubMenu.Count == 0)
            LoadUsers();
        // trigger the display menu function as normal
        DisplayMenu("usersMenu");
    }

    protected override bool Accessible() => Session.PermissionRank == Perm.Admin;

    private void LoadUsers()
    {
        // clear the current items list and gather the user's list anew
        SubMenu.Clear();
        List<Store.User> users = Users.Instance.GetAllUsers();
        // add the users to the SubMenu list
        for (int i = 0; i < users.Count; i++)
            SubMenu.Add((i + 1).ToString(), new UiUser(users[i]));
        // add the fixed items at the end
        foreach (KeyValuePair<int, UiItem> fixedItem in _fixedItems)
            SubMenu.Add((fixedItem.Key + users.Count).ToString(), fixedItem.Value);
    }
}