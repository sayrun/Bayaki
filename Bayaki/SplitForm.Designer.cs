namespace Bayaki
{
    partial class SplitForm
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
            this.byDateTime = new System.Windows.Forms.RadioButton();
            this.byDistance = new System.Windows.Forms.RadioButton();
            this.timeValue = new System.Windows.Forms.NumericUpDown();
            this.distanceValue = new System.Windows.Forms.NumericUpDown();
            this.preview = new System.Windows.Forms.Button();
            this._splitItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._OK = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.timeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.distanceValue)).BeginInit();
            this.SuspendLayout();
            // 
            // byDateTime
            // 
            this.byDateTime.AutoSize = true;
            this.byDateTime.Location = new System.Drawing.Point(12, 12);
            this.byDateTime.Name = "byDateTime";
            this.byDateTime.Size = new System.Drawing.Size(47, 16);
            this.byDateTime.TabIndex = 0;
            this.byDateTime.Text = "時間";
            this.byDateTime.UseVisualStyleBackColor = true;
            this.byDateTime.CheckedChanged += new System.EventHandler(this.byDateTime_CheckedChanged);
            // 
            // byDistance
            // 
            this.byDistance.AutoSize = true;
            this.byDistance.Location = new System.Drawing.Point(12, 37);
            this.byDistance.Name = "byDistance";
            this.byDistance.Size = new System.Drawing.Size(47, 16);
            this.byDistance.TabIndex = 0;
            this.byDistance.Text = "距離";
            this.byDistance.UseVisualStyleBackColor = true;
            this.byDistance.CheckedChanged += new System.EventHandler(this.byDistance_CheckedChanged);
            // 
            // timeValue
            // 
            this.timeValue.Enabled = false;
            this.timeValue.Location = new System.Drawing.Point(65, 12);
            this.timeValue.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.timeValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeValue.Name = "timeValue";
            this.timeValue.Size = new System.Drawing.Size(120, 19);
            this.timeValue.TabIndex = 1;
            this.timeValue.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // distanceValue
            // 
            this.distanceValue.Enabled = false;
            this.distanceValue.Location = new System.Drawing.Point(65, 37);
            this.distanceValue.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.distanceValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.distanceValue.Name = "distanceValue";
            this.distanceValue.Size = new System.Drawing.Size(120, 19);
            this.distanceValue.TabIndex = 1;
            this.distanceValue.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // preview
            // 
            this.preview.Enabled = false;
            this.preview.Location = new System.Drawing.Point(247, 34);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(75, 23);
            this.preview.TabIndex = 2;
            this.preview.Text = "分割";
            this.preview.UseVisualStyleBackColor = true;
            this.preview.Click += new System.EventHandler(this.preview_Click);
            // 
            // _splitItems
            // 
            this._splitItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this._splitItems.Enabled = false;
            this._splitItems.FullRowSelect = true;
            this._splitItems.Location = new System.Drawing.Point(12, 62);
            this._splitItems.Name = "_splitItems";
            this._splitItems.Size = new System.Drawing.Size(588, 251);
            this._splitItems.TabIndex = 3;
            this._splitItems.UseCompatibleStateImageBehavior = false;
            this._splitItems.View = System.Windows.Forms.View.Details;
            this._splitItems.DoubleClick += new System.EventHandler(this._splitItems_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.DisplayIndex = 3;
            this.columnHeader1.Text = "名前";
            this.columnHeader1.Width = 218;
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
            this.columnHeader3.Width = 136;
            // 
            // columnHeader4
            // 
            this.columnHeader4.DisplayIndex = 2;
            this.columnHeader4.Text = "点数";
            this.columnHeader4.Width = 66;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(191, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "km";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "時間";
            // 
            // _OK
            // 
            this._OK.Enabled = false;
            this._OK.Location = new System.Drawing.Point(444, 319);
            this._OK.Name = "_OK";
            this._OK.Size = new System.Drawing.Size(75, 23);
            this._OK.TabIndex = 5;
            this._OK.Text = "OK";
            this._OK.UseVisualStyleBackColor = true;
            this._OK.Click += new System.EventHandler(this._OK_Click);
            // 
            // _cancel
            // 
            this._cancel.Location = new System.Drawing.Point(525, 319);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 5;
            this._cancel.Text = "cancel";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // SplitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 354);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._OK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._splitItems);
            this.Controls.Add(this.preview);
            this.Controls.Add(this.distanceValue);
            this.Controls.Add(this.timeValue);
            this.Controls.Add(this.byDistance);
            this.Controls.Add(this.byDateTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplitForm";
            this.ShowInTaskbar = false;
            this.Text = "SplitForm";
            this.Load += new System.EventHandler(this.SplitForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.timeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.distanceValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton byDateTime;
        private System.Windows.Forms.RadioButton byDistance;
        private System.Windows.Forms.NumericUpDown timeValue;
        private System.Windows.Forms.NumericUpDown distanceValue;
        private System.Windows.Forms.Button preview;
        private System.Windows.Forms.ListView _splitItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button _OK;
        private System.Windows.Forms.Button _cancel;
    }
}