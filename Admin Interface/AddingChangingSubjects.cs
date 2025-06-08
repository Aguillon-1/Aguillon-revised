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
    public partial class AddingChangingSubjects : UserControl
    {
        // Database connection string - use the same as in AccountManager
        private readonly string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Earl\\source\\repos\\CMS_Revised\\Database1.mdf;Integrated Security=True;Connect Timeout=30";

        private DataTable studentsDataTable;
        private int currentUserId = -1;
        private int currentProgramId = -1;
        private int currentYearLevelId = -1;

        public AddingChangingSubjects()
        {
            InitializeComponent();
        }

        private void AddingChangingSubjects_Load(object sender, EventArgs e)
        {
            // Initialize the UI
            InitializeUI();

            // Load students data
            LoadStudents();

            // Set up event handlers
            AddEventHandlers();

            // Set initial placeholder for enrolled subjects and available subjects
            SetEnrolledSubjectsPlaceholder("Please select a student");
            SetAvailableSubjectsPlaceholder("Please select a student");
        }

        private void InitializeUI()
        {
            // Configure the StudentListDataGrid
            ConfigureStudentListDataGrid();

            // Configure enrolled subjects grid
            ConfigureEnrolledSubjectsGrid();

            // Configure available subjects grid
            ConfigureAvailableSubjectsGrid();

            // Set up search box placeholder texts
            StudentListSearchTextbox.Text = "Search student name or ID...";
            EnrolledSubjectsListTextbox.Text = "Search enrolled subjects...";
            AvailableSubjectsTextbox.Text = "Search available subjects...";

            // Make name and ID text boxes read-only
            FullNameDisplayTextbox.ReadOnly = true;
            FullNameDisplayTextbox.BackColor = Color.WhiteSmoke;
            StudentNumTextbox.ReadOnly = true;
            StudentNumTextbox.BackColor = Color.WhiteSmoke;

            // Set ComboBox value
            StudentStatusCombobox.SelectedIndex = 0;
        }

        private void ConfigureStudentListDataGrid()
        {
            // Configure the DataGridView properties
            StudentListDataGrid.AutoGenerateColumns = false;
            StudentListDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            StudentListDataGrid.MultiSelect = false;
            StudentListDataGrid.AllowUserToAddRows = false;
            StudentListDataGrid.ReadOnly = true;
            StudentListDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            StudentListDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Configure header style
            StudentListDataGrid.EnableHeadersVisualStyles = false;
            StudentListDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            StudentListDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            StudentListDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            StudentListDataGrid.ColumnHeadersHeight = 35;

            // Clear existing columns
            StudentListDataGrid.Columns.Clear();

            // Add edit button column
            var editButtonColumn = new DataGridViewButtonColumn
            {
                Name = "edit_button",
                HeaderText = "",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FillWeight = 40
            };
            StudentListDataGrid.Columns.Add(editButtonColumn);

            // Add other columns
            var userIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "user_id",
                HeaderText = "ID",
                DataPropertyName = "user_id",
                Visible = false
            };
            StudentListDataGrid.Columns.Add(userIdColumn);

            // Add hidden columns for program and year level IDs
            var programIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "program_id",
                HeaderText = "Program ID",
                DataPropertyName = "program_id",
                Visible = false
            };
            StudentListDataGrid.Columns.Add(programIdColumn);

            var yearLevelIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "year_level_id",
                HeaderText = "Year Level ID",
                DataPropertyName = "year_level_id",
                Visible = false
            };
            StudentListDataGrid.Columns.Add(yearLevelIdColumn);

            var fullNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "fullname",
                HeaderText = "Name",
                DataPropertyName = "fullname"
            };
            StudentListDataGrid.Columns.Add(fullNameColumn);

            var studentIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "student_id",
                HeaderText = "Student No.",
                DataPropertyName = "student_id"
            };
            StudentListDataGrid.Columns.Add(studentIdColumn);

            var programColumn = new DataGridViewTextBoxColumn
            {
                Name = "program_name",
                HeaderText = "Program/Course",
                DataPropertyName = "program_name"
            };
            StudentListDataGrid.Columns.Add(programColumn);

            var yearLevelColumn = new DataGridViewTextBoxColumn
            {
                Name = "year_name",
                HeaderText = "Year Level",
                DataPropertyName = "year_name"
            };
            StudentListDataGrid.Columns.Add(yearLevelColumn);

            var sectionColumn = new DataGridViewTextBoxColumn
            {
                Name = "section_name",
                HeaderText = "Section",
                DataPropertyName = "section_name"
            };
            StudentListDataGrid.Columns.Add(sectionColumn);

            var studentStatusColumn = new DataGridViewTextBoxColumn
            {
                Name = "student_status",
                HeaderText = "Status",
                DataPropertyName = "student_status"
            };
            StudentListDataGrid.Columns.Add(studentStatusColumn);

            // Configure column widths
            StudentListDataGrid.Columns["fullname"].FillWeight = 150;
            StudentListDataGrid.Columns["student_id"].FillWeight = 100;
            StudentListDataGrid.Columns["program_name"].FillWeight = 100;
            StudentListDataGrid.Columns["year_name"].FillWeight = 80;
            StudentListDataGrid.Columns["section_name"].FillWeight = 60;
            StudentListDataGrid.Columns["student_status"].FillWeight = 80;
        }

        private void ConfigureEnrolledSubjectsGrid()
        {
            // Configure the DataGridView properties
            EnrolledSubjectsListDataGrid.AutoGenerateColumns = false;
            EnrolledSubjectsListDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            EnrolledSubjectsListDataGrid.MultiSelect = false;
            EnrolledSubjectsListDataGrid.AllowUserToAddRows = false;
            EnrolledSubjectsListDataGrid.ReadOnly = true;
            EnrolledSubjectsListDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            EnrolledSubjectsListDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            EnrolledSubjectsListDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Enable text wrapping for header and all cells
            EnrolledSubjectsListDataGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            EnrolledSubjectsListDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Clear existing columns
            EnrolledSubjectsListDataGrid.Columns.Clear();

            // Add hidden enrollment_id column
            var enrollmentIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "enrollment_id",
                HeaderText = "Enrollment ID",
                DataPropertyName = "enrollment_id",
                Visible = false
            };
            EnrolledSubjectsListDataGrid.Columns.Add(enrollmentIdColumn);

            // Add hidden offering_id column
            var offeringIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "offering_id",
                HeaderText = "Offering ID",
                DataPropertyName = "offering_id",
                Visible = false
            };
            EnrolledSubjectsListDataGrid.Columns.Add(offeringIdColumn);

            // Add subject code column
            var subjectCodeColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_code",
                HeaderText = "Subject Code",
                DataPropertyName = "subject_code",
                Visible = true,
                FillWeight = 80,
                MinimumWidth = 80
            };
            EnrolledSubjectsListDataGrid.Columns.Add(subjectCodeColumn);

            // Add subject name column
            var subjectNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_name",
                HeaderText = "Subject Name",
                DataPropertyName = "subject_name",
                Visible = true,
                FillWeight = 200, // Prioritize subject name column
                MinimumWidth = 150
            };
            EnrolledSubjectsListDataGrid.Columns.Add(subjectNameColumn);

            // Add course year column
            var courseYearColumn = new DataGridViewTextBoxColumn
            {
                Name = "course_year",
                HeaderText = "Course",
                DataPropertyName = "course_year",
                Visible = true,
                FillWeight = 80,
                MinimumWidth = 80
            };
            EnrolledSubjectsListDataGrid.Columns.Add(courseYearColumn);

            // Add remove button column
            var removeButtonColumn = new DataGridViewButtonColumn
            {
                Name = "remove_button",
                HeaderText = "",
                Text = "Remove",
                UseColumnTextForButtonValue = true,
                Visible = true,
                FillWeight = 60,
                MinimumWidth = 60
            };
            EnrolledSubjectsListDataGrid.Columns.Add(removeButtonColumn);

            // Add hidden grade column
            var gradeColumn = new DataGridViewTextBoxColumn
            {
                Name = "grade",
                HeaderText = "Grade",
                DataPropertyName = "grade",
                Visible = false
            };
            EnrolledSubjectsListDataGrid.Columns.Add(gradeColumn);
        }

        private void ConfigureAvailableSubjectsGrid()
        {
            // Configure the DataGridView properties
            AvailableSubjectsDataGrid.AutoGenerateColumns = false;
            AvailableSubjectsDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AvailableSubjectsDataGrid.MultiSelect = false;
            AvailableSubjectsDataGrid.AllowUserToAddRows = false;
            AvailableSubjectsDataGrid.ReadOnly = true;
            AvailableSubjectsDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            AvailableSubjectsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            AvailableSubjectsDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Enable text wrapping for header and all cells
            AvailableSubjectsDataGrid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            AvailableSubjectsDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Clear existing columns
            AvailableSubjectsDataGrid.Columns.Clear();

            // Add hidden subject_id column
            var subjectIdColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_id",
                HeaderText = "Subject ID",
                DataPropertyName = "subject_id",
                Visible = false
            };
            AvailableSubjectsDataGrid.Columns.Add(subjectIdColumn);

            // Add subject code column
            var subjectCodeColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_code",
                HeaderText = "Subject Code",
                DataPropertyName = "subject_code",
                Visible = true,
                FillWeight = 80,
                MinimumWidth = 80
            };
            AvailableSubjectsDataGrid.Columns.Add(subjectCodeColumn);

            // Add subject name column
            var subjectNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "subject_name",
                HeaderText = "Subject Name",
                DataPropertyName = "subject_name",
                Visible = true,
                FillWeight = 200, // Prioritize subject name column
                MinimumWidth = 150
            };
            AvailableSubjectsDataGrid.Columns.Add(subjectNameColumn);

            // Add course year column
            var courseYearColumn = new DataGridViewTextBoxColumn
            {
                Name = "course_year",
                HeaderText = "Course",
                DataPropertyName = "course_year",
                Visible = true,
                FillWeight = 80,
                MinimumWidth = 80
            };
            AvailableSubjectsDataGrid.Columns.Add(courseYearColumn);

            // Add add button column
            var addButtonColumn = new DataGridViewButtonColumn
            {
                Name = "add_button",
                HeaderText = "",
                Text = "Add",
                UseColumnTextForButtonValue = true,
                Visible = true,
                FillWeight = 60,
                MinimumWidth = 60
            };
            AvailableSubjectsDataGrid.Columns.Add(addButtonColumn);
        }

        private void AddEventHandlers()
        {
            // Add search textbox event handlers
            StudentListSearchTextbox.TextChanged += StudentListSearchTextbox_TextChanged;
            StudentListSearchTextbox.GotFocus += StudentListSearchTextbox_GotFocus;
            StudentListSearchTextbox.LostFocus += StudentListSearchTextbox_LostFocus;

            // Add enrolled subjects search event handlers
            EnrolledSubjectsListTextbox.TextChanged += EnrolledSubjectsListTextbox_TextChanged;
            EnrolledSubjectsListTextbox.GotFocus += EnrolledSubjectsListTextbox_GotFocus;
            EnrolledSubjectsListTextbox.LostFocus += EnrolledSubjectsListTextbox_LostFocus;

            // Add available subjects search event handlers
            AvailableSubjectsTextbox.TextChanged += AvailableSubjectsTextbox_TextChanged;
            AvailableSubjectsTextbox.GotFocus += AvailableSubjectsTextbox_GotFocus;
            AvailableSubjectsTextbox.LostFocus += AvailableSubjectsTextbox_LostFocus;

            // Add student grid event handlers
            StudentListDataGrid.CellClick += StudentListDataGrid_CellClick;
            StudentListDataGrid.CellContentClick += StudentListDataGrid_CellContentClick;

            // Add enrolled subjects grid event handlers
            EnrolledSubjectsListDataGrid.CellContentClick += EnrolledSubjectsListDataGrid_CellContentClick;

            // Add available subjects grid event handlers
            AvailableSubjectsDataGrid.CellContentClick += AvailableSubjectsDataGrid_CellContentClick;
        }

        private void LoadStudents()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get current school year ID from system_configuration
                    int currentSchoolYearId = -1;
                    string schoolYearQuery = @"
                        SELECT TOP 1 current_school_year_id 
                        FROM system_configuration";

                    using (var schoolYearCmd = new SqlCommand(schoolYearQuery, connection))
                    {
                        var result = schoolYearCmd.ExecuteScalar();
                        if (result != null && !Convert.IsDBNull(result))
                        {
                            currentSchoolYearId = Convert.ToInt32(result);
                        }
                        else
                        {
                            // Fallback to current school year
                            string fallbackQuery = "SELECT TOP 1 school_year_id FROM school_years WHERE is_current = 1";
                            using (var fallbackCmd = new SqlCommand(fallbackQuery, connection))
                            {
                                result = fallbackCmd.ExecuteScalar();
                                if (result != null)
                                {
                                    currentSchoolYearId = Convert.ToInt32(result);
                                }
                            }
                        }
                    }

                    // SQL query to get ALL students regardless of school year
                    string query = @"
                        SELECT u.user_id, u.first_name, u.middle_name, u.last_name, 
                               sp.student_id, p.program_id, p.program_name, sp.year_level_id, yl.year_name, 
                               s.section_name, sp.student_status,
                               sp.school_year_id
                        FROM users u
                        JOIN student_profiles sp ON u.user_id = sp.user_id
                        JOIN programs p ON sp.program_id = p.program_id
                        LEFT JOIN year_levels yl ON sp.year_level_id = yl.year_level_id
                        LEFT JOIN sections s ON sp.section_id = s.section_id
                        WHERE u.user_type = 'Student' 
                        AND (u.is_archived = 0 OR u.is_archived IS NULL)";

                    // Optionally filter by school year (comment out if you want to show ALL students)
                    if (currentSchoolYearId > 0)
                    {
                        query += " AND (sp.school_year_id = @schoolYearId)";
                    }

                    query += " ORDER BY u.last_name, u.first_name";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        // Add school year parameter if we're filtering
                        if (currentSchoolYearId > 0)
                        {
                            cmd.Parameters.AddWithValue("@schoolYearId", currentSchoolYearId);
                        }

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            studentsDataTable = new DataTable();
                            adapter.Fill(studentsDataTable);

                            // Add the fullname calculated column
                            studentsDataTable.Columns.Add("fullname", typeof(string));

                            // Calculate full names with middle initial
                            foreach (DataRow row in studentsDataTable.Rows)
                            {
                                string firstName = Convert.ToString(row["first_name"]);
                                string middleName = Convert.ToString(row["middle_name"]);
                                string lastName = Convert.ToString(row["last_name"]);

                                // Format full name with middle initial if available
                                string fullName;
                                if (!string.IsNullOrWhiteSpace(middleName))
                                {
                                    if (middleName.Length > 0)
                                    {
                                        fullName = $"{firstName} {middleName[0]}. {lastName}".Trim();
                                    }
                                    else
                                    {
                                        fullName = $"{firstName} {lastName}".Trim();
                                    }
                                }
                                else
                                {
                                    fullName = $"{firstName} {lastName}".Trim();
                                }

                                row["fullname"] = fullName;

                                // Capitalize first letter of student status
                                if (!Convert.IsDBNull(row["student_status"]))
                                {
                                    string status = row["student_status"].ToString();
                                    row["student_status"] = CapitalizeFirstLetter(status);
                                }
                            }

                            // Bind data to the grid
                            StudentListDataGrid.DataSource = studentsDataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add this override to your AddingChangingSubjects.cs class
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // When the form becomes visible again (after potentially adding students elsewhere)
            if (this.Visible)
            {
                // Reload students to refresh the list with any new accounts
                LoadStudents();
            }
        }

        // Add this method to reload students from other forms
        public void RefreshStudentList()
        {
            // This can be called from other forms when they add a student
            LoadStudents();
        }

        private string CapitalizeFirstLetter(string input)
        {
            // Helper function to capitalize the first letter of a string
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }

        private void StudentListSearchTextbox_TextChanged(object sender, EventArgs e)
        {
            // Skip search if placeholder text is shown
            if (StudentListSearchTextbox.Text == "Search student name or ID...")
            {
                return;
            }

            // Filter the student list based on search text
            SearchStudents(StudentListSearchTextbox.Text);
        }

        private void SearchStudents(string searchText)
        {
            if (studentsDataTable == null) return;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If search is empty, show all students
                StudentListDataGrid.DataSource = studentsDataTable;
                return;
            }

            // Create a filter expression to search by name or student ID
            string filterExpression = $"fullname LIKE '%{searchText}%' OR student_id LIKE '%{searchText}%'";

            // Apply the filter
            var dataView = new DataView(studentsDataTable)
            {
                RowFilter = filterExpression
            };

            // Update the grid with filtered results
            StudentListDataGrid.DataSource = dataView;
        }

        private void StudentListSearchTextbox_GotFocus(object sender, EventArgs e)
        {
            // Clear placeholder text when the search box gets focus
            if (StudentListSearchTextbox.Text == "Search student name or ID...")
            {
                StudentListSearchTextbox.Text = "";
            }
        }

        private void StudentListSearchTextbox_LostFocus(object sender, EventArgs e)
        {
            // Restore placeholder text if search box is empty and loses focus
            if (string.IsNullOrWhiteSpace(StudentListSearchTextbox.Text))
            {
                StudentListSearchTextbox.Text = "Search student name or ID...";
                // Reset search to show all students
                StudentListDataGrid.DataSource = studentsDataTable;
            }
        }

        private void EnrolledSubjectsListTextbox_TextChanged(object sender, EventArgs e)
        {
            // Skip search if placeholder text is shown
            if (EnrolledSubjectsListTextbox.Text == "Search enrolled subjects...")
            {
                return;
            }

            // Search enrolled subjects if a student is selected
            if (currentUserId > 0)
            {
                SearchEnrolledSubjects(EnrolledSubjectsListTextbox.Text);
            }
        }

        private void SearchEnrolledSubjects(string searchText)
        {
            // Get the current data source
            var dataTable = EnrolledSubjectsListDataGrid.DataSource as DataTable;

            // If no data or student loaded, return
            if (dataTable == null) return;

            // If search is empty, reload all enrolled subjects
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadStudentEnrolledSubjects(currentUserId);
                return;
            }

            // Create a filter expression to search by subject code or name
            string filterExpression = $"subject_code LIKE '%{searchText}%' OR subject_name LIKE '%{searchText}%'";

            // Apply the filter
            var dataView = new DataView(dataTable)
            {
                RowFilter = filterExpression
            };

            // Update the grid with filtered results
            EnrolledSubjectsListDataGrid.DataSource = dataView;
        }

        private void EnrolledSubjectsListTextbox_GotFocus(object sender, EventArgs e)
        {
            // Clear placeholder text when the search box gets focus
            if (EnrolledSubjectsListTextbox.Text == "Search enrolled subjects...")
            {
                EnrolledSubjectsListTextbox.Text = "";
            }
        }

        private void EnrolledSubjectsListTextbox_LostFocus(object sender, EventArgs e)
        {
            // Restore placeholder text if search box is empty and loses focus
            if (string.IsNullOrWhiteSpace(EnrolledSubjectsListTextbox.Text))
            {
                EnrolledSubjectsListTextbox.Text = "Search enrolled subjects...";
                // Reload enrolled subjects for current student
                if (currentUserId > 0)
                {
                    LoadStudentEnrolledSubjects(currentUserId);
                }
            }
        }

        private void AvailableSubjectsTextbox_TextChanged(object? sender, EventArgs e)
        {
            // Skip search if placeholder text is shown
            if (AvailableSubjectsTextbox.Text == "Search available subjects...")
            {
                return;
            }

            // Search available subjects if a student is selected
            if (currentUserId > 0)
            {
                SearchAvailableSubjects(AvailableSubjectsTextbox.Text);
            }
        }


        private void SearchAvailableSubjects(string searchText)
        {
            // Get the current data source
            var dataTable = AvailableSubjectsDataGrid.DataSource as DataTable;

            // If no data or student loaded, return
            if (dataTable == null) return;

            // If search is empty, reload all available subjects
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadAvailableSubjects();
                return;
            }

            // Create a filter expression to search by subject code or name
            string filterExpression = $"subject_code LIKE '%{searchText}%' OR subject_name LIKE '%{searchText}%'";

            // Apply the filter
            var dataView = new DataView(dataTable)
            {
                RowFilter = filterExpression
            };

            // Update the grid with filtered results
            AvailableSubjectsDataGrid.DataSource = dataView;
        }

        private void AvailableSubjectsTextbox_GotFocus(object sender, EventArgs e)
        {
            // Clear placeholder text when the search box gets focus
            if (AvailableSubjectsTextbox.Text == "Search available subjects...")
            {
                AvailableSubjectsTextbox.Text = "";
            }
        }

        private void AvailableSubjectsTextbox_LostFocus(object sender, EventArgs e)
        {
            // Restore placeholder text if search box is empty and loses focus
            if (string.IsNullOrWhiteSpace(AvailableSubjectsTextbox.Text))
            {
                AvailableSubjectsTextbox.Text = "Search available subjects...";
                // Reload available subjects
                if (currentUserId > 0)
                {
                    LoadAvailableSubjects();
                }
            }
        }

        private void StudentListDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Skip if clicking on header or edit button column
            if (e.RowIndex < 0 || e.ColumnIndex == 0) return;

            SelectStudentRow(e.RowIndex);
        }

        private void StudentListDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the edit button was clicked
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                // Select the student and enable editing
                SelectStudentRow(e.RowIndex, true);
            }
        }

        private void SelectStudentRow(int rowIndex, bool enableEdit = false)
        {
            // Get selected student data
            int userId = Convert.ToInt32(StudentListDataGrid.Rows[rowIndex].Cells["user_id"].Value);
            string fullName = Convert.ToString(StudentListDataGrid.Rows[rowIndex].Cells["fullname"].Value);
            string studentId = Convert.ToString(StudentListDataGrid.Rows[rowIndex].Cells["student_id"].Value);
            string studentStatus = Convert.ToString(StudentListDataGrid.Rows[rowIndex].Cells["student_status"].Value);

            // Get program and year level IDs for filtering available subjects
            currentProgramId = Convert.ToInt32(StudentListDataGrid.Rows[rowIndex].Cells["program_id"].Value);
            currentYearLevelId = Convert.ToInt32(StudentListDataGrid.Rows[rowIndex].Cells["year_level_id"].Value);

            // Store the current user ID
            currentUserId = userId;

            // Display selected student info in text boxes
            FullNameDisplayTextbox.Text = fullName;
            StudentNumTextbox.Text = studentId;

            // Set student status in combo box
            StudentStatusCombobox.Text = studentStatus;

            // Set student name and ID fields to readonly
            FullNameDisplayTextbox.ReadOnly = true;
            StudentNumTextbox.ReadOnly = true;

            // Enable/disable student status combo box based on edit mode
            StudentStatusCombobox.Enabled = enableEdit;

            // Load the student's enrolled subjects
            LoadStudentEnrolledSubjects(userId);

            // Load available subjects for this student's program and year level
            LoadAvailableSubjects();

            // Display status indicator
            bool isRegularStudent = (studentStatus.ToLower() == "regular");
            Color statusColor = isRegularStudent ? Color.Green : Color.Orange;

            // Update the status label if it exists (you might need to add this label to your form)
            // StudentStatusLabel.ForeColor = statusColor;
            // StudentStatusLabel.Text = isRegularStudent ? "Regular" : "Irregular";

            // Add this code to the SelectStudentRow method after setting currentUserId
            // Check if default subject enrollment is enabled
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if auto enrollment is enabled
                bool autoEnrollEnabled = true;
                string configQuery = "SELECT TOP 1 auto_enroll_enabled FROM system_configuration";
                using (var configCmd = new SqlCommand(configQuery, connection))
                {
                    var result = configCmd.ExecuteScalar();
                    if (result != null && !Convert.IsDBNull(result))
                    {
                        autoEnrollEnabled = Convert.ToBoolean(result);
                    }
                }

                // Auto-enroll if enabled
                if (autoEnrollEnabled)
                {
                    try
                    {
                        // Call the EnrollStudentInDefaultSubjects stored procedure
                        using (var cmd = new SqlCommand("EnrollStudentInDefaultSubjects", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@student_user_id", currentUserId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log any enrollment errors but continue loading
                        Console.WriteLine($"Error auto-enrolling student: {ex.Message}");
                    }
                }
            }
        }

        private void LoadStudentEnrolledSubjects(int userId)
        {
            // Clear any existing data
            EnrolledSubjectsListDataGrid.DataSource = null;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Enhanced query to get subject enrollments with more details
                    string query = @"
                        -- Get current school year and semester from system_configuration
                        DECLARE @currentSchoolYearId INT;
                        DECLARE @currentSemesterId INT;
                        
                        -- Get the current settings from system_configuration
                        SELECT TOP 1 
                            @currentSchoolYearId = current_school_year_id, 
                            @currentSemesterId = current_semester_id
                        FROM system_configuration;
                        
                        -- If no configuration exists, use defaults
                        IF @currentSchoolYearId IS NULL
                            SELECT TOP 1 @currentSchoolYearId = school_year_id FROM school_years WHERE is_current = 1;
                        
                        IF @currentSemesterId IS NULL
                            SELECT TOP 1 @currentSemesterId = semester_id FROM semesters WHERE semester_code = '1ST';

                        -- This query gets all enrolled subjects for the student
                        SELECT 
                            e.enrollment_id, 
                            e.offering_id, 
                            s.subject_id,
                            s.subject_code, 
                            s.subject_name, 
                            CONCAT(yl.year_name, ' ', p.program_name) AS course_year, 
                            e.grade
                        FROM enrollments e
                        JOIN subject_offerings so ON e.offering_id = so.offering_id
                        JOIN subjects s ON so.subject_id = s.subject_id
                        JOIN year_levels yl ON so.year_level_id = yl.year_level_id
                        JOIN programs p ON p.program_id = (
                            SELECT sp.program_id FROM student_profiles sp WHERE sp.user_id = e.student_id
                        )
                        WHERE e.student_id = @userId
                        AND (so.is_active = 1 OR so.is_active IS NULL)
                        ORDER BY yl.year_level_id, s.subject_code";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var enrolledSubjectsTable = new DataTable();
                            adapter.Fill(enrolledSubjectsTable);

                            // Check if there are any enrolled subjects
                            if (enrolledSubjectsTable.Rows.Count == 0)
                            {
                                SetEnrolledSubjectsPlaceholder("No enrolled subjects found");
                            }
                            else
                            {
                                // Bind to the enrolled subjects grid
                                EnrolledSubjectsListDataGrid.DataSource = enrolledSubjectsTable;

                                // Make sure all columns are visible in the correct order
                                foreach (DataGridViewColumn col in EnrolledSubjectsListDataGrid.Columns)
                                {
                                    col.Visible = (col.Name == "subject_code" || col.Name == "subject_name" ||
                                          col.Name == "course_year" || col.Name == "remove_button" ||
                                          col.Name == "enrollment_id" || col.Name == "offering_id");
                                }

                                // Ensure columns are in the right order
                                EnrolledSubjectsListDataGrid.Columns["subject_code"].DisplayIndex = 0;
                                EnrolledSubjectsListDataGrid.Columns["subject_name"].DisplayIndex = 1;
                                EnrolledSubjectsListDataGrid.Columns["course_year"].DisplayIndex = 2;
                                EnrolledSubjectsListDataGrid.Columns["remove_button"].DisplayIndex = 3;

                                // Hide the internal IDs
                                EnrolledSubjectsListDataGrid.Columns["enrollment_id"].Visible = false;
                                EnrolledSubjectsListDataGrid.Columns["offering_id"].Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading enrolled subjects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetEnrolledSubjectsPlaceholder($"Error loading subjects: {ex.Message}");
            }
        }

        // Modified LoadAvailableSubjects method to remove the popup and ensure proper filtering
        private void LoadAvailableSubjects()
        {
            // Clear any existing data
            AvailableSubjectsDataGrid.DataSource = null;

            // If no student is selected, show placeholder
            if (currentUserId <= 0)
            {
                SetAvailableSubjectsPlaceholder("Please select a student");
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get the student's program ID to filter subjects
                    int studentProgramId;
                    string studentProgramQuery = "SELECT program_id FROM student_profiles WHERE user_id = @userId";
                    using (var programCmd = new SqlCommand(studentProgramQuery, connection))
                    {
                        programCmd.Parameters.AddWithValue("@userId", currentUserId);
                        studentProgramId = Convert.ToInt32(programCmd.ExecuteScalar());
                    }

                    // Get current semester from system_configuration
                    int currentSchoolYearId;
                    int currentSemesterId;

                    string currentSystemSettingsQuery = @"
                        SELECT TOP 1 
                            sc.current_school_year_id, 
                            sc.current_semester_id
                        FROM system_configuration sc";

                    using (var settingsCmd = new SqlCommand(currentSystemSettingsQuery, connection))
                    {
                        using (var reader = settingsCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentSchoolYearId = Convert.ToInt32(reader["current_school_year_id"]);
                                currentSemesterId = Convert.ToInt32(reader["current_semester_id"]);
                            }
                            else
                            {
                                // Fallback to defaults if no settings found
                                using (var fallbackCmd = new SqlCommand("SELECT TOP 1 school_year_id FROM school_years WHERE is_current = 1", connection))
                                {
                                    currentSchoolYearId = Convert.ToInt32(fallbackCmd.ExecuteScalar());
                                }
                                using (var fallbackCmd = new SqlCommand("SELECT TOP 1 semester_id FROM semesters ORDER BY semester_id", connection))
                                {
                                    currentSemesterId = Convert.ToInt32(fallbackCmd.ExecuteScalar());
                                }
                            }
                        }
                    }

                    // Get all enrolled subject IDs for this student to exclude them
                    var enrolledSubjectIds = new HashSet<int>();
                    string enrolledQuery = @"
                        SELECT s.subject_id 
                        FROM enrollments e
                        JOIN subject_offerings so ON e.offering_id = so.offering_id
                        JOIN subjects s ON so.subject_id = s.subject_id
                        WHERE e.student_id = @userId";

                    using (var enrolledCmd = new SqlCommand(enrolledQuery, connection))
                    {
                        enrolledCmd.Parameters.AddWithValue("@userId", currentUserId);
                        using (var reader = enrolledCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                enrolledSubjectIds.Add(Convert.ToInt32(reader["subject_id"]));
                            }
                        }
                    }

                    // Query for available subjects - show ALL subjects from the student's program that they're not enrolled in
                    // This includes subjects from the current semester and school year
                    string query = @"
                        SELECT 
                            -1 AS offering_id, -- We'll create an offering if needed when adding
                            s.subject_id,
                            s.subject_code,
                            s.subject_name,
                            CONCAT(yl.year_name, ' ', p.program_name) AS course_year
                        FROM curriculum c
                        JOIN subjects s ON c.subject_id = s.subject_id
                        JOIN programs p ON c.program_id = p.program_id
                        JOIN year_levels yl ON c.year_level_id = yl.year_level_id
                        WHERE 
                            c.program_id = @programId -- Only show subjects from the student's program
                            AND c.school_year_id = @schoolYearId -- Current school year
                            AND c.semester_id = @semesterId -- Current semester
                            AND c.is_active = 1
                            AND s.is_active = 1
                            AND s.subject_id NOT IN (
                                SELECT DISTINCT so.subject_id 
                                FROM enrollments e
                                JOIN subject_offerings so ON e.offering_id = so.offering_id
                                WHERE e.student_id = @userId
                            )
                        ORDER BY yl.year_level_id, s.subject_code";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", currentUserId);
                        cmd.Parameters.AddWithValue("@programId", studentProgramId);
                        cmd.Parameters.AddWithValue("@schoolYearId", currentSchoolYearId);
                        cmd.Parameters.AddWithValue("@semesterId", currentSemesterId);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            var availableSubjectsTable = new DataTable();
                            adapter.Fill(availableSubjectsTable);

                            // Check if there are any available subjects
                            if (availableSubjectsTable.Rows.Count == 0)
                            {
                                SetAvailableSubjectsPlaceholder("No additional subjects available for enrollment");
                            }
                            else
                            {
                                // Bind to the available subjects grid
                                AvailableSubjectsDataGrid.DataSource = availableSubjectsTable;

                                // Make sure all columns are visible in the correct order
                                foreach (DataGridViewColumn col in AvailableSubjectsDataGrid.Columns)
                                {
                                    col.Visible = (col.Name == "subject_code" || col.Name == "subject_name" ||
                                            col.Name == "course_year" || col.Name == "add_button" ||
                                            col.Name == "subject_id");
                                }

                                // Ensure columns are in the right order
                                AvailableSubjectsDataGrid.Columns["subject_code"].DisplayIndex = 0;
                                AvailableSubjectsDataGrid.Columns["subject_name"].DisplayIndex = 1;
                                AvailableSubjectsDataGrid.Columns["course_year"].DisplayIndex = 2;
                                AvailableSubjectsDataGrid.Columns["add_button"].DisplayIndex = 3;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading available subjects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetAvailableSubjectsPlaceholder($"Error loading subjects: {ex.Message}");
            }
        }

        private void SetEnrolledSubjectsPlaceholder(string message)
        {
            // Create a placeholder table for the enrolled subjects grid
            var placeholderTable = new DataTable();
            placeholderTable.Columns.Add("enrollment_id", typeof(int));
            placeholderTable.Columns.Add("offering_id", typeof(int));
            placeholderTable.Columns.Add("subject_code", typeof(string));
            placeholderTable.Columns.Add("subject_name", typeof(string));
            placeholderTable.Columns.Add("course_year", typeof(string));
            placeholderTable.Columns.Add("grade", typeof(string));

            // Add a placeholder row
            var row = placeholderTable.NewRow();
            row["subject_name"] = message;
            placeholderTable.Rows.Add(row);

            // Set the data source
            EnrolledSubjectsListDataGrid.DataSource = placeholderTable;

            // Hide all columns except subject name to show message
            foreach (DataGridViewColumn column in EnrolledSubjectsListDataGrid.Columns)
            {
                if (column.Name != "subject_name")
                {
                    column.Visible = false;
                }
            }

            // Center the message
            EnrolledSubjectsListDataGrid.Columns["subject_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            EnrolledSubjectsListDataGrid.Columns["subject_name"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
        }

        private void SetAvailableSubjectsPlaceholder(string message)
        {
            // Create a placeholder table for the available subjects grid
            var placeholderTable = new DataTable();
            placeholderTable.Columns.Add("offering_id", typeof(int));
            placeholderTable.Columns.Add("subject_id", typeof(int));
            placeholderTable.Columns.Add("subject_code", typeof(string));
            placeholderTable.Columns.Add("subject_name", typeof(string));
            placeholderTable.Columns.Add("course_year", typeof(string));

            // Add a placeholder row
            var row = placeholderTable.NewRow();
            row["subject_name"] = message;
            placeholderTable.Rows.Add(row);

            // Set the data source
            AvailableSubjectsDataGrid.DataSource = placeholderTable;

            // Hide all columns except subject name to show message
            foreach (DataGridViewColumn column in AvailableSubjectsDataGrid.Columns)
            {
                if (column.Name != "subject_name")
                {
                    column.Visible = false;
                }
            }

            // Center the message
            AvailableSubjectsDataGrid.Columns["subject_name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            AvailableSubjectsDataGrid.Columns["subject_name"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Italic);
        }

        private void EnrolledSubjectsListDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the remove button was clicked
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = EnrolledSubjectsListDataGrid.Columns[e.ColumnIndex].Name;
                if (columnName == "remove_button")
                {
                    // Get the enrollment ID from the selected row
                    var enrollmentId = Convert.ToInt32(EnrolledSubjectsListDataGrid.Rows[e.RowIndex].Cells["enrollment_id"].Value);
                    var subjectName = Convert.ToString(EnrolledSubjectsListDataGrid.Rows[e.RowIndex].Cells["subject_name"].Value);

                    // Confirm removal
                    var result = MessageBox.Show($"Are you sure you want to remove '{subjectName}' from this student's enrollment?",
                                               "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        RemoveSubjectEnrollment(enrollmentId);
                    }
                }
            }
        }

        private void AvailableSubjectsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the add button was clicked
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = AvailableSubjectsDataGrid.Columns[e.ColumnIndex].Name;
                if (columnName == "add_button")
                {
                    // Get the subject ID from the selected row
                    var subjectId = Convert.ToInt32(AvailableSubjectsDataGrid.Rows[e.RowIndex].Cells["subject_id"].Value);
                    var subjectName = Convert.ToString(AvailableSubjectsDataGrid.Rows[e.RowIndex].Cells["subject_name"].Value);

                    // Confirm addition
                    var result = MessageBox.Show($"Are you sure you want to enroll this student in '{subjectName}'?",
                                               "Confirm Enrollment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        AddSubjectEnrollment(subjectId);
                    }
                }
            }
        }

        private void RemoveSubjectEnrollment(int enrollmentId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM enrollments WHERE enrollment_id = @enrollmentId";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@enrollmentId", enrollmentId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Subject enrollment removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh both grids
                            LoadStudentEnrolledSubjects(currentUserId);
                            LoadAvailableSubjects();
                        }
                        else
                        {
                            MessageBox.Show("Failed to remove subject enrollment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing subject enrollment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSubjectEnrollment(int subjectId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // First, find or create a subject offering for this subject
                    int offeringId = GetOrCreateSubjectOffering(connection, subjectId);

                    if (offeringId > 0)
                    {
                        // Add the enrollment
                        string enrollQuery = @"
                            INSERT INTO enrollments (student_id, offering_id, enrollment_date)
                            VALUES (@studentId, @offeringId, @enrollmentDate)";

                        using (var cmd = new SqlCommand(enrollQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@studentId", currentUserId);
                            cmd.Parameters.AddWithValue("@offeringId", offeringId);
                            cmd.Parameters.AddWithValue("@enrollmentDate", DateTime.Now);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Subject enrollment added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Refresh both grids
                                LoadStudentEnrolledSubjects(currentUserId);
                                LoadAvailableSubjects();
                            }
                            else
                            {
                                MessageBox.Show("Failed to add subject enrollment.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding subject enrollment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetOrCreateSubjectOffering(SqlConnection connection, int subjectId)
        {
            try
            {
                // Get current system settings
                int currentSchoolYearId = 1;
                int currentSemesterId = 1;

                string configQuery = @"
                    SELECT TOP 1 current_school_year_id, current_semester_id 
                    FROM system_configuration";

                using (var configCmd = new SqlCommand(configQuery, connection))
                {
                    using (var reader = configCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentSchoolYearId = Convert.ToInt32(reader["current_school_year_id"]);
                            currentSemesterId = Convert.ToInt32(reader["current_semester_id"]);
                        }
                    }
                }

                // Check if an offering already exists
                string checkQuery = @"
                    SELECT offering_id 
                    FROM subject_offerings 
                    WHERE subject_id = @subjectId 
                    AND school_year_id = @schoolYearId 
                    AND semester_id = @semesterId 
                    AND is_active = 1";

                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@subjectId", subjectId);
                    checkCmd.Parameters.AddWithValue("@schoolYearId", currentSchoolYearId);
                    checkCmd.Parameters.AddWithValue("@semesterId", currentSemesterId);

                    var existingOffering = checkCmd.ExecuteScalar();
                    if (existingOffering != null)
                    {
                        return Convert.ToInt32(existingOffering);
                    }
                }

                // Create a new offering if none exists
                string insertQuery = @"
                    INSERT INTO subject_offerings (subject_id, class_code, school_year_id, semester_id, year_level_id, is_active)
                    VALUES (@subjectId, @classCode, @schoolYearId, @semesterId, @yearLevelId, 1);
                    SELECT SCOPE_IDENTITY();";

                using (var insertCmd = new SqlCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@subjectId", subjectId);
                    insertCmd.Parameters.AddWithValue("@classCode", $"AUTO-{subjectId}-{DateTime.Now:yyyyMMdd}");
                    insertCmd.Parameters.AddWithValue("@schoolYearId", currentSchoolYearId);
                    insertCmd.Parameters.AddWithValue("@semesterId", currentSemesterId);
                    insertCmd.Parameters.AddWithValue("@yearLevelId", currentYearLevelId);

                    var newOfferingId = insertCmd.ExecuteScalar();
                    return Convert.ToInt32(newOfferingId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating subject offering: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}