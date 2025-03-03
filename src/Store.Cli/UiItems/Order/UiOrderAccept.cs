using Store.Cli.Lang;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;

namespace Store.Cli.UiItems.Order;

internal class UiOrderAccept : UiItem
{
    internal UiOrderAccept() => NameId = "Menu_OrderAccept_option";
    
    internal override void Execute()
    {
        if (!Accessible()) return;
        if (UiState.CurrentOrder == null)
        {
            Console.WriteLine(LangHandler.GetLangString("order_noOrderSelected"));
            return;
        }
        
        UiState.CurrentOrder.OrderStatus = OrderStatus.Accepted;
        
        IDataWorker dataWorker = App.GetDataWorker();
        dataWorker.StartSession();
        dataWorker.Order.Update(UiState.CurrentOrder);
        Task result = dataWorker.FinishSessionAsync();
        result.Wait();
        if (!result.IsCompletedSuccessfully)
        {
            Console.WriteLine(LangHandler.GetLangString("order_statusFailure"));
            return;
        }
        Console.WriteLine(LangHandler.GetLangString("order_statusUpdated"));
        UiState.UpdateOrders?.Invoke();
        UiState.MenuBack = true;
    }

    protected override bool Accessible() => App.GetScopedService<IUserSession>().Permission == Perm.Admin;
}