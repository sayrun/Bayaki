using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bayaki
{
    public partial class TrackPointPreviewForm : Form
    {
        bykIFv1.TrackItem _trackItem;

        public TrackPointPreviewForm(bykIFv1.TrackItem trackItem)
        {
            InitializeComponent();

            _trackItem = trackItem;

            //webBrowser1.DocumentText = Properties.Resources.googlemapsHTML;
        }

        private void TrackPointPreviewForm_Load(object sender, EventArgs e)
        {
            bykIFv1.Point pm = _trackItem.Items[0];
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>");
            sb.Append("<script type=\"text/javascript\" src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"http://maps.google.com/maps/api/js?sensor=false&v=3.22\"></script>");
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("var map;");

            sb.Append("function initialize() {");
            sb.AppendFormat("var y = {0};", pm.Latitude);
            sb.AppendFormat("var x = {0};", pm.Longitude);
            sb.Append("var latlng = new google.maps.LatLng(y, x);");
            sb.Append("var opts = {");
            sb.Append("zoom: 14,");
            sb.Append("center: latlng,");
            sb.Append("streetViewControl: false,");
            sb.Append("mapTypeId: google.maps.MapTypeId.ROADMAP");
            sb.Append("};");
            sb.Append("map = new google.maps.Map(document.getElementById(\"gmap\"), opts);");

            sb.Append("var drawPathCoordinates = [");
            sb.AppendFormat("new google.maps.LatLng({0}, {1})", pm.Latitude, pm.Longitude);
            foreach (bykIFv1.Point p in _trackItem.Items)
            {
                if (pm.Time.AddSeconds(10) < p.Time)
                {
                    pm = p;
                    sb.AppendFormat(", new google.maps.LatLng({0}, {1})", pm.Latitude, pm.Longitude);
                }
            }
            if( pm != _trackItem.Items[_trackItem.Items.Count - 1])
            {
                pm = _trackItem.Items[_trackItem.Items.Count - 1];
                sb.AppendFormat(", new google.maps.LatLng({0}, {1})", pm.Latitude, pm.Longitude);
            }

            sb.Append("];");
            sb.Append("var flightPath=new google.maps.Polyline({");
            sb.Append("path: drawPathCoordinates,");
            sb.Append("strokeColor: '#8855FF',");
            sb.Append("strokeOpacity: 0.8,");
            sb.Append("strokeWeight: 4");
            sb.Append("});");
            sb.Append("flightPath.setMap(map);");
            sb.Append("}");

            sb.Append("</script>");
            sb.Append("<body topmargin=\"0\" leftmargin=\"0\" bottommargin=\"0\" rightmargin=\"0\" onload=\"initialize()\">");
            sb.Append("<div id=\"gmap\" style=\"width:100%; height:100%\"></div>");
            sb.Append("</body>");
            sb.Append("</html>");

            webBrowser1.DocumentText = sb.ToString();
        }
    }
}
