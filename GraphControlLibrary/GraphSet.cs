using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphControlLibrary
{
    public class GraphSet
    {
        public List<PointData> Items;
        public readonly ScaleSet XScale;
        public readonly ScaleSet YScale;
        public readonly bool DrawScale;

        public GraphSet(ScaleSet xScale, ScaleSet yScale, bool drawScale)
        {
            Items = new List<PointData>();

            XScale = xScale;
            YScale = yScale;
            DrawScale = drawScale;
        }
    }
}
