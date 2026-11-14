namespace CoffeeChain.Api.Contracts.Branches
{
    public sealed record BranchSaveRequest (
        string Name, 
        string Address, 
        bool IsOpen
    );
}
