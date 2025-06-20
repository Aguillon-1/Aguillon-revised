using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace CMS_Revised.User_Interface
{
    public partial class ProfileAccountSettings : UserControl
    {
        public ProfileAccountSettings()
        {
            InitializeComponent();
        }

        public void SetUserInfo(
            string name,
            string role,
            string studentNo,
            string firstName,
            string middleName,
            string lastName,
            string address,
            DateTime birthday,
            string contactNo)
        {
            NameLabel.Text = name;
            RoleLabel.Text = role;
            StudentNoLabel.Text = studentNo;
            FirstNameLabel.Text = firstName;
            MiddleNameLabel.Text = middleName;
            LastNameLabel.Text = lastName;
            AddressLabel.Text = address;
            BirthdayLabel.Text = birthday.ToString("yyyy-MM-dd");
            ContactNoLabel.Text = contactNo;
        }

        private void editbutton_Click(object sender, EventArgs e)
        {

        }
    }
}
