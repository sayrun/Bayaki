namespace SkyTraqPlugin
{
    partial class EraseDataForm
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
            this.label1 = new System.Windows.Forms.Label();
            this._posrts = new System.Windows.Forms.ComboBox();
            this._erase = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this._eraseWorker = new System.ComponentModel.BackgroundWorker();
            this._progress = new System.Windows.Forms.ProgressBar();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ポート";
            // 
            // _posrts
            // 
            this._posrts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._posrts.FormattingEnabled = true;
            this._posrts.Location = new System.Drawing.Point(104, 12);
            this._posrts.Name = "_posrts";
            this._posrts.Size = new System.Drawing.Size(121, 20);
            this._posrts.TabIndex = 1;
            this._posrts.SelectedIndexChanged += new System.EventHandler(this._posrts_SelectedIndexChanged);
            // 
            // _erase
            // 
            this._erase.Location = new System.Drawing.Point(69, 89);
            this._erase.Name = "_erase";
            this._erase.Size = new System.Drawing.Size(75, 23);
            this._erase.TabIndex = 2;
            this._erase.Text = "消去";
            this._erase.UseVisualStyleBackColor = true;
            this._erase.Click += new System.EventHandler(this._erase_Click);
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(150, 89);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 2;
            this._cancel.Text = "閉じる";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // _eraseWorker
            // 
            this._eraseWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._eraseWorker_DoWork);
            this._eraseWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._eraseWorker_RunWorkerCompleted);
            // 
            // _progress
            // 
            this._progress.Location = new System.Drawing.Point(23, 38);
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(202, 23);
            this._progress.TabIndex = 3;
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // EraseDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancel;
            this.ClientSize = new System.Drawing.Size(237, 124);
            this.ControlBox = false;
            this.Controls.Add(this._progress);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._erase);
            this.Controls.Add(this._posrts);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EraseDataForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "データ消去";
            this.Load += new System.EventHandler(this.SelectPortForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _posrts;
        private System.Windows.Forms.Button _erase;
        private System.Windows.Forms.Button _cancel;
        private System.ComponentModel.BackgroundWorker _eraseWorker;
        private System.Windows.Forms.ProgressBar _progress;
        private System.Windows.Forms.Timer _timer;
    }
}