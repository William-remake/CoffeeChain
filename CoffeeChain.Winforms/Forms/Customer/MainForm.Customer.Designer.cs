namespace CoffeeChain.Winforms.Forms
{
    partial class CustomerMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerMainForm));
            pBackGroundTittle = new Panel();
            btnExit = new Button();
            lbCustomer = new Label();
            lbTittle = new Label();
            pWindows = new Panel();
            pBackGroundTittle.SuspendLayout();
            SuspendLayout();
            // 
            // pBackGroundTittle
            // 
            pBackGroundTittle.BackColor = Color.FromArgb(128, 64, 0);
            pBackGroundTittle.Controls.Add(btnExit);
            pBackGroundTittle.Controls.Add(lbCustomer);
            pBackGroundTittle.Controls.Add(lbTittle);
            pBackGroundTittle.Location = new Point(10, 10);
            pBackGroundTittle.Margin = new Padding(2);
            pBackGroundTittle.Name = "pBackGroundTittle";
            pBackGroundTittle.Size = new Size(1109, 58);
            pBackGroundTittle.TabIndex = 0;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.ActiveCaption;
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(1005, 9);
            btnExit.Margin = new Padding(2);
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
            lbCustomer.Location = new Point(865, 0);
            lbCustomer.Margin = new Padding(2, 0, 2, 0);
            lbCustomer.Name = "lbCustomer";
            lbCustomer.Size = new Size(123, 58);
            lbCustomer.TabIndex = 2;
            lbCustomer.Text = "Khách Hàng ";
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
            // 
            // pWindows
            // 
            pWindows.Location = new Point(10, 72);
            pWindows.Margin = new Padding(2);
            pWindows.Name = "pWindows";
            pWindows.Size = new Size(1109, 548);
            pWindows.TabIndex = 4;
            // 
            // CustomerMainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1128, 631);
            Controls.Add(pWindows);
            Controls.Add(pBackGroundTittle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            Name = "CustomerMainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Coffee Chain - Khách Hàng";
            FormClosing += CustomerMainForm_FormClosing;
            pBackGroundTittle.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pBackGroundTittle;
        private Label lbTittle;
        private Label lbCustomer;
        private Button btnExit;
        private Panel pWindows;
    }
}
