namespace SkyTraqPlugin
{
    partial class SetEphemerisForm
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
            this._host = new System.Windows.Forms.TextBox();
            this._username = new System.Windows.Forms.TextBox();
            this._password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._cancel = new System.Windows.Forms.Button();
            this._start = new System.Windows.Forms.Button();
            this._posrts = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this._phase = new System.Windows.Forms.TextBox();
            this._progress = new System.Windows.Forms.ProgressBar();
            this._setEphemerisWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // _host
            // 
            this._host.Location = new System.Drawing.Point(157, 12);
            this._host.Name = "_host";
            this._host.Size = new System.Drawing.Size(198, 19);
            this._host.TabIndex = 0;
            this._host.Text = "60.250.205.31";
            // 
            // _username
            // 
            this._username.Location = new System.Drawing.Point(157, 37);
            this._username.Name = "_username";
            this._username.Size = new System.Drawing.Size(119, 19);
            this._username.TabIndex = 0;
            this._username.Text = "skytraq";
            // 
            // _password
            // 
            this._password.Location = new System.Drawing.Point(157, 62);
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(119, 19);
            this._password.TabIndex = 0;
            this._password.Text = "skytraq";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ホスト";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(99, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "パスワード";
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(403, 226);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 2;
            this._cancel.Text = "閉じる";
            this._cancel.UseVisualStyleBackColor = true;
            // 
            // _start
            // 
            this._start.Location = new System.Drawing.Point(157, 127);
            this._start.Name = "_start";
            this._start.Size = new System.Drawing.Size(75, 23);
            this._start.TabIndex = 2;
            this._start.Text = "開始";
            this._start.UseVisualStyleBackColor = true;
            this._start.Click += new System.EventHandler(this._start_Click);
            // 
            // _posrts
            // 
            this._posrts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._posrts.FormattingEnabled = true;
            this._posrts.Location = new System.Drawing.Point(157, 101);
            this._posrts.Name = "_posrts";
            this._posrts.Size = new System.Drawing.Size(121, 20);
            this._posrts.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "ポート";
            // 
            // _phase
            // 
            this._phase.Location = new System.Drawing.Point(157, 156);
            this._phase.Name = "_phase";
            this._phase.ReadOnly = true;
            this._phase.Size = new System.Drawing.Size(100, 19);
            this._phase.TabIndex = 12;
            this._phase.Text = "phase";
            // 
            // _progress
            // 
            this._progress.Location = new System.Drawing.Point(157, 181);
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(321, 23);
            this._progress.TabIndex = 11;
            // 
            // _setEphemerisWorker
            // 
            this._setEphemerisWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._setEphemerisWorker_DoWork);
            this._setEphemerisWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._setEphemerisWorker_RunWorkerCompleted);
            // 
            // SetEphemerisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancel;
            this.ClientSize = new System.Drawing.Size(490, 261);
            this.Controls.Add(this._phase);
            this.Controls.Add(this._progress);
            this.Controls.Add(this._posrts);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._start);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._password);
            this.Controls.Add(this._username);
            this.Controls.Add(this._host);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetEphemerisForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "エフェメリス更新";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SetEphemerisForm_FormClosed);
            this.Load += new System.EventHandler(this.SetEphemerisForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _host;
        private System.Windows.Forms.TextBox _username;
        private System.Windows.Forms.TextBox _password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Button _start;
        private System.Windows.Forms.ComboBox _posrts;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _phase;
        private System.Windows.Forms.ProgressBar _progress;
        private System.ComponentModel.BackgroundWorker _setEphemerisWorker;
    }
}