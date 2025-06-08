using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;


namespace ClassroomManagementSystem
{
    public partial class CurriculumManager : UserControl
    {
        // Database connection string
        private readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Earl\\source\\repos\\CMS_Revised\\Database1.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // Class level variables
        private DataTable subjectsTable;
        private int currentCurriculumId = -1;
        private bool isEditMode = false;
        private List<string> filteredColumns = new List<string>();

        public CurriculumManager()
        {
            InitializeComponent();
        }

        private void CurriculumManager_Load(object sender, EventArgs e)
        {
            // Initialize UI components
            InitializeComboBoxes();
            ConfigureDataGrid();
            InitializeGridFilterCheckList();

            // Load system configuration settings
            LoadSystemConfigurationSettings();

            // Load data
            LoadAllSubjects();

            // Configure search box
            SubjectSearchTextBox.Text = "Search subject code or name...";

            // Add event handlers
            AddEventHandlers();
        }

        private void InitializeComboBoxes()
        {
            // Load School Years
            LoadSchoolYears();

            // Load Programs
            LoadPrograms();

            // Load Semesters
            LoadSemesters();

            // Load Year Levels
            LoadYearLevels();

            // Load Professor list into the combobox
            LoadProfessorsIntoComboBox();

            // Load Curriculum Years (based on school years)
            LoadCurriculumYears();

            // Load Subject Status options
            SubjectStatusCombobox.Items.Clear();
            SubjectStatusCombobox.Items.AddRange(new object[] { "active", "inactive", "deprecated" });
            SubjectStatusCombobox.SelectedIndex = 0;

            // Make all comboboxes dropdown only (not editable)
            SYCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            CourseCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SemesterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            YearLevelCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            CurriculumYearCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SubjectStatusCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            ProfessorDepartmentCombobox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Set the current year/semester comboboxes to dropdown only
            SelectCurrentSYCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            CurrentSemesterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Load Professor Departments
            LoadDepartments();
        }

        private void LoadSchoolYears()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT school_year_id, year_name FROM school_years ORDER BY is_current DESC, start_date DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        SYCombobox.DataSource = dt;
                        SYCombobox.DisplayMember = "year_name";
                        SYCombobox.ValueMember = "school_year_id";

                        if (dt.Rows.Count > 0)
                        {
                            SYCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading school years: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPrograms()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT program_id, program_name, program_code FROM programs WHERE is_active = 1 ORDER BY program_name";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        CourseCombobox.DataSource = dt;
                        CourseCombobox.DisplayMember = "program_name";
                        CourseCombobox.ValueMember = "program_id";

                        if (dt.Rows.Count > 0)
                        {
                            CourseCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading programs: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSemesters()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT semester_id, semester_name FROM semesters ORDER BY semester_id";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        SemesterCombobox.DataSource = dt;
                        SemesterCombobox.DisplayMember = "semester_name";
                        SemesterCombobox.ValueMember = "semester_id";

                        if (dt.Rows.Count > 0)
                        {
                            SemesterCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading semesters: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadYearLevels()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT year_level_id, year_name FROM year_levels WHERE is_active = 1 ORDER BY year_level_id";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        YearLevelCombobox.DataSource = dt;
                        YearLevelCombobox.DisplayMember = "year_name";
                        YearLevelCombobox.ValueMember = "year_level_id";

                        if (dt.Rows.Count > 0)
                        {
                            YearLevelCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading year levels: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCurriculumYears()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get distinct curriculum years from database if they exist, otherwise use school years
                    string query = @"
                        SELECT DISTINCT curriculum_year 
                        FROM curriculum 
                        UNION 
                        SELECT year_name FROM school_years 
                        ORDER BY curriculum_year DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        CurriculumYearCombobox.DataSource = dt;
                        CurriculumYearCombobox.DisplayMember = "curriculum_year";
                        CurriculumYearCombobox.ValueMember = "curriculum_year";

                        if (dt.Rows.Count > 0)
                        {
                            CurriculumYearCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If there's an error (possibly because curriculum table doesn't exist yet),
                // just populate with some default years
                CurriculumYearCombobox.Items.Clear();
                CurriculumYearCombobox.Items.AddRange(new object[] { "2024-2025", "2023-2024", "2022-2023" });
                CurriculumYearCombobox.SelectedIndex = 0;
            }
        }

        private void LoadDepartments()
        {
            try
            {
                // Create a new DataTable to hold the departments
                var departmentsTable = new DataTable();
                departmentsTable.Columns.Add("department", typeof(string));

                // Always add CSD as the first option
                departmentsTable.Rows.Add("CSD");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if faculty_profiles table exists
                    string checkTableQuery = "SELECT OBJECT_ID('faculty_profiles', 'U') AS TableExists";
                    var checkCmd = new SqlCommand(checkTableQuery, connection);
                    var tableExists = checkCmd.ExecuteScalar();

                    if (tableExists != null && !Convert.IsDBNull(tableExists))
                    {
                        // Table exists, get departments from database
                        string query = "SELECT DISTINCT department FROM faculty_profiles WHERE department <> 'CSD' ORDER BY department";

                        using (var cmd = new SqlCommand(query, connection))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string dept = reader["department"].ToString();
                                // Only add department if it's not empty and not already in the table (CSD)
                                if (!string.IsNullOrWhiteSpace(dept) && dept.ToUpper() != "CSD")
                                {
                                    departmentsTable.Rows.Add(dept);
                                }
                            }
                        }
                    }
                }

                // If no additional departments were found, add some defaults
                if (departmentsTable.Rows.Count == 1)  // Only CSD exists
                {
                    departmentsTable.Rows.Add("Computer Science");
                    departmentsTable.Rows.Add("Information Technology");
                    departmentsTable.Rows.Add("Mathematics");
                }

                // Set the DataSource for the ComboBox
                ProfessorDepartmentCombobox.DataSource = departmentsTable;
                ProfessorDepartmentCombobox.DisplayMember = "department";
                ProfessorDepartmentCombobox.ValueMember = "department";

                // Set "CSD" as the default selection
                ProfessorDepartmentCombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Fallback if any error occurs
                ProfessorDepartmentCombobox.Items.Clear();
                ProfessorDepartmentCombobox.Items.AddRange(new object[] { "CSD", "Computer Science", "Information Technology", "Mathematics" });
                ProfessorDepartmentCombobox.SelectedIndex = 0;
            }
        }

        private void LoadProfessorsIntoComboBox()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            u.user_id,
                            CASE 
                                WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                                    u.first_name + ' ' + u.last_name
                                ELSE 
                                    u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
                            END AS full_name
                        FROM users u
                        WHERE u.user_type = 'Faculty' AND u.is_archived = 0
                        ORDER BY u.last_name, u.first_name";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);

                        // Add empty row for "no professor assigned"
                        var emptyRow = dt.NewRow();
                        emptyRow["user_id"] = DBNull.Value;
                        emptyRow["full_name"] = "-- No Professor Assigned --";
                        dt.Rows.InsertAt(emptyRow, 0);

                        AssignedprofessorCombobox.DataSource = dt;
                        AssignedprofessorCombobox.DisplayMember = "full_name";
                        AssignedprofessorCombobox.ValueMember = "user_id";

                        if (dt.Rows.Count > 0)
                        {
                            AssignedprofessorCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading professors: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGrid()
        {
            // Configure the DataGridView properties
            allsubjectdatagrid.AutoGenerateColumns = false;
            allsubjectdatagrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allsubjectdatagrid.MultiSelect = false;
            allsubjectdatagrid.AllowUserToAddRows = false;
            allsubjectdatagrid.ReadOnly = true;
            allsubjectdatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            allsubjectdatagrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Configure header style
            allsubjectdatagrid.EnableHeadersVisualStyles = false;
            allsubjectdatagrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            allsubjectdatagrid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            allsubjectdatagrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            allsubjectdatagrid.ColumnHeadersHeight = 35;

            // Clear existing columns
            allsubjectdatagrid.Columns.Clear();

            // Add columns with proper configuration
            // Edit button column
            var editButtonColumn = new DataGridViewButtonColumn
            {
                Name = "edit_button",
                HeaderText = "",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FillWeight = 40
            };
            allsubjectdatagrid.Columns.Add(editButtonColumn);

            // Delete button column
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                Name = "delete_button",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FillWeight = 40
            };
            allsubjectdatagrid.Columns.Add(deleteButtonColumn);

            // Hidden curriculum_id column
            var curriculumIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "curriculum_id",
                HeaderText = "ID",
                DataPropertyName = "curriculum_id",
                Visible = false
            };
            allsubjectdatagrid.Columns.Add(curriculumIdColumn);

            // Subject Code column
            var subjectCodeColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_code",
                HeaderText = "Subject Code",
                DataPropertyName = "subject_code",
                FillWeight = 80
            };
            allsubjectdatagrid.Columns.Add(subjectCodeColumn);

            // Subject Name column
            var subjectNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_name",
                HeaderText = "Subject Name",
                DataPropertyName = "subject_name",
                FillWeight = 150
            };
            allsubjectdatagrid.Columns.Add(subjectNameColumn);

            // Course column (Year + Program Code)
            var courseColumn = new DataGridViewTextBoxColumn
            {
                Name = "course",
                HeaderText = "Course",
                DataPropertyName = "course",
                FillWeight = 80
            };
            allsubjectdatagrid.Columns.Add(courseColumn);

            // Lecture Units column
            var lectureUnitsColumn = new DataGridViewTextBoxColumn
            {
                Name = "lecture_units",
                HeaderText = "Lecture Units",
                DataPropertyName = "lecture_units",
                FillWeight = 60
            };
            allsubjectdatagrid.Columns.Add(lectureUnitsColumn);

            // Lab Units column
            var labUnitsColumn = new DataGridViewTextBoxColumn
            {
                Name = "lab_units",
                HeaderText = "Lab Units",
                DataPropertyName = "lab_units",
                FillWeight = 60
            };
            allsubjectdatagrid.Columns.Add(labUnitsColumn);

            // Curriculum Year column
            var curriculumYearColumn = new DataGridViewTextBoxColumn
            {
                Name = "curriculum_year",
                HeaderText = "Curriculum Year",
                DataPropertyName = "curriculum_year",
                FillWeight = 80
            };
            allsubjectdatagrid.Columns.Add(curriculumYearColumn);

            // Subject Status column
            var subjectStatusColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_status",
                HeaderText = "Status",
                DataPropertyName = "subject_status",
                FillWeight = 60
            };
            allsubjectdatagrid.Columns.Add(subjectStatusColumn);

            // Assigned Professor column
            var professorColumn = new DataGridViewTextBoxColumn
            {
                Name = "professor",
                HeaderText = "Assigned Professor",
                DataPropertyName = "professor",
                FillWeight = 120
            };
            allsubjectdatagrid.Columns.Add(professorColumn);

            // Professor Department column
            var departmentColumn = new DataGridViewTextBoxColumn
            {
                Name = "professor_department",
                HeaderText = "Department",
                DataPropertyName = "professor_department",
                FillWeight = 100
            };
            allsubjectdatagrid.Columns.Add(departmentColumn);

            // Hidden columns for database IDs
            AddHiddenColumn("subject_id", "Subject ID");
            AddHiddenColumn("year_level_id", "Year Level ID");
            AddHiddenColumn("program_id", "Program ID");
            AddHiddenColumn("school_year_id", "School Year ID");
            AddHiddenColumn("semester_id", "Semester ID");
            AddHiddenColumn("faculty_id", "Faculty ID");
        }

        private void AddHiddenColumn(string name, string headerText)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Visible = false
            };
            allsubjectdatagrid.Columns.Add(column);
        }

        private void InitializeGridFilterCheckList()
        {
            // Add items to GridFilterCheckedListBox based on visible columns in the DataGridView
            GridFilterCheckedListBox.Items.Clear();

            // Add only visible columns to the filter checklist
            foreach (DataGridViewColumn column in allsubjectdatagrid.Columns)
            {
                if (column.Visible && !(column is DataGridViewButtonColumn))
                {
                    GridFilterCheckedListBox.Items.Add(column.HeaderText, true);
                    filteredColumns.Add(column.Name);
                }
            }

            // Set CheckOnClick to true for better UX
            GridFilterCheckedListBox.CheckOnClick = true;
        }

        private void AddEventHandlers()
        {
            // Add search textbox event handlers
            SubjectSearchTextBox.TextChanged += SubjectSearchTextBox_TextChanged;
            SubjectSearchTextBox.GotFocus += SubjectSearchTextBox_GotFocus;
            SubjectSearchTextBox.LostFocus += SubjectSearchTextBox_LostFocus;

            // Add button click event handlers
            SaveButton.Click += SaveButton_Click;
            ClearButton.Click += ClearButton_Click;
            SaveButtonforSY.Click += SaveButtonforSY_Click;

            // Add DataGridView event handler
            allsubjectdatagrid.CellContentClick += AllSubjectDataGrid_CellContentClick;

            // Add ComboBox event handlers
            CourseCombobox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            SYCombobox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            SemesterCombobox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;

            // Add CheckedListBox event handler
            GridFilterCheckedListBox.ItemCheck += GridFilterCheckedListBox_ItemCheck;
        }

        private void LoadSystemConfigurationSettings()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // First load all school years
                    string schoolYearsQuery = "SELECT school_year_id, year_name FROM school_years ORDER BY year_name DESC";
                    using (var schoolYearsAdapter = new SqlDataAdapter(schoolYearsQuery, connection))
                    {
                        var schoolYearsTable = new DataTable();
                        schoolYearsAdapter.Fill(schoolYearsTable);

                        if (schoolYearsTable.Rows.Count == 0)
                        {
                            // No school years exist, create at least one
                            string createSchoolYearQuery = @"
                            INSERT INTO school_years (year_name, is_current, start_date, end_date) 
                            VALUES ('2024-2025', 1, '2024-06-01', '2025-04-30')";

                            using (var createCmd = new SqlCommand(createSchoolYearQuery, connection))
                            {
                                createCmd.ExecuteNonQuery();
                            }

                            // Reload the table
                            schoolYearsTable = new DataTable();
                            schoolYearsAdapter.Fill(schoolYearsTable);
                        }

                        // Set data source for current SY combobox
                        SelectCurrentSYCombobox.DataSource = schoolYearsTable.Copy();
                        SelectCurrentSYCombobox.DisplayMember = "year_name";
                        SelectCurrentSYCombobox.ValueMember = "school_year_id";
                        SelectCurrentSYCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
                    }

                    // Then load all semesters for the current semester selection
                    string semestersQuery = "SELECT semester_id, semester_name FROM semesters ORDER BY semester_id";
                    using (var semestersAdapter = new SqlDataAdapter(semestersQuery, connection))
                    {
                        var semestersTable = new DataTable();
                        semestersAdapter.Fill(semestersTable);

                        if (semestersTable.Rows.Count == 0)
                        {
                            // No semesters exist, create defaults
                            string createSemestersQuery = @"
                            INSERT INTO semesters (semester_name, semester_code) VALUES 
                            ('First Semester', '1ST'), 
                            ('Second Semester', '2ND'), 
                            ('Summer', 'SUM')";

                            using (var createCmd = new SqlCommand(createSemestersQuery, connection))
                            {
                                createCmd.ExecuteNonQuery();
                            }

                            // Reload the table
                            semestersTable = new DataTable();
                            semestersAdapter.Fill(semestersTable);
                        }

                        // Set data source for current semester combobox
                        CurrentSemesterCombobox.DataSource = semestersTable;
                        CurrentSemesterCombobox.DisplayMember = "semester_name";
                        CurrentSemesterCombobox.ValueMember = "semester_id";
                        CurrentSemesterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
                    }

                    // Create system_configuration table if it doesn't exist
                    string checkConfigTableQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'system_configuration')
                    BEGIN
                        CREATE TABLE system_configuration (
                            config_id INT IDENTITY(1,1) PRIMARY KEY,
                            current_school_year_id INT NOT NULL,
                            current_semester_id INT NOT NULL,
                            registration_open BIT DEFAULT 1,
                            enrollment_open BIT DEFAULT 1,
                            auto_enroll_enabled BIT DEFAULT 1,
                            auto_year_level_promotion BIT DEFAULT 0,
                            last_updated DATETIME DEFAULT GETDATE(),
                            updated_by INT NULL,
                            CONSTRAINT FK_sysconfig_school_year FOREIGN KEY (current_school_year_id) 
                                REFERENCES school_years (school_year_id),
                            CONSTRAINT FK_sysconfig_semester FOREIGN KEY (current_semester_id) 
                                REFERENCES semesters (semester_id)
                        );
                        
                        DECLARE @defaultSchoolYearId INT;
                        DECLARE @defaultSemesterId INT;
                        
                        SELECT TOP 1 @defaultSchoolYearId = school_year_id 
                        FROM school_years 
                        WHERE is_current = 1
                        ORDER BY school_year_id DESC;
                        
                        IF @defaultSchoolYearId IS NULL
                            SELECT TOP 1 @defaultSchoolYearId = school_year_id 
                            FROM school_years 
                            ORDER BY school_year_id DESC;
                        
                        SELECT TOP 1 @defaultSemesterId = semester_id 
                        FROM semesters 
                        ORDER BY semester_id;
                        
                        INSERT INTO system_configuration 
                        (current_school_year_id, current_semester_id) 
                        VALUES (@defaultSchoolYearId, @defaultSemesterId);
                    END";

                    using (var checkCmd = new SqlCommand(checkConfigTableQuery, connection))
                    {
                        checkCmd.ExecuteNonQuery();
                    }

                    // Now get current settings from system_configuration
                    string configQuery = @"
                    SELECT 
                        sc.current_school_year_id, 
                        sc.current_semester_id,
                        sy.year_name,
                        sem.semester_name
                    FROM system_configuration sc
                    JOIN school_years sy ON sc.current_school_year_id = sy.school_year_id
                    JOIN semesters sem ON sc.current_semester_id = sem.semester_id";

                    using (var configCmd = new SqlCommand(configQuery, connection))
                    using (var reader = configCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Select the current values in ComboBoxes
                            SelectComboBoxItemByValue(SelectCurrentSYCombobox, "school_year_id", reader["current_school_year_id"]);
                            SelectComboBoxItemByValue(CurrentSemesterCombobox, "semester_id", reader["current_semester_id"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading system configuration: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectComboBoxItemByValue(ComboBox comboBox, string valueMember, object value)
        {
            if (comboBox.DataSource is DataTable dt)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][valueMember].Equals(value))
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // Add this method to add a new school year
        private void AddNewSchoolYear(string yearName)
        {
            if (string.IsNullOrWhiteSpace(yearName))
            {
                MessageBox.Show("Please enter a valid school year (e.g. 24-25).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the school year already exists
                            string checkQuery = "SELECT COUNT(*) FROM school_years WHERE year_name = @yearName";
                            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@yearName", yearName);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    MessageBox.Show($"School year {yearName} already exists.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    transaction.Rollback();
                                    return;
                                }
                            }

                            // Insert the new school year
                            // Parse the year to set appropriate start and end dates
                            DateTime startDate;
                            DateTime endDate;
                            string[] yearParts = yearName.Split('-');

                            if (yearParts.Length == 2 && yearParts[0].Length <= 2 && yearParts[1].Length <= 2)
                            {
                                // Format like "23-24"
                                int startYear = 2000 + int.Parse(yearParts[0]);
                                int endYear = 2000 + int.Parse(yearParts[1]);
                                startDate = new DateTime(startYear, 6, 1); // Assuming school year starts in June
                                endDate = new DateTime(endYear, 3, 31); // Assuming school year ends in March
                            }
                            else
                            {
                                // Default to current year if format is not as expected
                                startDate = new DateTime(DateTime.Now.Year, 6, 1);
                                endDate = new DateTime(DateTime.Now.Year + 1, 3, 31);
                            }

                            string insertQuery = @"
                            INSERT INTO school_years (year_name, is_current, start_date, end_date)
                            VALUES (@yearName, 0, @startDate, @endDate);
                            SELECT SCOPE_IDENTITY();";

                            using (var insertCmd = new SqlCommand(insertQuery, connection, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@yearName", yearName);
                                insertCmd.Parameters.AddWithValue("@startDate", startDate);
                                insertCmd.Parameters.AddWithValue("@endDate", endDate);

                                int newId = Convert.ToInt32(insertCmd.ExecuteScalar());
                                transaction.Commit();

                                MessageBox.Show($"School year {yearName} added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Reload the school years
                                LoadSystemConfigurationSettings();
                                LoadSchoolYears(); // Reload original school year combobox
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error adding school year: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add this method to save the current school year and semester settings
        private void SaveCurrentSystemSettings()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if values are selected
                    if (SelectCurrentSYCombobox.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a school year.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (CurrentSemesterCombobox.SelectedValue == null)
                    {
                        MessageBox.Show("Please select a semester.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Get the selected values
                    int schoolYearId = Convert.ToInt32(SelectCurrentSYCombobox.SelectedValue);
                    int semesterId = Convert.ToInt32(CurrentSemesterCombobox.SelectedValue);

                    // First verify that the selected school year exists
                    string verifyQuery = "SELECT COUNT(*) FROM school_years WHERE school_year_id = @schoolYearId";
                    using (var verifyCmd = new SqlCommand(verifyQuery, connection))
                    {
                        verifyCmd.Parameters.AddWithValue("@schoolYearId", schoolYearId);
                        int count = Convert.ToInt32(verifyCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            MessageBox.Show("The selected school year no longer exists in the database. Please refresh and try again.",
                                        "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            // Reload comboboxes
                            LoadSystemConfigurationSettings();
                            return;
                        }
                    }

                    // Verify that the selected semester exists
                    verifyQuery = "SELECT COUNT(*) FROM semesters WHERE semester_id = @semesterId";
                    using (var verifyCmd = new SqlCommand(verifyQuery, connection))
                    {
                        verifyCmd.Parameters.AddWithValue("@semesterId", semesterId);
                        int count = Convert.ToInt32(verifyCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            MessageBox.Show("The selected semester no longer exists in the database. Please refresh and try again.",
                                        "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            // Reload comboboxes
                            LoadSystemConfigurationSettings();
                            return;
                        }
                    }

                    // Check if system_configuration table exists and has records
                    string checkConfigQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'system_configuration')
                    BEGIN
                        CREATE TABLE system_configuration (
                            config_id INT IDENTITY(1,1) PRIMARY KEY,
                            current_school_year_id INT NOT NULL,
                            current_semester_id INT NOT NULL,
                            registration_open BIT DEFAULT 1,
                            enrollment_open BIT DEFAULT 1,
                            auto_enroll_enabled BIT DEFAULT 1,
                            auto_year_level_promotion BIT DEFAULT 0,
                            last_updated DATETIME DEFAULT GETDATE(),
                            updated_by INT NULL,
                            CONSTRAINT FK_sysconfig_school_year FOREIGN KEY (current_school_year_id) 
                                REFERENCES school_years (school_year_id),
                            CONSTRAINT FK_sysconfig_semester FOREIGN KEY (current_semester_id) 
                                REFERENCES semesters (semester_id)
                        );
                    END

                    IF NOT EXISTS (SELECT 1 FROM system_configuration)
                    BEGIN
                        INSERT INTO system_configuration (current_school_year_id, current_semester_id) 
                        VALUES (@schoolYearId, @semesterId);
                    END";

                    using (var checkCmd = new SqlCommand(checkConfigQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@schoolYearId", schoolYearId);
                        checkCmd.Parameters.AddWithValue("@semesterId", semesterId);
                        checkCmd.ExecuteNonQuery();
                    }

                    // Now use direct SQL instead of the stored procedure to avoid any conflicts
                    string updateQuery = @"
                    UPDATE system_configuration 
                    SET current_school_year_id = @schoolYearId,
                        current_semester_id = @semesterId,
                        last_updated = GETDATE()
                    WHERE config_id = (SELECT TOP 1 config_id FROM system_configuration);
                    
                    -- Update school year is_current flag
                    UPDATE school_years SET is_current = 0;
                    UPDATE school_years SET is_current = 1 WHERE school_year_id = @schoolYearId;";

                    using (var updateCmd = new SqlCommand(updateQuery, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@schoolYearId", schoolYearId);
                        updateCmd.Parameters.AddWithValue("@semesterId", semesterId);
                        updateCmd.ExecuteNonQuery();

                        // Get current school year and semester names
                        string namesQuery = @"
                        SELECT 
                            sy.year_name AS current_school_year,
                            sem.semester_name AS current_semester
                        FROM system_configuration sc
                        JOIN school_years sy ON sc.current_school_year_id = sy.school_year_id
                        JOIN semesters sem ON sc.current_semester_id = sem.semester_id";

                        using (var namesCmd = new SqlCommand(namesQuery, connection))
                        using (var reader = namesCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string schoolYear = reader["current_school_year"].ToString();
                                string semester = reader["current_semester"].ToString();

                                MessageBox.Show($"System settings updated successfully.{Environment.NewLine}{Environment.NewLine}Current School Year: {schoolYear}{Environment.NewLine}Current Semester: {semester}",
                                           "Settings Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating system settings: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add this event handler for the SaveButtonforSY button
        private void SaveButtonforSY_Click(object sender, EventArgs e)
        {
            // If addSYtextbox has text, add a new school year
            if (!string.IsNullOrWhiteSpace(addSYtextbox.Text))
            {
                AddNewSchoolYear(addSYtextbox.Text.Trim());
                addSYtextbox.Clear();
            }

            // Save the current system settings
            SaveCurrentSystemSettings();
        }

        private void LoadAllSubjects()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Updated query to concatenate year_level_id and program_code as course
                    string query = @"
                    SELECT 
                        c.curriculum_id, 
                        s.subject_id,
                        s.subject_code, 
                        s.subject_name, 
                        CONCAT(yl.year_level_id, '-', p.program_code) AS course, -- Concatenate year_level_id and program_code
                        s.lecture_units, 
                        s.lab_units,
                        c.curriculum_year,
                        c.subject_status,
                        CASE 
                            WHEN u.middle_name IS NULL OR u.middle_name = '' THEN 
                                u.first_name + ' ' + u.last_name
                            ELSE 
                                u.first_name + ' ' + LEFT(u.middle_name, 1) + '. ' + u.last_name
                        END AS professor,
                        ISNULL(fp.department, '') AS professor_department,
                        yl.year_level_id,
                        c.program_id,
                        c.school_year_id,
                        c.semester_id,
                        c.faculty_id
                    FROM curriculum c
                    JOIN subjects s ON c.subject_id = s.subject_id
                    JOIN programs p ON c.program_id = p.program_id
                    JOIN school_years sy ON c.school_year_id = sy.school_year_id
                    JOIN semesters sem ON c.semester_id = sem.semester_id
                    JOIN year_levels yl ON c.year_level_id = yl.year_level_id
                    LEFT JOIN users u ON c.faculty_id = u.user_id
                    LEFT JOIN (
                        SELECT user_id, department 
                        FROM faculty_profiles
                    ) fp ON u.user_id = fp.user_id
                    WHERE c.is_active = 1";

                    // Add filtering based on selected ComboBox values
                    if (CourseCombobox.SelectedValue != null)
                    {
                        query += " AND c.program_id = @programId";
                    }

                    if (SYCombobox.SelectedValue != null)
                    {
                        query += " AND c.school_year_id = @schoolYearId";
                    }

                    if (SemesterCombobox.SelectedValue != null)
                    {
                        query += " AND c.semester_id = @semesterId";
                    }

                    // Complete the query with ordering
                    query += " ORDER BY s.subject_code";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        // Add parameters if needed
                        if (CourseCombobox.SelectedValue != null)
                        {
                            cmd.Parameters.AddWithValue("@programId", CourseCombobox.SelectedValue);
                        }

                        if (SYCombobox.SelectedValue != null)
                        {
                            cmd.Parameters.AddWithValue("@schoolYearId", SYCombobox.SelectedValue);
                        }

                        if (SemesterCombobox.SelectedValue != null)
                        {
                            cmd.Parameters.AddWithValue("@semesterId", SemesterCombobox.SelectedValue);
                        }

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            subjectsTable = new DataTable();
                            adapter.Fill(subjectsTable);

                            // Bind the data to the DataGridView
                            allsubjectdatagrid.DataSource = subjectsTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubjectSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Skip search if placeholder text is shown
            if (SubjectSearchTextBox.Text == "Search subject code or name...")
            {
                return;
            }

            // Search subjects by subject code or name
            SearchSubjects(SubjectSearchTextBox.Text);
        }

        private void SearchSubjects(string searchText)
        {
            if (subjectsTable == null) return;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If search is empty, show all subjects based on filters
                allsubjectdatagrid.DataSource = subjectsTable;
                return;
            }

            // Create a filter expression to search by subject code or name
            string filterExpression = $"subject_code LIKE '%{searchText}%' OR subject_name LIKE '%{searchText}%'";

            // Apply the filter
            var dataView = new DataView(subjectsTable)
            {
                RowFilter = filterExpression
            };

            // Update the grid with filtered results
            allsubjectdatagrid.DataSource = dataView;
        }

        private void SubjectSearchTextBox_GotFocus(object sender, EventArgs e)
        {
            // Clear placeholder text when the search box gets focus
            if (SubjectSearchTextBox.Text == "Search subject code or name...")
            {
                SubjectSearchTextBox.Text = "";
            }
        }

        private void SubjectSearchTextBox_LostFocus(object sender, EventArgs e)
        {
            // Restore placeholder text if search box is empty and loses focus
            if (string.IsNullOrWhiteSpace(SubjectSearchTextBox.Text))
            {
                SubjectSearchTextBox.Text = "Search subject code or name...";
                // Reset search to show all subjects based on filters
                allsubjectdatagrid.DataSource = subjectsTable;
            }
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When filter ComboBox changes, reload subjects with the new filter
            LoadAllSubjects();
        }

        private void AllSubjectDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle Edit button click (column index 0)
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // Get the selected row data
                var row = allsubjectdatagrid.Rows[e.RowIndex];

                // Set the current curriculum ID
                currentCurriculumId = Convert.ToInt32(row.Cells["curriculum_id"].Value);

                // Fill the form fields with data from the selected row
                SubjectCodeTextbox.Text = Convert.ToString(row.Cells["subject_code"].Value);
                SubjectNameTextbox.Text = Convert.ToString(row.Cells["subject_name"].Value);
                LectureUnitsTextbox.Text = Convert.ToString(row.Cells["lecture_units"].Value);
                LabUnitsTextbox.Text = Convert.ToString(row.Cells["lab_units"].Value);

                // Select the correct items in ComboBoxes
                SelectComboBoxItemByValue(YearLevelCombobox, "year_level_id", Convert.ToInt32(row.Cells["year_level_id"].Value));
                SelectComboBoxItemByValue(CourseCombobox, "program_id", Convert.ToInt32(row.Cells["program_id"].Value));
                SelectComboBoxItemByValue(SYCombobox, "school_year_id", Convert.ToInt32(row.Cells["school_year_id"].Value));
                SelectComboBoxItemByValue(SemesterCombobox, "semester_id", Convert.ToInt32(row.Cells["semester_id"].Value));

                // Set curriculum year
                CurriculumYearCombobox.Text = Convert.ToString(row.Cells["curriculum_year"].Value);

                // Set subject status
                SubjectStatusCombobox.Text = Convert.ToString(row.Cells["subject_status"].Value);

                // Set professor info
                string professorName = Convert.ToString(row.Cells["professor"].Value);
                object facultyId = row.Cells["faculty_id"].Value;

                if (facultyId != null && !Convert.IsDBNull(facultyId))
                {
                    // Try to select the professor by ID
                    SelectComboBoxItemByValue(AssignedprofessorCombobox, "user_id", facultyId);
                }
                else
                {
                    // No professor assigned, select the first item (empty)
                    AssignedprofessorCombobox.SelectedIndex = 0;
                }

                // Set edit mode
                isEditMode = true;
            }
            // Handle Delete button click (column index 1)
            else if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            {
                // Get the selected row data
                var row = allsubjectdatagrid.Rows[e.RowIndex];
                int curriculumId = Convert.ToInt32(row.Cells["curriculum_id"].Value);
                string subjectCode = Convert.ToString(row.Cells["subject_code"].Value);
                string subjectName = Convert.ToString(row.Cells["subject_name"].Value);

                // Confirm deletion
                var result = MessageBox.Show($"Are you sure you want to delete this curriculum entry?{Environment.NewLine}{Environment.NewLine}Subject: {subjectCode} - {subjectName}",
                                           "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DeleteCurriculumEntry(curriculumId);
                }
            }
        }

        private void DeleteCurriculumEntry(int curriculumId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Instead of hard delete, set is_active to 0 for soft delete
                    string deleteQuery = "UPDATE curriculum SET is_active = 0, updated_at = GETDATE() WHERE curriculum_id = @curriculumId";

                    using (var cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@curriculumId", curriculumId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Curriculum entry deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAllSubjects(); // Refresh the grid
                        }
                        else
                        {
                            MessageBox.Show("No curriculum entry was deleted. The entry may have already been removed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting curriculum entry: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(SubjectCodeTextbox.Text))
            {
                MessageBox.Show("Please enter a subject code.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SubjectNameTextbox.Text))
            {
                MessageBox.Show("Please enter a subject name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isEditMode)
            {
                UpdateCurriculumEntry();
            }
            else
            {
                AddNewCurriculumEntry();
            }
        }

        private void AddNewCurriculumEntry()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // First, check if subject exists or create it
                            int subjectId = GetOrCreateSubject(connection, transaction);

                            // Then create or update curriculum entry
                            string insertQuery = @"
                            INSERT INTO curriculum 
                            (program_id, subject_id, school_year_id, semester_id, year_level_id, 
                             curriculum_year, subject_status, faculty_id, is_active, created_at)
                            VALUES 
                            (@programId, @subjectId, @schoolYearId, @semesterId, @yearLevelId, 
                             @curriculumYear, @subjectStatus, @facultyId, 1, GETDATE())";

                            using (var cmd = new SqlCommand(insertQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@programId", CourseCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@subjectId", subjectId);
                                cmd.Parameters.AddWithValue("@schoolYearId", SYCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@semesterId", SemesterCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@yearLevelId", YearLevelCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@curriculumYear", CurriculumYearCombobox.Text);
                                cmd.Parameters.AddWithValue("@subjectStatus", SubjectStatusCombobox.Text);

                                // Handle faculty assignment
                                object facultyId = AssignedprofessorCombobox.SelectedValue;
                                cmd.Parameters.AddWithValue("@facultyId", facultyId == null || Convert.IsDBNull(facultyId) ? DBNull.Value : facultyId);

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Curriculum entry added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadAllSubjects(); // Refresh the grid
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error adding curriculum entry: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCurriculumEntry()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // First, check if subject exists or create it
                            int subjectId = GetOrCreateSubject(connection, transaction);

                            // Then update curriculum entry
                            string updateQuery = @"
                            UPDATE curriculum SET
                                program_id = @programId,
                                subject_id = @subjectId,
                                school_year_id = @schoolYearId,
                                semester_id = @semesterId,
                                year_level_id = @yearLevelId,
                                curriculum_year = @curriculumYear,
                                subject_status = @subjectStatus,
                                faculty_id = @facultyId,
                                updated_at = GETDATE()
                            WHERE curriculum_id = @curriculumId";

                            using (var cmd = new SqlCommand(updateQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@curriculumId", currentCurriculumId);
                                cmd.Parameters.AddWithValue("@programId", CourseCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@subjectId", subjectId);
                                cmd.Parameters.AddWithValue("@schoolYearId", SYCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@semesterId", SemesterCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@yearLevelId", YearLevelCombobox.SelectedValue ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@curriculumYear", CurriculumYearCombobox.Text);
                                cmd.Parameters.AddWithValue("@subjectStatus", SubjectStatusCombobox.Text);

                                // Handle faculty assignment
                                object facultyId = AssignedprofessorCombobox.SelectedValue;
                                cmd.Parameters.AddWithValue("@facultyId", facultyId == null || Convert.IsDBNull(facultyId) ? DBNull.Value : facultyId);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Curriculum entry updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearForm();
                                    LoadAllSubjects(); // Refresh the grid
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("No curriculum entry was updated. The entry may have been deleted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error updating curriculum entry: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetOrCreateSubject(SqlConnection connection, SqlTransaction transaction)
        {
            // First check if subject with this code already exists
            string checkQuery = "SELECT subject_id FROM subjects WHERE subject_code = @subjectCode";
            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@subjectCode", SubjectCodeTextbox.Text.Trim());
                object result = checkCmd.ExecuteScalar();

                if (result != null)
                {
                    // Subject exists, update it and return the ID
                    string updateQuery = @"
                    UPDATE subjects SET
                        subject_name = @subjectName,
                        lecture_units = @lectureUnits,
                        lab_units = @labUnits,
                        units = @lectureUnits + @labUnits,
                        updated_at = GETDATE()
                    WHERE subject_id = @subjectId";

                    using (var updateCmd = new SqlCommand(updateQuery, connection, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@subjectId", result);
                        updateCmd.Parameters.AddWithValue("@subjectName", SubjectNameTextbox.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@lectureUnits", int.TryParse(LectureUnitsTextbox.Text, out int lectureUnits) ? lectureUnits : 0);
                        updateCmd.Parameters.AddWithValue("@labUnits", int.TryParse(LabUnitsTextbox.Text, out int labUnits) ? labUnits : 0);
                        updateCmd.ExecuteNonQuery();
                    }

                    return Convert.ToInt32(result);
                }
                else
                {
                    // Subject doesn't exist, create it
                    string insertQuery = @"
                    INSERT INTO subjects (subject_code, subject_name, lecture_units, lab_units, units, is_active, created_at)
                    VALUES (@subjectCode, @subjectName, @lectureUnits, @labUnits, @lectureUnits + @labUnits, 1, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                    using (var insertCmd = new SqlCommand(insertQuery, connection, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@subjectCode", SubjectCodeTextbox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@subjectName", SubjectNameTextbox.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@lectureUnits", int.TryParse(LectureUnitsTextbox.Text, out int lectureUnits) ? lectureUnits : 0);
                        insertCmd.Parameters.AddWithValue("@labUnits", int.TryParse(LabUnitsTextbox.Text, out int labUnits) ? labUnits : 0);

                        return Convert.ToInt32(insertCmd.ExecuteScalar());
                    }
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            SubjectCodeTextbox.Clear();
            SubjectNameTextbox.Clear();
            LectureUnitsTextbox.Clear();
            LabUnitsTextbox.Clear();

            // Reset comboboxes to default selections if they have items
            if (YearLevelCombobox.Items.Count > 0) YearLevelCombobox.SelectedIndex = 0;
            if (CourseCombobox.Items.Count > 0) CourseCombobox.SelectedIndex = 0;
            if (SYCombobox.Items.Count > 0) SYCombobox.SelectedIndex = 0;
            if (SemesterCombobox.Items.Count > 0) SemesterCombobox.SelectedIndex = 0;
            if (CurriculumYearCombobox.Items.Count > 0) CurriculumYearCombobox.SelectedIndex = 0;
            if (SubjectStatusCombobox.Items.Count > 0) SubjectStatusCombobox.SelectedIndex = 0;
            if (AssignedprofessorCombobox.Items.Count > 0) AssignedprofessorCombobox.SelectedIndex = 0;
            if (ProfessorDepartmentCombobox.Items.Count > 0) ProfessorDepartmentCombobox.SelectedIndex = 0;

            // Reset edit mode
            isEditMode = false;
            currentCurriculumId = -1;
        }

        private void GridFilterCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Use BeginInvoke to handle the event after the check state has changed
            this.BeginInvoke(new Action(() =>
            {
                string columnHeaderText = GridFilterCheckedListBox.Items[e.Index].ToString();

                // Find the corresponding column in the DataGridView
                foreach (DataGridViewColumn column in allsubjectdatagrid.Columns)
                {
                    if (column.HeaderText == columnHeaderText)
                    {
                        column.Visible = GridFilterCheckedListBox.GetItemChecked(e.Index);
                        break;
                    }
                }
            }));
        }
    }
}