using System;
using System.Windows.Forms;
using CMS_Revised.Connections;

namespace CMS_Revised.User_Interface
{
    public partial class MainForm : Form
    {
        private int _userId;
        private string _userEmail;
        private string _userName;

        public MainForm(int userId, string userEmail, string userName)
        {
            InitializeComponent();
            _userId = userId;
            _userEmail = userEmail;
            _userName = userName;

            // Make sure MainForm_Load is hooked up
            this.Load += MainForm_Load;

            // Ensure the app closes when MainForm is closed
            this.FormClosed += (s, e) => Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Make sure 'home1' is the actual name of your Home user control instance
            home1.SetUserInfo(_userId, _userEmail, _userName);
        }
    }
}