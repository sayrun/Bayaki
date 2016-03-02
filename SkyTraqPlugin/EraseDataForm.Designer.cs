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
            this._OK = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this._eraseWorker = new System.ComponentModel.BackgroundWorker();
            this._progress = new System.Windows.Forms.ProgressBar();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "port";
            // 
            // _posrts
            // 
            this._posrts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._posrts.FormattingEnabled = true;
            this._posrts.Location = new System.Drawing.Point(93, 12);
            this._posrts.Name = "_posrts";
            this._posrts.Size = new System.Drawing.Size(121, 20);
            this._posrts.TabIndex = 1;
            this._posrts.SelectedIndexChanged += new System.EventHandler(this._posrts_SelectedIndexChanged);
            // 
            // _OK
            // 
            this._OK.Location = new System.Drawing.Point(116, 72);
            this._OK.Name = "_OK";
            this._OK.Size = new System.Drawing.Size(75, 23);
            this._OK.TabIndex = 2;
            this._OK.Text = "OK";
            this._OK.UseVisualStyleBackColor = true;
            this._OK.Click += new System.EventHandler(this._OK_Click);
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(197, 72);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 2;
            this._cancel.Text = "cancel";
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
            this._progress.Location = new System.Drawing.Point(93, 43);
            this._progress.MarqueeAnimationSpeed = 1000;
            this._progress.Maximum = 1000;
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(179, 23);
            this._progress.TabIndex = 3;
            // 
            // _timer
            // 
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // EraseDataForm
            // 
            this.AcceptButton = this._OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancel;
            this.ClientSize = new System.Drawing.Size(284, 107);
            this.ControlBox = false;
            this.Controls.Add(this._progress);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._OK);
            this.Controls.Add(this._posrts);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EraseDataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "データ消去";
            this.Load += new System.EventHandler(this.SelectPortForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _posrts;
        private System.Windows.Forms.Button _OK;
        private System.Windows.Forms.Button _cancel;
        private System.ComponentModel.BackgroundWorker _eraseWorker;
        private System.Windows.Forms.ProgressBar _progress;
        private System.Windows.Forms.Timer _timer;
    }
}