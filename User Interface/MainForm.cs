using System;
using System.IO;
using System.Windows.Forms;
using CMS_Revised.Connections;

namespace CMS_Revised.User_Interface
{
    public partial class MainForm : Form
    {
        private int _userId;
        private string _userEmail;
        private string _userName;
        private FormClosedEventHandler formClosedHandler; // Changed type to FormClosedEventHandler

        // Path to store logout state - same as in Login_Interface
        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public MainForm(int userId, string userEmail, string userName)
        {
            InitializeComponent();
            _userId = userId;
            _userEmail = userEmail;
            _userName = userName;

            // Make sure MainForm_Load is hooked up
            this.Load += MainForm_Load;

            // Hook up the MenuLogoutButton click event
            MenuLogoutButton.Click += MenuLogoutButton_Click;

            // Store the handler so we can remove it later if needed
            formClosedHandler = new FormClosedEventHandler((s, e) => Application.Exit()); // Create handler with correct type

            // Ensure the app closes when MainForm is closed (normal case)
            this.FormClosed += formClosedHandler;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Make sure 'home1' is the actual name of your Home user control instance
            home1.SetUserInfo(_userId, _userEmail, _userName);
        }

        private async void MenuLogoutButton_Click(object sender, EventArgs e)
        {
            // Ask the user to confirm logout
            DialogResult result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Clear Google credentials (same as in StatusDialog.LogoutAsync)
                    await GAuthclass.ClearGoogleCredentialsAsync();

                    // Create logout flag to force new login next time (same as in StatusDialog.LogoutAsync)
                    try
                    {
                        string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                        if (!Directory.Exists(dataDir))
                        {
                            Directory.CreateDirectory(dataDir);
                        }
                        File.WriteAllText(LogoutFlagPath, DateTime.Now.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Warning: Could not set logout flag: {ex.Message}");
                    }

                    // CRITICAL: Remove the FormClosed event handler that would exit the application
                    this.FormClosed -= formClosedHandler;

                    // Create and show a new LoginForm
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    loginForm.BringToFront();

                    // Close this MainForm without exiting the application
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Logout failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}