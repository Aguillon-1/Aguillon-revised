namespace CMS_Revised.User_Interface
{
    partial class ProfileAccountSettings
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
            NameLabel = new Label();
            RoleLabel = new Label();
            StudentNoLabel = new Label();
            FirstNameLabel = new Label();
            MiddleNameLabel = new Label();
            AddressLabel = new Label();
            BirthdayLabel = new Label();
            ContactNoLabel = new Label();
            LastNameLabel = new Label();
            panel1 = new Panel();
            editbutton = new Button();
            SuspendLayout();
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.BorderStyle = BorderStyle.FixedSingle;
            NameLabel.Location = new Point(226, 85);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(69, 17);
            NameLabel.TabIndex = 0;
            NameLabel.Text = "NameLabel";
            // 
            // RoleLabel
            // 
            RoleLabel.AutoSize = true;
            RoleLabel.BorderStyle = BorderStyle.FixedSingle;
            RoleLabel.Location = new Point(226, 119);
            RoleLabel.Name = "RoleLabel";
            RoleLabel.Size = new Size(60, 17);
            RoleLabel.TabIndex = 1;
            RoleLabel.Text = "RoleLabel";
            // 
            // StudentNoLabel
            // 
            StudentNoLabel.AutoSize = true;
            StudentNoLabel.BorderStyle = BorderStyle.FixedSingle;
            StudentNoLabel.Location = new Point(226, 102);
            StudentNoLabel.Name = "StudentNoLabel";
            StudentNoLabel.Size = new Size(94, 17);
            StudentNoLabel.TabIndex = 2;
            StudentNoLabel.Text = "StudentNoLabel";
            // 
            // FirstNameLabel
            // 
            FirstNameLabel.AutoSize = true;
            FirstNameLabel.BorderStyle = BorderStyle.FixedSingle;
            FirstNameLabel.Location = new Point(155, 260);
            FirstNameLabel.Name = "FirstNameLabel";
            FirstNameLabel.Size = new Size(91, 17);
            FirstNameLabel.TabIndex = 3;
            FirstNameLabel.Text = "FirstNameLabel";
            // 
            // MiddleNameLabel
            // 
            MiddleNameLabel.AutoSize = true;
            MiddleNameLabel.BorderStyle = BorderStyle.FixedSingle;
            MiddleNameLabel.Location = new Point(319, 260);
            MiddleNameLabel.Name = "MiddleNameLabel";
            MiddleNameLabel.Size = new Size(106, 17);
            MiddleNameLabel.TabIndex = 4;
            MiddleNameLabel.Text = "MiddleNameLabel";
            // 
            // AddressLabel
            // 
            AddressLabel.AutoSize = true;
            AddressLabel.BorderStyle = BorderStyle.FixedSingle;
            AddressLabel.Location = new Point(155, 332);
            AddressLabel.Name = "AddressLabel";
            AddressLabel.Size = new Size(79, 17);
            AddressLabel.TabIndex = 6;
            AddressLabel.Text = "AddressLabel";
            // 
            // BirthdayLabel
            // 
            BirthdayLabel.AutoSize = true;
            BirthdayLabel.BorderStyle = BorderStyle.FixedSingle;
            BirthdayLabel.Location = new Point(155, 295);
            BirthdayLabel.Name = "BirthdayLabel";
            BirthdayLabel.Size = new Size(81, 17);
            BirthdayLabel.TabIndex = 7;
            BirthdayLabel.Text = "BirthdayLabel";
            // 
            // ContactNoLabel
            // 
            ContactNoLabel.AutoSize = true;
            ContactNoLabel.BorderStyle = BorderStyle.FixedSingle;
            ContactNoLabel.Location = new Point(319, 295);
            ContactNoLabel.Name = "ContactNoLabel";
            ContactNoLabel.Size = new Size(95, 17);
            ContactNoLabel.TabIndex = 8;
            ContactNoLabel.Text = "ContactNoLabel";
            // 
            // LastNameLabel
            // 
            LastNameLabel.AutoSize = true;
            LastNameLabel.BorderStyle = BorderStyle.FixedSingle;
            LastNameLabel.Location = new Point(521, 260);
            LastNameLabel.Name = "LastNameLabel";
            LastNameLabel.Size = new Size(90, 17);
            LastNameLabel.TabIndex = 9;
            LastNameLabel.Text = "LastNameLabel";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveBorder;
            panel1.Location = new Point(113, 60);
            panel1.Name = "panel1";
            panel1.Size = new Size(108, 94);
            panel1.TabIndex = 10;
            // 
            // editbutton
            // 
            editbutton.Location = new Point(416, 461);
            editbutton.Name = "editbutton";
            editbutton.Size = new Size(75, 23);
            editbutton.TabIndex = 11;
            editbutton.Text = "editbutton";
            editbutton.UseVisualStyleBackColor = true;
            editbutton.Click += editbutton_Click;
            // 
            // ProfileAccountSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(editbutton);
            Controls.Add(panel1);
            Controls.Add(LastNameLabel);
            Controls.Add(ContactNoLabel);
            Controls.Add(BirthdayLabel);
            Controls.Add(AddressLabel);
            Controls.Add(MiddleNameLabel);
            Controls.Add(FirstNameLabel);
            Controls.Add(StudentNoLabel);
            Controls.Add(RoleLabel);
            Controls.Add(NameLabel);
            Name = "ProfileAccountSettings";
            Size = new Size(1024, 576);
            Load += ProfileAccountSettings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label NameLabel;
        private Label RoleLabel;
        private Label StudentNoLabel;
        private Label FirstNameLabel;
        private Label MiddleNameLabel;
        private Label AddressLabel;
        private Label BirthdayLabel;
        private Label ContactNoLabel;
        private Label LastNameLabel;

        // Add this method to handle the Load event
        private void ProfileAccountSettings_Load(object sender, EventArgs e)
        {
            // Add any initialization logic here if needed
        }
        private Panel panel1;
        private Button editbutton;
    }
}
