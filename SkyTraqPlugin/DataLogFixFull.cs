using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    internal class DataLogFixFull
    {
        public TYPE type;

        public enum TYPE
        {
            FULL_POI,   // point of interest
            FULL,
            COMPACT
        };

        public UInt16 V;   // user velocity
        public UInt16 WN;  // GPS Week Number
        public UInt32 TOW; // GPS Time of Week
        public Int32 X;   // X position in ECEF Coordinate
        public Int32 Y;   // X position in ECEF Coordinate
        public Int32 Z;   // X position in ECEF Coordinate

    }
}
