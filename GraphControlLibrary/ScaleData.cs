using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphControlLibrary
{
    public class ScaleData
    {
        public readonly string Text;
        public readonly Single Value;

        public ScaleData(string text, Single value)
        {
            Text = text;
            Value = value;
        }
    }
}
