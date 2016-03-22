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

        public bykIFv1.TrackItem[] GetItems()
        {
            if (null != _item)
            {
                return new bykIFv1.TrackItem[] { _item };
            }
            else
            {
                return new bykIFv1.TrackItem[] {  };
            }
        }

        private void _exec_Click(object sender, EventArgs e)
        {
            try
            {
                _exec.Enabled = false;
                _cancel.Enabled = false;
                _selExecFile.Enabled = false;

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
                p.StartInfo.CreateNoWindow = false;

                p.StartInfo.Arguments = string.Format("{0} -o gpx -F {1}", _parameters.Text, tempFileName);

                p.Start();
                //出力を読み取る
                string results = p.StandardOutput.ReadToEnd();  // なぜか読めない
                System.Diagnostics.Debug.Print(results);

                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                p.WaitForExit();
                p.Close();

                try
                {
                    using (Bayaki.TrackItemReader tir = new Bayaki.TrackItemReader(tempFileName))
                    {
                        bykIFv1.TrackItem item = tir.Read();
                        _item = (0 < item.Items.Count) ? item : null;
                    }
                }
                finally
                {
                    System.IO.File.Delete(tempFileName);
                }
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
    }
}
