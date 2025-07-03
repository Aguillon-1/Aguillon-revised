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
            panel1 = new Panel();
            passwordlabel = new Label();
            emaillabel = new Label();
            passwordtxtbox = new TextBox();
            confirmpwtxtbox = new TextBox();
            Editbtn = new Button();
            Discardbtn = new Button();
            Savebtn = new Button();
            label1 = new Label();
            Seebutton = new Button();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveBorder;
            panel1.Location = new Point(121, 65);
            panel1.Name = "panel1";
            panel1.Size = new Size(108, 100);
            panel1.TabIndex = 4;
            // 
            // passwordlabel
            // 
            passwordlabel.AutoSize = true;
            passwordlabel.Location = new Point(119, 253);
            passwordlabel.Name = "passwordlabel";
            passwordlabel.Size = new Size(57, 15);
            passwordlabel.TabIndex = 2;
            passwordlabel.Text = "Password";
            passwordlabel.Click += passwordlabel_Click;
            // 
            // emaillabel
            // 
            emaillabel.AutoSize = true;
            emaillabel.Location = new Point(121, 199);
            emaillabel.Name = "emaillabel";
            emaillabel.Size = new Size(39, 15);
            emaillabel.TabIndex = 0;
            emaillabel.Text = "Email:";
            emaillabel.Click += emaillabel_Click;
            // 
            // passwordtxtbox
            // 
            passwordtxtbox.Location = new Point(119, 271);
            passwordtxtbox.Name = "passwordtxtbox";
            passwordtxtbox.ReadOnly = true;
            passwordtxtbox.Size = new Size(201, 23);
            passwordtxtbox.TabIndex = 5;
            passwordtxtbox.TextChanged += passwordtxtbox_TextChanged;
            // 
            // confirmpwtxtbox
            // 
            confirmpwtxtbox.Location = new Point(119, 327);
            confirmpwtxtbox.Name = "confirmpwtxtbox";
            confirmpwtxtbox.Size = new Size(201, 23);
            confirmpwtxtbox.TabIndex = 6;
            confirmpwtxtbox.Visible = false;
            confirmpwtxtbox.TextChanged += confirmpwtxtbox_TextChanged;
            // 
            // Editbtn
            // 
            Editbtn.Location = new Point(275, 370);
            Editbtn.Name = "Editbtn";
            Editbtn.Size = new Size(75, 23);
            Editbtn.TabIndex = 7;
            Editbtn.Text = "EDIT";
            Editbtn.UseVisualStyleBackColor = true;
            Editbtn.Click += Editbtn_Click;
            // 
            // Discardbtn
            // 
            Discardbtn.Location = new Point(366, 370);
            Discardbtn.Name = "Discardbtn";
            Discardbtn.Size = new Size(75, 23);
            Discardbtn.TabIndex = 8;
            Discardbtn.Text = "DISCARD";
            Discardbtn.UseVisualStyleBackColor = true;
            Discardbtn.Visible = false;
            Discardbtn.Click += Discardbtn_Click;
            // 
            // Savebtn
            // 
            Savebtn.Location = new Point(460, 370);
            Savebtn.Name = "Savebtn";
            Savebtn.Size = new Size(75, 23);
            Savebtn.TabIndex = 9;
            Savebtn.Text = "SAVE";
            Savebtn.UseVisualStyleBackColor = true;
            Savebtn.Visible = false;
            Savebtn.Click += Savebtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(119, 309);
            label1.Name = "label1";
            label1.Size = new Size(104, 15);
            label1.TabIndex = 10;
            label1.Text = "Confirm Password";
            label1.Visible = false;
            // 
            // Seebutton
            // 
            Seebutton.Location = new Point(366, 271);
            Seebutton.Name = "Seebutton";
            Seebutton.Size = new Size(111, 23);
            Seebutton.TabIndex = 11;
            Seebutton.Text = "See Password";
            Seebutton.UseVisualStyleBackColor = true;
            Seebutton.Click += Seebutton_Click;
            // 
            // AccountSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Seebutton);
            Controls.Add(label1);
            Controls.Add(Savebtn);
            Controls.Add(Discardbtn);
            Controls.Add(Editbtn);
            Controls.Add(confirmpwtxtbox);
            Controls.Add(passwordtxtbox);
            Controls.Add(panel1);
            Controls.Add(passwordlabel);
            Controls.Add(emaillabel);
            Name = "AccountSettings";
            Size = new Size(1024, 576);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private Label passwordlabel;
        private Label emaillabel;
        private TextBox passwordtxtbox;
        private TextBox confirmpwtxtbox;
        private Button Editbtn;
        private Button Discardbtn;
        private Button Savebtn;
        private Label label1;
        private Button Seebutton;
    }
}
