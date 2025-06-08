namespace CMS_Revised
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
            TopPanel = new Panel();
            MinimizeButton = new Button();
            ExitButton = new Button();
            MainSplitContainer = new SplitContainer();
            BrandingPanel = new Panel();
            CMSdescrptionlabel = new Label();
            CMSlabel = new Label();
            pictureBox1 = new PictureBox();
            LoginSignupPanel = new Panel();
            LoginControl1 = new LoginControl();
            SwitcherPanel = new Panel();
            loginSignupSwitcher1 = new LoginSignupSwitcher();
            s1_studentinfo1 = new CMS_Revised.Login_Interface.S1_studentinfo();
            s2_personalinfo1 = new CMS_Revised.Login_Interface.S2_personalinfo();
            s3_accountcredentials1 = new CMS_Revised.Login_Interface.S3_accountcredentials();
            TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).BeginInit();
            MainSplitContainer.Panel1.SuspendLayout();
            MainSplitContainer.Panel2.SuspendLayout();
            MainSplitContainer.SuspendLayout();
            BrandingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            LoginSignupPanel.SuspendLayout();
            SwitcherPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TopPanel
            // 
            TopPanel.Controls.Add(MinimizeButton);
            TopPanel.Controls.Add(ExitButton);
            TopPanel.Dock = DockStyle.Top;
            TopPanel.Location = new Point(0, 0);
            TopPanel.Name = "TopPanel";
            TopPanel.Size = new Size(1184, 45);
            TopPanel.TabIndex = 0;
            // 
            // MinimizeButton
            // 
            MinimizeButton.BackColor = SystemColors.Highlight;
            MinimizeButton.Dock = DockStyle.Right;
            MinimizeButton.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MinimizeButton.ForeColor = SystemColors.Control;
            MinimizeButton.Location = new Point(1094, 0);
            MinimizeButton.Name = "MinimizeButton";
            MinimizeButton.Size = new Size(45, 45);
            MinimizeButton.TabIndex = 1;
            MinimizeButton.Text = "-";
            MinimizeButton.TextAlign = ContentAlignment.TopCenter;
            MinimizeButton.UseVisualStyleBackColor = false;
            MinimizeButton.Click += MinimizeButton_Click;
            // 
            // ExitButton
            // 
            ExitButton.BackColor = Color.FromArgb(192, 0, 0);
            ExitButton.Dock = DockStyle.Right;
            ExitButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ExitButton.ForeColor = SystemColors.Control;
            ExitButton.Location = new Point(1139, 0);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(45, 45);
            ExitButton.TabIndex = 0;
            ExitButton.Text = "X";
            ExitButton.UseCompatibleTextRendering = true;
            ExitButton.UseVisualStyleBackColor = false;
            ExitButton.Click += ExitButton_Click;
            // 
            // MainSplitContainer
            // 
            MainSplitContainer.Dock = DockStyle.Fill;
            MainSplitContainer.Location = new Point(0, 45);
            MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            MainSplitContainer.Panel1.Controls.Add(BrandingPanel);
            // 
            // MainSplitContainer.Panel2
            // 
            MainSplitContainer.Panel2.Controls.Add(LoginSignupPanel);
            MainSplitContainer.Size = new Size(1184, 639);
            MainSplitContainer.SplitterDistance = 505;
            MainSplitContainer.TabIndex = 1;
            // 
            // BrandingPanel
            // 
            BrandingPanel.Controls.Add(CMSdescrptionlabel);
            BrandingPanel.Controls.Add(CMSlabel);
            BrandingPanel.Controls.Add(pictureBox1);
            BrandingPanel.Dock = DockStyle.Fill;
            BrandingPanel.Location = new Point(0, 0);
            BrandingPanel.Name = "BrandingPanel";
            BrandingPanel.Size = new Size(505, 639);
            BrandingPanel.TabIndex = 0;
            // 
            // CMSdescrptionlabel
            // 
            CMSdescrptionlabel.AutoSize = true;
            CMSdescrptionlabel.Font = new Font("Berlin Sans FB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CMSdescrptionlabel.Location = new Point(63, 400);
            CMSdescrptionlabel.Name = "CMSdescrptionlabel";
            CMSdescrptionlabel.Size = new Size(392, 84);
            CMSdescrptionlabel.TabIndex = 5;
            CMSdescrptionlabel.Text = "Empowering class organization \r\nthrough streamlined group management \r\nand collaborative digital spaces for\r\nannouncements, scheduling, and idea exchange.";
            // 
            // CMSlabel
            // 
            CMSlabel.AutoSize = true;
            CMSlabel.Font = new Font("Copperplate Gothic Bold", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CMSlabel.Location = new Point(33, 360);
            CMSlabel.Name = "CMSlabel";
            CMSlabel.Size = new Size(444, 26);
            CMSlabel.TabIndex = 4;
            CMSlabel.Text = "Classroom Management System";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Classroom_Management_System__1_;
            pictureBox1.Location = new Point(106, 72);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(299, 285);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // LoginSignupPanel
            // 
            LoginSignupPanel.Controls.Add(LoginControl1);
            LoginSignupPanel.Controls.Add(SwitcherPanel);
            LoginSignupPanel.Controls.Add(s1_studentinfo1);
            LoginSignupPanel.Controls.Add(s2_personalinfo1);
            LoginSignupPanel.Controls.Add(s3_accountcredentials1);
            LoginSignupPanel.Dock = DockStyle.Fill;
            LoginSignupPanel.Location = new Point(0, 0);
            LoginSignupPanel.Name = "LoginSignupPanel";
            LoginSignupPanel.Size = new Size(675, 639);
            LoginSignupPanel.TabIndex = 0;
            // 
            // LoginControl1
            // 
            LoginControl1.BackColor = SystemColors.ButtonHighlight;
            LoginControl1.Dock = DockStyle.Fill;
            LoginControl1.Location = new Point(0, 66);
            LoginControl1.Name = "LoginControl1";
            LoginControl1.Size = new Size(675, 573);
            LoginControl1.TabIndex = 2;
            // 
            // SwitcherPanel
            // 
            SwitcherPanel.Controls.Add(loginSignupSwitcher1);
            SwitcherPanel.Dock = DockStyle.Top;
            SwitcherPanel.Location = new Point(0, 0);
            SwitcherPanel.Name = "SwitcherPanel";
            SwitcherPanel.Size = new Size(675, 66);
            SwitcherPanel.TabIndex = 1;
            // 
            // loginSignupSwitcher1
            // 
            loginSignupSwitcher1.Dock = DockStyle.Fill;
            loginSignupSwitcher1.Location = new Point(0, 0);
            loginSignupSwitcher1.Name = "loginSignupSwitcher1";
            loginSignupSwitcher1.Size = new Size(675, 66);
            loginSignupSwitcher1.TabIndex = 0;
            loginSignupSwitcher1.Load += loginSignupSwitcher1_Load;
            // 
            // s1_studentinfo1
            // 
            s1_studentinfo1.Dock = DockStyle.Fill;
            s1_studentinfo1.Location = new Point(0, 0);
            s1_studentinfo1.Name = "s1_studentinfo1";
            s1_studentinfo1.Size = new Size(675, 639);
            s1_studentinfo1.TabIndex = 3;
            // 
            // s2_personalinfo1
            // 
            s2_personalinfo1.Dock = DockStyle.Fill;
            s2_personalinfo1.Location = new Point(0, 0);
            s2_personalinfo1.Name = "s2_personalinfo1";
            s2_personalinfo1.Size = new Size(675, 639);
            s2_personalinfo1.TabIndex = 4;
            // 
            // s3_accountcredentials1
            // 
            s3_accountcredentials1.Dock = DockStyle.Fill;
            s3_accountcredentials1.Location = new Point(0, 0);
            s3_accountcredentials1.Name = "s3_accountcredentials1";
            s3_accountcredentials1.Size = new Size(675, 639);
            s3_accountcredentials1.TabIndex = 5;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 684);
            ControlBox = false;
            Controls.Add(MainSplitContainer);
            Controls.Add(TopPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(1200, 700);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Load += LoginForm_Load;
            TopPanel.ResumeLayout(false);
            MainSplitContainer.Panel1.ResumeLayout(false);
            MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainSplitContainer).EndInit();
            MainSplitContainer.ResumeLayout(false);
            BrandingPanel.ResumeLayout(false);
            BrandingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            LoginSignupPanel.ResumeLayout(false);
            SwitcherPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel TopPanel;
        private Button MinimizeButton;
        private Button ExitButton;
        private SplitContainer MainSplitContainer;
        private Panel BrandingPanel;
        private Label CMSdescrptionlabel;
        private Label CMSlabel;
        private PictureBox pictureBox1;
        private Panel LoginSignupPanel;
        private Panel SwitcherPanel;
        private LoginSignupSwitcher loginSignupSwitcher1;
        private LoginControl LoginControl1;
    }
}
