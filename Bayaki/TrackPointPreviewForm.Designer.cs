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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._mapView = new MapControlLibrary.MapControl();
            this._graphView = new GraphControlLibrary.GraphControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._mapView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._graphView);
            this.splitContainer1.Size = new System.Drawing.Size(893, 538);
            this.splitContainer1.SplitterDistance = 407;
            this.splitContainer1.TabIndex = 0;
            // 
            // _mapView
            // 
            this._mapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mapView.Location = new System.Drawing.Point(0, 0);
            this._mapView.MinimumSize = new System.Drawing.Size(20, 20);
            this._mapView.Name = "_mapView";
            this._mapView.Size = new System.Drawing.Size(893, 407);
            this._mapView.TabIndex = 0;
            // 
            // _graphView
            // 
            this._graphView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._graphView.Location = new System.Drawing.Point(0, 0);
            this._graphView.Name = "_graphView";
            this._graphView.Size = new System.Drawing.Size(893, 127);
            this._graphView.TabIndex = 0;
            // 
            // TrackPointPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 538);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TrackPointPreviewForm";
            this.Text = "TrackPointPreviewForm";
            this.Load += new System.EventHandler(this.TrackPointPreviewForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private MapControlLibrary.MapControl _mapView;
        private GraphControlLibrary.GraphControl _graphView;
    }
}