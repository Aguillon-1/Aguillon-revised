using ClassroomManagementSystem;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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
            // Remove borders from specific buttons
            MenuButton.FlatAppearance.BorderSize = 0;
            HomeButton.FlatAppearance.BorderSize = 0;
            AccountManagerButton.FlatAppearance.BorderSize = 0;
            AddingChangingSubjectsButton.FlatAppearance.BorderSize = 0;
            SystemConfigButton.FlatAppearance.BorderSize = 0;
            ClassModerationButton.FlatAppearance.BorderSize = 0;
            GradingButton.FlatAppearance.BorderSize = 0;
            CurriculumManager.FlatAppearance.BorderSize = 0;

            HeaderPanel.BorderStyle = BorderStyle.FixedSingle;
            MenuPanel.BackColor = Color.FromArgb(47, 97, 83);
            MenuPanel.BorderStyle = BorderStyle.FixedSingle;

            // Initially hide all user controls
            HideAllUserControls();

            // Also specifically hide the designer-added controls
            AccountManager1.Visible = false;
            AddingChangingSubjects1.Visible = false;
            CurriculumManager1.Visible = false;
            SystemConfiguration1.Visible = false;
            //GradeAdminView1.Visible = false;
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
            MenuButton.Text = string.Empty;
            HomeButton.Text = string.Empty;
            AccountManagerButton.Text = string.Empty;
            AddingChangingSubjectsButton.Text = string.Empty;
            SystemConfigButton.Text = string.Empty;
            ClassModerationButton.Text = string.Empty;
            GradingButton.Text = string.Empty;
            CurriculumManager.Text = string.Empty;
            LogoutButton.Text = string.Empty;
        }

        private void AddTextButtons()
        {
            MenuButton.Text = buttons[0];
            HomeButton.Text = buttons[1];
            AccountManagerButton.Text = buttons[2];
            AddingChangingSubjectsButton.Text = buttons[3];
            SystemConfigButton.Text = buttons[4];
            ClassModerationButton.Text = buttons[5];
            GradingButton.Text = buttons[6];
            CurriculumManager.Text = buttons[7];
            LogoutButton.Text = buttons[8];
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;
            if (activeButton != null)
                activeButton.BackColor = Color.FromArgb(47, 97, 83);
            clickedButton.BackColor = Color.FromArgb(93, 242, 167);
            activeButton = clickedButton;
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
