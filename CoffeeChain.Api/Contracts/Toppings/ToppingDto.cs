namespace CoffeeChain.Api.Contracts.Toppings
{
    public sealed record ToppingDto(
        int Id, 
        string Name, 
        decimal Price, 
        bool IsAvailable
    );
}
