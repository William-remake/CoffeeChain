using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms;

public partial class LoginForm : Form
{
    private readonly CoffeeApiClient _api;
    private readonly Label _accountsLabel = new();

    public LoginResponse? CurrentUser { get; private set; }

    public LoginForm(CoffeeApiClient api)
    {
        InitializeComponent();
        _api = api ?? throw new ArgumentNullException(nameof(api));

        txtPassWord.UseSystemPasswordChar = true;

        txtEmail.Text = "customer@coffeechain.local";
        txtPassWord.Text = "Customer@123";
        AcceptButton = btnLogin;
    }

    private async Task LoginAsync()
    {
        var email = txtEmail.Text.Trim();
        var password = txtPassWord.Text;

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowValidation("Vui lòng nhập email.");
            txtEmail.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowValidation("Vui lòng nhập mật khẩu.");
            txtPassWord.Focus();
            return;
        }

        btnLogin.Enabled = false;
        lbVerity.ForeColor = Color.DimGray;
        lbVerity.Text = "Đang kết nối API và xác thực...";

        try
        {
            var response = await _api.LoginAsync(new LoginRequest(email, password));
            if (string.IsNullOrWhiteSpace(response.Token))
                throw new InvalidOperationException("API không trả về token đăng nhập.");

            _api.SetToken(response.Token);
            CurrentUser = response;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (HttpRequestException ex)
        {
            ShowValidation("Không kết nối được CoffeeChain API.");
            MessageBox.Show(
                $"Không thể gọi API tại:\n{_api.ApiBaseAddress}\n\nChi tiết: {ex.Message}",
                "Không kết nối được API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (TaskCanceledException)
        {
            ShowValidation("API phản hồi quá lâu. Vui lòng thử lại.");
        }
        catch (Exception ex)
        {
            ShowValidation(ex.Message);
        }
        finally
        {
            if (DialogResult != DialogResult.OK) btnLogin.Enabled = true;
        }
    }

    private void ShowValidation(string message)
    {
        lbVerity.ForeColor = Color.Firebrick;
        lbVerity.Text = message;
    }

    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.OK) return;

        var result = MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Xác nhận thoát",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (result != DialogResult.OK) e.Cancel = true;
    }

    private void btnExit_Click(object sender, EventArgs e) => Close();

    private async void btnLogin_Click(object? sender, EventArgs e) => await LoginAsync();
}
