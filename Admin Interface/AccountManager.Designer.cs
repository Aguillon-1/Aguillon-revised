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
            this.SaveButton = new System.Windows.Forms.Button();
            this.UserDataGrid = new System.Windows.Forms.DataGridView();
            this.dbSearchTextBox = new System.Windows.Forms.TextBox();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.FnameTextBox = new System.Windows.Forms.TextBox();
            this.FnameLabel = new System.Windows.Forms.Label();
            this.MnameLabel = new System.Windows.Forms.Label();
            this.MnameTextBox = new System.Windows.Forms.TextBox();
            this.Lname = new System.Windows.Forms.Label();
            this.LnameTextBox = new System.Windows.Forms.TextBox();
            this.RoleLabel = new System.Windows.Forms.Label();
            this.RoleComboBox = new System.Windows.Forms.ComboBox();
            this.DbsortCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.StudentNoTextBox = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.EmailLabel = new System.Windows.Forms.Label();
            this.CourseCombobox = new System.Windows.Forms.ComboBox();
            this.CourseLabel = new System.Windows.Forms.Label();
            this.SectionCombobox = new System.Windows.Forms.ComboBox();
            this.SectionLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.YearLabel = new System.Windows.Forms.Label();
            this.EmailTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.YearCombobox = new System.Windows.Forms.ComboBox();
            this.PassEyeButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ContactNoTextBox = new System.Windows.Forms.TextBox();
            this.ContactNoLabel = new System.Windows.Forms.Label();
            this.StudentStatusCombobox = new System.Windows.Forms.ComboBox();
            this.StudentStatusLabel = new System.Windows.Forms.Label();
            this.SchoolYRCombobox = new System.Windows.Forms.ComboBox();
            this.SYLabel = new System.Windows.Forms.Label();
            this.SexCombobox = new System.Windows.Forms.ComboBox();
            this.SexLabel = new System.Windows.Forms.Label();
            this.ComboBox4 = new System.Windows.Forms.ComboBox();
            this.AcademicStatusLabel = new System.Windows.Forms.Label();
            this.BdayLabel = new System.Windows.Forms.Label();
            this.Bdaypicket = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.UserDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(229)))), ((int)(((byte)(189)))));
            this.SaveButton.Location = new System.Drawing.Point(1086, 651);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(130, 42);
            this.SaveButton.TabIndex = 0;
            this.SaveButton.Text = "SAVE";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // UserDataGrid
            // 
            this.UserDataGrid.AllowUserToAddRows = false;
            this.UserDataGrid.AllowUserToDeleteRows = false;
            this.UserDataGrid.AllowUserToResizeColumns = false;
            this.UserDataGrid.AllowUserToResizeRows = false;
            this.UserDataGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.UserDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserDataGrid.Location = new System.Drawing.Point(22, 14);
            this.UserDataGrid.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.UserDataGrid.Name = "UserDataGrid";
            this.UserDataGrid.ReadOnly = true;
            this.UserDataGrid.RowHeadersWidth = 82;
            this.UserDataGrid.Size = new System.Drawing.Size(1069, 272);
            this.UserDataGrid.TabIndex = 1;
            this.UserDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.UserDataGrid_CellClick);
            // 
            // dbSearchTextBox
            // 
            this.dbSearchTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dbSearchTextBox.Location = new System.Drawing.Point(60, 290);
            this.dbSearchTextBox.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.dbSearchTextBox.MaximumSize = new System.Drawing.Size(300, 34);
            this.dbSearchTextBox.MinimumSize = new System.Drawing.Size(273, 34);
            this.dbSearchTextBox.Multiline = true;
            this.dbSearchTextBox.Name = "dbSearchTextBox";
            this.dbSearchTextBox.Size = new System.Drawing.Size(300, 34);
            this.dbSearchTextBox.TabIndex = 2;
            this.dbSearchTextBox.Text = "Search Name, ID and ect...";
            // 
            // PictureBox1
            // 
            this.PictureBox1.Location = new System.Drawing.Point(22, 290);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(32, 34);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox1.TabIndex = 3;
            this.PictureBox1.TabStop = false;
            // 
            // FnameTextBox
            // 
            this.FnameTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FnameTextBox.Location = new System.Drawing.Point(22, 358);
            this.FnameTextBox.Name = "FnameTextBox";
            this.FnameTextBox.Size = new System.Drawing.Size(158, 27);
            this.FnameTextBox.TabIndex = 4;
            // 
            // FnameLabel
            // 
            this.FnameLabel.AutoSize = true;
            this.FnameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FnameLabel.Location = new System.Drawing.Point(21, 340);
            this.FnameLabel.Name = "FnameLabel";
            this.FnameLabel.Size = new System.Drawing.Size(67, 15);
            this.FnameLabel.TabIndex = 6;
            this.FnameLabel.Text = "First Name";
            // 
            // MnameLabel
            // 
            this.MnameLabel.AutoSize = true;
            this.MnameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.MnameLabel.Location = new System.Drawing.Point(206, 340);
            this.MnameLabel.Name = "MnameLabel";
            this.MnameLabel.Size = new System.Drawing.Size(81, 15);
            this.MnameLabel.TabIndex = 8;
            this.MnameLabel.Text = "Middle Name";
            // 
            // MnameTextBox
            // 
            this.MnameTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.MnameTextBox.Location = new System.Drawing.Point(206, 358);
            this.MnameTextBox.Name = "MnameTextBox";
            this.MnameTextBox.Size = new System.Drawing.Size(158, 27);
            this.MnameTextBox.TabIndex = 7;
            // 
            // Lname
            // 
            this.Lname.AutoSize = true;
            this.Lname.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Lname.Location = new System.Drawing.Point(389, 340);
            this.Lname.Name = "Lname";
            this.Lname.Size = new System.Drawing.Size(65, 15);
            this.Lname.TabIndex = 10;
            this.Lname.Text = "Last Name";
            // 
            // LnameTextBox
            // 
            this.LnameTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.LnameTextBox.Location = new System.Drawing.Point(389, 358);
            this.LnameTextBox.Name = "LnameTextBox";
            this.LnameTextBox.Size = new System.Drawing.Size(158, 27);
            this.LnameTextBox.TabIndex = 9;
            // 
            // RoleLabel
            // 
            this.RoleLabel.AutoSize = true;
            this.RoleLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.RoleLabel.Location = new System.Drawing.Point(21, 397);
            this.RoleLabel.Name = "RoleLabel";
            this.RoleLabel.Size = new System.Drawing.Size(35, 15);
            this.RoleLabel.TabIndex = 11;
            this.RoleLabel.Text = "Role:";
            // 
            // RoleComboBox
            // 
            this.RoleComboBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.RoleComboBox.FormattingEnabled = true;
            this.RoleComboBox.Items.AddRange(new object[] {
            "Student",
            "Faculty",
            "Admin-MIS",
            "Registrar"});
            this.RoleComboBox.Location = new System.Drawing.Point(22, 415);
            this.RoleComboBox.Name = "RoleComboBox";
            this.RoleComboBox.Size = new System.Drawing.Size(158, 28);
            this.RoleComboBox.TabIndex = 15;
            // 
            // DbsortCheckedListBox
            // 
            this.DbsortCheckedListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(229)))), ((int)(((byte)(189)))));
            this.DbsortCheckedListBox.FormattingEnabled = true;
            this.DbsortCheckedListBox.Location = new System.Drawing.Point(1096, 14);
            this.DbsortCheckedListBox.Name = "DbsortCheckedListBox";
            this.DbsortCheckedListBox.Size = new System.Drawing.Size(120, 274);
            this.DbsortCheckedListBox.TabIndex = 17;
            // 
            // StudentNoTextBox
            // 
            this.StudentNoTextBox.AutoSize = true;
            this.StudentNoTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.StudentNoTextBox.Location = new System.Drawing.Point(574, 340);
            this.StudentNoTextBox.Name = "StudentNoTextBox";
            this.StudentNoTextBox.Size = new System.Drawing.Size(74, 15);
            this.StudentNoTextBox.TabIndex = 19;
            this.StudentNoTextBox.Text = "Student No.";
            // 
            // TextBox1
            // 
            this.TextBox1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.TextBox1.Location = new System.Drawing.Point(574, 358);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(158, 27);
            this.TextBox1.TabIndex = 18;
            // 
            // EmailLabel
            // 
            this.EmailLabel.AutoSize = true;
            this.EmailLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.EmailLabel.Location = new System.Drawing.Point(21, 464);
            this.EmailLabel.Name = "EmailLabel";
            this.EmailLabel.Size = new System.Drawing.Size(39, 15);
            this.EmailLabel.TabIndex = 20;
            this.EmailLabel.Text = "Email:";
            // 
            // CourseCombobox
            // 
            this.CourseCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.CourseCombobox.FormattingEnabled = true;
            this.CourseCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            this.CourseCombobox.Location = new System.Drawing.Point(388, 415);
            this.CourseCombobox.Name = "CourseCombobox";
            this.CourseCombobox.Size = new System.Drawing.Size(159, 28);
            this.CourseCombobox.TabIndex = 23;
            this.CourseCombobox.Text = "Select course";
            // 
            // CourseLabel
            // 
            this.CourseLabel.AutoSize = true;
            this.CourseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.CourseLabel.Location = new System.Drawing.Point(387, 397);
            this.CourseLabel.Name = "CourseLabel";
            this.CourseLabel.Size = new System.Drawing.Size(45, 15);
            this.CourseLabel.TabIndex = 22;
            this.CourseLabel.Text = "Course";
            // 
            // SectionCombobox
            // 
            this.SectionCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.SectionCombobox.FormattingEnabled = true;
            this.SectionCombobox.Items.AddRange(new object[] {
            "A",
            "B",
            "C"});
            this.SectionCombobox.Location = new System.Drawing.Point(574, 415);
            this.SectionCombobox.Name = "SectionCombobox";
            this.SectionCombobox.Size = new System.Drawing.Size(158, 28);
            this.SectionCombobox.TabIndex = 25;
            this.SectionCombobox.Text = "select section";
            // 
            // SectionLabel
            // 
            this.SectionLabel.AutoSize = true;
            this.SectionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.SectionLabel.Location = new System.Drawing.Point(573, 397);
            this.SectionLabel.Name = "SectionLabel";
            this.SectionLabel.Size = new System.Drawing.Size(49, 15);
            this.SectionLabel.TabIndex = 24;
            this.SectionLabel.Text = "Section";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.PasswordLabel.Location = new System.Drawing.Point(22, 520);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(59, 15);
            this.PasswordLabel.TabIndex = 12;
            this.PasswordLabel.Text = "Password";
            // 
            // YearLabel
            // 
            this.YearLabel.AutoSize = true;
            this.YearLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.YearLabel.Location = new System.Drawing.Point(206, 397);
            this.YearLabel.Name = "YearLabel";
            this.YearLabel.Size = new System.Drawing.Size(31, 15);
            this.YearLabel.TabIndex = 26;
            this.YearLabel.Text = "Year";
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.EmailTextBox.Location = new System.Drawing.Point(22, 482);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(158, 27);
            this.EmailTextBox.TabIndex = 28;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.PasswordTextBox.Location = new System.Drawing.Point(22, 538);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(177, 27);
            this.PasswordTextBox.TabIndex = 29;
            // 
            // YearCombobox
            // 
            this.YearCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.YearCombobox.FormattingEnabled = true;
            this.YearCombobox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.YearCombobox.Location = new System.Drawing.Point(206, 415);
            this.YearCombobox.Name = "YearCombobox";
            this.YearCombobox.Size = new System.Drawing.Size(158, 28);
            this.YearCombobox.TabIndex = 30;
            this.YearCombobox.Text = "Select year";
            // 
            // PassEyeButton
            // 
            this.PassEyeButton.Location = new System.Drawing.Point(205, 537);
            this.PassEyeButton.Name = "PassEyeButton";
            this.PassEyeButton.Size = new System.Drawing.Size(34, 28);
            this.PassEyeButton.TabIndex = 31;
            this.PassEyeButton.Text = "H";
            this.PassEyeButton.UseVisualStyleBackColor = true;
            this.PassEyeButton.Click += new System.EventHandler(this.PassEyeButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(40)))), ((int)(((byte)(23)))));
            this.ClearButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClearButton.Location = new System.Drawing.Point(1086, 607);
            this.ClearButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(130, 42);
            this.ClearButton.TabIndex = 33;
            this.ClearButton.Text = "CLEAR";
            this.ClearButton.UseVisualStyleBackColor = false;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.DeleteButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.DeleteButton.Location = new System.Drawing.Point(1086, 563);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(130, 42);
            this.DeleteButton.TabIndex = 34;
            this.DeleteButton.Text = "DELETE";
            this.DeleteButton.UseVisualStyleBackColor = false;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ContactNoTextBox
            // 
            this.ContactNoTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ContactNoTextBox.Location = new System.Drawing.Point(200, 482);
            this.ContactNoTextBox.Name = "ContactNoTextBox";
            this.ContactNoTextBox.Size = new System.Drawing.Size(158, 27);
            this.ContactNoTextBox.TabIndex = 44;
            // 
            // ContactNoLabel
            // 
            this.ContactNoLabel.AutoSize = true;
            this.ContactNoLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.ContactNoLabel.Location = new System.Drawing.Point(199, 464);
            this.ContactNoLabel.Name = "ContactNoLabel";
            this.ContactNoLabel.Size = new System.Drawing.Size(69, 15);
            this.ContactNoLabel.TabIndex = 43;
            this.ContactNoLabel.Text = "Contact No";
            // 
            // StudentStatusCombobox
            // 
            this.StudentStatusCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.StudentStatusCombobox.FormattingEnabled = true;
            this.StudentStatusCombobox.Items.AddRange(new object[] {
            "Regular",
            "Irregular"});
            this.StudentStatusCombobox.Location = new System.Drawing.Point(758, 358);
            this.StudentStatusCombobox.Name = "StudentStatusCombobox";
            this.StudentStatusCombobox.Size = new System.Drawing.Size(158, 28);
            this.StudentStatusCombobox.TabIndex = 46;
            this.StudentStatusCombobox.Text = "Regular";
            // 
            // StudentStatusLabel
            // 
            this.StudentStatusLabel.AutoSize = true;
            this.StudentStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.StudentStatusLabel.Location = new System.Drawing.Point(757, 340);
            this.StudentStatusLabel.Name = "StudentStatusLabel";
            this.StudentStatusLabel.Size = new System.Drawing.Size(90, 15);
            this.StudentStatusLabel.TabIndex = 45;
            this.StudentStatusLabel.Text = "Student Status";
            // 
            // SchoolYRCombobox
            // 
            this.SchoolYRCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.SchoolYRCombobox.FormattingEnabled = true;
            this.SchoolYRCombobox.Items.AddRange(new object[] {
            "24-25",
            "25-26"});
            this.SchoolYRCombobox.Location = new System.Drawing.Point(758, 415);
            this.SchoolYRCombobox.Name = "SchoolYRCombobox";
            this.SchoolYRCombobox.Size = new System.Drawing.Size(158, 28);
            this.SchoolYRCombobox.TabIndex = 48;
            this.SchoolYRCombobox.Text = "Select SY";
            // 
            // SYLabel
            // 
            this.SYLabel.AutoSize = true;
            this.SYLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.SYLabel.Location = new System.Drawing.Point(757, 397);
            this.SYLabel.Name = "SYLabel";
            this.SYLabel.Size = new System.Drawing.Size(71, 15);
            this.SYLabel.TabIndex = 47;
            this.SYLabel.Text = "School Year";
            // 
            // SexCombobox
            // 
            this.SexCombobox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.SexCombobox.FormattingEnabled = true;
            this.SexCombobox.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.SexCombobox.Location = new System.Drawing.Point(386, 481);
            this.SexCombobox.Name = "SexCombobox";
            this.SexCombobox.Size = new System.Drawing.Size(159, 28);
            this.SexCombobox.TabIndex = 50;
            this.SexCombobox.Text = "Select sex";
            // 
            // SexLabel
            // 
            this.SexLabel.AutoSize = true;
            this.SexLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.SexLabel.Location = new System.Drawing.Point(386, 463);
            this.SexLabel.Name = "SexLabel";
            this.SexLabel.Size = new System.Drawing.Size(28, 15);
            this.SexLabel.TabIndex = 49;
            this.SexLabel.Text = "Sex";
            // 
            // ComboBox4
            // 
            this.ComboBox4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.ComboBox4.FormattingEnabled = true;
            this.ComboBox4.Items.AddRange(new object[] {
            "Active",
            "Graduated",
            "Returnee",
            "Transferee",
            "Deactivated"});
            this.ComboBox4.Location = new System.Drawing.Point(757, 478);
            this.ComboBox4.Name = "ComboBox4";
            this.ComboBox4.Size = new System.Drawing.Size(159, 28);
            this.ComboBox4.TabIndex = 52;
            this.ComboBox4.Text = "Select Academic status";
            // 
            // AcademicStatusLabel
            // 
            this.AcademicStatusLabel.AutoSize = true;
            this.AcademicStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.AcademicStatusLabel.Location = new System.Drawing.Point(757, 460);
            this.AcademicStatusLabel.Name = "AcademicStatusLabel";
            this.AcademicStatusLabel.Size = new System.Drawing.Size(99, 15);
            this.AcademicStatusLabel.TabIndex = 51;
            this.AcademicStatusLabel.Text = "Academic Status";
            // 
            // BdayLabel
            // 
            this.BdayLabel.AutoSize = true;
            this.BdayLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.BdayLabel.Location = new System.Drawing.Point(573, 465);
            this.BdayLabel.Name = "BdayLabel";
            this.BdayLabel.Size = new System.Drawing.Size(54, 15);
            this.BdayLabel.TabIndex = 53;
            this.BdayLabel.Text = "Birthday";
            // 
            // Bdaypicket
            // 
            this.Bdaypicket.CalendarFont = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bdaypicket.Location = new System.Drawing.Point(573, 483);
            this.Bdaypicket.Name = "Bdaypicket";
            this.Bdaypicket.Size = new System.Drawing.Size(159, 23);
            this.Bdaypicket.TabIndex = 54;
            // 
            // AccountManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(157)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.Bdaypicket);
            this.Controls.Add(this.BdayLabel);
            this.Controls.Add(this.ComboBox4);
            this.Controls.Add(this.AcademicStatusLabel);
            this.Controls.Add(this.SexCombobox);
            this.Controls.Add(this.SexLabel);
            this.Controls.Add(this.SchoolYRCombobox);
            this.Controls.Add(this.SYLabel);
            this.Controls.Add(this.StudentStatusCombobox);
            this.Controls.Add(this.StudentStatusLabel);
            this.Controls.Add(this.ContactNoTextBox);
            this.Controls.Add(this.ContactNoLabel);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.PassEyeButton);
            this.Controls.Add(this.YearCombobox);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.YearLabel);
            this.Controls.Add(this.SectionCombobox);
            this.Controls.Add(this.SectionLabel);
            this.Controls.Add(this.CourseCombobox);
            this.Controls.Add(this.CourseLabel);
            this.Controls.Add(this.EmailLabel);
            this.Controls.Add(this.StudentNoTextBox);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.DbsortCheckedListBox);
            this.Controls.Add(this.RoleComboBox);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.RoleLabel);
            this.Controls.Add(this.Lname);
            this.Controls.Add(this.LnameTextBox);
            this.Controls.Add(this.MnameLabel);
            this.Controls.Add(this.MnameTextBox);
            this.Controls.Add(this.FnameLabel);
            this.Controls.Add(this.FnameTextBox);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.dbSearchTextBox);
            this.Controls.Add(this.UserDataGrid);
            this.Controls.Add(this.SaveButton);
            this.Name = "AccountManager";
            this.Size = new System.Drawing.Size(1229, 704);
            this.Load += new System.EventHandler(this.AccountManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.UserDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}