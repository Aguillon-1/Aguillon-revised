namespace CMS_Revised.User_Interface
{
    partial class AccountSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            emaillabel = new Label();
            emaileditbtn = new Button();
            passwordeditbtn = new Button();
            passwordlabel = new Label();
            SuspendLayout();
            // 
            // emaillabel
            // 
            emaillabel.AutoSize = true;
            emaillabel.Location = new Point(292, 167);
            emaillabel.Name = "emaillabel";
            emaillabel.Size = new Size(36, 15);
            emaillabel.TabIndex = 0;
            emaillabel.Text = "email";
            emaillabel.Click += emaillabel_Click;
            // 
            // emaileditbtn
            // 
            emaileditbtn.Location = new Point(612, 163);
            emaileditbtn.Name = "emaileditbtn";
            emaileditbtn.Size = new Size(75, 23);
            emaileditbtn.TabIndex = 1;
            emaileditbtn.Text = "emaileditbtn";
            emaileditbtn.UseVisualStyleBackColor = true;
            emaileditbtn.Click += emaileditbtn_Click;
            // 
            // passwordeditbtn
            // 
            passwordeditbtn.Location = new Point(612, 235);
            passwordeditbtn.Name = "passwordeditbtn";
            passwordeditbtn.Size = new Size(75, 23);
            passwordeditbtn.TabIndex = 3;
            passwordeditbtn.Text = "passwordbtn";
            passwordeditbtn.UseVisualStyleBackColor = true;
            passwordeditbtn.Click += passwordeditbtn_Click;
            // 
            // passwordlabel
            // 
            passwordlabel.AutoSize = true;
            passwordlabel.Location = new Point(292, 239);
            passwordlabel.Name = "passwordlabel";
            passwordlabel.Size = new Size(57, 15);
            passwordlabel.TabIndex = 2;
            passwordlabel.Text = "password";
            passwordlabel.Click += passwordlabel_Click;
            // 
            // AccountSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(passwordeditbtn);
            Controls.Add(passwordlabel);
            Controls.Add(emaileditbtn);
            Controls.Add(emaillabel);
            Name = "AccountSettings";
            Size = new Size(1024, 576);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label emaillabel;
        private Button emaileditbtn;
        private Button passwordeditbtn;
        private Label passwordlabel;
    }
}
