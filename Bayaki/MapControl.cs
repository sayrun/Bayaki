using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    public delegate void NotifyMakerDrag(double lat, double lon);

    [System.Runtime.InteropServices.ComVisible(true)]
    public class MapControl
    {
        private WebBrowser _web;

        public event NotifyMakerDrag OnMakerDrag;

        public MapControl( WebBrowser web)
        {
            _web = web;
#if _MAP_YAHOO
            _web.DocumentText = Properties.Resources.yahoomapsHTML;
#else
#if _MAP_GOOGLE
            _web.DocumentText = Properties.Resources.googlemapsHTML;
#endif
#endif
            _web.ObjectForScripting = this;
        }

        public void notifyLatLon( object lat, object lon)
        {
            double dLat = (double)lat;
            double dLon = (double)lon;

            OnMakerDrag(dLat, dLon);
        }

        public void resetMarker()
        {
            _web.Document.InvokeScript("resetMarker");
        }

        public  void movePos(double latitude, double longitude)
        {
            _web.Document.InvokeScript("movePos", new object[] { latitude, longitude });
        }

        public void resizeMap()
        {
            _web.Document.InvokeScript("resizeMap");
        }

        public void Initialize()
        {
            _web.Document.InvokeScript("Initialize");
        }

        public void dropMarker()
        {
            _web.Document.InvokeScript("dropMarker");
        }

        public void clearPoint()
        {
            _web.Document.InvokeScript("clearPoint");
        }

        public void addPoint(double latitude, double longitude, string title)
        {
            string markerText = string.Empty;
            if (0 < title.Length)
            {
#if _MAP_YAHOO
                markerText = string.Format("{0}<br>{1:0.######}, {2:0.######}", title, latitude, longitude);
#else
#if _MAP_GOOGLE
                markerText = string.Format("{0}\r\n{1:0.######}, {2:0.######}", title, latitude, longitude);
#endif
#endif
            }
            _web.Document.InvokeScript("addPoint", new object[] { latitude, longitude, markerText });
        }

        public void drawPolyline()
        {
            _web.Document.InvokeScript("drawPolyline");
        }
    }
}
