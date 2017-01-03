using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    class JavascriptInvokeException : Exception
    {
        public readonly string FunctionName;

        public  JavascriptInvokeException(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
