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
            ProfilePicturepanel = new Panel();
            editbutton = new Button();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            Discardbtn = new Button();
            Savebtn = new Button();
            FirstNameTextBox = new TextBox();
            MiddleNameTextBox = new TextBox();
            LastNameTextBox = new TextBox();
            BirthdayTextBox = new TextBox();
            ContactNoTextBox = new TextBox();
            AddressTextBox = new TextBox();
            NameTextBox = new TextBox();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // NameLabel
            // 
            NameLabel.AutoSize = true;
            NameLabel.BorderStyle = BorderStyle.FixedSingle;
            NameLabel.Location = new Point(265, 40);
            NameLabel.Name = "NameLabel";
            NameLabel.Size = new Size(69, 17);
            NameLabel.TabIndex = 0;
            NameLabel.Text = "NameLabel";
            // 
            // RoleLabel
            // 
            RoleLabel.AutoSize = true;
            RoleLabel.BorderStyle = BorderStyle.FixedSingle;
            RoleLabel.Location = new Point(265, 99);
            RoleLabel.Name = "RoleLabel";
            RoleLabel.Size = new Size(60, 17);
            RoleLabel.TabIndex = 1;
            RoleLabel.Text = "RoleLabel";
            // 
            // StudentNoLabel
            // 
            StudentNoLabel.AutoSize = true;
            StudentNoLabel.BorderStyle = BorderStyle.FixedSingle;
            StudentNoLabel.Location = new Point(265, 82);
            StudentNoLabel.Name = "StudentNoLabel";
            StudentNoLabel.Size = new Size(94, 17);
            StudentNoLabel.TabIndex = 2;
            StudentNoLabel.Text = "StudentNoLabel";
            // 
            // FirstNameLabel
            // 
            FirstNameLabel.AutoSize = true;
            FirstNameLabel.BorderStyle = BorderStyle.FixedSingle;
            FirstNameLabel.Location = new Point(202, 172);
            FirstNameLabel.Name = "FirstNameLabel";
            FirstNameLabel.Size = new Size(91, 17);
            FirstNameLabel.TabIndex = 3;
            FirstNameLabel.Text = "FirstNameLabel";
            // 
            // MiddleNameLabel
            // 
            MiddleNameLabel.AutoSize = true;
            MiddleNameLabel.BorderStyle = BorderStyle.FixedSingle;
            MiddleNameLabel.Location = new Point(366, 172);
            MiddleNameLabel.Name = "MiddleNameLabel";
            MiddleNameLabel.Size = new Size(106, 17);
            MiddleNameLabel.TabIndex = 4;
            MiddleNameLabel.Text = "MiddleNameLabel";
            // 
            // AddressLabel
            // 
            AddressLabel.AutoSize = true;
            AddressLabel.BorderStyle = BorderStyle.FixedSingle;
            AddressLabel.Location = new Point(204, 344);
            AddressLabel.Name = "AddressLabel";
            AddressLabel.Size = new Size(79, 17);
            AddressLabel.TabIndex = 6;
            AddressLabel.Text = "AddressLabel";
            // 
            // BirthdayLabel
            // 
            BirthdayLabel.AutoSize = true;
            BirthdayLabel.BorderStyle = BorderStyle.FixedSingle;
            BirthdayLabel.Location = new Point(202, 247);
            BirthdayLabel.Name = "BirthdayLabel";
            BirthdayLabel.Size = new Size(81, 17);
            BirthdayLabel.TabIndex = 7;
            BirthdayLabel.Text = "BirthdayLabel";
            // 
            // ContactNoLabel
            // 
            ContactNoLabel.AutoSize = true;
            ContactNoLabel.BorderStyle = BorderStyle.FixedSingle;
            ContactNoLabel.Location = new Point(366, 247);
            ContactNoLabel.Name = "ContactNoLabel";
            ContactNoLabel.Size = new Size(95, 17);
            ContactNoLabel.TabIndex = 8;
            ContactNoLabel.Text = "ContactNoLabel";
            // 
            // LastNameLabel
            // 
            LastNameLabel.AutoSize = true;
            LastNameLabel.BorderStyle = BorderStyle.FixedSingle;
            LastNameLabel.Location = new Point(535, 172);
            LastNameLabel.Name = "LastNameLabel";
            LastNameLabel.Size = new Size(90, 17);
            LastNameLabel.TabIndex = 9;
            LastNameLabel.Text = "LastNameLabel";
            // 
            // ProfilePicturepanel
            // 
            ProfilePicturepanel.BackColor = SystemColors.ActiveBorder;
            ProfilePicturepanel.Location = new Point(152, 40);
            ProfilePicturepanel.Name = "ProfilePicturepanel";
            ProfilePicturepanel.Size = new Size(108, 94);
            ProfilePicturepanel.TabIndex = 10;
            ProfilePicturepanel.Paint += ProfilePicturepanel_Paint;
            // 
            // editbutton
            // 
            editbutton.Location = new Point(669, 436);
            editbutton.Name = "editbutton";
            editbutton.Size = new Size(75, 45);
            editbutton.TabIndex = 11;
            editbutton.Text = "Edit";
            editbutton.UseVisualStyleBackColor = true;
            editbutton.Click += editbutton_Click;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // Discardbtn
            // 
            Discardbtn.Location = new Point(759, 436);
            Discardbtn.Name = "Discardbtn";
            Discardbtn.Size = new Size(75, 45);
            Discardbtn.TabIndex = 12;
            Discardbtn.Text = "Discard Changes";
            Discardbtn.UseVisualStyleBackColor = true;
            Discardbtn.Click += Discardbtn_Click;
            // 
            // Savebtn
            // 
            Savebtn.Location = new Point(850, 436);
            Savebtn.Name = "Savebtn";
            Savebtn.Size = new Size(75, 45);
            Savebtn.TabIndex = 13;
            Savebtn.Text = "Save";
            Savebtn.UseVisualStyleBackColor = true;
            Savebtn.Click += Savebtn_Click;
            // 
            // FirstNameTextBox
            // 
            FirstNameTextBox.Location = new Point(202, 192);
            FirstNameTextBox.Name = "FirstNameTextBox";
            FirstNameTextBox.ReadOnly = true;
            FirstNameTextBox.Size = new Size(100, 23);
            FirstNameTextBox.TabIndex = 14;
            FirstNameTextBox.TextChanged += FirstNameTextBox_TextChanged;
            // 
            // MiddleNameTextBox
            // 
            MiddleNameTextBox.Location = new Point(366, 192);
            MiddleNameTextBox.Name = "MiddleNameTextBox";
            MiddleNameTextBox.ReadOnly = true;
            MiddleNameTextBox.Size = new Size(100, 23);
            MiddleNameTextBox.TabIndex = 15;
            MiddleNameTextBox.TextChanged += MiddleNameTextBox_TextChanged;
            // 
            // LastNameTextBox
            // 
            LastNameTextBox.Location = new Point(535, 192);
            LastNameTextBox.Name = "LastNameTextBox";
            LastNameTextBox.ReadOnly = true;
            LastNameTextBox.Size = new Size(100, 23);
            LastNameTextBox.TabIndex = 16;
            LastNameTextBox.TextChanged += LastNameTextBox_TextChanged;
            // 
            // BirthdayTextBox
            // 
            BirthdayTextBox.Location = new Point(202, 267);
            BirthdayTextBox.Name = "BirthdayTextBox";
            BirthdayTextBox.ReadOnly = true;
            BirthdayTextBox.Size = new Size(100, 23);
            BirthdayTextBox.TabIndex = 17;
            BirthdayTextBox.TextChanged += BirthdayTextBox_TextChanged;
            // 
            // ContactNoTextBox
            // 
            ContactNoTextBox.Location = new Point(366, 267);
            ContactNoTextBox.Name = "ContactNoTextBox";
            ContactNoTextBox.ReadOnly = true;
            ContactNoTextBox.Size = new Size(100, 23);
            ContactNoTextBox.TabIndex = 18;
            ContactNoTextBox.TextChanged += ContactNoTextBox_TextChanged;
            // 
            // AddressTextBox
            // 
            AddressTextBox.Location = new Point(202, 364);
            AddressTextBox.Name = "AddressTextBox";
            AddressTextBox.ReadOnly = true;
            AddressTextBox.Size = new Size(100, 23);
            AddressTextBox.TabIndex = 19;
            AddressTextBox.TextChanged += AddressTextBox_TextChanged;
            // 
            // NameTextBox
            // 
            NameTextBox.Location = new Point(265, 56);
            NameTextBox.Name = "NameTextBox";
            NameTextBox.ReadOnly = true;
            NameTextBox.Size = new Size(100, 23);
            NameTextBox.TabIndex = 20;
            NameTextBox.TextChanged += NameTextBox_TextChanged;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveBorder;
            panel1.Location = new Point(152, 140);
            panel1.Name = "panel1";
            panel1.Size = new Size(813, 360);
            panel1.TabIndex = 21;
            // 
            // ProfileAccountSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(NameTextBox);
            Controls.Add(AddressTextBox);
            Controls.Add(ContactNoTextBox);
            Controls.Add(BirthdayTextBox);
            Controls.Add(LastNameTextBox);
            Controls.Add(MiddleNameTextBox);
            Controls.Add(FirstNameTextBox);
            Controls.Add(Savebtn);
            Controls.Add(Discardbtn);
            Controls.Add(editbutton);
            Controls.Add(ProfilePicturepanel);
            Controls.Add(LastNameLabel);
            Controls.Add(ContactNoLabel);
            Controls.Add(BirthdayLabel);
            Controls.Add(AddressLabel);
            Controls.Add(MiddleNameLabel);
            Controls.Add(FirstNameLabel);
            Controls.Add(StudentNoLabel);
            Controls.Add(RoleLabel);
            Controls.Add(NameLabel);
            Controls.Add(panel1);
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
        private Panel ProfilePicturepanel;
        private Button editbutton;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private Button Discardbtn;
        private Button Savebtn;
        private TextBox FirstNameTextBox;
        private TextBox MiddleNameTextBox;
        private TextBox LastNameTextBox;
        private TextBox BirthdayTextBox;
        private TextBox ContactNoTextBox;
        private TextBox AddressTextBox;
        private TextBox NameTextBox;
        private Panel panel1;
    }
}
