using CoffeeChain.Api;
using CoffeeChain.Winforms.Forms;
using CoffeeChain.Winforms.Forms.Admin;
using CoffeeChain.Winforms.Forms.Barista;
using CoffeeChain.Winforms.Services;
using Microsoft.AspNetCore.Builder;

namespace CoffeeChain.Winforms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        WebApplication? api = null;
        try
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("COFFEECHAIN_API_URL")))
            {
                // Keep the local API running while the user switches accounts.
                api = ApiHost.Build(Array.Empty<string>(), "http://127.0.0.1:5187");
                api.StartAsync().GetAwaiter().GetResult();
            }

            while (true)
            {
                using var apiClient = new CoffeeApiClient();
                LoginResponse loggedInUser;
                using (var loginForm = new LoginForm(apiClient))
                {
                    if (loginForm.ShowDialog() != DialogResult.OK || loginForm.CurrentUser is null)
                        return;

                    loggedInUser = loginForm.CurrentUser;
                }

                using Form mainForm = loggedInUser.Role switch
                {
                    "Admin" => new AdminMainForm(apiClient, loggedInUser),
                    "Barista" => new BaristaMainForm(apiClient, loggedInUser),
                    "Customer" => new CustomerMainForm(apiClient, loggedInUser),
                    _ => throw new InvalidOperationException("Vai trò tài khoản không được hỗ trợ.")
                };

                if (mainForm.ShowDialog() != DialogResult.Retry)
                    return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Không thể khởi động Coffee Chain.\n\n" + ex.Message +
                "\n\nHãy kiểm tra .NET 8 và cổng 5187 có đang được ứng dụng khác sử dụng không.",
                "Lỗi khởi động",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            if (api is not null)
            {
                try { api.StopAsync().GetAwaiter().GetResult(); }
                catch { /* Ignore shutdown errors. */ }
                try { api.DisposeAsync().AsTask().GetAwaiter().GetResult(); }
                catch { /* Ignore shutdown errors. */ }
            }
        }
    }
}
