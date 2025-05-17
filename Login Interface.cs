using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;
using Microsoft.Data.SqlClient;
using System.IO;

namespace CMS_Revised
{
    public partial class Login_Interface : UserControl
    {
        // Path to store logout state
        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public Login_Interface()
        {
            InitializeComponent();
            GoogleButton.Click += GoogleButton_Click;
            
        }

        private void GoogleButton_Click(object? sender, EventArgs e)
        {
            var statusDialog = new StatusDialog();
            LoadingAnimation? loading = null;

            // Show the dialog modally (this keeps the UI responsive)
            statusDialog.Shown += async (s, args) =>
            {
                try
                {
                    statusDialog.UpdateStatus("Login initiated...");

                    // Show loading animation on UI thread
                    loading = new LoadingAnimation();
                    loading.SpinSpeed = 5; // You can adjust the speed here
                    loading.Show();

                    // Check if user previously logged out and needs a fresh login
                    bool forceNewLogin = File.Exists(LogoutFlagPath);

                    if (forceNewLogin)
                    {
                        statusDialog.UpdateStatus("Previous logout detected. Clearing any saved credentials...");
                        await GAuthclass.ClearGoogleCredentialsAsync();

                        // Delete the flag file after clearing credentials
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

                    string[] nameParts = userInfo.Name.Split(' ');
                    string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                    string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                    string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                    statusDialog.UpdateStatus("Checking if user already exists in the database...");
                    bool userExists = false;
                    using (var conn = DatabaseConn.GetConnection())
                    {
                        await conn.OpenAsync();
                        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM users WHERE email = @Email", conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                            int count = (int)await cmd.ExecuteScalarAsync();
                            userExists = count > 0;
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
                                "VALUES (@Username, @PasswordHash, @Email, @FirstName, @MiddleName, @LastName, @UserType, GETDATE())", conn))
                            {
                                cmd.Parameters.AddWithValue("@Username", userInfo.Email);
                                cmd.Parameters.AddWithValue("@PasswordHash", "GOOGLE_AUTH");
                                cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                                cmd.Parameters.AddWithValue("@FirstName", firstName);
                                cmd.Parameters.AddWithValue("@MiddleName", middleName);
                                cmd.Parameters.AddWithValue("@LastName", lastName);
                                cmd.Parameters.AddWithValue("@UserType", "Student");

                                int rows = await cmd.ExecuteNonQueryAsync();
                                if (rows > 0)
                                    statusDialog.UpdateStatus("User inserted successfully.");
                                else
                                    statusDialog.UpdateStatus("Failed to insert user.");
                            }
                        }
                    }

                    statusDialog.UpdateStatus("");
                    statusDialog.UpdateStatus("All operations completed successfully.");

                    // Hide loading animation after 3 seconds
                    if (loading != null)
                    {
                        await Task.Delay(3000);
                        if (loading.IsHandleCreated)
                        {
                            loading.BeginInvoke(new Action(() =>
                            {
                                loading.StopSpin();
                                loading.Close();
                            }));
                        }
                        else
                        {
                            loading.StopSpin();
                            loading.Close();
                        }
                    }

                    // Show logout prompt on the UI thread
                    if (statusDialog.IsHandleCreated)
                    {
                        statusDialog.Invoke(new Action(() =>
                        {
                            var result = MessageBox.Show(
                                "Do you want to logout now?",
                                "Logout Confirmation",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );
                            if (result == DialogResult.Yes)
                            {
                                // Call the public logout method
                                statusDialog.Logout();
                            }
                            else
                            {
                                statusDialog.UpdateStatus("You are still logged in. You can test another account.");
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    if (loading != null)
                    {
                        if (loading.IsHandleCreated)
                        {
                            loading.Invoke(new Action(() =>
                            {
                                loading.StopSpin();
                                loading.Close();
                            }));
                        }
                        else
                        {
                            loading.StopSpin();
                            loading.Close();
                        }
                    }
                    statusDialog.UpdateStatus("Error: " + ex.Message);
                    statusDialog.EnableLogout();
                }
            };

            statusDialog.ShowDialog();
        }
    }

    public class StatusDialog : Form
    {
        private readonly TextBox textBox;
        private readonly Button logoutBtn;
        private readonly StringBuilder statusBuilder = new StringBuilder();

        // Path to store logout state (same as in Login_Interface)
        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public StatusDialog()
        {
            Width = 500;
            Height = 400;
            Text = "Google Login Status";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            textBox = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Top,
                Height = 320,
                ScrollBars = ScrollBars.Vertical
            };
            logoutBtn = new Button()
            {
                Text = "Logout",
                Dock = DockStyle.Bottom,
                Enabled = false
            };
            logoutBtn.Click += async (s, e) =>
            {
                await LogoutAsync();
            };
            Controls.Add(textBox);
            Controls.Add(logoutBtn);
        }

        public void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            statusBuilder.AppendLine(message);
            textBox.Text = statusBuilder.ToString();
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        public void EnableLogout()
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new Action(EnableLogout));
                return;
            }
            logoutBtn.Enabled = true;
        }

        // Public method to trigger logout logic
        public void Logout()
        {
            // Run logout logic on the UI thread
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new Action(Logout));
                return;
            }
            // Call the async logout logic
            _ = LogoutAsync();
        }

        // Async logout logic
        private async Task LogoutAsync()
        {
            try
            {
                // Clear Google credentials to force new login on next attempt
                await GAuthclass.ClearGoogleCredentialsAsync();

                // Create a flag file to indicate logout
                try
                {
                    // Ensure Data directory exists
                    string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                    if (!Directory.Exists(dataDir))
                    {
                        Directory.CreateDirectory(dataDir);
                    }

                    // Create the flag file
                    File.WriteAllText(LogoutFlagPath, DateTime.Now.ToString());
                    UpdateStatus("Logout flag set. Next login will require account selection.");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Warning: Could not set logout flag: {ex.Message}");
                }

                UpdateStatus("Logged out and credentials cleared.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Logout failed: {ex.Message}");
            }
            Close();
        }
    }
}