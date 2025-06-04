using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ClassroomManagementSystem
{
    partial class CurriculumManager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            ProgramLabel = new Label();
            CourseCombobox = new ComboBox();
            allsubjectdatagrid = new DataGridView();
            PictureBox1 = new PictureBox();
            SubjectSearchTextBox = new TextBox();
            SYCombobox = new ComboBox();
            Label1 = new Label();
            SemesterCombobox = new ComboBox();
            Label2 = new Label();
            Label3 = new Label();
            Label4 = new Label();
            SubjectCodeTextbox = new TextBox();
            SubjectNameTextbox = new TextBox();
            Label5 = new Label();
            LectureUnitsTextbox = new TextBox();
            Label6 = new Label();
            LabUnitsTextbox = new TextBox();
            Label7 = new Label();
            YearLevelCombobox = new ComboBox();
            Label8 = new Label();
            CurriculumYearCombobox = new ComboBox();
            Label10 = new Label();
            SubjectStatusCombobox = new ComboBox();
            Label11 = new Label();
            ProfessorLabel = new Label();
            ProfessorDepartmentCombobox = new ComboBox();
            ProfDeptLabel = new Label();
            ClearButton = new Button();
            SaveButton = new Button();
            GridFilterCheckedListBox = new CheckedListBox();
            CurrentSemesterCombobox = new ComboBox();
            Label9 = new Label();
            SelectCurrentSYCombobox = new ComboBox();
            Label12 = new Label();
            addSYtextbox = new TextBox();
            Label14 = new Label();
            SaveButtonforSY = new Button();
            AssignedprofessorCombobox = new ComboBox();
            ((ISupportInitialize)(allsubjectdatagrid)).BeginInit();
            ((ISupportInitialize)(PictureBox1)).BeginInit();
            SuspendLayout();
            // 
            // ProgramLabel
            // 
            ProgramLabel.AutoSize = true;
            ProgramLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ProgramLabel.Location = new Point(246, 37);
            ProgramLabel.Name = "ProgramLabel";
            ProgramLabel.Size = new Size(76, 21);
            ProgramLabel.TabIndex = 47;
            ProgramLabel.Text = "Program";
            // 
            // CourseCombobox
            // 
            CourseCombobox.Font = new Font("Segoe UI", 11F);
            CourseCombobox.FormattingEnabled = true;
            CourseCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            CourseCombobox.Location = new Point(246, 61);
            CourseCombobox.Name = "CourseCombobox";
            CourseCombobox.Size = new Size(159, 28);
            CourseCombobox.TabIndex = 48;
            CourseCombobox.Text = "Select course";
            // 
            // allsubjectdatagrid
            // 
            allsubjectdatagrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            allsubjectdatagrid.Location = new Point(42, 301);
            allsubjectdatagrid.Name = "allsubjectdatagrid";
            allsubjectdatagrid.Size = new Size(812, 333);
            allsubjectdatagrid.TabIndex = 49;
            // 
            // PictureBox1
            // 
            PictureBox1.Location = new Point(44, 248);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(32, 36);
            PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            PictureBox1.TabIndex = 51;
            PictureBox1.TabStop = false;
            // 
            // SubjectSearchTextBox
            // 
            SubjectSearchTextBox.Font = new Font("Segoe UI", 10F);
            SubjectSearchTextBox.Location = new Point(81, 248);
            SubjectSearchTextBox.Margin = new Padding(2, 1, 2, 1);
            SubjectSearchTextBox.Multiline = true;
            SubjectSearchTextBox.Name = "SubjectSearchTextBox";
            SubjectSearchTextBox.Size = new Size(203, 36);
            SubjectSearchTextBox.TabIndex = 50;
            // 
            // SYCombobox
            // 
            SYCombobox.Font = new Font("Segoe UI", 11F);
            SYCombobox.FormattingEnabled = true;
            SYCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            SYCombobox.Location = new Point(45, 61);
            SYCombobox.Name = "SYCombobox";
            SYCombobox.Size = new Size(159, 28);
            SYCombobox.TabIndex = 53;
            SYCombobox.Text = "Select course";
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label1.Location = new Point(45, 37);
            Label1.Name = "Label1";
            Label1.Size = new Size(99, 21);
            Label1.TabIndex = 52;
            Label1.Text = "School Year";
            // 
            // SemesterCombobox
            // 
            SemesterCombobox.Font = new Font("Segoe UI", 11F);
            SemesterCombobox.FormattingEnabled = true;
            SemesterCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            SemesterCombobox.Location = new Point(439, 61);
            SemesterCombobox.Name = "SemesterCombobox";
            SemesterCombobox.Size = new Size(159, 28);
            SemesterCombobox.TabIndex = 55;
            SemesterCombobox.Text = "Select course";
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label2.Location = new Point(439, 37);
            Label2.Name = "Label2";
            Label2.Size = new Size(80, 21);
            Label2.TabIndex = 54;
            Label2.Text = "Semester";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Font = new Font("Microsoft YaHei", 15F, FontStyle.Bold);
            Label3.Location = new Point(44, 220);
            Label3.Name = "Label3";
            Label3.Size = new Size(161, 27);
            Label3.TabIndex = 59;
            Label3.Text = "SUBJECTS LIST";
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label4.Location = new Point(45, 101);
            Label4.Name = "Label4";
            Label4.Size = new Size(110, 21);
            Label4.TabIndex = 60;
            Label4.Text = "Subject Code";
            // 
            // SubjectCodeTextbox
            // 
            SubjectCodeTextbox.Font = new Font("Segoe UI", 11F);
            SubjectCodeTextbox.Location = new Point(45, 125);
            SubjectCodeTextbox.Name = "SubjectCodeTextbox";
            SubjectCodeTextbox.Size = new Size(159, 27);
            SubjectCodeTextbox.TabIndex = 61;
            // 
            // SubjectNameTextbox
            // 
            SubjectNameTextbox.Font = new Font("Segoe UI", 11F);
            SubjectNameTextbox.Location = new Point(246, 125);
            SubjectNameTextbox.Name = "SubjectNameTextbox";
            SubjectNameTextbox.Size = new Size(352, 27);
            SubjectNameTextbox.TabIndex = 63;
            // 
            // Label5
            // 
            Label5.AutoSize = true;
            Label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label5.Location = new Point(246, 101);
            Label5.Name = "Label5";
            Label5.Size = new Size(117, 21);
            Label5.TabIndex = 62;
            Label5.Text = "Subject Name";
            // 
            // LectureUnitsTextbox
            // 
            LectureUnitsTextbox.Font = new Font("Segoe UI", 11F);
            LectureUnitsTextbox.Location = new Point(631, 64);
            LectureUnitsTextbox.Name = "LectureUnitsTextbox";
            LectureUnitsTextbox.Size = new Size(119, 27);
            LectureUnitsTextbox.TabIndex = 65;
            // 
            // Label6
            // 
            Label6.AutoSize = true;
            Label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label6.Location = new Point(631, 40);
            Label6.Name = "Label6";
            Label6.Size = new Size(110, 21);
            Label6.TabIndex = 64;
            Label6.Text = "Lecture Units";
            // 
            // LabUnitsTextbox
            // 
            LabUnitsTextbox.Font = new Font("Segoe UI", 11F);
            LabUnitsTextbox.Location = new Point(631, 126);
            LabUnitsTextbox.Name = "LabUnitsTextbox";
            LabUnitsTextbox.Size = new Size(119, 27);
            LabUnitsTextbox.TabIndex = 67;
            // 
            // Label7
            // 
            Label7.AutoSize = true;
            Label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label7.Location = new Point(631, 102);
            Label7.Name = "Label7";
            Label7.Size = new Size(81, 21);
            Label7.TabIndex = 66;
            Label7.Text = "Lab Units";
            // 
            // YearLevelCombobox
            // 
            YearLevelCombobox.Font = new Font("Segoe UI", 11F);
            YearLevelCombobox.FormattingEnabled = true;
            YearLevelCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            YearLevelCombobox.Location = new Point(796, 64);
            YearLevelCombobox.Name = "YearLevelCombobox";
            YearLevelCombobox.Size = new Size(159, 28);
            YearLevelCombobox.TabIndex = 69;
            YearLevelCombobox.Text = "Select course";
            // 
            // Label8
            // 
            Label8.AutoSize = true;
            Label8.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label8.Location = new Point(796, 40);
            Label8.Name = "Label8";
            Label8.Size = new Size(87, 21);
            Label8.TabIndex = 68;
            Label8.Text = "Year Level";
            // 
            // CurriculumYearCombobox
            // 
            CurriculumYearCombobox.Font = new Font("Segoe UI", 11F);
            CurriculumYearCombobox.FormattingEnabled = true;
            CurriculumYearCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            CurriculumYearCombobox.Location = new Point(796, 125);
            CurriculumYearCombobox.Name = "CurriculumYearCombobox";
            CurriculumYearCombobox.Size = new Size(159, 28);
            CurriculumYearCombobox.TabIndex = 75;
            CurriculumYearCombobox.Text = "Select course";
            // 
            // Label10
            // 
            Label10.AutoSize = true;
            Label10.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label10.Location = new Point(796, 101);
            Label10.Name = "Label10";
            Label10.Size = new Size(132, 21);
            Label10.TabIndex = 74;
            Label10.Text = "Curriculum year";
            // 
            // SubjectStatusCombobox
            // 
            SubjectStatusCombobox.Font = new Font("Segoe UI", 11F);
            SubjectStatusCombobox.FormattingEnabled = true;
            SubjectStatusCombobox.Items.AddRange(new object[] {
            "BS Computer Science",
            "BS Information Technology",
            "BS Information System",
            "BS Entertainment and Multimedia Computing"});
            SubjectStatusCombobox.Location = new Point(975, 64);
            SubjectStatusCombobox.Name = "SubjectStatusCombobox";
            SubjectStatusCombobox.Size = new Size(159, 28);
            SubjectStatusCombobox.TabIndex = 73;
            SubjectStatusCombobox.Text = "Select course";
            // 
            // Label11
            // 
            Label11.AutoSize = true;
            Label11.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label11.Location = new Point(975, 40);
            Label11.Name = "Label11";
            Label11.Size = new Size(118, 21);
            Label11.TabIndex = 72;
            Label11.Text = "Subject Status";
            // 
            // ProfessorLabel
            // 
            ProfessorLabel.AutoSize = true;
            ProfessorLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ProfessorLabel.Location = new Point(44, 164);
            ProfessorLabel.Name = "ProfessorLabel";
            ProfessorLabel.Size = new Size(154, 21);
            ProfessorLabel.TabIndex = 76;
            ProfessorLabel.Text = "Assigned professor";
            // 
            // ProfessorDepartmentCombobox
            // 
            ProfessorDepartmentCombobox.Font = new Font("Segoe UI", 11F);
            ProfessorDepartmentCombobox.FormattingEnabled = true;
            ProfessorDepartmentCombobox.Items.AddRange(new object[] { "CSD" });
            ProfessorDepartmentCombobox.Location = new Point(439, 188);
            ProfessorDepartmentCombobox.Name = "ProfessorDepartmentCombobox";
            ProfessorDepartmentCombobox.Size = new Size(177, 28);
            ProfessorDepartmentCombobox.TabIndex = 79;
            ProfessorDepartmentCombobox.Text = "Select department";
            // 
            // ProfDeptLabel
            // 
            ProfDeptLabel.AutoSize = true;
            ProfDeptLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            ProfDeptLabel.Location = new Point(439, 164);
            ProfDeptLabel.Name = "ProfDeptLabel";
            ProfDeptLabel.Size = new Size(177, 21);
            ProfDeptLabel.TabIndex = 78;
            ProfDeptLabel.Text = "Professor Department";
            // 
            // ClearButton
            // 
            ClearButton.BackColor = Color.FromArgb(192, 0, 0);
            ClearButton.ForeColor = SystemColors.ButtonHighlight;
            ClearButton.Location = new Point(1047, 126);
            ClearButton.Margin = new Padding(2, 1, 2, 1);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(130, 42);
            ClearButton.TabIndex = 81;
            ClearButton.Text = "CLEAR";
            ClearButton.UseVisualStyleBackColor = false;
            // 
            // SaveButton
            // 
            SaveButton.BackColor = Color.FromArgb(72, 229, 189);
            SaveButton.Location = new Point(1047, 170);
            SaveButton.Margin = new Padding(2, 1, 2, 1);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(130, 42);
            SaveButton.TabIndex = 80;
            SaveButton.Text = "SAVE";
            SaveButton.UseVisualStyleBackColor = false;
            // 
            // GridFilterCheckedListBox
            // 
            GridFilterCheckedListBox.BackColor = Color.FromArgb(72, 229, 189);
            GridFilterCheckedListBox.FormattingEnabled = true;
            GridFilterCheckedListBox.Location = new Point(860, 301);
            GridFilterCheckedListBox.Name = "GridFilterCheckedListBox";
            GridFilterCheckedListBox.Size = new Size(131, 328);
            GridFilterCheckedListBox.TabIndex = 82;
            // 
            // CurrentSemesterCombobox
            // 
            CurrentSemesterCombobox.Font = new Font("Segoe UI", 11F);
            CurrentSemesterCombobox.FormattingEnabled = true;
            CurrentSemesterCombobox.Items.AddRange(new object[] {
            "First Semester",
            "Second Semester",
            "Summer"});
            CurrentSemesterCombobox.Location = new Point(1018, 437);
            CurrentSemesterCombobox.Name = "CurrentSemesterCombobox";
            CurrentSemesterCombobox.Size = new Size(159, 28);
            CurrentSemesterCombobox.TabIndex = 86;
            CurrentSemesterCombobox.Text = "Select Semester";
            // 
            // Label9
            // 
            Label9.AutoSize = true;
            Label9.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label9.Location = new Point(1018, 413);
            Label9.Name = "Label9";
            Label9.Size = new Size(141, 21);
            Label9.TabIndex = 85;
            Label9.Text = "Current Semester";
            // 
            // SelectCurrentSYCombobox
            // 
            SelectCurrentSYCombobox.Font = new Font("Segoe UI", 11F);
            SelectCurrentSYCombobox.FormattingEnabled = true;
            SelectCurrentSYCombobox.Items.AddRange(new object[] { "2024-2025" });
            SelectCurrentSYCombobox.Location = new Point(1018, 376);
            SelectCurrentSYCombobox.Name = "SelectCurrentSYCombobox";
            SelectCurrentSYCombobox.Size = new Size(159, 28);
            SelectCurrentSYCombobox.TabIndex = 84;
            SelectCurrentSYCombobox.Text = "Select SY";
            // 
            // Label12
            // 
            Label12.AutoSize = true;
            Label12.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label12.Location = new Point(1018, 352);
            Label12.Name = "Label12";
            Label12.Size = new Size(90, 21);
            Label12.TabIndex = 83;
            Label12.Text = "Current SY";
            // 
            // addSYtextbox
            // 
            addSYtextbox.Font = new Font("Segoe UI", 11F);
            addSYtextbox.Location = new Point(1018, 322);
            addSYtextbox.Name = "addSYtextbox";
            addSYtextbox.Size = new Size(159, 27);
            addSYtextbox.TabIndex = 88;
            // 
            // Label14
            // 
            Label14.AutoSize = true;
            Label14.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            Label14.Location = new Point(1018, 298);
            Label14.Name = "Label14";
            Label14.Size = new Size(64, 21);
            Label14.TabIndex = 87;
            Label14.Text = "Add SY";
            // 
            // SaveButtonforSY
            // 
            SaveButtonforSY.Location = new Point(1018, 487);
            SaveButtonforSY.Margin = new Padding(2, 1, 2, 1);
            SaveButtonforSY.Name = "SaveButtonforSY";
            SaveButtonforSY.Size = new Size(159, 42);
            SaveButtonforSY.TabIndex = 89;
            SaveButtonforSY.Text = "SAVE";
            SaveButtonforSY.UseVisualStyleBackColor = true;
            // 
            // AssignedprofessorCombobox
            // 
            AssignedprofessorCombobox.Font = new Font("Segoe UI", 11F);
            AssignedprofessorCombobox.FormattingEnabled = true;
            AssignedprofessorCombobox.Location = new Point(45, 188);
            AssignedprofessorCombobox.Name = "AssignedprofessorCombobox";
            AssignedprofessorCombobox.Size = new Size(388, 28);
            AssignedprofessorCombobox.TabIndex = 90;
            // 
            // CurriculumManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 157, 0);
            Controls.Add(AssignedprofessorCombobox);
            Controls.Add(SaveButtonforSY);
            Controls.Add(addSYtextbox);
            Controls.Add(Label14);
            Controls.Add(CurrentSemesterCombobox);
            Controls.Add(Label9);
            Controls.Add(SelectCurrentSYCombobox);
            Controls.Add(Label12);
            Controls.Add(GridFilterCheckedListBox);
            Controls.Add(ClearButton);
            Controls.Add(SaveButton);
            Controls.Add(ProfessorDepartmentCombobox);
            Controls.Add(ProfDeptLabel);
            Controls.Add(ProfessorLabel);
            Controls.Add(CurriculumYearCombobox);
            Controls.Add(Label10);
            Controls.Add(SubjectStatusCombobox);
            Controls.Add(Label11);
            Controls.Add(YearLevelCombobox);
            Controls.Add(Label8);
            Controls.Add(LabUnitsTextbox);
            Controls.Add(Label7);
            Controls.Add(LectureUnitsTextbox);
            Controls.Add(Label6);
            Controls.Add(SubjectNameTextbox);
            Controls.Add(Label5);
            Controls.Add(SubjectCodeTextbox);
            Controls.Add(Label4);
            Controls.Add(Label3);
            Controls.Add(SemesterCombobox);
            Controls.Add(Label2);
            Controls.Add(SYCombobox);
            Controls.Add(Label1);
            Controls.Add(PictureBox1);
            Controls.Add(SubjectSearchTextBox);
            Controls.Add(allsubjectdatagrid);
            Controls.Add(CourseCombobox);
            Controls.Add(ProgramLabel);
            Name = "CurriculumManager";
            Size = new Size(1229, 704);
            Load += CurriculumManager_Load;
            ((ISupportInitialize)(allsubjectdatagrid)).EndInit();
            ((ISupportInitialize)(PictureBox1)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ProgramLabel;
        private ComboBox CourseCombobox;
        private DataGridView allsubjectdatagrid;
        private PictureBox PictureBox1;
        private TextBox SubjectSearchTextBox;
        private ComboBox SYCombobox;
        private Label Label1;
        private Label Label2;
        private Label Label3;
        private Label Label4;
        private TextBox SubjectCodeTextbox;
        private TextBox SubjectNameTextbox;
        private Label Label5;
        private TextBox LectureUnitsTextbox;
        private Label Label6;
        private TextBox LabUnitsTextbox;
        private Label Label7;
        private ComboBox YearLevelCombobox;
        private Label Label8;
        private ComboBox SemesterCombobox;
        private ComboBox CurriculumYearCombobox;
        private Label Label10;
        private ComboBox SubjectStatusCombobox;
        private Label Label11;
        private Label ProfessorLabel;
        private ComboBox ProfessorDepartmentCombobox;
        private Label ProfDeptLabel;
        private Button ClearButton;
        private Button SaveButton;
        private CheckedListBox GridFilterCheckedListBox;
        private ComboBox CurrentSemesterCombobox;
        private Label Label9;
        private ComboBox SelectCurrentSYCombobox;
        private Label Label12;
        private TextBox addSYtextbox;
        private Label Label14;
        private Button SaveButtonforSY;
        private ComboBox AssignedprofessorCombobox;
    }
}