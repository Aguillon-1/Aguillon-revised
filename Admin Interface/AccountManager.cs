using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

public partial class AccountManager
{
    // Database connection string - updated to use |DataDirectory| for a relative path
    private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True";

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
        base.Load += AccountManager_Load;
    }

    private void AccountManager_Load(object sender, EventArgs e)
    {
        // Check database connection first
        if (!CheckDatabaseConnection())
        {
            MessageBox.Show("Failed to connect to the database. Some features may not work correctly.", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // Continue with UI initialization but mark connection as failed
            isDbConnected = false;
        }
        else
        {
            isDbConnected = true;

            // Initialize the school years combo box (add this line)
            InitializeSchoolYearsComboBox();

            // Ensure required tables and columns exist (call this after establishing connection is possible)
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    EnsureRequiredTablesExist(connection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to ensure database schema: {ex.Message}", "Database Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Potentially handle this more gracefully, e.g., disable parts of the UI
                }
            }
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
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // If we get here, connection is successful
                return true;
            }
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Database connection error: {ex.Message}");
            return false;
        }
    }

    private void InitializeColumnMappings()
    {
        // Create dictionary of column name to display name mappings
        columnMappings = new Dictionary<string, string>() { { "user_id", "ID" }, { "fullname", "Name" }, { "student_id", "Student ID" }, { "year_level_id", "Year Level" }, { "program_id", "Course" }, { "section_id", "Section" }, { "sex", "Sex" }, { "email", "Email" }, { "contact_number", "Contact No." }, { "school_year", "School Year" }, { "user_type", "Role" }, { "student_status", "Student Status" }, { "birthday", "Birthday" }, { "is_archived", "Archived" } };            // New column for concatenated name
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added sex after section
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added contact number after email
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added school year before role
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added student status
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added birthday at the far right
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Added archived status
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Removed "username" from the mappings
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             // Removed "first_name" and "last_name" as individual columns
    }





    private void SetupDbsortCheckedListBox()
    {
        // Clear any existing items
        DbsortCheckedListBox.Items.Clear();

        // Add items based on column mappings (excluding user_id and is_archived which is handled differently)
        foreach (KeyValuePair<string, string> kvp in columnMappings)
        {
            if (kvp.Key != "user_id" && kvp.Key != "is_archived")
            {
                // Add display name to the CheckedListBox
                int index = DbsortCheckedListBox.Items.Add(kvp.Value);

                // Default columns to checked (visible) except contact_number and birthday
                bool shouldCheck = kvp.Key != "contact_number" && kvp.Key != "birthday";
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
        if (userDataTable is null)
            return;

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
            filteredTable.ImportRow(row);

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
            bool showArchived = e.NewValue == CheckState.Checked;

            // Set archived column visibility
            if (UserDataGrid.Columns.Contains("is_archived"))
            {
                UserDataGrid.Columns["is_archived"].Visible = showArchived;
            }

            // Filter data on the next UI update to avoid modification during enumeration
            BeginInvoke(() => FilterArchivedUsers(showArchived));
            return;
        }

        // For regular columns, find the column name from the display name
        string columnName = "";
        foreach (KeyValuePair<string, string> kvp in columnMappings)
        {
            if ((kvp.Value ?? "") == (columnDisplayName ?? ""))
            {
                columnName = kvp.Key;
                break;
            }
        }

        // If we found the column name, toggle its visibility
        if (!string.IsNullOrEmpty(columnName) && UserDataGrid.Columns.Contains(columnName))
        {
            // Set visibility based on the new check state
            UserDataGrid.Columns[columnName].Visible = e.NewValue == CheckState.Checked;
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
        // If you have any other ComboBoxes, set them here
    }


    private void ConfigureDataGrid()
    {
        // Ensure UserDataGrid is not null
        if (UserDataGrid is null)
        {
            MessageBox.Show("UserDataGrid is not initialized. Please check the Designer file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        string[] columnKeys = new[] { "user_id", "fullname", "student_id", "year_level_id", "program_id", "section_id", "sex", "email", "contact_number", "school_year", "user_type", "student_status", "birthday", "is_archived" };
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
                    column.DataPropertyName = (object)null; // We'll set this value manually
                }

                // Special handling for is_archived to show as Yes/No
                if (key == "is_archived")
                {
                    column.DataPropertyName = (object)null; // We'll set this value manually
                }

                UserDataGrid.Columns.Add(column);
            }
            else
            {
                MessageBox.Show($"Column key '{key}' is missing in columnMappings. Please check the InitializeColumnMappings method.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            ((DataGridViewTextBoxColumn)column).DefaultCellStyle.WrapMode = DataGridViewTriState.False;

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
        UserDataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnMode.ColumnHeader);

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


    // try
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
            if (!System.Text.RegularExpressions.Regex.IsMatch(ContactNoTextBox.Text, @"^(09|\+639)\d{9}$"))
            {
                MessageBox.Show("Please enter a valid phone number format (09XXXXXXXXX or +639XXXXXXXXX)", "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (!System.Text.RegularExpressions.Regex.IsMatch(TextBox1.Text, @"^\d{8}-[A-Z]$"))
            {
                MessageBox.Show("Student ID must be in the format: 8 digits followed by a dash and a letter (e.g., 20230902-N)", "Invalid Student ID Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        // Get the school year ID from the database
        if (SchoolYRCombobox.SelectedIndex >= 0)
        {
            string selectedSY = SchoolYRCombobox.SelectedItem.ToString();

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT school_year_id FROM school_years WHERE year_name = @yearName";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@yearName", selectedSY);
                        var result = cmd.ExecuteScalar();

                        if (result is not null)
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
                    MessageBox.Show($"Error retrieving school year ID: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        // Don't search if empty or connection failed
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            LoadUsers();
            return;
        }

        // Don't search if not connected to DB
        if (!isDbConnected && !CheckDatabaseConnection())
        {
            return;
        }

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Search query - searches directly in the database - removed archived filter
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

            ORDER BY u.created_at DESC
        ";

                var adapter = new SqlDataAdapter();
                var command = new SqlCommand(query, connection);

                // Using parameterized query for security
                command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                adapter.SelectCommand = command;

                var searchResults = new DataTable();
                adapter.Fill(searchResults);

                // Update the user data table and populate grid
                userDataTable = searchResults;
                PopulateDataGrid(searchResults);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
            MessageBox.Show($"Search error: {ex.Message}", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            FilterUsers(searchTerm);
        }
    }





    private void FilterUsers(string searchText)
    {
        if (userDataTable is not null && !string.IsNullOrEmpty(searchText))
        {
            // Filter the data based on search text
            DataRow[] filteredRows = userDataTable.Select($"first_name LIKE '%{searchText}%' OR " + $"middle_name LIKE '%{searchText}%' OR " + $"last_name LIKE '%{searchText}%' OR " + $"email LIKE '%{searchText}%' OR " + $"student_id LIKE '%{searchText}%'");

            // Create filtered table
            DataTable filteredTable = userDataTable.Clone();
            foreach (DataRow row in filteredRows)
                filteredTable.ImportRow(row);

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
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Load all users (including archived ones)
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
            ORDER BY u.created_at DESC
            ";

                var adapter = new SqlDataAdapter(query, connection);
                userDataTable = new DataTable();
                adapter.Fill(userDataTable);

                // Check if the "Archived Users" checkbox is checked
                bool showArchived = false;
                for (int i = 0, loopTo = DbsortCheckedListBox.Items.Count - 1; i <= loopTo; i++)
                {
                    if (DbsortCheckedListBox.Items[i].ToString() == "Archived Users")
                    {
                        showArchived = DbsortCheckedListBox.GetItemChecked(i);
                        break;
                    }
                }

                // Filter the data if needed
                if (!showArchived)
                {
                    FilterArchivedUsers(false);
                }
                else
                {
                    // Populate the grid with all data including archived users
                    PopulateDataGrid(userDataTable);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading user data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        ('25-26', 0, '2025-06-01', '2026-03-31');
    ";

        using (var createCmd = new SqlCommand(createTableScript, connection))
        {
            createCmd.ExecuteNonQuery();
            // Console.WriteLine("Created school_years table and inserted initial data"); // Logging to console, consider MessageBox or proper logging for UI app
            MessageBox.Show("Created school_years table and inserted initial data.", "Database Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void EnsureRequiredTablesExist(SqlConnection connection)
    {
        // Check if school_years table exists and create it if it doesn't
        string tableCheckQuery = "SELECT OBJECT_ID('school_years', 'U') AS TableExists";
        using (var cmd = new SqlCommand(tableCheckQuery, connection))
        {
            var tableExists = cmd.ExecuteScalar();

            if (ReferenceEquals(tableExists, DBNull.Value) || tableExists is null)
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
                    // Console.WriteLine("Added school_year_id column to student_profiles table");
                    MessageBox.Show("Added school_year_id column to student_profiles table.", "Database Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string middleName = Convert.ToString(row["middle_name"));
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

            if (!Convert.IsDBNull(row["section_name"]))
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
                        SetProgramYearSection(Convert.ToInt32(programId), Convert.ToInt32(yearLevelId), Convert.ToInt32(sectionId));
                    }
                }
            }
        }
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
        // In the DeleteButton_Click method
        string middleInitial = string.IsNullOrWhiteSpace(MnameTextBox.Text) ? "" : (MnameTextBox.Text.Length > 0 ? $" {MnameTextBox.Text[0]}." : "");
        string selectedName = $"{FnameTextBox.Text}{middleInitial} {LnameTextBox.Text}";

        string studentIdText = string.IsNullOrEmpty(TextBox1.Text) ? "" : $" (ID: {TextBox1.Text})";

        string confirmMessage = $"Are you sure you want to archive user {selectedName}{studentIdText}?" + Environment.NewLine + Environment.NewLine + "This will hide the user from the system, but the data will be preserved." + Environment.NewLine + Environment.NewLine + "Archived users can be restored by an administrator if needed.";

        DialogResult result = MessageBox.Show(confirmMessage, "Confirm Archive", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.No)
        {
            return;
        }

        // Archive the user
        try
        {
            ArchiveUser(currentUserId);
            MessageBox.Show($"User {selectedName} has been successfully archived.", "Archive Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reload data to reflect changes
            LoadUsers();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error archiving user: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ArchiveUser(int userId)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Mark the user as archived
                    var archiveCommand = new SqlCommand(@"
                    UPDATE users
                    SET is_archived = 1, updated_at = GETDATE()
                    WHERE user_id = @user_id
                ", connection, transaction);

                    archiveCommand.Parameters.AddWithValue("@user_id", userId);
                    archiveCommand.ExecuteNonQuery();

                    // Commit the transaction
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


    private void SaveButton_Click(object sender, EventArgs e)
    {
        // Check database connection first
        if (!isDbConnected && !CheckDatabaseConnection())
        {
            MessageBox.Show("Cannot save changes: Database connection failed.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Validate form
        if (!ValidateForm())
            return;

        // Check if the user already exists based on email, student ID, or full name
        int existingUserId = FindExistingUser();

        // If user exists and we're in create mode, update the user record instead
        if (existingUserId > -1 && isNewUser)
        {
            // Ask if the user wants to update the existing record
            string existingUserInfo = GetExistingUserInfo(existingUserId);
            string message = $"A user with matching information already exists:{Environment.NewLine}{Environment.NewLine}{existingUserInfo}{Environment.NewLine}{Environment.NewLine}Do you want to update this record instead?";

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
                // User chose not to update, so exit
                return;
            }
        }

        // For new users, show the full information
        if (isNewUser)
        {
            // Build formatted user information
            var newUserInfo = new StringBuilder();

            // Include middle name if provided for display
            string fullNameDisplay;
            if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
            {
                fullNameDisplay = $"{FnameTextBox.Text} {LnameTextBox.Text}";
            }
            else
            {
                fullNameDisplay = $"{FnameTextBox.Text} {MnameTextBox.Text} {LnameTextBox.Text}";
            }

            newUserInfo.AppendLine($"Name: {fullNameDisplay}");
            newUserInfo.AppendLine($"Email: {EmailTextBox.Text}");

            // Add contact number if provided, otherwise show dash
            string contactDisplay = string.IsNullOrWhiteSpace(ContactNoTextBox.Text) ? "-" : ContactNoTextBox.Text;
            newUserInfo.AppendLine($"Contact No.: {contactDisplay}");

            // Add sex if selected, otherwise show dash
            string sexDisplay = SexCombobox.SelectedIndex >= 0 ? SexCombobox.SelectedItem.ToString() : "-";
            newUserInfo.AppendLine($"Sex: {sexDisplay}");

            // Add birthday if not today, otherwise show dash
            string birthdayDisplay = Bdaypicket.Value == DateTime.Today ? "-" : Bdaypicket.Value.ToString("yyyy-MM-dd");
            newUserInfo.AppendLine($"Birthday: {birthdayDisplay}");

            newUserInfo.AppendLine($"Role: {RoleComboBox.SelectedItem.ToString()}");

            if (RoleComboBox.SelectedItem.ToString() == "Student")
            {
                newUserInfo.AppendLine($"Student ID: {TextBox1.Text}");

                // Add student status with default if not selected
                string studentStatusDisplay = StudentStatusCombobox.SelectedIndex >= 0 ? StudentStatusCombobox.SelectedItem.ToString() : "Regular";
                newUserInfo.AppendLine($"Student Status: {studentStatusDisplay}");

                // Add academic status with default if not selected
                string academicStatusDisplay = ComboBox4.SelectedIndex >= 0 ? ComboBox4.SelectedItem.ToString() : "Active";
                newUserInfo.AppendLine($"Academic Status: {academicStatusDisplay}");

                // Program with dash if not selected
                string programDisplay = "-";
                if (CourseCombobox.SelectedIndex >= 0 && CourseCombobox.SelectedIndex < CourseCombobox.Items.Count)
                {
                    programDisplay = CourseCombobox.Items[CourseCombobox.SelectedIndex].ToString();
                }
                newUserInfo.AppendLine($"Program: {programDisplay}");

                // Year with dash if not selected
                string yearDisplay = "-";
                if (YearCombobox.SelectedIndex >= 0 && YearCombobox.SelectedIndex < YearCombobox.Items.Count)
                {
                    yearDisplay = YearCombobox.Items[YearCombobox.SelectedIndex].ToString();
                }
                newUserInfo.AppendLine($"Year: {yearDisplay}");

                // Section with dash if not selected
                string sectionDisplay = "-";
                if (SectionCombobox.SelectedIndex >= 0 && SectionCombobox.SelectedIndex < SectionCombobox.Items.Count)
                {
                    sectionDisplay = SectionCombobox.Items[SectionCombobox.SelectedIndex].ToString();
                }
                newUserInfo.AppendLine($"Section: {sectionDisplay}");

                // School year with dash if not selected
                string schoolYearDisplay = "-";
                if (SchoolYRCombobox.SelectedIndex >= 0)
                {
                    schoolYearDisplay = SchoolYRCombobox.SelectedItem.ToString();
                }
                newUserInfo.AppendLine($"School Year: {schoolYearDisplay}");
            }

            // Ask for confirmation before saving - personalized message
            string confirmationMessage = $"Are you sure you want to create a new account for {fullNameDisplay}?" + Environment.NewLine + Environment.NewLine + "The following information will be saved:" + Environment.NewLine + Environment.NewLine + newUserInfo.ToString();

            DialogResult result = MessageBox.Show(confirmationMessage, "Confirm Create Account", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                return;

            try
            {
                CreateNewUser();

                // Personalized success message
                string successMessage = $"User {fullNameDisplay} ({RoleComboBox.SelectedItem}) has been successfully created.";
                MessageBox.Show(successMessage, "Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload data to reflect changes
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            // For updates, compare old and new values
            try
            {
                // Fetch the original user info from the data table
                DataRow[] selectedRows = userDataTable.Select($"user_id = {currentUserId}");
                if (selectedRows.Length > 0)
                {
                    // Rest of the update logic remains the same...
                    // Get name components for display
                    string oldFirstName = Convert.ToString(selectedRows[0]["first_name"]);
                    string oldLastName = Convert.ToString(selectedRows[0]["last_name"]);
                    string oldMiddleName = "";

                    if (!Convert.IsDBNull(selectedRows[0]["middle_name"]))
                    {
                        oldMiddleName = Convert.ToString(selectedRows[0]["middle_name"]);
                    }

                    // Format full names for display
                    string oldFullName;
                    if (string.IsNullOrWhiteSpace(oldMiddleName))
                    {
                        oldFullName = $"{oldFirstName} {oldLastName}";
                    }
                    else
                    {
                        oldFullName = $"{oldFirstName} {oldMiddleName} {oldLastName}";
                    }

                    string newFullName;
                    if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
                    {
                        newFullName = $"{FnameTextBox.Text} {LnameTextBox.Text}";
                    }
                    else
                    {
                        newFullName = $"{FnameTextBox.Text} {MnameTextBox.Text} {LnameTextBox.Text}";
                    }

                    // Get role for display
                    string oldRole = Convert.ToString(selectedRows[0]["user_type"]);
                    string newRole = RoleComboBox.SelectedItem.ToString();

                    // Compare old values with new values to detect changes
                    var changes = new StringBuilder();

                    // Add a personalized confirmation header
                    changes.AppendLine($"Are you sure you want to update information for {oldFullName} ({oldRole})?");
                    changes.AppendLine("");
                    changes.AppendLine("Changes you are about to make:");
                    changes.AppendLine("");

                    // Track if any changes were made
                    bool hasChanges = false;

                    // Name changes (first, middle, last)
                    if (oldFirstName != FnameTextBox.Text)
                    {
                        changes.AppendLine($"First name: {oldFirstName} > {FnameTextBox.Text}");
                        hasChanges = true;
                    }

                    if (oldMiddleName != MnameTextBox.Text)
                    {
                        // Display dash for empty values
                        string oldDisplay = string.IsNullOrEmpty(oldMiddleName) ? "-" : oldMiddleName;
                        string newDisplay = string.IsNullOrEmpty(MnameTextBox.Text) ? "-" : MnameTextBox.Text;
                        changes.AppendLine($"Middle name: {oldDisplay} > {newDisplay}");
                        hasChanges = true;
                    }

                    if (oldLastName != LnameTextBox.Text)
                    {
                        changes.AppendLine($"Last name: {oldLastName} > {LnameTextBox.Text}");
                        hasChanges = true;
                    }

                    // Email change
                    string oldEmail = Convert.ToString(selectedRows[0]["email"]);
                    if (oldEmail != EmailTextBox.Text)
                    {
                        changes.AppendLine($"Email: {oldEmail} > {EmailTextBox.Text}");
                        hasChanges = true;
                    }

                    // Password change
                    if (PasswordTextBox.Text != "********")
                    {
                        changes.AppendLine("Password: ******** > [New Password]");
                        hasChanges = true;
                    }

                    // Role change
                    if ((oldRole ?? "") != (newRole ?? ""))
                    {
                        changes.AppendLine($"Role: {oldRole} > {newRole}");
                        hasChanges = true;
                    }

                    // Contact Number change
                    string oldContactNo = "";
                    if (!Convert.IsDBNull(selectedRows[0]["contact_number"]))
                    {
                        oldContactNo = Convert.ToString(selectedRows[0]["contact_number"]);
                    }

                    if (oldContactNo != ContactNoTextBox.Text)
                    {
                        // Display dash for empty values
                        string oldDisplay = string.IsNullOrEmpty(oldContactNo) ? "-" : oldContactNo;
                        string newDisplay = string.IsNullOrEmpty(ContactNoTextBox.Text) ? "-" : ContactNoTextBox.Text;
                        changes.AppendLine($"Contact No.: {oldDisplay} > {newDisplay}");
                        hasChanges = true;
                    }

                    // Sex change
                    string oldSex = "";
                    string newSex = SexCombobox.SelectedIndex >= 0 ? SexCombobox.SelectedItem.ToString() : "";

                    if (!Convert.IsDBNull(selectedRows[0]["sex"]))
                    {
                        oldSex = Convert.ToString(selectedRows[0]["sex"]);
                    }

                    if ((oldSex ?? "") != (newSex ?? ""))
                    {
                        // Display dash for empty values
                        string oldDisplay = string.IsNullOrEmpty(oldSex) ? "-" : oldSex;
                        string newDisplay = string.IsNullOrEmpty(newSex) ? "-" : newSex;
                        changes.AppendLine($"Sex: {oldDisplay} > {newDisplay}");
                        hasChanges = true;
                    }

                    // Birthday change
                    var oldBirthday = DateTime.Today;
                    bool hasBirthday = false;

                    if (!Convert.IsDBNull(selectedRows[0]["birthday"]))
                    {
                        oldBirthday = Convert.ToDateTime(selectedRows[0]["birthday"]);
                        hasBirthday = true;
                    }

                    if (hasBirthday & oldBirthday != Bdaypicket.Value | !hasBirthday & Bdaypicket.Value != DateTime.Today)
                    {
                        string oldBirthdayStr = hasBirthday ? oldBirthday.ToString("yyyy-MM-dd") : "-";
                        string newBirthdayStr = Bdaypicket.Value == DateTime.Today ? "-" : Bdaypicket.Value.ToString("yyyy-MM-dd");
                        changes.AppendLine($"Birthday: {oldBirthdayStr} > {newBirthdayStr}");
                        hasChanges = true;
                    }

                    // For student-specific fields
                    string oldStudentId = Convert.ToString(selectedRows[0]["student_id"]);
                    if (!string.IsNullOrEmpty(oldStudentId) | newRole == "Student")
                    {
                        // Student ID change
                        if (oldStudentId != TextBox1.Text)
                        {
                            // Display dash for empty values
                            string oldDisplay = string.IsNullOrEmpty(oldStudentId) ? "-" : oldStudentId;
                            string newDisplay = string.IsNullOrEmpty(TextBox1.Text) ? "-" : TextBox1.Text;
                            changes.AppendLine($"Student ID: {oldDisplay} > {newDisplay}");
                            hasChanges = true;
                        }

                        // Program change
                        int oldProgramId = -1;
                        if (!Convert.IsDBNull(selectedRows[0]["program_id"]))
                        {
                            oldProgramId = Convert.ToInt32(selectedRows[0]["program_id"]) - 1;
                        }

                        int newProgramIndex = CourseCombobox.SelectedIndex;
                        if (oldProgramId != newProgramIndex && newProgramIndex >= 0 && newProgramIndex < CourseCombobox.Items.Count)
                        {
                            string oldProgram = oldProgramId >= 0 && oldProgramId < CourseCombobox.Items.Count ? CourseCombobox.Items[oldProgramId].ToString() : "-";
                            changes.AppendLine($"Program: {oldProgram} > {CourseCombobox.Items[newProgramIndex]}");
                            hasChanges = true;
                        }

                        // Year level change
                        int oldYearLevelId = -1;
                        if (!Convert.IsDBNull(selectedRows[0]["year_level_id"]))
                        {
                            oldYearLevelId = Convert.ToInt32(selectedRows[0]["year_level_id"]) - 1;
                        }

                        int newYearIndex = YearCombobox.SelectedIndex;
                        if (oldYearLevelId != newYearIndex && newYearIndex >= 0 && newYearIndex < YearCombobox.Items.Count)
                        {
                            string oldYear = oldYearLevelId >= 0 && oldYearLevelId < YearCombobox.Items.Count ? YearCombobox.Items[oldYearLevelId].ToString() : "-";
                            changes.AppendLine($"Year: {oldYear} > {YearCombobox.Items[newYearIndex]}");
                            hasChanges = true;
                        }

                        // Section change
                        int oldSectionId = -1;
                        if (!Convert.IsDBNull(selectedRows[0]["section_id"]))
                        {
                            oldSectionId = Convert.ToInt32(selectedRows[0]["section_id"]) - 1;
                        }

                        int newSectionIndex = SectionCombobox.SelectedIndex;
                        if (oldSectionId != newSectionIndex && newSectionIndex >= 0 && newSectionIndex < SectionCombobox.Items.Count)
                        {
                            string oldSection = oldSectionId >= 0 && oldSectionId < SectionCombobox.Items.Count ? SectionCombobox.Items[oldSectionId].ToString() : "-";
                            changes.AppendLine($"Section: {oldSection} > {SectionCombobox.Items[newSectionIndex]}");
                            hasChanges = true;
                        }

                        // Student Status change
                        string oldStudentStatus = "";
                        if (!Convert.IsDBNull(selectedRows[0]["student_status"]))
                        {
                            oldStudentStatus = CapitalizeFirstLetter(Convert.ToString(selectedRows[0]["student_status"]));
                        }

                        if (StudentStatusCombobox.SelectedIndex >= 0 && oldStudentStatus != StudentStatusCombobox.SelectedItem.ToString())
                        {
                            string oldDisplay = string.IsNullOrEmpty(oldStudentStatus) ? "-" : oldStudentStatus;
                            changes.AppendLine($"Student Status: {oldDisplay} > {StudentStatusCombobox.SelectedItem}");
                            hasChanges = true;
                        }

                        // Academic Status change
                        string oldAcademicStatus = "";
                        if (!Convert.IsDBNull(selectedRows[0]["academic_status"]))
                        {
                            oldAcademicStatus = CapitalizeFirstLetter(Convert.ToString(selectedRows[0]["academic_status"]));
                        }

                        if (ComboBox4.SelectedIndex >= 0 && oldAcademicStatus != ComboBox4.SelectedItem.ToString())
                        {
                            string oldDisplay = string.IsNullOrEmpty(oldAcademicStatus) ? "-" : oldAcademicStatus;
                            changes.AppendLine($"Academic Status: {oldDisplay} > {ComboBox4.SelectedItem}");
                            hasChanges = true;
                        }

                        // School Year change
                        string oldSchoolYear = "";
                        if (!Convert.IsDBNull(selectedRows[0]["school_year"]))
                        {
                            oldSchoolYear = Convert.ToString(selectedRows[0]["school_year"]);
                        }

                        string newSchoolYear = "";
                        if (SchoolYRCombobox.SelectedIndex >= 0)
                        {
                            newSchoolYear = SchoolYRCombobox.SelectedItem.ToString();
                        }

                        if ((oldSchoolYear ?? "") != (newSchoolYear ?? ""))
                        {
                            string oldSYDisplay = string.IsNullOrEmpty(oldSchoolYear) ? "-" : oldSchoolYear;
                            string newSYDisplay = string.IsNullOrEmpty(newSchoolYear) ? "-" : newSchoolYear;
                            changes.AppendLine($"School Year: {oldSYDisplay} > {newSYDisplay}");
                            hasChanges = true;
                        }
                    }

                    // If no changes were detected
                    if (!hasChanges)
                    {
                        changes.AppendLine("No changes detected. Save anyway?");
                    }

                    // Ask for confirmation before saving
                    DialogResult result = MessageBox.Show(changes.ToString(), "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        return;

                    // Proceed with the update
                    UpdateExistingUser();

                    // Create a personalized success message
                    var updateReceipt = new StringBuilder();

                    // Display full name with role
                    updateReceipt.AppendLine($"User {newFullName} ({newRole}) has been successfully updated.");
                    updateReceipt.AppendLine("");

                    if (hasChanges)
                    {
                        updateReceipt.AppendLine("Updated information:");

                        // Loop through each line of the changes StringBuilder that contains actual changes
                        foreach (string line in changes.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            // Only include lines that show actual changes (contain ">" symbol)
                            if (line.Contains(">") && !line.StartsWith("Are you") && !line.StartsWith("Changes") && !string.IsNullOrWhiteSpace(line))
                            {
                                updateReceipt.AppendLine(line);
                            }
                        }
                    }
                    else
                    {
                        updateReceipt.AppendLine("No fields were changed.");
                    }

                    // Show the personalized success message
                    MessageBox.Show(updateReceipt.ToString(), "User Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload data to reflect changes
                    LoadUsers();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("User data could not be found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private int FindExistingUser()
    {
        // If we're already in update mode, don't check for existing users
        if (!isNewUser)
            return -1;

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // First check for exact student ID match (for students only)
                if (RoleComboBox.SelectedItem.ToString() == "Student" && !string.IsNullOrWhiteSpace(TextBox1.Text))
                {
                    string studentIdQuery = "SELECT u.user_id FROM users u INNER JOIN student_profiles sp ON u.user_id = sp.user_id WHERE sp.student_id = @studentId AND (u.is_archived = 0 OR u.is_archived IS NULL)";
                    using (var cmd = new SqlCommand(studentIdQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@studentId", TextBox1.Text);
                        var result = cmd.ExecuteScalar();
                        if (result is not null && !Convert.IsDBNull(result))
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }

                // Then check for email match
                if (!string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    string emailQuery = "SELECT user_id FROM users WHERE email = @email AND (is_archived = 0 OR is_archived IS NULL)";
                    using (var cmd = new SqlCommand(emailQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@email", EmailTextBox.Text);
                        var result = cmd.ExecuteScalar();
                        if (result is not null && !Convert.IsDBNull(result))
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                }

                // Finally, check for full name match (exact matches on first, middle, and last name)
                string nameQuery = "SELECT user_id FROM users WHERE first_name = @firstName ";

                if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
                {
                    nameQuery += "AND (middle_name IS NULL OR middle_name = '') ";
                }
                else
                {
                    nameQuery += "AND middle_name = @middleName ";
                }

                nameQuery += "AND last_name = @lastName AND (is_archived = 0 OR is_archived IS NULL)";

                using (var cmd = new SqlCommand(nameQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@firstName", FnameTextBox.Text);

                    if (!string.IsNullOrWhiteSpace(MnameTextBox.Text))
                    {
                        cmd.Parameters.AddWithValue("@middleName", MnameTextBox.Text);
                    }

                    cmd.Parameters.AddWithValue("@lastName", LnameTextBox.Text);

                    var result = cmd.ExecuteScalar();
                    if (result is not null && !Convert.IsDBNull(result))
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            // If we get here, no matching user was found
            return -1;
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"Error checking for existing user: {ex.Message}");
            MessageBox.Show($"Error checking for existing user: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return -1;
        }
    }

    // New function to get information about an existing user
    private string GetExistingUserInfo(int userId)
    {
        var userInfo = new StringBuilder();

        try
        {
            // Find the user in the data table
            DataRow[] selectedRows = userDataTable.Select($"user_id = {userId}");

            if (selectedRows.Length > 0)
            {
                // Get user name
                string firstName = Convert.ToString(selectedRows[0]["first_name"]);
                string lastName = Convert.ToString(selectedRows[0]["last_name"]);
                string middleName = "";

                if (!Convert.IsDBNull(selectedRows[0]["middle_name"]))
                {
                    middleName = Convert.ToString(selectedRows[0]["middle_name"]);
                }

                // Format the name display
                string fullName;
                if (string.IsNullOrWhiteSpace(middleName))
                {
                    fullName = $"{firstName} {lastName}";
                }
                else
                {
                    fullName = $"{firstName} {middleName} {lastName}";
                }

                userInfo.AppendLine($"Name: {fullName}");

                // Add email
                string email = Convert.ToString(selectedRows[0]["email"]);
                userInfo.AppendLine($"Email: {email}");

                // Add role
                string role = Convert.ToString(selectedRows[0]["user_type"]);
                userInfo.AppendLine($"Role: {role}");

                // Add student ID if available
                string studentId = Convert.ToString(selectedRows[0]["student_id"]);
                if (!string.IsNullOrWhiteSpace(studentId))
                {
                    userInfo.AppendLine($"Student ID: {studentId}");

                    // Add student details if available
                    string programName = "-";
                    if (!Convert.IsDBNull(selectedRows[0]["program_name"]))
                    {
                        programName = Convert.ToString(selectedRows[0]["program_name"]);
                    }
                    userInfo.AppendLine($"Program: {programName}");

                    // Add year level if available
                    if (!Convert.IsDBNull(selectedRows[0]["year_level_id"]))
                    {
                        string yearLevel = $"Year {selectedRows[0]["year_level_id"]}";
                        userInfo.AppendLine($"Year: {yearLevel}");
                    }

                    // Add section if available
                    string sectionName = "-";
                    if (!Convert.IsDBNull(selectedRows[0]["section_name"]))
                    {
                        sectionName = Convert.ToString(selectedRows[0]["section_name"]);
                    }
                    userInfo.AppendLine($"Section: {sectionName}");
                }
            }

            return userInfo.ToString();
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"Error getting existing user info: {ex.Message}");
            MessageBox.Show($"Error getting existing user info: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return "Error retrieving user information";
        }
    }

    // Helper method to load user data from database by ID
    private void LoadUserData(int userId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT u.user_id,  u.email, u.password_hash, 
                       u.first_name, u.middle_name, u.last_name, u.user_type,
                       u.birthday, u.sex, u.address, u.contact_number,
                       sp.student_id, sp.program_id, 
                       p.program_name,
                       sp.year_level_id, sp.section_id, s.section_name,
                       sp.student_status, sp.academic_status, sp.enrollment_date,
                       sp.school_year_id, sy.year_name AS school_year
                FROM users u
                LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                LEFT JOIN programs p ON sp.program_id = p.program_id
                LEFT JOIN sections s ON sp.section_id = s.section_id
                LEFT JOIN school_years sy ON sp.school_year_id = sy.school_year_id
                WHERE u.user_id = @userId
            ";

                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set form fields with user data
                            FnameTextBox.Text = reader["first_name"].ToString();

                            // Set middle name if available in data
                            if (!reader.IsDBNull(reader.GetOrdinal("middle_name")))
                            {
                                MnameTextBox.Text = reader["middle_name"].ToString();
                            }
                            else
                            {
                                MnameTextBox.Text = "";
                            }

                            LnameTextBox.Text = reader["last_name"].ToString();
                            EmailTextBox.Text = reader["email"].ToString();
                            PasswordTextBox.Text = "********"; // Don't display actual password for security

                            // Populate new fields
                            if (!reader.IsDBNull(reader.GetOrdinal("contact_number")))
                            {
                                ContactNoTextBox.Text = reader["contact_number"].ToString();
                            }
                            else
                            {
                                ContactNoTextBox.Text = "";
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("birthday")))
                            {
                                Bdaypicket.Value = Convert.ToDateTime(reader["birthday"]);
                            }
                            else
                            {
                                Bdaypicket.Value = DateTime.Today;
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("sex")))
                            {
                                SexCombobox.SelectedItem = reader["sex"].ToString();
                            }
                            else
                            {
                                SexCombobox.SelectedIndex = -1;
                            }

                            // Set role
                            string userType = reader["user_type"].ToString();
                            RoleComboBox.SelectedItem = string.IsNullOrEmpty(userType) ? "Student" : userType;

                            // Set student-specific fields if applicable
                            if (!reader.IsDBNull(reader.GetOrdinal("student_id")))
                            {
                                TextBox1.Text = reader["student_id"].ToString();

                                // Set student status
                                if (!reader.IsDBNull(reader.GetOrdinal("student_status")))
                                {
                                    StudentStatusCombobox.SelectedItem = CapitalizeFirstLetter(reader["student_status"].ToString());
                                    currentStudentStatus = reader["student_status"].ToString();
                                }
                                else
                                {
                                    StudentStatusCombobox.SelectedItem = "Regular";
                                    currentStudentStatus = "regular";
                                }

                                // Set academic status
                                if (!reader.IsDBNull(reader.GetOrdinal("academic_status")))
                                {
                                    ComboBox4.SelectedItem = CapitalizeFirstLetter(reader["academic_status"].ToString());
                                    currentAcademicStatus = reader["academic_status"].ToString();
                                }
                                else
                                {
                                    ComboBox4.SelectedItem = "Active";
                                    currentAcademicStatus = "active";
                                }

                                // Set school year
                                if (!reader.IsDBNull(reader.GetOrdinal("school_year_id")))
                                {
                                    schoolYearId = Convert.ToInt32(reader["school_year_id"]);

                                    // Get the year_name for the ComboBox
                                    if (!reader.IsDBNull(reader.GetOrdinal("school_year")))
                                    {
                                        SchoolYRCombobox.Text = reader["school_year"].ToString();
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
                                if (!reader.IsDBNull(reader.GetOrdinal("program_id")) && !reader.IsDBNull(reader.GetOrdinal("year_level_id")) && !reader.IsDBNull(reader.GetOrdinal("section_id")))
                                {

                                    SetProgramYearSection(Convert.ToInt32(reader["program_id"]), Convert.ToInt32(reader["year_level_id"]), Convert.ToInt32(reader["section_id"]));
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"Error loading user data: {ex.Message}");
            MessageBox.Show($"Error loading user data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }




    private void CreateNewUser()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Ensure basic sections exist
                    var checkSectionsCmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'sections')
                BEGIN
                    CREATE TABLE sections (
                        section_id INT IDENTITY(1,1) PRIMARY KEY,
                        section_name VARCHAR(20) NOT NULL,
                        year_level_id INT NOT NULL,
                        program_id INT NOT NULL,
                        is_active BIT DEFAULT 1,
                        created_at DATETIME DEFAULT GETDATE(),
                        updated_at DATETIME NULL
                    )

                    INSERT INTO sections (section_name, year_level_id, program_id)
                    VALUES ('A', 1, 1), ('B', 1, 1), ('C', 1, 1)
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM sections)
                BEGIN
                    INSERT INTO sections (section_name, year_level_id, program_id)
                    VALUES ('A', 1, 1), ('B', 1, 1), ('C', 1, 1)
                END", connection, transaction);
                    checkSectionsCmd.ExecuteNonQuery();

                    // Create new user with all fields including middle name
                    var userCommand = new SqlCommand(@"
            INSERT INTO users (email, password_hash, first_name, middle_name, last_name, user_type, 
                             birthday, sex, contact_number)
            VALUES (@Email, @PasswordHash, @FirstName, @MiddleName, @LastName, @UserType, 
                   @Birthday, @Sex, @ContactNumber);
            SELECT SCOPE_IDENTITY();
            ", connection, transaction);

                    // Set parameters for user info
                    userCommand.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    userCommand.Parameters.AddWithValue("@PasswordHash", HashPassword(PasswordTextBox.Text));
                    userCommand.Parameters.AddWithValue("@FirstName", FnameTextBox.Text);

                    // Add middle name parameter (allowing NULL if empty)
                    if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
                    {
                        userCommand.Parameters.AddWithValue("@MiddleName", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@MiddleName", MnameTextBox.Text);
                    }

                    userCommand.Parameters.AddWithValue("@LastName", LnameTextBox.Text);
                    userCommand.Parameters.AddWithValue("@UserType", RoleComboBox.SelectedItem.ToString());

                    // Add parameters for new fields
                    if (Bdaypicket.Value == DateTime.Today)
                    {
                        userCommand.Parameters.AddWithValue("@Birthday", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@Birthday", Bdaypicket.Value);
                    }

                    if (SexCombobox.SelectedIndex >= 0)
                    {
                        userCommand.Parameters.AddWithValue("@Sex", SexCombobox.SelectedItem.ToString());
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@Sex", DBNull.Value);
                    }

                    if (string.IsNullOrWhiteSpace(ContactNoTextBox.Text))
                    {
                        userCommand.Parameters.AddWithValue("@ContactNumber", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@ContactNumber", ContactNoTextBox.Text);
                    }

                    // Get the new user ID
                    int newUserId = Convert.ToInt32(userCommand.ExecuteScalar());

                    // Check if this is a student role (and not an admin role)
                    string selectedRole = RoleComboBox.SelectedItem.ToString().ToLower();
                    bool isAdminOrFacultyRole = selectedRole == "admin-mis" || selectedRole == "registrar" || selectedRole == "faculty";

                    // If this is a student, add student profile
                    if (RoleComboBox.SelectedItem.ToString() == "Student" && !isAdminOrFacultyRole)
                    {
                        // For each section name ("A", "B", "C"), ensure section records exist
                        string sectionName = SectionCombobox.Items[SectionCombobox.SelectedIndex].ToString();
                        int programId = CourseCombobox.SelectedIndex + 1;
                        int yearLevelId = YearCombobox.SelectedIndex + 1;

                        // Ensure this specific section exists
                        string ensureSectionQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM sections WHERE section_name = @sectionName 
                                  AND year_level_id = @yearLevelId AND program_id = @programId)
                    BEGIN
                        INSERT INTO sections (section_name, year_level_id, program_id)
                        VALUES (@sectionName, @yearLevelId, @programId)
                    END
                    
                    SELECT section_id FROM sections 
                    WHERE section_name = @sectionName 
                    AND year_level_id = @yearLevelId 
                    AND program_id = @programId";

                        var sectionCmd = new SqlCommand(ensureSectionQuery, connection, transaction);
                        sectionCmd.Parameters.AddWithValue("@sectionName", sectionName);
                        sectionCmd.Parameters.AddWithValue("@yearLevelId", yearLevelId);
                        sectionCmd.Parameters.AddWithValue("@programId", programId);

                        // Get the section ID (either existing or newly created)
                        int sectionId = Convert.ToInt32(sectionCmd.ExecuteScalar());

                        // Now create student profile with the correct section ID
                        var studentCommand = new SqlCommand(@"
                    INSERT INTO student_profiles (user_id, student_id, program_id, year_level_id, 
                                          section_id, school_year_id, student_status, academic_status)
                    VALUES (@UserId, @StudentId, @ProgramId, @YearLevelId, 
                       @SectionId, @SchoolYearId, @StudentStatus, @AcademicStatus);
                ", connection, transaction);

                        studentCommand.Parameters.AddWithValue("@UserId", newUserId);
                        studentCommand.Parameters.AddWithValue("@StudentId", TextBox1.Text);
                        studentCommand.Parameters.AddWithValue("@ProgramId", programId);
                        studentCommand.Parameters.AddWithValue("@YearLevelId", yearLevelId);
                        studentCommand.Parameters.AddWithValue("@SectionId", sectionId);

                        // Add school year ID parameter
                        if (schoolYearId <= 0)
                        {
                            // If no school year is selected, get the current school year
                            var schoolYearCmd = new SqlCommand("SELECT school_year_id FROM school_years WHERE is_current = 1", connection, transaction);
                            var currentSchoolYearId = schoolYearCmd.ExecuteScalar();

                            if (currentSchoolYearId is not null && !Convert.IsDBNull(currentSchoolYearId))
                            {
                                schoolYearId = Convert.ToInt32(currentSchoolYearId);
                            }
                            else
                            {
                                schoolYearId = -1;
                            }
                        }

                        // Properly handle NULL values for school year
                        if (schoolYearId > 0)
                        {
                            studentCommand.Parameters.AddWithValue("@SchoolYearId", schoolYearId);
                        }
                        else
                        {
                            studentCommand.Parameters.AddWithValue("@SchoolYearId", DBNull.Value);
                        }

                        studentCommand.Parameters.AddWithValue("@StudentStatus", currentStudentStatus.ToLower());
                        studentCommand.Parameters.AddWithValue("@AcademicStatus", currentAcademicStatus.ToLower());

                        studentCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }





    private string GetUserInfoString(string firstName, string lastName, string email, string userType, string studentId, int programIndex, int yearIndex, int sectionIndex)
    {

        var sb = new StringBuilder();

        // Include middle name if provided
        string fullName;
        if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
        {
            fullName = $"{firstName} {lastName}";
        }
        else
        {
            fullName = $"{firstName} {MnameTextBox.Text} {lastName}";
        }

        sb.AppendLine($"Name: {fullName}");
        sb.AppendLine($"Email: {email}");

        // Add contact number if provided
        if (!string.IsNullOrWhiteSpace(ContactNoTextBox.Text))
        {
            sb.AppendLine($"Contact No.: {ContactNoTextBox.Text}");
        }

        // Add sex if selected
        if (SexCombobox.SelectedIndex >= 0)
        {
            sb.AppendLine($"Sex: {SexCombobox.SelectedItem}");
        }

        // Add birthday if not today
        if (Bdaypicket.Value != DateTime.Today)
        {
            sb.AppendLine($"Birthday: {Bdaypicket.Value.ToString("yyyy-MM-dd")}");
        }

        sb.AppendLine($"Role: {userType}");

        if (userType == "Student")
        {
            sb.AppendLine($"Student ID: {studentId}");

            // Add student status
            if (StudentStatusCombobox.SelectedIndex >= 0)
            {
                sb.AppendLine($"Student Status: {StudentStatusCombobox.SelectedItem}");
            }

            // Add academic status
            if (ComboBox4.SelectedIndex >= 0)
            {
                sb.AppendLine($"Academic Status: {ComboBox4.SelectedItem}");
            }

            string program = programIndex >= 0 && programIndex < CourseCombobox.Items.Count ? CourseCombobox.Items[programIndex].ToString() : "";
            sb.AppendLine($"Program: {program}");

            string year = yearIndex >= 0 && yearIndex < YearCombobox.Items.Count ? YearCombobox.Items[yearIndex].ToString() : "";
            sb.AppendLine($"Year: {year}");

            string section = sectionIndex >= 0 && sectionIndex < SectionCombobox.Items.Count ? SectionCombobox.Items[sectionIndex].ToString() : "";
            sb.AppendLine($"Section: {section}");

            // Add school year if selected
            if (SchoolYRCombobox.SelectedIndex >= 0)
            {
                sb.AppendLine($"School Year: {SchoolYRCombobox.SelectedItem}");
            }
        }

        return sb.ToString();
    }


    private void UpdateExistingUser()
    {
        if (currentUserId <= 0)
        {
            throw new Exception("Invalid user ID for update");
        }

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Update user table with all fields
                    var userCommand = new SqlCommand(@"
               UPDATE users
               SET email = @email, first_name = @first_name, middle_name = @middle_name, last_name = @last_name, 
                   user_type = @user_type, birthday = @birthday, sex = @sex,
                   contact_number = @contact_number, updated_at = GETDATE()
               WHERE user_id = @user_id
               ", connection, transaction);

                    // And add the parameter
                    if (string.IsNullOrWhiteSpace(MnameTextBox.Text))
                    {
                        userCommand.Parameters.AddWithValue("@middle_name", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@middle_name", MnameTextBox.Text);
                    }

                    userCommand.Parameters.AddWithValue("@user_id", currentUserId);
                    userCommand.Parameters.AddWithValue("@email", EmailTextBox.Text);
                    userCommand.Parameters.AddWithValue("@first_name", FnameTextBox.Text);
                    userCommand.Parameters.AddWithValue("@last_name", LnameTextBox.Text);
                    userCommand.Parameters.AddWithValue("@user_type", RoleComboBox.SelectedItem.ToString());

                    // Add parameters for new fields
                    if (Bdaypicket.Value == DateTime.Today)
                    {
                        userCommand.Parameters.AddWithValue("@birthday", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@birthday", Bdaypicket.Value);
                    }

                    if (SexCombobox.SelectedIndex >= 0)
                    {
                        userCommand.Parameters.AddWithValue("@sex", SexCombobox.SelectedItem.ToString());
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@sex", DBNull.Value);
                    }

                    if (string.IsNullOrWhiteSpace(ContactNoTextBox.Text))
                    {
                        userCommand.Parameters.AddWithValue("@contact_number", DBNull.Value);
                    }
                    else
                    {
                        userCommand.Parameters.AddWithValue("@contact_number", ContactNoTextBox.Text);
                    }

                    userCommand.ExecuteNonQuery();

                    // Update password if it's not the placeholder
                    if (PasswordTextBox.Text != "********")
                    {
                        var passwordCommand = new SqlCommand(@"
                    UPDATE users
                    SET password_hash = @password_hash
                    WHERE user_id = @user_id
                ", connection, transaction);

                        passwordCommand.Parameters.AddWithValue("@user_id", currentUserId);
                        passwordCommand.Parameters.AddWithValue("@password_hash", HashPassword(PasswordTextBox.Text));

                        passwordCommand.ExecuteNonQuery();
                    }

                    // Check if student profile exists
                    var checkCommand = new SqlCommand(@"
                SELECT COUNT(*) FROM student_profiles WHERE user_id = @user_id
            ", connection, transaction);

                    checkCommand.Parameters.AddWithValue("@user_id", currentUserId);
                    bool profileExists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

                    // Check if this is a student role (and not an admin role)
                    string selectedRole = RoleComboBox.SelectedItem.ToString().ToLower();
                    bool isAdminOrFacultyRole = selectedRole == "admin-mis" || selectedRole == "registrar" || selectedRole == "faculty";

                    // If this is a student, update or create student profile
                    if (RoleComboBox.SelectedItem.ToString() == "Student" && !isAdminOrFacultyRole)
                    {
                        // Convert combobox selections to IDs (1-based)
                        int programId = CourseCombobox.SelectedIndex + 1;
                        int yearLevelId = YearCombobox.SelectedIndex + 1;
                        int sectionId = SectionCombobox.SelectedIndex + 1;

                        // If no school year is selected, get the current school year
                        if (schoolYearId <= 0)
                        {
                            var schoolYearCmd = new SqlCommand("SELECT school_year_id FROM school_years WHERE is_current = 1", connection, transaction);
                            var currentSchoolYearId = schoolYearCmd.ExecuteScalar();

                            if (currentSchoolYearId is not null && !Convert.IsDBNull(currentSchoolYearId))
                            {
                                schoolYearId = Convert.ToInt32(currentSchoolYearId);
                            }
                            else
                            {
                                schoolYearId = -1;
                            }
                        }

                        if (profileExists)
                        {
                            // Update existing profile including school year and statuses
                            var studentCommand = new SqlCommand(@"
                        UPDATE student_profiles
                        SET student_id = @student_id, program_id = @program_id,
                            year_level_id = @year_level_id, section_id = @section_id,
                            school_year_id = @school_year_id,
                            student_status = @student_status,
                            academic_status = @academic_status
                        WHERE user_id = @user_id
                    ", connection, transaction);

                            studentCommand.Parameters.AddWithValue("@user_id", currentUserId);
                            studentCommand.Parameters.AddWithValue("@student_id", TextBox1.Text);
                            studentCommand.Parameters.AddWithValue("@program_id", programId);
                            studentCommand.Parameters.AddWithValue("@year_level_id", yearLevelId);
                            studentCommand.Parameters.AddWithValue("@section_id", sectionId);

                            // Properly handle NULL values for school year
                            if (schoolYearId > 0)
                            {
                                studentCommand.Parameters.AddWithValue("@school_year_id", schoolYearId);
                            }
                            else
                            {
                                studentCommand.Parameters.AddWithValue("@school_year_id", DBNull.Value);
                            }

                            studentCommand.Parameters.AddWithValue("@student_status", currentStudentStatus.ToLower());
                            studentCommand.Parameters.AddWithValue("@academic_status", currentAcademicStatus.ToLower());

                            studentCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            // Create new profile with all fields
                            var studentCommand = new SqlCommand(@"
                        INSERT INTO student_profiles (user_id, student_id, program_id, year_level_id, 
                                                 section_id, school_year_id, student_status, academic_status)
                        VALUES (@user_id, @student_id, @program_id, @year_level_id, 
                               @section_id, @school_year_id, @student_status, @academic_status)
                    ", connection, transaction);

                            studentCommand.Parameters.AddWithValue("@user_id", currentUserId);
                            studentCommand.Parameters.AddWithValue("@student_id", TextBox1.Text);
                            studentCommand.Parameters.AddWithValue("@program_id", programId);
                            studentCommand.Parameters.AddWithValue("@year_level_id", yearLevelId);
                            studentCommand.Parameters.AddWithValue("@section_id", sectionId);

                            // Properly handle NULL values for school year
                            if (schoolYearId > 0)
                            {
                                studentCommand.Parameters.AddWithValue("@school_year_id", schoolYearId);
                            }
                            else
                            {
                                studentCommand.Parameters.AddWithValue("@school_year_id", DBNull.Value);
                            }

                            studentCommand.Parameters.AddWithValue("@student_status", currentStudentStatus.ToLower());
                            studentCommand.Parameters.AddWithValue("@academic_status", currentAcademicStatus.ToLower());

                            studentCommand.ExecuteNonQuery();
                        }
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



    private string HashPassword(string password)
    {
        // Using PBKDF2 for password hashing
        // Generate a salt
        byte[] salt;
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

        // Create the Rfc2898DeriveBytes and get the hash value
        var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 10000); // 10000 iterations
        byte[] hash = pbkdf2.GetBytes(20); // 20-byte hash

        // Combine salt and hash for storage
        byte[] hashBytes = new byte[36]; // 16 bytes for salt + 20 bytes for hash
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        // Convert to base64 for string storage
        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string savedPasswordHash, string passwordToCheck)
    {
        // Extract bytes from base64 string
        byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

        // Get salt from the stored hash
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Compute hash of the password to check
        var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(passwordToCheck, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Compare the results
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
            {
                return false;
            }
        }
        return true;
    }

    private bool ValidateForm()
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(FnameTextBox.Text))
        {
            MessageBox.Show("First name is required", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(LnameTextBox.Text))
        {
            MessageBox.Show("Last name is required", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
        {
            MessageBox.Show("Email is required", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Validate email format
        if (!System.Text.RegularExpressions.Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MessageBox.Show("Invalid email format", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Check if role is selected
        if (RoleComboBox.SelectedItem is null)
        {
            MessageBox.Show("Role must be selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Get the selected role (case insensitive comparison)
        string selectedRole = RoleComboBox.SelectedItem.ToString().ToLower();

        // Determine if this is an admin/faculty type role that doesn't need student details
        bool isAdminOrFacultyRole = selectedRole == "admin-mis" || selectedRole == "registrar" || selectedRole == "faculty";

        // Validate student-specific fields only if role is Student and not an admin/faculty role
        if (RoleComboBox.SelectedItem.ToString() == "Student" && !isAdminOrFacultyRole)
        {
            if (string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                MessageBox.Show("Student ID is required", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate student ID format (8 digits-Letter)
            if (!System.Text.RegularExpressions.Regex.IsMatch(TextBox1.Text, @"^\d{8}-[A-Z]$"))
            {
                MessageBox.Show("Student ID must be in the format: 8 digits followed by a dash and a letter (e.g., 20230902-N)", "Invalid Student ID Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (CourseCombobox.SelectedItem is null)
            {
                MessageBox.Show("Course must be selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (YearCombobox.SelectedItem is null)
            {
                MessageBox.Show("Year must be selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (SectionCombobox.SelectedItem is null)
            {
                MessageBox.Show("Section must be selected", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        // Validate password for new users
        if (isNewUser && string.IsNullOrWhiteSpace(PasswordTextBox.Text))
        {
            MessageBox.Show("Password is required for new users", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }




    private void InitializeSchoolYearsComboBox()
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // The school_years table creation is now handled by EnsureRequiredTablesExist.
                // We just need to populate the ComboBox here.

                // Now get the school years for the ComboBox
                string query = "SELECT year_name FROM school_years ORDER BY is_current DESC, year_name DESC";
                using (var cmd = new SqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        SchoolYRCombobox.Items.Clear();

                        while (reader.Read())
                            SchoolYRCombobox.Items.Add(reader.GetString(0));

                        if (SchoolYRCombobox.Items.Count > 0)
                        {
                            SchoolYRCombobox.SelectedIndex = 0;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"Error loading school years: {ex.Message}");
            MessageBox.Show($"Error loading school years: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Add fallback items if database connection fails
            SchoolYRCombobox.Items.Clear();
            SchoolYRCombobox.Items.Add("24-25");
            SchoolYRCombobox.Items.Add("25-26");

            if (SchoolYRCombobox.Items.Count > 0)
            {
                SchoolYRCombobox.SelectedIndex = 0;
            }
        }
    }


    private void ClearForm()
    {
        // Clear all input fields
        FnameTextBox.Text = "";
        MnameTextBox.Text = "";
        LnameTextBox.Text = "";
        EmailTextBox.Text = "";
        PasswordTextBox.Text = "";
        TextBox1.Text = "";
        ContactNoTextBox.Text = "";

        // Reset comboboxes
        if (RoleComboBox.Items.Count > 0)
            RoleComboBox.SelectedIndex = 0;
        if (CourseCombobox.Items.Count > 0)
            CourseCombobox.SelectedIndex = -1;
        if (YearCombobox.Items.Count > 0)
            YearCombobox.SelectedIndex = -1;
        if (SectionCombobox.Items.Count > 0)
            SectionCombobox.SelectedIndex = -1;
        if (StudentStatusCombobox.Items.Count > 0)
            StudentStatusCombobox.SelectedIndex = 0;  // Student Status (Regular by default)
        if (SchoolYRCombobox.Items.Count > 0)
            SchoolYRCombobox.SelectedIndex = -1;  // School Year
        if (SexCombobox.Items.Count > 0)
            SexCombobox.SelectedIndex = -1;  // Sex
        if (ComboBox4.Items.Count > 0)
            ComboBox4.SelectedIndex = 0;  // Academic Status (Active by default)

        // Reset date picker
        Bdaypicket.Value = DateTime.Today;

        // Reset internal state variables
        // Note: We don't reset isNewUser here anymore since it's set in ClearButton_Click
        currentUserId = -1;
        schoolYearId = -1;
        currentStudentStatus = "Regular";
        currentAcademicStatus = "Active";
    }

    private void PassEyeButton_Click(object sender, EventArgs e)
    {
        // Toggle password visibility
        PasswordTextBox.UseSystemPasswordChar = !PasswordTextBox.UseSystemPasswordChar;

        // Update button text to indicate current state
        PassEyeButton.Text = PasswordTextBox.UseSystemPasswordChar ? "H" : "S";
    }

    private void EmailTextBox_Leave(object sender, EventArgs e)
    {
        // Validate email format when focus leaves the textbox
        if (!string.IsNullOrEmpty(EmailTextBox.Text) && !System.Text.RegularExpressions.Regex.IsMatch(EmailTextBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MessageBox.Show("Please enter a valid email address", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void NameTextBox_Leave(object sender, EventArgs e)
    {
        // Auto-capitalize first letter of names
        TextBox textBox = (TextBox)sender;
        if (!string.IsNullOrEmpty(textBox.Text))
        {
            textBox.Text = CapitalizeFirstLetter(textBox.Text);
        }
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

    private void ClearButton_Click(object sender, EventArgs e)
    {
        ClearForm();
        // Set isNewUser to True to create a new user when the form is cleared
        isNewUser = true;
    }

    private void UserDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }
}