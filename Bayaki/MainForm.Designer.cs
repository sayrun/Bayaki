namespace Bayaki
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._dropCover = new System.Windows.Forms.Label();
            this._update = new System.Windows.Forms.Button();
            this._targets = new System.Windows.Forms.ListView();
            this._targetsImage = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._previewImage = new System.Windows.Forms.PictureBox();
            this._previewImageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._removeLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._addLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._rematchingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._previewMap = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this._locationSources = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._locationContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._renameTarckItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportTrackItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportGPX = new System.Windows.Forms.ToolStripMenuItem();
            this._exportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this._exportKML = new System.Windows.Forms.ToolStripMenuItem();
            this._routePreview = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._upPriority = new System.Windows.Forms.ToolStripMenuItem();
            this._downPriority = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._deleteTrackItem = new System.Windows.Forms.ToolStripMenuItem();
            this._locationToolbar = new System.Windows.Forms.ToolStrip();
            this._locationToolbarContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exportFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._previewImage)).BeginInit();
            this._previewImageContextMenu.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this._locationContextMenu.SuspendLayout();
            this._locationToolbarContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(891, 521);
            this.panel1.TabIndex = 1;
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
            this.splitContainer1.Panel1.Controls.Add(this._dropCover);
            this.splitContainer1.Panel1.Controls.Add(this._update);
            this.splitContainer1.Panel1.Controls.Add(this._targets);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(891, 521);
            this.splitContainer1.SplitterDistance = 299;
            this.splitContainer1.TabIndex = 0;
            // 
            // _dropCover
            // 
            this._dropCover.AllowDrop = true;
            this._dropCover.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this._dropCover.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dropCover.Location = new System.Drawing.Point(0, 0);
            this._dropCover.Name = "_dropCover";
            this._dropCover.Size = new System.Drawing.Size(891, 299);
            this._dropCover.TabIndex = 2;
            this._dropCover.Text = "JPEGファイルをここにドロップしてください";
            this._dropCover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._dropCover.DragDrop += new System.Windows.Forms.DragEventHandler(this._dropCover_DragDrop);
            this._dropCover.DragEnter += new System.Windows.Forms.DragEventHandler(this._dropCover_DragEnter);
            // 
            // _update
            // 
            this._update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._update.Enabled = false;
            this._update.Location = new System.Drawing.Point(813, 273);
            this._update.Name = "_update";
            this._update.Size = new System.Drawing.Size(75, 23);
            this._update.TabIndex = 1;
            this._update.Text = "更新保存";
            this._update.UseVisualStyleBackColor = true;
            this._update.Click += new System.EventHandler(this._update_Click);
            // 
            // _targets
            // 
            this._targets.AllowDrop = true;
            this._targets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._targets.CheckBoxes = true;
            this._targets.HideSelection = false;
            this._targets.LargeImageList = this._targetsImage;
            this._targets.Location = new System.Drawing.Point(0, 0);
            this._targets.Name = "_targets";
            this._targets.Size = new System.Drawing.Size(891, 267);
            this._targets.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._targets.TabIndex = 0;
            this._targets.UseCompatibleStateImageBehavior = false;
            this._targets.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this._targets_ItemChecked);
            this._targets.SelectedIndexChanged += new System.EventHandler(this._targets_SelectedIndexChanged);
            this._targets.DragDrop += new System.Windows.Forms.DragEventHandler(this._targets_DragDrop);
            this._targets.DragEnter += new System.Windows.Forms.DragEventHandler(this._dropCover_DragEnter);
            // 
            // _targetsImage
            // 
            this._targetsImage.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this._targetsImage.ImageSize = new System.Drawing.Size(64, 64);
            this._targetsImage.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(891, 218);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(883, 192);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "画像プレビュー";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._previewImage);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._previewMap);
            this.splitContainer2.Size = new System.Drawing.Size(877, 186);
            this.splitContainer2.SplitterDistance = 494;
            this.splitContainer2.TabIndex = 0;
            // 
            // _previewImage
            // 
            this._previewImage.ContextMenuStrip = this._previewImageContextMenu;
            this._previewImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewImage.Location = new System.Drawing.Point(0, 0);
            this._previewImage.Name = "_previewImage";
            this._previewImage.Size = new System.Drawing.Size(494, 186);
            this._previewImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._previewImage.TabIndex = 0;
            this._previewImage.TabStop = false;
            // 
            // _previewImageContextMenu
            // 
            this._previewImageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._removeLocationToolStripMenuItem,
            this._addLocationToolStripMenuItem,
            this._rematchingToolStripMenuItem});
            this._previewImageContextMenu.Name = "_previewImageContextMenu";
            this._previewImageContextMenu.Size = new System.Drawing.Size(161, 70);
            this._previewImageContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._previewImageContextMenu_Opening);
            // 
            // _removeLocationToolStripMenuItem
            // 
            this._removeLocationToolStripMenuItem.Name = "_removeLocationToolStripMenuItem";
            this._removeLocationToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this._removeLocationToolStripMenuItem.Text = "位置情報削除...";
            this._removeLocationToolStripMenuItem.Click += new System.EventHandler(this._removeLocationToolStripMenuItem_Click);
            // 
            // _addLocationToolStripMenuItem
            // 
            this._addLocationToolStripMenuItem.Name = "_addLocationToolStripMenuItem";
            this._addLocationToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this._addLocationToolStripMenuItem.Text = "位置情報追加";
            this._addLocationToolStripMenuItem.Click += new System.EventHandler(this._addLocationToolStripMenuItem_Click);
            // 
            // _rematchingToolStripMenuItem
            // 
            this._rematchingToolStripMenuItem.Enabled = false;
            this._rematchingToolStripMenuItem.Name = "_rematchingToolStripMenuItem";
            this._rematchingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this._rematchingToolStripMenuItem.Text = "位置情報再検索";
            // 
            // _previewMap
            // 
            this._previewMap.AllowWebBrowserDrop = false;
            this._previewMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewMap.IsWebBrowserContextMenuEnabled = false;
            this._previewMap.Location = new System.Drawing.Point(0, 0);
            this._previewMap.MinimumSize = new System.Drawing.Size(20, 20);
            this._previewMap.Name = "_previewMap";
            this._previewMap.ScrollBarsEnabled = false;
            this._previewMap.Size = new System.Drawing.Size(379, 186);
            this._previewMap.TabIndex = 0;
            this._previewMap.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this._previewMap_DocumentCompleted);
            this._previewMap.SizeChanged += new System.EventHandler(this._previewMap_SizeChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._locationSources);
            this.tabPage2.Controls.Add(this._locationToolbar);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(883, 192);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "位置情報元";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // _locationSources
            // 
            this._locationSources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this._locationSources.ContextMenuStrip = this._locationContextMenu;
            this._locationSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this._locationSources.FullRowSelect = true;
            this._locationSources.HideSelection = false;
            this._locationSources.LabelEdit = true;
            this._locationSources.Location = new System.Drawing.Point(3, 28);
            this._locationSources.MultiSelect = false;
            this._locationSources.Name = "_locationSources";
            this._locationSources.ShowItemToolTips = true;
            this._locationSources.Size = new System.Drawing.Size(877, 161);
            this._locationSources.TabIndex = 1;
            this._locationSources.UseCompatibleStateImageBehavior = false;
            this._locationSources.View = System.Windows.Forms.View.Details;
            this._locationSources.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this._locationSources_AfterLabelEdit);
            this._locationSources.DoubleClick += new System.EventHandler(this._routePreview_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 3;
            this.columnHeader1.Text = "名前";
            this.columnHeader1.Width = 349;
            // 
            // columnHeader2
            // 
            this.columnHeader2.DisplayIndex = 0;
            this.columnHeader2.Text = "開始時間";
            this.columnHeader2.Width = 136;
            // 
            // columnHeader3
            // 
            this.columnHeader3.DisplayIndex = 1;
            this.columnHeader3.Text = "終了時間";
            this.columnHeader3.Width = 159;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 2;
            this.columnHeader4.Text = "点数";
            this.columnHeader4.Width = 96;
            // 
            // _locationContextMenu
            // 
            this._locationContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._renameTarckItem,
            this._exportTrackItem,
            this._routePreview,
            this.toolStripSeparator1,
            this._upPriority,
            this._downPriority,
            this.toolStripSeparator2,
            this._deleteTrackItem});
            this._locationContextMenu.Name = "_locationContextMenu";
            this._locationContextMenu.Size = new System.Drawing.Size(137, 148);
            this._locationContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._locationContextMenu_Opening);
            // 
            // _renameTarckItem
            // 
            this._renameTarckItem.Name = "_renameTarckItem";
            this._renameTarckItem.Size = new System.Drawing.Size(136, 22);
            this._renameTarckItem.Text = "名称変更...";
            this._renameTarckItem.Click += new System.EventHandler(this._renameTarckItem_Click);
            // 
            // _exportTrackItem
            // 
            this._exportTrackItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._exportGPX,
            this._exportCSV,
            this._exportKML});
            this._exportTrackItem.Name = "_exportTrackItem";
            this._exportTrackItem.Size = new System.Drawing.Size(136, 22);
            this._exportTrackItem.Text = "出力...";
            // 
            // _exportGPX
            // 
            this._exportGPX.Name = "_exportGPX";
            this._exportGPX.Size = new System.Drawing.Size(101, 22);
            this._exportGPX.Text = "GPX";
            this._exportGPX.Click += new System.EventHandler(this._export_Click);
            // 
            // _exportCSV
            // 
            this._exportCSV.Enabled = false;
            this._exportCSV.Name = "_exportCSV";
            this._exportCSV.Size = new System.Drawing.Size(101, 22);
            this._exportCSV.Text = "CSV";
            this._exportCSV.Click += new System.EventHandler(this._export_Click);
            // 
            // _exportKML
            // 
            this._exportKML.Enabled = false;
            this._exportKML.Name = "_exportKML";
            this._exportKML.Size = new System.Drawing.Size(101, 22);
            this._exportKML.Text = "KML";
            this._exportKML.Click += new System.EventHandler(this._export_Click);
            // 
            // _routePreview
            // 
            this._routePreview.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Bold);
            this._routePreview.Name = "_routePreview";
            this._routePreview.Size = new System.Drawing.Size(136, 22);
            this._routePreview.Text = "道順表示";
            this._routePreview.Click += new System.EventHandler(this._routePreview_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // _upPriority
            // 
            this._upPriority.Name = "_upPriority";
            this._upPriority.Size = new System.Drawing.Size(136, 22);
            this._upPriority.Text = "上へ";
            this._upPriority.Click += new System.EventHandler(this._upPriority_Click);
            // 
            // _downPriority
            // 
            this._downPriority.Name = "_downPriority";
            this._downPriority.Size = new System.Drawing.Size(136, 22);
            this._downPriority.Text = "下へ";
            this._downPriority.Click += new System.EventHandler(this._downPriority_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(133, 6);
            // 
            // _deleteTrackItem
            // 
            this._deleteTrackItem.Name = "_deleteTrackItem";
            this._deleteTrackItem.Size = new System.Drawing.Size(136, 22);
            this._deleteTrackItem.Text = "削除...";
            this._deleteTrackItem.Click += new System.EventHandler(this._deleteTrackItem_Click);
            // 
            // _locationToolbar
            // 
            this._locationToolbar.ContextMenuStrip = this._locationToolbarContextMenu;
            this._locationToolbar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this._locationToolbar.Location = new System.Drawing.Point(3, 3);
            this._locationToolbar.Name = "_locationToolbar";
            this._locationToolbar.Size = new System.Drawing.Size(877, 25);
            this._locationToolbar.TabIndex = 0;
            this._locationToolbar.Text = "toolStrip1";
            // 
            // _locationToolbarContextMenu
            // 
            this._locationToolbarContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripSeparator3,
            this.removeToolStripMenuItem});
            this._locationToolbarContextMenu.Name = "_locationToolbarContextMenu";
            this._locationToolbarContextMenu.Size = new System.Drawing.Size(173, 54);
            this._locationToolbarContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._locationToolbarContextMenu_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.addToolStripMenuItem.Text = "プラグイン追加...";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(169, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.removeToolStripMenuItem.Text = "プラグイン削除...";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 521);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "場所を焼きこむツール（パイロット版）";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._previewImage)).EndInit();
            this._previewImageContextMenu.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this._locationContextMenu.ResumeLayout(false);
            this._locationToolbarContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button _update;
        private System.Windows.Forms.ListView _targets;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStrip _locationToolbar;
        private System.Windows.Forms.ListView _locationSources;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox _previewImage;
        private System.Windows.Forms.WebBrowser _previewMap;
        private System.Windows.Forms.Label _dropCover;
        private System.Windows.Forms.ImageList _targetsImage;
        private System.Windows.Forms.ContextMenuStrip _locationContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _renameTarckItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _upPriority;
        private System.Windows.Forms.ToolStripMenuItem _downPriority;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _deleteTrackItem;
        private System.Windows.Forms.ContextMenuStrip _locationToolbarContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exportTrackItem;
        private System.Windows.Forms.ToolStripMenuItem _exportGPX;
        private System.Windows.Forms.ToolStripMenuItem _exportCSV;
        private System.Windows.Forms.ToolStripMenuItem _exportKML;
        private System.Windows.Forms.SaveFileDialog _exportFileDialog;
        private System.Windows.Forms.ToolStripMenuItem _routePreview;
        private System.Windows.Forms.ContextMenuStrip _previewImageContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _removeLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _addLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _rematchingToolStripMenuItem;
    }
}

