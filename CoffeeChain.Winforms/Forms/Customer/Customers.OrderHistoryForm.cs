using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms.Forms;

public partial class CustomersOrderHistoryForm : Form
{
    private readonly CoffeeApiClient? _api;

    public CustomersOrderHistoryForm()
    {
        InitializeComponent();
        ConfigureGrid();
        CoffeeTheme.Apply(this);
        btnHistory.Click += async (_, _) => await LoadHistoryAsync();
        Load += async (_, _) => await LoadHistoryAsync();
    }

    public CustomersOrderHistoryForm(CoffeeApiClient api) : this()
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    private void ConfigureGrid()
    {
        dgvHistory.ReadOnly = true;
        dgvHistory.AllowUserToAddRows = false;
        dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistory.MultiSelect = false;
        dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    private async Task LoadHistoryAsync()
    {
        if (_api is null) return;

        btnHistory.Enabled = false;
        try
        {
            var orders = await _api.GetOrderHistoryAsync();
            dgvHistory.Rows.Clear();

            var index = 1;
            foreach (var order in orders)
            {
                var itemText = string.Join(Environment.NewLine, order.Items.Select(x =>
                    $"{x.BeverageName} x{x.Quantity} ({x.Size})"));
                var toppingText = string.Join(Environment.NewLine, order.Items
                    .Where(x => x.Toppings.Count > 0)
                    .Select(x => $"{x.BeverageName}: {string.Join(", ", x.Toppings.Select(t => t.Name))}"));

                dgvHistory.Rows.Add(index++, $"{order.Code}\n{itemText}",
                    string.IsNullOrWhiteSpace(toppingText) ? "Không" : toppingText,
                    order.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                    ToVietnameseStatus(order.Status), order.Total.ToString("N0") + " đ");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Không tải được lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnHistory.Enabled = true;
        }
    }

    private static string ToVietnameseStatus(string status) => status switch
    {
        "Pending" => "Chờ xác nhận",
        "Preparing" => "Đang pha chế",
        "Ready" => "Sẵn sàng",
        "Completed" => "Hoàn thành",
        _ => status
    };
}
