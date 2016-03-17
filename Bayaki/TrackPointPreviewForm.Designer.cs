namespace Bayaki
{
    partial class TrackPointPreviewForm
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
            this._routePreview = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // _routePreview
            // 
            this._routePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this._routePreview.Location = new System.Drawing.Point(0, 0);
            this._routePreview.MinimumSize = new System.Drawing.Size(20, 20);
            this._routePreview.Name = "_routePreview";
            this._routePreview.ScrollBarsEnabled = false;
            this._routePreview.Size = new System.Drawing.Size(893, 538);
            this._routePreview.TabIndex = 0;
            this._routePreview.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this._routePreview_DocumentCompleted);
            // 
            // TrackPointPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 538);
            this.Controls.Add(this._routePreview);
            this.Name = "TrackPointPreviewForm";
            this.Text = "TrackPointPreviewForm";
            this.Load += new System.EventHandler(this.TrackPointPreviewForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser _routePreview;
    }
}