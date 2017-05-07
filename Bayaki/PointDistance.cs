using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bayaki
{
    class PointDistance
    {
        public static double Distance( IEnumerable<bykIFv1.Point> points)
        {
            double result = 0;
            bykIFv1.Point from = null;
            foreach ( var to in points)
            {
                if( null != from)
                {
                    result += Distance(from, to);
                }
                from = to;
            }
            return result;
        }

        /// <summary>
        /// 二点間の距離を求める(ヒュベニの公式)
        /// ※
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns>m</returns>
        public static double Distance(bykIFv1.Point pt1, bykIFv1.Point pt2)
        {
            const double PI = 3.1415926535898;
            // a = 6,378,137
            const double a = 6378137;
            // f = 1 / 298.257 223 563
            const double f = 1 / 298.257223563;
            // b = a - ( a * f)
            const double b = a - (a * f);

            double x1 = (pt1.Longitude * PI) / 180;
            double x2 = (pt2.Longitude * PI) / 180;
            double y1 = (pt1.Latitude * PI) / 180;
            double y2 = (pt2.Latitude * PI) / 180;

            // e = √((a^2 - b^2) / a^2)
            //double e = Math.Sqrt((Math.Pow(a, 2) + Math.Pow(b, 2)) / Math.Pow(a, 2));
            const double e = 1.4118447577583941;
            // μy = (y1 + y2) / 2
            double uy = (y1 + y2) / 2;
            // W = √(1-(e^2 * sin(μy)^2))
            double W = Math.Sqrt(1 - (Math.Pow(e, 2) * Math.Pow(Math.Sin((uy * PI) / 180), 2)));
            // N = a / W
            double N = a / W;
            // M = a * (1 - e^2) / W^3
            double M = (a * (1 - Math.Pow(e, 2))) / Math.Pow(W, 3);
            // dy = y1 - y2
            double dy = y1 - y2;
            // dx = x1 - x2
            double dx = x1 - x2;

            // d = √((dy*M)^2 + (dx*N*cos μy)^2)
            double d = Math.Sqrt(Math.Pow(dy * M, 2) + Math.Pow(dx * N * Math.Cos(uy), 2));

            return d;

        }

    }
}
