using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Bayaki
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, "Bayaki.exe");
            if (mutex.WaitOne(0, false) == false)
            {
                MessageBox.Show(Properties.Resources.MSG6);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            mutex.ReleaseMutex();
        }
    }
}
