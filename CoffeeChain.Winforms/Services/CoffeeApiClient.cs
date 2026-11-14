using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoffeeChain.Winforms.Services;

/// <summary>
/// HTTP client used by WinForms to communicate with CoffeeChain.Api.
/// The DTOs below intentionally live in the WinForms project so this project
/// does not need a compile-time reference to the Web API project.
/// JSON property names and shapes match the API Contracts records.
/// </summary>
public sealed class CoffeeApiClient : IDisposable
{
    private const string DefaultApiUrl = "http://127.0.0.1:5187/";

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public CoffeeApiClient()
    {
        _http = new HttpClient
        {
            BaseAddress = GetDefaultApiAddress(),
            Timeout = TimeSpan.FromSeconds(20)
        };
        _ownsHttpClient = true;
    }

    /// <summary>
    /// Allows a custom HttpClient to be supplied, for example in tests.
    /// The caller remains responsible for the supplied HttpClient lifetime.
    /// </summary>
    public CoffeeApiClient(HttpClient httpClient)
    {
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = false;

        if (_http.BaseAddress is null)
            _http.BaseAddress = GetDefaultApiAddress();

        if (_http.Timeout == Timeout.InfiniteTimeSpan)
            _http.Timeout = TimeSpan.FromSeconds(20);
    }

    /// <summary>Base address currently used by this client.</summary>
    public Uri ApiBaseAddress => _http.BaseAddress!;

    private static Uri GetDefaultApiAddress()
    {
        var configuredUrl = Environment.GetEnvironmentVariable("COFFEECHAIN_API_URL");
        var url = string.IsNullOrWhiteSpace(configuredUrl)
            ? DefaultApiUrl
            : configuredUrl.Trim();

        if (!url.EndsWith("/", StringComparison.Ordinal))
            url += "/";

        if (!Uri.TryCreate(url, UriKind.Absolute, out var address) ||
            (address.Scheme != Uri.UriSchemeHttp && address.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"Địa chỉ CoffeeChain API không hợp lệ: {url}. Hãy dùng địa chỉ http:// hoặc https://.");
        }

        return address;
    }

    public void SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Trim());
    }

    public Task<LoginResponse> LoginAsync(LoginRequest request) =>
        PostAsync<LoginRequest, LoginResponse>("api/auth/login", request);

    public Task<List<BranchDto>> GetBranchesAsync() =>
        GetAsync<List<BranchDto>>("api/branches");

    public Task<MenuDto> GetMenuAsync(int branchId) =>
        GetAsync<MenuDto>($"api/branches/{branchId}/menu");

    public Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request) =>
        PostAsync<PlaceOrderRequest, OrderDto>("api/orders", request);

    public Task<List<OrderDto>> GetOrderHistoryAsync() =>
        GetAsync<List<OrderDto>>("api/orders/history");

    public Task<List<OrderDto>> GetBaristaOrdersAsync() =>
        GetAsync<List<OrderDto>>("api/barista/orders");

    public Task<List<OrderDto>> GetAllOrdersAsync() =>
        GetAsync<List<OrderDto>>("api/admin/orders");

    public Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status) =>
        PatchAsync<UpdateOrderStatusRequest, OrderDto>(
            $"api/barista/orders/{orderId}/status",
            new UpdateOrderStatusRequest(status));

    public Task<List<BeverageDto>> GetBeveragesAsync() =>
        GetAsync<List<BeverageDto>>("api/admin/beverages");

    public Task<BeverageDto> CreateBeverageAsync(BeverageSaveRequest request) =>
        PostAsync<BeverageSaveRequest, BeverageDto>("api/admin/beverages", request);

    public Task<BeverageDto> UpdateBeverageAsync(int id, BeverageSaveRequest request) =>
        PutAsync<BeverageSaveRequest, BeverageDto>($"api/admin/beverages/{id}", request);

    public Task<List<ToppingDto>> GetToppingsAsync() =>
        GetAsync<List<ToppingDto>>("api/admin/toppings");

    public Task<ToppingDto> CreateToppingAsync(ToppingSaveRequest request) =>
        PostAsync<ToppingSaveRequest, ToppingDto>("api/admin/toppings", request);

    public Task<ToppingDto> UpdateToppingAsync(int id, ToppingSaveRequest request) =>
        PutAsync<ToppingSaveRequest, ToppingDto>($"api/admin/toppings/{id}", request);

    public Task<BranchDto> CreateBranchAsync(BranchSaveRequest request) =>
        PostAsync<BranchSaveRequest, BranchDto>("api/admin/branches", request);

    public Task<BranchDto> UpdateBranchAsync(int id, BranchSaveRequest request) =>
        PutAsync<BranchSaveRequest, BranchDto>($"api/admin/branches/{id}", request);

    public Task<BranchDto> SetBranchStatusAsync(int id, bool isOpen) =>
        PatchAsync<BranchStatusRequest, BranchDto>(
            $"api/admin/branches/{id}/status",
            new BranchStatusRequest(isOpen));

    private async Task<T> GetAsync<T>(string path)
    {
        using var response = await _http.GetAsync(path).ConfigureAwait(false);
        return await ReadResponseAsync<T>(response).ConfigureAwait(false);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        using var response = await _http.PostAsJsonAsync(path, request, _jsonOptions).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response).ConfigureAwait(false);
    }

    private async Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        using var response = await _http.PutAsJsonAsync(path, request, _jsonOptions).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response).ConfigureAwait(false);
    }

    private async Task<TResponse> PatchAsync<TRequest, TResponse>(string path, TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        using var response = await _http.PatchAsJsonAsync(path, request, _jsonOptions).ConfigureAwait(false);
        return await ReadResponseAsync<TResponse>(response).ConfigureAwait(false);
    }

    private async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = TryExtractMessage(body);

            if (string.IsNullOrWhiteSpace(message))
            {
                message = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Email/mật khẩu không đúng hoặc phiên đăng nhập đã hết hiệu lực.",
                    HttpStatusCode.Forbidden => "Bạn không có quyền thực hiện chức năng này.",
                    HttpStatusCode.NotFound => "Không tìm thấy dữ liệu hoặc API được yêu cầu.",
                    HttpStatusCode.BadRequest => "Dữ liệu gửi lên không hợp lệ.",
                    _ => $"API trả về lỗi {(int)response.StatusCode} ({response.ReasonPhrase})."
                };
            }

            throw new InvalidOperationException(message);
        }

        // All current API endpoints return JSON data. An empty response is treated as an API contract error.
        try
        {
            var result = await response.Content
                .ReadFromJsonAsync<T>(_jsonOptions)
                .ConfigureAwait(false);

            return result ?? throw new InvalidOperationException("API không trả về dữ liệu hợp lệ.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Không thể đọc dữ liệu JSON do API trả về. Hãy kiểm tra DTO giữa WinForms và API.", ex);
        }
    }

    private static string? TryExtractMessage(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var propertyName in new[] { "message", "detail", "title" })
                {
                    if (root.TryGetProperty(propertyName, out var value) &&
                        value.ValueKind == JsonValueKind.String)
                    {
                        var text = value.GetString();
                        if (!string.IsNullOrWhiteSpace(text))
                            return text;
                    }
                }

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = errors.EnumerateObject()
                        .SelectMany(property => property.Value.ValueKind == JsonValueKind.Array
                            ? property.Value.EnumerateArray()
                                .Where(item => item.ValueKind == JsonValueKind.String)
                                .Select(item => item.GetString())
                            : Enumerable.Empty<string?>())
                        .Where(value => !string.IsNullOrWhiteSpace(value));

                    var combined = string.Join(Environment.NewLine, messages);
                    if (!string.IsNullOrWhiteSpace(combined))
                        return combined;
                }
            }
            else if (root.ValueKind == JsonValueKind.String)
            {
                return root.GetString();
            }
        }
        catch (JsonException)
        {
            // The server may return plain text instead of JSON.
        }

        return json.Trim();
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
            _http.Dispose();
    }
}

// -----------------------------------------------------------------------------
// WinForms-side DTOs. Keep these property names/types aligned with API Contracts.
// They are deliberately defined here so WinForms does not need a project
// reference to CoffeeChain.Api just to compile the HTTP client.
// -----------------------------------------------------------------------------

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string Token, string Email, string DisplayName, string Role, int? BranchId);

public sealed record BranchDto(int Id, string Name, string Address, bool IsOpen);
public sealed record BranchSaveRequest(string Name, string Address, bool IsOpen);
public sealed record BranchStatusRequest(bool IsOpen);

public sealed record BeverageDto(int Id, string Name, string Description, string Category, decimal BasePrice, bool IsAvailable);
public sealed record BeverageSaveRequest(string Name, string Description, string Category, decimal BasePrice, bool IsAvailable);

public sealed record ToppingDto(int Id, string Name, decimal Price, bool IsAvailable);
public sealed record ToppingSaveRequest(string Name, decimal Price, bool IsAvailable);

public sealed record MenuDto(BranchDto Branch, List<BeverageDto> Beverages, List<ToppingDto> Toppings);

public sealed record PlaceOrderRequest(int BranchId, List<OrderItemRequest> Items);
public sealed record OrderItemRequest(
    int BeverageId,
    string Size,
    int SugarPercent,
    int IcePercent,
    int Quantity,
    List<int>? ToppingIds);
public sealed record OrderItemToppingDto(int ToppingId, string Name, decimal UnitPrice);
public sealed record OrderItemDto(
    int BeverageId,
    string BeverageName,
    string Size,
    int SugarPercent,
    int IcePercent,
    int Quantity,
    decimal DrinkUnitPrice,
    decimal ToppingUnitPrice,
    decimal LineTotal,
    List<OrderItemToppingDto> Toppings);
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
    List<OrderItemDto> Items);
public sealed record UpdateOrderStatusRequest(string Status);