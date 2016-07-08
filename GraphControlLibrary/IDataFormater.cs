using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphControlLibrary
{
    public interface IDataFormater
    {
        string ConvertFrom(Single d);
    }
}
