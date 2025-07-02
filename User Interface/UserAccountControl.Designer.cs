namespace CMS_Revised.User_Interface
{
    partial class UserAccountControl
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
            buttonPanel = new Panel();
            personalinfobtn = new Button();
            accountsettingsbtn = new Button();
            contentPanel = new Panel();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(personalinfobtn);
            buttonPanel.Controls.Add(accountsettingsbtn);
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Location = new Point(0, 0);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(994, 44);
            buttonPanel.TabIndex = 1;
            buttonPanel.Paint += buttonPanel_Paint;
            // 
            // personalinfobtn
            // 
            personalinfobtn.Location = new Point(140, 6);
            personalinfobtn.Name = "personalinfobtn";
            personalinfobtn.Size = new Size(345, 35);
            personalinfobtn.TabIndex = 14;
            personalinfobtn.Text = "personalinfobtn";
            personalinfobtn.UseVisualStyleBackColor = true;
            personalinfobtn.Click += personalinfobtn_Click;
            // 
            // accountsettingsbtn
            // 
            accountsettingsbtn.Location = new Point(482, 6);
            accountsettingsbtn.Name = "accountsettingsbtn";
            accountsettingsbtn.Size = new Size(383, 35);
            accountsettingsbtn.TabIndex = 15;
            accountsettingsbtn.Text = "accountsettingsbtn";
            accountsettingsbtn.UseVisualStyleBackColor = true;
            accountsettingsbtn.Click += accountsettingsbtn_Click;
            // 
            // contentPanel
            // 
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 44);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(994, 496);
            contentPanel.TabIndex = 0;
            contentPanel.Paint += contentPanel_Paint;
            // 
            // UserAccountControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(contentPanel);
            Controls.Add(buttonPanel);
            Name = "UserAccountControl";
            Size = new Size(994, 540);
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ProfileAccountSettings profileAccountSettings1;
        private Button accountsettingsbtn;
        private Button personalinfobtn;
        private Panel buttonPanel;
        private Panel contentPanel;
    }
}
