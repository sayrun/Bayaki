using System;
using System.Collections.Generic;
using System.Text;

namespace Map
{
    public delegate void NotifyMakerDrag(double lat, double lon);

    [System.Runtime.InteropServices.ComVisible(true)]
    public class Map : System.Windows.Forms.WebBrowser
    {
        public event NotifyMakerDrag OnMakerDrag;

        public enum MapProvider
        {
            GOOGLE,
            YAHOO
        };

        private MapProvider _provider;

        public Map() : base()
        {
            this.ObjectForScripting = this;
        }

        public MapProvider Provider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;

                switch( _provider)
                {
                    case MapProvider.GOOGLE:
                        base.DocumentText = Properties.Resources.googlemapsHTML;
                        break;
                    case MapProvider.YAHOO:
                        base.DocumentText = "<html><body>hoge</body></html>";
                        //base.DocumentText = Properties.Resources.yahoomapsHTML;
                        break;
                    default:
                        new Exception("不正なプロバイダです");
                        return;
                }
            }
        }

        public void LoadHtml()
        {
            string a = Properties.Resources.googlemapsHTML;
            string b = Properties.Resources.yahoomapsHTML;
            switch (_provider)
            {
                case MapProvider.GOOGLE:
                    base.DocumentText = Properties.Resources.googlemapsHTML;
                    break;
                case MapProvider.YAHOO:
                    base.DocumentText = Properties.Resources.yahoomapsHTML;
                    break;
                default:
                    new Exception("不正なプロバイダです");
                    return;
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
    }
}
