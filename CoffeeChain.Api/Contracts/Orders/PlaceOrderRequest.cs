namespace CoffeeChain.Api.Contracts.Orders
{
    public sealed record PlaceOrderRequest (
        int BranchId, 
        List<OrderItemRequest> Items
    );
}
