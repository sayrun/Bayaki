﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkyTraqPlugin
{
    internal partial class EraseDataForm : Form
    {
        public EraseDataForm()
        {
            InitializeComponent();
        }

        private void SelectPortForm_Load(object sender, EventArgs e)
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

        private void _posrts_SelectedIndexChanged(object sender, EventArgs e)
        {
            _erase.Enabled = (0 <= _posrts.SelectedIndex);
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
                if( 0 <= index)
                {
                    _posrts.SelectedIndex = index;
                }
            }
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool NowProcessing
        {
            set
            {
                _erase.Enabled = value;
                _cancel.Enabled = value;
                _posrts.Enabled = value;
            }
        }

        private void _erase_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != MessageBox.Show(Properties.Resources.MSG5, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                // 実行を中止しますよ
                return;
            }

            UseWaitCursor = true;
            NowProcessing = false;

            _progress.Value = 0;

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
            _eraseWorker.RunWorkerAsync(portName);

            _timer.Start();
        }

        private void _eraseWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string portName = e.Argument as string;

            try
            {
                using (SkytraqController skytraq = new SkytraqController(portName))
                {
                    MyEnviroment.LatestPortName = portName;

                    bool result = skytraq.EraceLatLonData();

                    e.Result = result;
                }
            }
            catch
            {
                e.Result = false;
            }
        }

        private void _eraseWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool result = (bool)e.Result;

            NowProcessing = true;
            UseWaitCursor = false;

            _timer.Stop();
            _progress.Value = _progress.Maximum;

            if ( result)
            {
                MessageBox.Show(Properties.Resources.MSG6, Properties.Resources.Eraser_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(Properties.Resources.MSG7, Properties.Resources.Eraser_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            _progress.Value = 0;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_progress.Maximum > _progress.Value)
            {
                _progress.Value++;
            }
            else
            {
                _timer.Stop();
            }
        }
    }
}
