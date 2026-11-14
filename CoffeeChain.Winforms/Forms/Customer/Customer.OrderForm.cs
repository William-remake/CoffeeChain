using System.Data;
using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms.Forms;

public partial class CustomerOrderForm : Form
{
    private readonly CoffeeApiClient? _api;
    private readonly LoginResponse? _user;
    private readonly DataTable _cartTable = new();
    private readonly List<CartLine> _cart = new();
    private readonly Label _cartTotalLabel = new()
    {
        Text = "Tổng cộng: 0 đ",
        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor = Color.FromArgb(128, 64, 0),
        AutoSize = true,
        Location = new Point(205, 495)
    };
    private bool _loadingBranches;
    private bool _placingOrder;

    public CustomerOrderForm()
    {
        InitializeComponent();
        ConfigureControls();
        btnThanhToan.Click += async (_, _) => await CheckoutAsync();
        CoffeeTheme.Apply(this);
    }

    public CustomerOrderForm(CoffeeApiClient api, LoginResponse user) : this()
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _user = user ?? throw new ArgumentNullException(nameof(user));
    }

    private void ConfigureControls()
    {
        cboSize.Items.Clear();
        cboSize.Items.AddRange(new object[] { "S", "M", "L" });
        cboSize.SelectedIndex = 1;

        cboDuong.Items.Clear();
        cboDuong.Items.AddRange(new object[] { "0", "25", "50", "75", "100" });
        cboDuong.SelectedItem = "50";

        cboDa.Items.Clear();
        cboDa.Items.AddRange(new object[] { "0", "25", "50", "75", "100" });
        cboDa.SelectedItem = "50";

        numSoLuong.Minimum = 1;
        numSoLuong.Maximum = 20;
        numSoLuong.Value = 1;

        _cartTable.Columns.Add("STT", typeof(int));
        _cartTable.Columns.Add("DoUong", typeof(string));
        _cartTable.Columns.Add("Size", typeof(string));
        _cartTable.Columns.Add("Duong", typeof(string));
        _cartTable.Columns.Add("Da", typeof(string));
        _cartTable.Columns.Add("SoLuong", typeof(int));
        _cartTable.Columns.Add("Topping", typeof(string));
        _cartTable.Columns.Add("ThanhTien", typeof(decimal));

        dgvGioHang.DataSource = _cartTable;
        dgvGioHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvGioHang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvGioHang.MultiSelect = false;
        dgvGioHang.AllowUserToAddRows = false;
        dgvGioHang.ReadOnly = true;
        foreach (DataGridViewColumn column in dgvGioHang.Columns)
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        gbGioHang.Controls.Add(_cartTotalLabel);
    }

    private async void CustomerOrderForm_Load(object? sender, EventArgs e)
    {
        if (_api is null)
        {
            lbTrangThai.Text = "Chưa kết nối API";
            return;
        }

        await LoadBranchesAsync();
    }

    private async Task LoadBranchesAsync()
    {
        try
        {
            _loadingBranches = true;
            var branches = await _api!.GetBranchesAsync();
            cboChiNhanh.DataSource = null;
            cboChiNhanh.DisplayMember = nameof(BranchDto.Name);
            cboChiNhanh.ValueMember = nameof(BranchDto.Id);
            cboChiNhanh.DataSource = branches;
            _loadingBranches = false;

            if (branches.Count == 0)
            {
                lbTrangThai.Text = "Chưa có chi nhánh";
                return;
            }

            cboChiNhanh.SelectedIndexChanged -= cboChiNhanh_SelectedIndexChanged;
            cboChiNhanh.SelectedIndexChanged += cboChiNhanh_SelectedIndexChanged;
            await LoadSelectedBranchMenuAsync();
        }
        catch (Exception ex)
        {
            _loadingBranches = false;
            ShowError("Không tải được danh sách chi nhánh", ex);
        }
    }

    private async void cboChiNhanh_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_loadingBranches) return;
        await LoadSelectedBranchMenuAsync();
    }

    private async Task LoadSelectedBranchMenuAsync()
    {
        if (_api is null || cboChiNhanh.SelectedItem is not BranchDto branch) return;

        try
        {
            var menu = await _api.GetMenuAsync(branch.Id);
            lbTrangThai.Text = menu.Branch.IsOpen ? "● Đang mở cửa" : "● Đang đóng cửa";
            lbTrangThai.ForeColor = menu.Branch.IsOpen ? Color.ForestGreen : Color.Firebrick;

            cboDoUong.DataSource = null;
            cboDoUong.DisplayMember = nameof(BeverageDto.Name);
            cboDoUong.ValueMember = nameof(BeverageDto.Id);
            cboDoUong.DataSource = menu.Beverages;

            chklistTopping.Items.Clear();
            chklistTopping.DisplayMember = nameof(ToppingDto.Name);
            foreach (var topping in menu.Toppings)
                chklistTopping.Items.Add(topping, false);

            btnThanhToan.Enabled = menu.Branch.IsOpen && !_placingOrder;
        }
        catch (Exception ex)
        {
            ShowError("Không tải được menu chi nhánh", ex);
        }
    }

    private void btnAdd_Click(object? sender, EventArgs e)
    {
        if (cboDoUong.SelectedItem is not BeverageDto beverage)
        {
            MessageBox.Show("Chi nhánh chưa có đồ uống đang bán.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var size = cboSize.SelectedItem?.ToString() ?? "M";
        var sugar = int.TryParse(cboDuong.SelectedItem?.ToString(), out var sugarValue) ? sugarValue : 50;
        var ice = int.TryParse(cboDa.SelectedItem?.ToString(), out var iceValue) ? iceValue : 50;
        var quantity = (int)numSoLuong.Value;
        var selectedToppings = chklistTopping.CheckedItems.Cast<ToppingDto>().ToList();
        var toppingIds = selectedToppings.Select(x => x.Id).ToList();
        var sizeExtra = size switch { "M" => 5000m, "L" => 10000m, _ => 0m };
        var lineTotal = (beverage.BasePrice + sizeExtra + selectedToppings.Sum(x => x.Price)) * quantity;

        _cart.Add(new CartLine(
            beverage.Id, beverage.Name, size, sugar, ice, quantity,
            toppingIds, string.Join(", ", selectedToppings.Select(x => x.Name)), lineTotal));
        RefreshCartTable();

        numSoLuong.Value = 1;
        for (var i = 0; i < chklistTopping.Items.Count; i++) chklistTopping.SetItemChecked(i, false);
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        if (dgvGioHang.CurrentRow is null || dgvGioHang.CurrentRow.Index < 0 ||
            dgvGioHang.CurrentRow.Index >= _cart.Count)
        {
            MessageBox.Show("Vui lòng chọn món cần xóa.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _cart.RemoveAt(dgvGioHang.CurrentRow.Index);
        RefreshCartTable();
    }

    private void RefreshCartTable()
    {
        _cartTable.Rows.Clear();
        for (var i = 0; i < _cart.Count; i++)
        {
            var item = _cart[i];
            _cartTable.Rows.Add(i + 1, item.BeverageName, item.Size,
                $"{item.SugarPercent}%", $"{item.IcePercent}%", item.Quantity,
                string.IsNullOrWhiteSpace(item.ToppingNames) ? "Không" : item.ToppingNames,
                item.LineTotal);
        }

        if (dgvGioHang.Columns.Contains("ThanhTien"))
            dgvGioHang.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
        _cartTotalLabel.Text = $"Tổng cộng: {_cart.Sum(x => x.LineTotal):N0} đ";
    }

    private async Task CheckoutAsync()
    {
        if (_placingOrder || _api is null || cboChiNhanh.SelectedItem is not BranchDto branch) return;
        if (_cart.Count == 0)
        {
            MessageBox.Show("Giỏ hàng đang trống.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!branch.IsOpen)
        {
            MessageBox.Show("Chi nhánh đang đóng cửa, không thể đặt món.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _placingOrder = true;
        btnThanhToan.Enabled = false;
        btnAdd.Enabled = false;
        btnXoa.Enabled = false;
        cboChiNhanh.Enabled = false;
        try
        {
            var request = new PlaceOrderRequest(branch.Id, _cart.Select(item => new OrderItemRequest(
                item.BeverageId, item.Size, item.SugarPercent, item.IcePercent,
                item.Quantity, item.ToppingIds)).ToList());

            var order = await _api.PlaceOrderAsync(request);
            _cart.Clear();
            RefreshCartTable();
            MessageBox.Show(this,
                $"Đã lưu đơn hàng.\nMã đơn: {order.Code}\nTổng tiền: {order.Total:N0} đ",
                "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Không thể thanh toán đơn hàng", ex);
        }
        finally
        {
            _placingOrder = false;
            btnThanhToan.Enabled = branch.IsOpen;
            btnAdd.Enabled = true;
            btnXoa.Enabled = true;
            cboChiNhanh.Enabled = true;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (_placingOrder && e.CloseReason == CloseReason.UserClosing)
            e.Cancel = true;
    }

    private static void ShowError(string title, Exception ex) =>
        MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private sealed record CartLine(
        int BeverageId,
        string BeverageName,
        string Size,
        int SugarPercent,
        int IcePercent,
        int Quantity,
        List<int> ToppingIds,
        string ToppingNames,
        decimal LineTotal);
}
