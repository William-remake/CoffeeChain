using System.Globalization;
using System.Security.Cryptography;
using CoffeeChain.Api.Contracts.Auth;
using CoffeeChain.Api.Contracts.Beverages;
using CoffeeChain.Api.Contracts.Branches;
using CoffeeChain.Api.Contracts.Menu;
using CoffeeChain.Api.Contracts.Orders;
using CoffeeChain.Api.Contracts.Toppings;
using CoffeeChain.Api.Security;

namespace CoffeeChain.Api.Services;

/// <summary>
/// Coffee Chain data store with persistent orders.
/// Initial branch, beverage and topping data are loaded from the supplied TXT files.
/// All writes are synchronized so an order is committed only after every line is valid.
/// </summary>
public sealed class CoffeeChainStore
{
    private readonly object _gate = new();
    private readonly List<UserRecord> _users = new();
    private readonly List<BranchRecord> _branches = new();
    private readonly List<BeverageRecord> _beverages = new();
    private readonly List<ToppingRecord> _toppings = new();
    private readonly OrderFile _orderFile;
    private readonly Dictionary<string, AuthenticatedUser> _tokens = new(StringComparer.Ordinal);

    public CoffeeChainStore(string? ordersPath = null)
    {
        _orderFile = new OrderFile(ordersPath);
        SeedUsers();
        LoadReferenceData();
    }

    private void SeedUsers()
    {
        _users.AddRange(new[]
        {
            new UserRecord(1, "admin@coffeechain.local", "Admin@123", "Quản trị viên", "Admin", null),
            new UserRecord(2, "barista@coffeechain.local", "Barista@123", "Nhân viên pha chế", "Barista", 1),
            new UserRecord(3, "customer@coffeechain.local", "Customer@123", "Khách hàng Demo", "Customer", null)
        });
    }

    private void LoadReferenceData()
    {
        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

        var branchLines = ReadDataLines(Path.Combine(dataDirectory, "Address.txt"));
        var beverageLines = ReadDataLines(Path.Combine(dataDirectory, "Drinks.txt"));
        var toppingLines = ReadDataLines(Path.Combine(dataDirectory, "Toppings.txt"));

        foreach (var line in branchLines)
        {
            var parts = line.Split('|');
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0])) continue;
            _branches.Add(new BranchRecord(
                _branches.Count + 1,
                parts[0].Trim(),
                parts[1].Trim(),
                true));
        }

        foreach (var line in beverageLines)
        {
            var parts = line.Split('|');
            if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[0])) continue;
            if (!decimal.TryParse(parts[2].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
                continue;

            var name = parts[0].Trim();
            var description = parts[1].Trim();
            _beverages.Add(new BeverageRecord(
                _beverages.Count + 1,
                name,
                description,
                InferCategory(name),
                price,
                true));
        }

        foreach (var line in toppingLines)
        {
            var parts = line.Split('|');
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0])) continue;
            if (!decimal.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
                continue;

            _toppings.Add(new ToppingRecord(_toppings.Count + 1, parts[0].Trim(), price, true));
        }

        // Fallback data keeps the API usable if the TXT files were not copied to the output folder.
        if (_branches.Count == 0)
        {
            _branches.AddRange(new[]
            {
                new BranchRecord(1, "Coffee Chain - Biên Hòa", "Đường Võ Thị Sáu, Biên Hòa", true),
                new BranchRecord(2, "Coffee Chain - Thủ Đức", "Đường Võ Văn Ngân, TP. Thủ Đức", true),
                new BranchRecord(3, "Coffee Chain - Quận 1", "Đường Nguyễn Huệ, Quận 1", true)
            });
        }

        if (_beverages.Count == 0)
            _beverages.Add(new BeverageRecord(1, "Trà sữa truyền thống", "Trà sữa truyền thống.", "Trà sữa", 20000m, true));

        if (_toppings.Count == 0)
            _toppings.Add(new ToppingRecord(1, "Trân Châu Đen", 5000m, true));
    }

    private static List<string> ReadDataLines(string path)
    {
        if (!File.Exists(path)) return new List<string>();
        return File.ReadAllLines(path)
            .Select(line => line.Trim().TrimStart('\uFEFF'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }

    private static string InferCategory(string name)
    {
        if (name.Contains("trà sữa", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("sữa tươi", StringComparison.OrdinalIgnoreCase)) return "Trà sữa";
        if (name.Contains("trà", StringComparison.OrdinalIgnoreCase)) return "Trà trái cây";
        if (name.Contains("cà phê", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("bạc xỉu", StringComparison.OrdinalIgnoreCase)) return "Cà phê";
        return "Đồ uống khác";
    }

    public LoginResponse? Login(string email, string password)
    {
        lock (_gate)
        {
            var normalizedEmail = (email ?? string.Empty).Trim();
            var user = _users.FirstOrDefault(x =>
                string.Equals(x.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);
            if (user is null) return null;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var authenticated = new AuthenticatedUser(user.Id, user.Email, user.DisplayName, user.Role, user.BranchId);
            _tokens[token] = authenticated;
            return new LoginResponse(token, user.Email, user.DisplayName, user.Role, user.BranchId);
        }
    }

    public bool TryGetUser(string token, out AuthenticatedUser user)
    {
        lock (_gate)
        {
            if (_tokens.TryGetValue(token, out var found))
            {
                user = found;
                return true;
            }
        }

        user = null!;
        return false;
    }

    public List<BranchDto> GetBranches()
    {
        lock (_gate) return _branches.Select(ToDto).ToList();
    }

    public MenuDto? GetMenu(int branchId)
    {
        lock (_gate)
        {
            var branch = _branches.FirstOrDefault(x => x.Id == branchId);
            if (branch is null) return null;

            return new MenuDto(
                ToDto(branch),
                _beverages.Where(x => x.IsAvailable).Select(ToDto).ToList(),
                _toppings.Where(x => x.IsAvailable).Select(ToDto).ToList());
        }
    }

    public List<BeverageDto> GetBeverages()
    {
        lock (_gate) return _beverages.Select(ToDto).ToList();
    }

    public List<ToppingDto> GetToppings()
    {
        lock (_gate) return _toppings.Select(ToDto).ToList();
    }

    public List<OrderDto> GetCustomerOrders(int customerId)
    {
        return _orderFile.Read(orders => orders.Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt).ToList());
    }

    public List<OrderDto> GetBaristaOrders(int branchId)
    {
        return _orderFile.Read(orders => orders.Where(x => x.BranchId == branchId && x.Status != "Completed")
            .OrderBy(x => x.CreatedAt).ToList());
    }

    public List<OrderDto> GetAllOrders()
    {
        return _orderFile.Read(orders => orders.OrderByDescending(x => x.CreatedAt).ToList());
    }

    /// <summary>
    /// Validates and prices every item before committing the order.
    /// </summary>
    public OrderDto PlaceOrder(AuthenticatedUser customer, PlaceOrderRequest request)
    {
        lock (_gate)
        {
            if (customer.Role != "Customer") throw new InvalidOperationException("Chỉ Customer mới được đặt món.");
            if (request.Items is null || request.Items.Count == 0) throw new InvalidOperationException("Giỏ hàng đang trống.");
            if (request.Items.Count > 20) throw new InvalidOperationException("Mỗi đơn hàng tối đa 20 dòng món.");

            var branch = _branches.FirstOrDefault(x => x.Id == request.BranchId)
                ?? throw new InvalidOperationException("Không tìm thấy chi nhánh.");
            if (!branch.IsOpen) throw new InvalidOperationException("Chi nhánh hiện đang đóng cửa, không thể đặt món.");

            var preparedItems = new List<OrderItemDto>();
            decimal orderTotal = 0m;

            foreach (var item in request.Items)
            {
                if (item.Quantity < 1 || item.Quantity > 20)
                    throw new InvalidOperationException("Số lượng mỗi món phải từ 1 đến 20.");
                if (!IsValidPercent(item.SugarPercent) || !IsValidPercent(item.IcePercent))
                    throw new InvalidOperationException("Mức đường và đá phải là 0%, 25%, 50%, 75% hoặc 100%.");

                var size = (item.Size ?? string.Empty).Trim().ToUpperInvariant();
                var sizeExtra = size switch
                {
                    "S" => 0m,
                    "M" => 5000m,
                    "L" => 10000m,
                    _ => throw new InvalidOperationException("Kích cỡ không hợp lệ. Chỉ hỗ trợ S, M hoặc L.")
                };

                var beverage = _beverages.FirstOrDefault(x => x.Id == item.BeverageId && x.IsAvailable)
                    ?? throw new InvalidOperationException($"Đồ uống mã {item.BeverageId} không tồn tại hoặc đang ngừng bán.");

                var toppingIds = item.ToppingIds ?? new List<int>();
                if (toppingIds.Distinct().Count() != toppingIds.Count)
                    throw new InvalidOperationException("Một topping không được chọn lặp lại trong cùng một món.");

                var selectedToppings = new List<OrderItemToppingDto>();
                foreach (var toppingId in toppingIds)
                {
                    var topping = _toppings.FirstOrDefault(x => x.Id == toppingId && x.IsAvailable)
                        ?? throw new InvalidOperationException($"Topping mã {toppingId} không tồn tại hoặc đang ngừng bán.");
                    selectedToppings.Add(new OrderItemToppingDto(topping.Id, topping.Name, topping.Price));
                }

                var drinkUnitPrice = beverage.BasePrice + sizeExtra;
                var toppingUnitPrice = selectedToppings.Sum(x => x.UnitPrice);
                var lineTotal = (drinkUnitPrice + toppingUnitPrice) * item.Quantity;
                orderTotal += lineTotal;

                preparedItems.Add(new OrderItemDto(
                    beverage.Id, beverage.Name, size,
                    item.SugarPercent, item.IcePercent, item.Quantity,
                    drinkUnitPrice, toppingUnitPrice, lineTotal, selectedToppings));
            }

            return _orderFile.Update(orders =>
            {
                var id = orders.Count == 0 ? 1 : orders.Max(x => x.Id) + 1;
                var createdAt = DateTimeOffset.Now;
                var order = new OrderDto(
                    id, $"CC{createdAt:yyyyMMdd}-{id:0000}", branch.Id, branch.Name, customer.Id,
                    customer.DisplayName, createdAt, "Pending", orderTotal, preparedItems);
                orders.Add(order);
                return order;
            });
        }
    }

    public OrderDto UpdateOrderStatus(int orderId, AuthenticatedUser barista, string requestedStatus)
    {
        if (barista.Role != "Barista") throw new InvalidOperationException("Chỉ Barista mới được cập nhật trạng thái pha chế.");
        return _orderFile.Update(orders =>
        {
            var orderIndex = orders.FindIndex(x => x.Id == orderId);
            if (orderIndex < 0) throw new InvalidOperationException("Không tìm thấy đơn hàng.");

            var order = orders[orderIndex];
            if (barista.BranchId != order.BranchId)
                throw new InvalidOperationException("Bạn chỉ được xử lý đơn hàng thuộc chi nhánh được phân công.");

            var next = (requestedStatus ?? string.Empty).Trim();
            var validTransition = (order.Status, next) switch
            {
                ("Pending", "Preparing") => true,
                ("Preparing", "Ready") => true,
                ("Ready", "Completed") => true,
                _ => false
            };
            if (!validTransition)
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ {order.Status} sang {next}. Quy trình: Pending → Preparing → Ready → Completed.");

            var updated = order with { Status = next };
            orders[orderIndex] = updated;
            return updated;
        });
    }

    public BeverageDto SaveBeverage(BeverageSaveRequest request, int? id = null)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("Tên đồ uống không được để trống.");
            if (request.BasePrice <= 0) throw new InvalidOperationException("Giá đồ uống phải lớn hơn 0.");

            BeverageRecord record;
            if (id is null)
            {
                var nextId = _beverages.Count == 0 ? 1 : _beverages.Max(x => x.Id) + 1;
                record = new BeverageRecord(nextId, request.Name.Trim(), request.Description?.Trim() ?? string.Empty,
                    string.IsNullOrWhiteSpace(request.Category) ? "Khác" : request.Category.Trim(), request.BasePrice, request.IsAvailable);
                _beverages.Add(record);
            }
            else
            {
                var index = _beverages.FindIndex(x => x.Id == id.Value);
                if (index < 0) throw new InvalidOperationException("Không tìm thấy đồ uống cần cập nhật.");
                record = new BeverageRecord(id.Value, request.Name.Trim(), request.Description?.Trim() ?? string.Empty,
                    string.IsNullOrWhiteSpace(request.Category) ? "Khác" : request.Category.Trim(), request.BasePrice, request.IsAvailable);
                _beverages[index] = record;
            }
            return ToDto(record);
        }
    }

    public ToppingDto SaveTopping(ToppingSaveRequest request, int? id = null)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("Tên topping không được để trống.");
            if (request.Price < 0) throw new InvalidOperationException("Giá topping không được âm.");

            ToppingRecord record;
            if (id is null)
            {
                var nextId = _toppings.Count == 0 ? 1 : _toppings.Max(x => x.Id) + 1;
                record = new ToppingRecord(nextId, request.Name.Trim(), request.Price, request.IsAvailable);
                _toppings.Add(record);
            }
            else
            {
                var index = _toppings.FindIndex(x => x.Id == id.Value);
                if (index < 0) throw new InvalidOperationException("Không tìm thấy topping cần cập nhật.");
                record = new ToppingRecord(id.Value, request.Name.Trim(), request.Price, request.IsAvailable);
                _toppings[index] = record;
            }
            return ToDto(record);
        }
    }

    public BranchDto SaveBranch(BranchSaveRequest request, int? id = null)
    {
        lock (_gate)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new InvalidOperationException("Tên chi nhánh không được để trống.");
            if (string.IsNullOrWhiteSpace(request.Address)) throw new InvalidOperationException("Địa chỉ chi nhánh không được để trống.");

            BranchRecord record;
            if (id is null)
            {
                var nextId = _branches.Count == 0 ? 1 : _branches.Max(x => x.Id) + 1;
                record = new BranchRecord(nextId, request.Name.Trim(), request.Address.Trim(), request.IsOpen);
                _branches.Add(record);
            }
            else
            {
                var index = _branches.FindIndex(x => x.Id == id.Value);
                if (index < 0) throw new InvalidOperationException("Không tìm thấy chi nhánh cần cập nhật.");
                record = new BranchRecord(id.Value, request.Name.Trim(), request.Address.Trim(), request.IsOpen);
                _branches[index] = record;
            }
            return ToDto(record);
        }
    }

    public BranchDto SetBranchStatus(int branchId, bool isOpen)
    {
        lock (_gate)
        {
            var index = _branches.FindIndex(x => x.Id == branchId);
            if (index < 0) throw new InvalidOperationException("Không tìm thấy chi nhánh.");
            var updated = _branches[index] with { IsOpen = isOpen };
            _branches[index] = updated;
            return ToDto(updated);
        }
    }

    private static bool IsValidPercent(int value) => value is 0 or 25 or 50 or 75 or 100;
    private static BranchDto ToDto(BranchRecord x) => new(x.Id, x.Name, x.Address, x.IsOpen);
    private static BeverageDto ToDto(BeverageRecord x) => new(x.Id, x.Name, x.Description, x.Category, x.BasePrice, x.IsAvailable);
    private static ToppingDto ToDto(ToppingRecord x) => new(x.Id, x.Name, x.Price, x.IsAvailable);

    private sealed record UserRecord(int Id, string Email, string Password, string DisplayName, string Role, int? BranchId);
    private sealed record BranchRecord(int Id, string Name, string Address, bool IsOpen);
    private sealed record BeverageRecord(int Id, string Name, string Description, string Category, decimal BasePrice, bool IsAvailable);
    private sealed record ToppingRecord(int Id, string Name, decimal Price, bool IsAvailable);
}
