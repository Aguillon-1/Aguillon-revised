using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;
using Microsoft.Data.SqlClient;
using System.IO;
using CMS_Revised.User_Interface;

namespace CMS_Revised
{
    public partial class LoginControl : UserControl
    {
        // Path to store logout state
        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public LoginControl()
        {
            InitializeComponent();
            GoogleButton.Click += GoogleButton_Click;
            LoginButton.Click += LoginButton_Click;
            ShowHidePassButton.Click += ShowHidePassButton_Click;
            PasswordTextbox.UseSystemPasswordChar = true;
            ShowHidePassButton.Text = "👁";
        }

        private void GoogleButton_Click(object? sender, EventArgs e)
        {
            var statusDialog = StatusDialog.ShowStatusDialog();
            _ = RunLoginProcessAsync(statusDialog);
        }

        private void LoginForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Prevent the default close behavior only if MainForm is already open
            if (Application.OpenForms.OfType<MainForm>().Any())
            {
                // Don't exit the application, just close this form
                e.Cancel = false; // Allow the form to close
            }
        }

        private async Task RunLoginProcessAsync(StatusDialog statusDialog)
        {
            try
            {
                statusDialog.UpdateStatus("Login initiated...");

                bool forceNewLogin = File.Exists(LogoutFlagPath);
                if (forceNewLogin)
                {
                    statusDialog.UpdateStatus("Previous logout detected. Clearing any saved credentials...");
                    await GAuthclass.ClearGoogleCredentialsAsync();
                    try
                    {
                        File.Delete(LogoutFlagPath);
                        statusDialog.UpdateStatus("Ready for new account login.");
                    }
                    catch
                    {
                        statusDialog.UpdateStatus("Warning: Could not clear logout flag.");
                    }
                }

                statusDialog.UpdateStatus("Opening browser for Google login...");

                var gauth = new GAuthclass();
                var userInfo = await gauth.GetGoogleUserInfoAsync((msg) =>
                {
                    statusDialog.UpdateStatus(msg);

                    // Bring LoginForm to front after "Google authentication complete."
                    if (msg.StartsWith("Google authentication complete", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (Form form in Application.OpenForms)
                        {
                            if (form is LoginForm)
                            {
                                if (form.IsHandleCreated)
                                {
                                    form.Invoke(new Action(() =>
                                    {
                                        form.TopMost = true;
                                        form.BringToFront();
                                        form.Activate();
                                        form.TopMost = false;
                                    }));
                                }
                                break;
                            }
                        }
                    }
                });

                statusDialog.UpdateStatus("Authenticated successfully...");
                statusDialog.UpdateStatus("Fetching user details...");
                statusDialog.UpdateStatus($"Email: {userInfo.Email}");
                statusDialog.UpdateStatus($"Name: {userInfo.Name}");

                // Split name into first, middle, last
                string[] nameParts = userInfo.Name.Split(' ');
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                statusDialog.UpdateStatus("Checking if user already exists in the database...");
                bool userExists = false;
                int userId = -1;
                string userType = "Student"; // Default user type
                int isSignedUp = 0; // Default signup status

                using (var conn = DatabaseConn.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT user_id, user_type, first_name, middle_name, last_name, is_signedup FROM users WHERE email = @Email", conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                userExists = true;
                                userId = reader.GetInt32(0);
                                userType = reader["user_type"]?.ToString() ?? "Student";
                                firstName = reader["first_name"]?.ToString() ?? firstName;
                                middleName = reader["middle_name"]?.ToString() ?? middleName;
                                lastName = reader["last_name"]?.ToString() ?? lastName;
                                isSignedUp = reader["is_signedup"] != DBNull.Value ? Convert.ToInt32(reader["is_signedup"]) : 0;
                            }
                        }
                    }
                }

                statusDialog.UpdateStatus($"[DEBUG] userExists={userExists}, userId={userId}, isSignedUp={isSignedUp}");

                if (!userExists)
                {
                    statusDialog.UpdateStatus("User does not exist. Inserting new user...");
                    using (var conn = DatabaseConn.GetConnection())
                    {
                        await conn.OpenAsync();
                        using (var cmd = new SqlCommand(
                            "INSERT INTO users (username, password_hash, email, first_name, middle_name, last_name, user_type, created_at) " +
                            "OUTPUT INSERTED.user_id " +
                            "VALUES (@Username, @PasswordHash, @Email, @FirstName, @MiddleName, @LastName, @UserType, GETDATE())", conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", userInfo.Email);
                            cmd.Parameters.AddWithValue("@PasswordHash", "GOOGLE_AUTH");
                            cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                            cmd.Parameters.AddWithValue("@FirstName", firstName);
                            cmd.Parameters.AddWithValue("@MiddleName", middleName);
                            cmd.Parameters.AddWithValue("@LastName", lastName);
                            cmd.Parameters.AddWithValue("@UserType", userType);

                            var result = await cmd.ExecuteScalarAsync();
                            if (result != null && result != DBNull.Value)
                            {
                                userId = Convert.ToInt32(result);
                                statusDialog.UpdateStatus("User inserted successfully.");
                            }
                            else
                            {
                                statusDialog.UpdateStatus("Failed to insert user.");
                            }
                        }
                    }
                    isSignedUp = 0; // New users are not signed up yet
                }

                if (isSignedUp == 1)
                {
                    statusDialog.UpdateStatus("[DEBUG] User is fully signed up. Proceeding to MainForm.");
                    // Start the session and open MainForm as before
                    CMS_Revised.User_Interface.SessionManager.StartSession(
                        userId,
                        userInfo.Email,
                        userType,
                        firstName,
                        middleName,
                        lastName
                    );
                    statusDialog.ConfirmSessionStatus();

                    statusDialog.UpdateStatus("All operations completed successfully.");
                    statusDialog.UpdateStatus("Hiding LoginForm...");
                    statusDialog.UpdateStatus("Opening MainForm...");

                    Form? parentForm = this.ParentForm;
                    if (parentForm is LoginForm loginForm)
                    {
                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                        statusDialog.UpdateStatus("MainForm opened. User is logged in.");
                        loginForm.FormClosing -= LoginForm_FormClosing;
                        loginForm.FormClosing += LoginForm_FormClosing;
                        loginForm.Close();
                    }
                    statusDialog.EnableLogout();
                }
                else
                {
                    statusDialog.UpdateStatus("[DEBUG] User is not fully signed up. Starting signup process.");
                    // Start signup process using the existing controls
                    Form? parentForm = this.ParentForm;
                    if (parentForm is LoginForm loginForm)
                    {
                        // This will show the welcome message, and after OK, S1 will be shown
                        loginForm.Invoke(new Action(async () =>
                        {
                            await loginForm.StartSignupProcess(userId, userInfo.Email, userInfo.Name);
                        }));
                        statusDialog.UpdateStatus("[DEBUG] Signup process started. Closing status dialog.");

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                statusDialog.UpdateStatus("Error: " + ex.Message);
                statusDialog.EnableLogout();
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string userOrEmail = EmailTextbox.Text.Trim();
            string password = PasswordTextbox.Text;

            if (string.IsNullOrWhiteSpace(userOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your email/username and password.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = DatabaseConn.GetConnection())
            {
                await conn.OpenAsync();
                using var cmd = new SqlCommand(
                    "SELECT user_id, username, email, user_type, first_name, middle_name, last_name, password_hash, is_signedup FROM users WHERE email = @UserOrEmail OR username = @UserOrEmail", conn);
                cmd.Parameters.AddWithValue("@UserOrEmail", userOrEmail);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string storedHash = reader["password_hash"]?.ToString() ?? "";
                    if (PasswordHelper.VerifyPassword(password, storedHash))
                    {
                        // Check if user is fully signed up
                        int isSignedUp = reader["is_signedup"] != DBNull.Value ? Convert.ToInt32(reader["is_signedup"]) : 0;
                        if (isSignedUp != 1)
                        {
                            MessageBox.Show("Your account signup is not complete. Please finish the signup process.", "Signup Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Start session
                        int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
                        string username = reader["username"]?.ToString() ?? "";
                        string email = reader["email"]?.ToString() ?? "";
                        string userType = reader["user_type"]?.ToString() ?? "";
                        string firstName = reader["first_name"]?.ToString() ?? "";
                        string middleName = reader["middle_name"]?.ToString() ?? "";
                        string lastName = reader["last_name"]?.ToString() ?? "";

                        CMS_Revised.User_Interface.SessionManager.StartSession(
                            userId,
                            email,
                            userType,
                            firstName,
                            middleName,
                            lastName
                        );

                        // Optionally update last_login
                        reader.Close();
                        using (var updateCmd = new SqlCommand("UPDATE users SET last_login = GETDATE() WHERE user_id = @UserId", conn))
                        {
                            updateCmd.Parameters.AddWithValue("@UserId", userId);
                            await updateCmd.ExecuteNonQueryAsync();
                        }

                        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Open MainForm and close LoginForm
                        Form? parentForm = this.ParentForm;
                        if (parentForm is LoginForm loginForm)
                        {
                            MainForm mainForm = new MainForm();
                            mainForm.Show();
                            loginForm.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Account not found.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private bool isPasswordHidden = true;

        private void ShowHidePassButton_Click(object sender, EventArgs e)
        {
            isPasswordHidden = !isPasswordHidden;
            PasswordTextbox.UseSystemPasswordChar = isPasswordHidden;
            ShowHidePassButton.Text = isPasswordHidden ? "👁" : "🙈";
        }

        private void PasswordTextbox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}