namespace CoffeeChain.Api.Contracts.Toppings
{
    public sealed record ToppingSaveRequest (
        string Name, 
        decimal Price, 
        bool IsAvailable
    );
}
