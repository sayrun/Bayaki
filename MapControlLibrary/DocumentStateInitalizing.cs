using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapControlLibrary
{
    class DocumentStateInitalizing : IDocumentState
    {
        #region 内部クラス
        private interface IInternalRequest
        {
            void DoAction(MapControl parent);
        }

        private class resetMarkerRequest : IInternalRequest
        {
            public void DoAction(MapControl parent)
            {
                parent._resetMarker();
            }
        }

        private class movePosRequest : IInternalRequest
        {
            private double _latitude;
            private double _longitude;
            public movePosRequest(double latitude, double longitude)
            {
                _latitude = latitude;
                _longitude = longitude;
            }
            public void DoAction(MapControl parent)
            {
                parent._movePos(_latitude, _longitude);
            }
        }

        private class resizeMapRequest : IInternalRequest
        {
            public void DoAction(MapControl parent)
            {
                parent._resetMarker();
            }
        }

        private class dropMarkerRequest : IInternalRequest
        {
            public void DoAction(MapControl parent)
            {
                parent._dropMarker();
            }
        }

        private class clearPointRequest : IInternalRequest
        {
            public void DoAction(MapControl parent)
            {
                parent._clearPoint();
            }
        }

        private class addPointRequest : IInternalRequest
        {
            double _latitude;
            double _longitude;
            string _title;
            public addPointRequest(double latitude, double longitude, string title)
            {
                _latitude = latitude;
                _longitude = longitude;
                _title = title;
            }
            public void DoAction(MapControl parent)
            {
                parent._addPoint(_latitude, _longitude, _title);
            }
        }

        private class drawPolylineRequest : IInternalRequest
        {
            public void DoAction(MapControl parent)
            {
                parent._drawPolyline();
            }
        }
        #endregion

        private MapControl _parent;
        private Queue<IInternalRequest> _request;

        public DocumentStateInitalizing(MapControl parent)
        {
            _parent = parent;
            _request = new Queue<IInternalRequest>();
        }

        public IDocumentState addPoint(double latitude, double longitude, string title)
        {
            _request.Enqueue(new addPointRequest(latitude, longitude, title));

            return this;
        }

        public IDocumentState clearPoint()
        {
            _request.Enqueue(new clearPointRequest());

            return this;
        }

        public IDocumentState drawPolyline()
        {
            _request.Enqueue(new drawPolylineRequest());

            return this;
        }

        public IDocumentState dropMarker()
        {
            _request.Enqueue(new dropMarkerRequest());

            return this;
        }

        public IDocumentState Initialize()
        {
            _parent._Initialize();

            IInternalRequest req;

            while (0 < _request.Count)
            {
                req = _request.Dequeue();
                req.DoAction(_parent);
            }

            return new DocumentStateInitialized(_parent);
        }

        public IDocumentState movePos(double latitude, double longitude)
        {
            _request.Enqueue(new movePosRequest(latitude, longitude));

            return this;
        }

        public IDocumentState resetMarker()
        {
            _request.Enqueue(new resetMarkerRequest());

            return this;
        }

        public IDocumentState resizeMap()
        {
            _request.Enqueue(new resizeMapRequest());

            return this;
        }
    }

}
