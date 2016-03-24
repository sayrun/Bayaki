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
    internal partial class ConfiurationForm : Form
    {
        private SkytraqController _port;
        private BufferStatus _bs;

        public ConfiurationForm()
        {
            InitializeComponent();

            _threshold.Maximum = UInt32.MaxValue;
            _speed.Maximum = UInt32.MaxValue;
            _thresholdType.SelectedIndex = -1;
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
            try
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
                _port = new SkytraqController(portName);
                MyEnviroment.LatestPortName = portName;

                // Version情報
                SoftwareVersion sv = _port.SoftwareVersion;
                _kernelVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.KernelVersion >> 16) & 0x00ff), ((sv.KernelVersion >> 8) & 0x00ff), (sv.KernelVersion & 0x00ff));
                _ODMVersion.Text = string.Format("{0:D2}.{1:D2}.{2:D2}", ((sv.ODMVersion >> 16) & 0x00ff), ((sv.ODMVersion >> 8) & 0x00ff), (sv.ODMVersion & 0x00ff));
                _Revision.Text = string.Format("{0:D2}/{1:D2}/{2:D2}", ((sv.Revision >> 16) & 0x00ff), ((sv.Revision >> 8) & 0x00ff), (sv.Revision & 0x00ff));

                // バッファ情報
                _bs = _port.GetBufferStatus();
                if(0 != _bs.Time)
                {
                    _thresholdType.SelectedIndex = 0;
                }
                else
                {
                    if( 0 != _bs.Distance)
                    {
                        _thresholdType.SelectedIndex = 1;
                    }
                    else
                    {
                        _bs.Time = 5;
                        _thresholdType.SelectedIndex = 0;
                    }
                }
                _speed.Value = _bs.Speed;

                _thresholdType.Enabled = true;
                _threshold.Enabled = true;
                _speed.Enabled = true;
                _update.Enabled = true;
            }
            catch(Exception ex)
            {
                _thresholdType.Enabled = false;
                _threshold.Enabled = false;
                _speed.Enabled = false;
                _update.Enabled = false;

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ConfiurationForm_Load(object sender, EventArgs e)
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

        private void ConfiurationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( null != _port)
            {
                _port.Dispose();
                _port = null;
            }
        }

        private void _update_Click(object sender, EventArgs e)
        {
            try
            {
                UInt32 time = 0;
                UInt32 distance = 0;
                UInt32 speed = (UInt32)_speed.Value;

                if (1 == _thresholdType.SelectedIndex)
                {
                    distance = (UInt32)_threshold.Value;
                }
                else
                {
                    time = (UInt32)_threshold.Value;
                }

                BufferStatus bs = new BufferStatus(0, 0, time, distance, speed, true);

                _port.SetLogConfigure(bs);

                MessageBox.Show(Properties.Resources.MSG2, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                _threshold.Enabled = false;
                _speed.Enabled = false;
                _update.Enabled = false;

                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void _thresholdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (null == cb) return;

            if( 1 == cb.SelectedIndex)
            {
                _thresholdLabel.Text = Properties.Resources.MSG15;
                _threshold.Value = _bs.Distance;
            }
            else
            {
                _thresholdLabel.Text = Properties.Resources.MSG16;
                _threshold.Value = _bs.Time;
            }
        }
    }
}
