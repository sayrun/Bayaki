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
    internal partial class SetEphemerisForm : Form
    {
        private SkytraqController _port;

        public SetEphemerisForm()
        {
            InitializeComponent();
        }

        private void SetEphemerisForm_Load(object sender, EventArgs e)
        {
            try
            {
                _posrts.BeginUpdate();
                _posrts.Items.Add(Properties.Resources.PORT_AUTOSEL);
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
                    int index = _posrts.Items.IndexOf(Properties.Resources.PORT_AUTOSEL);
                    _posrts.SelectedIndex = index;
                }
            }
            finally
            {
                _posrts.EndUpdate();
            }
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

        private void _start_Click(object sender, EventArgs e)
        {
            string portName = this.PortName;
            if (Properties.Resources.PORT_AUTOSEL == portName)
            {
                try
                {
                    portName = SkytraqController.AutoSelectPort();
                    this.PortName = portName;
                }
                catch
                {
                    _posrts.Items.Remove(portName);
                    MessageBox.Show(Properties.Resources.MSG1, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            try
            {
                _port = new SkytraqController(portName);
                MyEnviroment.LatestPortName = portName;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            _port.OnSetEphemeris += _port_OnSetEphemeris;

            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Credentials = new System.Net.NetworkCredential(_username.Text, _password.Text);
            string URL = string.Format("ftp://{0}/ephemeris/Eph.dat", _host.Text);
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadDataCompleted += Wc_DownloadDataCompleted;
            wc.DownloadDataAsync(new Uri(URL));

            return;
        }

        private void _port_OnSetEphemeris(SetEphemerisProgressEvent progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetEphemerisProgressHandler(_port_OnSetEphemeris), new object[] { progress });
                return;
            }
            _phase.Text = (progress.Max == progress.Value) ? Properties.Resources.MSG10 : Properties.Resources.MSG11;
            _progress.Maximum = progress.Max;
            _progress.Value = progress.Value;
        }

        private void Wc_DownloadDataCompleted(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            if( null != e.Error)
            {
                MessageBox.Show(e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _phase.Text = Properties.Resources.MSG12;
                return;
            }

            byte[] buffer = e.Result;

            _phase.Text = Properties.Resources.MSG13;

            _setEphemerisWorker.RunWorkerAsync(buffer);

            return;
        }

        private void Wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            _phase.Text = Properties.Resources.MSG14;
            if (0 < e.TotalBytesToReceive)
            {
                _progress.Maximum = (int)e.TotalBytesToReceive;
                _progress.Value = (int)e.BytesReceived;
            }
        }

        private void _setEphemerisWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] ephemeris = e.Argument as byte[];
            if( null == ephemeris)
            {
                e.Result = false;
                return;
            }

            _port.SetEphemeris(ephemeris);

            e.Result = true;
        }

        private void _setEphemerisWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                MessageBox.Show(e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if( (bool)e.Result)
            {
                MessageBox.Show(Properties.Resources.MSG8, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(Properties.Resources.MSG9, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void SetEphemerisForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != _port)
            {
                _port.Dispose();
                _port = null;
            }
        }
    }
}
