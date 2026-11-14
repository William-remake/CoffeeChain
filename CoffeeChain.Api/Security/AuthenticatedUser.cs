namespace CoffeeChain.Api.Security
{
    public sealed record AuthenticatedUser (
        int Id, 
        string Email, 
        string DisplayName, 
        string Role, 
        int? BranchId
    );
}
