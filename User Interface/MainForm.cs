using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;

namespace CMS_Revised.User_Interface
{
    public partial class MainForm : Form
    {
        private FormClosedEventHandler formClosedHandler;

        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public MainForm()
        {
            InitializeComponent();

            this.Load += MainForm_Load;
            MenuLogoutButton.Click += MenuLogoutButton_Click;
            formClosedHandler = new FormClosedEventHandler((s, e) => Application.Exit());
            this.FormClosed += formClosedHandler;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Use SessionManager for user info
            home1.SetUserInfo(
                SessionManager.UserId,
                SessionManager.Email,
                $"{SessionManager.FirstName} {SessionManager.LastName}"
            );
        }

        private async void MenuLogoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Are you sure you want to logout?",
        "Logout Confirmation",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Only use the status dialog for logout and do not create a new LoginForm here
                var statusDialog = StatusDialog.ShowStatusDialog();
                statusDialog.UpdateStatus("Preparing to logout...");
                statusDialog.Logout();

                // Just close the main form, do not open LoginForm here
                this.FormClosed -= formClosedHandler;
                this.Close();
            }

            if (result == DialogResult.Yes)
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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Warning: Could not set logout flag: {ex.Message}");
                    }

                    // Clear session
                    SessionManager.EndSession();

                    this.FormClosed -= formClosedHandler;
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    loginForm.BringToFront();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Logout failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var statusDialog = StatusDialog.ShowStatusDialog();
                statusDialog.UpdateStatus("Preparing to logout...");
                statusDialog.Logout();
            }
        }
    }
}
