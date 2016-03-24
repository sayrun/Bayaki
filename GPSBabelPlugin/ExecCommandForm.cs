using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GPSBabelPlugin
{
    public partial class ExecCommandForm : Form
    {
        private bykIFv1.TrackItem _item;

        public ExecCommandForm()
        {
            InitializeComponent();

            _item = null;
        }

        private void _selExecFile_Click(object sender, EventArgs e)
        {
            if( DialogResult.OK == _openGPSBabelExecFile.ShowDialog( this))
            {
                _execPath.Text = _openGPSBabelExecFile.FileName;
            }
        }

        public bykIFv1.TrackItem Item
        {
            get
            {
                return _item;
            }
        }

        private void _exec_Click(object sender, EventArgs e)
        {
            try
            {
                _exec.Enabled = false;
                _cancel.Enabled = false;
                _selExecFile.Enabled = false;

                if ((0 <= _parameters.Text.IndexOf("-o")) || (0 <= _parameters.Text.IndexOf("-F")))
                {
                    _parameters.Focus();
                    MessageBox.Show(Properties.Resources.MSG1, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                this.UseWaitCursor = true;

                string tempFileName = System.IO.Path.GetTempFileName();

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = _execPath.Text;
                //出力を読み取れるようにする
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.RedirectStandardError = false;
                //ウィンドウを表示しないようにする
                p.StartInfo.CreateNoWindow = true;

                // 一時ファイルに出力させる
                p.StartInfo.Arguments = string.Format("{0} -o gpx -F {1}", _parameters.Text, tempFileName);

                p.Start();
                //出力を読み取る
                string results = p.StandardOutput.ReadToEnd();  // なぜか読めない
                System.Diagnostics.Debug.Print(results);

                //プロセス終了まで待機する
                p.WaitForExit();
                p.Close();

                try
                {
                    // 一時ファイルから読み込む
                    using (Bayaki.TrackItemReader tir = new Bayaki.TrackItemReader(tempFileName))
                    {
                        bykIFv1.TrackItem item = tir.Read();
                        _item = (0 < item.Items.Count) ? item : null;
                    }
                }
                finally
                {
                    // 一時ファイルを削除する
                    System.IO.File.Delete(tempFileName);
                }

                // 設定を更新
                Properties.Settings.Default.GPSBabel_PATH = _execPath.Text;
                Properties.Settings.Default.GPSBabel_Param = _parameters.Text;
                Properties.Settings.Default.Save();

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                this.UseWaitCursor = false;

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                _exec.Enabled = true;
                _cancel.Enabled = true;
                _selExecFile.Enabled = true;

                this.UseWaitCursor = false;
            }
        }

        private void ExecCommandForm_Load(object sender, EventArgs e)
        {
            _execPath.Text = Properties.Settings.Default.GPSBabel_PATH;
            _parameters.Text = Properties.Settings.Default.GPSBabel_Param;
        }
    }
}
