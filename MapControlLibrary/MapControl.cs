using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapControlLibrary
{
    public delegate void NotifyMakerDrag(double lat, double lon);

    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class MapControl: WebBrowser
    {
        private IDocumentState _proxy;

        public event NotifyMakerDrag OnMakerDrag;

        public enum MapProvider
        {
            GOOGLE,
            YAHOO
        };

        private MapProvider _provider;
        private string _key;

        public MapControl()
        {
            InitializeComponent();

            _proxy = new DocumentStateNotInitialized(this);

            base.ObjectForScripting = this;
        }

        public void SetProvider(MapProvider provider, string key)
        {
            _provider = provider;
            _key = key;
        }

        internal void _Start()
        {
            switch (_provider)
            {
                case MapProvider.GOOGLE:
                    base.DocumentText = Properties.Resources.googlemapsHTML.Replace( "[[KEY]]", _key);
                    break;
                case MapProvider.YAHOO:
                    base.DocumentText = Properties.Resources.yahoomapsHTML.Replace("[[KEY]]", _key);
                    break;
                default:
                    new Exception("不正なプロバイダです");
                    return;
            }
        }

        public MapProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public void notifyLatLon(object lat, object lon)
        {
            double dLat = (double)lat;
            double dLon = (double)lon;

            OnMakerDrag(dLat, dLon);
        }

        public void resetMarker()
        {
            lock(_proxy)
            {
                _proxy = _proxy.resetMarker();
            }
        }

        internal void _resetMarker()
        {
            this.Document.InvokeScript("resetMarker");
        }

        public void movePos(double latitude, double longitude)
        {
            lock (_proxy)
            {
                _proxy = _proxy.movePos(latitude, longitude);
            }
        }

        internal void _movePos(double latitude, double longitude)
        {
            this.Document.InvokeScript("movePos", new object[] { latitude, longitude });
        }

        public void resizeMap()
        {
            lock (_proxy)
            {
                _proxy = _proxy.resizeMap();
            }
        }

        internal void _resizeMap()
        {
            this.Document.InvokeScript("resizeMap");
        }

        internal void _Initialize()
        {
            this.Document.InvokeScript("Initialize");
        }

        public void dropMarker()
        {
            lock (_proxy)
            {
                _proxy = _proxy.dropMarker();
            }
        }

        internal void _dropMarker()
        {
            this.Document.InvokeScript("dropMarker");
        }

        public void clearPoint()
        {
            lock (_proxy)
            {
                _proxy = _proxy.clearPoint();
            }
        }

        internal void _clearPoint()
        {
            this.Document.InvokeScript("clearPoint");
        }

        public void addPoint(double latitude, double longitude, string title)
        {
            lock (_proxy)
            {
                _proxy = _proxy.addPoint(latitude, longitude, title);
            }
        }

        internal void _addPoint(double latitude, double longitude, string title)
        {
            string markerText = string.Empty;
            if (0 < title.Length)
            {
                switch (_provider)
                {
                    case MapProvider.GOOGLE:
                        markerText = string.Format("{0}\r\n{1:0.######}, {2:0.######}", title, latitude, longitude);
                        break;
                    case MapProvider.YAHOO:
                        markerText = string.Format("{0}<br>{1:0.######}, {2:0.######}", title, latitude, longitude);
                        break;
                    default:
                        new Exception("不正なプロバイダです");
                        return;
                }
            }
            this.Document.InvokeScript("addPoint", new object[] { latitude, longitude, markerText });
        }

        public void drawPolyline()
        {
            lock (_proxy)
            {
                _proxy = _proxy.drawPolyline();
            }
        }

        internal void _drawPolyline()
        {
            this.Document.InvokeScript("drawPolyline");
        }

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            lock (_proxy)
            {
                _proxy = _proxy.Initialize();
            }

            base.OnDocumentCompleted(e);
        }

        protected override void OnResize(EventArgs e)
        {
            lock (_proxy)
            {
                _proxy = _proxy.resizeMap();
            }
            base.OnResize(e);
        }
    }
}
