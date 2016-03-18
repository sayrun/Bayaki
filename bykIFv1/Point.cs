using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace bykIFv1
{
    [Serializable]
    public class Point : IComparable<Point>, IComparable<DateTime>
    {
        public readonly DateTime Time;
        public readonly decimal Latitude;
        public readonly decimal Longitude;
        public readonly decimal Elevation;
        public readonly decimal Speed;
        public readonly bool Interest;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="time">時間は世界標準時</param>
        /// <param name="latitude">緯度</param>
        /// <param name="longitude">経度</param>
        /// <param name="elevation">高度</param>
        /// <param name="speed">速度</param>
        /// <param name="interest">要点</param>
        public Point(DateTime time, decimal latitude, decimal longitude, decimal elevation, decimal speed, bool interest)
        {
            this.Time = time;
            this.Latitude = latitude;
            this.Longitude = longitude;

            this.Elevation = elevation;
            this.Speed = speed;

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
    }
}
