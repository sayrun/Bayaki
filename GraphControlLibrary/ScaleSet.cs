using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphControlLibrary
{
    public class ScaleSet
    {
        public readonly String Title;
        public readonly String Unit;
        public readonly ScaleData Max;
        public readonly ScaleData Min;

        public readonly List<ScaleData> Items;

        private IDataFormater _dataFormater;

        public ScaleSet( string title, string unit, ScaleData min, ScaleData max, IDataFormater dataFormater)
        {
            Title = title;
            Unit = unit;

            Min = min;
            Max = max;

            _dataFormater = dataFormater;

            Items = new List<ScaleData>();
        }

        public string ConvertFrom(Single d)
        {
            return _dataFormater.ConvertFrom(d);
        }
    }
}
