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
using CMS_Revised.Connections;

namespace CMS_Revised.User_Interface
{
    public partial class UserAccountControl : UserControl
    {
        // Keep references to the controls so you don't create multiple instances
        private ProfileAccountSettings profileAccountSettings;
        private AccountSettings accountSettings;

        public UserAccountControl()
        {
            InitializeComponent();

            // Initialize the user controls (but don't add them yet)
            profileAccountSettings = new ProfileAccountSettings();
            accountSettings = new AccountSettings();

            // Optionally dock them to fill the parent
            profileAccountSettings.Dock = DockStyle.Fill;
            accountSettings.Dock = DockStyle.Fill;

            // Show ProfileAccountSettings by default
            ShowControl(profileAccountSettings);
        }

        private void ShowControl(UserControl control)
        {
            // Remove any existing controls from the content panel
            contentPanel.Controls.Clear();

            // Add the requested control to the content panel
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            control.BringToFront();
        }

        public async Task LoadUserProfileAsync(int userId)
        {
            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            string name = "", role = "", studentNo = "", firstName = "", middleName = "", lastName = "", address = "", contactNo = "";
            DateTime birthday = DateTime.MinValue;

            using (var cmd = new SqlCommand(@"
                SELECT u.first_name, u.middle_name, u.last_name, u.user_type, sp.student_id, u.suffix, u.birthday, u.address, u.contact_number
                FROM users u
                LEFT JOIN student_profiles sp ON u.user_id = sp.user_id
                WHERE u.user_id = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    firstName = reader["first_name"]?.ToString() ?? "";
                    middleName = reader["middle_name"]?.ToString() ?? "";
                    lastName = reader["last_name"]?.ToString() ?? "";
                    role = reader["user_type"]?.ToString() ?? "";
                    studentNo = reader["student_id"]?.ToString() ?? "";
                    address = reader["address"]?.ToString() ?? "";
                    contactNo = reader["contact_number"]?.ToString() ?? "";
                    birthday = reader["birthday"] != DBNull.Value ? Convert.ToDateTime(reader["birthday"]) : DateTime.MinValue;
                    name = $"{firstName} {middleName} {lastName}".Trim();
                }
            }

            // Make sure this matches the name of your ProfileAccountSettings control in the designer
            profileAccountSettings1.SetUserInfo(
                name, role, studentNo, firstName, middleName, lastName, address, birthday, contactNo
            );
        }

        private void personalinfobtn_Click(object sender, EventArgs e)
        {
            ShowControl(profileAccountSettings);
        }

        private void accountsettingsbtn_Click(object sender, EventArgs e)
        {
            ShowControl(accountSettings);
        }

        private void contentPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
