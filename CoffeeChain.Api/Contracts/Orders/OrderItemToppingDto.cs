namespace CoffeeChain.Api.Contracts.Orders
{
    public sealed record OrderItemToppingDto (
        int ToppingId, 
        string Name, 
        decimal UnitPrice
    );
}
