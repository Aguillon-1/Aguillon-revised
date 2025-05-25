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
            statusDialog.Show(); // Show modelessly

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

                    string[] nameParts = userInfo.Name.Split(' ');
                    string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                    string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                    string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                    statusDialog.UpdateStatus("Checking if user already exists in the database...");
                    bool userExists = false;
                    int userId = -1;
                    using (var conn = DatabaseConn.GetConnection())
                    {
                        await conn.OpenAsync();
                        using (var cmd = new SqlCommand("SELECT user_id FROM users WHERE email = @Email", conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                            var result = await cmd.ExecuteScalarAsync();
                            if (result != null && result != DBNull.Value)
                            {
                                userExists = true;
                                userId = Convert.ToInt32(result);
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
                                cmd.Parameters.AddWithValue("@UserType", "Student");

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

                    statusDialog.UpdateStatus("");
                    statusDialog.UpdateStatus("All operations completed successfully.");
                    statusDialog.UpdateStatus("Hiding LoginForm...");
                    statusDialog.UpdateStatus("Opening MainForm...");

                    // Open MainForm and hide LoginForm (do not close)
                    Form? parentForm = this.ParentForm;
                    if (parentForm is LoginForm loginForm)
                    {
                        MainForm mainForm = new MainForm(userId, userInfo.Email, userInfo.Name); // Pass all info
                        mainForm.Show();
                        loginForm.Hide();
                        statusDialog.UpdateStatus("MainForm opened. User is logged in.");
                        statusDialog.UpdateStatus($"User ID: {userId}");
                        statusDialog.UpdateStatus($"User Email: {userInfo.Email}");
                        statusDialog.UpdateStatus($"User Name: {userInfo.Name}");
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
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    Invoke(new Action(Logout));
                return;
            }
            _ = LogoutAsync();
        }

        private async Task LogoutAsync()
        {
            try
            {
                await GAuthclass.ClearGoogleCredentialsAsync();

                try
                {
                    string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                    if (!Directory.Exists(dataDir))
                    {
                        Directory.CreateDirectory(dataDir);
                    }
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