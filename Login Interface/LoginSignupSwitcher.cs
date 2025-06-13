using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMS_Revised
{
    public partial class LoginSignupSwitcher : UserControl
    {
        // Events to notify parent form (LoginForm) of mode switch
        public event EventHandler? LoginSelected;
        public event EventHandler? SignupSelected;

        public LoginSignupSwitcher()
        {
            InitializeComponent();
            SwitchtoLoginButton.Click += SwitchtoLoginButton_Click;
            SwitchtoSignupButton.Click += SwitchtoSignupButton_Click;
            SetActiveMode(isLogin: true);
        }

        private void SwitchtoLoginButton_Click(object sender, EventArgs e)
        {
            SetActiveMode(isLogin: true);
            LoginSelected?.Invoke(this, EventArgs.Empty);
        }

        private void SwitchtoSignupButton_Click(object sender, EventArgs e)
        {
            SetActiveMode(isLogin: false);
            SignupSelected?.Invoke(this, EventArgs.Empty);
        }

        // Call this from parent to update button highlight
        public void SetActiveMode(bool isLogin)
        {
            if (isLogin)
            {
                SwitchtoLoginButton.ForeColor = Color.Black;
                SwitchtoSignupButton.ForeColor = Color.Gray;
            }
            else
            {
                SwitchtoLoginButton.ForeColor = Color.Gray;
                SwitchtoSignupButton.ForeColor = Color.Black;
            }
        }
    }
}
