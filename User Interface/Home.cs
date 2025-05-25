using System;
using System.Windows.Forms;

namespace CMS_Revised.User_Interface
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        public void SetUserInfo(int id, string email, string name)
        {
            HomeNameLabel.Text = "Name: " + (name ?? "");
        }
    }
}