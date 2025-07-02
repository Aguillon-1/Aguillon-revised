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

namespace CMS_Revised.User_Interface
{
    public partial class ProfileAccountSettings : UserControl
    {
        // Store original values for discard
        private string originalFirstName, originalMiddleName, originalLastName, originalAddress, originalBirthday, originalContactNo;

        public ProfileAccountSettings()
        {
            InitializeComponent();
            SetEditMode(false);
        }

        public void SetUserInfo(
            string name,
            string role,
            string studentNo,
            string firstName,
            string middleName,
            string lastName,
            string address,
            DateTime birthday,
            string contactNo)
        {
            // Set non-editable fields
            RoleLabel.Text = role;
            StudentNoLabel.Text = studentNo;

            // Set editable fields
            NameTextBox.Text = name;
            FirstNameTextBox.Text = firstName;
            MiddleNameTextBox.Text = middleName;
            LastNameTextBox.Text = lastName;
            AddressTextBox.Text = address;
            BirthdayTextBox.Text = birthday.ToString("yyyy-MM-dd");
            ContactNoTextBox.Text = contactNo;

            // Store originals for discard
            originalFirstName = firstName;
            originalMiddleName = middleName;
            originalLastName = lastName;
            originalAddress = address;
            originalBirthday = birthday.ToString("yyyy-MM-dd");
            originalContactNo = contactNo;
        }

        private void SetEditMode(bool enabled)
        {
            // Only editable fields
            NameTextBox.ReadOnly = !enabled;
            FirstNameTextBox.ReadOnly = !enabled;
            MiddleNameTextBox.ReadOnly = !enabled;
            LastNameTextBox.ReadOnly = !enabled;
            AddressTextBox.ReadOnly = !enabled;
            BirthdayTextBox.ReadOnly = !enabled;
            ContactNoTextBox.ReadOnly = !enabled;

            Savebtn.Visible = enabled;
            Discardbtn.Visible = enabled;
            editbutton.Enabled = !enabled;
        }

        private void editbutton_Click(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void Discardbtn_Click(object sender, EventArgs e)
        {
            // Revert to original values
            FirstNameTextBox.Text = originalFirstName;
            MiddleNameTextBox.Text = originalMiddleName;
            LastNameTextBox.Text = originalLastName;
            AddressTextBox.Text = originalAddress;
            BirthdayTextBox.Text = originalBirthday;
            ContactNoTextBox.Text = originalContactNo;

            SetEditMode(false);
        }

        private async void Savebtn_Click(object sender, EventArgs e)
        {
            // Save to database (update users table)
            using var conn = CMS_Revised.Connections.DatabaseConn.GetConnection();
            await conn.OpenAsync();

            using (var cmd = new SqlCommand(@"
                UPDATE users SET
                    first_name = @FirstName,
                    middle_name = @MiddleName,
                    last_name = @LastName,
                    address = @Address,
                    birthday = @Birthday,
                    contact_number = @ContactNo,
                    updated_at = GETDATE()
                WHERE student_id = @StudentNo
            ", conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", FirstNameTextBox.Text);
                cmd.Parameters.AddWithValue("@MiddleName", MiddleNameTextBox.Text);
                cmd.Parameters.AddWithValue("@LastName", LastNameTextBox.Text);
                cmd.Parameters.AddWithValue("@Address", AddressTextBox.Text);
                cmd.Parameters.AddWithValue("@Birthday", DateTime.Parse(BirthdayTextBox.Text));
                cmd.Parameters.AddWithValue("@ContactNo", ContactNoTextBox.Text);
                cmd.Parameters.AddWithValue("@StudentNo", StudentNoLabel.Text);

                await cmd.ExecuteNonQueryAsync();
            }

            // Update originals
            originalFirstName = FirstNameTextBox.Text;
            originalMiddleName = MiddleNameTextBox.Text;
            originalLastName = LastNameTextBox.Text;
            originalAddress = AddressTextBox.Text;
            originalBirthday = BirthdayTextBox.Text;
            originalContactNo = ContactNoTextBox.Text;

            SetEditMode(false);
            MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FirstNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MiddleNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void LastNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BirthdayTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ContactNoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddressTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ProfilePicturepanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
