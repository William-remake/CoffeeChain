using CoffeeChain.Winforms.Forms;
using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms.Forms.Barista;

public partial class BaristaMainForm : Form
{
    private readonly CoffeeApiClient? _api;
    private readonly LoginResponse? _user;
    private List<OrderDto> _orders = new();
    private bool _updatingOrder;

    public BaristaMainForm()
    {
        InitializeComponent();
        ConfigureControls();
        CoffeeTheme.Apply(this);
    }

    public BaristaMainForm(CoffeeApiClient api, LoginResponse user) : this()
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _user = user ?? throw new ArgumentNullException(nameof(user));
        lbCustomer.Text = user.DisplayName;
        Shown += async (_, _) => await LoadOrdersAsync();
    }

    private void ConfigureControls()
    {
        dgvBarista.ReadOnly = true;
        dgvBarista.AllowUserToAddRows = false;
        dgvBarista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBarista.MultiSelect = false;
        dgvBarista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        cbTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cbTrangThai.DisplayMember = nameof(StatusOption.Label);
        btnHistory.Click += async (_, _) => await LoadOrdersAsync();
        btnTrangThai.Click += async (_, _) => await UpdateSelectedOrderAsync();
        dgvBarista.SelectionChanged += (_, _) => UpdateAvailableStatuses();
        dgvBarista.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) ShowSelectedOrderDetails();
        };
    }

    private async Task LoadOrdersAsync()
    {
        if (_api is null) return;
        btnHistory.Enabled = false;
        try
        {
            _orders = await _api.GetBaristaOrdersAsync();
            dgvBarista.Rows.Clear();
            foreach (var order in _orders)
            {
                dgvBarista.Rows.Add(order.Code, order.BranchName, order.CustomerName,
                    order.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                    ToVietnameseStatus(order.Status), order.Total.ToString("N0") + " đ");
            }
            if (dgvBarista.Rows.Count > 0) dgvBarista.Rows[0].Selected = true;
            UpdateAvailableStatuses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Không tải được đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnHistory.Enabled = true;
        }
    }

    private async Task UpdateSelectedOrderAsync()
    {
        if (_updatingOrder) return;
        if (_api is null || dgvBarista.CurrentRow is null)
        {
            MessageBox.Show("Vui lòng chọn đơn hàng cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var code = Convert.ToString(dgvBarista.CurrentRow.Cells[0].Value);
        var order = _orders.FirstOrDefault(x => x.Code == code);
        if (order is null)
        {
            MessageBox.Show("Không tìm thấy đơn hàng đã chọn. Hãy làm mới danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cbTrangThai.SelectedItem is not StatusOption next)
        {
            MessageBox.Show("Đơn hàng không có trạng thái tiếp theo để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _updatingOrder = true;
        btnTrangThai.Enabled = false;
        btnExit.Enabled = false;
        try
        {
            var updated = await _api.UpdateOrderStatusAsync(order.Id, next.Value);
            await LoadOrdersAsync();
            MessageBox.Show(this, $"Đã cập nhật đơn {updated.Code} sang trạng thái {ToVietnameseStatus(updated.Status)}.",
                "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Không cập nhật được trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _updatingOrder = false;
            btnExit.Enabled = true;
            UpdateAvailableStatuses();
        }
    }

    private void UpdateAvailableStatuses()
    {
        cbTrangThai.Items.Clear();
        btnTrangThai.Enabled = false;
        if (dgvBarista.CurrentRow is null) return;

        var code = Convert.ToString(dgvBarista.CurrentRow.Cells[0].Value);
        var order = _orders.FirstOrDefault(x => x.Code == code);
        if (order is null) return;

        var next = order.Status switch
        {
            "Pending" => "Preparing",
            "Preparing" => "Ready",
            "Ready" => "Completed",
            _ => null
        };

        if (next is not null)
        {
            cbTrangThai.Items.Add(new StatusOption(next, ToVietnameseStatus(next)));
            cbTrangThai.SelectedIndex = 0;
            btnTrangThai.Enabled = !_updatingOrder;
        }
    }

    private void ShowSelectedOrderDetails()
    {
        if (dgvBarista.CurrentRow is null) return;
        var code = Convert.ToString(dgvBarista.CurrentRow.Cells[0].Value);
        var order = _orders.FirstOrDefault(x => x.Code == code);
        if (order is null) return;

        var lines = order.Items.Select(item =>
            $"- {item.BeverageName} x{item.Quantity} | Size {item.Size} | Đường {item.SugarPercent}% | Đá {item.IcePercent}%\n" +
            $"  Topping: {(item.Toppings.Count == 0 ? "Không" : string.Join(", ", item.Toppings.Select(t => t.Name)))}");

        MessageBox.Show(
            $"Mã đơn: {order.Code}\nKhách hàng: {order.CustomerName}\nChi nhánh: {order.BranchName}\n\n" +
            string.Join("\n", lines) + $"\n\nTổng tiền: {order.Total:N0} đ",
            "Chi tiết đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static string ToVietnameseStatus(string status) => status switch
    {
        "Pending" => "Chờ pha chế",
        "Preparing" => "Đang pha chế",
        "Ready" => "Sẵn sàng",
        "Completed" => "Hoàn thành",
        _ => status
    };

    private void BaristaMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_updatingOrder && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            return;
        }
        if (DialogResult is DialogResult.OK or DialogResult.Retry) return;
        var result = MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Xác nhận thoát",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (result == DialogResult.OK) DialogResult = DialogResult.OK;
        else e.Cancel = true;
    }

    private void btnExit_Click(object sender, EventArgs e) => DialogResult = DialogResult.Retry;

    private void lbTittle_Click(object sender, EventArgs e)
    {

    }

    private sealed record StatusOption(string Value, string Label);
}
