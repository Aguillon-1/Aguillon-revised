using CMS_Revised.User_Interface;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using CMS_Revised.Connections;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.SqlClient;

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

                var statusDialog = CMS_Revised.User_Interface.StatusDialog.ShowStatusDialog();
                statusDialog.UpdateStatus("Checking database connection...");

                bool dbOk = true;
                var latencies = new double[5];

                for (int i = 0; i < 5; i++)
                {
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        using (var conn = DatabaseConn.GetConnection())
                        {
                            conn.Open();
                            using (var cmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT 1", conn))
                            {
                                cmd.ExecuteScalar();
                            }
                        }
                        sw.Stop();
                        latencies[i] = sw.Elapsed.TotalMilliseconds;
                        statusDialog.UpdateStatus($"Ping {i + 1}: Success ({latencies[i]:0.##} ms)");
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        latencies[i] = -1;
                        statusDialog.UpdateStatus($"Ping {i + 1}: Failed ({ex.Message})");
                        dbOk = false;
                    }
                    Application.DoEvents(); // Allow UI update
                    Thread.Sleep(100); // Small delay between pings
                }

                if (latencies.All(x => x < 0))
                {
                    statusDialog.UpdateStatus("Database is unreachable.");
                    MessageBox.Show("Database is unreachable. Some features may not work.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    var validLatencies = latencies.Where(x => x >= 0).ToArray();
                    double avg = validLatencies.Average();
                    double min = validLatencies.Min();
                    double max = validLatencies.Max();
                    statusDialog.UpdateStatus($"Summary: {validLatencies.Length} successful, {latencies.Count(x => x < 0)} failed.");
                    statusDialog.UpdateStatus($"Latency (ms): avg={avg:0.##}, min={min:0.##}, max={max:0.##}");
                }

                statusDialog.UpdateStatus("Database connection check complete.");
                

                Application.ThreadException += (sender, args) =>
                {
                    MessageBox.Show($"An error occurred: {args.Exception.Message}",
                        "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // Always run MainForm
                var mainForm = new MainForm();
                mainForm.TopMost = true;
                mainForm.Show();
                mainForm.BringToFront();
                mainForm.Activate();
                mainForm.TopMost = false;

                Application.Run();
            }
        }
    }
}