namespace Store.Ui.Order;

internal class UiOrderReject : UiItem
{
    internal UiOrderReject()
    {
        NameId = "Menu_OrderReject_option";
    }
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (StateHolder.CurrentOrder == null)
        {
            Console.WriteLine(Lang.GetLangString("order_noOrderSelected"));
            return;
        }

        if (!StateHolder.CurrentOrder.UpdateStatus(OrderStatus.Rejected))
        {
            Console.WriteLine(Lang.GetLangString("order_statusFailure"));
            return;
        }
        Console.WriteLine(Lang.GetLangString("order_statusUpdated"));
        StateHolder.UpdateOrders?.Invoke();
        StateHolder.MenuBack = true;
    }

    protected override bool Accessible()
    {
        return Session.PermissionRank == Perm.Admin;
    }
}