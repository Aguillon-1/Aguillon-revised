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
            S1Cancelbutton = new Button();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            comboBox1 = new ComboBox();
            label5 = new Label();
            label4 = new Label();
            S1Nextbutton = new Button();
            dateTimePicker1 = new DateTimePicker();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            SuspendLayout();
            // 
            // S1Cancelbutton
            // 
            S1Cancelbutton.Location = new Point(71, 457);
            S1Cancelbutton.Name = "S1Cancelbutton";
            S1Cancelbutton.Size = new Size(112, 44);
            S1Cancelbutton.TabIndex = 37;
            S1Cancelbutton.Text = "Cancel";
            S1Cancelbutton.UseVisualStyleBackColor = true;
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
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(318, 110);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(185, 23);
            comboBox1.TabIndex = 30;
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
            // S1Nextbutton
            // 
            S1Nextbutton.Location = new Point(491, 457);
            S1Nextbutton.Name = "S1Nextbutton";
            S1Nextbutton.Size = new Size(112, 44);
            S1Nextbutton.TabIndex = 23;
            S1Nextbutton.Text = "Next";
            S1Nextbutton.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(66, 110);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(220, 23);
            dateTimePicker1.TabIndex = 38;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(66, 192);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(220, 23);
            textBox1.TabIndex = 39;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(318, 192);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(220, 23);
            textBox2.TabIndex = 40;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(66, 271);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(472, 23);
            textBox3.TabIndex = 41;
            // 
            // S2_personalinfo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(dateTimePicker1);
            Controls.Add(S1Cancelbutton);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(comboBox1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(S1Nextbutton);
            Name = "S2_personalinfo";
            Size = new Size(675, 573);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button S1Cancelbutton;
        private Label label8;
        private Label label7;
        private Label label6;
        private ComboBox comboBox1;
        private Label label5;
        private Label label4;
        private Button S1Nextbutton;
        private DateTimePicker dateTimePicker1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
    }
}
