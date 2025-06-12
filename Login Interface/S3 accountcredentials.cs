using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;
using Microsoft.Data.SqlClient;

namespace CMS_Revised.Login_Interface
{
    public partial class S3_accountcredentials : UserControl
    {
        // Placeholders for textboxes
        private readonly Dictionary<TextBox, string> _placeholders = new();

        public event EventHandler<AccountCredentialsEventArgs>? FinalizeClicked;
        public event EventHandler? BackClicked;

        public int CurrentUserId { get; set; } // Set this from LoginForm
        public string UserEmail { get; set; } = ""; // Set this from LoginForm

        public S3_accountcredentials()
        {
            InitializeComponent();
            InitializePlaceholders();
            Load += S3_accountcredentials_Load;
            S3Finalizebutton.Click += S3Finalizebutton_Click;
            S2Backbutton.Click += (s, e) => BackClicked?.Invoke(this, EventArgs.Empty);
            Passwordtextbox.TextChanged += Passwordtextbox_TextChanged;
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(Usernametextbox, "juan.delacruz");
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

        private void S3_accountcredentials_Load(object? sender, EventArgs e)
        {
            // Set email textbox (greyed out, uneditable, unselectable)
            Emailtextbox.Text = UserEmail;
            Emailtextbox.ReadOnly = true;
            Emailtextbox.TabStop = false;
            Emailtextbox.BackColor = Color.LightGray;
            Emailtextbox.ForeColor = Color.DimGray;
            Emailtextbox.Cursor = Cursors.Default;

            PasswordNoticelabel.Text = "Password must be at least 6 characters.";
            PasswordNoticelabel.ForeColor = Color.Gray;
        }

        private void Passwordtextbox_TextChanged(object? sender, EventArgs e)
        {
            string password = Passwordtextbox.Text;
            PasswordStrength strength = EvaluatePasswordStrength(password, out string warning);

            switch (strength)
            {
                case PasswordStrength.TooShort:
                    PasswordNoticelabel.Text = "Password too short (min 6 characters).";
                    PasswordNoticelabel.ForeColor = Color.Red;
                    break;
                case PasswordStrength.Weak:
                    PasswordNoticelabel.Text = "Weak password." + warning;
                    PasswordNoticelabel.ForeColor = Color.OrangeRed;
                    break;
                case PasswordStrength.Normal:
                    PasswordNoticelabel.Text = "Normal password." + warning;
                    PasswordNoticelabel.ForeColor = Color.Orange;
                    break;
                case PasswordStrength.Good:
                    PasswordNoticelabel.Text = "Good password." + warning;
                    PasswordNoticelabel.ForeColor = Color.Green;
                    break;
                case PasswordStrength.Strong:
                    PasswordNoticelabel.Text = "Strong password!" + warning;
                    PasswordNoticelabel.ForeColor = Color.DarkGreen;
                    break;
                case PasswordStrength.Extreme:
                    PasswordNoticelabel.Text = "Extreme password!" + warning;
                    PasswordNoticelabel.ForeColor = Color.BlueViolet;
                    break;
            }
        }

        private PasswordStrength EvaluatePasswordStrength(string password, out string warning)
        {
            warning = "";
            if (password.Length < 6)
                return PasswordStrength.TooShort;

            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[^a-zA-Z0-9]")) score++;

            // Check for common passwords (simple list)
            string[] common = { "password", "123456", "qwerty", "letmein", "admin", "welcome" };
            foreach (var c in common)
            {
                if (password.Equals(c, StringComparison.OrdinalIgnoreCase))
                {
                    warning = " (This password is too common!)";
                    return PasswordStrength.Weak;
                }
            }

            if (score <= 2) return PasswordStrength.Weak;
            if (score == 3) return PasswordStrength.Normal;
            if (score == 4) return PasswordStrength.Good;
            if (score == 5) return PasswordStrength.Strong;
            if (score >= 6) return PasswordStrength.Extreme;
            return PasswordStrength.Weak;
        }

        private async void S3Finalizebutton_Click(object? sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                MessageBox.Show("This field is not filled up yet, please fill out to continue...", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Passwordtextbox.Text != ConfirmPasswordtextbox.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation dialog
            var summary = $"Username: {Usernametextbox.Text}\nEmail: {Emailtextbox.Text}\n\nFinalize signup with these credentials?";
            var result = MessageBox.Show(summary, "Confirm Signup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var info = new AccountCredentialsInfo
            {
                Username = GetTextOrEmpty(Usernametextbox),
                Email = Emailtextbox.Text,
                Password = Passwordtextbox.Text // Will be hashed before saving
            };

            await SaveAccountCredentialsAsync(info);

            FinalizeClicked?.Invoke(this, new AccountCredentialsEventArgs(info));
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Usernametextbox.Text) || Usernametextbox.Text == _placeholders[Usernametextbox])
                return false;
            if (string.IsNullOrWhiteSpace(Passwordtextbox.Text))
                return false;
            if (string.IsNullOrWhiteSpace(ConfirmPasswordtextbox.Text))
                return false;
            return true;
        }

        private string GetTextOrEmpty(TextBox textBox)
        {
            return (_placeholders.TryGetValue(textBox, out var placeholder) && textBox.Text == placeholder) ? "" : textBox.Text.Trim();
        }

        private async Task SaveAccountCredentialsAsync(AccountCredentialsInfo info)
        {
            if (CurrentUserId <= 0) return;

            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            // Hash password (SHA256 for demo, use a better hash in production)
            string passwordHash = HashPassword(info.Password);

            using (var cmd = new SqlCommand(@"
UPDATE users SET
    username = @Username,
    password_hash = @PasswordHash,
    is_signedup = 1,
    updated_at = GETDATE()
WHERE user_id = @UserId
", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                cmd.Parameters.AddWithValue("@Username", info.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private enum PasswordStrength
        {
            TooShort,
            Weak,
            Normal,
            Good,
            Strong,
            Extreme
        }
    }

    public class AccountCredentialsInfo
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AccountCredentialsEventArgs : EventArgs
    {
        public AccountCredentialsInfo Info { get; }
        public AccountCredentialsEventArgs(AccountCredentialsInfo info) => Info = info;
    }
}