using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkyTraqPlugin
{
    internal partial class DownloadDataForm : Form
    {
        private const string PORT_AUTO = "Auto";
        private SkytraqController _port;

        private bykIFv1.TrackItem _item;


        public DownloadDataForm()
        {
            InitializeComponent();

            _item = null;
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

        public bykIFv1.TrackItem[] Items
        {
            get
            {
                return new bykIFv1.TrackItem [] { _item } ;
            }
        }

        private void _connect_Click(object sender, EventArgs e)
        {
            try
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
                MyEnviroment.LatestPortName = portName;
                _port.OnRead += _port_OnRead;

                // Version情報
                SoftwareVersion sv = _port.SoftwareVersion;
                _kernelVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.KernelVersion >> 16) & 0x00ff), ((sv.KernelVersion >> 8) & 0x00ff), (sv.KernelVersion & 0x00ff));
                _ODMVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.ODMVersion >> 16) & 0x00ff), ((sv.ODMVersion >> 8) & 0x00ff), (sv.ODMVersion & 0x00ff));
                _Revision.Text = string.Format("{0:D2}/{1:D2}/{2:D2}", ((sv.Revision >> 16) & 0x00ff), ((sv.Revision >> 8) & 0x00ff), (sv.Revision & 0x00ff));

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

                if (bs.TotalSectors == bs.FreeSectors)
                {
                    _download.Enabled = false;
                    MessageBox.Show("ダウンロードするべきデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Skytraqの接続に失敗しました\n\n" + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void _port_OnRead(ReadProgressEvent progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ReadProgressEventHandler(_port_OnRead), new object[] { progress });
                return;
            }

            System.Diagnostics.Debug.Print("{0} - {1}/{2}", progress.Phase, progress.Value, progress.Max);

            _phase.Text = progress.PhaseName;
            _progress.Maximum = progress.Max;
            _progress.Value = progress.Value;
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
                if (MyEnviroment.IsValidPortName)
                {
                    int index = _posrts.Items.IndexOf(MyEnviroment.LatestPortName);
                    _posrts.SelectedIndex = index;
                }
                else
                {
                    int index = _posrts.Items.IndexOf(PORT_AUTO);
                    _posrts.SelectedIndex = index;
                }
            }
            finally
            {
                _posrts.EndUpdate();
            }

        }

        /*
        private void ReadLatLogDataProgress(ReadProgressEvent progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new LocalReadLatLogDataProgress(ReadLatLogDataProgress), new object[] { progress });
                return;
            }

            System.Diagnostics.Debug.Print("{0} - {1}/{2}", progress.Phase, progress.Value, progress.Max);

            _phase.Text = progress.PhaseName;
            _progress.Maximum = progress.Max;
            _progress.Value = progress.Value;
        }*/

        private void _download_Click(object sender, EventArgs e)
        {
            _download.Enabled = false;
            _cancel.Enabled = false;

            _downloadWorker.RunWorkerAsync(_port);

            return;
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void _downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<bykIFv1.Point> points = _port.ReadLatLonData();

            bykIFv1.TrackItem item = new bykIFv1.TrackItem("Skytraq Download Data", DateTime.Now);
            item.Items = points;

            e.Result = item;
        }

        private void _downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null == e.Error)
            {
                _item = e.Result as bykIFv1.TrackItem;

                DialogResult = DialogResult.OK;
            }
            else
            {
                _cancel.Enabled = true;
                _download.Enabled = true;
                MessageBox.Show(e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void DownloadDataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( null != _port)
            {
                _port.Dispose();
                _port = null;
            }
        }
    }
}
