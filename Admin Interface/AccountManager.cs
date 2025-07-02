using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using CMS_Revised.Connections;
using CMS_Revised.User_Interface; // Add this at the top if not present

namespace ClassroomManagementSystem
{
    public partial class AccountManager : UserControl
    {
        // Database connection string - updated to use local MDF file


        private DataTable userDataTable;
        private bool isNewUser = true;
        private int currentUserId = -1;
        private bool isDbConnected = false;
        private int schoolYearId = -1;
        private string currentStudentStatus = "Regular";
        private string currentAcademicStatus = "Active";

        // Dictionary to map column names to display names
        private Dictionary<string, string> columnMappings;

        public AccountManager()
        {
            InitializeComponent();
            SetEditMode(false); // Make all fields read-only/disabled by default
        }

        private void AccountManager_Load(object sender, EventArgs e)
        {
            // Check database connection first
            if (!CheckDatabaseConnection())
            {
                MessageBox.Show("Failed to connect to the database. Some features may not work correctly.",
                    "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Continue with UI initialization but mark connection as failed
                isDbConnected = false;
            }
            else
            {
                isDbConnected = true;

                // Initialize the school years combo box
                InitializeSchoolYearsComboBox();
            }

            // Initialize column mappings first
            InitializeColumnMappings();

            // Then initialize UI
            InitializeUI();

            // Load users after UI is ready (only if database is connected)
            if (isDbConnected)
            {
                LoadUsers();
            }

            // Set up CheckedListBox for column visibility control
            SetupDbsortCheckedListBox();

            // Set the password textbox to use password char
            PasswordTextBox.UseSystemPasswordChar = true;

            Bdaypicket.Format = DateTimePickerFormat.Custom;
            Bdaypicket.CustomFormat = "MM/dd/yyyy";
        }

        private bool CheckDatabaseConnection()
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
        }

        private void InitializeColumnMappings()
        {
            // Create dictionary of column name to display name mappings
            columnMappings = new Dictionary<string, string>
            {
                {"user_id", "ID"},
                {"fullname", "Name"},            // New column for concatenated name
                {"student_id", "Student ID"},
                {"year_level_id", "Year Level"},
                {"program_id", "Course"},
                {"section_id", "Section"},
                {"sex", "Sex"},                  // Added sex after section
                {"email", "Email"},
                {"contact_number", "Contact No."},  // Added contact number after email
                {"school_year", "School Year"},  // Added school year before role
                {"user_type", "Role"},
                {"student_status", "Student Status"}, // Added student status
                {"birthday", "Birthday"},        // Added birthday at the far right
                {"is_archived", "Archived"}      // Added archived status
            };
            // Removed "username" from the mappings
            // Removed "first_name" and "last_name" as individual columns
        }

        private void SetupDbsortCheckedListBox()
        {
            // Clear any existing items
            DbsortCheckedListBox.Items.Clear();

            // Add items based on column mappings (excluding user_id and is_archived which is handled differently)
            foreach (var kvp in columnMappings)
            {
                if (kvp.Key != "user_id" && kvp.Key != "is_archived")
                {
                    // Add display name to the CheckedListBox
                    int index = DbsortCheckedListBox.Items.Add(kvp.Value);

                    // Default columns to checked (visible) except contact_number and birthday
                    bool shouldCheck = (kvp.Key != "contact_number" && kvp.Key != "birthday");
                    DbsortCheckedListBox.SetItemChecked(index, shouldCheck);

                    // Also set the column visibility
                    if (UserDataGrid.Columns.Contains(kvp.Key))
                    {
                        UserDataGrid.Columns[kvp.Key].Visible = shouldCheck;
                    }
                }
            }

            // Add special "Archived Users" item at the end that controls both visibility and filtering
            int archiveIndex = DbsortCheckedListBox.Items.Add("Archived Users");
            DbsortCheckedListBox.SetItemChecked(archiveIndex, false); // Default to NOT showing archived users

            // Hide the archived column by default
            if (UserDataGrid.Columns.Contains("is_archived"))
            {
                UserDataGrid.Columns["is_archived"].Visible = false;
            }

            // Add event handler for item check state change
            DbsortCheckedListBox.ItemCheck += DbsortCheckedListBox_ItemCheck;

            // Initially filter to show only non-archived users
            FilterArchivedUsers(false);
        }

        private void FilterArchivedUsers(bool showArchived)
        {
            if (userDataTable == null) return;

            // If showing all users, just reload the data
            if (showArchived)
            {
                PopulateDataGrid(userDataTable);
                return;
            }

            // Otherwise, filter out archived users
            DataRow[] filteredRows = userDataTable.Select("is_archived = 0 OR is_archived IS NULL");

            // Create filtered table
            DataTable filteredTable = userDataTable.Clone();
            foreach (DataRow row in filteredRows)
            {
                filteredTable.ImportRow(row);
            }

            // Update grid with filtered data
            PopulateDataGrid(filteredTable);
        }

        private void DbsortCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Get the display name of the column being toggled
            string columnDisplayName = DbsortCheckedListBox.Items[e.Index].ToString();

            // Special handling for "Archived Users" item
            if (columnDisplayName == "Archived Users")
            {
                // This controls both filtering and visibility of the archived column
                bool showArchived = (e.NewValue == CheckState.Checked);

                // Set archived column visibility
                if (UserDataGrid.Columns.Contains("is_archived"))
                {
                    UserDataGrid.Columns["is_archived"].Visible = showArchived;
                }

                // Filter data on the next UI update to avoid modification during enumeration
                BeginInvoke(new Action(() => FilterArchivedUsers(showArchived)));
                return;
            }

            // For regular columns, find the column name from the display name
            string columnName = "";
            foreach (var kvp in columnMappings)
            {
                if (kvp.Value == columnDisplayName)
                {
                    columnName = kvp.Key;
                    break;
                }
            }

            // If we found the column name, toggle its visibility
            if (!string.IsNullOrEmpty(columnName) && UserDataGrid.Columns.Contains(columnName))
            {
                // Set visibility based on the new check state
                UserDataGrid.Columns[columnName].Visible = (e.NewValue == CheckState.Checked);
            }
        }

        private void InitializeUI()
        {
            // Clear textboxes and set default values
            ClearForm();

            // Configure UserDataGrid
            ConfigureDataGrid();

            // Configure ComboBoxes to be non-editable (drop-down list only)
            ConfigureComboBoxes();

            // Add event handlers for textbox validation
            AddTextBoxHandlers();
        }

        private void ConfigureComboBoxes()
        {
            // Make all ComboBoxes dropdown-only (non-editable)
            RoleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            CourseCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            YearCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SectionCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SchoolYRCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SexCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            StudentStatusCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ConfigureDataGrid()
        {
            // Ensure UserDataGrid is not null
            if (UserDataGrid == null)
            {
                MessageBox.Show("UserDataGrid is not initialized. Please check the Designer file.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Configure the DataGridView properties
            UserDataGrid.AutoGenerateColumns = false;
            UserDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            UserDataGrid.MultiSelect = false;
            UserDataGrid.AllowUserToAddRows = false;
            UserDataGrid.ReadOnly = true;

            // Set AutoSizeMode for the control itself
            UserDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Configure header style - make headers bold and 11pt
            UserDataGrid.EnableHeadersVisualStyles = false;
            UserDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            UserDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            UserDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            UserDataGrid.ColumnHeadersHeight = 35; // Make headers slightly taller to accommodate larger font

            // Clear existing columns
            UserDataGrid.Columns.Clear();

            // Add columns in the specified order
            string[] columnKeys = {"user_id", "fullname", "student_id", "year_level_id", "program_id",
                                 "section_id", "sex", "email", "contact_number", "school_year",
                                 "user_type", "student_status", "birthday", "is_archived"};

            foreach (string key in columnKeys)
            {
                if (columnMappings.ContainsKey(key))
                {
                    var column = new DataGridViewTextBoxColumn();
                    column.Name = key;
                    column.HeaderText = columnMappings[key];
                    column.DataPropertyName = key;

                    // Special handling for fullname which doesn't directly map to a database column
                    if (key == "fullname")
                    {
                        column.DataPropertyName = null; // We'll set this value manually
                    }

                    // Special handling for is_archived to show as Yes/No
                    if (key == "is_archived")
                    {
                        column.DataPropertyName = null; // We'll set this value manually
                    }

                    UserDataGrid.Columns.Add(column);
                }
                else
                {
                    MessageBox.Show($"Column key '{key}' is missing in columnMappings. Please check the InitializeColumnMappings method.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Configure column properties
            if (UserDataGrid.Columns.Contains("user_id"))
            {
                UserDataGrid.Columns["user_id"].Visible = false;
            }

            // Prioritize name and student ID columns by giving them more fill weight
            if (UserDataGrid.Columns.Contains("fullname"))
            {
                UserDataGrid.Columns["fullname"].FillWeight = 150; // Give full name more space
                UserDataGrid.Columns["fullname"].MinimumWidth = 140;
            }

            if (UserDataGrid.Columns.Contains("student_id"))
            {
                UserDataGrid.Columns["student_id"].FillWeight = 100;
                UserDataGrid.Columns["student_id"].MinimumWidth = 100;
            }

            // Configure other columns
            if (UserDataGrid.Columns.Contains("program_id"))
            {
                UserDataGrid.Columns["program_id"].FillWeight = 100; // Course
                UserDataGrid.Columns["program_id"].MinimumWidth = 100;
            }

            if (UserDataGrid.Columns.Contains("student_status"))
            {
                UserDataGrid.Columns["student_status"].FillWeight = 80; // Student Status
            }

            // Set automatic text trimming for cells with overflow text
            foreach (DataGridViewColumn column in UserDataGrid.Columns)
            {
                ((DataGridViewTextBoxColumn)column).DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            }

            // Set the row height to accommodate the data properly
            UserDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            UserDataGrid.RowTemplate.Height = 25;

            // After data is loaded, we'll perform an additional auto-size
            UserDataGrid.DataBindingComplete += UserDataGrid_DataBindingComplete;
        }

        private void UserDataGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // After data is loaded, calculate and adjust column widths based on content
            // This ensures names and student IDs are fully visible

            // Temporarily switch to DisplayedCells mode to calculate optimal widths
            DataGridViewAutoSizeColumnsMode originalMode = UserDataGrid.AutoSizeColumnsMode;
            UserDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // First auto-size by header (to ensure column headers are visible)
            UserDataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);

            // Then auto-size high-priority columns by their contents
            if (UserDataGrid.Columns.Contains("fullname"))
            {
                UserDataGrid.Columns["fullname"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (UserDataGrid.Columns.Contains("student_id"))
            {
                UserDataGrid.Columns["student_id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            // Return to original sizing mode
            UserDataGrid.AutoSizeColumnsMode = originalMode;
        }


        private void AddTextBoxHandlers()
        {
            // Original handlers
            EmailTextBox.Leave += EmailTextBox_Leave;
            FnameTextBox.Leave += NameTextBox_Leave;
            MnameTextBox.Leave += NameTextBox_Leave;
            LnameTextBox.Leave += NameTextBox_Leave;

            // Add handlers for search functionality
            dbSearchTextBox.TextChanged += dbSearchTextBox_TextChanged;
            dbSearchTextBox.GotFocus += dbSearchTextBox_GotFocus;
            dbSearchTextBox.LostFocus += dbSearchTextBox_LostFocus;

            // New handlers for additional fields
            ContactNoTextBox.Leave += ContactNoTextBox_Leave;
            TextBox1.Leave += StudentIdTextBox_Leave;
            Bdaypicket.ValueChanged += Bdaypicket_ValueChanged;
            StudentStatusCombobox.SelectedIndexChanged += StudentStatus_SelectedIndexChanged;
            SchoolYRCombobox.SelectedIndexChanged += SchoolYear_SelectedIndexChanged;
            SexCombobox.SelectedIndexChanged += Sex_SelectedIndexChanged;
            ComboBox4.SelectedIndexChanged += AcademicStatus_SelectedIndexChanged;
        }

        private void ContactNoTextBox_Leave(object sender, EventArgs e)
        {
            // Validate phone number format
            if (!string.IsNullOrEmpty(ContactNoTextBox.Text))
            {
                // Simple validation for Philippine phone numbers
                if (!Regex.IsMatch(ContactNoTextBox.Text, @"^(09|\+639)\d{9}$"))
                {
                    MessageBox.Show("Please enter a valid phone number format (09XXXXXXXXX or +639XXXXXXXXX)",
                        "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void StudentIdTextBox_Leave(object sender, EventArgs e)
        {
            // Format and validate student ID 
            if (!string.IsNullOrEmpty(TextBox1.Text))
            {
                // Convert to uppercase
                TextBox1.Text = TextBox1.Text.Trim().ToUpper();

                // Validate student ID format (8 digits-Letter)
                if (!Regex.IsMatch(TextBox1.Text, @"^\d{8}-[A-Z]$"))
                {
                    MessageBox.Show("Student ID must be in the format: 8 digits followed by a dash and a letter (e.g., 20230902-N)",
                               "Invalid Student ID Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Bdaypicket_ValueChanged(object sender, EventArgs e)
        {
            // Check that birthdate is not in the future
            if (Bdaypicket.Value > DateTime.Today)
            {
                MessageBox.Show("Birthday cannot be in the future", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Bdaypicket.Value = DateTime.Today;
            }
        }

        private void StudentStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update current student status
            if (StudentStatusCombobox.SelectedIndex >= 0)
            {
                currentStudentStatus = StudentStatusCombobox.SelectedItem.ToString();
            }
        }

        private void SchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchoolYRCombobox.SelectedIndex >= 0)
            {
                string selectedSY = SchoolYRCombobox.SelectedItem.ToString();

                using (var connection = DatabaseConn.GetConnection())
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT school_year_id FROM school_years WHERE year_name = @yearName";
                        using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@yearName", selectedSY);
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                schoolYearId = Convert.ToInt32(result);
                            }
                            else
                            {
                                schoolYearId = -1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving school year ID: {ex.Message}");
                        schoolYearId = -1;
                    }
                }
            }
        }

        private void Sex_SelectedIndexChanged(object sender, EventArgs e)
        {
            // No additional logic needed, the value will be read directly when saving
        }

        private void AcademicStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update current academic status
            if (ComboBox4.SelectedIndex >= 0)
            {
                currentAcademicStatus = ComboBox4.SelectedItem.ToString();
            }
        }

        private void dbSearchTextBox_GotFocus(object sender, EventArgs e)
        {
            if (dbSearchTextBox.Text == "Search Name, ID and ect...")
            {
                dbSearchTextBox.Text = "";
            }
        }

        private void dbSearchTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dbSearchTextBox.Text))
            {
                dbSearchTextBox.Text = "Search Name, ID and ect...";
            }
        }

        private void dbSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (dbSearchTextBox.Text != "Search Name, ID and ect...")
            {
                PerformDatabaseSearch(dbSearchTextBox.Text);
            }
            else
            {
                // If placeholder text, load all users
                LoadUsers();
            }
        }

        private void PerformDatabaseSearch(string searchTerm)
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection()) // Explicitly using Microsoft.Data.SqlClient.SqlConnection
                {
                    connection.Open();
                    string query = @"
                        SELECT u.user_id, u.email, u.password_hash, 
                               u.first_name, u.middle_name, u.last_name, u.user_type,
                               u.birthday, u.sex, u.address, u.contact_number,
                               u.is_archived,
                               sp.student_id, sp.program_id, 
                               CASE 
                                   WHEN p.program_name = 'BS Computer Science' THEN 'BSCS'
                                   WHEN p.program_name = 'BS Information Technology' THEN 'BSIT'
                                   WHEN p.program_name = 'BS Information System' THEN 'BSIS'
                                   WHEN p.program_name = 'BS Entertainment and Multimedia Computing' THEN 'BSEMC'
                                   ELSE p.program_name 
                               END AS program_name,
                               sp.year_level_id, sp.section_id, s.section_name,
                               sp.academic_status, sp.enrollment_date,
                               sp.school_year_id, sy.year_name AS school_year
                        FROM users u
                        LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                        LEFT JOIN programs p ON sp.program_id = p.program_id
                        LEFT JOIN sections s ON sp.section_id = s.section_id
                        LEFT JOIN school_years sy ON sp.school_year_id = sy.school_year_id
                        WHERE (u.first_name LIKE @SearchTerm 
                          OR u.middle_name LIKE @SearchTerm
                          OR u.last_name LIKE @SearchTerm 
                          OR u.email LIKE @SearchTerm
                          OR sp.student_id LIKE @SearchTerm
                          OR p.program_name LIKE @SearchTerm)
                        ORDER BY u.created_at DESC";

                    var adapter = new SqlDataAdapter();
                    var command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    adapter.SelectCommand = command;

                    var searchResults = new DataTable();
                    adapter.Fill(searchResults);

                    userDataTable = searchResults;
                    PopulateDataGrid(searchResults);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                FilterUsers(searchTerm);
            }
        }

        private void FilterUsers(string searchText)
        {
            if (userDataTable != null && !string.IsNullOrEmpty(searchText))
            {
                // Filter the data based on search text
                DataRow[] filteredRows = userDataTable.Select(
                   $"first_name LIKE '%{searchText}%' OR " +
                   $"middle_name LIKE '%{searchText}%' OR " +
                   $"last_name LIKE '%{searchText}%' OR " +
                   $"email LIKE '%{searchText}%' OR " +
                   $"student_id LIKE '%{searchText}%'"
                );

                // Create filtered table
                DataTable filteredTable = userDataTable.Clone();
                foreach (DataRow row in filteredRows)
                {
                    filteredTable.ImportRow(row);
                }

                // Update grid with filtered data
                PopulateDataGrid(filteredTable);
            }
            else
            {
                // If empty search, show all users
                PopulateDataGrid(userDataTable);
            }
        }

        private void LoadUsers()
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        SELECT u.user_id, u.email, u.password_hash, 
                               u.first_name, u.middle_name, u.last_name, u.user_type,
                               u.birthday, u.sex, u.address, u.contact_number,
                               sp.student_id, sp.program_id, 
                               CASE 
                                   WHEN p.program_name = 'BS Computer Science' THEN 'BSCS'
                                   WHEN p.program_name = 'BS Information Technology' THEN 'BSIT'
                                   WHEN p.program_name = 'BS Information System' THEN 'BSIS'
                                   WHEN p.program_name = 'BS Entertainment and Multimedia Computing' THEN 'BSEMC'
                                   ELSE p.program_name 
                               END AS program_name,
                               sp.year_level_id, sp.section_id, s.section_name,
                               sp.student_status, sp.academic_status, sp.enrollment_date,
                               sp.school_year_id, sy.year_name AS school_year,
                               u.is_archived
                        FROM users u
                        LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                        LEFT JOIN programs p ON sp.program_id = p.program_id
                        LEFT JOIN sections s ON sp.section_id = s.section_id
                        LEFT JOIN school_years sy ON sp.school_year_id = sy.school_year_id
                        ORDER BY u.created_at DESC";

                    var adapter = new Microsoft.Data.SqlClient.SqlDataAdapter(query, connection);
                    userDataTable = new DataTable();
                    adapter.Fill(userDataTable);

                    PopulateDataGrid(userDataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSchoolYearsComboBox()
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection())
                {
                    connection.Open();

                    string query = "SELECT year_name FROM school_years ORDER BY year_name DESC";
                    using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            SchoolYRCombobox.Items.Clear();
                            while (reader.Read())
                            {
                                SchoolYRCombobox.Items.Add(reader["year_name"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing school years: {ex.Message}");
            }
        }

        private void InitializeSchoolYearsTable(SqlConnection connection)
        {
            string createTableScript = @"
                CREATE TABLE school_years (
                    school_year_id INT IDENTITY(1,1) PRIMARY KEY,
                    year_name VARCHAR(5) NOT NULL UNIQUE,
                    is_current BIT DEFAULT 0 NOT NULL,
                    start_date DATE NULL,
                    end_date DATE NULL,
                    created_at DATETIME DEFAULT GETDATE(),
                    updated_at DATETIME NULL
                );
                
                -- Insert initial school years with YY-YY format
                INSERT INTO school_years (year_name, is_current, start_date, end_date) VALUES 
                ('23-24', 0, '2023-06-01', '2024-03-31'),
                ('24-25', 1, '2024-06-01', '2025-03-31'),
                ('25-26', 0, '2025-06-01', '2026-03-31');";

            using (var createCmd = new SqlCommand(createTableScript, connection))
            {
                createCmd.ExecuteNonQuery();
                Console.WriteLine("Created school_years table and inserted initial data");
            }
        }

        private void EnsureRequiredTablesExist(SqlConnection connection)
        {
            // Check if school_years table exists and create it if it doesn't
            string tableCheckQuery = "SELECT OBJECT_ID('school_years', 'U') AS TableExists";
            using (var cmd = new SqlCommand(tableCheckQuery, connection))
            {
                object tableExists = cmd.ExecuteScalar();

                if (tableExists == DBNull.Value || tableExists == null)
                {
                    // Create the school_years table
                    InitializeSchoolYearsTable(connection);
                }
            }

            // Check if student_profiles has the school_year_id column and add it if missing
            string columnCheckQuery = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'student_profiles' 
                AND COLUMN_NAME = 'school_year_id'";

            using (var cmd = new SqlCommand(columnCheckQuery, connection))
            {
                int columnExists = Convert.ToInt32(cmd.ExecuteScalar());

                if (columnExists == 0)
                {
                    // Add the school_year_id column to student_profiles
                    string alterTableQuery = @"
                        ALTER TABLE student_profiles
                        ADD school_year_id INT NULL;
                        
                        ALTER TABLE student_profiles
                        ADD CONSTRAINT FK_student_profiles_school_year
                        FOREIGN KEY (school_year_id) REFERENCES school_years(school_year_id);";

                    using (var alterCmd = new SqlCommand(alterTableQuery, connection))
                    {
                        alterCmd.ExecuteNonQuery();
                        Console.WriteLine("Added school_year_id column to student_profiles table");
                    }
                }
            }
        }

        private void PopulateDataGrid(DataTable dataTable)
        {
            // Clear existing rows
            UserDataGrid.Rows.Clear();

            // Populate with data from the data table
            foreach (DataRow row in dataTable.Rows)
            {
                int index = UserDataGrid.Rows.Add();
                DataGridViewRow gridRow = UserDataGrid.Rows[index];

                // Fill in the row data for standard columns
                gridRow.Cells["user_id"].Value = row["user_id"];

                // Concatenate first, middle, and last name for the fullname column
                string firstName = Convert.ToString(row["first_name"]);
                string middleName = Convert.ToString(row["middle_name"]);
                string lastName = Convert.ToString(row["last_name"]);

                // Format the name with middle initial if available
                string fullName;
                if (!string.IsNullOrWhiteSpace(middleName))  // Check middleName from row, not MnameTextBox.Text
                {
                    // Use middle initial with period
                    if (middleName.Length > 0)  // Add this to prevent index error for empty strings
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

                gridRow.Cells["fullname"].Value = fullName;

                // Fill in the rest of the columns in the requested order
                gridRow.Cells["student_id"].Value = row["student_id"];
                gridRow.Cells["email"].Value = row["email"];
                gridRow.Cells["user_type"].Value = row["user_type"];

                // Use the name fields from joined tables instead of IDs
                if (!Convert.IsDBNull(row["program_name"]))
                {
                    gridRow.Cells["program_id"].Value = row["program_name"];
                }

                // For section, we need to map section_id to proper section names
                if (!Convert.IsDBNull(row["section_id"]))
                {
                    int sectionId = Convert.ToInt32(row["section_id"]);
                    // Map section IDs to correct section names (Section A=1, Section B=2, etc.)
                    string sectionName = $"Section {(char)('A' + sectionId - 1)}";
                    gridRow.Cells["section_id"].Value = sectionName;
                }
                else if (!Convert.IsDBNull(row["section_name"]))
                {
                    gridRow.Cells["section_id"].Value = row["section_name"];
                }

                // For year level, we'll still use the ID since there's no year_levels table yet
                if (!Convert.IsDBNull(row["year_level_id"]))
                {
                    gridRow.Cells["year_level_id"].Value = $"Year {row["year_level_id"]}";
                }

                // Add the new columns to the grid
                if (!Convert.IsDBNull(row["sex"]))
                {
                    gridRow.Cells["sex"].Value = row["sex"];
                }

                if (!Convert.IsDBNull(row["contact_number"]))
                {
                    gridRow.Cells["contact_number"].Value = row["contact_number"];
                }

                if (!Convert.IsDBNull(row["school_year"]))
                {
                    gridRow.Cells["school_year"].Value = row["school_year"];
                }

                if (!Convert.IsDBNull(row["birthday"]))
                {
                    gridRow.Cells["birthday"].Value = Convert.ToDateTime(row["birthday"]).ToString("MM/dd/yyyy");
                }

                if (!Convert.IsDBNull(row["student_status"]))
                {
                    gridRow.Cells["student_status"].Value = row["student_status"];
                }

                // Set archived status as Yes/No
                bool isArchived = Convert.IsDBNull(row["is_archived"]) ? false : Convert.ToBoolean(row["is_archived"]);
                gridRow.Cells["is_archived"].Value = isArchived ? "Yes" : "No";

                // Apply styling to archived users (e.g., gray out the text)
                if (isArchived)
                {
                    gridRow.DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }


        private void UserDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected user ID
                currentUserId = Convert.ToInt32(UserDataGrid.Rows[e.RowIndex].Cells["user_id"].Value);
                isNewUser = false;

                // Find the user in the data table
                DataRow[] selectedRows = userDataTable.Select($"user_id = {currentUserId}");

                if (selectedRows.Length > 0)
                {
                    // Populate form fields with user data
                    FnameTextBox.Text = Convert.ToString(selectedRows[0]["first_name"]);

                    // Set middle name if available in data
                    if (!Convert.IsDBNull(selectedRows[0]["middle_name"]))
                    {
                        MnameTextBox.Text = Convert.ToString(selectedRows[0]["middle_name"]);
                    }
                    else
                    {
                        MnameTextBox.Text = "";
                    }

                    LnameTextBox.Text = Convert.ToString(selectedRows[0]["last_name"]);
                    EmailTextBox.Text = Convert.ToString(selectedRows[0]["email"]);
                    PasswordTextBox.Text = "********"; // Don't display actual password for security

                    // Populate new fields
                    if (!Convert.IsDBNull(selectedRows[0]["contact_number"]))
                    {
                        ContactNoTextBox.Text = Convert.ToString(selectedRows[0]["contact_number"]);
                    }
                    else
                    {
                        ContactNoTextBox.Text = "";
                    }

                    if (!Convert.IsDBNull(selectedRows[0]["birthday"]))
                    {
                        Bdaypicket.Value = Convert.ToDateTime(selectedRows[0]["birthday"]);
                    }
                    else
                    {
                        Bdaypicket.Value = DateTime.Today;
                    }

                    if (!Convert.IsDBNull(selectedRows[0]["sex"]))
                    {
                        string sex = Convert.ToString(selectedRows[0]["sex"]);
                        SexCombobox.SelectedItem = sex;
                    }
                    else
                    {
                        SexCombobox.SelectedIndex = -1;
                    }

                    // Set role
                    string userType = Convert.ToString(selectedRows[0]["user_type"]);
                    RoleComboBox.SelectedItem = string.IsNullOrEmpty(userType) ? "Student" : userType;

                    // Set student-specific fields if applicable
                    string studentId = Convert.ToString(selectedRows[0]["student_id"]);
                    TextBox1.Text = studentId;

                    if (!string.IsNullOrEmpty(studentId))
                    {
                        // Try to set program, year level and section from database values
                        object programId = selectedRows[0]["program_id"];
                        object yearLevelId = selectedRows[0]["year_level_id"];
                        object sectionId = selectedRows[0]["section_id"];

                        // Set student status
                        if (!Convert.IsDBNull(selectedRows[0]["student_status"]))
                        {
                            string studentStatus = Convert.ToString(selectedRows[0]["student_status"]);
                            StudentStatusCombobox.SelectedItem = CapitalizeFirstLetter(studentStatus);
                            currentStudentStatus = studentStatus;
                        }
                        else
                        {
                            StudentStatusCombobox.SelectedItem = "Regular";
                            currentStudentStatus = "regular";
                        }

                        // Set academic status
                        if (!Convert.IsDBNull(selectedRows[0]["academic_status"]))
                        {
                            string academicStatus = Convert.ToString(selectedRows[0]["academic_status"]);
                            ComboBox4.SelectedItem = CapitalizeFirstLetter(academicStatus);
                            currentAcademicStatus = academicStatus;
                        }
                        else
                        {
                            ComboBox4.SelectedItem = "Active";
                            currentAcademicStatus = "active";
                        }

                        // Set school year
                        if (!Convert.IsDBNull(selectedRows[0]["school_year_id"]))
                        {
                            schoolYearId = Convert.ToInt32(selectedRows[0]["school_year_id"]);

                            // Get the year_name for the ComboBox
                            if (!Convert.IsDBNull(selectedRows[0]["school_year"]))
                            {
                                SchoolYRCombobox.Text = Convert.ToString(selectedRows[0]["school_year"]);
                            }
                            else
                            {
                                SchoolYRCombobox.SelectedIndex = -1;
                            }
                        }
                        else
                        {
                            schoolYearId = -1;
                            SchoolYRCombobox.SelectedIndex = -1;
                        }

                        // Set values based on IDs (check for DBNull)
                        if (!Convert.IsDBNull(programId) && !Convert.IsDBNull(yearLevelId) && !Convert.IsDBNull(sectionId))
                        {
                            SetProgramYearSection(
                                Convert.ToInt32(programId),
                                Convert.ToInt32(yearLevelId),
                                Convert.ToInt32(sectionId)
                            );
                        }
                    }
                }
            }
        }

        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private void SetProgramYearSection(int programId, int yearLevelId, int sectionId)
        {
            // Map program ID to CourseCombobox index (0-based)
            // Assuming program IDs are 1-based and match the order in the combobox
            if (programId > 0 && programId <= CourseCombobox.Items.Count)
            {
                CourseCombobox.SelectedIndex = programId - 1;
            }

            // Map year level ID to YearCombobox index
            // Assuming year level IDs are 1-based (1=year 1, 2=year 2, etc.)
            if (yearLevelId > 0 && yearLevelId <= YearCombobox.Items.Count)
            {
                YearCombobox.SelectedIndex = yearLevelId - 1;
            }

            // Map section ID to SectionCombobox index
            // Assuming section IDs are 1-based (1=A, 2=B, 3=C, etc.)
            if (sectionId > 0 && sectionId <= SectionCombobox.Items.Count)
            {
                SectionCombobox.SelectedIndex = sectionId - 1;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Check if there's a user selected
            if (currentUserId <= 0)
            {
                MessageBox.Show("Please select a user to archive first.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm with the user before archiving
            string middleInitial = string.IsNullOrWhiteSpace(MnameTextBox.Text) ? "" : $" {MnameTextBox.Text[0]}.";
            string selectedName = $"{FnameTextBox.Text}{middleInitial} {LnameTextBox.Text}";

            string studentIdText = string.IsNullOrEmpty(TextBox1.Text) ? "" : $" (ID: {TextBox1.Text})";

            string confirmMessage = $"Are you sure you want to archive user {selectedName}{studentIdText}?" +
                                     Environment.NewLine + Environment.NewLine +
                                     "This will hide the user from the system, but the data will be preserved." +
                                     Environment.NewLine + Environment.NewLine +
                                     "Archived users can be restored by an administrator if needed.";

            DialogResult result = MessageBox.Show(confirmMessage, "Confirm Archive",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            // Archive the user
            try
            {
                ArchiveUser(currentUserId);
                MessageBox.Show($"User {selectedName} has been successfully archived.",
                          "Archive Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload data to reflect changes
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error archiving user: {ex.Message}", "Database Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ArchiveUser(int userId)
        {
            using (var connection = DatabaseConn.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var archiveCommand = new Microsoft.Data.SqlClient.SqlCommand(@"
                            UPDATE users
                            SET is_archived = 1, updated_at = GETDATE()
                            WHERE user_id = @user_id", connection, transaction);

                        archiveCommand.Parameters.AddWithValue("@user_id", userId);
                        archiveCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /*private void SaveButton_Click(object sender, EventArgs e)
        {
            // Check database connection first
            if (!isDbConnected && !CheckDatabaseConnection())
            {
                MessageBox.Show("Cannot save changes: Database connection failed.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate form
            if (!ValidateForm()) return;

            // Implementation continues with save logic...
            // This would include the rest of the save functionality from the VB.NET code
        }

        /*private bool ValidateForm()
        {
            // Add form validation logic here
            // Return true if valid, false otherwise
            return true; // Placeholder
        }*/

        private void ClearForm()
        {
            // Clear all form fields
            FnameTextBox.Text = "";
            MnameTextBox.Text = "";
            LnameTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordTextBox.Text = "";
            ContactNoTextBox.Text = "";
            TextBox1.Text = "";

            // Reset ComboBoxes
            RoleComboBox.SelectedIndex = -1;
            CourseCombobox.SelectedIndex = -1;
            YearCombobox.SelectedIndex = -1;
            SectionCombobox.SelectedIndex = -1;
            SchoolYRCombobox.SelectedIndex = -1;
            SexCombobox.SelectedIndex = -1;
            StudentStatusCombobox.SelectedIndex = -1;
            ComboBox4.SelectedIndex = -1;

            // Reset other controls
            Bdaypicket.Value = DateTime.Today;

            // Reset state variables
            isNewUser = true;
            currentUserId = -1;
            schoolYearId = -1;
            currentStudentStatus = "Regular";
            currentAcademicStatus = "Active";
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Check database connection first
            if (!isDbConnected && !CheckDatabaseConnection())
            {
                MessageBox.Show("Cannot save changes: Database connection failed.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate form
            if (!ValidateForm()) return;

            // Check if the user already exists based on email, student ID, or full name
            int existingUserId = FindExistingUser();

            // If user exists and we're in create mode, update the user record instead
            if (existingUserId > -1 && isNewUser)
            {
                // Ask if the user wants to update the existing record
                string existingUserInfo = GetExistingUserInfo(existingUserId);
                string message = $"A user with matching information already exists:{Environment.NewLine}{Environment.NewLine}{existingUserInfo}{Environment.NewLine}{Environment.NewLine}Do you want to update the existing record instead?";

                DialogResult result = MessageBox.Show(message, "User Already Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Switch to update mode
                    currentUserId = existingUserId;
                    isNewUser = false;

                    // Load the existing user data to ensure we have the full record
                    LoadUserData(currentUserId);

                    // Call SaveButton_Click again now that we're in update mode
                    SaveButton_Click(sender, e);
                    return;
                }
                else
                {
                    return; // User chose not to update
                }
            }

            try
            {
                if (isNewUser)
                {
                    CreateNewUser();
                }
                else
                {
                    UpdateExistingUser();
                }

                MessageBox.Show("User information saved successfully!", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers(); // Refresh the grid
                ClearForm(); // Clear form for next entry
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FnameTextBox.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FnameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(LnameTextBox.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LnameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                EmailTextBox.Focus();
                return false;
            }

            // Validate email format
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                EmailTextBox.Focus();
                return false;
            }

            if (RoleComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RoleComboBox.Focus();
                return false;
            }

            // Validate password for new users
            if (isNewUser && string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Password is required for new users.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PasswordTextBox.Focus();
                return false;
            }

            // Validate student-specific fields
            if (RoleComboBox.SelectedItem.ToString() == "Student")
            {
                if (string.IsNullOrWhiteSpace(TextBox1.Text))
                {
                    MessageBox.Show("Student ID is required for students.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TextBox1.Focus();
                    return false;
                }

                if (CourseCombobox.SelectedIndex < 0)
                {
                    MessageBox.Show("Course is required for students.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    CourseCombobox.Focus();
                    return false;
                }

                if (YearCombobox.SelectedIndex < 0)
                {
                    MessageBox.Show("Year level is required for students.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    YearCombobox.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private int FindExistingUser()
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        SELECT TOP 1 u.user_id 
                        FROM users u
                        LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                        WHERE u.email = @email 
                        OR (sp.student_id = @studentId AND @studentId IS NOT NULL AND @studentId != '')
                        OR (u.first_name = @firstName AND u.last_name = @lastName)";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@email", EmailTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@studentId", string.IsNullOrWhiteSpace(TextBox1.Text) ? (object)DBNull.Value : TextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@firstName", FnameTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@lastName", LnameTextBox.Text.Trim());

                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding existing user: {ex.Message}");
                return -1;
            }
        }

        private string GetExistingUserInfo(int userId)
        {
            try
            {
                using (var connection = DatabaseConn.GetConnection())
                {
                    connection.Open();

                    string query = @"
                        SELECT u.first_name, u.middle_name, u.last_name, u.email, sp.student_id
                        FROM users u
                        LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                        WHERE u.user_id = @userId";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["first_name"].ToString();
                                string middleName = reader["middle_name"].ToString();
                                string lastName = reader["last_name"].ToString();
                                string email = reader["email"].ToString();
                                string studentId = reader["student_id"].ToString();

                                string fullName = string.IsNullOrWhiteSpace(middleName)
                                    ? $"{firstName} {lastName}"
                                    : $"{firstName} {middleName[0]}. {lastName}";

                                return $"Name: {fullName}{Environment.NewLine}Email: {email}" +
                                       (string.IsNullOrWhiteSpace(studentId) ? "" : $"{Environment.NewLine}Student ID: {studentId}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting existing user info: {ex.Message}");
            }

            return "Unknown user";
        }

        private void LoadUserData(int userId)
        {
            // Find and select the user in the grid, which will populate the form
            foreach (DataGridViewRow row in UserDataGrid.Rows)
            {
                if (Convert.ToInt32(row.Cells["user_id"].Value) == userId)
                {
                    UserDataGrid.ClearSelection();
                    row.Selected = true;
                    UserDataGrid_CellClick(UserDataGrid, new DataGridViewCellEventArgs(0, row.Index));
                    break;
                }
            }
        }

        private void CreateNewUser()
        {
            using (var connection = DatabaseConn.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into users table
                        string userQuery = @"
                            INSERT INTO users (email, password_hash, first_name, middle_name, last_name, user_type, 
                                             birthday, sex, contact_number, created_at)
                            VALUES (@email, @password_hash, @first_name, @middle_name, @last_name, @user_type,
                                   @birthday, @sex, @contact_number, GETDATE());
                            SELECT SCOPE_IDENTITY();";

                        int newUserId;
                        using (var cmd = new SqlCommand(userQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@email", EmailTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@password_hash", PasswordHelper.HashPassword(PasswordTextBox.Text));
                            cmd.Parameters.AddWithValue("@first_name", FnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@middle_name", string.IsNullOrWhiteSpace(MnameTextBox.Text) ? (object)DBNull.Value : MnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@last_name", LnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@user_type", RoleComboBox.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@birthday", Bdaypicket.Value.Date);
                            cmd.Parameters.AddWithValue("@sex", SexCombobox.SelectedIndex >= 0 ? SexCombobox.SelectedItem.ToString() : (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@contact_number", string.IsNullOrWhiteSpace(ContactNoTextBox.Text) ? (object)DBNull.Value : ContactNoTextBox.Text.Trim());

                            newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // If it's a student, insert into student_profiles
                        if (RoleComboBox.SelectedItem.ToString() == "Student")
                        {
                            CreateStudentProfile(newUserId, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void UpdateExistingUser()
        {
            using (var connection = DatabaseConn.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update users table
                        string userQuery = @"
                            UPDATE users 
                            SET email = @email, first_name = @first_name, middle_name = @middle_name, 
                                last_name = @last_name, user_type = @user_type, birthday = @birthday,
                                sex = @sex, contact_number = @contact_number, updated_at = GETDATE()";

                        // Only update password if it's not the placeholder
                        if (PasswordTextBox.Text != "********")
                        {
                            userQuery += ", password_hash = @password_hash";
                        }

                        userQuery += " WHERE user_id = @user_id";

                        using (var cmd = new SqlCommand(userQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@user_id", currentUserId);
                            cmd.Parameters.AddWithValue("@email", EmailTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@first_name", FnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@middle_name", string.IsNullOrWhiteSpace(MnameTextBox.Text) ? (object)DBNull.Value : MnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@last_name", LnameTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("@user_type", RoleComboBox.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@birthday", Bdaypicket.Value.Date);
                            cmd.Parameters.AddWithValue("@sex", SexCombobox.SelectedIndex >= 0 ? SexCombobox.SelectedItem.ToString() : (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@contact_number", string.IsNullOrWhiteSpace(ContactNoTextBox.Text) ? (object)DBNull.Value : ContactNoTextBox.Text.Trim());

                            if (PasswordTextBox.Text != "********")
                            {
                                cmd.Parameters.AddWithValue("@password_hash", PasswordHelper.HashPassword(PasswordTextBox.Text));
                            }

                            cmd.ExecuteNonQuery();
                        }

                        // Handle student profile
                        if (RoleComboBox.SelectedItem.ToString() == "Student")
                        {
                            UpdateOrCreateStudentProfile(currentUserId, connection, transaction);
                        }
                        else
                        {
                            // Remove student profile if user is no longer a student
                            RemoveStudentProfile(currentUserId, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void CreateStudentProfile(int userId, SqlConnection connection, SqlTransaction transaction)
        {
            string studentQuery = @"
                INSERT INTO student_profiles (user_id, student_id, program_id, year_level_id, section_id,
                                            school_year_id, student_status, academic_status, enrollment_date)
                VALUES (@user_id, @student_id, @program_id, @year_level_id, @section_id,
                       @school_year_id, @student_status, @academic_status, GETDATE())";

            using (var cmd = new SqlCommand(studentQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.Parameters.AddWithValue("@student_id", TextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@program_id", CourseCombobox.SelectedIndex + 1);
                cmd.Parameters.AddWithValue("@year_level_id", YearCombobox.SelectedIndex + 1);
                cmd.Parameters.AddWithValue("@section_id", SectionCombobox.SelectedIndex >= 0 ? SectionCombobox.SelectedIndex + 1 : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@school_year_id", schoolYearId > 0 ? schoolYearId : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@student_status", currentStudentStatus.ToLower());
                cmd.Parameters.AddWithValue("@academic_status", currentAcademicStatus.ToLower());

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateOrCreateStudentProfile(int userId, SqlConnection connection, SqlTransaction transaction)
        {
            // Check if student profile exists
            string checkQuery = "SELECT COUNT(*) FROM student_profiles WHERE user_id = @user_id";
            using (var checkCmd = new SqlCommand(checkQuery, connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@user_id", userId);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    // Update existing profile
                    string updateQuery = @"
                        UPDATE student_profiles 
                        SET student_id = @student_id, program_id = @program_id, year_level_id = @year_level_id,
                            section_id = @section_id, school_year_id = @school_year_id, 
                            student_status = @student_status, academic_status = @academic_status
                        WHERE user_id = @user_id";

                    using (var cmd = new SqlCommand(updateQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@student_id", TextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@program_id", CourseCombobox.SelectedIndex + 1);
                        cmd.Parameters.AddWithValue("@year_level_id", YearCombobox.SelectedIndex + 1);
                        cmd.Parameters.AddWithValue("@section_id", SectionCombobox.SelectedIndex >= 0 ? SectionCombobox.SelectedIndex + 1 : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@school_year_id", schoolYearId > 0 ? schoolYearId : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@student_status", currentStudentStatus.ToLower());
                        cmd.Parameters.AddWithValue("@academic_status", currentAcademicStatus.ToLower());

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Create new profile
                    CreateStudentProfile(userId, connection, transaction);
                }
            }
        }

        private void RemoveStudentProfile(int userId, SqlConnection connection, SqlTransaction transaction)
        {
            string deleteQuery = "DELETE FROM student_profiles WHERE user_id = @user_id";
            using (var cmd = new SqlCommand(deleteQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void SetEditMode(bool enabled)
        {
            FnameTextBox.ReadOnly = !enabled;
            MnameTextBox.ReadOnly = !enabled;
            LnameTextBox.ReadOnly = !enabled;
            EmailTextBox.ReadOnly = !enabled;
            PasswordTextBox.ReadOnly = !enabled;
            ContactNoTextBox.ReadOnly = !enabled;
            TextBox1.ReadOnly = !enabled; // Student ID

            Bdaypicket.Enabled = enabled;
            RoleComboBox.Enabled = enabled;
            CourseCombobox.Enabled = enabled;
            YearCombobox.Enabled = enabled;
            SectionCombobox.Enabled = enabled;
            SchoolYRCombobox.Enabled = enabled;
            SexCombobox.Enabled = enabled;
            StudentStatusCombobox.Enabled = enabled;
            ComboBox4.Enabled = enabled;

            SaveButton.Enabled = enabled;
            SaveButton.Visible = enabled;
        }

        // Add placeholder methods for events that need implementation
        private void EmailTextBox_Leave(object sender, EventArgs e)
        {
            // Email validation logic
        }

        private void NameTextBox_Leave(object sender, EventArgs e)
        {
            // Name validation logic
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void PassEyeButton_Click(object sender, EventArgs e)
        {
            // Toggle password visibility
            PasswordTextBox.UseSystemPasswordChar = !PasswordTextBox.UseSystemPasswordChar;
            PassEyeButton.Text = PasswordTextBox.UseSystemPasswordChar ? "S" : "H";
        }

        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            SetEditMode(true);
            Editbtn.Enabled = false; // Optionally disable the edit button while editing
        }

        private void FnameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MnameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void LnameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}