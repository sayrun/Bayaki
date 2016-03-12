using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    internal class MyEnviroment
    {
        private static string _latestPort = string.Empty;

        private MyEnviroment()
        { }

        static MyEnviroment()
        {

        }

        public static bool IsValidPortName
        {
            get
            {
                return (0 < _latestPort.Length);
            }
        }

        public static string LatestPortName
        {
            set
            {
                _latestPort = value;
            }
            get
            {
                return _latestPort;
            }
        }
    }
}
