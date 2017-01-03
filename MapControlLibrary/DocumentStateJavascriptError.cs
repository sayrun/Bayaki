using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    class DocumentStateJavascriptError : IDocumentState
    {
        private MapControl _parent;

        public DocumentStateJavascriptError(MapControl parent)
        {
            _parent = parent;

            // Javascriptエラーが発生したので、エラー用のHTMLを表示します。
            _parent._SetJavascriptErrorHTML();
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
