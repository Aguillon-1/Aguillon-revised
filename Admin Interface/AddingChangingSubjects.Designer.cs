namespace ClassroomManagementSystem
{
    partial class AddingChangingSubjects
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
            StudentListDataGrid = new DataGridView();
            FullNameLabel = new Label();
            FullNameDisplayTextbox = new TextBox();
            StudentNumTextbox = new TextBox();
            Label1 = new Label();
            StudentStatusCombobox = new ComboBox();
            StudentStatusLabel = new Label();
            Label2 = new Label();
            AvailableSubjectsDataGrid = new DataGridView();
            Label4 = new Label();
            PictureBox1 = new PictureBox();
            AvailableSubjectsTextbox = new TextBox();
            EnrolledSubjectsListDataGrid = new DataGridView();
            Label3 = new Label();
            ClearButton = new Button();
            SaveButton = new Button();
            Button1 = new Button();
            PictureBox2 = new PictureBox();
            StudentListSearchTextbox = new TextBox();
            PictureBox3 = new PictureBox();
            EnrolledSubjectsListTextbox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)StudentListDataGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)AvailableSubjectsDataGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)EnrolledSubjectsListDataGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox3).BeginInit();
            SuspendLayout();
            // 
            // StudentListDataGrid
            // 
            StudentListDataGrid.AllowUserToAddRows = false;
            StudentListDataGrid.AllowUserToDeleteRows = false;
            StudentListDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            StudentListDataGrid.Location = new Point(14, 68);
            StudentListDataGrid.Name = "StudentListDataGrid";
            StudentListDataGrid.ReadOnly = true;
            StudentListDataGrid.Size = new Size(479, 612);
            StudentListDataGrid.TabIndex = 0;
            // 
            // FullNameLabel
            // 
            FullNameLabel.AutoSize = true;
            FullNameLabel.Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold);
            FullNameLabel.Location = new Point(450, 12);
            FullNameLabel.Name = "FullNameLabel";
            FullNameLabel.Size = new Size(51, 19);
            FullNameLabel.TabIndex = 2;
            FullNameLabel.Text = "Name";
            // 
            // FullNameDisplayTextbox
            // 
            FullNameDisplayTextbox.BorderStyle = BorderStyle.FixedSingle;
            FullNameDisplayTextbox.Cursor = Cursors.No;
            FullNameDisplayTextbox.Font = new Font("Segoe UI", 12F);
            FullNameDisplayTextbox.Location = new Point(450, 34);
            FullNameDisplayTextbox.Name = "FullNameDisplayTextbox";
            FullNameDisplayTextbox.Size = new Size(244, 29);
            FullNameDisplayTextbox.TabIndex = 3;
            // 
            // StudentNumTextbox
            // 
            StudentNumTextbox.BorderStyle = BorderStyle.FixedSingle;
            StudentNumTextbox.Cursor = Cursors.No;
            StudentNumTextbox.Font = new Font("Segoe UI", 12F);
            StudentNumTextbox.Location = new Point(723, 34);
            StudentNumTextbox.Name = "StudentNumTextbox";
            StudentNumTextbox.Size = new Size(152, 29);
            StudentNumTextbox.TabIndex = 5;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold);
            Label1.Location = new Point(723, 12);
            Label1.Name = "Label1";
            Label1.Size = new Size(93, 19);
            Label1.TabIndex = 4;
            Label1.Text = "Student No.";
            // 
            // StudentStatusCombobox
            // 
            StudentStatusCombobox.Font = new Font("Segoe UI", 11F);
            StudentStatusCombobox.FormattingEnabled = true;
            StudentStatusCombobox.Items.AddRange(new object[] { "Regular", "Irregular" });
            StudentStatusCombobox.Location = new Point(907, 35);
            StudentStatusCombobox.Name = "StudentStatusCombobox";
            StudentStatusCombobox.Size = new Size(158, 28);
            StudentStatusCombobox.TabIndex = 48;
            StudentStatusCombobox.Text = "Regular";
            // 
            // StudentStatusLabel
            // 
            StudentStatusLabel.AutoSize = true;
            StudentStatusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            StudentStatusLabel.Location = new Point(907, 10);
            StudentStatusLabel.Name = "StudentStatusLabel";
            StudentStatusLabel.Size = new Size(121, 21);
            StudentStatusLabel.TabIndex = 47;
            StudentStatusLabel.Text = "Student Status";
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("Microsoft YaHei", 11F, FontStyle.Bold);
            Label2.Location = new Point(507, 394);
            Label2.Name = "Label2";
            Label2.Size = new Size(129, 19);
            Label2.TabIndex = 49;
            Label2.Text = "Enrolled subjets";
            // 
            // AvailableSubjectsDataGrid
            // 
            AvailableSubjectsDataGrid.AllowUserToAddRows = false;
            AvailableSubjectsDataGrid.AllowUserToDeleteRows = false;
            AvailableSubjectsDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            AvailableSubjectsDataGrid.Location = new Point(509, 144);
            AvailableSubjectsDataGrid.Name = "AvailableSubjectsDataGrid";
            AvailableSubjectsDataGrid.ReadOnly = true;
            AvailableSubjectsDataGrid.Size = new Size(548, 231);
            AvailableSubjectsDataGrid.TabIndex = 53;
            AvailableSubjectsDataGrid.CellContentClick += AvailableSubjectsDataGrid_CellContentClick_1;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label4.Location = new Point(507, 82);
            Label4.Name = "Label4";
            Label4.Size = new Size(120, 21);
            Label4.TabIndex = 54;
            Label4.Text = "Search subject";
            // 
            // PictureBox1
            // 
            PictureBox1.Location = new Point(507, 106);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(32, 34);
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox1.TabIndex = 56;
            PictureBox1.TabStop = false;
            // 
            // AvailableSubjectsTextbox
            // 
            AvailableSubjectsTextbox.Font = new Font("Segoe UI", 10F);
            AvailableSubjectsTextbox.Location = new Point(544, 106);
            AvailableSubjectsTextbox.Margin = new Padding(2, 1, 2, 1);
            AvailableSubjectsTextbox.Multiline = true;
            AvailableSubjectsTextbox.Name = "AvailableSubjectsTextbox";
            AvailableSubjectsTextbox.Size = new Size(252, 34);
            AvailableSubjectsTextbox.TabIndex = 55;
            // 
            // EnrolledSubjectsListDataGrid
            // 
            EnrolledSubjectsListDataGrid.AllowUserToAddRows = false;
            EnrolledSubjectsListDataGrid.AllowUserToDeleteRows = false;
            EnrolledSubjectsListDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            EnrolledSubjectsListDataGrid.Location = new Point(509, 453);
            EnrolledSubjectsListDataGrid.Name = "EnrolledSubjectsListDataGrid";
            EnrolledSubjectsListDataGrid.ReadOnly = true;
            EnrolledSubjectsListDataGrid.Size = new Size(548, 227);
            EnrolledSubjectsListDataGrid.TabIndex = 57;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Font = new Font("Microsoft YaHei", 12F, FontStyle.Bold);
            Label3.Location = new Point(14, 41);
            Label3.Name = "Label3";
            Label3.Size = new Size(104, 22);
            Label3.TabIndex = 58;
            Label3.Text = "Student list";
            // 
            // ClearButton
            // 
            ClearButton.BackColor = Color.FromArgb(192, 0, 0);
            ClearButton.ForeColor = SystemColors.ButtonHighlight;
            ClearButton.Location = new Point(1085, 211);
            ClearButton.Margin = new Padding(2, 1, 2, 1);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(130, 42);
            ClearButton.TabIndex = 60;
            ClearButton.Text = "CLEAR";
            ClearButton.UseVisualStyleBackColor = false;
            // 
            // SaveButton
            // 
            SaveButton.BackColor = Color.FromArgb(72, 229, 189);
            SaveButton.Location = new Point(1085, 255);
            SaveButton.Margin = new Padding(2, 1, 2, 1);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(130, 42);
            SaveButton.TabIndex = 59;
            SaveButton.Text = "SAVE";
            SaveButton.UseVisualStyleBackColor = false;
            // 
            // Button1
            // 
            Button1.BackColor = Color.FromArgb(34, 40, 23);
            Button1.ForeColor = SystemColors.ButtonHighlight;
            Button1.Location = new Point(1085, 166);
            Button1.Margin = new Padding(2, 1, 2, 1);
            Button1.Name = "Button1";
            Button1.Size = new Size(130, 42);
            Button1.TabIndex = 61;
            Button1.Text = "LOAD DEFAULT";
            Button1.UseVisualStyleBackColor = false;
            // 
            // PictureBox2
            // 
            PictureBox2.Location = new Point(127, 30);
            PictureBox2.Name = "PictureBox2";
            PictureBox2.Size = new Size(32, 34);
            PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox2.TabIndex = 63;
            PictureBox2.TabStop = false;
            // 
            // StudentListSearchTextbox
            // 
            StudentListSearchTextbox.Font = new Font("Segoe UI", 10F);
            StudentListSearchTextbox.Location = new Point(164, 30);
            StudentListSearchTextbox.Margin = new Padding(2, 1, 2, 1);
            StudentListSearchTextbox.Multiline = true;
            StudentListSearchTextbox.Name = "StudentListSearchTextbox";
            StudentListSearchTextbox.Size = new Size(252, 34);
            StudentListSearchTextbox.TabIndex = 62;
            // 
            // PictureBox3
            // 
            PictureBox3.Location = new Point(507, 416);
            PictureBox3.Name = "PictureBox3";
            PictureBox3.Size = new Size(32, 34);
            PictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox3.TabIndex = 65;
            PictureBox3.TabStop = false;
            // 
            // EnrolledSubjectsListTextbox
            // 
            EnrolledSubjectsListTextbox.Font = new Font("Segoe UI", 10F);
            EnrolledSubjectsListTextbox.Location = new Point(544, 415);
            EnrolledSubjectsListTextbox.Margin = new Padding(2, 1, 2, 1);
            EnrolledSubjectsListTextbox.Multiline = true;
            EnrolledSubjectsListTextbox.Name = "EnrolledSubjectsListTextbox";
            EnrolledSubjectsListTextbox.Size = new Size(232, 34);
            EnrolledSubjectsListTextbox.TabIndex = 64;
            // 
            // AddingChangingSubjects
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 157, 0);
            Controls.Add(PictureBox3);
            Controls.Add(EnrolledSubjectsListTextbox);
            Controls.Add(PictureBox2);
            Controls.Add(StudentListSearchTextbox);
            Controls.Add(Button1);
            Controls.Add(ClearButton);
            Controls.Add(SaveButton);
            Controls.Add(Label3);
            Controls.Add(EnrolledSubjectsListDataGrid);
            Controls.Add(PictureBox1);
            Controls.Add(AvailableSubjectsTextbox);
            Controls.Add(Label4);
            Controls.Add(AvailableSubjectsDataGrid);
            Controls.Add(Label2);
            Controls.Add(StudentStatusCombobox);
            Controls.Add(StudentStatusLabel);
            Controls.Add(StudentNumTextbox);
            Controls.Add(Label1);
            Controls.Add(FullNameDisplayTextbox);
            Controls.Add(FullNameLabel);
            Controls.Add(StudentListDataGrid);
            Name = "AddingChangingSubjects";
            Size = new Size(1229, 704);
            Load += AddingChangingSubjects_Load;
            ((System.ComponentModel.ISupportInitialize)StudentListDataGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)AvailableSubjectsDataGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)EnrolledSubjectsListDataGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView StudentListDataGrid;
        private System.Windows.Forms.Label FullNameLabel;
        private System.Windows.Forms.TextBox FullNameDisplayTextbox;
        private System.Windows.Forms.TextBox StudentNumTextbox;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ComboBox StudentStatusCombobox;
        private System.Windows.Forms.Label StudentStatusLabel;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.DataGridView AvailableSubjectsDataGrid;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.TextBox AvailableSubjectsTextbox;
        private System.Windows.Forms.DataGridView EnrolledSubjectsListDataGrid;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.PictureBox PictureBox2;
        private System.Windows.Forms.TextBox StudentListSearchTextbox;
        private System.Windows.Forms.PictureBox PictureBox3;
        private System.Windows.Forms.TextBox EnrolledSubjectsListTextbox;
    }
}