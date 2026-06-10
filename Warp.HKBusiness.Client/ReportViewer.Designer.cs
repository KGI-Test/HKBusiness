namespace Warp.HKBusiness.Client
{
    partial class ReportViewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtViewer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtViewer
            // 
            this.txtViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtViewer.Location = new System.Drawing.Point(0, 0);
            this.txtViewer.Multiline = true;
            this.txtViewer.Name = "txtViewer";
            this.txtViewer.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtViewer.Size = new System.Drawing.Size(800, 415);
            this.txtViewer.TabIndex = 0;
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 415);
            this.Controls.Add(this.txtViewer);
            this.Name = "ReportViewer";
            this.Text = "Report Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtViewer;
    }
}
