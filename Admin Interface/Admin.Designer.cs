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
        private System.Windows.Forms.Button GradingButton;
        private System.Windows.Forms.Button ClassModerationButton;
        private System.Windows.Forms.Button SystemConfigButton;
        private System.Windows.Forms.Button ClassManagerButton;
        private System.Windows.Forms.Button AccountManagerButton;
        private System.Windows.Forms.Button HomeButton;
        private System.Windows.Forms.Button MenuButton;
        private System.Windows.Forms.Timer slidingMenuTimer;
        private AccountManager AccountManager1;
        //private Class_Manager Class_Manager1;
        //private Curriculum_Manager Curriculum_Manager1;
        

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Admin));
            this.HeaderPanel = new System.Windows.Forms.Panel();
            this.NotificationButton = new System.Windows.Forms.Button();
            this.AccountButton = new System.Windows.Forms.Button();
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.CurriculumManager = new System.Windows.Forms.Button();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.GradingButton = new System.Windows.Forms.Button();
            this.ClassModerationButton = new System.Windows.Forms.Button();
            this.SystemConfigButton = new System.Windows.Forms.Button();
            this.ClassManagerButton = new System.Windows.Forms.Button();
            this.AccountManagerButton = new System.Windows.Forms.Button();
            this.HomeButton = new System.Windows.Forms.Button();
            this.MenuButton = new System.Windows.Forms.Button();
            this.slidingMenuTimer = new System.Windows.Forms.Timer(this.components);
            this.AccountManager1 = new AccountManager();
            //this.Class_Manager1 = new Class_Manager();
            //this.Curriculum_Manager1 = new Curriculum_Manager();
            //this.GradeAdminView1 = new GradeAdminView();
            this.HeaderPanel.SuspendLayout();
            this.MenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // HeaderPanel
            // 
            this.HeaderPanel.Controls.Add(this.NotificationButton);
            this.HeaderPanel.Controls.Add(this.AccountButton);
            this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.HeaderPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.HeaderPanel.Name = "HeaderPanel";
            this.HeaderPanel.Size = new System.Drawing.Size(1696, 79);
            this.HeaderPanel.TabIndex = 1;
            // 
            // NotificationButton
            // 
            this.NotificationButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.NotificationButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.NotificationButton.Location = new System.Drawing.Point(1523, 0);
            this.NotificationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.NotificationButton.Name = "NotificationButton";
            this.NotificationButton.Size = new System.Drawing.Size(98, 79);
            this.NotificationButton.TabIndex = 1;
            this.NotificationButton.Text = "Nottification";
            this.NotificationButton.UseVisualStyleBackColor = true;
            // 
            // AccountButton
            // 
            this.AccountButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AccountButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.AccountButton.Location = new System.Drawing.Point(1621, 0);
            this.AccountButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AccountButton.Name = "AccountButton";
            this.AccountButton.Size = new System.Drawing.Size(75, 79);
            this.AccountButton.TabIndex = 0;
            this.AccountButton.Text = "Account";
            this.AccountButton.UseVisualStyleBackColor = true;
            // 
            // MenuPanel
            // 
            this.MenuPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MenuPanel.BackColor = System.Drawing.Color.FromArgb(47, 97, 83);
            this.MenuPanel.Controls.Add(this.CurriculumManager);
            this.MenuPanel.Controls.Add(this.LogoutButton);
            this.MenuPanel.Controls.Add(this.GradingButton);
            this.MenuPanel.Controls.Add(this.ClassModerationButton);
            this.MenuPanel.Controls.Add(this.SystemConfigButton);
            this.MenuPanel.Controls.Add(this.ClassManagerButton);
            this.MenuPanel.Controls.Add(this.AccountManagerButton);
            this.MenuPanel.Controls.Add(this.HomeButton);
            this.MenuPanel.Controls.Add(this.MenuButton);
            this.MenuPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.MenuPanel.Location = new System.Drawing.Point(0, 79);
            this.MenuPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MenuPanel.MaximumSize = new System.Drawing.Size(262, 0);
            this.MenuPanel.MinimumSize = new System.Drawing.Size(82, 0);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(262, 936);
            this.MenuPanel.TabIndex = 2;
            // 
            // CurriculumManager
            // 
            this.CurriculumManager.AutoSize = true;
            this.CurriculumManager.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CurriculumManager.Dock = System.Windows.Forms.DockStyle.Top;
            this.CurriculumManager.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CurriculumManager.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.CurriculumManager.Image = global::Admin.Properties.Resources.Note;
            this.CurriculumManager.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CurriculumManager.Location = new System.Drawing.Point(0, 511);
            this.CurriculumManager.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CurriculumManager.MinimumSize = new System.Drawing.Size(11, 73);
            this.CurriculumManager.Name = "CurriculumManager";
            this.CurriculumManager.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.CurriculumManager.Size = new System.Drawing.Size(262, 73);
            this.CurriculumManager.TabIndex = 16;
            this.CurriculumManager.Text = "Curriculum Manager";
            this.CurriculumManager.UseVisualStyleBackColor = true;
            // 
            // LogoutButton
            // 
            this.LogoutButton.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.LogoutButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LogoutButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.LogoutButton.Image = global::Admin.Properties.Resources.Log_out;
            this.LogoutButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LogoutButton.Location = new System.Drawing.Point(0, 865);
            this.LogoutButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.LogoutButton.Size = new System.Drawing.Size(262, 71);
            this.LogoutButton.TabIndex = 15;
            this.LogoutButton.Text = "Logout";
            this.LogoutButton.UseVisualStyleBackColor = false;
            // 
            // GradingButton
            // 
            this.GradingButton.AutoSize = true;
            this.GradingButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GradingButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.GradingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GradingButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.GradingButton.Image = global::Admin.Properties.Resources.Document_Text;
            this.GradingButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GradingButton.Location = new System.Drawing.Point(0, 438);
            this.GradingButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GradingButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.GradingButton.Name = "GradingButton";
            this.GradingButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.GradingButton.Size = new System.Drawing.Size(262, 73);
            this.GradingButton.TabIndex = 14;
            this.GradingButton.Text = "Grading Sheet";
            this.GradingButton.UseVisualStyleBackColor = true;
            // 
            // ClassModerationButton
            // 
            this.ClassModerationButton.AutoSize = true;
            this.ClassModerationButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClassModerationButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.ClassModerationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClassModerationButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.ClassModerationButton.Image = global::Admin.Properties.Resources.Setting_4;
            this.ClassModerationButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ClassModerationButton.Location = new System.Drawing.Point(0, 365);
            this.ClassModerationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ClassModerationButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.ClassModerationButton.Name = "ClassModerationButton";
            this.ClassModerationButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.ClassModerationButton.Size = new System.Drawing.Size(262, 73);
            this.ClassModerationButton.TabIndex = 13;
            this.ClassModerationButton.Text = "Class Moderation";
            this.ClassModerationButton.UseVisualStyleBackColor = true;
            // 
            // SystemConfigButton
            // 
            this.SystemConfigButton.AutoSize = true;
            this.SystemConfigButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SystemConfigButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.SystemConfigButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SystemConfigButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.SystemConfigButton.Image = global::Admin.Properties.Resources.Setting_2;
            this.SystemConfigButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SystemConfigButton.Location = new System.Drawing.Point(0, 292);
            this.SystemConfigButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SystemConfigButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.SystemConfigButton.Name = "SystemConfigButton";
            this.SystemConfigButton.Padding = new System.Windows.Forms.Padding(11, 0, 29, 0);
            this.SystemConfigButton.Size = new System.Drawing.Size(262, 73);
            this.SystemConfigButton.TabIndex = 12;
            this.SystemConfigButton.Text = "System Configuration";
            this.SystemConfigButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SystemConfigButton.UseVisualStyleBackColor = true;
            // 
            // ClassManagerButton
            // 
            this.ClassManagerButton.AutoSize = true;
            this.ClassManagerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClassManagerButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.ClassManagerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ClassManagerButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //.ClassManagerButton.Image = global::Admin.Properties.Resources.People;
            this.ClassManagerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ClassManagerButton.Location = new System.Drawing.Point(0, 219);
            this.ClassManagerButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ClassManagerButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.ClassManagerButton.Name = "ClassManagerButton";
            this.ClassManagerButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.ClassManagerButton.Size = new System.Drawing.Size(262, 73);
            this.ClassManagerButton.TabIndex = 11;
            this.ClassManagerButton.Text = "Adding/Changing \r\nof Subjects";
            this.ClassManagerButton.UseVisualStyleBackColor = true;
            // 
            // AccountManagerButton
            // 
            this.AccountManagerButton.AutoSize = true;
            this.AccountManagerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AccountManagerButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.AccountManagerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AccountManagerButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.AccountManagerButton.Image = global::Admin.Properties.Resources.User_Edit;
            this.AccountManagerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.AccountManagerButton.Location = new System.Drawing.Point(0, 146);
            this.AccountManagerButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AccountManagerButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.AccountManagerButton.Name = "AccountManagerButton";
            this.AccountManagerButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.AccountManagerButton.Size = new System.Drawing.Size(262, 73);
            this.AccountManagerButton.TabIndex = 10;
            this.AccountManagerButton.Text = "Account Manager";
            this.AccountManagerButton.UseVisualStyleBackColor = true;
            // 
            // HomeButton
            // 
            this.HomeButton.AutoSize = true;
            this.HomeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HomeButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.HomeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HomeButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.HomeButton.Image = global::Admin.Properties.Resources.House_2;
            this.HomeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HomeButton.Location = new System.Drawing.Point(0, 73);
            this.HomeButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.HomeButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.HomeButton.Name = "HomeButton";
            this.HomeButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.HomeButton.Size = new System.Drawing.Size(262, 73);
            this.HomeButton.TabIndex = 9;
            this.HomeButton.Text = "Home";
            this.HomeButton.UseVisualStyleBackColor = true;
            // 
            // MenuButton
            // 
            this.MenuButton.AutoSize = true;
            this.MenuButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MenuButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.MenuButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MenuButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            //this.MenuButton.Image = global::Admin.Properties.Resources.Grid_4;
            this.MenuButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MenuButton.Location = new System.Drawing.Point(0, 0);
            this.MenuButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MenuButton.MinimumSize = new System.Drawing.Size(11, 73);
            this.MenuButton.Name = "MenuButton";
            this.MenuButton.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.MenuButton.Size = new System.Drawing.Size(262, 73);
            this.MenuButton.TabIndex = 8;
            this.MenuButton.Text = "Menu";
            this.MenuButton.UseVisualStyleBackColor = true;
            // 
            // slidingMenuTimer
            // 
            // 
            // AccountManager1
            // 
            this.AccountManager1.BackColor = System.Drawing.Color.FromArgb(255, 157, 0);
            this.AccountManager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccountManager1.Location = new System.Drawing.Point(262, 79);
            this.AccountManager1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.AccountManager1.Name = "AccountManager1";
            this.AccountManager1.Size = new System.Drawing.Size(1434, 936);
            this.AccountManager1.TabIndex = 3;
            // 
            // Class_Manager1
            // 
            /*this.Class_Manager1.BackColor = System.Drawing.Color.FromArgb(255, 157, 0);
            this.Class_Manager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Class_Manager1.Location = new System.Drawing.Point(262, 79);
            this.Class_Manager1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Class_Manager1.Name = "Class_Manager1";
            this.Class_Manager1.Size = new System.Drawing.Size(1434, 936);
            this.Class_Manager1.TabIndex = 4;*/
            // 
            // Curriculum_Manager1
            // 
            /*this.Curriculum_Manager1.BackColor = System.Drawing.Color.FromArgb(255, 157, 0);
            this.Curriculum_Manager1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Curriculum_Manager1.Location = new System.Drawing.Point(262, 79);
            this.Curriculum_Manager1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Curriculum_Manager1.Name = "Curriculum_Manager1";
            this.Curriculum_Manager1.Size = new System.Drawing.Size(1434, 936);
            this.Curriculum_Manager1.TabIndex = 5;*/
            // 
            // GradeAdminView1
            // 
            /*this.GradeAdminView1.BackColor = System.Drawing.Color.FromArgb(249, 218, 182);
            this.GradeAdminView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GradeAdminView1.Location = new System.Drawing.Point(262, 79);
            this.GradeAdminView1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.GradeAdminView1.MinimumSize = new System.Drawing.Size(1405, 939);
            this.GradeAdminView1.Name = "GradeAdminView1";
            this.GradeAdminView1.Size = new System.Drawing.Size(1434, 939);
            this.GradeAdminView1.TabIndex = 6;*/
            // 
            // Admin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 157, 0);
            this.ClientSize = new System.Drawing.Size(1696, 1015);
            //this.Controls.Add(this.GradeAdminView1);
            //this.Controls.Add(this.Curriculum_Manager1);
            //this.Controls.Add(this.Class_Manager1);
            this.Controls.Add(this.AccountManager1);
            this.Controls.Add(this.MenuPanel);
            this.Controls.Add(this.HeaderPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1597, 1018);
            this.Name = "Admin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CMS ADMIN";
            this.HeaderPanel.ResumeLayout(false);
            this.MenuPanel.ResumeLayout(false);
            this.MenuPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
