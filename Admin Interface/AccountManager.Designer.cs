namespace ClassroomManagementSystem
{
    partial class AccountManager
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
            SaveButton = new Button();
            UserDataGrid = new DataGridView();
            dbSearchTextBox = new TextBox();
            PictureBox1 = new PictureBox();
            FnameTextBox = new TextBox();
            FnameLabel = new Label();
            MnameLabel = new Label();
            MnameTextBox = new TextBox();
            Lname = new Label();
            LnameTextBox = new TextBox();
            RoleLabel = new Label();
            RoleComboBox = new ComboBox();
            DbsortCheckedListBox = new CheckedListBox();
            StudentNoTextBox = new Label();
            TextBox1 = new TextBox();
            EmailLabel = new Label();
            CourseCombobox = new ComboBox();
            CourseLabel = new Label();
            SectionCombobox = new ComboBox();
            SectionLabel = new Label();
            PasswordLabel = new Label();
            YearLabel = new Label();
            EmailTextBox = new TextBox();
            PasswordTextBox = new TextBox();
            YearCombobox = new ComboBox();
            PassEyeButton = new Button();
            ClearButton = new Button();
            DeleteButton = new Button();
            ContactNoTextBox = new TextBox();
            ContactNoLabel = new Label();
            StudentStatusCombobox = new ComboBox();
            StudentStatusLabel = new Label();
            SchoolYRCombobox = new ComboBox();
            SYLabel = new Label();
            SexCombobox = new ComboBox();
            SexLabel = new Label();
            ComboBox4 = new ComboBox();
            AcademicStatusLabel = new Label();
            BdayLabel = new Label();
            Bdaypicket = new DateTimePicker();
            Editbtn = new Button();
            ((System.ComponentModel.ISupportInitialize)UserDataGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            SuspendLayout();
            // 
            // SaveButton
            // 
            SaveButton.BackColor = Color.FromArgb(72, 229, 189);
            SaveButton.Location = new Point(1086, 651);
            SaveButton.Margin = new Padding(2, 1, 2, 1);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(130, 42);
            SaveButton.TabIndex = 0;
            SaveButton.Text = "SAVE";
            SaveButton.UseVisualStyleBackColor = false;
            SaveButton.Click += SaveButton_Click;
            // 
            // UserDataGrid
            // 
            UserDataGrid.AllowUserToAddRows = false;
            UserDataGrid.AllowUserToDeleteRows = false;
            UserDataGrid.AllowUserToResizeColumns = false;
            UserDataGrid.AllowUserToResizeRows = false;
            UserDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            UserDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            UserDataGrid.Location = new Point(22, 14);
            UserDataGrid.Margin = new Padding(2, 1, 2, 1);
            UserDataGrid.Name = "UserDataGrid";
            UserDataGrid.ReadOnly = true;
            UserDataGrid.RowHeadersWidth = 82;
            UserDataGrid.Size = new Size(1069, 272);
            UserDataGrid.TabIndex = 1;
            UserDataGrid.CellClick += UserDataGrid_CellClick;
            // 
            // dbSearchTextBox
            // 
            dbSearchTextBox.Font = new Font("Segoe UI", 10F);
            dbSearchTextBox.Location = new Point(60, 290);
            dbSearchTextBox.Margin = new Padding(2, 1, 2, 1);
            dbSearchTextBox.MaximumSize = new Size(300, 34);
            dbSearchTextBox.MinimumSize = new Size(273, 34);
            dbSearchTextBox.Multiline = true;
            dbSearchTextBox.Name = "dbSearchTextBox";
            dbSearchTextBox.Size = new Size(300, 34);
            dbSearchTextBox.TabIndex = 2;
            dbSearchTextBox.Text = "Search Name, ID and ect...";
            // 
            // PictureBox1
            // 
            PictureBox1.Location = new Point(22, 290);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(32, 34);
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox1.TabIndex = 3;
            PictureBox1.TabStop = false;
            // 
            // FnameTextBox
            // 
            FnameTextBox.Font = new Font("Segoe UI", 11F);
            FnameTextBox.Location = new Point(22, 358);
            FnameTextBox.Name = "FnameTextBox";
            FnameTextBox.Size = new Size(158, 27);
            FnameTextBox.TabIndex = 4;
            FnameTextBox.TextChanged += FnameTextBox_TextChanged;
            // 
            // FnameLabel
            // 
            FnameLabel.AutoSize = true;
            FnameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            FnameLabel.Location = new Point(21, 340);
            FnameLabel.Name = "FnameLabel";
            FnameLabel.Size = new Size(67, 15);
            FnameLabel.TabIndex = 6;
            FnameLabel.Text = "First Name";
            // 
            // MnameLabel
            // 
            MnameLabel.AutoSize = true;
            MnameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            MnameLabel.Location = new Point(206, 340);
            MnameLabel.Name = "MnameLabel";
            MnameLabel.Size = new Size(81, 15);
            MnameLabel.TabIndex = 8;
            MnameLabel.Text = "Middle Name";
            // 
            // MnameTextBox
            // 
            MnameTextBox.Font = new Font("Segoe UI", 11F);
            MnameTextBox.Location = new Point(206, 358);
            MnameTextBox.Name = "MnameTextBox";
            MnameTextBox.Size = new Size(158, 27);
            MnameTextBox.TabIndex = 7;
            MnameTextBox.TextChanged += MnameTextBox_TextChanged;
            // 
            // Lname
            // 
            Lname.AutoSize = true;
            Lname.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Lname.Location = new Point(389, 340);
            Lname.Name = "Lname";
            Lname.Size = new Size(65, 15);
            Lname.TabIndex = 10;
            Lname.Text = "Last Name";
            // 
            // LnameTextBox
            // 
            LnameTextBox.Font = new Font("Segoe UI", 11F);
            LnameTextBox.Location = new Point(389, 358);
            LnameTextBox.Name = "LnameTextBox";
            LnameTextBox.Size = new Size(158, 27);
            LnameTextBox.TabIndex = 9;
            LnameTextBox.TextChanged += LnameTextBox_TextChanged;
            // 
            // RoleLabel
            // 
            RoleLabel.AutoSize = true;
            RoleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            RoleLabel.Location = new Point(21, 397);
            RoleLabel.Name = "RoleLabel";
            RoleLabel.Size = new Size(35, 15);
            RoleLabel.TabIndex = 11;
            RoleLabel.Text = "Role:";
            // 
            // RoleComboBox
            // 
            RoleComboBox.Font = new Font("Segoe UI", 11F);
            RoleComboBox.FormattingEnabled = true;
            RoleComboBox.Items.AddRange(new object[] { "Student", "Faculty", "Admin-MIS", "Registrar" });
            RoleComboBox.Location = new Point(22, 415);
            RoleComboBox.Name = "RoleComboBox";
            RoleComboBox.Size = new Size(158, 28);
            RoleComboBox.TabIndex = 15;
            // 
            // DbsortCheckedListBox
            // 
            DbsortCheckedListBox.BackColor = Color.FromArgb(72, 229, 189);
            DbsortCheckedListBox.FormattingEnabled = true;
            DbsortCheckedListBox.Location = new Point(1096, 14);
            DbsortCheckedListBox.Name = "DbsortCheckedListBox";
            DbsortCheckedListBox.Size = new Size(120, 274);
            DbsortCheckedListBox.TabIndex = 17;
            // 
            // StudentNoTextBox
            // 
            StudentNoTextBox.AutoSize = true;
            StudentNoTextBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            StudentNoTextBox.Location = new Point(574, 340);
            StudentNoTextBox.Name = "StudentNoTextBox";
            StudentNoTextBox.Size = new Size(74, 15);
            StudentNoTextBox.TabIndex = 19;
            StudentNoTextBox.Text = "Student No.";
            // 
            // TextBox1
            // 
            TextBox1.Font = new Font("Segoe UI", 11F);
            TextBox1.Location = new Point(574, 358);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new Size(158, 27);
            TextBox1.TabIndex = 18;
            TextBox1.TextChanged += TextBox1_TextChanged;
            // 
            // EmailLabel
            // 
            EmailLabel.AutoSize = true;
            EmailLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            EmailLabel.Location = new Point(21, 464);
            EmailLabel.Name = "EmailLabel";
            EmailLabel.Size = new Size(39, 15);
            EmailLabel.TabIndex = 20;
            EmailLabel.Text = "Email:";
            // 
            // CourseCombobox
            // 
            CourseCombobox.Font = new Font("Segoe UI", 11F);
            CourseCombobox.FormattingEnabled = true;
            CourseCombobox.Items.AddRange(new object[] { "BS Computer Science", "BS Information Technology", "BS Information System", "BS Entertainment and Multimedia Computing" });
            CourseCombobox.Location = new Point(388, 415);
            CourseCombobox.Name = "CourseCombobox";
            CourseCombobox.Size = new Size(159, 28);
            CourseCombobox.TabIndex = 23;
            CourseCombobox.Text = "Select course";
            // 
            // CourseLabel
            // 
            CourseLabel.AutoSize = true;
            CourseLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            CourseLabel.Location = new Point(387, 397);
            CourseLabel.Name = "CourseLabel";
            CourseLabel.Size = new Size(45, 15);
            CourseLabel.TabIndex = 22;
            CourseLabel.Text = "Course";
            // 
            // SectionCombobox
            // 
            SectionCombobox.Font = new Font("Segoe UI", 11F);
            SectionCombobox.FormattingEnabled = true;
            SectionCombobox.Items.AddRange(new object[] { "A", "B", "C" });
            SectionCombobox.Location = new Point(574, 415);
            SectionCombobox.Name = "SectionCombobox";
            SectionCombobox.Size = new Size(158, 28);
            SectionCombobox.TabIndex = 25;
            SectionCombobox.Text = "select section";
            // 
            // SectionLabel
            // 
            SectionLabel.AutoSize = true;
            SectionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            SectionLabel.Location = new Point(573, 397);
            SectionLabel.Name = "SectionLabel";
            SectionLabel.Size = new Size(49, 15);
            SectionLabel.TabIndex = 24;
            SectionLabel.Text = "Section";
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            PasswordLabel.Location = new Point(22, 520);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(59, 15);
            PasswordLabel.TabIndex = 12;
            PasswordLabel.Text = "Password";
            // 
            // YearLabel
            // 
            YearLabel.AutoSize = true;
            YearLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            YearLabel.Location = new Point(206, 397);
            YearLabel.Name = "YearLabel";
            YearLabel.Size = new Size(31, 15);
            YearLabel.TabIndex = 26;
            YearLabel.Text = "Year";
            // 
            // EmailTextBox
            // 
            EmailTextBox.Font = new Font("Segoe UI", 11F);
            EmailTextBox.Location = new Point(22, 482);
            EmailTextBox.Name = "EmailTextBox";
            EmailTextBox.Size = new Size(158, 27);
            EmailTextBox.TabIndex = 28;
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Font = new Font("Segoe UI", 11F);
            PasswordTextBox.Location = new Point(22, 538);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new Size(177, 27);
            PasswordTextBox.TabIndex = 29;
            PasswordTextBox.TextChanged += PasswordTextBox_TextChanged;
            // 
            // YearCombobox
            // 
            YearCombobox.Font = new Font("Segoe UI", 11F);
            YearCombobox.FormattingEnabled = true;
            YearCombobox.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            YearCombobox.Location = new Point(206, 415);
            YearCombobox.Name = "YearCombobox";
            YearCombobox.Size = new Size(158, 28);
            YearCombobox.TabIndex = 30;
            YearCombobox.Text = "Select year";
            // 
            // PassEyeButton
            // 
            PassEyeButton.Location = new Point(205, 537);
            PassEyeButton.Name = "PassEyeButton";
            PassEyeButton.Size = new Size(34, 28);
            PassEyeButton.TabIndex = 31;
            PassEyeButton.Text = "H";
            PassEyeButton.UseVisualStyleBackColor = true;
            PassEyeButton.Click += PassEyeButton_Click;
            // 
            // ClearButton
            // 
            ClearButton.BackColor = Color.FromArgb(34, 40, 23);
            ClearButton.ForeColor = SystemColors.ButtonHighlight;
            ClearButton.Location = new Point(1086, 607);
            ClearButton.Margin = new Padding(2, 1, 2, 1);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(130, 42);
            ClearButton.TabIndex = 33;
            ClearButton.Text = "CLEAR";
            ClearButton.UseVisualStyleBackColor = false;
            ClearButton.Click += ClearButton_Click;
            // 
            // DeleteButton
            // 
            DeleteButton.BackColor = Color.FromArgb(192, 0, 0);
            DeleteButton.ForeColor = SystemColors.ButtonFace;
            DeleteButton.Location = new Point(1086, 563);
            DeleteButton.Margin = new Padding(2, 1, 2, 1);
            DeleteButton.Name = "DeleteButton";
            DeleteButton.Size = new Size(130, 42);
            DeleteButton.TabIndex = 34;
            DeleteButton.Text = "DELETE";
            DeleteButton.UseVisualStyleBackColor = false;
            DeleteButton.Click += DeleteButton_Click;
            // 
            // ContactNoTextBox
            // 
            ContactNoTextBox.Font = new Font("Segoe UI", 11F);
            ContactNoTextBox.Location = new Point(200, 482);
            ContactNoTextBox.Name = "ContactNoTextBox";
            ContactNoTextBox.Size = new Size(158, 27);
            ContactNoTextBox.TabIndex = 44;
            // 
            // ContactNoLabel
            // 
            ContactNoLabel.AutoSize = true;
            ContactNoLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            ContactNoLabel.Location = new Point(199, 464);
            ContactNoLabel.Name = "ContactNoLabel";
            ContactNoLabel.Size = new Size(69, 15);
            ContactNoLabel.TabIndex = 43;
            ContactNoLabel.Text = "Contact No";
            // 
            // StudentStatusCombobox
            // 
            StudentStatusCombobox.Font = new Font("Segoe UI", 11F);
            StudentStatusCombobox.FormattingEnabled = true;
            StudentStatusCombobox.Items.AddRange(new object[] { "Regular", "Irregular" });
            StudentStatusCombobox.Location = new Point(758, 358);
            StudentStatusCombobox.Name = "StudentStatusCombobox";
            StudentStatusCombobox.Size = new Size(158, 28);
            StudentStatusCombobox.TabIndex = 46;
            StudentStatusCombobox.Text = "Regular";
            // 
            // StudentStatusLabel
            // 
            StudentStatusLabel.AutoSize = true;
            StudentStatusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            StudentStatusLabel.Location = new Point(757, 340);
            StudentStatusLabel.Name = "StudentStatusLabel";
            StudentStatusLabel.Size = new Size(90, 15);
            StudentStatusLabel.TabIndex = 45;
            StudentStatusLabel.Text = "Student Status";
            // 
            // SchoolYRCombobox
            // 
            SchoolYRCombobox.Font = new Font("Segoe UI", 11F);
            SchoolYRCombobox.FormattingEnabled = true;
            SchoolYRCombobox.Items.AddRange(new object[] { "24-25", "25-26" });
            SchoolYRCombobox.Location = new Point(758, 415);
            SchoolYRCombobox.Name = "SchoolYRCombobox";
            SchoolYRCombobox.Size = new Size(158, 28);
            SchoolYRCombobox.TabIndex = 48;
            SchoolYRCombobox.Text = "Select SY";
            // 
            // SYLabel
            // 
            SYLabel.AutoSize = true;
            SYLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            SYLabel.Location = new Point(757, 397);
            SYLabel.Name = "SYLabel";
            SYLabel.Size = new Size(71, 15);
            SYLabel.TabIndex = 47;
            SYLabel.Text = "School Year";
            // 
            // SexCombobox
            // 
            SexCombobox.Font = new Font("Segoe UI", 11F);
            SexCombobox.FormattingEnabled = true;
            SexCombobox.Items.AddRange(new object[] { "Male", "Female" });
            SexCombobox.Location = new Point(386, 481);
            SexCombobox.Name = "SexCombobox";
            SexCombobox.Size = new Size(159, 28);
            SexCombobox.TabIndex = 50;
            SexCombobox.Text = "Select sex";
            // 
            // SexLabel
            // 
            SexLabel.AutoSize = true;
            SexLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            SexLabel.Location = new Point(386, 463);
            SexLabel.Name = "SexLabel";
            SexLabel.Size = new Size(28, 15);
            SexLabel.TabIndex = 49;
            SexLabel.Text = "Sex";
            // 
            // ComboBox4
            // 
            ComboBox4.Font = new Font("Segoe UI", 11F);
            ComboBox4.FormattingEnabled = true;
            ComboBox4.Items.AddRange(new object[] { "Active", "Graduated", "Returnee", "Transferee", "Deactivated" });
            ComboBox4.Location = new Point(757, 478);
            ComboBox4.Name = "ComboBox4";
            ComboBox4.Size = new Size(159, 28);
            ComboBox4.TabIndex = 52;
            ComboBox4.Text = "Select Academic status";
            // 
            // AcademicStatusLabel
            // 
            AcademicStatusLabel.AutoSize = true;
            AcademicStatusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            AcademicStatusLabel.Location = new Point(757, 460);
            AcademicStatusLabel.Name = "AcademicStatusLabel";
            AcademicStatusLabel.Size = new Size(99, 15);
            AcademicStatusLabel.TabIndex = 51;
            AcademicStatusLabel.Text = "Academic Status";
            // 
            // BdayLabel
            // 
            BdayLabel.AutoSize = true;
            BdayLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            BdayLabel.Location = new Point(573, 465);
            BdayLabel.Name = "BdayLabel";
            BdayLabel.Size = new Size(54, 15);
            BdayLabel.TabIndex = 53;
            BdayLabel.Text = "Birthday";
            // 
            // Bdaypicket
            // 
            Bdaypicket.CalendarFont = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Bdaypicket.Location = new Point(573, 483);
            Bdaypicket.Name = "Bdaypicket";
            Bdaypicket.Size = new Size(159, 23);
            Bdaypicket.TabIndex = 54;
            // 
            // Editbtn
            // 
            Editbtn.Location = new Point(1086, 520);
            Editbtn.Name = "Editbtn";
            Editbtn.Size = new Size(130, 39);
            Editbtn.TabIndex = 55;
            Editbtn.Text = "EDIT";
            Editbtn.UseVisualStyleBackColor = true;
            Editbtn.Click += Editbtn_Click;
            // 
            // AccountManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 157, 0);
            Controls.Add(Editbtn);
            Controls.Add(Bdaypicket);
            Controls.Add(BdayLabel);
            Controls.Add(ComboBox4);
            Controls.Add(AcademicStatusLabel);
            Controls.Add(SexCombobox);
            Controls.Add(SexLabel);
            Controls.Add(SchoolYRCombobox);
            Controls.Add(SYLabel);
            Controls.Add(StudentStatusCombobox);
            Controls.Add(StudentStatusLabel);
            Controls.Add(ContactNoTextBox);
            Controls.Add(ContactNoLabel);
            Controls.Add(DeleteButton);
            Controls.Add(ClearButton);
            Controls.Add(PassEyeButton);
            Controls.Add(YearCombobox);
            Controls.Add(PasswordTextBox);
            Controls.Add(EmailTextBox);
            Controls.Add(YearLabel);
            Controls.Add(SectionCombobox);
            Controls.Add(SectionLabel);
            Controls.Add(CourseCombobox);
            Controls.Add(CourseLabel);
            Controls.Add(EmailLabel);
            Controls.Add(StudentNoTextBox);
            Controls.Add(TextBox1);
            Controls.Add(DbsortCheckedListBox);
            Controls.Add(RoleComboBox);
            Controls.Add(PasswordLabel);
            Controls.Add(RoleLabel);
            Controls.Add(Lname);
            Controls.Add(LnameTextBox);
            Controls.Add(MnameLabel);
            Controls.Add(MnameTextBox);
            Controls.Add(FnameLabel);
            Controls.Add(FnameTextBox);
            Controls.Add(PictureBox1);
            Controls.Add(dbSearchTextBox);
            Controls.Add(UserDataGrid);
            Controls.Add(SaveButton);
            Name = "AccountManager";
            Size = new Size(1229, 704);
            Load += AccountManager_Load;
            ((System.ComponentModel.ISupportInitialize)UserDataGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.DataGridView UserDataGrid;
        private System.Windows.Forms.TextBox dbSearchTextBox;
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.TextBox FnameTextBox;
        private System.Windows.Forms.Label FnameLabel;
        private System.Windows.Forms.Label MnameLabel;
        private System.Windows.Forms.TextBox MnameTextBox;
        private System.Windows.Forms.Label Lname;
        private System.Windows.Forms.TextBox LnameTextBox;
        private System.Windows.Forms.Label RoleLabel;
        private System.Windows.Forms.ComboBox RoleComboBox;
        private System.Windows.Forms.CheckedListBox DbsortCheckedListBox;
        private System.Windows.Forms.Label StudentNoTextBox;
        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.ComboBox YearCombobox;
        private System.Windows.Forms.Label EmailLabel;
        private System.Windows.Forms.ComboBox CourseCombobox;
        private System.Windows.Forms.Label CourseLabel;
        private System.Windows.Forms.ComboBox SectionCombobox;
        private System.Windows.Forms.Label SectionLabel;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.ComboBox ComboBox4;
        private System.Windows.Forms.Label YearLabel;
        private System.Windows.Forms.TextBox EmailTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Button PassEyeButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.TextBox ContactNoTextBox;
        private System.Windows.Forms.Label ContactNoLabel;
        private System.Windows.Forms.ComboBox StudentStatusCombobox;
        private System.Windows.Forms.Label StudentStatusLabel;
        private System.Windows.Forms.ComboBox SchoolYRCombobox;
        private System.Windows.Forms.Label SYLabel;
        private System.Windows.Forms.ComboBox SexCombobox;
        private System.Windows.Forms.Label SexLabel;
        private System.Windows.Forms.Label AcademicStatusLabel;
        private System.Windows.Forms.Label BdayLabel;
        private System.Windows.Forms.DateTimePicker Bdaypicket;
        private Button Editbtn;
    }
}