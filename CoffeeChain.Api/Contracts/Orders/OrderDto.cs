namespace CoffeeChain.Api.Contracts.Orders
{
    public sealed record OrderDto(
        int Id,
        string Code,
        int BranchId,
        string BranchName,
        int CustomerId,
        string CustomerName,
        DateTimeOffset CreatedAt,
        string Status,
        decimal Total,
        List<OrderItemDto> Items
    );
}
