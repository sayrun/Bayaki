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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._mapView = new MapControlLibrary.MapControl();
            this._graphView = new GraphControlLibrary.GraphControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.range10kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.range25kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.range50kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.range100kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.range200kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.range300kmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
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
            this._graphView.ContextMenuStrip = this.contextMenuStrip1;
            this._graphView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._graphView.Location = new System.Drawing.Point(0, 0);
            this._graphView.Name = "_graphView";
            this._graphView.Size = new System.Drawing.Size(893, 127);
            this._graphView.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.range10kmToolStripMenuItem,
            this.range25kmToolStripMenuItem,
            this.range50kmToolStripMenuItem,
            this.range100kmToolStripMenuItem,
            this.range200kmToolStripMenuItem,
            this.range300kmToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 136);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // range10kmToolStripMenuItem
            // 
            this.range10kmToolStripMenuItem.Name = "range10kmToolStripMenuItem";
            this.range10kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range10kmToolStripMenuItem.Tag = 10;
            this.range10kmToolStripMenuItem.Text = "～10km";
            this.range10kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
            // 
            // range25kmToolStripMenuItem
            // 
            this.range25kmToolStripMenuItem.Name = "range25kmToolStripMenuItem";
            this.range25kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range25kmToolStripMenuItem.Tag = 25;
            this.range25kmToolStripMenuItem.Text = "～25km";
            this.range25kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
            // 
            // range50kmToolStripMenuItem
            // 
            this.range50kmToolStripMenuItem.Name = "range50kmToolStripMenuItem";
            this.range50kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range50kmToolStripMenuItem.Tag = 50;
            this.range50kmToolStripMenuItem.Text = "～50km";
            this.range50kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
            // 
            // range100kmToolStripMenuItem
            // 
            this.range100kmToolStripMenuItem.Name = "range100kmToolStripMenuItem";
            this.range100kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range100kmToolStripMenuItem.Tag = 100;
            this.range100kmToolStripMenuItem.Text = "～100km";
            this.range100kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
            // 
            // range200kmToolStripMenuItem
            // 
            this.range200kmToolStripMenuItem.Name = "range200kmToolStripMenuItem";
            this.range200kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range200kmToolStripMenuItem.Tag = 200;
            this.range200kmToolStripMenuItem.Text = "～200km";
            this.range200kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
            // 
            // range300kmToolStripMenuItem
            // 
            this.range300kmToolStripMenuItem.Name = "range300kmToolStripMenuItem";
            this.range300kmToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.range300kmToolStripMenuItem.Tag = 300;
            this.range300kmToolStripMenuItem.Text = "～300km";
            this.range300kmToolStripMenuItem.Click += new System.EventHandler(this.rangeXXkmToolStripMenuItem_Click);
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private MapControlLibrary.MapControl _mapView;
        private GraphControlLibrary.GraphControl _graphView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem range10kmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem range25kmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem range50kmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem range100kmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem range200kmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem range300kmToolStripMenuItem;
    }
}