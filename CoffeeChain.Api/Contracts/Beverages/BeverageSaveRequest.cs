namespace CoffeeChain.Api.Contracts.Beverages
{
    public sealed record BeverageSaveRequest (
        string Name, 
        string Description, 
        string Category, 
        decimal BasePrice, 
        bool IsAvailable
    );
}
