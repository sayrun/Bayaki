using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    class SoftwareVersion
    {
        public readonly byte SoftType;
        public readonly UInt32 KernelVersion;
        public readonly UInt32 ODMVersion;
        public readonly UInt32 Revision;

        private const int OFFSET_SoftType = 0;
        private const int OFFSET_KernelVersion = 1;
        private const int OFFSET_ODMVersion = 5;
        private const int OFFSET_Revision = 9;

        public SoftwareVersion(byte[] versionSource)
        {
            SoftType = versionSource[OFFSET_SoftType];

            KernelVersion = (UInt32)versionSource[OFFSET_KernelVersion];
            for ( int index = 1; index < sizeof(UInt32);++index)
            {
                KernelVersion <<= 8;
                KernelVersion |= (UInt32)versionSource[OFFSET_KernelVersion + index];
            }

            ODMVersion = (UInt32)versionSource[OFFSET_ODMVersion];
            for (int index = 1; index < sizeof(UInt32); ++index)
            {
                ODMVersion <<= 8;
                ODMVersion |= (UInt32)versionSource[OFFSET_ODMVersion + index];
            }

            Revision = (UInt32)versionSource[OFFSET_Revision];
            for (int index = 1; index < sizeof(UInt32); ++index)
            {
                Revision <<= 8;
                Revision |= (UInt32)versionSource[OFFSET_Revision + index];
            }
        }
    }
}
