using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    public class BufferStatus
    {
        public readonly UInt16 TotalSectors;
        public readonly UInt16 FreeSectors;
        public readonly bool DataLogEnable;

        public BufferStatus( UInt16 totalSectors,  UInt16 freeSectors, bool dataLogEnable)
        {
            TotalSectors = totalSectors;
            FreeSectors = freeSectors;
            DataLogEnable = dataLogEnable;
        }
    }
}
