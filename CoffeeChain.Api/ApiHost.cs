using CoffeeChain.Api.Contracts.Auth;
using CoffeeChain.Api.Contracts.Beverages;
using CoffeeChain.Api.Contracts.Branches;
using CoffeeChain.Api.Contracts.Orders;
using CoffeeChain.Api.Contracts.Toppings;
using CoffeeChain.Api.Security;
using CoffeeChain.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeChain.Api;

public static class ApiHost
{
    public static WebApplication Build(string[] args, string url)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(ApiHost).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.WebHost.UseUrls(url);
        builder.Services.AddSingleton(_ => new CoffeeChainStore(builder.Configuration["CoffeeChain:OrdersPath"]));

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = ex is InvalidOperationException
                    ? StatusCodes.Status400BadRequest
                    : StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = context.Response.StatusCode == StatusCodes.Status400BadRequest
                        ? ex.Message
                        : "Đã xảy ra lỗi nội bộ trong API."
                });
            }
        });

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;
            var isPublic = string.Equals(path.Value, "/api/health", StringComparison.OrdinalIgnoreCase) ||
                           (HttpMethods.IsPost(context.Request.Method) && string.Equals(path.Value, "/api/auth/login", StringComparison.OrdinalIgnoreCase));

            if (path.StartsWithSegments("/api") && !isPublic)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authHeader[7..].Trim()
                    : string.Empty;

                var store = context.RequestServices.GetRequiredService<CoffeeChainStore>();
                if (token.Length == 0 || !store.TryGetUser(token, out var user))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hiệu lực." });
                    return;
                }

                context.Items["CurrentUser"] = user;
            }

            await next();
        });

        MapEndpoints(app);
        return app;
    }

    private static void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/health", () => Results.Ok(new { status = "ok", application = "Coffee Chain API" }));

        app.MapPost("/api/auth/login", (LoginRequest request, CoffeeChainStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest(new { message = "Vui lòng nhập email và mật khẩu." });

            var result = store.Login(request.Email, request.Password);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        });

        app.MapGet("/api/branches", (CoffeeChainStore store) => Results.Ok(store.GetBranches()));
        app.MapGet("/api/admin/branches", (HttpContext context, CoffeeChainStore store) =>
            IsRole(context, "Admin") ? Results.Ok(store.GetBranches()) : Forbidden());

        app.MapGet("/api/branches/{branchId:int}/menu", (int branchId, CoffeeChainStore store) =>
        {
            var menu = store.GetMenu(branchId);
            return menu is null
                ? Results.NotFound(new { message = "Không tìm thấy chi nhánh." })
                : Results.Ok(menu);
        });

        app.MapPost("/api/orders", (HttpContext context, PlaceOrderRequest request, CoffeeChainStore store) =>
        {
            var user = CurrentUser(context);
            if (user.Role != "Customer") return Forbidden();
            try { return Results.Ok(store.PlaceOrder(user, request)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapGet("/api/orders/history", (HttpContext context, CoffeeChainStore store) =>
        {
            var user = CurrentUser(context);
            return user.Role != "Customer" ? Forbidden() : Results.Ok(store.GetCustomerOrders(user.Id));
        });

        app.MapGet("/api/barista/orders", (HttpContext context, CoffeeChainStore store) =>
        {
            var user = CurrentUser(context);
            if (user.Role != "Barista" || user.BranchId is null) return Forbidden();
            return Results.Ok(store.GetBaristaOrders(user.BranchId.Value));
        });

        app.MapPatch("/api/barista/orders/{orderId:int}/status", (int orderId, HttpContext context, UpdateOrderStatusRequest request, CoffeeChainStore store) =>
        {
            var user = CurrentUser(context);
            if (user.Role != "Barista") return Forbidden();
            try { return Results.Ok(store.UpdateOrderStatus(orderId, user, request.Status)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapGet("/api/admin/orders", (HttpContext context, CoffeeChainStore store) =>
            IsRole(context, "Admin") ? Results.Ok(store.GetAllOrders()) : Forbidden());

        app.MapGet("/api/admin/beverages", (HttpContext context, CoffeeChainStore store) =>
            IsRole(context, "Admin") ? Results.Ok(store.GetBeverages()) : Forbidden());

        app.MapPost("/api/admin/beverages", (HttpContext context, BeverageSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveBeverage(request)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapPut("/api/admin/beverages/{id:int}", (int id, HttpContext context, BeverageSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveBeverage(request, id)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapGet("/api/admin/toppings", (HttpContext context, CoffeeChainStore store) =>
            IsRole(context, "Admin") ? Results.Ok(store.GetToppings()) : Forbidden());

        app.MapPost("/api/admin/toppings", (HttpContext context, ToppingSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveTopping(request)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapPut("/api/admin/toppings/{id:int}", (int id, HttpContext context, ToppingSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveTopping(request, id)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapPost("/api/admin/branches", (HttpContext context, BranchSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveBranch(request)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapPut("/api/admin/branches/{id:int}", (int id, HttpContext context, BranchSaveRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SaveBranch(request, id)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });

        app.MapPatch("/api/admin/branches/{id:int}/status", (int id, HttpContext context, BranchStatusRequest request, CoffeeChainStore store) =>
        {
            if (!IsRole(context, "Admin")) return Forbidden();
            try { return Results.Ok(store.SetBranchStatus(id, request.IsOpen)); }
            catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); }
        });
    }

    private static bool IsRole(HttpContext context, string role) =>
        string.Equals(CurrentUser(context).Role, role, StringComparison.OrdinalIgnoreCase);

    private static AuthenticatedUser CurrentUser(HttpContext context) =>
        (AuthenticatedUser)context.Items["CurrentUser"]!;

    private static IResult Forbidden() =>
        Results.Json(new { message = "Bạn không có quyền thực hiện chức năng này." }, statusCode: StatusCodes.Status403Forbidden);
}
