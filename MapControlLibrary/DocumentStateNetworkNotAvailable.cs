using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    class DocumentStateNetworkNotAvailable : IDocumentState
    {
        private MapControl _parent;

        public DocumentStateNetworkNotAvailable(MapControl parent)
        {
            _parent = parent;

            // ネットワークが利用できないので、エラー用のHTMLを表示します。
            _parent._SetNetUnavailableHTML();
        }

        public IDocumentState addPoint(double latitude, double longitude, string title)
        {
            return this;
        }

        public IDocumentState clearPoint()
        {
            return this;
        }

        public IDocumentState drawPolyline()
        {
            return this;
        }

        public IDocumentState dropMarker()
        {
            return this;
        }

        public IDocumentState initializeScript()
        {
            return this;
        }

        public IDocumentState movePos(double latitude, double longitude)
        {
            return this;
        }

        public IDocumentState resetMarker()
        {
            return this;
        }

        public IDocumentState resizeMap()
        {
            return this;
        }
    }
}
