using CMS_Revised.User_Interface;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace CMS_Revised
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, "CMS_Revised_SingleInstanceMutex", out createdNew))
            {
                if (!createdNew)
                {
                    DialogResult result = MessageBox.Show(
                        "The program is already running.\n\nDo you want to Restart the program or Close it?\n\nClick Retry to Restart, Cancel to Close.",
                        "Program Already Running",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Retry)
                    {
                        // Kill all other instances and restart
                        foreach (var proc in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
                        {
                            if (proc.Id != Process.GetCurrentProcess().Id)
                            {
                                try { proc.Kill(); } catch { }
                            }
                        }
                        Application.Restart();
                    }
                    else // Cancel: kill all instances including this one
                    {
                        foreach (var proc in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
                        {
                            try { proc.Kill(); } catch { }
                        }
                        Environment.Exit(0);
                    }
                    return;
                }

                ApplicationConfiguration.Initialize();

                Application.ThreadException += (sender, args) =>
                {
                    MessageBox.Show($"An error occurred: {args.Exception.Message}",
                        "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // Determine which form to show (LoginForm or MainForm)
                Form formToShow;
                if (CMS_Revised.User_Interface.SessionManager.IsLoggedIn)
                {
                    formToShow = new MainForm();
                }
                else
                {
                    formToShow = new LoginForm();
                }

                // Show the form and bring it to the top
                formToShow.TopMost = true;
                formToShow.Show();
                formToShow.BringToFront();
                formToShow.Activate();
                formToShow.TopMost = false;

                Application.Run();
            }
        }
    }
}