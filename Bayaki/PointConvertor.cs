using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayaki
{
    class PointConvertor
    {
        private bykIFv1.Point _point;

        public PointConvertor( bykIFv1.Point point)
        {
            _point = point;
        }

        private UInt32[] ToEleArray(decimal value)
        {
            decimal lonA = Math.Floor(Math.Abs(value) * 1000);

            lonA *= Math.Sign(value);

            UInt32[] result = new UInt32[] { (UInt32)lonA, 1000 };

            return result;
        }

        private UInt32[] ToLonLatArray(decimal value)
        {
            value = Math.Abs(value);

            decimal lonA = Math.Floor(value);

            decimal lonB = Math.Floor((value - lonA) * 60);

            decimal lonC = Math.Floor(((value - lonA) - (lonB / 60)) * 60 * 60 * 1000);

            UInt32[] result = new UInt32[] { (UInt32)lonA, 1, (UInt32)lonB, 1, (UInt32)lonC, 1000 };

            return result;
        }

        private byte[] ConvertTo(UInt32[] param1)
        {
            byte[] result = new byte[sizeof(UInt32) * param1.Length];
            System.Buffer.BlockCopy(param1, 0, result, 0, result.Length);

            return result;
        }

        private byte[] ConvertTo(string param1)
        {
            char[] value = param1.ToCharArray();
            byte[] result = new byte[value.Length + 1];
            System.Buffer.BlockCopy(value, 0, result, 0, value.Length);
            result[value.Length] = 0x00;

            return result;
        }

        public byte[] LatitudeMark
        {
            get
            {
                return ConvertTo((_point.Latitude >= decimal.Zero) ? "S" : "N");
            }
        }

        public byte[] Latitude
        {
            get
            {
                decimal latitude = Math.Abs(_point.Latitude);

                return ConvertTo(ToLonLatArray(latitude));
            }
        }

        public byte[] LongitudeMark
        {
            get
            {
                return ConvertTo((_point.Longitude >= decimal.Zero) ? "E" : "W");
            }
        }

        public byte[] Longtude
        {
            get
            {
                decimal longitude = Math.Abs(_point.Longitude);

                return ConvertTo(ToLonLatArray(longitude));
            }
        }

        public byte[] Altitude
        {
            get
            {
                decimal elevation = Math.Abs(_point.Elevation);

                return ConvertTo(ToEleArray(elevation));
            }
        }

        public byte[] AltitudeRef
        {
            get
            {
                return new byte[] { 0 };
            }
        }
    }
}
