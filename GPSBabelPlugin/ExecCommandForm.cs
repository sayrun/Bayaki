using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

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

                this.Cursor = Cursors.WaitCursor;

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = _execPath.Text;
                //出力を読み取れるようにする
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = false;
                p.StartInfo.RedirectStandardError = false;
                //ウィンドウを表示しないようにする
                p.StartInfo.CreateNoWindow = true;

                // 結果をGPXとしてstdoutに出力させる（-oは出力形式 -Fは出力先[-]はstdout指定
                p.StartInfo.Arguments = string.Format("{0} -o gpx -F -", _parameters.Text);

                // stdoutを受け取る
                StringBuilder sb = new StringBuilder();
                p.OutputDataReceived += delegate (object s, System.Diagnostics.DataReceivedEventArgs dre) { sb.AppendLine(dre.Data); };

                // GPSBabelを起動する
                p.Start();

                //出力を読み取る
                p.BeginOutputReadLine();

                //プロセス終了まで待機する
                p.WaitForExit();
                p.Close();

                // stdoutの結果をgpxに読み込む
                using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(sb.ToString())))
                {
                    using (TextReader tr = new StreamReader(ms))
                    {
                        using (XmlReader xr = new XmlTextReader(tr))
                        {
                            using (Bayaki.TrackItemReader tir = new Bayaki.TrackItemReader(xr))
                            {
                                bykIFv1.TrackItem item = tir.Read();
                                _item = (0 < item.Items.Count) ? item : null;
                            }
                        }
                    }
                }

                // 成功していたら設定を反映させます。
                RollParamList(_parameters.Text);
                Properties.Settings.Default.GPSBabel_PATH = _execPath.Text;
                Properties.Settings.Default.GPSBabel_Param0 = _parameters.Text;
                Properties.Settings.Default.Save();

                // 正常終了
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // 異常終了とする
                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                _exec.Enabled = true;
                _cancel.Enabled = true;
                _selExecFile.Enabled = true;

                this.Cursor = Cursors.Default;
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
