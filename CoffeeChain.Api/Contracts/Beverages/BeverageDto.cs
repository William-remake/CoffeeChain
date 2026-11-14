namespace CoffeeChain.Api.Contracts.Beverages
{
    public sealed record BeverageDto (
        int Id, 
        string Name, 
        string Description, 
        string Category, 
        decimal BasePrice, 
        bool IsAvailable
    );
}
