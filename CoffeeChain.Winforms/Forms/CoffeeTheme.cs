using System.Drawing.Drawing2D;

namespace CoffeeChain.Winforms.Forms;

/// <summary>
/// Giao diện dùng chung cho các màn hình Coffee Chain.
/// Chỉ thay đổi thuộc tính hiển thị, không can thiệp vào dữ liệu hoặc sự kiện nghiệp vụ.
/// LoginForm được cố ý loại khỏi phạm vi áp dụng.
/// </summary>
internal static class CoffeeTheme
{
    internal static readonly Color Page = Color.FromArgb(247, 244, 239);
    internal static readonly Color Surface = Color.White;
    internal static readonly Color Primary = Color.FromArgb(91, 59, 40);
    internal static readonly Color PrimaryDark = Color.FromArgb(60, 40, 29);
    internal static readonly Color Accent = Color.FromArgb(185, 133, 86);
    internal static readonly Color Text = Color.FromArgb(48, 42, 36);
    internal static readonly Color Muted = Color.FromArgb(117, 105, 93);
    internal static readonly Color Border = Color.FromArgb(230, 222, 212);
    internal static readonly Color Selected = Color.FromArgb(239, 226, 211);
    internal static readonly Color Danger = Color.FromArgb(166, 66, 57);
    internal static readonly Color DangerLight = Color.FromArgb(250, 235, 232);

    internal static void Apply(Form form)
    {
        if (form is null) return;

        form.BackColor = Page;
        form.ForeColor = Text;
        form.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);

        foreach (Control control in form.Controls)
            ApplyControl(control);

        form.PerformLayout();
    }

    private static void ApplyControl(Control control)
    {
        switch (control)
        {
            case DataGridView grid:
                StyleGrid(grid);
                break;

            case TabControl tabs:
                StyleTabs(tabs);
                break;

            case Button button:
                StyleButton(button);
                break;

            case Label label:
                StyleLabel(label);
                break;

            case GroupBox group:
                group.ForeColor = Primary;
                group.BackColor = Surface;
                group.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
                break;

            case TextBox textBox:
                textBox.BackColor = Surface;
                textBox.ForeColor = Text;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;

            case ComboBox combo:
                combo.BackColor = Surface;
                combo.ForeColor = Text;
                combo.FlatStyle = FlatStyle.Flat;
                combo.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;

            case NumericUpDown numeric:
                numeric.BackColor = Surface;
                numeric.ForeColor = Text;
                numeric.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;

            case CheckedListBox checkedList:
                checkedList.BackColor = Surface;
                checkedList.ForeColor = Text;
                checkedList.BorderStyle = BorderStyle.FixedSingle;
                checkedList.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;

            case CheckBox checkBox:
                checkBox.ForeColor = Text;
                checkBox.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;

            case TabPage page:
                page.BackColor = Page;
                page.ForeColor = Text;
                page.Padding = new Padding(4);
                break;

            case TableLayoutPanel table:
                table.BackColor = Page;
                break;

            case FlowLayoutPanel flow:
                flow.BackColor = Page;
                break;

            case Panel panel:
                StylePanel(panel);
                break;

            case ListView listView:
                listView.BackColor = Surface;
                listView.ForeColor = Text;
                listView.BorderStyle = BorderStyle.FixedSingle;
                listView.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                break;
        }

        if (control is ToolStrip toolStrip)
            StyleToolStrip(toolStrip);

        foreach (Control child in control.Controls)
            ApplyControl(child);
    }

    private static void StylePanel(Panel panel)
    {
        panel.BackColor = panel.Name switch
        {
            "pBackGroundTittle" => PrimaryDark,
            "pWindows" => Page,
            "panel1" => Color.FromArgb(241, 234, 224),
            _ => Surface
        };
    }

    private static void StyleLabel(Label label)
    {
        var inHeader = label.Parent?.Name == "pBackGroundTittle";
        label.ForeColor = inHeader ? Color.White : Text;
        label.Font = new Font("Segoe UI", Math.Max(9F, label.Font.SizeInPoints), label.Font.Style, GraphicsUnit.Point);

        if (label.Name == "lbTittle")
            label.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
        else if (label.Name == "lbCustomer")
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
    }

    private static void StyleButton(Button button)
    {
        var text = button.Text.Trim().ToLowerInvariant();
        var isExit = text.Contains("thoát") || text == "đăng xuất";
        var isDelete = text.Contains("xóa") || text.Contains("xoá");
        var isRefresh = text.Contains("làm mới");

        var backColor = isExit
            ? Color.FromArgb(235, 226, 216)
            : isDelete
                ? DangerLight
                : isRefresh
                    ? Color.FromArgb(226, 216, 204)
                    : Primary;

        var foreColor = isExit || isRefresh ? Primary : isDelete ? Danger : Color.White;

        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.10F);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.06F);
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.UseVisualStyleBackColor = false;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI", Math.Max(9F, button.Font.SizeInPoints), FontStyle.Bold, GraphicsUnit.Point);

        // Giữ nguyên mọi Click handler hiện có; chỉ thêm hiệu ứng hover.
        button.MouseEnter += (_, _) => button.BackColor = ControlPaint.Light(backColor, 0.10F);
        button.MouseLeave += (_, _) => button.BackColor = backColor;
    }

    private static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.GridColor = Border;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersHeight = 42;
        grid.RowTemplate.Height = 38;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
        grid.RowHeadersVisible = false;
        grid.AllowUserToResizeRows = false;

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Primary,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 8, 0),
            SelectionBackColor = Primary,
            SelectionForeColor = Color.White,
            WrapMode = DataGridViewTriState.False
        };

        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Surface,
            ForeColor = Text,
            Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Point),
            SelectionBackColor = Selected,
            SelectionForeColor = Text,
            Padding = new Padding(8, 3, 8, 3),
            WrapMode = DataGridViewTriState.True
        };

        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(250, 248, 245),
            ForeColor = Text,
            SelectionBackColor = Selected,
            SelectionForeColor = Text
        };

        foreach (DataGridViewColumn column in grid.Columns)
        {
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            var header = column.HeaderText.ToLowerInvariant();
            if (header.Contains("giá") || header.Contains("tiền") || header.Contains("tổng"))
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // Nhấn màu nhẹ cho trạng thái đơn, không thay đổi dữ liệu hiển thị.
        grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var header = grid.Columns[e.ColumnIndex].HeaderText.ToLowerInvariant();
            if (!header.Contains("trạng thái")) return;

            var status = Convert.ToString(e.Value)?.Trim().ToLowerInvariant();
            var (back, fore) = status switch
            {
                "chờ pha chế" or "chờ xác nhận" => (Color.FromArgb(255, 244, 219), Color.FromArgb(139, 94, 24)),
                "đang pha chế" => (Color.FromArgb(225, 238, 250), Color.FromArgb(45, 94, 139)),
                "sẵn sàng" => (Color.FromArgb(224, 242, 229), Color.FromArgb(42, 112, 66)),
                "hoàn thành" => (Color.FromArgb(235, 235, 235), Color.FromArgb(80, 80, 80)),
                _ => (Color.Empty, Color.Empty)
            };

            if (back != Color.Empty && e.CellStyle is { } cellStyle)
            {
                cellStyle.BackColor = back;
                cellStyle.ForeColor = fore;
                cellStyle.Font = grid.ColumnHeadersDefaultCellStyle.Font;
                e.FormattingApplied = true;
            }
        };
    }

    private static void StyleTabs(TabControl tabs)
    {
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.ItemSize = new Size(126, 38);
        tabs.Padding = new Point(14, 7);
        tabs.BackColor = Page;
        tabs.DrawItem += (_, e) =>
        {
            if (e.Index < 0 || e.Index >= tabs.TabPages.Count) return;

            var selected = e.Index == tabs.SelectedIndex;
            var bounds = e.Bounds;
            bounds.Inflate(-2, -2);
            using var brush = new SolidBrush(selected ? Primary : Color.FromArgb(232, 224, 214));
            using var path = CreateRoundedRectangle(bounds, 7);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(brush, path);

            var textColor = selected ? Color.White : Primary;
            using var tabFont = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            TextRenderer.DrawText(
                e.Graphics,
                tabs.TabPages[e.Index].Text,
                tabFont,
                bounds,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
    {
        var diameter = radius * 2;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private static void StyleToolStrip(ToolStrip strip)
    {
        strip.BackColor = PrimaryDark;
        strip.ForeColor = Color.White;
        strip.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        strip.GripStyle = ToolStripGripStyle.Hidden;
        strip.RenderMode = ToolStripRenderMode.System;

        foreach (ToolStripItem item in strip.Items)
            StyleToolStripItem(item);
    }

    private static void StyleToolStripItem(ToolStripItem item)
    {
        item.ForeColor = Color.White;
        item.BackColor = PrimaryDark;
        item.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);

        if (item is ToolStripMenuItem menuItem)
        {
            foreach (ToolStripItem child in menuItem.DropDownItems)
            {
                child.ForeColor = Text;
                child.BackColor = Surface;
                child.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                if (child is ToolStripMenuItem childMenu)
                    StyleToolStripDropDown(childMenu);
            }
        }
    }

    private static void StyleToolStripDropDown(ToolStripMenuItem menuItem)
    {
        foreach (ToolStripItem child in menuItem.DropDownItems)
        {
            child.ForeColor = Text;
            child.BackColor = Surface;
            child.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            if (child is ToolStripMenuItem childMenu)
                StyleToolStripDropDown(childMenu);
        }
    }
}
