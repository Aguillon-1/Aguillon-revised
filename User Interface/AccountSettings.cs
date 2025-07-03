using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMS_Revised.User_Interface
{
    public partial class AccountSettings : UserControl
    {
        private string _currentEmail = "";
        private string _currentPassword = "";
        private bool _isPasswordVisible = false;

        public AccountSettings()
        {
            InitializeComponent();
        }

        private void emaillabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EMAIL FIELD IS UNEDITABLE, contact administration for modification", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void passwordlabel_Click(object sender, EventArgs e)
        {

        }

        private void passwordtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void confirmpwtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            SetEditMode(true);
        }

        private void Discardbtn_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
        }

        private async void Savebtn_Click(object sender, EventArgs e)
        {
            if (passwordtxtbox.Text != confirmpwtxtbox.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(passwordtxtbox.Text))
            {
                MessageBox.Show("Password cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var conn = CMS_Revised.Connections.DatabaseConn.GetConnection();
            await conn.OpenAsync();
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                "UPDATE users SET password = @Password, updated_at = GETDATE() WHERE email = @Email", conn);
            cmd.Parameters.AddWithValue("@Password", passwordtxtbox.Text);
            cmd.Parameters.AddWithValue("@Email", _currentEmail);
            await cmd.ExecuteNonQueryAsync();

            _currentPassword = passwordtxtbox.Text;
            SetEditMode(false);
            MessageBox.Show("Password updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Seebutton_Click(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            passwordtxtbox.UseSystemPasswordChar = !_isPasswordVisible;
            confirmpwtxtbox.UseSystemPasswordChar = !_isPasswordVisible;
        }

        private void SetEditMode(bool enabled)
        {
            passwordtxtbox.ReadOnly = !enabled;
            confirmpwtxtbox.Visible = enabled;
            confirmpwtxtbox.ReadOnly = !enabled;
            Savebtn.Visible = enabled;
            Discardbtn.Visible = enabled;
            Editbtn.Enabled = !enabled;
            passwordlabel.Text = enabled ? "New Password" : "Password";
            if (!enabled)
            {
                passwordtxtbox.Text = _currentPassword;
                passwordtxtbox.UseSystemPasswordChar = true;
                confirmpwtxtbox.Text = "";
                _isPasswordVisible = false;
            }
        }

        public async void LoadAccountInfo(string email)
        {
            _currentEmail = email;
            using var conn = CMS_Revised.Connections.DatabaseConn.GetConnection();
            await conn.OpenAsync();
            using var cmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT email, password FROM users WHERE email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                emaillabel.Text = reader["email"]?.ToString() ?? "";
                _currentPassword = reader["password"]?.ToString() ?? "";
                passwordtxtbox.Text = _currentPassword;
            }
            passwordtxtbox.UseSystemPasswordChar = true;
            confirmpwtxtbox.Text = "";
            confirmpwtxtbox.UseSystemPasswordChar = true;
        }
    }
}
