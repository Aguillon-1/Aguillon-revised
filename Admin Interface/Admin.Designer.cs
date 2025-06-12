using ClassroomManagementSystem;

namespace Admin// Replace with your actual namespace
{
    partial class Admin : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Panel HeaderPanel;
        private System.Windows.Forms.Button NotificationButton;
        private System.Windows.Forms.Button AccountButton;
        private System.Windows.Forms.Panel MenuPanel;
        private System.Windows.Forms.Button CurriculumManager;
        private System.Windows.Forms.Button LogoutButton;
        private System.Windows.Forms.Button SystemConfigButton;
        private System.Windows.Forms.Button AddingChangingSubjectsButton;
        private System.Windows.Forms.Button AccountManagerButton;
        private System.Windows.Forms.Button HomeButton;
        private System.Windows.Forms.Button MenuButton;
        private System.Windows.Forms.Timer slidingMenuTimer;
        private AccountManager AccountManager1;
        private AddingChangingSubjects AddingChangingSubjects1;
        private CurriculumManager CurriculumManager1;
        private SystemConfiguration SystemConfiguration1;


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

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            HeaderPanel = new Panel();
            NotificationButton = new Button();
            AccountButton = new Button();
            MenuPanel = new Panel();
            CurriculumManager = new Button();
            LogoutButton = new Button();
            SystemConfigButton = new Button();
            AddingChangingSubjectsButton = new Button();
            AccountManagerButton = new Button();
            HomeButton = new Button();
            MenuButton = new Button();
            slidingMenuTimer = new System.Windows.Forms.Timer(components);
            AccountManager1 = new AccountManager();
            AddingChangingSubjects1 = new AddingChangingSubjects();
            CurriculumManager1 = new CurriculumManager();
            SystemConfiguration1 = new SystemConfiguration();
            HeaderPanel.SuspendLayout();
            MenuPanel.SuspendLayout();
            SuspendLayout();
            // 
            // HeaderPanel
            // 
            HeaderPanel.Controls.Add(NotificationButton);
            HeaderPanel.Controls.Add(AccountButton);
            HeaderPanel.Dock = DockStyle.Top;
            HeaderPanel.Location = new Point(0, 0);
            HeaderPanel.Name = "HeaderPanel";
            HeaderPanel.Size = new Size(1348, 59);
            HeaderPanel.TabIndex = 1;
            // 
            // NotificationButton
            // 
            NotificationButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            NotificationButton.Dock = DockStyle.Right;
            NotificationButton.Location = new Point(1196, 0);
            NotificationButton.Name = "NotificationButton";
            NotificationButton.Size = new Size(86, 59);
            NotificationButton.TabIndex = 1;
            NotificationButton.Text = "Nottification";
            NotificationButton.UseVisualStyleBackColor = true;
            // 
            // AccountButton
            // 
            AccountButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AccountButton.Dock = DockStyle.Right;
            AccountButton.Location = new Point(1282, 0);
            AccountButton.Name = "AccountButton";
            AccountButton.Size = new Size(66, 59);
            AccountButton.TabIndex = 0;
            AccountButton.Text = "Account";
            AccountButton.UseVisualStyleBackColor = true;
            // 
            // MenuPanel
            // 
            MenuPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MenuPanel.BackColor = Color.FromArgb(47, 97, 83);
            MenuPanel.Controls.Add(CurriculumManager);
            MenuPanel.Controls.Add(LogoutButton);
            MenuPanel.Controls.Add(SystemConfigButton);
            MenuPanel.Controls.Add(AddingChangingSubjectsButton);
            MenuPanel.Controls.Add(AccountManagerButton);
            MenuPanel.Controls.Add(HomeButton);
            MenuPanel.Controls.Add(MenuButton);
            MenuPanel.Dock = DockStyle.Left;
            MenuPanel.Location = new Point(0, 59);
            MenuPanel.MaximumSize = new Size(229, 0);
            MenuPanel.MinimumSize = new Size(72, 0);
            MenuPanel.Name = "MenuPanel";
            MenuPanel.Size = new Size(229, 575);
            MenuPanel.TabIndex = 2;
            // 
            // CurriculumManager
            // 
            CurriculumManager.AutoSize = true;
            CurriculumManager.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CurriculumManager.Dock = DockStyle.Top;
            CurriculumManager.FlatStyle = FlatStyle.Flat;
            CurriculumManager.ForeColor = SystemColors.ButtonHighlight;
            CurriculumManager.ImageAlign = ContentAlignment.MiddleLeft;
            CurriculumManager.Location = new Point(0, 275);
            CurriculumManager.MinimumSize = new Size(10, 55);
            CurriculumManager.Name = "CurriculumManager";
            CurriculumManager.Padding = new Padding(10, 0, 0, 0);
            CurriculumManager.Size = new Size(229, 55);
            CurriculumManager.TabIndex = 16;
            CurriculumManager.Text = "Curriculum Manager";
            CurriculumManager.UseVisualStyleBackColor = true;
            // 
            // LogoutButton
            // 
            LogoutButton.BackColor = Color.FromArgb(192, 0, 0);
            LogoutButton.Dock = DockStyle.Bottom;
            LogoutButton.ForeColor = SystemColors.ButtonHighlight;
            LogoutButton.ImageAlign = ContentAlignment.MiddleLeft;
            LogoutButton.Location = new Point(0, 522);
            LogoutButton.Name = "LogoutButton";
            LogoutButton.Padding = new Padding(22, 0, 0, 0);
            LogoutButton.Size = new Size(229, 53);
            LogoutButton.TabIndex = 15;
            LogoutButton.Text = "Logout";
            LogoutButton.UseVisualStyleBackColor = false;
            // 
            // SystemConfigButton
            // 
            SystemConfigButton.AutoSize = true;
            SystemConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            SystemConfigButton.Dock = DockStyle.Top;
            SystemConfigButton.FlatStyle = FlatStyle.Flat;
            SystemConfigButton.ForeColor = SystemColors.ButtonHighlight;
            SystemConfigButton.ImageAlign = ContentAlignment.MiddleLeft;
            SystemConfigButton.Location = new Point(0, 220);
            SystemConfigButton.MinimumSize = new Size(10, 55);
            SystemConfigButton.Name = "SystemConfigButton";
            SystemConfigButton.Padding = new Padding(10, 0, 25, 0);
            SystemConfigButton.Size = new Size(229, 55);
            SystemConfigButton.TabIndex = 12;
            SystemConfigButton.Text = "System Configuration";
            SystemConfigButton.TextAlign = ContentAlignment.MiddleRight;
            SystemConfigButton.UseVisualStyleBackColor = true;
            SystemConfigButton.Click += SystemConfigButton_Click;
            // 
            // AddingChangingSubjectsButton
            // 
            AddingChangingSubjectsButton.AutoSize = true;
            AddingChangingSubjectsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AddingChangingSubjectsButton.Dock = DockStyle.Top;
            AddingChangingSubjectsButton.FlatStyle = FlatStyle.Flat;
            AddingChangingSubjectsButton.ForeColor = SystemColors.ButtonHighlight;
            AddingChangingSubjectsButton.ImageAlign = ContentAlignment.MiddleLeft;
            AddingChangingSubjectsButton.Location = new Point(0, 165);
            AddingChangingSubjectsButton.MinimumSize = new Size(10, 55);
            AddingChangingSubjectsButton.Name = "AddingChangingSubjectsButton";
            AddingChangingSubjectsButton.Padding = new Padding(10, 0, 0, 0);
            AddingChangingSubjectsButton.Size = new Size(229, 55);
            AddingChangingSubjectsButton.TabIndex = 11;
            AddingChangingSubjectsButton.Text = "Adding/Changing \r\nof Subjects";
            AddingChangingSubjectsButton.UseVisualStyleBackColor = true;
            // 
            // AccountManagerButton
            // 
            AccountManagerButton.AutoSize = true;
            AccountManagerButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AccountManagerButton.Dock = DockStyle.Top;
            AccountManagerButton.FlatStyle = FlatStyle.Flat;
            AccountManagerButton.ForeColor = SystemColors.ButtonHighlight;
            AccountManagerButton.ImageAlign = ContentAlignment.MiddleLeft;
            AccountManagerButton.Location = new Point(0, 110);
            AccountManagerButton.MinimumSize = new Size(10, 55);
            AccountManagerButton.Name = "AccountManagerButton";
            AccountManagerButton.Padding = new Padding(10, 0, 0, 0);
            AccountManagerButton.Size = new Size(229, 55);
            AccountManagerButton.TabIndex = 10;
            AccountManagerButton.Text = "Account Manager";
            AccountManagerButton.UseVisualStyleBackColor = true;
            // 
            // HomeButton
            // 
            HomeButton.AutoSize = true;
            HomeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            HomeButton.Dock = DockStyle.Top;
            HomeButton.FlatStyle = FlatStyle.Flat;
            HomeButton.ForeColor = SystemColors.ButtonHighlight;
            HomeButton.ImageAlign = ContentAlignment.MiddleLeft;
            HomeButton.Location = new Point(0, 55);
            HomeButton.MinimumSize = new Size(10, 55);
            HomeButton.Name = "HomeButton";
            HomeButton.Padding = new Padding(10, 0, 0, 0);
            HomeButton.Size = new Size(229, 55);
            HomeButton.TabIndex = 9;
            HomeButton.Text = "Home";
            HomeButton.UseVisualStyleBackColor = true;
            // 
            // MenuButton
            // 
            MenuButton.AutoSize = true;
            MenuButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MenuButton.Dock = DockStyle.Top;
            MenuButton.FlatStyle = FlatStyle.Flat;
            MenuButton.ForeColor = SystemColors.ButtonHighlight;
            MenuButton.ImageAlign = ContentAlignment.MiddleLeft;
            MenuButton.Location = new Point(0, 0);
            MenuButton.MinimumSize = new Size(10, 55);
            MenuButton.Name = "MenuButton";
            MenuButton.Padding = new Padding(10, 0, 0, 0);
            MenuButton.Size = new Size(229, 55);
            MenuButton.TabIndex = 8;
            MenuButton.Text = "Menu";
            MenuButton.UseVisualStyleBackColor = true;
            // 
            // AccountManager1
            // 
            AccountManager1.BackColor = Color.FromArgb(255, 157, 0);
            AccountManager1.Dock = DockStyle.Fill;
            AccountManager1.Location = new Point(229, 59);
            AccountManager1.Margin = new Padding(3, 4, 3, 4);
            AccountManager1.Name = "AccountManager1";
            AccountManager1.Size = new Size(1119, 575);
            AccountManager1.TabIndex = 3;
            AccountManager1.Visible = false;
            // 
            // AddingChangingSubjects1
            // 
            AddingChangingSubjects1.BackColor = Color.FromArgb(255, 157, 0);
            AddingChangingSubjects1.Dock = DockStyle.Fill;
            AddingChangingSubjects1.Location = new Point(229, 59);
            AddingChangingSubjects1.Name = "AddingChangingSubjects1";
            AddingChangingSubjects1.Size = new Size(1119, 575);
            AddingChangingSubjects1.TabIndex = 4;
            AddingChangingSubjects1.Visible = false;
            // 
            // CurriculumManager1
            // 
            CurriculumManager1.BackColor = Color.FromArgb(255, 157, 0);
            CurriculumManager1.Dock = DockStyle.Fill;
            CurriculumManager1.Location = new Point(229, 59);
            CurriculumManager1.Name = "CurriculumManager1";
            CurriculumManager1.Size = new Size(1119, 575);
            CurriculumManager1.TabIndex = 5;
            CurriculumManager1.Visible = false;
            // 
            // SystemConfiguration1
            // 
            SystemConfiguration1.BackColor = Color.FromArgb(255, 157, 0);
            SystemConfiguration1.Dock = DockStyle.Fill;
            SystemConfiguration1.Location = new Point(229, 59);
            SystemConfiguration1.Name = "SystemConfiguration1";
            SystemConfiguration1.Size = new Size(1119, 575);
            SystemConfiguration1.TabIndex = 6;
            SystemConfiguration1.Visible = false;
            // 
            // Admin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 157, 0);
            ClientSize = new Size(1348, 634);
            Controls.Add(SystemConfiguration1);
            Controls.Add(CurriculumManager1);
            Controls.Add(AddingChangingSubjects1);
            Controls.Add(AccountManager1);
            Controls.Add(MenuPanel);
            Controls.Add(HeaderPanel);
            MinimumSize = new Size(1344, 620);
            Name = "Admin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CMS ADMIN";
            HeaderPanel.ResumeLayout(false);
            MenuPanel.ResumeLayout(false);
            MenuPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}