using CoffeeChain.Api.Contracts.Beverages;
using CoffeeChain.Api.Contracts.Branches;
using CoffeeChain.Api.Contracts.Toppings;

namespace CoffeeChain.Api.Contracts.Menu
{
    public sealed record MenuDto(
        BranchDto Branch, 
        List<BeverageDto> Beverages, 
        List<ToppingDto> Toppings
    );
}
