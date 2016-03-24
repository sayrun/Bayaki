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

                // 受け渡し用の一時ファイルを作成します
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

                // 成功していたら設定を反映させます。
                RollParamList(_parameters.Text);
                Properties.Settings.Default.GPSBabel_PATH = _execPath.Text;
                Properties.Settings.Default.GPSBabel_Param0 = _parameters.Text;
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

            for (int index = 0; index < 10; ++index)
            {
                string propName = string.Format("GPSBabel_Param{0}", index);

                string propValue = Properties.Settings.Default[propName] as string;

                if (null == propValue) continue;
                if (0 >= propValue.Length) continue;

                _parameters.Items.Add(propValue);
            }

            _parameters.SelectedIndex = 0;
        }

        private void RollParamList( string param)
        {
            if (0 >= param.Length) return;

            int findIndex = -1;
            for (int index = 0; index < 10; ++index)
            {
                string propName = string.Format("GPSBabel_Param{0}", index);

                string propValue = Properties.Settings.Default[propName] as string;

                findIndex = index;
                if (param == propValue)
                {
                    // 最初の項目が一致するなら更新は不要です
                    if (0 == index) return;
                    
                    break;
                }
            }
            // 途中が一致するなら途中の文字を先頭に移動するため、各行をずらします。
            for (int jndex = (findIndex - 1); jndex >= 0; --jndex)
            {
                string propName = string.Format("GPSBabel_Param{0}", jndex);
                string propValue = Properties.Settings.Default[propName] as string;

                propName = string.Format("GPSBabel_Param{0}", jndex + 1);
                Properties.Settings.Default[propName] = propValue;
            }
        }
    }
}
