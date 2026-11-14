namespace CoffeeChain.Api.Contracts.Orders
{
    public sealed record OrderItemDto(
        int BeverageId,
        string BeverageName,
        string Size,
        int SugarPercent,
        int IcePercent,
        int Quantity,
        decimal DrinkUnitPrice,
        decimal ToppingUnitPrice,
        decimal LineTotal,
        List<OrderItemToppingDto> Toppings
    );
}
