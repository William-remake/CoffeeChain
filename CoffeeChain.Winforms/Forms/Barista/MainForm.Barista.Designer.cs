namespace CoffeeChain.Winforms.Forms.Barista
{
    partial class BaristaMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaristaMainForm));
            pBackGroundTittle = new Panel();
            btnExit = new Button();
            lbCustomer = new Label();
            lbTittle = new Label();
            btnHistory = new Button();
            dgvBarista = new DataGridView();
            colMaDon = new DataGridViewTextBoxColumn();
            colAddress = new DataGridViewTextBoxColumn();
            colCustomer = new DataGridViewTextBoxColumn();
            colTime = new DataGridViewTextBoxColumn();
            colTrangThai = new DataGridViewTextBoxColumn();
            colMoney = new DataGridViewTextBoxColumn();
            cbTrangThai = new ComboBox();
            lbTrangThai = new Label();
            btnTrangThai = new Button();
            pBackGroundTittle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBarista).BeginInit();
            SuspendLayout();
            // 
            // pBackGroundTittle
            // 
            pBackGroundTittle.BackColor = Color.FromArgb(128, 64, 0);
            pBackGroundTittle.Controls.Add(btnExit);
            pBackGroundTittle.Controls.Add(lbCustomer);
            pBackGroundTittle.Controls.Add(lbTittle);
            pBackGroundTittle.Location = new Point(10, 10);
            pBackGroundTittle.Margin = new Padding(2, 2, 2, 2);
            pBackGroundTittle.Name = "pBackGroundTittle";
            pBackGroundTittle.Size = new Size(1109, 58);
            pBackGroundTittle.TabIndex = 1;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.ActiveCaption;
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(1005, 9);
            btnExit.Margin = new Padding(2, 2, 2, 2);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(90, 44);
            btnExit.TabIndex = 7;
            btnExit.Text = "Đăng xuất";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lbCustomer
            // 
            lbCustomer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbCustomer.ForeColor = Color.White;
            lbCustomer.Location = new Point(791, 0);
            lbCustomer.Margin = new Padding(2, 0, 2, 0);
            lbCustomer.Name = "lbCustomer";
            lbCustomer.Size = new Size(197, 58);
            lbCustomer.TabIndex = 2;
            lbCustomer.Text = "Nhân Viên Pha Chế";
            lbCustomer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbTittle
            // 
            lbTittle.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbTittle.ForeColor = Color.White;
            lbTittle.Location = new Point(0, 0);
            lbTittle.Margin = new Padding(2, 0, 2, 0);
            lbTittle.Name = "lbTittle";
            lbTittle.Size = new Size(342, 58);
            lbTittle.TabIndex = 1;
            lbTittle.Text = "COFFEE CHAIN";
            lbTittle.TextAlign = ContentAlignment.MiddleLeft;
            lbTittle.Click += lbTittle_Click;
            // 
            // btnHistory
            // 
            btnHistory.BackColor = SystemColors.ActiveCaption;
            btnHistory.ForeColor = Color.White;
            btnHistory.Location = new Point(10, 73);
            btnHistory.Margin = new Padding(2, 2, 2, 2);
            btnHistory.Name = "btnHistory";
            btnHistory.Size = new Size(162, 57);
            btnHistory.TabIndex = 2;
            btnHistory.Text = "Làm Mới Đơn Hàng";
            btnHistory.UseVisualStyleBackColor = false;
            // 
            // dgvBarista
            // 
            dgvBarista.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBarista.Columns.AddRange(new DataGridViewColumn[] { colMaDon, colAddress, colCustomer, colTime, colTrangThai, colMoney });
            dgvBarista.Location = new Point(10, 152);
            dgvBarista.Margin = new Padding(2, 2, 2, 2);
            dgvBarista.Name = "dgvBarista";
            dgvBarista.RowHeadersWidth = 62;
            dgvBarista.Size = new Size(1109, 335);
            dgvBarista.TabIndex = 3;
            // 
            // colMaDon
            // 
            colMaDon.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMaDon.HeaderText = "Mã Đơn";
            colMaDon.MinimumWidth = 8;
            colMaDon.Name = "colMaDon";
            // 
            // colAddress
            // 
            colAddress.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colAddress.HeaderText = "Chi Nhánh";
            colAddress.MinimumWidth = 8;
            colAddress.Name = "colAddress";
            // 
            // colCustomer
            // 
            colCustomer.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCustomer.HeaderText = "Khách Hàng";
            colCustomer.MinimumWidth = 8;
            colCustomer.Name = "colCustomer";
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTime.HeaderText = "Thời Gian";
            colTime.MinimumWidth = 8;
            colTime.Name = "colTime";
            // 
            // colTrangThai
            // 
            colTrangThai.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTrangThai.HeaderText = "Trạng Thái";
            colTrangThai.MinimumWidth = 8;
            colTrangThai.Name = "colTrangThai";
            // 
            // colMoney
            // 
            colMoney.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMoney.HeaderText = "Tổng Tiền";
            colMoney.MinimumWidth = 8;
            colMoney.Name = "colMoney";
            // 
            // cbTrangThai
            // 
            cbTrangThai.FormattingEnabled = true;
            cbTrangThai.Location = new Point(153, 519);
            cbTrangThai.Margin = new Padding(2, 2, 2, 2);
            cbTrangThai.Name = "cbTrangThai";
            cbTrangThai.Size = new Size(140, 28);
            cbTrangThai.TabIndex = 4;
            // 
            // lbTrangThai
            // 
            lbTrangThai.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbTrangThai.Location = new Point(10, 522);
            lbTrangThai.Margin = new Padding(2, 0, 2, 0);
            lbTrangThai.Name = "lbTrangThai";
            lbTrangThai.Size = new Size(146, 24);
            lbTrangThai.TabIndex = 5;
            lbTrangThai.Text = "Chuyển Trạng Thái:";
            // 
            // btnTrangThai
            // 
            btnTrangThai.BackColor = Color.FromArgb(128, 64, 0);
            btnTrangThai.ForeColor = Color.White;
            btnTrangThai.Location = new Point(963, 522);
            btnTrangThai.Margin = new Padding(2, 2, 2, 2);
            btnTrangThai.Name = "btnTrangThai";
            btnTrangThai.Size = new Size(155, 72);
            btnTrangThai.TabIndex = 6;
            btnTrangThai.Text = "Cập Nhật Trạng Thái";
            btnTrangThai.UseVisualStyleBackColor = false;
            // 
            // BaristaMainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1128, 631);
            Controls.Add(btnTrangThai);
            Controls.Add(cbTrangThai);
            Controls.Add(lbTrangThai);
            Controls.Add(dgvBarista);
            Controls.Add(btnHistory);
            Controls.Add(pBackGroundTittle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 2, 2, 2);
            Name = "BaristaMainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Coffee Chain - Nhân Viên Pha Chế";
            FormClosing += BaristaMainForm_FormClosing;
            pBackGroundTittle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBarista).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pBackGroundTittle;
        private Button btnExit;
        private Label lbCustomer;
        private Label lbTittle;
        private Button btnHistory;
        private DataGridView dgvBarista;
        private DataGridViewTextBoxColumn colMaDon;
        private DataGridViewTextBoxColumn colAddress;
        private DataGridViewTextBoxColumn colCustomer;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colTrangThai;
        private DataGridViewTextBoxColumn colMoney;
        private ComboBox cbTrangThai;
        private Label lbTrangThai;
        private Button btnTrangThai;
    }
}
