using System;
using System.Windows.Forms;
using CMS_Revised.Login_Interface;
using CMS_Revised.User_Interface;

namespace CMS_Revised
{
    public partial class LoginForm : Form
    {
        // Store signup info locally
        private StudentSignupInfo studentSignupInfo = new();
        private PersonalSignupInfo personalSignupInfo = new();
        private AccountCredentialsInfo accountCredentialsInfo = new();

        private int currentUserId = 0;
        private string currentUserEmail = "";
        private string currentUserName = "";

        public LoginForm()
        {
            InitializeComponent();

            // Hide all signup controls by default
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = false;

            // Wire up events to designer controls
            s1_studentinfo1.NextClicked += S1_NextClicked;
            s1_studentinfo1.CancelClicked += Signup_CancelClicked;
            s2_personalinfo1.NextClicked += S2_NextClicked;
            s2_personalinfo1.BackClicked += S2_BackClicked;
            s3_accountcredentials1.FinalizeClicked += S3_FinalizeClicked;
            s3_accountcredentials1.BackClicked += S3_BackClicked;

            // Hide signup controls on load
            Load += async (s, e) =>
            {
                s1_studentinfo1.Visible = false;
                s2_personalinfo1.Visible = false;
                s3_accountcredentials1.Visible = false;
                LoginControl1.Visible = true;

                await s1_studentinfo1.LoadSavedStudentInfoAsync();
            };


            // Subscribe to switcher events
            loginSignupSwitcher1.LoginSelected += (s, e) =>
            {
                LoginControl1.Visible = true;
                s1_studentinfo1.Visible = false;
                s2_personalinfo1.Visible = false;
                s3_accountcredentials1.Visible = false;
                LoginControl1.BringToFront();
                loginSignupSwitcher1.SetActiveMode(true);
            };

            loginSignupSwitcher1.SignupSelected += (s, e) =>
            {
                LoginControl1.Visible = false;
                s1_studentinfo1.Visible = true;
                s2_personalinfo1.Visible = false;
                s3_accountcredentials1.Visible = false;
                s1_studentinfo1.BringToFront();
                loginSignupSwitcher1.SetActiveMode(false);
            };
        }

        private string ExtractFirstName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : "";
        }

        private string ExtractLastName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[^1] : "";
        }

        private string ExtractMiddleName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 2 ? string.Join(" ", parts, 1, parts.Length - 2) : "";
        }

        // This should be called after Google Auth is successful and user is not signed up
        public async Task StartSignupProcess(int userId, string email, string name)
        {
            currentUserId = userId;
            currentUserEmail = email;
            currentUserName = name;

            // Reset local data
            studentSignupInfo = new();
            personalSignupInfo = new();
            accountCredentialsInfo = new();

            // Set user info for controls
            s1_studentinfo1.CurrentUserId = userId;
            s2_personalinfo1.CurrentUserId = userId;
            s3_accountcredentials1.CurrentUserId = userId;
            s2_personalinfo1.UserEmail = email;
            s3_accountcredentials1.UserEmail = email;

            // Load previously saved data (auto-fill all fields)
            await s1_studentinfo1.LoadSavedStudentInfoAsync();

            // Pre-fill names from Google only if fields are empty
            s1_studentinfo1.PreFillNamesIfEmpty(
                ExtractFirstName(currentUserName),
                ExtractMiddleName(currentUserName),
                ExtractLastName(currentUserName)
            );

            // Show welcome message
            MessageBox.Show(
                $"Hi {name}!\n\nWelcome to Classroom Management System.\nPlease complete the signup process to continue.",
                "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Show S1, hide others
            s1_studentinfo1.Visible = true;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = false;
            LoginControl1.Visible = false;
            s1_studentinfo1.BringToFront();
        }

        private void S1_NextClicked(object? sender, StudentInfoEventArgs e)
        {
            studentSignupInfo = e.Info;
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = true;
            s3_accountcredentials1.Visible = false;
            s2_personalinfo1.BringToFront();
        }

        private void S2_NextClicked(object? sender, PersonalInfoEventArgs e)
        {
            personalSignupInfo = e.Info;
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = true;
            s3_accountcredentials1.BringToFront();
        }

        private void S2_BackClicked(object? sender, EventArgs e)
        {
            s1_studentinfo1.Visible = true;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = false;
            s1_studentinfo1.BringToFront();
        }

        private void S3_BackClicked(object? sender, EventArgs e)
        {
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = true;
            s3_accountcredentials1.Visible = false;
            s2_personalinfo1.BringToFront();
        }

        private void S3_FinalizeClicked(object? sender, AccountCredentialsEventArgs e)
        {
            accountCredentialsInfo = e.Info;

            // Show summary for confirmation
            string summary =
                $"Name: {studentSignupInfo.FirstName} {studentSignupInfo.MiddleName} {studentSignupInfo.LastName} {studentSignupInfo.Suffix}\n" +
                $"Student Number: {studentSignupInfo.StudentNumber}\n" +
                $"Program: {studentSignupInfo.ProgramId}, Year Level: {studentSignupInfo.YearLevelId}, Section: {studentSignupInfo.SectionId}\n" +
                $"Status: {studentSignupInfo.StudentStatus}\n" +
                $"Birthday: {personalSignupInfo.Birthday:yyyy-MM-dd}\n" +
                $"Sex: {personalSignupInfo.Sex}\n" +
                $"Email: {personalSignupInfo.Email}\n" +
                $"Contact: {personalSignupInfo.ContactNumber}\n" +
                $"Address: {personalSignupInfo.Address}\n" +
                $"Username: {accountCredentialsInfo.Username}\n";

            var result = MessageBox.Show(
                summary + "\n\nConfirm all information and finalize signup?",
                "Confirm Signup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // All data already saved in each step, just close the signup and show a success message
                MessageBox.Show("Signup complete! You may now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Hide all signup controls and show login control
                s1_studentinfo1.Visible = false;
                s2_personalinfo1.Visible = false;
                s3_accountcredentials1.Visible = false;
                LoginControl1.Visible = true;
            }
        }

        private void Signup_CancelClicked(object? sender, EventArgs e)
        {
            // Reset everything and hide all signup controls
            studentSignupInfo = new();
            personalSignupInfo = new();
            accountCredentialsInfo = new();
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = false;
            LoginControl1.Visible = true;
        }

        private void ExitButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MinimizeButton_Click(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void loginSignupSwitcher1_Load(object? sender, EventArgs e)
        {
            // Optional: Add logic if you want to handle something when the switcher loads
        }

        private void LoginForm_Load(object? sender, EventArgs e)
        {
            // Hide all signup controls on form load
            s1_studentinfo1.Visible = false;
            s2_personalinfo1.Visible = false;
            s3_accountcredentials1.Visible = false;
            // Show login control by default
            LoginControl1.Visible = true;
        }
    }
}