namespace GPSBabelPlugin
{
    partial class ExecCommandForm
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
            this._openGPSBabelExecFile = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this._execPath = new System.Windows.Forms.TextBox();
            this._selExecFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._exec = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._parameters = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _openGPSBabelExecFile
            // 
            this._openGPSBabelExecFile.DefaultExt = "exe";
            this._openGPSBabelExecFile.FileName = "GPSBabel.exe";
            this._openGPSBabelExecFile.Filter = "executable file(*.exe)|*.exe|all(*.*)|*.*";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "GPSBabel.exeのパス";
            // 
            // _execPath
            // 
            this._execPath.Location = new System.Drawing.Point(146, 12);
            this._execPath.Name = "_execPath";
            this._execPath.Size = new System.Drawing.Size(541, 19);
            this._execPath.TabIndex = 1;
            // 
            // _selExecFile
            // 
            this._selExecFile.Location = new System.Drawing.Point(693, 10);
            this._selExecFile.Name = "_selExecFile";
            this._selExecFile.Size = new System.Drawing.Size(75, 23);
            this._selExecFile.TabIndex = 2;
            this._selExecFile.Text = "sel...";
            this._selExecFile.UseVisualStyleBackColor = true;
            this._selExecFile.Click += new System.EventHandler(this._selExecFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "起動時のパラメータ";
            // 
            // _exec
            // 
            this._exec.Location = new System.Drawing.Point(531, 117);
            this._exec.Name = "_exec";
            this._exec.Size = new System.Drawing.Size(75, 23);
            this._exec.TabIndex = 2;
            this._exec.Text = "実行";
            this._exec.UseVisualStyleBackColor = true;
            this._exec.Click += new System.EventHandler(this._exec_Click);
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(693, 117);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 2;
            this._cancel.Text = "cancel";
            this._cancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(307, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "※-o -Fはプログラム側で付加しますので、設定しないでください。";
            // 
            // _parameters
            // 
            this._parameters.FormattingEnabled = true;
            this._parameters.Location = new System.Drawing.Point(146, 57);
            this._parameters.Name = "_parameters";
            this._parameters.Size = new System.Drawing.Size(541, 20);
            this._parameters.TabIndex = 3;
            // 
            // ExecCommandForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancel;
            this.ClientSize = new System.Drawing.Size(780, 152);
            this.ControlBox = false;
            this.Controls.Add(this._parameters);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._exec);
            this.Controls.Add(this._selExecFile);
            this.Controls.Add(this._execPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExecCommandForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GPSBabel.exeで一時ファイルを作成して取り込むプラグイン（なぜかCONからリダイレクトできない）";
            this.Load += new System.EventHandler(this.ExecCommandForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog _openGPSBabelExecFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _execPath;
        private System.Windows.Forms.Button _selExecFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _exec;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _parameters;
    }
}