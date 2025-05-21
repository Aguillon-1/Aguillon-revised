namespace CMS_Revised
{
    partial class LoadingAnimation
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.CMSLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CMSLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // CMSLoading
            // 
            this.CMSLoading.Image = global::CMS_Revised.Properties.Resources.Classroom_Management_System;
            this.CMSLoading.Location = new System.Drawing.Point(23, 12);
            this.CMSLoading.Name = "CMSLoading";
            this.CMSLoading.Size = new System.Drawing.Size(134, 126);
            this.CMSLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CMSLoading.TabIndex = 0;
            this.CMSLoading.TabStop = false;
            // 
            // LoadingAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CMSLoading);
            this.Name = "LoadingAnimation";
            this.Size = new System.Drawing.Size(184, 161);
            ((System.ComponentModel.ISupportInitialize)(this.CMSLoading)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox CMSLoading;
    }
}