using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    internal delegate void SetEphemerisProgressHandler(SetEphemerisProgressEvent progress);

    internal class SetEphemerisProgressEvent
    {
        public readonly int Value;
        public readonly int Max;

        public SetEphemerisProgressEvent( int value, int max)
        {
            Value = value;
            Max = max;
        }

    }
}
