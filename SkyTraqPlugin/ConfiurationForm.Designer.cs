namespace SkyTraqPlugin
{
    partial class ConfiurationForm
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
            this._Revision = new System.Windows.Forms.TextBox();
            this._ODMVersion = new System.Windows.Forms.TextBox();
            this._kernelVersion = new System.Windows.Forms.TextBox();
            this._connect = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this._update = new System.Windows.Forms.Button();
            this._posrts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._threshold = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this._speed = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this._thresholdLabel = new System.Windows.Forms.Label();
            this._thresholdType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._speed)).BeginInit();
            this.SuspendLayout();
            // 
            // _Revision
            // 
            this._Revision.Location = new System.Drawing.Point(291, 40);
            this._Revision.Name = "_Revision";
            this._Revision.ReadOnly = true;
            this._Revision.Size = new System.Drawing.Size(100, 19);
            this._Revision.TabIndex = 18;
            this._Revision.Text = "Revision";
            // 
            // _ODMVersion
            // 
            this._ODMVersion.Location = new System.Drawing.Point(185, 40);
            this._ODMVersion.Name = "_ODMVersion";
            this._ODMVersion.ReadOnly = true;
            this._ODMVersion.Size = new System.Drawing.Size(100, 19);
            this._ODMVersion.TabIndex = 19;
            this._ODMVersion.Text = "ODM Version";
            // 
            // _kernelVersion
            // 
            this._kernelVersion.Location = new System.Drawing.Point(79, 40);
            this._kernelVersion.Name = "_kernelVersion";
            this._kernelVersion.ReadOnly = true;
            this._kernelVersion.Size = new System.Drawing.Size(100, 19);
            this._kernelVersion.TabIndex = 20;
            this._kernelVersion.Text = "Kernel Version";
            // 
            // _connect
            // 
            this._connect.Location = new System.Drawing.Point(206, 12);
            this._connect.Name = "_connect";
            this._connect.Size = new System.Drawing.Size(75, 23);
            this._connect.TabIndex = 17;
            this._connect.Text = "接続";
            this._connect.UseVisualStyleBackColor = true;
            this._connect.Click += new System.EventHandler(this._connect_Click);
            // 
            // _cancel
            // 
            this._cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancel.Location = new System.Drawing.Point(356, 229);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(75, 23);
            this._cancel.TabIndex = 14;
            this._cancel.Text = "閉じる";
            this._cancel.UseVisualStyleBackColor = true;
            // 
            // _update
            // 
            this._update.Enabled = false;
            this._update.Location = new System.Drawing.Point(169, 161);
            this._update.Name = "_update";
            this._update.Size = new System.Drawing.Size(75, 23);
            this._update.TabIndex = 15;
            this._update.Text = "設定";
            this._update.UseVisualStyleBackColor = true;
            this._update.Click += new System.EventHandler(this._update_Click);
            // 
            // _posrts
            // 
            this._posrts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._posrts.FormattingEnabled = true;
            this._posrts.Location = new System.Drawing.Point(79, 14);
            this._posrts.Name = "_posrts";
            this._posrts.Size = new System.Drawing.Size(121, 20);
            this._posrts.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "バージョン";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "ポート";
            // 
            // _threshold
            // 
            this._threshold.Enabled = false;
            this._threshold.Location = new System.Drawing.Point(169, 111);
            this._threshold.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this._threshold.Name = "_threshold";
            this._threshold.Size = new System.Drawing.Size(80, 19);
            this._threshold.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 138);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "速度";
            // 
            // _speed
            // 
            this._speed.Enabled = false;
            this._speed.Location = new System.Drawing.Point(169, 136);
            this._speed.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this._speed.Name = "_speed";
            this._speed.Size = new System.Drawing.Size(80, 19);
            this._speed.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(255, 138);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "km/h";
            // 
            // _thresholdLabel
            // 
            this._thresholdLabel.AutoSize = true;
            this._thresholdLabel.Location = new System.Drawing.Point(255, 113);
            this._thresholdLabel.Name = "_thresholdLabel";
            this._thresholdLabel.Size = new System.Drawing.Size(17, 12);
            this._thresholdLabel.TabIndex = 11;
            this._thresholdLabel.Text = "秒";
            // 
            // _thresholdType
            // 
            this._thresholdType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._thresholdType.Enabled = false;
            this._thresholdType.FormattingEnabled = true;
            this._thresholdType.Items.AddRange(new object[] {
            "時間",
            "距離"});
            this._thresholdType.Location = new System.Drawing.Point(79, 110);
            this._thresholdType.Name = "_thresholdType";
            this._thresholdType.Size = new System.Drawing.Size(84, 20);
            this._thresholdType.TabIndex = 22;
            this._thresholdType.SelectedIndexChanged += new System.EventHandler(this._thresholdType_SelectedIndexChanged);
            // 
            // ConfiurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 262);
            this.Controls.Add(this._thresholdType);
            this.Controls.Add(this._speed);
            this.Controls.Add(this._threshold);
            this.Controls.Add(this._Revision);
            this.Controls.Add(this._ODMVersion);
            this.Controls.Add(this._kernelVersion);
            this.Controls.Add(this._connect);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._update);
            this.Controls.Add(this._posrts);
            this.Controls.Add(this._thresholdLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfiurationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "設定変更";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfiurationForm_FormClosed);
            this.Load += new System.EventHandler(this.ConfiurationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._speed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _Revision;
        private System.Windows.Forms.TextBox _ODMVersion;
        private System.Windows.Forms.TextBox _kernelVersion;
        private System.Windows.Forms.Button _connect;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Button _update;
        private System.Windows.Forms.ComboBox _posrts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown _threshold;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown _speed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label _thresholdLabel;
        private System.Windows.Forms.ComboBox _thresholdType;
    }
}