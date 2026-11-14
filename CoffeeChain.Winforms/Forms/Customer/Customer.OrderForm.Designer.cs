namespace CoffeeChain.Winforms.Forms
{
    partial class CustomerOrderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            lbTrangThai = new Label();
            cboChiNhanh = new ComboBox();
            lbAddress = new Label();
            panel2 = new Panel();
            gbDoUong = new GroupBox();
            btnAdd = new Button();
            chklistTopping = new CheckedListBox();
            lbTopping = new Label();
            numSoLuong = new NumericUpDown();
            lbSoLuong = new Label();
            cboDa = new ComboBox();
            lbIce = new Label();
            cboDuong = new ComboBox();
            lbSugar = new Label();
            cboSize = new ComboBox();
            lbSize = new Label();
            cboDoUong = new ComboBox();
            lbDirnk = new Label();
            panel3 = new Panel();
            gbGioHang = new GroupBox();
            btnThanhToan = new Button();
            btnXoa = new Button();
            dgvGioHang = new DataGridView();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            gbDoUong.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSoLuong).BeginInit();
            panel3.SuspendLayout();
            gbGioHang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGioHang).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.SeaShell;
            panel1.Controls.Add(lbTrangThai);
            panel1.Controls.Add(cboChiNhanh);
            panel1.Controls.Add(lbAddress);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(1032, 60);
            panel1.TabIndex = 0;
            // 
            // lbTrangThai
            // 
            lbTrangThai.AutoSize = true;
            lbTrangThai.Location = new Point(523, 20);
            lbTrangThai.Name = "lbTrangThai";
            lbTrangThai.Size = new Size(92, 25);
            lbTrangThai.TabIndex = 4;
            lbTrangThai.Text = "Trạng Thái";
            // 
            // cboChiNhanh
            // 
            cboChiNhanh.FormattingEnabled = true;
            cboChiNhanh.Location = new Point(216, 17);
            cboChiNhanh.Name = "cboChiNhanh";
            cboChiNhanh.Size = new Size(287, 33);
            cboChiNhanh.TabIndex = 0;
            // 
            // lbAddress
            // 
            lbAddress.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbAddress.ForeColor = SystemColors.ActiveCaptionText;
            lbAddress.Location = new Point(3, 9);
            lbAddress.Name = "lbAddress";
            lbAddress.Size = new Size(207, 43);
            lbAddress.TabIndex = 2;
            lbAddress.Text = "Chọn Chi Nhánh: ";
            lbAddress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.Controls.Add(gbDoUong);
            panel2.Location = new Point(12, 75);
            panel2.Name = "panel2";
            panel2.Size = new Size(399, 547);
            panel2.TabIndex = 1;
            // 
            // gbDoUong
            // 
            gbDoUong.Controls.Add(btnAdd);
            gbDoUong.Controls.Add(chklistTopping);
            gbDoUong.Controls.Add(lbTopping);
            gbDoUong.Controls.Add(numSoLuong);
            gbDoUong.Controls.Add(lbSoLuong);
            gbDoUong.Controls.Add(cboDa);
            gbDoUong.Controls.Add(lbIce);
            gbDoUong.Controls.Add(cboDuong);
            gbDoUong.Controls.Add(lbSugar);
            gbDoUong.Controls.Add(cboSize);
            gbDoUong.Controls.Add(lbSize);
            gbDoUong.Controls.Add(cboDoUong);
            gbDoUong.Controls.Add(lbDirnk);
            gbDoUong.Location = new Point(3, 3);
            gbDoUong.Name = "gbDoUong";
            gbDoUong.Size = new Size(393, 541);
            gbDoUong.TabIndex = 0;
            gbDoUong.TabStop = false;
            gbDoUong.Text = "Chọn Đồ Uống";
            // 
            // btnAdd
            // 
            btnAdd.BackColor = SystemColors.ActiveCaption;
            btnAdd.ForeColor = SystemColors.ButtonHighlight;
            btnAdd.Location = new Point(99, 485);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(184, 50);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "Thêm Vào Giỏ Hàng";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // chklistTopping
            // 
            chklistTopping.FormattingEnabled = true;
            chklistTopping.Location = new Point(6, 305);
            chklistTopping.Name = "chklistTopping";
            chklistTopping.Size = new Size(381, 144);
            chklistTopping.TabIndex = 0;
            // 
            // lbTopping
            // 
            lbTopping.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbTopping.Location = new Point(6, 272);
            lbTopping.Name = "lbTopping";
            lbTopping.Size = new Size(381, 30);
            lbTopping.TabIndex = 8;
            lbTopping.Text = "Toppings (Có Thể Chọn Nhiều)";
            // 
            // numSoLuong
            // 
            numSoLuong.Location = new Point(120, 223);
            numSoLuong.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSoLuong.Name = "numSoLuong";
            numSoLuong.Size = new Size(136, 31);
            numSoLuong.TabIndex = 5;
            numSoLuong.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lbSoLuong
            // 
            lbSoLuong.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbSoLuong.Location = new Point(6, 222);
            lbSoLuong.Name = "lbSoLuong";
            lbSoLuong.Size = new Size(108, 30);
            lbSoLuong.TabIndex = 7;
            lbSoLuong.Text = "Số Lượng:";
            // 
            // cboDa
            // 
            cboDa.FormattingEnabled = true;
            cboDa.Location = new Point(245, 172);
            cboDa.Name = "cboDa";
            cboDa.Size = new Size(91, 33);
            cboDa.TabIndex = 4;
            // 
            // lbIce
            // 
            lbIce.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbIce.Location = new Point(197, 175);
            lbIce.Name = "lbIce";
            lbIce.Size = new Size(59, 30);
            lbIce.TabIndex = 5;
            lbIce.Text = "Đá:";
            // 
            // cboDuong
            // 
            cboDuong.FormattingEnabled = true;
            cboDuong.Location = new Point(90, 172);
            cboDuong.Name = "cboDuong";
            cboDuong.Size = new Size(91, 33);
            cboDuong.TabIndex = 3;
            // 
            // lbSugar
            // 
            lbSugar.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbSugar.Location = new Point(6, 172);
            lbSugar.Name = "lbSugar";
            lbSugar.Size = new Size(78, 30);
            lbSugar.TabIndex = 3;
            lbSugar.Text = "Đường:";
            // 
            // cboSize
            // 
            cboSize.FormattingEnabled = true;
            cboSize.Location = new Point(71, 122);
            cboSize.Name = "cboSize";
            cboSize.Size = new Size(62, 33);
            cboSize.TabIndex = 2;
            // 
            // lbSize
            // 
            lbSize.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbSize.Location = new Point(6, 122);
            lbSize.Name = "lbSize";
            lbSize.Size = new Size(59, 30);
            lbSize.TabIndex = 1;
            lbSize.Text = "Size:";
            // 
            // cboDoUong
            // 
            cboDoUong.FormattingEnabled = true;
            cboDoUong.Location = new Point(6, 76);
            cboDoUong.Name = "cboDoUong";
            cboDoUong.Size = new Size(381, 33);
            cboDoUong.TabIndex = 1;
            // 
            // lbDirnk
            // 
            lbDirnk.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbDirnk.Location = new Point(6, 43);
            lbDirnk.Name = "lbDirnk";
            lbDirnk.Size = new Size(381, 30);
            lbDirnk.TabIndex = 0;
            lbDirnk.Text = "Đồ Uống: ";
            // 
            // panel3
            // 
            panel3.Controls.Add(gbGioHang);
            panel3.Location = new Point(417, 75);
            panel3.Name = "panel3";
            panel3.Size = new Size(627, 547);
            panel3.TabIndex = 2;
            // 
            // gbGioHang
            // 
            gbGioHang.Controls.Add(btnThanhToan);
            gbGioHang.Controls.Add(btnXoa);
            gbGioHang.Controls.Add(dgvGioHang);
            gbGioHang.Location = new Point(3, 3);
            gbGioHang.Name = "gbGioHang";
            gbGioHang.Size = new Size(621, 541);
            gbGioHang.TabIndex = 0;
            gbGioHang.TabStop = false;
            gbGioHang.Text = "Giỏ Hàng";
            // 
            // btnThanhToan
            // 
            btnThanhToan.BackColor = SystemColors.Highlight;
            btnThanhToan.ForeColor = Color.White;
            btnThanhToan.Location = new Point(431, 485);
            btnThanhToan.Name = "btnThanhToan";
            btnThanhToan.Size = new Size(184, 50);
            btnThanhToan.TabIndex = 2;
            btnThanhToan.Text = "Thanh Toán";
            btnThanhToan.UseVisualStyleBackColor = false;
            // 
            // btnXoa
            // 
            btnXoa.BackColor = Color.SeaShell;
            btnXoa.ForeColor = SystemColors.ActiveCaptionText;
            btnXoa.Location = new Point(6, 485);
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(184, 50);
            btnXoa.TabIndex = 1;
            btnXoa.Text = "Xóa Món Chọn";
            btnXoa.UseVisualStyleBackColor = false;
            btnXoa.Click += btnXoa_Click;
            // 
            // dgvGioHang
            // 
            dgvGioHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGioHang.Location = new Point(6, 43);
            dgvGioHang.Name = "dgvGioHang";
            dgvGioHang.RowHeadersWidth = 62;
            dgvGioHang.Size = new Size(609, 406);
            dgvGioHang.TabIndex = 0;
            // 
            // CustomerOrderForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1056, 634);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "CustomerOrderForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Khách Hàng - Đặt Món";
            Load += CustomerOrderForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            gbDoUong.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numSoLuong).EndInit();
            panel3.ResumeLayout(false);
            gbGioHang.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvGioHang).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label lbAddress;
        private Label lbTrangThai;
        private ComboBox cboChiNhanh;
        private Panel panel2;
        private Panel panel3;
        private GroupBox gbDoUong;
        private Label lbDirnk;
        private ComboBox cboDoUong;
        private Label lbSize;
        private ComboBox cboDa;
        private Label lbIce;
        private ComboBox cboDuong;
        private Label lbSugar;
        private ComboBox cboSize;
        private NumericUpDown numSoLuong;
        private Label lbSoLuong;
        private Label lbTopping;
        private Button btnAdd;
        private CheckedListBox chklistTopping;
        private GroupBox gbGioHang;
        private DataGridView dgvGioHang;
        private Button btnThanhToan;
        private Button btnXoa;
    }
}