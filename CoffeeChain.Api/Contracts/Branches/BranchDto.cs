namespace CoffeeChain.Api.Contracts.Branches
{
    public sealed record BranchDto (
        int Id, 
        string Name, 
        string Address, 
        bool IsOpen
    );
}
