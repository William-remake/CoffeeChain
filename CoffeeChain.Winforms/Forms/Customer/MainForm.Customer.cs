using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms.Forms;

public partial class CustomerMainForm : Form
{
    private readonly CoffeeApiClient? _api;
    private readonly LoginResponse? _user;
    private readonly Panel _homePanel = new()
    {
        Location = new Point(12, 155),
        Size = new Size(1386, 622),
        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
        BackColor = Color.FromArgb(250, 248, 244)
    };

    public CustomerMainForm()
    {
        InitializeComponent();
        BuildHomePanel();
        CoffeeTheme.Apply(this);
    }

    public CustomerMainForm(CoffeeApiClient api, LoginResponse user) : this()
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _user = user ?? throw new ArgumentNullException(nameof(user));
        lbCustomer.Text = user.DisplayName;
    }

    private void BuildHomePanel()
    {
        var title = new Label
        {
            Text = "Chào mừng đến với Coffee Chain",
            Font = new Font("Segoe UI", 25F, FontStyle.Bold),
            ForeColor = Color.FromArgb(95, 51, 20),
            AutoSize = true,
            Location = new Point(55, 55)
        };

        var description = new Label
        {
            Text = "Chọn chi nhánh yêu thích, tùy chỉnh đồ uống và theo dõi đơn hàng của bạn.",
            Font = new Font("Segoe UI", 12F),
            ForeColor = Color.FromArgb(90, 85, 80),
            AutoSize = true,
            Location = new Point(59, 112)
        };

        var orderButton = CreateHomeButton("ĐẶT MÓN NGAY", new Point(60, 205));
        orderButton.Click += (_, _) => OpenOrderForm();

        var historyButton = CreateHomeButton("LỊCH SỬ ĐẶT MÓN", new Point(330, 205));
        historyButton.Click += (_, _) => OpenOrderHistoryForm();

        var note = new Label
        {
            Text = "Gợi ý: Bạn có thể chọn size S / M / L, điều chỉnh lượng đường – đá và thêm nhiều topping cho từng món.",
            Font = new Font("Segoe UI", 10F, FontStyle.Italic),
            ForeColor = Color.DimGray,
            AutoSize = true,
            Location = new Point(60, 300)
        };

        _homePanel.Controls.AddRange(new Control[] { title, description, orderButton, historyButton, note });
        Controls.Add(_homePanel);
        _homePanel.BringToFront();
    }

    private static Button CreateHomeButton(string text, Point location) => new()
    {
        Text = text,
        Location = location,
        Size = new Size(235, 72),
        BackColor = Color.FromArgb(128, 64, 0),
        ForeColor = Color.White,
        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
        FlatStyle = FlatStyle.Flat,
        Cursor = Cursors.Hand
    };

    private void CustomerMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DialogResult is DialogResult.OK or DialogResult.Retry) return;
        var result = MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Xác nhận thoát",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (result == DialogResult.OK) DialogResult = DialogResult.OK;
        else e.Cancel = true;
    }

    private void btnExit_Click(object sender, EventArgs e) => DialogResult = DialogResult.Retry;

    private void OpenOrderForm()
    {
        if (_api is null || _user is null) return;
        using var orderForm = new CustomerOrderForm(_api, _user);
        orderForm.ShowDialog(this);
    }

    private void OpenOrderHistoryForm()
    {
        if (_api is null) return;
        using var historyForm = new CustomersOrderHistoryForm(_api);
        historyForm.ShowDialog(this);
    }
}
