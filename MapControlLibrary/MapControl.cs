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
        public event NotifyMakerDrag OnMakerDrag;

        public enum MapProvider
        {
            GOOGLE,
            YAHOO
        };

        private MapProvider _provider;
        private bool _documentComplated;

        public MapControl()
        {
            InitializeComponent();

            base.ObjectForScripting = this;
            _documentComplated = false;
        }

        public void Show( MapProvider provider, string key)
        {
            _documentComplated = false;
            _provider = provider;
            switch (_provider)
            {
                case MapProvider.GOOGLE:
                    base.DocumentText = Properties.Resources.googlemapsHTML.Replace( "[[KEY]]", key);
                    break;
                case MapProvider.YAHOO:
                    base.DocumentText = Properties.Resources.yahoomapsHTML.Replace("[[KEY]]", key);
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
            this.Document.InvokeScript("resetMarker");
        }

        public void movePos(double latitude, double longitude)
        {
            this.Document.InvokeScript("movePos", new object[] { latitude, longitude });
        }

        public void resizeMap()
        {
            this.Document.InvokeScript("resizeMap");
        }

        public void Initialize()
        {
            this.Document.InvokeScript("Initialize");
            _documentComplated = true;
        }

        public void dropMarker()
        {
            this.Document.InvokeScript("dropMarker");
        }

        public void clearPoint()
        {
            this.Document.InvokeScript("clearPoint");
        }

        public void addPoint(double latitude, double longitude, string title)
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
            this.Document.InvokeScript("drawPolyline");
        }

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            this.Initialize();

            base.OnDocumentCompleted(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (_documentComplated)
            {
                this.resizeMap();
            }
            base.OnResize(e);
        }
    }
}
