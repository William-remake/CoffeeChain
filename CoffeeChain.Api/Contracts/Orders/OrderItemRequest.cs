namespace CoffeeChain.Api.Contracts.Orders
{
    public sealed record OrderItemRequest (
        int BeverageId, 
        string Size, 
        int SugarPercent, 
        int IcePercent, 
        int Quantity, 
        List<int>? ToppingIds
    );
}
