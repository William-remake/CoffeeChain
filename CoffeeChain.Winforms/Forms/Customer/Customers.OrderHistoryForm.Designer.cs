namespace CoffeeChain.Winforms.Forms
{
    partial class CustomersOrderHistoryForm
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
            btnHistory = new Button();
            dgvHistory = new DataGridView();
            colStt = new DataGridViewTextBoxColumn();
            colItem = new DataGridViewTextBoxColumn();
            colTopping = new DataGridViewTextBoxColumn();
            colTime = new DataGridViewTextBoxColumn();
            colTrangThai = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            SuspendLayout();
            // 
            // btnHistory
            // 
            btnHistory.BackColor = SystemColors.ActiveCaption;
            btnHistory.ForeColor = Color.White;
            btnHistory.Location = new Point(12, 12);
            btnHistory.Name = "btnHistory";
            btnHistory.Size = new Size(203, 71);
            btnHistory.TabIndex = 0;
            btnHistory.Text = "Làm Mới Lịch Sử";
            btnHistory.UseVisualStyleBackColor = false;
            // 
            // dgvHistory
            // 
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Columns.AddRange(new DataGridViewColumn[] { colStt, colItem, colTopping, colTime, colTrangThai, colTotal });
            dgvHistory.Location = new Point(12, 108);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.RowHeadersWidth = 62;
            dgvHistory.Size = new Size(1032, 497);
            dgvHistory.TabIndex = 1;
            // 
            // colStt
            // 
            colStt.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colStt.HeaderText = "STT";
            colStt.MinimumWidth = 8;
            colStt.Name = "colStt";
            // 
            // colItem
            // 
            colItem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colItem.HeaderText = "Món";
            colItem.MinimumWidth = 8;
            colItem.Name = "colItem";
            // 
            // colTopping
            // 
            colTopping.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTopping.HeaderText = "Thêm Topping";
            colTopping.MinimumWidth = 8;
            colTopping.Name = "colTopping";
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTime.HeaderText = "Thời Gian Đặt";
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
            // colTotal
            // 
            colTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTotal.HeaderText = "Tổng Giá";
            colTotal.MinimumWidth = 8;
            colTotal.Name = "colTotal";
            // 
            // CustomersOrderHistoryForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1056, 634);
            Controls.Add(dgvHistory);
            Controls.Add(btnHistory);
            Name = "CustomersOrderHistoryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Khách Hàng - Lịch Sử Đặt Hàng";
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnHistory;
        private DataGridView dgvHistory;
        private DataGridViewTextBoxColumn colStt;
        private DataGridViewTextBoxColumn colItem;
        private DataGridViewTextBoxColumn colTopping;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colTrangThai;
        private DataGridViewTextBoxColumn colTotal;
    }
}