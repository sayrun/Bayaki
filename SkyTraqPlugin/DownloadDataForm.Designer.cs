namespace SkyTraqPlugin
{
    partial class DownloadDataForm
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
            this._bufferStatus = new System.Windows.Forms.ProgressBar();
            this._cancel = new System.Windows.Forms.Button();
            this._download = new System.Windows.Forms.Button();
            this._posrts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._connect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._kernelVersion = new System.Windows.Forms.TextBox();
            this._ODMVersion = new System.Windows.Forms.TextBox();
            this._Revision = new System.Windows.Forms.TextBox();
            this._progress = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this._downloadWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // _bufferStatus
            // 
            this._bufferStatus.Location = new System.Drawing.Point(74, 43);
            this._bufferStatus.Name = "_bufferStatus";
            this._bufferStatus.Size = new System.Drawing.Size(352, 23);
            this._bufferStatus.TabIndex = 8;
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(351, 227);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 6;
            this._cancel.Text = "cancel";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // _download
            // 
            this._download.Enabled = false;
            this._download.Location = new System.Drawing.Point(74, 100);
            this._download.Name = "_download";
            this._download.Size = new System.Drawing.Size(75, 23);
            this._download.TabIndex = 7;
            this._download.Text = "download";
            this._download.UseVisualStyleBackColor = true;
            this._download.Click += new System.EventHandler(this._download_Click);
            // 
            // _posrts
            // 
            this._posrts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._posrts.FormattingEnabled = true;
            this._posrts.Location = new System.Drawing.Point(74, 12);
            this._posrts.Name = "_posrts";
            this._posrts.Size = new System.Drawing.Size(121, 20);
            this._posrts.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "port";
            // 
            // _connect
            // 
            this._connect.Location = new System.Drawing.Point(201, 10);
            this._connect.Name = "_connect";
            this._connect.Size = new System.Drawing.Size(75, 23);
            this._connect.TabIndex = 9;
            this._connect.Text = "接続";
            this._connect.UseVisualStyleBackColor = true;
            this._connect.Click += new System.EventHandler(this._connect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "buffer";
            // 
            // _kernelVersion
            // 
            this._kernelVersion.Location = new System.Drawing.Point(74, 72);
            this._kernelVersion.Name = "_kernelVersion";
            this._kernelVersion.ReadOnly = true;
            this._kernelVersion.Size = new System.Drawing.Size(100, 19);
            this._kernelVersion.TabIndex = 10;
            this._kernelVersion.Text = "Kernel Version";
            // 
            // _ODMVersion
            // 
            this._ODMVersion.Location = new System.Drawing.Point(180, 72);
            this._ODMVersion.Name = "_ODMVersion";
            this._ODMVersion.ReadOnly = true;
            this._ODMVersion.Size = new System.Drawing.Size(100, 19);
            this._ODMVersion.TabIndex = 10;
            this._ODMVersion.Text = "ODM Version";
            // 
            // _Revision
            // 
            this._Revision.Location = new System.Drawing.Point(286, 72);
            this._Revision.Name = "_Revision";
            this._Revision.ReadOnly = true;
            this._Revision.Size = new System.Drawing.Size(100, 19);
            this._Revision.TabIndex = 10;
            this._Revision.Text = "Revision";
            // 
            // _progress
            // 
            this._progress.Location = new System.Drawing.Point(74, 129);
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(352, 23);
            this._progress.TabIndex = 8;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(74, 158);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(352, 23);
            this.progressBar2.TabIndex = 8;
            // 
            // _downloadWorker
            // 
            this._downloadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._downloadWorker_DoWork);
            // 
            // DownloadDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 262);
            this.ControlBox = false;
            this.Controls.Add(this._Revision);
            this.Controls.Add(this._ODMVersion);
            this.Controls.Add(this._kernelVersion);
            this.Controls.Add(this._connect);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this._progress);
            this.Controls.Add(this._bufferStatus);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._download);
            this.Controls.Add(this._posrts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DownloadDataForm";
            this.ShowInTaskbar = false;
            this.Text = "データダウンロード";
            this.Load += new System.EventHandler(this.DownloadDataForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar _bufferStatus;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Button _download;
        private System.Windows.Forms.ComboBox _posrts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _connect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _kernelVersion;
        private System.Windows.Forms.TextBox _ODMVersion;
        private System.Windows.Forms.TextBox _Revision;
        private System.Windows.Forms.ProgressBar _progress;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.ComponentModel.BackgroundWorker _downloadWorker;
    }
}