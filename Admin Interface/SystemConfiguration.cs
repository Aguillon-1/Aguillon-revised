using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClassroomManagementSystem
{
    public partial class SystemConfiguration : UserControl
    {
        // Connection string for database access
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=F:\OneDrive - Pesonal\OneDrive\Videos\adminupdated\Classroom-Mangement-System-VB-master\Database1.mdf;Integrated Security=True";

        // Explicitly specify the namespace for Timer to resolve ambiguity
        private System.Windows.Forms.Timer dateTimeTimer;

        // DataTable for the school year list
        private DataTable schoolYearTable;

        // Button column indices for tracking
        private int editButtonIndex = -1;
        private int deleteButtonIndex = -1;
        private int scheduleButtonIndex = -1;

        public SystemConfiguration()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Initialize the form and set up events
            this.Load += SystemConfiguration_Load;
        }

        private void SystemConfiguration_Load(object sender, EventArgs e)
        {
            // Initialize UI components and load data
            SetupDateTimeDisplay();
            LoadSchoolYears();
            LoadSemesters();
            ConfigureSchoolYearDataGrid();

            // Disable editing in comboboxes
            SetSchoolYearCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            SetSemesterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Timer to update DateTime display
        private void SetupDateTimeDisplay()
        {
            // Create and configure timer for date/time updates
            dateTimeTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000,  // Update every second
                Enabled = true
            };

            // Add event handler for timer tick
            dateTimeTimer.Tick += DateTimeTimer_Tick;

            // Initial update of date time label
            UpdateDateTimeLabel();
        }

        // Update DateTime label with current time
        private void UpdateDateTimeLabel()
        {
            DateTime now = DateTime.Now;
            string dayName = now.ToString("dddd").ToUpper();
            DateTimeLabel.Text = $"{now:MM/dd/yyyy - HH:mm:ss} - {dayName}";
        }

        // Timer tick event handler
        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            UpdateDateTimeLabel();
        }

        // Add School Year button click event
        private void AddSYButton_Click(object sender, EventArgs e)
        {
            // Validate input format (yyyy-yyyy)
            string schoolYearText = AddSchoolYearTextBox.Text.Trim();
            if (!IsValidSchoolYearFormat(schoolYearText))
            {
                MessageBox.Show("Please enter a valid school year in the format 'yyyy-yyyy' (e.g., 2023-2024)",
                               "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ask for confirmation
            DialogResult result = MessageBox.Show($"Are you sure you want to add '{schoolYearText}' as a new school year?",
                                                "Confirm Addition", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Add school year to database
                    if (AddSchoolYearToDatabase(schoolYearText))
                    {
                        MessageBox.Show("School year added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AddSchoolYearTextBox.Clear();
                        LoadSchoolYears(); // Refresh the dropdown and grid
                    }
                    else
                    {
                        MessageBox.Show("The school year already exists or could not be added.",
                                       "Cannot Add", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding school year: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Validate school year format (yyyy-yyyy)
        private bool IsValidSchoolYearFormat(string schoolYearText)
        {
            // Check for yyyy-yyyy format
            if (!Regex.IsMatch(schoolYearText, @"^\d{4}-\d{4}$"))
            {
                return false;
            }

            // Check that the second year is exactly one more than the first year
            string[] years = schoolYearText.Split('-');
            int firstYear = int.Parse(years[0]);
            int secondYear = int.Parse(years[1]);

            return secondYear == firstYear + 1;
        }

        // Add a new school year to the database
        private bool AddSchoolYearToDatabase(string schoolYearText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // First check if the school year already exists
                string checkQuery = "SELECT COUNT(*) FROM school_years WHERE year_name = @yearName";
                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@yearName", schoolYearText);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        return false; // Already exists
                    }
                }

                // If we get here, we can add the new school year
                string insertQuery = "INSERT INTO school_years (year_name, is_current) VALUES (@yearName, 0)";
                using (var insertCmd = new SqlCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@yearName", schoolYearText);
                    int rowsAffected = insertCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Load school years from database to the combobox
        private void LoadSchoolYears()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get current school year from system_configuration
                    string currentSchoolYearIdQuery = "SELECT current_school_year_id FROM system_configuration";
                    int currentSchoolYearId = -1;

                    using (var cmdCurrent = new SqlCommand(currentSchoolYearIdQuery, connection))
                    {
                        object result = cmdCurrent.ExecuteScalar();
                        if (result != null && !Convert.IsDBNull(result))
                        {
                            currentSchoolYearId = Convert.ToInt32(result);
                        }
                    }

                    // Get all school years
                    string query = "SELECT school_year_id, year_name FROM school_years ORDER BY year_name DESC";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var schoolYearsTable = new DataTable();
                        adapter.Fill(schoolYearsTable);

                        // Set up the combobox
                        SetSchoolYearCombobox.DataSource = schoolYearsTable;
                        SetSchoolYearCombobox.DisplayMember = "year_name";
                        SetSchoolYearCombobox.ValueMember = "school_year_id";

                        // Select current school year if it exists
                        if (currentSchoolYearId != -1)
                        {
                            for (int i = 0; i < schoolYearsTable.Rows.Count; i++)
                            {
                                if ((int)schoolYearsTable.Rows[i]["school_year_id"] == currentSchoolYearId)
                                {
                                    SetSchoolYearCombobox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading school years: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load semesters from database to the combobox
        private void LoadSemesters()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get current semester from system_configuration
                    string currentSemesterIdQuery = "SELECT current_semester_id FROM system_configuration";
                    int currentSemesterId = -1;

                    using (var cmdCurrent = new SqlCommand(currentSemesterIdQuery, connection))
                    {
                        object result = cmdCurrent.ExecuteScalar();
                        if (result != null && !Convert.IsDBNull(result))
                        {
                            currentSemesterId = Convert.ToInt32(result);
                        }
                    }

                    // Get all semesters
                    string query = "SELECT semester_id, semester_name FROM semesters ORDER BY semester_id";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var semestersTable = new DataTable();
                        adapter.Fill(semestersTable);

                        // Set up the combobox
                        SetSemesterCombobox.DataSource = semestersTable;
                        SetSemesterCombobox.DisplayMember = "semester_name";
                        SetSemesterCombobox.ValueMember = "semester_id";

                        // Select current semester if it exists
                        if (currentSemesterId != -1)
                        {
                            for (int i = 0; i < semestersTable.Rows.Count; i++)
                            {
                                if ((int)semestersTable.Rows[i]["semester_id"] == currentSemesterId)
                                {
                                    SetSemesterCombobox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading semesters: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Set current school year and semester in system configuration
        private void SetButton_Click(object sender, EventArgs e)
        {
            if (SetSchoolYearCombobox.SelectedValue == null || SetSemesterCombobox.SelectedValue == null)
            {
                MessageBox.Show("Please select both a school year and semester.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected values
            int schoolYearId = (int)SetSchoolYearCombobox.SelectedValue;
            int semesterId = (int)SetSemesterCombobox.SelectedValue;
            string schoolYearName = SetSchoolYearCombobox.Text;
            string semesterName = SetSemesterCombobox.Text;

            // Ask for confirmation
            DialogResult result = MessageBox.Show($"Are you sure you want to set {schoolYearName} - {semesterName} " +
                                              $"as the current academic period?",
                                             "Confirm System Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
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
                                // Update system configuration
                                string updateQuery = @"
                                    UPDATE system_configuration 
                                    SET current_school_year_id = @schoolYearId, 
                                        current_semester_id = @semesterId
                                ";

                                using (var updateCmd = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@schoolYearId", schoolYearId);
                                    updateCmd.Parameters.AddWithValue("@semesterId", semesterId);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Update school_years table to reflect current year
                                string resetYearsQuery = "UPDATE school_years SET is_current = 0";
                                using (var resetCmd = new SqlCommand(resetYearsQuery, connection, transaction))
                                {
                                    resetCmd.ExecuteNonQuery();
                                }

                                string setCurrentYearQuery = "UPDATE school_years SET is_current = 1 WHERE school_year_id = @yearId";
                                using (var setCurrentCmd = new SqlCommand(setCurrentYearQuery, connection, transaction))
                                {
                                    setCurrentCmd.Parameters.AddWithValue("@yearId", schoolYearId);
                                    setCurrentCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                MessageBox.Show("System configuration updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Refresh data
                                LoadSchoolYears();
                                LoadSemesters();
                                LoadSchoolYearData();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw; // Re-throw the exception for the outer catch block
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating system configuration: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Configure the School Year DataGridView with necessary columns
        private void ConfigureSchoolYearDataGrid()
        {
            SchoolYearListDatagrid.DataSource = null;
            SchoolYearListDatagrid.Columns.Clear();

            // Create DataTable with the required structure
            schoolYearTable = new DataTable();
            schoolYearTable.Columns.Add("school_year_id", typeof(int));
            schoolYearTable.Columns.Add("year_name", typeof(string));
            schoolYearTable.Columns.Add("semester_id", typeof(int));
            schoolYearTable.Columns.Add("semester_name", typeof(string));
            schoolYearTable.Columns.Add("schedule_date", typeof(DateTime));

            // Set up the DataGridView columns
            SchoolYearListDatagrid.AutoGenerateColumns = false;

            // School Year column
            var schoolYearColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "year_name",
                HeaderText = "School Year",
                Width = 120,
                ReadOnly = true
            };
            SchoolYearListDatagrid.Columns.Add(schoolYearColumn);

            // Semester column
            var semesterColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "semester_name",
                HeaderText = "Semester",
                Width = 120,
                ReadOnly = true
            };
            SchoolYearListDatagrid.Columns.Add(semesterColumn);

            // Schedule column
            var scheduleColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "schedule_date",
                HeaderText = "Schedule",
                Width = 150,
                ReadOnly = true
            };
            scheduleColumn.DefaultCellStyle.Format = "MM/dd/yyyy hh:mm tt";
            SchoolYearListDatagrid.Columns.Add(scheduleColumn);

            // Schedule Button column
            var scheduleButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Set Schedule",
                Text = "Set Schedule",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            SchoolYearListDatagrid.Columns.Add(scheduleButtonColumn);
            scheduleButtonIndex = 3;

            // Edit Button column - only for school year rows
            var editButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 70
            };
            SchoolYearListDatagrid.Columns.Add(editButtonColumn);
            editButtonIndex = 4;

            // Delete Button column - only for school year rows
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                HeaderText = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 70
            };
            SchoolYearListDatagrid.Columns.Add(deleteButtonColumn);
            deleteButtonIndex = 5;

            // Add event handlers
            SchoolYearListDatagrid.CellContentClick += SchoolYearListDatagrid_CellContentClick;
            SchoolYearListDatagrid.CellPainting += SchoolYearListDatagrid_CellPainting;

            // Load data
            LoadSchoolYearData();
        }

        // Load school year data into the grid
        private void LoadSchoolYearData()
        {
            try
            {
                if (schoolYearTable != null)
                {
                    schoolYearTable.Clear();
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get school years
                    string query = "SELECT school_year_id, year_name FROM school_years ORDER BY year_name DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var schoolYears = new System.Collections.Generic.Dictionary<int, string>();

                            while (reader.Read())
                            {
                                int schoolYearId = reader.GetInt32(0);
                                string schoolYearName = reader.GetString(1);
                                schoolYears.Add(schoolYearId, schoolYearName);
                            }

                            reader.Close();

                            // Get semester data
                            string semesterQuery = "SELECT semester_id, semester_name FROM semesters ORDER BY semester_id";
                            using (var semesterCmd = new SqlCommand(semesterQuery, connection))
                            {
                                using (var semesterReader = semesterCmd.ExecuteReader())
                                {
                                    var semesters = new System.Collections.Generic.List<(int, string)>();

                                    while (semesterReader.Read())
                                    {
                                        semesters.Add((semesterReader.GetInt32(0), semesterReader.GetString(1)));
                                    }

                                    semesterReader.Close();

                                    // Get schedule data if it exists (assuming there's a schedule table)
                                    var schedules = new System.Collections.Generic.Dictionary<string, DateTime>();

                                    try
                                    {
                                        // Check if academic_schedules table exists
                                        string tableExistsQuery = "SELECT OBJECT_ID('academic_schedules', 'U')";
                                        using (var checkCmd = new SqlCommand(tableExistsQuery, connection))
                                        {
                                            object tableId = checkCmd.ExecuteScalar();

                                            if (tableId != null && !Convert.IsDBNull(tableId))
                                            {
                                                // Table exists, get data
                                                string scheduleQuery = @"
                                                    SELECT school_year_id, semester_id, schedule_date 
                                                    FROM academic_schedules
                                                    WHERE school_year_id IS NOT NULL AND semester_id IS NOT NULL
                                                ";

                                                using (var scheduleCmd = new SqlCommand(scheduleQuery, connection))
                                                {
                                                    using (var scheduleReader = scheduleCmd.ExecuteReader())
                                                    {
                                                        while (scheduleReader.Read())
                                                        {
                                                            int syId = scheduleReader.GetInt32(0);
                                                            int semId = scheduleReader.GetInt32(1);
                                                            DateTime scheduleDate = scheduleReader.GetDateTime(2);
                                                            string key = $"{syId}_{semId}";

                                                            schedules[key] = scheduleDate;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Table might not exist, that's OK for now
                                        Console.WriteLine("Schedule table might not exist: " + ex.Message);
                                    }

                                    // Create all the required rows for the DataGridView
                                    foreach (var sy in schoolYears)
                                    {
                                        foreach (var sem in semesters)
                                        {
                                            DataRow row = schoolYearTable.NewRow();
                                            row["school_year_id"] = sy.Key;
                                            row["year_name"] = sy.Value;
                                            row["semester_id"] = sem.Item1;
                                            row["semester_name"] = sem.Item2;

                                            // Add schedule if it exists
                                            string scheduleKey = $"{sy.Key}_{sem.Item1}";
                                            if (schedules.ContainsKey(scheduleKey))
                                            {
                                                row["schedule_date"] = schedules[scheduleKey];
                                            }
                                            else
                                            {
                                                row["schedule_date"] = DBNull.Value;
                                            }

                                            schoolYearTable.Rows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Bind the data to the grid
                SchoolYearListDatagrid.DataSource = schoolYearTable;

                // Apply styling and formatting
                ApplySchoolYearGridFormatting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading school year data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Apply formatting to the SchoolYearListDatagrid
        private void ApplySchoolYearGridFormatting()
        {
            // Apply alternating row colors
            SchoolYearListDatagrid.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Customize the appearance of the grid
            SchoolYearListDatagrid.RowHeadersVisible = false;
            SchoolYearListDatagrid.AllowUserToAddRows = false;
            SchoolYearListDatagrid.AllowUserToDeleteRows = false;
            SchoolYearListDatagrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            SchoolYearListDatagrid.MultiSelect = false;
            SchoolYearListDatagrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            SchoolYearListDatagrid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            SchoolYearListDatagrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            SchoolYearListDatagrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        // Handle cell painting to show/hide edit and delete buttons only on school year rows
        private void SchoolYearListDatagrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Determine if this is the first row for a school year
            bool isFirst = IsFirstRowOfSchoolYear(e.RowIndex);

            // Hide or show the edit and delete buttons
            if (e.ColumnIndex == editButtonIndex || e.ColumnIndex == deleteButtonIndex)
            {
                if (isFirst)
                {
                    // Show button
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                }
                else
                {
                    // Hide button, paint the cell background instead
                    using (var brush = new SolidBrush(e.CellStyle.BackColor))
                    {
                        e.Graphics.FillRectangle(brush, e.CellBounds);
                    }
                    e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
                    e.Handled = true;
                }
            }
        }

        // Determine if this is the first row for a given school year
        private bool IsFirstRowOfSchoolYear(int rowIndex)
        {
            if (rowIndex == 0) return true;

            // Get the school year ID for the current row
            int currentSchoolYearId = (int)SchoolYearListDatagrid.Rows[rowIndex].Cells["school_year_id"].Value;

            // Get the school year ID for the previous row
            int previousSchoolYearId = (int)SchoolYearListDatagrid.Rows[rowIndex - 1].Cells["school_year_id"].Value;

            // If they are different, this is the first row for this school year
            return currentSchoolYearId != previousSchoolYearId;
        }

        // Handle button clicks in the DataGridView
        private void SchoolYearListDatagrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = SchoolYearListDatagrid.Rows[e.RowIndex];
            int schoolYearId = (int)row.Cells["school_year_id"].Value;
            int semesterId = (int)row.Cells["semester_id"].Value;
            string schoolYearName = row.Cells["year_name"].Value.ToString();
            string semesterName = row.Cells["semester_name"].Value.ToString();

            // Handle Schedule button click
            if (e.ColumnIndex == scheduleButtonIndex)
            {
                SetSchedule(schoolYearId, semesterId, schoolYearName, semesterName);
                return;
            }

            // Only process Edit/Delete if this is the first row for this school year
            if (IsFirstRowOfSchoolYear(e.RowIndex))
            {
                // Handle Edit button click
                if (e.ColumnIndex == editButtonIndex)
                {
                    EditSchoolYear(schoolYearId, schoolYearName);
                    return;
                }

                // Handle Delete button click
                if (e.ColumnIndex == deleteButtonIndex)
                {
                    DeleteSchoolYear(schoolYearId, schoolYearName);
                    return;
                }
            }
        }

        // Set schedule for a school year and semester
        private void SetSchedule(int schoolYearId, int semesterId, string schoolYearName, string semesterName)
        {
            // Create a custom form with DateTimePicker
            using (var scheduleForm = new Form())
            {
                scheduleForm.Text = $"Set Schedule for {schoolYearName} - {semesterName}";
                scheduleForm.Size = new Size(400, 200);
                scheduleForm.StartPosition = FormStartPosition.CenterParent;
                scheduleForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                scheduleForm.MaximizeBox = false;
                scheduleForm.MinimizeBox = false;

                // Create label
                var label = new Label
                {
                    Text = "Select start date and time:",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10)
                };

                // Create DateTimePicker
                var picker = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "MM/dd/yyyy hh:mm tt",
                    ShowUpDown = true,
                    Location = new Point(20, 50),
                    Size = new Size(200, 30),
                    Font = new Font("Segoe UI", 10)
                };

                // Set current value if one exists
                DataRow currentRow = null;
                foreach (DataRow row in schoolYearTable.Rows)
                {
                    if ((int)row["school_year_id"] == schoolYearId && (int)row["semester_id"] == semesterId)
                    {
                        currentRow = row;
                        break;
                    }
                }

                if (currentRow != null && !currentRow.IsNull("schedule_date"))
                {
                    picker.Value = (DateTime)currentRow["schedule_date"];
                }
                else
                {
                    picker.Value = DateTime.Now;
                }

                // Create buttons
                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(130, 100),
                    Font = new Font("Segoe UI", 10)
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(230, 100),
                    Font = new Font("Segoe UI", 10)
                };

                // Add controls to form
                scheduleForm.Controls.Add(label);
                scheduleForm.Controls.Add(picker);
                scheduleForm.Controls.Add(okButton);
                scheduleForm.Controls.Add(cancelButton);

                // Show the form
                if (scheduleForm.ShowDialog() == DialogResult.OK)
                {
                    // Save the selected date to the database
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Check if schedule table exists
                            string tableCheckQuery = "SELECT OBJECT_ID('academic_schedules', 'U')";
                            using (var checkCmd = new SqlCommand(tableCheckQuery, connection))
                            {
                                object tableId = checkCmd.ExecuteScalar();

                                // Create the table if it doesn't exist
                                if (tableId == null || Convert.IsDBNull(tableId))
                                {
                                    string createTableQuery = @"
                                        CREATE TABLE academic_schedules (
                                            schedule_id INT IDENTITY(1,1) PRIMARY KEY,
                                            school_year_id INT,
                                            semester_id INT,
                                            schedule_date DATETIME,
                                            created_at DATETIME DEFAULT GETDATE(),
                                            CONSTRAINT FK_SchoolYear FOREIGN KEY (school_year_id) REFERENCES school_years(school_year_id),
                                            CONSTRAINT FK_Semester FOREIGN KEY (semester_id) REFERENCES semesters(semester_id),
                                            CONSTRAINT UQ_SchoolYear_Semester UNIQUE (school_year_id, semester_id)
                                        )
                                    ";
                                    using (var createCmd = new SqlCommand(createTableQuery, connection))
                                    {
                                        createCmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Now insert or update the schedule
                            string upsertQuery = @"
                                MERGE academic_schedules AS target
                                USING (SELECT @schoolYearId, @semesterId, @scheduleDate) AS source (school_year_id, semester_id, schedule_date)
                                ON target.school_year_id = source.school_year_id AND target.semester_id = source.semester_id
                                WHEN MATCHED THEN
                                    UPDATE SET schedule_date = source.schedule_date
                                WHEN NOT MATCHED THEN
                                    INSERT (school_year_id, semester_id, schedule_date)
                                    VALUES (source.school_year_id, source.semester_id, source.schedule_date);
                            ";

                            using (var upsertCmd = new SqlCommand(upsertQuery, connection))
                            {
                                upsertCmd.Parameters.AddWithValue("@schoolYearId", schoolYearId);
                                upsertCmd.Parameters.AddWithValue("@semesterId", semesterId);
                                upsertCmd.Parameters.AddWithValue("@scheduleDate", picker.Value);
                                upsertCmd.ExecuteNonQuery();
                            }

                            // Update the display
                            foreach (DataRow row in schoolYearTable.Rows)
                            {
                                if ((int)row["school_year_id"] == schoolYearId && (int)row["semester_id"] == semesterId)
                                {
                                    row["schedule_date"] = picker.Value;
                                    break;
                                }
                            }

                            SchoolYearListDatagrid.Refresh();
                            MessageBox.Show("Schedule updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating schedule: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Edit a school year
        private void EditSchoolYear(int schoolYearId, string currentSchoolYearName)
        {
            // Create a form to edit the school year name
            using (var editForm = new Form())
            {
                editForm.Text = "Edit School Year";
                editForm.Size = new Size(400, 200);
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;

                // Create label
                var label = new Label
                {
                    Text = "Edit School Year (format yyyy-yyyy):",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10)
                };

                // Create TextBox
                var textBox = new TextBox
                {
                    Text = currentSchoolYearName,
                    Location = new Point(20, 50),
                    Size = new Size(200, 30),
                    Font = new Font("Segoe UI", 10)
                };

                // Create buttons
                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(130, 100),
                    Font = new Font("Segoe UI", 10)
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(230, 100),
                    Font = new Font("Segoe UI", 10)
                };

                // Add controls to form
                editForm.Controls.Add(label);
                editForm.Controls.Add(textBox);
                editForm.Controls.Add(okButton);
                editForm.Controls.Add(cancelButton);

                // Show the form
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    string newSchoolYearName = textBox.Text.Trim();

                    // Validate the format
                    if (!IsValidSchoolYearFormat(newSchoolYearName))
                    {
                        MessageBox.Show("Please enter a valid school year in the format 'yyyy-yyyy' (e.g., 2023-2024)",
                                       "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Update in the database
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Check if the new name already exists but with a different ID
                            string checkQuery = "SELECT COUNT(*) FROM school_years WHERE year_name = @yearName AND school_year_id <> @yearId";
                            using (var checkCmd = new SqlCommand(checkQuery, connection))
                            {
                                checkCmd.Parameters.AddWithValue("@yearName", newSchoolYearName);
                                checkCmd.Parameters.AddWithValue("@yearId", schoolYearId);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (count > 0)
                                {
                                    MessageBox.Show("A school year with this name already exists.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }

                            // Update the school year
                            string updateQuery = "UPDATE school_years SET year_name = @yearName WHERE school_year_id = @yearId";
                            using (var updateCmd = new SqlCommand(updateQuery, connection))
                            {
                                updateCmd.Parameters.AddWithValue("@yearName", newSchoolYearName);
                                updateCmd.Parameters.AddWithValue("@yearId", schoolYearId);
                                updateCmd.ExecuteNonQuery();
                            }

                            // Update all rows in the grid with this school year ID
                            foreach (DataRow row in schoolYearTable.Rows)
                            {
                                if ((int)row["school_year_id"] == schoolYearId)
                                {
                                    row["year_name"] = newSchoolYearName;
                                }
                            }

                            SchoolYearListDatagrid.Refresh();
                            MessageBox.Show("School year updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Also refresh the combobox
                            LoadSchoolYears();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating school year: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Delete a school year
        private void DeleteSchoolYear(int schoolYearId, string schoolYearName)
        {
            // Ask for confirmation
            DialogResult result = MessageBox.Show($"Are you sure you want to delete the school year '{schoolYearName}'? " +
                                              $"This will also delete all associated schedules.",
                                             "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
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
                                // Check if this is the current school year
                                string currentYearCheckQuery = "SELECT current_school_year_id FROM system_configuration";
                                using (var checkCmd = new SqlCommand(currentYearCheckQuery, connection, transaction))
                                {
                                    object currentYearId = checkCmd.ExecuteScalar();

                                    if (currentYearId != null && !Convert.IsDBNull(currentYearId))
                                    {
                                        if (Convert.ToInt32(currentYearId) == schoolYearId)
                                        {
                                            throw new Exception("Cannot delete the current school year. Please set a different school year as current first.");
                                        }
                                    }
                                }

                                // Check if academic_schedules table exists
                                string scheduleTableExistsQuery = "SELECT OBJECT_ID('academic_schedules', 'U')";
                                using (var tableCheckCmd = new SqlCommand(scheduleTableExistsQuery, connection, transaction))
                                {
                                    object tableId = tableCheckCmd.ExecuteScalar();

                                    if (tableId != null && !Convert.IsDBNull(tableId))
                                    {
                                        // Delete associated schedules from academic_schedules
                                        string deleteSchedulesQuery = "DELETE FROM academic_schedules WHERE school_year_id = @yearId";
                                        using (var deleteSchedulesCmd = new SqlCommand(deleteSchedulesQuery, connection, transaction))
                                        {
                                            deleteSchedulesCmd.Parameters.AddWithValue("@yearId", schoolYearId);
                                            deleteSchedulesCmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                // Now delete the school year
                                string deleteQuery = "DELETE FROM school_years WHERE school_year_id = @yearId";
                                using (var deleteCmd = new SqlCommand(deleteQuery, connection, transaction))
                                {
                                    deleteCmd.Parameters.AddWithValue("@yearId", schoolYearId);
                                    deleteCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                MessageBox.Show("School year deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Refresh the grid and combobox
                                LoadSchoolYears();
                                LoadSchoolYearData();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting school year: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
