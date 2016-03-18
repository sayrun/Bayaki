using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    internal partial class NowProcessingForm<Type> : Form
    {
        readonly Type[] _files;
        readonly ProcessingStrategy _strategy;

        internal interface ProcessingStrategy
        {
            void DoProcess(Type filePath);
        }

        public NowProcessingForm(ProcessingStrategy strategy, Type[] files)
        {
            InitializeComponent();

            _strategy = strategy;
            _files = files;

            _progressBar.Value = 0;
            _progressBar.Maximum = 100;
        }

        private void NowProcessingForm_Load(object sender, EventArgs e)
        {
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            foreach( Type filename in _files)
            {
                ++index;
                int progress = (index * 100) / _files.Length;
                _backgroundWorker.ReportProgress(progress);

                try
                {

                    _strategy.DoProcess(filename);
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("対象ファイルの処理に失敗しました。\n\n{0}", filename), ex);
                }
            }
            _backgroundWorker.ReportProgress(100);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if( null != e.Error)
            {
                MessageBox.Show(e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressBar.Value = e.ProgressPercentage;
        }
    }
}
