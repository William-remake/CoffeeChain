namespace CoffeeChain.Api.Contracts.Auth
{
    public sealed record LoginResponse (
        string Token, 
        string Email, 
        string DisplayName, 
        string Role, 
        int? BranchId
    );
}
