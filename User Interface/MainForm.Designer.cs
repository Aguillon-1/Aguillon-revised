namespace CMS_Revised.User_Interface
{
    partial class MainForm
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
            TopPanel = new Panel();
            MenuPanel = new Panel();
            MenuLogoutButton = new Button();
            MenuNotesButton = new Button();
            MenuCalendarButton = new Button();
            MenuAnnouncementsButton = new Button();
            MenuHomeButton = new Button();
            panel1 = new Panel();
            label1 = new Label();
            MenuCourseLabel = new Label();
            MenuStudentNoLabel = new Label();
            MenuNameLabel = new Label();
            MenuButton = new Button();
            UserMainPanel = new Panel();
            home1 = new Home();
            MenuPanel.SuspendLayout();
            panel1.SuspendLayout();
            UserMainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TopPanel
            // 
            TopPanel.Dock = DockStyle.Top;
            TopPanel.Location = new Point(0, 0);
            TopPanel.Name = "TopPanel";
            TopPanel.Size = new Size(1384, 67);
            TopPanel.TabIndex = 1;
            // 
            // MenuPanel
            // 
            MenuPanel.Controls.Add(MenuLogoutButton);
            MenuPanel.Controls.Add(MenuNotesButton);
            MenuPanel.Controls.Add(MenuCalendarButton);
            MenuPanel.Controls.Add(MenuAnnouncementsButton);
            MenuPanel.Controls.Add(MenuHomeButton);
            MenuPanel.Controls.Add(panel1);
            MenuPanel.Controls.Add(MenuButton);
            MenuPanel.Dock = DockStyle.Left;
            MenuPanel.Location = new Point(0, 67);
            MenuPanel.Name = "MenuPanel";
            MenuPanel.Size = new Size(225, 694);
            MenuPanel.TabIndex = 2;
            // 
            // MenuLogoutButton
            // 
            MenuLogoutButton.Dock = DockStyle.Bottom;
            MenuLogoutButton.Location = new Point(0, 639);
            MenuLogoutButton.Name = "MenuLogoutButton";
            MenuLogoutButton.Size = new Size(225, 55);
            MenuLogoutButton.TabIndex = 7;
            MenuLogoutButton.Text = "Logout";
            MenuLogoutButton.UseVisualStyleBackColor = true;
            // 
            // MenuNotesButton
            // 
            MenuNotesButton.Dock = DockStyle.Top;
            MenuNotesButton.Location = new Point(0, 333);
            MenuNotesButton.Name = "MenuNotesButton";
            MenuNotesButton.Size = new Size(225, 55);
            MenuNotesButton.TabIndex = 6;
            MenuNotesButton.Text = "Notes";
            MenuNotesButton.UseVisualStyleBackColor = true;
            // 
            // MenuCalendarButton
            // 
            MenuCalendarButton.Dock = DockStyle.Top;
            MenuCalendarButton.Location = new Point(0, 278);
            MenuCalendarButton.Name = "MenuCalendarButton";
            MenuCalendarButton.Size = new Size(225, 55);
            MenuCalendarButton.TabIndex = 5;
            MenuCalendarButton.Text = "Calendar";
            MenuCalendarButton.UseVisualStyleBackColor = true;
            // 
            // MenuAnnouncementsButton
            // 
            MenuAnnouncementsButton.Dock = DockStyle.Top;
            MenuAnnouncementsButton.Location = new Point(0, 223);
            MenuAnnouncementsButton.Name = "MenuAnnouncementsButton";
            MenuAnnouncementsButton.Size = new Size(225, 55);
            MenuAnnouncementsButton.TabIndex = 4;
            MenuAnnouncementsButton.Text = "Announcements";
            MenuAnnouncementsButton.UseVisualStyleBackColor = true;
            // 
            // MenuHomeButton
            // 
            MenuHomeButton.Dock = DockStyle.Top;
            MenuHomeButton.Location = new Point(0, 168);
            MenuHomeButton.Name = "MenuHomeButton";
            MenuHomeButton.Size = new Size(225, 55);
            MenuHomeButton.TabIndex = 3;
            MenuHomeButton.Text = "Home";
            MenuHomeButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(MenuCourseLabel);
            panel1.Controls.Add(MenuStudentNoLabel);
            panel1.Controls.Add(MenuNameLabel);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 55);
            panel1.Name = "panel1";
            panel1.Size = new Size(225, 113);
            panel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(23, 49);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 3;
            label1.Text = "Icon";
            // 
            // MenuCourseLabel
            // 
            MenuCourseLabel.AutoSize = true;
            MenuCourseLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MenuCourseLabel.Location = new Point(91, 79);
            MenuCourseLabel.Name = "MenuCourseLabel";
            MenuCourseLabel.Size = new Size(38, 15);
            MenuCourseLabel.TabIndex = 2;
            MenuCourseLabel.Text = "C/Y/S";
            // 
            // MenuStudentNoLabel
            // 
            MenuStudentNoLabel.AutoSize = true;
            MenuStudentNoLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MenuStudentNoLabel.Location = new Point(91, 49);
            MenuStudentNoLabel.Name = "MenuStudentNoLabel";
            MenuStudentNoLabel.Size = new Size(68, 15);
            MenuStudentNoLabel.TabIndex = 1;
            MenuStudentNoLabel.Text = "StudentNo:";
            // 
            // MenuNameLabel
            // 
            MenuNameLabel.AutoSize = true;
            MenuNameLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            MenuNameLabel.Location = new Point(90, 15);
            MenuNameLabel.Name = "MenuNameLabel";
            MenuNameLabel.Size = new Size(42, 15);
            MenuNameLabel.TabIndex = 0;
            MenuNameLabel.Text = "Name:";
            // 
            // MenuButton
            // 
            MenuButton.Dock = DockStyle.Top;
            MenuButton.Location = new Point(0, 0);
            MenuButton.Name = "MenuButton";
            MenuButton.Size = new Size(225, 55);
            MenuButton.TabIndex = 0;
            MenuButton.Text = "Menu";
            MenuButton.UseVisualStyleBackColor = true;
            // 
            // UserMainPanel
            // 
            UserMainPanel.Controls.Add(home1);
            UserMainPanel.Dock = DockStyle.Fill;
            UserMainPanel.Location = new Point(225, 67);
            UserMainPanel.Name = "UserMainPanel";
            UserMainPanel.Size = new Size(1159, 694);
            UserMainPanel.TabIndex = 3;
            // 
            // home1
            // 
            home1.Dock = DockStyle.Fill;
            home1.Location = new Point(0, 0);
            home1.Name = "home1";
            home1.Size = new Size(1159, 694);
            home1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1384, 761);
            Controls.Add(UserMainPanel);
            Controls.Add(MenuPanel);
            Controls.Add(TopPanel);
            MinimumSize = new Size(1400, 800);
            Name = "MainForm";
            MenuPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            UserMainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel TopPanel;
        private Panel MenuPanel;
        private Button MenuButton;
        private Panel panel1;
        private Label MenuStudentNoLabel;
        private Label MenuNameLabel;
        private Button MenuNotesButton;
        private Button MenuCalendarButton;
        private Button MenuAnnouncementsButton;
        private Button MenuHomeButton;
        private Label label1;
        private Label MenuCourseLabel;
        private Button MenuLogoutButton;
        private Panel UserMainPanel;
        private Home home1;
    }
}