using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    internal interface IDocumentState
    {
        IDocumentState initializeScript();
        IDocumentState resetMarker();
        IDocumentState movePos(double latitude, double longitude);
        IDocumentState resizeMap();
        IDocumentState dropMarker();
        IDocumentState clearPoint();
        IDocumentState addPoint(double latitude, double longitude, string title);
        IDocumentState drawPolyline();
    }
}
