using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Device.Location;

namespace bykIFv1
{
    public class Point : IComparable<Point>, IComparable<DateTime>
    {
        public readonly DateTime Time;
        public readonly bool Interest;

        public readonly GeoCoordinate Location;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="time">時間は世界標準時</param>
        /// <param name="latitude">緯度</param>
        /// <param name="longitude">経度</param>
        /// <param name="altitude">高度</param>
        /// <param name="speed">速度</param>
        /// <param name="interest">要点</param>
        public Point(DateTime time, double latitude, double longitude, double altitude, double speed, bool interest)
        {
            this.Time = time;
            this.Location = new GeoCoordinate(latitude, longitude, altitude, 0, 0, speed, 0);

            this.Interest = interest;
        }

        public int CompareTo(Point other)
        {
            return Time.CompareTo(other.Time);
        }

        public int CompareTo(DateTime other)
        {
            return Time.CompareTo(other);
        }

        public double Latitude
        {
            get
            {
                return Location.Latitude;
            }
        }

        public double Longitude
        {
            get
            {
                return Location.Longitude;
            }
        }

        public double Altitude
        {
            get
            {
                return Location.Altitude;
            }
        }
        
        public double Speed
        {
            get
            {
                return Location.Speed;
            }
        }
    }
}
