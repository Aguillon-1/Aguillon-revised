using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CMS_Revised.Connections;
using Microsoft.Data.SqlClient;

namespace CMS_Revised.Login_Interface
{
    public partial class S2_personalinfo : UserControl
    {
        // Placeholders for textboxes
        private readonly Dictionary<TextBox, string> _placeholders = new();

        public event EventHandler<PersonalInfoEventArgs>? NextClicked;
        public event EventHandler? BackClicked;

        public int CurrentUserId { get; set; } // Set this from LoginForm
        public string UserEmail { get; set; } = ""; // Set this from LoginForm

        public S2_personalinfo()
        {
            InitializeComponent();
            InitializePlaceholders();
            Load += S2_personalinfo_Load;
            S2Nextbutton.Click += S2Nextbutton_Click;
            S2Backbutton.Click += (s, e) => BackClicked?.Invoke(this, EventArgs.Empty);

            // Make Sexcombobox untypable
            Sexcombobox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void InitializePlaceholders()
        {
            SetPlaceholder(Contactnotextbox, "09XXXXXXXXX");
            SetPlaceholder(Addresstextbox, "Blk 1 Lot 2, Brgy. Example, City, Province");
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            _placeholders[textBox] = placeholder;
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };
            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void S2_personalinfo_Load(object? sender, EventArgs e)
        {
            // Populate sex combobox with placeholder
            Sexcombobox.Items.Clear();
            Sexcombobox.Items.Add("Select sex");
            Sexcombobox.Items.AddRange(new object[] { "Male", "Female" });
            Sexcombobox.SelectedIndex = 0;

            // Set email textbox (greyed out, uneditable, unselectable)
            Emailtextbox.Text = UserEmail;
            Emailtextbox.ReadOnly = true;
            Emailtextbox.TabStop = false;
            Emailtextbox.BackColor = Color.LightGray;
            Emailtextbox.ForeColor = Color.DimGray;
            Emailtextbox.Cursor = Cursors.Default;
        }

        private void S2Nextbutton_Click(object? sender, EventArgs e)
        {
            var missingFields = new List<string>();

            // Only require birthday and sex
            if (Sexcombobox.SelectedIndex == 0)
                missingFields.Add("Sex");

            // (Birthday is always set, but you can check for a valid range if needed)

            if (missingFields.Count > 0)
            {
                MessageBox.Show("Please fill out the following fields:\n- " + string.Join("\n- ", missingFields), "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var info = new PersonalSignupInfo
            {
                Birthday = Birthdaypicker.Value.Date,
                Sex = Sexcombobox.Text,
                Email = Emailtextbox.Text,
                ContactNumber = GetTextOrEmpty(Contactnotextbox),
                Address = GetTextOrEmpty(Addresstextbox)
            };

            _ = SavePartialPersonalInfoAsync(info);

            NextClicked?.Invoke(this, new PersonalInfoEventArgs(info));
        }

        private bool ValidateFields()
        {
            if (Sexcombobox.SelectedIndex < 0)
                return false;
            if (string.IsNullOrWhiteSpace(Emailtextbox.Text))
                return false;
            foreach (var pair in _placeholders)
            {
                if (string.IsNullOrWhiteSpace(pair.Key.Text) || pair.Key.Text == pair.Value)
                    return false;
            }
            return true;
        }

        private string GetTextOrEmpty(TextBox textBox)
        {
            return (_placeholders.TryGetValue(textBox, out var placeholder) && textBox.Text == placeholder) ? "" : textBox.Text.Trim();
        }

        private async Task SavePartialPersonalInfoAsync(PersonalSignupInfo info)
        {
            if (CurrentUserId <= 0) return;

            using var conn = DatabaseConn.GetConnection();
            await conn.OpenAsync();

            // Update users table for personal info
            using (var cmd = new SqlCommand(@"
                UPDATE users SET
                    birthday = @Birthday,
                    sex = @Sex,
                    address = @Address,
                    contact_number = @ContactNumber,
                    updated_at = GETDATE()
                WHERE user_id = @UserId
                ", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                cmd.Parameters.AddWithValue("@Birthday", info.Birthday);
                cmd.Parameters.AddWithValue("@Sex", info.Sex);
                cmd.Parameters.AddWithValue("@Address", info.Address);
                cmd.Parameters.AddWithValue("@ContactNumber", info.ContactNumber);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public class PersonalSignupInfo
    {
        public DateTime Birthday { get; set; }
        public string Sex { get; set; } = "";
        public string Email { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string Address { get; set; } = "";
    }

    public class PersonalInfoEventArgs : EventArgs
    {
        public PersonalSignupInfo Info { get; }
        public PersonalInfoEventArgs(PersonalSignupInfo info) => Info = info;
    }
}