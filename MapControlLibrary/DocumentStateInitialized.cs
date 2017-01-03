using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    internal class DocumentStateInitialized : IDocumentState
    {
        private MapControl _parent;

        public DocumentStateInitialized(MapControl parent)
        {
            _parent = parent;
        }

        public IDocumentState addPoint(double latitude, double longitude, string title)
        {
            _parent._addPoint(latitude, longitude, title);
            return this;
        }

        public IDocumentState clearPoint()
        {
            _parent._clearPoint();
            return this;
        }

        public IDocumentState drawPolyline()
        {
            _parent._drawPolyline();
            return this;
        }

        public IDocumentState dropMarker()
        {
            _parent._dropMarker();
            return this;
        }

        public IDocumentState initializeScript()
        {
            return this;
        }

        public IDocumentState movePos(double latitude, double longitude)
        {
            _parent._movePos(latitude, longitude);
            return this;
        }

        public IDocumentState resetMarker()
        {
            _parent._resetMarker();
            return this;
        }

        public IDocumentState resizeMap()
        {
            _parent._resizeMap();
            return this;
        }
    }
}
