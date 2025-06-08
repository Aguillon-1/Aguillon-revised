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
        }

        private void GoogleButton_Click(object? sender, EventArgs e)
        {
            var statusDialog = StatusDialog.ShowStatusDialog();
            _ = RunLoginProcessAsync(statusDialog);

            statusDialog.Shown += async (s, args) =>
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

                    using (var conn = DatabaseConn.GetConnection())
                    {
                        await conn.OpenAsync();
                        using (var cmd = new SqlCommand("SELECT user_id, user_type, first_name, middle_name, last_name FROM users WHERE email = @Email", conn))
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
                                }
                            }
                        }
                    }

                    if (userExists)
                    {
                        statusDialog.UpdateStatus("User already exists in the database.");
                    }
                    else
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
                    }

                    // Start the session
                    CMS_Revised.User_Interface.SessionManager.StartSession(
                        userId,
                        userInfo.Email,
                        userType,
                        firstName,
                        middleName,
                        lastName
                    );
                    statusDialog.ConfirmSessionStatus();

                    statusDialog.UpdateStatus("");
                    statusDialog.UpdateStatus("All operations completed successfully.");
                    statusDialog.UpdateStatus("Hiding LoginForm...");
                    statusDialog.UpdateStatus("Opening MainForm...");

                    // Open MainForm and hide LoginForm (do not close)
                    Form? parentForm = this.ParentForm;
                    if (parentForm is LoginForm loginForm)
                    {
                        MainForm mainForm = new MainForm(); // No arguments
                        mainForm.Show();
                        statusDialog.UpdateStatus("MainForm opened. User is logged in.");
                        statusDialog.UpdateStatus($"User ID: {userId}");
                        statusDialog.UpdateStatus($"User Email: {userInfo.Email}");
                        statusDialog.UpdateStatus($"User Name: {userInfo.Name}");

                        // Close the form instead of hiding it - but do this after MainForm is shown
                        loginForm.FormClosing -= LoginForm_FormClosing; // Remove handler if it exists
                        loginForm.FormClosing += LoginForm_FormClosing; // Add custom handler
                        loginForm.Close();
                    }

                    statusDialog.EnableLogout();
                }
                catch (Exception ex)
                {
                    statusDialog.UpdateStatus("Error: " + ex.Message);
                    statusDialog.EnableLogout();
                }
            };
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

                using (var conn = DatabaseConn.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT user_id, user_type, first_name, middle_name, last_name FROM users WHERE email = @Email", conn))
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
                            }
                        }
                    }
                }

                if (userExists)
                {
                    statusDialog.UpdateStatus("User already exists in the database.");
                }
                else
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
                }

                // Start the session
                CMS_Revised.User_Interface.SessionManager.StartSession(
                    userId,
                    userInfo.Email,
                    userType,
                    firstName,
                    middleName,
                    lastName
                );
                statusDialog.ConfirmSessionStatus();

                statusDialog.UpdateStatus("");
                statusDialog.UpdateStatus("All operations completed successfully.");
                statusDialog.UpdateStatus("Hiding LoginForm...");
                statusDialog.UpdateStatus("Opening MainForm...");

                // Open MainForm and hide LoginForm (do not close)
                Form? parentForm = this.ParentForm;
                if (parentForm is LoginForm loginForm)
                {
                    MainForm mainForm = new MainForm(); // No arguments
                    mainForm.Show();
                    statusDialog.UpdateStatus("MainForm opened. User is logged in.");
                    statusDialog.UpdateStatus($"User ID: {userId}");
                    statusDialog.UpdateStatus($"User Email: {userInfo.Email}");
                    statusDialog.UpdateStatus($"User Name: {userInfo.Name}");

                    // Close the form instead of hiding it - but do this after MainForm is shown
                    loginForm.FormClosing -= LoginForm_FormClosing; // Remove handler if it exists
                    loginForm.FormClosing += LoginForm_FormClosing; // Add custom handler
                    loginForm.Close();
                }

                statusDialog.EnableLogout();
            }
            catch (Exception ex)
            {
                statusDialog.UpdateStatus("Error: " + ex.Message);
                statusDialog.EnableLogout();
            }
        }
    }

}