using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkyTraqPlugin
{
    public partial class DownloadDataForm : Form
    {
        private const string PORT_AUTO = "Auto";
        private SkytraqController _port;


        public DownloadDataForm()
        {
            InitializeComponent();
        }

        public string PortName
        {
            get
            {
                return _posrts.SelectedItem.ToString();
            }
            set
            {
                int index = _posrts.Items.IndexOf(value);
                if (0 <= index)
                {
                    _posrts.SelectedIndex = index;
                }
            }
        }


        private void _connect_Click(object sender, EventArgs e)
        {
            string portName = this.PortName;
            if (PORT_AUTO == portName)
            {
                try
                {
                    portName = SkytraqController.AutoSelectPort();
                    this.PortName = portName;
                }
                catch
                {
                    _posrts.Items.Remove(portName);
                    MessageBox.Show("自動でポートを選択できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            _port = new SkytraqController(portName);

            // Version情報
            SoftwareVersion sv = _port.SoftwareVersion;
            _kernelVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.KernelVersion >> 16) & 0x00ff), ((sv.KernelVersion >> 8) & 0x00ff), (sv.KernelVersion & 0x00ff));
            _ODMVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.ODMVersion >> 16) & 0x00ff), ((sv.ODMVersion >> 8) & 0x00ff), (sv.ODMVersion & 0x00ff));
            _Revision.Text = string.Format("{0:D2}/{1:D2}/{2:D2}", ( ( sv.Revision >> 16) & 0x00ff), ((sv.Revision >> 8) & 0x00ff), (sv.Revision & 0x00ff));

            // バッファーサイズ
            BufferStatus bs = _port.GetBufferStatus();
            UInt16 usedSectors = bs.TotalSectors;
            usedSectors -= bs.FreeSectors;
            _bufferStatus.Maximum = bs.TotalSectors;
            _bufferStatus.Value = usedSectors;

            // ボタンを制御
            _connect.Enabled = false;
            _download.Enabled = true;
            _posrts.Enabled = false;

            if(bs.TotalSectors == bs.FreeSectors)
            {
                _download.Enabled = false;
                MessageBox.Show("ダウンロードするべきデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DownloadDataForm_Load(object sender, EventArgs e)
        {
            try
            {
                _posrts.BeginUpdate();
                _posrts.Items.Add(PORT_AUTO);
                foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
                {
                    _posrts.Items.Add(portName);
                }
                int index = _posrts.Items.IndexOf(PORT_AUTO);
                _posrts.SelectedIndex = index;
            }
            finally
            {
                _posrts.EndUpdate();
            }

        }

        delegate void  LocalReadLatLogDataProgress(int phaseValue, int phaseMax, int progressValue, int progressMax);

        private void MyReadLatLogDataProgress(int phaseValue, int phaseMax, int progressValue, int progressMax)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LocalReadLatLogDataProgress(MyReadLatLogDataProgress), new object[] { phaseValue, phaseMax, progressValue, progressMax });
                return;
            }
            _progress.Maximum = phaseMax;
            _progress.Value = phaseValue;
            progressBar2.Maximum = progressMax;
            progressBar2.Value = progressValue;
        }

        private void ReadLatLogDataProgress(int phaseValue, int phaseMax, int progressValue, int progressMax)
        {
            MyReadLatLogDataProgress(phaseValue, phaseMax, progressValue, progressMax);

            System.Diagnostics.Debug.Print("{0}/{1}, {2}/{3}", phaseValue, phaseMax, progressValue, progressMax);
        }

        private void _download_Click(object sender, EventArgs e)
        {
            _downloadWorker.RunWorkerAsync(_port);

            return;
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            if( null != _port)
            {
                _port.Dispose();
                _port = null;
            }
            DialogResult = DialogResult.Cancel;
        }

        private void _downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<bykIFv1.Point> points = _port.ReadLatLonData(new SkytraqController.ReadLatLogDataProgress(this.ReadLatLogDataProgress));

            e.Result = points;
        }
    }
}
