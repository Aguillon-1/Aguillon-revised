namespace CMS_Revised
{
    partial class LoginInterface
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LoginButton = new Button();
            EmailTextbox = new TextBox();
            PasswordTextbox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            ForgotPasswordLink = new LinkLabel();
            label3 = new Label();
            GoogleButton = new Button();
            ShowHidePassButton = new Button();
            SuspendLayout();
            // 
            // LoginButton
            // 
            LoginButton.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LoginButton.Location = new Point(294, 324);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(120, 33);
            LoginButton.TabIndex = 0;
            LoginButton.Text = "Login";
            LoginButton.UseVisualStyleBackColor = true;
            // 
            // EmailTextbox
            // 
            EmailTextbox.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            EmailTextbox.Location = new Point(214, 126);
            EmailTextbox.Name = "EmailTextbox";
            EmailTextbox.Size = new Size(273, 35);
            EmailTextbox.TabIndex = 1;
            // 
            // PasswordTextbox
            // 
            PasswordTextbox.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PasswordTextbox.Location = new Point(214, 209);
            PasswordTextbox.Name = "PasswordTextbox";
            PasswordTextbox.Size = new Size(227, 35);
            PasswordTextbox.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(214, 102);
            label1.Name = "label1";
            label1.Size = new Size(53, 21);
            label1.TabIndex = 3;
            label1.Text = "Email";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(214, 185);
            label2.Name = "label2";
            label2.Size = new Size(82, 21);
            label2.TabIndex = 4;
            label2.Text = "Password";
            // 
            // ForgotPasswordLink
            // 
            ForgotPasswordLink.AutoSize = true;
            ForgotPasswordLink.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForgotPasswordLink.Location = new Point(214, 262);
            ForgotPasswordLink.Name = "ForgotPasswordLink";
            ForgotPasswordLink.Size = new Size(114, 17);
            ForgotPasswordLink.TabIndex = 5;
            ForgotPasswordLink.TabStop = true;
            ForgotPasswordLink.Text = "Forgot password?";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(304, 383);
            label3.Name = "label3";
            label3.Size = new Size(101, 17);
            label3.TabIndex = 6;
            label3.Text = "or continue with";
            // 
            // GoogleButton
            // 
            GoogleButton.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            GoogleButton.Location = new Point(294, 436);
            GoogleButton.Name = "GoogleButton";
            GoogleButton.Size = new Size(120, 33);
            GoogleButton.TabIndex = 0;
            GoogleButton.Text = "Google";
            GoogleButton.UseVisualStyleBackColor = true;
            // 
            // ShowHidePassButton
            // 
            ShowHidePassButton.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ShowHidePassButton.Location = new Point(447, 211);
            ShowHidePassButton.Name = "ShowHidePassButton";
            ShowHidePassButton.Size = new Size(40, 33);
            ShowHidePassButton.TabIndex = 0;
            ShowHidePassButton.Text = "S";
            ShowHidePassButton.UseVisualStyleBackColor = true;
            // 
            // LoginInterface
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label3);
            Controls.Add(ForgotPasswordLink);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(PasswordTextbox);
            Controls.Add(EmailTextbox);
            Controls.Add(GoogleButton);
            Controls.Add(ShowHidePassButton);
            Controls.Add(LoginButton);
            Name = "LoginInterface";
            Size = new Size(711, 616);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button LoginButton;
        private TextBox EmailTextbox;
        private TextBox PasswordTextbox;
        private Label label1;
        private Label label2;
        private LinkLabel ForgotPasswordLink;
        private Label label3;
        private Button GoogleButton;
        private Button ShowHidePassButton;
    }
}
