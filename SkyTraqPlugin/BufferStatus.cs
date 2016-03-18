using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyTraqPlugin
{
    internal class BufferStatus
    {
        public readonly UInt16 TotalSectors;
        public readonly UInt16 FreeSectors;
        public readonly bool DataLogEnable;

        public UInt32 Time;
        public UInt32 Distance;
        public UInt32 Speed;

        public BufferStatus(UInt16 totalSectors, UInt16 freeSectors, UInt32 time, UInt32 distance, UInt32 speed, bool dataLogEnable)
        {
            TotalSectors = totalSectors;
            FreeSectors = freeSectors;
            DataLogEnable = dataLogEnable;

            Time = time;
            Distance = distance;
            Speed = speed;
        }
    }
}
