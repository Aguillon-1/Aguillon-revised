using ClassroomManagementSystem;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CMS_Revised.Connections;

namespace Admin
{
    public partial class Admin : Form
    {
        private Button activeButton = null;
        private bool slide = true;
        private string[] buttons = { "Menu", "Home", "Account Manager", "Class Manager", "System Configuration", "Class Moderation", "Grading Sheet", "Curriculum Manager", "Log Out" };

        // Animation variables
        private int animationTargetWidth = 0;
        private int animationStep = 5;
        private double animationAcceleration = 1.4;

        public Admin()
        {
            InitializeComponent();
            // Attach event handlers for controls not set in designer
            this.Load += Admin_Load;
            this.Resize += Admin_Resize;
            MenuButton.Click += MenuButton_Click;
            HomeButton.Click += HomeButton_Click;
            AccountManagerButton.Click += AccountManagerButton_Click;
            AddingChangingSubjectsButton.Click += AddingChangingSubjectsButton_Click;
            CurriculumManager.Click += CurriculumManager_Click;
            slidingMenuTimer.Tick += slidingMenuTimer_Tick;
            //GradingButton.Click += GradingButton_Click;
            SystemConfigButton.Click += SystemConfigButton_Click;
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            try
            {
                // Remove borders from specific buttons - add null checks
                if (MenuButton != null) MenuButton.FlatAppearance.BorderSize = 0;
                if (HomeButton != null) HomeButton.FlatAppearance.BorderSize = 0;
                if (AccountManagerButton != null) AccountManagerButton.FlatAppearance.BorderSize = 0;
                if (AddingChangingSubjectsButton != null) AddingChangingSubjectsButton.FlatAppearance.BorderSize = 0;
                if (SystemConfigButton != null) SystemConfigButton.FlatAppearance.BorderSize = 0;
                if (CurriculumManager != null) CurriculumManager.FlatAppearance.BorderSize = 0;

                // Configure panels - add null checks
                if (HeaderPanel != null) HeaderPanel.BorderStyle = BorderStyle.FixedSingle;
                if (MenuPanel != null)
                {
                    MenuPanel.BackColor = Color.FromArgb(47, 97, 83);
                    MenuPanel.BorderStyle = BorderStyle.FixedSingle;
                }

                // Initially hide all user controls
                HideAllUserControls();

                // Also specifically hide the designer-added controls with null checks
                if (AccountManager1 != null) AccountManager1.Visible = false;
                if (AddingChangingSubjects1 != null) AddingChangingSubjects1.Visible = false;
                if (CurriculumManager1 != null) CurriculumManager1.Visible = false;
                if (SystemConfiguration1 != null) SystemConfiguration1.Visible = false;
                //GradeAdminView1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during form load: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Admin_Load error: {ex}");
            }
        }

        private void ShowUserControl<T>() where T : UserControl, new()
        {
            HideAllUserControls();

            T userControl = null;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is T)
                {
                    userControl = (T)ctrl;
                    break;
                }
            }

            if (userControl == null)
            {
                userControl = new T();
                userControl.Name = typeof(T).Name + "Control";
                this.Controls.Add(userControl);
            }

            userControl.Location = new Point(MenuPanel.Width, HeaderPanel.Height);
            userControl.Width = this.ClientSize.Width - MenuPanel.Width;
            userControl.Height = this.ClientSize.Height - HeaderPanel.Height;
            userControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            userControl.Visible = true;
            userControl.BringToFront();

            RefreshUserControl(userControl);
        }

        private void RefreshUserControl(UserControl userControl)
        {
            try
            {
                if (userControl is AccountManager)
                {
                    var method = userControl.GetType().GetMethod("RefreshStudentList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    method?.Invoke(userControl, null);
                }
                else if (userControl is AddingChangingSubjects)
                {
                    var method = userControl.GetType().GetMethod("RefreshStudentList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    method?.Invoke(userControl, null);
                }
                else if (userControl is CurriculumManager)
                {
                    var method = userControl.GetType().GetMethod("LoadAllSubjects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    method?.Invoke(userControl, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing {userControl.GetType().Name}: {ex.Message}");
            }

            userControl.Invalidate();
            Application.DoEvents();
        }

        private void HideAllUserControls()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is UserControl)
                {
                    ctrl.Visible = false;
                }
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);
            HideAllUserControls();
        }

        private void AccountManagerButton_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);
            ShowUserControl<AccountManager>();
        }

        private void AddingChangingSubjectsButton_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);
            ShowUserControl<AddingChangingSubjects>();
        }

        private void CurriculumManager_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);
            ShowUserControl<CurriculumManager>();
        }

        private void Admin_Resize(object sender, EventArgs e)
        {
            AdjustUserControlsBounds();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);

            if (slide)
                animationTargetWidth = MenuPanel.MinimumSize.Width;
            else
                animationTargetWidth = MenuPanel.MaximumSize.Width;

            slidingMenuTimer.Interval = 10;
            slidingMenuTimer.Start();
        }

        private void slidingMenuTimer_Tick(object sender, EventArgs e)
        {
            if (slide && MenuPanel.Width == MenuPanel.MaximumSize.Width)
                RemoveTextButtons();

            MenuPanel.BringToFront();

            int distanceToTarget = Math.Abs(MenuPanel.Width - animationTargetWidth);
            int currentStep = Math.Max(1, Math.Min(animationStep, distanceToTarget / 3));

            if (slide) // Collapsing
            {
                MenuPanel.Width -= currentStep;
                if (MenuPanel.Width <= animationTargetWidth)
                {
                    MenuPanel.Width = MenuPanel.MinimumSize.Width;
                    slidingMenuTimer.Stop();
                    slide = false;
                    AdjustUserControlsBounds();
                }
            }
            else // Expanding
            {
                MenuPanel.Width += currentStep;
                if (MenuPanel.Width >= animationTargetWidth)
                {
                    MenuPanel.Width = MenuPanel.MaximumSize.Width;
                    AddTextButtons();
                    slidingMenuTimer.Stop();
                    slide = true;
                    AdjustUserControlsBounds();
                }
            }
            AdjustUserControlsBounds();
        }

        private void AdjustUserControlsBounds()
        {
            int newX = MenuPanel.Width;
            int newWidth = this.ClientSize.Width - MenuPanel.Width;
            int newHeight = this.ClientSize.Height - HeaderPanel.Height;
            int headerHeight = HeaderPanel.Height;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is UserControl)
                {
                    ctrl.Location = new Point(newX, headerHeight);
                    ctrl.Width = newWidth;
                    ctrl.Height = newHeight;
                }
            }
        }

        private void RemoveTextButtons()
        {
            if (MenuButton != null) MenuButton.Text = string.Empty;
            if (HomeButton != null) HomeButton.Text = string.Empty;
            if (AccountManagerButton != null) AccountManagerButton.Text = string.Empty;
            if (AddingChangingSubjectsButton != null) AddingChangingSubjectsButton.Text = string.Empty;
            if (SystemConfigButton != null) SystemConfigButton.Text = string.Empty;
            if (CurriculumManager != null) CurriculumManager.Text = string.Empty;
            if (LogoutButton != null) LogoutButton.Text = string.Empty;
        }

        private void AddTextButtons()
        {
            if (MenuButton != null) MenuButton.Text = buttons[0];
            if (HomeButton != null) HomeButton.Text = buttons[1];
            if (AccountManagerButton != null) AccountManagerButton.Text = buttons[2];
            if (AddingChangingSubjectsButton != null) AddingChangingSubjectsButton.Text = buttons[3];
            if (SystemConfigButton != null) SystemConfigButton.Text = buttons[4];
            if (CurriculumManager != null) CurriculumManager.Text = buttons[7];
            if (LogoutButton != null) LogoutButton.Text = buttons[8];
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;
            if (activeButton != null)
                activeButton.BackColor = Color.FromArgb(47, 97, 83);
            if (clickedButton != null)
            {
                clickedButton.BackColor = Color.FromArgb(93, 242, 167);
                activeButton = clickedButton;
            }
        }

        private void SystemConfigButton_Click(object sender, EventArgs e)
        {
            Button_Click(sender, e);
            ShowUserControl<SystemConfiguration>();
        }
        /*
private void GradingButton_Click(object sender, EventArgs e)
{
   HideAllUserControls();
   GradeAdminView1.Visible = true;
}
*/


    }
}