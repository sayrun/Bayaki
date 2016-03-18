using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bayaki
{
    public delegate void NotifyMakerDrag(double lat, double lon);

    [System.Runtime.InteropServices.ComVisible(true)]
    public class WebBrowserObserver
    {
        public event NotifyMakerDrag OnMakerDrag;

        public WebBrowserObserver()
        {

        }

        public void notifyLatLon( object lat, object lon)
        {
            double dLat = (double)lat;
            double dLon = (double)lon;

            OnMakerDrag(dLat, dLon);
        }
    }
}
