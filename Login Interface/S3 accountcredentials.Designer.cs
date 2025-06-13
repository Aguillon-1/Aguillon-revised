namespace CMS_Revised.Login_Interface
{
    partial class S3_accountcredentials
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
            S2Backbutton = new Button();
            label7 = new Label();
            label6 = new Label();
            S3Finalizebutton = new Button();
            Usernametextbox = new TextBox();
            Emailtextbox = new TextBox();
            Passwordtextbox = new TextBox();
            label1 = new Label();
            ConfirmPasswordtextbox = new TextBox();
            label2 = new Label();
            PasswordNoticelabel = new Label();
            ShowHidePassButton = new Button();
            SuspendLayout();
            // 
            // S2Backbutton
            // 
            S2Backbutton.Location = new Point(71, 457);
            S2Backbutton.Name = "S2Backbutton";
            S2Backbutton.Size = new Size(112, 44);
            S2Backbutton.TabIndex = 37;
            S2Backbutton.Text = "Back";
            S2Backbutton.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(88, 91);
            label7.Name = "label7";
            label7.Size = new Size(87, 21);
            label7.TabIndex = 33;
            label7.Text = "Username:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(349, 91);
            label6.Name = "label6";
            label6.Size = new Size(52, 21);
            label6.TabIndex = 31;
            label6.Text = "Email:";
            // 
            // S3Finalizebutton
            // 
            S3Finalizebutton.Location = new Point(491, 457);
            S3Finalizebutton.Name = "S3Finalizebutton";
            S3Finalizebutton.Size = new Size(112, 44);
            S3Finalizebutton.TabIndex = 23;
            S3Finalizebutton.Text = "Finalize";
            S3Finalizebutton.UseVisualStyleBackColor = true;
            // 
            // Usernametextbox
            // 
            Usernametextbox.Location = new Point(89, 115);
            Usernametextbox.Name = "Usernametextbox";
            Usernametextbox.Size = new Size(220, 23);
            Usernametextbox.TabIndex = 39;
            // 
            // Emailtextbox
            // 
            Emailtextbox.Location = new Point(350, 115);
            Emailtextbox.Name = "Emailtextbox";
            Emailtextbox.Size = new Size(220, 23);
            Emailtextbox.TabIndex = 40;
            // 
            // Passwordtextbox
            // 
            Passwordtextbox.Location = new Point(88, 197);
            Passwordtextbox.Name = "Passwordtextbox";
            Passwordtextbox.Size = new Size(220, 23);
            Passwordtextbox.TabIndex = 42;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(87, 173);
            label1.Name = "label1";
            label1.Size = new Size(83, 21);
            label1.TabIndex = 41;
            label1.Text = "Password:";
            // 
            // ConfirmPasswordtextbox
            // 
            ConfirmPasswordtextbox.Location = new Point(87, 282);
            ConfirmPasswordtextbox.Name = "ConfirmPasswordtextbox";
            ConfirmPasswordtextbox.Size = new Size(220, 23);
            ConfirmPasswordtextbox.TabIndex = 44;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(86, 258);
            label2.Name = "label2";
            label2.Size = new Size(147, 21);
            label2.TabIndex = 43;
            label2.Text = "Confirm password:";
            // 
            // PasswordNoticelabel
            // 
            PasswordNoticelabel.AutoSize = true;
            PasswordNoticelabel.Location = new Point(89, 232);
            PasswordNoticelabel.Name = "PasswordNoticelabel";
            PasswordNoticelabel.Size = new Size(38, 15);
            PasswordNoticelabel.TabIndex = 45;
            PasswordNoticelabel.Text = "label3";
            // 
            // ShowHidePassButton
            // 
            ShowHidePassButton.Location = new Point(314, 197);
            ShowHidePassButton.Name = "ShowHidePassButton";
            ShowHidePassButton.Size = new Size(48, 23);
            ShowHidePassButton.TabIndex = 46;
            ShowHidePassButton.Text = "button1";
            ShowHidePassButton.UseVisualStyleBackColor = true;
            // 
            // S3_accountcredentials
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonHighlight;
            Controls.Add(ShowHidePassButton);
            Controls.Add(PasswordNoticelabel);
            Controls.Add(ConfirmPasswordtextbox);
            Controls.Add(label2);
            Controls.Add(Passwordtextbox);
            Controls.Add(label1);
            Controls.Add(Emailtextbox);
            Controls.Add(Usernametextbox);
            Controls.Add(S2Backbutton);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(S3Finalizebutton);
            Name = "S3_accountcredentials";
            Size = new Size(675, 573);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button S2Backbutton;
        private Label label7;
        private Label label6;
        private Button S3Finalizebutton;
        private TextBox Usernametextbox;
        private TextBox Emailtextbox;
        private TextBox Passwordtextbox;
        private Label label1;
        private TextBox ConfirmPasswordtextbox;
        private Label label2;
        private Label PasswordNoticelabel;
        private Button ShowHidePassButton;
    }
}
