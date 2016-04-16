using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    internal class DocumentStateNotInitialized : IDocumentState
    {
        private MapControl _parent;

        public DocumentStateNotInitialized(MapControl parent)
        {
            _parent = parent;
        }

        public IDocumentState addPoint(double latitude, double longitude, string title)
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.addPoint(latitude, longitude, title);

            return result;
        }

        public IDocumentState clearPoint()
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.clearPoint();

            return result;
        }

        public IDocumentState drawPolyline()
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.drawPolyline();

            return result;
        }

        public IDocumentState dropMarker()
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.dropMarker();

            return result;
        }

        public IDocumentState initializeScript()
        {
            return this;
        }

        public IDocumentState movePos(double latitude, double longitude)
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.movePos(latitude, longitude);

            return result;
        }

        public IDocumentState resetMarker()
        {
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.resetMarker();

            return result;
        }

        public IDocumentState resizeMap()
        {
            // 読み込み前なら、リサイズは処理しないでよい
            /*
            IDocumentState result = new DocumentStateInitalizing(_parent);

            _parent._Start();

            result.resizeMap();

            return result;
            */

            return this;
        }
    }
}
