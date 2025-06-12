using CMS_Revised.Connections;
using CMS_Revised.User_Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMS_Revised.Login_Interface
{
    public partial class S1_studentinfo : UserControl
    {
        // Placeholders for textboxes
        private readonly Dictionary<TextBox, string> _placeholders = new();

        public event EventHandler<StudentInfoEventArgs>? NextClicked;
        public event EventHandler? CancelClicked;

        public int CurrentUserId { get; set; } // Set this from LoginForm

        public S1_studentinfo()
        {
            InitializeComponent();
            InitializePlaceholders();
            SetComboBoxDropDownList();
            Load += S1_studentinfo_Load;
            Programcombobox.SelectedIndexChanged += ProgramOrYearLevel_Changed;
            Yearlevelcombobox.SelectedIndexChanged += ProgramOrYearLevel_Changed;
            S1Nextbutton.Click += S1Nextbutton_Click;
            S1Cancelbutton.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);


        }

        private async void ProgramOrYearLevel_Changed(object? sender, EventArgs e)
        {
            await PopulateSectionsAsync();
        }

        public void PreFillNamesIfEmpty(string firstName, string middleName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(Fnametextbox.Text) || Fnametextbox.ForeColor == Color.Gray)
            {
                Fnametextbox.Text = firstName;
                Fnametextbox.ForeColor = Color.Black;
            }
            if (string.IsNullOrWhiteSpace(Mnametextbox.Text) || Mnametextbox.ForeColor == Color.Gray)
            {
                Mnametextbox.Text = middleName;
                Mnametextbox.ForeColor = Color.Black;
            }
            if (string.IsNullOrWhiteSpace(Lnametextbox.Text) || Lnametextbox.ForeColor == Color.Gray)
            {
                Lnametextbox.Text = lastName;
                Lnametextbox.ForeColor = Color.Black;
            }
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(Fnametextbox, "Juan");
            SetPlaceholder(Mnametextbox, "Santos");
            SetPlaceholder(Lnametextbox, "Dela Cruz");
            SetPlaceholder(Studentnotextbox, "2024XXXX-N");
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            _placeholders[textBox] = placeholder;
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };
            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void SetComboBoxDropDownList()
        {
            Programcombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            Yearlevelcombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            Sectioncombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            Studentstatuscombobox.DropDownStyle = ComboBoxStyle.DropDownList;
            Suffixnamecombobox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private async void S1_studentinfo_Load(object? sender, EventArgs e)
        {
            // Suffix
            Suffixnamecombobox.Items.Clear();
            Suffixnamecombobox.Items.Add("Select suffix");
            Suffixnamecombobox.Items.Add("N/A");
            Suffixnamecombobox.Items.AddRange(new object[] { "Jr.", "Sr.", "II", "III", "IV", "V" });
            if (Suffixnamecombobox.Items.Count > 0)
                Suffixnamecombobox.SelectedIndex = 0;

            // Student status
            Studentstatuscombobox.Items.Clear();
            Studentstatuscombobox.Items.Add("Select student status");
            Studentstatuscombobox.Items.Add("Regular");
            Studentstatuscombobox.Items.Add("Irregular");
            if (Studentstatuscombobox.Items.Count > 1)
                Studentstatuscombobox.SelectedIndex = 1; // Default to "Regular"

            await PopulateProgramsAsync();
            await PopulateYearLevelsAsync();
            await PopulateSectionsAsync();

            // Load saved data if any
            await LoadSavedStudentInfoAsync();
        }

        private async Task PopulateProgramsAsync()
        {
            Programcombobox.Items.Clear();
            Programcombobox.Items.Add("Select program");
            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT program_id, program_name FROM programs WHERE is_active = 1", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Programcombobox.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
            }
            if (Programcombobox.Items.Count > 0)
                Programcombobox.SelectedIndex = 0;
        }

        private async Task PopulateYearLevelsAsync()
        {
            Yearlevelcombobox.Items.Clear();
            Yearlevelcombobox.Items.Add("Select year level");
            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT year_level_id, year_name FROM year_levels WHERE is_active = 1", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Yearlevelcombobox.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
            }
            if (Yearlevelcombobox.Items.Count > 0)
                Yearlevelcombobox.SelectedIndex = 0;
        }

        private async Task PopulateSectionsAsync()
        {
            Sectioncombobox.Items.Clear();
            Sectioncombobox.Items.Add("Select section");
            if (Programcombobox.SelectedIndex > 0 && Yearlevelcombobox.SelectedIndex > 0 &&
                Programcombobox.SelectedItem is ComboBoxItem program &&
                Yearlevelcombobox.SelectedItem is ComboBoxItem yearLevel)
            {
                using var conn = DatabaseConn.GetConnection();
                await conn.OpenAsync();
                using var cmd = new SqlCommand(
                    "SELECT section_id, section_name FROM sections WHERE program_id = @ProgramId AND year_level_id = @YearLevelId AND is_active = 1",
                    conn);
                cmd.Parameters.AddWithValue("@ProgramId", program.Id);
                cmd.Parameters.AddWithValue("@YearLevelId", yearLevel.Id);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Sectioncombobox.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            if (Sectioncombobox.Items.Count > 0)
                Sectioncombobox.SelectedIndex = 0;
        }

        private async void S1Nextbutton_Click(object? sender, EventArgs e)
        {
            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(Fnametextbox.Text) || Fnametextbox.Text == _placeholders[Fnametextbox])
                missingFields.Add("First Name");
            if (string.IsNullOrWhiteSpace(Mnametextbox.Text) || Mnametextbox.Text == _placeholders[Mnametextbox])
                missingFields.Add("Middle Name");
            if (string.IsNullOrWhiteSpace(Lnametextbox.Text) || Lnametextbox.Text == _placeholders[Lnametextbox])
                missingFields.Add("Last Name");
            if (string.IsNullOrWhiteSpace(Studentnotextbox.Text) || Studentnotextbox.Text == _placeholders[Studentnotextbox])
                missingFields.Add("Student Number");
            if (Programcombobox.SelectedIndex == 0)
                missingFields.Add("Program");
            if (Yearlevelcombobox.SelectedIndex == 0)
                missingFields.Add("Year Level");
            if (Sectioncombobox.SelectedIndex == 0)
                missingFields.Add("Section");
            if (Studentstatuscombobox.SelectedIndex == 0)
                missingFields.Add("Student Status");

            if (missingFields.Count > 0)
            {
                MessageBox.Show("Please fill out the following fields:\n- " + string.Join("\n- ", missingFields), "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prepare data
            var info = new StudentSignupInfo
            {
                FirstName = GetTextOrEmpty(Fnametextbox),
                MiddleName = GetTextOrEmpty(Mnametextbox),
                LastName = GetTextOrEmpty(Lnametextbox),
                Suffix = Suffixnamecombobox.Text,
                StudentNumber = GetTextOrEmpty(Studentnotextbox),
                ProgramId = (Programcombobox.SelectedItem as ComboBoxItem)?.Id ?? 0,
                YearLevelId = (Yearlevelcombobox.SelectedItem as ComboBoxItem)?.Id ?? 0,
                SectionId = (Sectioncombobox.SelectedItem as ComboBoxItem)?.Id ?? 0,
                StudentStatus = Studentstatuscombobox.Text
            };

            // Save to DB (partial, is_signedup = 0)
            await SavePartialStudentInfoAsync(info);

            // Raise event to parent (LoginForm) to proceed to next step
            NextClicked?.Invoke(this, new StudentInfoEventArgs(info));
        }

        private void SetComboBoxSelectedById(ComboBox comboBox, int id)
        {
            if (comboBox.Items.Count <= 1) // Only placeholder or empty
                return;
            for (int i = 1; i < comboBox.Items.Count; i++) // skip placeholder
            {
                if (comboBox.Items[i] is ComboBoxItem item && item.Id == id)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
            // If not found, default to placeholder if available
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        public async Task LoadSavedStudentInfoAsync()
        {
            if (CurrentUserId <= 0) return;

            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            // 1. Load names and suffix from users table
            using (var cmd = new SqlCommand(@"
        SELECT first_name, middle_name, last_name, suffix
        FROM users WHERE user_id = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Fnametextbox.Text = reader["first_name"]?.ToString() ?? "";
                    Fnametextbox.ForeColor = string.IsNullOrWhiteSpace(Fnametextbox.Text) ? Color.Gray : Color.Black;
                    Mnametextbox.Text = reader["middle_name"]?.ToString() ?? "";
                    Mnametextbox.ForeColor = string.IsNullOrWhiteSpace(Mnametextbox.Text) ? Color.Gray : Color.Black;
                    Lnametextbox.Text = reader["last_name"]?.ToString() ?? "";
                    Lnametextbox.ForeColor = string.IsNullOrWhiteSpace(Lnametextbox.Text) ? Color.Gray : Color.Black;

                    string suffix = reader["suffix"]?.ToString() ?? "";
                    if (Suffixnamecombobox.Items.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(suffix) || suffix == "Select suffix")
                            Suffixnamecombobox.SelectedIndex = 0;
                        else
                        {
                            int idx = Suffixnamecombobox.Items.IndexOf(suffix == "N/A" ? "N/A" : suffix);
                            Suffixnamecombobox.SelectedIndex = idx >= 0 ? idx : 0;
                        }
                    }
                }
            }

            // 2. Load student profile from student_profiles table
            int? programId = null, yearLevelId = null, sectionId = null;
            string? status = null;
            using (var cmd = new SqlCommand(@"
        SELECT student_id, program_id, year_level_id, section_id, student_status
        FROM student_profiles WHERE user_id = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Studentnotextbox.Text = reader["student_id"]?.ToString() ?? "";
                    Studentnotextbox.ForeColor = string.IsNullOrWhiteSpace(Studentnotextbox.Text) ? Color.Gray : Color.Black;
                    programId = reader["program_id"] as int? ?? 0;
                    yearLevelId = reader["year_level_id"] as int? ?? 0;
                    sectionId = reader["section_id"] as int? ?? 0;
                    status = reader["student_status"]?.ToString() ?? "Regular";
                }
            }

            // Set program and year level first
            if (Programcombobox.Items.Count > 1 && programId > 0)
                SetComboBoxSelectedById(Programcombobox, programId.Value);
            if (Yearlevelcombobox.Items.Count > 1 && yearLevelId > 0)
                SetComboBoxSelectedById(Yearlevelcombobox, yearLevelId.Value);

            // Now repopulate sections for the selected program/year level
            await PopulateSectionsAsync();

            // Now set the section
            if (Sectioncombobox.Items.Count > 1 && sectionId > 0)
                SetComboBoxSelectedById(Sectioncombobox, sectionId.Value);

            // Set student status
            if (Studentstatuscombobox.Items.Count > 1 && status != null)
            {
                int statusIdx = Studentstatuscombobox.Items.IndexOf(status);
                Studentstatuscombobox.SelectedIndex = statusIdx > 0 ? statusIdx : 1; // Default to "Regular"
            }
        }

        private int _savedProgramId, _savedYearLevelId, _savedSectionId;

        private bool ValidateFields()
        {
            // Check if any textbox is empty or still has placeholder
            foreach (var pair in _placeholders)
            {
                if (string.IsNullOrWhiteSpace(pair.Key.Text) || pair.Key.Text == pair.Value)
                    return false;
            }
            if (Programcombobox.SelectedIndex < 0 || Yearlevelcombobox.SelectedIndex < 0 || Sectioncombobox.SelectedIndex < 0)
                return false;
            if (string.IsNullOrWhiteSpace(Studentstatuscombobox.Text))
                return false;
            return true;
        }

        private string GetTextOrEmpty(TextBox textBox)
        {
            return (_placeholders.TryGetValue(textBox, out var placeholder) && textBox.Text == placeholder) ? "" : textBox.Text.Trim();
        }

        private async Task SavePartialStudentInfoAsync(StudentSignupInfo info)
        {
            if (CurrentUserId <= 0) return;

            string suffixToSave = (Suffixnamecombobox.SelectedIndex <= 0) ? null :
                (Suffixnamecombobox.SelectedItem?.ToString() == "N/A" ? "N/A" : Suffixnamecombobox.SelectedItem?.ToString());

            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            // 1. Update users table for names and suffix
            using (var cmd = new SqlCommand(@"
        UPDATE users SET
            first_name = @FirstName,
            middle_name = @MiddleName,
            last_name = @LastName,
            suffix = @Suffix,
            updated_at = GETDATE()
        WHERE user_id = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                cmd.Parameters.AddWithValue("@FirstName", info.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MiddleName", info.MiddleName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", info.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Suffix", (object?)suffixToSave ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            // 2. Upsert student_profiles for the rest
            using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM student_profiles WHERE user_id = @UserId)
    UPDATE student_profiles SET
        student_id = @StudentNumber,
        program_id = @ProgramId,
        year_level_id = @YearLevelId,
        section_id = @SectionId,
        student_status = @StudentStatus,
        updated_at = GETDATE()
    WHERE user_id = @UserId
ELSE
    INSERT INTO student_profiles (user_id, student_id, program_id, year_level_id, section_id, student_status, created_at)
    VALUES (@UserId, @StudentNumber, @ProgramId, @YearLevelId, @SectionId, @StudentStatus, GETDATE())
", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                cmd.Parameters.AddWithValue("@StudentNumber", info.StudentNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProgramId", info.ProgramId);
                cmd.Parameters.AddWithValue("@YearLevelId", info.YearLevelId);
                cmd.Parameters.AddWithValue("@SectionId", info.SectionId);
                cmd.Parameters.AddWithValue("@StudentStatus", info.StudentStatus ?? (object)DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            // StatusDialog update
            StatusDialog.ShowStatusDialog().UpdateStatus(
                "Pre-saved to the database:\r\n" +
                $"- First Name: {info.FirstName}\r\n" +
                $"- Middle Name: {info.MiddleName}\r\n" +
                $"- Last Name: {info.LastName}\r\n" +
                $"- Suffix: {suffixToSave}\r\n" +
                $"- Student Number: {info.StudentNumber}\r\n" +
                $"- Program ID: {info.ProgramId}\r\n" +
                $"- Year Level ID: {info.YearLevelId}\r\n" +
                $"- Section ID: {info.SectionId}\r\n" +
                $"- Student Status: {info.StudentStatus}"
);
        }

        // Helper class for ComboBox items
        private class ComboBoxItem
        {
            public int Id { get; }
            public string Name { get; }
            public ComboBoxItem(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public override string ToString() => Name;
        }
    }

    // Data transfer object for student info
    public class StudentSignupInfo
    {
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Suffix { get; set; } = "";
        public string StudentNumber { get; set; } = "";
        public int ProgramId { get; set; }
        public int YearLevelId { get; set; }
        public int SectionId { get; set; }
        public string StudentStatus { get; set; } = "";
    }

    public class StudentInfoEventArgs : EventArgs
    {
        public StudentSignupInfo Info { get; }
        public StudentInfoEventArgs(StudentSignupInfo info) => Info = info;
    }


}