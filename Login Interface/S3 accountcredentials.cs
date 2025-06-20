using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;
using CMS_Revised.User_Interface;
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
            ShowHidePassButton.Click += ShowHidePassButton_Click;
            Emailtextbox.ReadOnly = false; // Allow manual entry for email
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(Usernametextbox, "juan.delacruz");
            SetPlaceholder(Emailtextbox, "your.email@gmail.com");
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
            // If UserEmail is set (e.g., from Google), prefill and lock the email field
            if (!string.IsNullOrWhiteSpace(UserEmail))
            {
                Emailtextbox.Text = UserEmail;
                Emailtextbox.ReadOnly = true;
                Emailtextbox.TabStop = false;
                Emailtextbox.BackColor = Color.LightGray;
                Emailtextbox.ForeColor = Color.DimGray;
                Emailtextbox.Cursor = Cursors.Default;
            }
            else
            {
                Emailtextbox.ReadOnly = false;
                Emailtextbox.TabStop = true;
                Emailtextbox.BackColor = Color.White;
                Emailtextbox.ForeColor = Color.Black;
                Emailtextbox.Cursor = Cursors.IBeam;
            }

            PasswordNoticelabel.Text = "Password must be at least 6 characters.";
            PasswordNoticelabel.ForeColor = Color.Gray;
            Passwordtextbox.UseSystemPasswordChar = true;
            ConfirmPasswordtextbox.UseSystemPasswordChar = true;
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
            if (Regex.IsMatch(password, "[A-Z]")) score++;
            if (Regex.IsMatch(password, "[a-z]")) score++;
            if (Regex.IsMatch(password, "[0-9]")) score++;
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) score++;

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
                MessageBox.Show("Please fill out all fields to continue.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string email = GetTextOrEmpty(Emailtextbox);

            // Email validation
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                MessageBox.Show("Please enter a valid @gmail.com email address.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Email uniqueness check
            if (!await IsEmailUniqueAsync(email))
            {
                MessageBox.Show("This email is already registered.", "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Passwordtextbox.Text != ConfirmPasswordtextbox.Text)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation dialog
            var summary = $"Username: {Usernametextbox.Text}\nEmail: {email}\n\nFinalize signup with these credentials?";
            var result = MessageBox.Show(summary, "Confirm Signup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var info = new AccountCredentialsInfo
            {
                Username = GetTextOrEmpty(Usernametextbox),
                Email = email,
                Password = Passwordtextbox.Text // Will be hashed before saving
            };

            await SaveAccountCredentialsAsync(info);

            FinalizeClicked?.Invoke(this, new AccountCredentialsEventArgs(info));
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Usernametextbox.Text) || Usernametextbox.Text == _placeholders[Usernametextbox])
                return false;
            if (string.IsNullOrWhiteSpace(Emailtextbox.Text) || Emailtextbox.Text == _placeholders[Emailtextbox])
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

        private async Task<bool> IsEmailUniqueAsync(string email)
        {
            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM users WHERE email = @Email AND user_id <> @UserId", conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
            int count = (int)await cmd.ExecuteScalarAsync();
            return count == 0;
        }

        private async Task SaveAccountCredentialsAsync(AccountCredentialsInfo info)
        {
            if (CurrentUserId <= 0) return;

            // Use PasswordHelper for PBKDF2 hashing
            string passwordHash = PasswordHelper.HashPassword(info.Password);

            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            using (var cmd = new SqlCommand(@"
                UPDATE users SET
                    username = @Username,
                    email = @Email,
                    password_hash = @PasswordHash,
                    is_signedup = 1,
                    updated_at = GETDATE()
                WHERE user_id = @UserId
            ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                cmd.Parameters.AddWithValue("@Username", info.Username);
                cmd.Parameters.AddWithValue("@Email", info.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@IsSignedUp", 1);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                Console.WriteLine($"Rows affected during signup: {rowsAffected}");
            }
        }

        private void ShowHidePassButton_Click(object? sender, EventArgs e)
        {
            bool isHidden = Passwordtextbox.UseSystemPasswordChar;
            Passwordtextbox.UseSystemPasswordChar = !isHidden;
            ConfirmPasswordtextbox.UseSystemPasswordChar = !isHidden;
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