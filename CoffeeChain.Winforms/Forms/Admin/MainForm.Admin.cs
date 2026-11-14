using CoffeeChain.Winforms.Forms;
using System.Globalization;
using CoffeeChain.Winforms.Services;

namespace CoffeeChain.Winforms.Forms.Admin;

public partial class AdminMainForm : Form
{
    private readonly CoffeeApiClient? _api;
    private readonly LoginResponse? _user;
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
    private readonly TabPage _orderPage = new("Đơn hàng");

    private readonly DataGridView _beverageGrid = CreateGrid();
    private readonly DataGridView _toppingGrid = CreateGrid();
    private readonly DataGridView _branchGrid = CreateGrid();
    private readonly DataGridView _orderGrid = CreateGrid();
    private readonly Label _orderListStatus = new() { AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(12, 0, 0, 0) };
    private readonly TextBox _orderDetails = new()
    {
        Dock = DockStyle.Fill,
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical
    };
    private List<OrderDto> _orders = new();
    private bool _loadingOrders;

    private readonly TextBox _beverageName = new();
    private readonly TextBox _beverageDescription = new();
    private readonly TextBox _beverageCategory = new();
    private readonly TextBox _beveragePrice = new();
    private readonly CheckBox _beverageAvailable = new() { Text = "Đang bán", Checked = true, AutoSize = true };

    private readonly TextBox _toppingName = new();
    private readonly TextBox _toppingPrice = new();
    private readonly CheckBox _toppingAvailable = new() { Text = "Đang bán", Checked = true, AutoSize = true };

    private readonly TextBox _branchName = new();
    private readonly TextBox _branchAddress = new();
    private readonly CheckBox _branchOpen = new() { Text = "Đang mở cửa", Checked = true, AutoSize = true };

    private int? _selectedBeverageId;
    private int? _selectedToppingId;
    private int? _selectedBranchId;

    public AdminMainForm()
    {
        InitializeComponent();
        BuildManagementViews();
        WireNavigation();
        CoffeeTheme.Apply(this);
    }

    public AdminMainForm(CoffeeApiClient api, LoginResponse user) : this()
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _user = user ?? throw new ArgumentNullException(nameof(user));
        lbCustomer.Text = user.DisplayName;
        Shown += async (_, _) => await LoadAllAsync();

        var orderRefreshTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        orderRefreshTimer.Tick += async (_, _) =>
        {
            if (Visible && _tabs.SelectedTab == _orderPage)
                await LoadOrdersAsync();
        };
        Shown += (_, _) => orderRefreshTimer.Start();
        Disposed += (_, _) => orderRefreshTimer.Dispose();
    }

    private void BuildManagementViews()
    {
        pWindows.Controls.Clear();
        pWindows.Controls.Add(_tabs);

        var beveragePage = new TabPage("Đồ uống");
        var toppingPage = new TabPage("Topping");
        var branchPage = new TabPage("Chi nhánh");

        beveragePage.Controls.Add(CreateEditorLayout(_beverageGrid,
            MakeField("Tên đồ uống", _beverageName, 180),
            MakeField("Mô tả", _beverageDescription, 260),
            MakeField("Danh mục", _beverageCategory, 140),
            MakeField("Giá (VNĐ)", _beveragePrice, 110),
            _beverageAvailable,
            MakeButton("Thêm đồ uống", async () => await SaveBeverageAsync(false)),
            MakeButton("Cập nhật", async () => await SaveBeverageAsync(true)),
            MakeButton("Làm mới", async () => await LoadBeveragesAsync()),
            MakeButton("Xóa trắng", () => { ClearBeverageEditor(); return Task.CompletedTask; })));

        toppingPage.Controls.Add(CreateEditorLayout(_toppingGrid,
            MakeField("Tên topping", _toppingName, 240),
            MakeField("Giá (VNĐ)", _toppingPrice, 130),
            _toppingAvailable,
            MakeButton("Thêm topping", async () => await SaveToppingAsync(false)),
            MakeButton("Cập nhật", async () => await SaveToppingAsync(true)),
            MakeButton("Làm mới", async () => await LoadToppingsAsync()),
            MakeButton("Xóa trắng", () => { ClearToppingEditor(); return Task.CompletedTask; })));

        branchPage.Controls.Add(CreateEditorLayout(_branchGrid,
            MakeField("Tên chi nhánh", _branchName, 240),
            MakeField("Địa chỉ", _branchAddress, 330),
            _branchOpen,
            MakeButton("Thêm chi nhánh", async () => await SaveBranchAsync(false)),
            MakeButton("Cập nhật", async () => await SaveBranchAsync(true)),
            MakeButton("Đóng / Mở cửa", async () => await ToggleBranchStatusAsync()),
            MakeButton("Làm mới", async () => await LoadBranchesAsync()),
            MakeButton("Xóa trắng", () => { ClearBranchEditor(); return Task.CompletedTask; })));

        BuildOrderView();

        _tabs.TabPages.AddRange(new[] { beveragePage, toppingPage, branchPage, _orderPage });
        _tabs.SelectedIndexChanged += async (_, _) => await RefreshSelectedTabAsync();
    }

    private void BuildOrderView()
    {
        _orderGrid.AutoGenerateColumns = false;
        _orderGrid.Dock = DockStyle.Fill;
        _orderGrid.Columns.AddRange(
            OrderColumn(nameof(OrderDto.Code), "Mã đơn", 95),
            OrderColumn(nameof(OrderDto.BranchName), "Chi nhánh", 115),
            OrderColumn(nameof(OrderDto.CustomerName), "Khách hàng", 105),
            OrderColumn(nameof(OrderDto.CreatedAt), "Thời gian đặt", 105),
            OrderColumn(nameof(OrderDto.Status), "Trạng thái", 90),
            OrderColumn(nameof(OrderDto.Total), "Tổng tiền (VNĐ)", 90));
        _orderGrid.Columns[nameof(OrderDto.Total)].DefaultCellStyle.Format = "N0";
        _orderGrid.CurrentCellChanged += (_, _) => ShowSelectedOrderDetails();
        _orderGrid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            switch (_orderGrid.Columns[e.ColumnIndex].Name)
            {
                case nameof(OrderDto.CreatedAt) when e.Value is DateTimeOffset createdAt:
                    e.Value = createdAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                    e.FormattingApplied = true;
                    break;
                case nameof(OrderDto.Status) when e.Value is string status:
                    e.Value = ToVietnameseStatus(status);
                    e.FormattingApplied = true;
                    break;
            }
        };

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
        toolbar.Controls.Add(MakeButton("Làm mới", LoadOrdersAsync));
        toolbar.Controls.Add(_orderListStatus);

        var detailsGroup = new GroupBox { Text = "Chi tiết đơn hàng", Dock = DockStyle.Fill, Padding = new Padding(8) };
        detailsGroup.Controls.Add(_orderDetails);
        ShowSelectedOrderDetails();

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(_orderGrid, 0, 1);
        layout.Controls.Add(detailsGroup, 0, 2);
        _orderPage.Controls.Add(layout);
    }

    private static DataGridViewTextBoxColumn OrderColumn(string propertyName, string header, int weight) => new()
    {
        Name = propertyName,
        DataPropertyName = propertyName,
        HeaderText = header,
        FillWeight = weight,
        SortMode = DataGridViewColumnSortMode.NotSortable
    };

    private void WireNavigation()
    {
        // MenuStrip đã được bỏ khỏi giao diện.
        // TabControl là thành phần điều hướng chính nên không đăng ký sự kiện menu ở đây.
        _beverageGrid.SelectionChanged += (_, _) => SelectBeverageFromGrid();
        _toppingGrid.SelectionChanged += (_, _) => SelectToppingFromGrid();
        _branchGrid.SelectionChanged += (_, _) => SelectBranchFromGrid();
    }

    private async Task LoadAllAsync()
    {
        await LoadBeveragesAsync();
        await LoadToppingsAsync();
        await LoadBranchesAsync();
        await LoadOrdersAsync();
    }

    private async Task RefreshSelectedTabAsync()
    {
        switch (_tabs.SelectedIndex)
        {
            case 0: await LoadBeveragesAsync(); break;
            case 1: await LoadToppingsAsync(); break;
            case 2: await LoadBranchesAsync(); break;
            case 3: await LoadOrdersAsync(); break;
        }
    }

    private async Task LoadBeveragesAsync()
    {
        if (_api is null) return;
        try
        {
            var rows = await _api.GetBeveragesAsync();
            _beverageGrid.DataSource = null;
            _beverageGrid.DataSource = rows;
            SetHeader(_beverageGrid, "Id", "Mã");
            SetHeader(_beverageGrid, "Name", "Tên đồ uống");
            SetHeader(_beverageGrid, "Description", "Mô tả");
            SetHeader(_beverageGrid, "Category", "Danh mục");
            SetHeader(_beverageGrid, "IsAvailable", "Đang bán");
            FormatPriceColumn(_beverageGrid, nameof(BeverageDto.BasePrice));
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task LoadToppingsAsync()
    {
        if (_api is null) return;
        try
        {
            var rows = await _api.GetToppingsAsync();
            _toppingGrid.DataSource = null;
            _toppingGrid.DataSource = rows;
            SetHeader(_toppingGrid, "Id", "Mã");
            SetHeader(_toppingGrid, "Name", "Tên topping");
            SetHeader(_toppingGrid, "IsAvailable", "Đang bán");
            FormatPriceColumn(_toppingGrid, nameof(ToppingDto.Price));
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task LoadBranchesAsync()
    {
        if (_api is null) return;
        try
        {
            var rows = await _api.GetBranchesAsync();
            _branchGrid.DataSource = null;
            _branchGrid.DataSource = rows;
            SetHeader(_branchGrid, "Id", "Mã chi nhánh");
            SetHeader(_branchGrid, "Name", "Tên chi nhánh");
            SetHeader(_branchGrid, "Address", "Địa chỉ");
            SetHeader(_branchGrid, "IsOpen", "Đang mở cửa");
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task LoadOrdersAsync()
    {
        if (_api is null || _loadingOrders) return;
        _loadingOrders = true;
        try
        {
            var orders = await _api.GetAllOrdersAsync();
            if (IsDisposed || !Visible) return;
            if (!_orders.Select(order => (order.Id, order.Status)).SequenceEqual(
                orders.Select(order => (order.Id, order.Status))))
            {
                var selectedId = (_orderGrid.CurrentRow?.DataBoundItem as OrderDto)?.Id;
                _orders = orders;
                _orderGrid.DataSource = orders;
                var selectedRow = orders.FindIndex(order => order.Id == selectedId);
                if (selectedRow >= 0)
                    _orderGrid.CurrentCell = _orderGrid.Rows[selectedRow].Cells[0];
                ShowSelectedOrderDetails();
            }
            _orderListStatus.ForeColor = CoffeeTheme.Muted;
            _orderListStatus.Text = orders.Count == 0 ? "Chưa có đơn hàng" : $"{orders.Count} đơn hàng";
        }
        catch (Exception)
        {
            if (IsDisposed || !Visible) return;
            _orderListStatus.ForeColor = CoffeeTheme.Danger;
            _orderListStatus.Text = "Không tải được đơn hàng. Hãy thử làm mới.";
        }
        finally
        {
            _loadingOrders = false;
        }
    }

    private void ShowSelectedOrderDetails()
    {
        if (_orderGrid.CurrentRow?.DataBoundItem is not OrderDto order)
        {
            _orderDetails.Text = "Chọn một đơn hàng để xem các món.";
            return;
        }

        var lines = order.Items.Select(item =>
            $"{item.Quantity} × {item.BeverageName} (size {item.Size}, đường {item.SugarPercent}%, đá {item.IcePercent}%)" +
            $"   |   Topping: {(item.Toppings.Count == 0 ? "Không" : string.Join(", ", item.Toppings.Select(topping => topping.Name)))}" +
            $"   |   {item.LineTotal:N0} đ");
        _orderDetails.Text = $"{order.Code}   |   {ToVietnameseStatus(order.Status)}   |   Tổng tiền: {order.Total:N0} đ" +
            Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, lines);
    }

    private async Task SaveBeverageAsync(bool update)
    {
        if (_api is null) return;
        if (string.IsNullOrWhiteSpace(_beverageName.Text) ||
            !decimal.TryParse(_beveragePrice.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
        {
            MessageBox.Show("Vui lòng nhập tên và giá đồ uống hợp lệ.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var request = new BeverageSaveRequest(_beverageName.Text.Trim(), _beverageDescription.Text.Trim(),
                _beverageCategory.Text.Trim(), price, _beverageAvailable.Checked);
            if (update)
            {
                if (_selectedBeverageId is null) throw new InvalidOperationException("Hãy chọn đồ uống cần cập nhật.");
                await _api.UpdateBeverageAsync(_selectedBeverageId.Value, request);
            }
            else await _api.CreateBeverageAsync(request);

            ClearBeverageEditor();
            await LoadBeveragesAsync();
            MessageBox.Show("Đã lưu thông tin đồ uống.", "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task SaveToppingAsync(bool update)
    {
        if (_api is null) return;
        if (string.IsNullOrWhiteSpace(_toppingName.Text) ||
            !decimal.TryParse(_toppingPrice.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
        {
            MessageBox.Show("Vui lòng nhập tên và giá topping hợp lệ.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var request = new ToppingSaveRequest(_toppingName.Text.Trim(), price, _toppingAvailable.Checked);
            if (update)
            {
                if (_selectedToppingId is null) throw new InvalidOperationException("Hãy chọn topping cần cập nhật.");
                await _api.UpdateToppingAsync(_selectedToppingId.Value, request);
            }
            else await _api.CreateToppingAsync(request);

            ClearToppingEditor();
            await LoadToppingsAsync();
            MessageBox.Show("Đã lưu thông tin topping.", "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task SaveBranchAsync(bool update)
    {
        if (_api is null) return;
        if (string.IsNullOrWhiteSpace(_branchName.Text) || string.IsNullOrWhiteSpace(_branchAddress.Text))
        {
            MessageBox.Show("Vui lòng nhập tên và địa chỉ chi nhánh.", "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var request = new BranchSaveRequest(_branchName.Text.Trim(), _branchAddress.Text.Trim(), _branchOpen.Checked);
            if (update)
            {
                if (_selectedBranchId is null) throw new InvalidOperationException("Hãy chọn chi nhánh cần cập nhật.");
                await _api.UpdateBranchAsync(_selectedBranchId.Value, request);
            }
            else await _api.CreateBranchAsync(request);

            ClearBranchEditor();
            await LoadBranchesAsync();
            MessageBox.Show("Đã lưu thông tin chi nhánh.", "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task ToggleBranchStatusAsync()
    {
        if (_api is null || _selectedBranchId is null)
        {
            MessageBox.Show("Hãy chọn chi nhánh cần đóng hoặc mở cửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            await _api.SetBranchStatusAsync(_selectedBranchId.Value, !_branchOpen.Checked);
            await LoadBranchesAsync();
            MessageBox.Show("Đã cập nhật trạng thái chi nhánh.", "Coffee Chain", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void SelectBeverageFromGrid()
    {
        if (_beverageGrid.CurrentRow?.DataBoundItem is not BeverageDto item) return;
        _selectedBeverageId = item.Id;
        _beverageName.Text = item.Name;
        _beverageDescription.Text = item.Description;
        _beverageCategory.Text = item.Category;
        _beveragePrice.Text = item.BasePrice.ToString("0.##", CultureInfo.CurrentCulture);
        _beverageAvailable.Checked = item.IsAvailable;
    }

    private void SelectToppingFromGrid()
    {
        if (_toppingGrid.CurrentRow?.DataBoundItem is not ToppingDto item) return;
        _selectedToppingId = item.Id;
        _toppingName.Text = item.Name;
        _toppingPrice.Text = item.Price.ToString("0.##", CultureInfo.CurrentCulture);
        _toppingAvailable.Checked = item.IsAvailable;
    }

    private void SelectBranchFromGrid()
    {
        if (_branchGrid.CurrentRow?.DataBoundItem is not BranchDto item) return;
        _selectedBranchId = item.Id;
        _branchName.Text = item.Name;
        _branchAddress.Text = item.Address;
        _branchOpen.Checked = item.IsOpen;
    }

    private void ClearBeverageEditor()
    {
        _selectedBeverageId = null;
        _beverageName.Clear(); _beverageDescription.Clear(); _beverageCategory.Clear(); _beveragePrice.Clear();
        _beverageAvailable.Checked = true;
        _beverageGrid.ClearSelection();
    }

    private void ClearToppingEditor()
    {
        _selectedToppingId = null;
        _toppingName.Clear(); _toppingPrice.Clear(); _toppingAvailable.Checked = true;
        _toppingGrid.ClearSelection();
    }

    private void ClearBranchEditor()
    {
        _selectedBranchId = null;
        _branchName.Clear(); _branchAddress.Clear(); _branchOpen.Checked = true;
        _branchGrid.ClearSelection();
    }

    private static Control CreateEditorLayout(DataGridView grid, params Control[] controls)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(8)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 68));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 32));
        grid.Dock = DockStyle.Fill;
        grid.Margin = new Padding(3);
        var editor = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding(4)
        };
        foreach (var control in controls)
        {
            control.Margin = new Padding(5);
            if (control is Button button) button.Height = 42;
            editor.Controls.Add(control);
        }
        layout.Controls.Add(grid, 0, 0);
        layout.Controls.Add(editor, 0, 1);
        return layout;
    }

    private static GroupBox MakeField(string title, TextBox textBox, int width)
    {
        var group = new GroupBox { Text = title, Width = width, Height = 68, Padding = new Padding(5) };
        textBox.Dock = DockStyle.Fill;
        group.Controls.Add(textBox);
        return group;
    }

    private static Button MakeButton(string text, Func<Task> action)
    {
        var button = new Button
        {
            Text = text,
            Width = 125,
            Height = 38,
            BackColor = Color.FromArgb(128, 64, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        button.Click += async (_, _) =>
        {
            button.Enabled = false;
            try { await action(); }
            finally { button.Enabled = true; }
        };
        return button;
    }

    private static DataGridView CreateGrid() => new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        RowHeadersVisible = false,
        BackgroundColor = Color.White
    };

    private static void SetHeader(DataGridView grid, string propertyName, string headerText)
    {
        if (grid.Columns.Contains(propertyName))
            grid.Columns[propertyName].HeaderText = headerText;
    }

    private static void FormatPriceColumn(DataGridView grid, string propertyName)
    {
        if (grid.Columns.Contains(propertyName))
        {
            grid.Columns[propertyName].DefaultCellStyle.Format = "N0";
            grid.Columns[propertyName].HeaderText = "Giá (VNĐ)";
        }
    }

    private static string ToVietnameseStatus(string status) => status switch
    {
        "Pending" => "Chờ pha chế",
        "Preparing" => "Đang pha chế",
        "Ready" => "Sẵn sàng",
        "Completed" => "Hoàn thành",
        _ => status
    };

    private static void ShowError(Exception ex) =>
        MessageBox.Show(ex.Message, "Coffee Chain - Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

    private void AdminMainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DialogResult is DialogResult.OK or DialogResult.Retry) return;
        var result = MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Xác nhận thoát",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (result == DialogResult.OK) DialogResult = DialogResult.OK;
        else e.Cancel = true;
    }

    private void btnExit_Click(object sender, EventArgs e) => DialogResult = DialogResult.Retry;
}
