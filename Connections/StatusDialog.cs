using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMS_Revised.User_Interface
{
    // Singleton, resizable, pinnable status dialog for reporting system status and logout/login progress
    public class StatusDialog : Form
    {
        private readonly TextBox textBox;
        private readonly Button logoutBtn;
        private readonly Button pinBtn;
        private readonly StringBuilder statusBuilder = new StringBuilder();
        private bool isPinned = true;

        // Singleton instance
        private static StatusDialog? currentInstance;

        // Path to store logout state
        private static readonly string LogoutFlagPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "logout_required.flag");

        public StatusDialog()
        {
            Width = 300;
            Height = 400;
            Text = "CMS system report";
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
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

            pinBtn = new Button()
            {
                Text = "Unpin",
                Dock = DockStyle.Bottom
            };
            pinBtn.Click += (s, e) =>
            {
                isPinned = !isPinned;
                this.TopMost = isPinned;
                pinBtn.Text = isPinned ? "Unpin" : "Pin to Top";
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
            Controls.Add(pinBtn);
            Controls.Add(logoutBtn);

            // When closed, clear the static instance
            this.FormClosed += (s, e) => { if (currentInstance == this) currentInstance = null; };
        }

        public static StatusDialog ShowStatusDialog()
        {
            if (currentInstance == null || currentInstance.IsDisposed)
            {
                currentInstance = new StatusDialog();
                currentInstance.Show();
            }
            else
            {
                if (!currentInstance.Visible)
                    currentInstance.Show();
                currentInstance.BringToFront();
            }
            return currentInstance;
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

        public void ConfirmSessionStatus()
        {
            var sessionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "session.json");
            bool sessionOk = CMS_Revised.User_Interface.SessionManager.IsLoggedIn;
            bool fileExists = File.Exists(sessionFile);

            if (sessionOk && fileExists)
            {
                UpdateStatus("Session started and session file created successfully.");
            }
            else if (!sessionOk && fileExists)
            {
                UpdateStatus("Session file exists but session is not valid.");
            }
            else if (sessionOk && !fileExists)
            {
                UpdateStatus("Session is valid but session file is missing.");
            }
            else
            {
                UpdateStatus("Session failed to start or session file missing.");
            }
        }

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
                UpdateStatus("Logging out user...");
                await CMS_Revised.Connections.GAuthclass.ClearGoogleCredentialsAsync();
                UpdateStatus("Google credentials cleared.");

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

                UpdateStatus("User logged out. Opening login form...");
                this.BeginInvoke(new Action(() =>
                {
                    var loginForm = new LoginForm();
                    loginForm.Show();
                    UpdateStatus("Login form opened.");
                }));
            }
            catch (Exception ex)
            {
                UpdateStatus($"Logout failed: {ex.Message}");
            }
            Close();
        }

        // Helper for global status updates
        public static void GlobalUpdateStatus(string message)
        {
            var dlg = ShowStatusDialog();
            dlg.UpdateStatus(message);
        }
    }
}