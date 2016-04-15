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
            this._mapView = new MapControlLibrary.MapControl();
            this.SuspendLayout();
            // 
            // _mapView
            // 
            this._mapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mapView.Location = new System.Drawing.Point(0, 0);
            this._mapView.MinimumSize = new System.Drawing.Size(20, 20);
            this._mapView.Name = "_mapView";
            this._mapView.ScrollBarsEnabled = false;
            this._mapView.Size = new System.Drawing.Size(893, 538);
            this._mapView.TabIndex = 0;
            // 
            // TrackPointPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 538);
            this.Controls.Add(this._mapView);
            this.Name = "TrackPointPreviewForm";
            this.Text = "TrackPointPreviewForm";
            this.Load += new System.EventHandler(this.TrackPointPreviewForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MapControlLibrary.MapControl _mapView;
    }
}