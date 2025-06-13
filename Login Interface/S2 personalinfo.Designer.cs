namespace CMS_Revised.Login_Interface
{
    partial class S2_personalinfo
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
            S2Backbutton = new Button();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            Sexcombobox = new ComboBox();
            label5 = new Label();
            label4 = new Label();
            S2Nextbutton = new Button();
            Birthdaypicker = new DateTimePicker();
            Emailtextbox = new TextBox();
            Contactnotextbox = new TextBox();
            Addresstextbox = new TextBox();
            SuspendLayout();
            // 
            // S2Backbutton
            // 
            S2Backbutton.Location = new Point(71, 457);
            S2Backbutton.Name = "S2Backbutton";
            S2Backbutton.Size = new Size(112, 44);
            S2Backbutton.TabIndex = 37;
            S2Backbutton.Text = "Back";
            S2Backbutton.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(66, 247);
            label8.Name = "label8";
            label8.Size = new Size(74, 21);
            label8.TabIndex = 35;
            label8.Text = "Address:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(65, 168);
            label7.Name = "label7";
            label7.Size = new Size(52, 21);
            label7.TabIndex = 33;
            label7.Text = "Email:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(317, 168);
            label6.Name = "label6";
            label6.Size = new Size(133, 21);
            label6.TabIndex = 31;
            label6.Text = "Contact number:";
            // 
            // Sexcombobox
            // 
            Sexcombobox.FormattingEnabled = true;
            Sexcombobox.Location = new Point(318, 110);
            Sexcombobox.Name = "Sexcombobox";
            Sexcombobox.Size = new Size(185, 23);
            Sexcombobox.TabIndex = 30;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(318, 86);
            label5.Name = "label5";
            label5.Size = new Size(40, 21);
            label5.TabIndex = 29;
            label5.Text = "Sex:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(66, 86);
            label4.Name = "label4";
            label4.Size = new Size(75, 21);
            label4.TabIndex = 27;
            label4.Text = "Birthday:";
            // 
            // S2Nextbutton
            // 
            S2Nextbutton.Location = new Point(491, 457);
            S2Nextbutton.Name = "S2Nextbutton";
            S2Nextbutton.Size = new Size(112, 44);
            S2Nextbutton.TabIndex = 23;
            S2Nextbutton.Text = "Next";
            S2Nextbutton.UseVisualStyleBackColor = true;
            // 
            // Birthdaypicker
            // 
            Birthdaypicker.Location = new Point(66, 110);
            Birthdaypicker.Name = "Birthdaypicker";
            Birthdaypicker.Size = new Size(220, 23);
            Birthdaypicker.TabIndex = 38;
            // 
            // Emailtextbox
            // 
            Emailtextbox.Location = new Point(66, 192);
            Emailtextbox.Name = "Emailtextbox";
            Emailtextbox.Size = new Size(220, 23);
            Emailtextbox.TabIndex = 39;
            // 
            // Contactnotextbox
            // 
            Contactnotextbox.Location = new Point(318, 192);
            Contactnotextbox.Name = "Contactnotextbox";
            Contactnotextbox.Size = new Size(220, 23);
            Contactnotextbox.TabIndex = 40;
            // 
            // Addresstextbox
            // 
            Addresstextbox.Location = new Point(66, 271);
            Addresstextbox.Name = "Addresstextbox";
            Addresstextbox.Size = new Size(472, 23);
            Addresstextbox.TabIndex = 41;
            // 
            // S2_personalinfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonHighlight;
            Controls.Add(Addresstextbox);
            Controls.Add(Contactnotextbox);
            Controls.Add(Emailtextbox);
            Controls.Add(Birthdaypicker);
            Controls.Add(S2Backbutton);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(Sexcombobox);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(S2Nextbutton);
            Name = "S2_personalinfo";
            Size = new Size(675, 573);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button S2Backbutton;
        private Label label8;
        private Label label7;
        private Label label6;
        private ComboBox Sexcombobox;
        private Label label5;
        private Label label4;
        private Button S2Nextbutton;
        private DateTimePicker Birthdaypicker;
        private TextBox Emailtextbox;
        private TextBox Contactnotextbox;
        private TextBox Addresstextbox;
    }
}
