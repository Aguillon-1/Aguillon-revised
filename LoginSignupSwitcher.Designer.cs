namespace CMS_Revised
{
    partial class LoginSignupSwitcher
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
            SwitchtoLoginButton = new Button();
            SwitchtoSignupButton = new Button();
            SuspendLayout();
            // 
            // SwitchtoLoginButton
            // 
            SwitchtoLoginButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            SwitchtoLoginButton.Location = new Point(177, 23);
            SwitchtoLoginButton.Name = "SwitchtoLoginButton";
            SwitchtoLoginButton.Size = new Size(146, 43);
            SwitchtoLoginButton.TabIndex = 0;
            SwitchtoLoginButton.Text = "Login";
            SwitchtoLoginButton.UseVisualStyleBackColor = true;
            // 
            // SwitchtoSignupButton
            // 
            SwitchtoSignupButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold);
            SwitchtoSignupButton.Location = new Point(358, 23);
            SwitchtoSignupButton.Name = "SwitchtoSignupButton";
            SwitchtoSignupButton.Size = new Size(146, 43);
            SwitchtoSignupButton.TabIndex = 1;
            SwitchtoSignupButton.Text = "Signup";
            SwitchtoSignupButton.UseVisualStyleBackColor = true;
            // 
            // LoginSignupSwitcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(SwitchtoSignupButton);
            Controls.Add(SwitchtoLoginButton);
            Name = "LoginSignupSwitcher";
            Size = new Size(675, 66);
            ResumeLayout(false);
        }

        #endregion

        private Button SwitchtoLoginButton;
        private Button SwitchtoSignupButton;
    }
}
