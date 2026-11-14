namespace CoffeeChain.Winforms
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            lbTittle = new Label();
            lbEmail = new Label();
            lbPassWord = new Label();
            txtEmail = new TextBox();
            txtPassWord = new TextBox();
            btnLogin = new Button();
            btnExit = new Button();
            lbVerity = new Label();
            SuspendLayout();
            // 
            // lbTittle
            // 
            lbTittle.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbTittle.ForeColor = Color.FromArgb(128, 64, 0);
            lbTittle.Location = new Point(10, 7);
            lbTittle.Margin = new Padding(2, 0, 2, 0);
            lbTittle.Name = "lbTittle";
            lbTittle.Size = new Size(330, 42);
            lbTittle.TabIndex = 0;
            lbTittle.Text = "COFFEE CHAIN";
            lbTittle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbEmail
            // 
            lbEmail.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbEmail.ForeColor = SystemColors.ActiveCaptionText;
            lbEmail.Location = new Point(10, 70);
            lbEmail.Margin = new Padding(2, 0, 2, 0);
            lbEmail.Name = "lbEmail";
            lbEmail.Size = new Size(330, 30);
            lbEmail.TabIndex = 1;
            lbEmail.Text = "Email";
            // 
            // lbPassWord
            // 
            lbPassWord.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbPassWord.ForeColor = SystemColors.ActiveCaptionText;
            lbPassWord.Location = new Point(10, 150);
            lbPassWord.Margin = new Padding(2, 0, 2, 0);
            lbPassWord.Name = "lbPassWord";
            lbPassWord.Size = new Size(330, 30);
            lbPassWord.TabIndex = 2;
            lbPassWord.Text = "Mật Khẩu";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(10, 103);
            txtEmail.Margin = new Padding(2, 2, 2, 2);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(330, 27);
            txtEmail.TabIndex = 3;
            // 
            // txtPassWord
            // 
            txtPassWord.Location = new Point(10, 183);
            txtPassWord.Margin = new Padding(2, 2, 2, 2);
            txtPassWord.Name = "txtPassWord";
            txtPassWord.Size = new Size(330, 27);
            txtPassWord.TabIndex = 4;
            txtPassWord.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(128, 64, 0);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(148, 256);
            btnLogin.Margin = new Padding(2, 2, 2, 2);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(96, 44);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Đăng Nhập";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.ActiveCaption;
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(249, 256);
            btnExit.Margin = new Padding(2, 2, 2, 2);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(90, 44);
            btnExit.TabIndex = 6;
            btnExit.Text = "Thoát";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lbVerity
            // 
            lbVerity.Font = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lbVerity.ForeColor = Color.IndianRed;
            lbVerity.Location = new Point(10, 223);
            lbVerity.Margin = new Padding(2, 0, 2, 0);
            lbVerity.Name = "lbVerity";
            lbVerity.Size = new Size(330, 30);
            lbVerity.TabIndex = 7;
            lbVerity.Text = "Xác Thực...";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(349, 310);
            Controls.Add(lbVerity);
            Controls.Add(btnExit);
            Controls.Add(btnLogin);
            Controls.Add(txtPassWord);
            Controls.Add(txtEmail);
            Controls.Add(lbPassWord);
            Controls.Add(lbEmail);
            Controls.Add(lbTittle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 2, 2, 2);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CoffeeChain - Đăng Nhập";
            FormClosing += LoginForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbTittle;
        private Label lbEmail;
        private Label lbPassWord;
        private TextBox txtEmail;
        private TextBox txtPassWord;
        private Button btnLogin;
        private Button btnExit;
        private Label lbVerity;
    }
}
