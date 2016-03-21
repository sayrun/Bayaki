using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        }

        private void TrackPointPreviewForm_Load(object sender, EventArgs e)
        {
            this.Text = _trackItem.Name;

#if _MAP_YAHOO
            _routePreview.DocumentText = Properties.Resources.yahoomapsHTML;
#else
#if _MAP_GOOGLE
            _routePreview.DocumentText = Properties.Resources.googlemapsHTML;
#endif
#endif
        }

        private void _routePreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string format;
#if _MAP_YAHOO
            format = "{0}<br>{1:0.######}, {2:0.######}";
#else
#if _MAP_GOOGLE
            format = "{0}\r\n{1:0.######}, {2:0.######}";
#endif
#endif

            // 初期化してあげます
            _routePreview.Document.InvokeScript("Initialize");
            _routePreview.Document.InvokeScript("clearPoint");

            // パス情報を追加していきます
            bykIFv1.Point pm = _trackItem.Items[0];
            string markerText = (pm.Interest) ? string.Format(format, pm.Time.ToLocalTime(), pm.Latitude, pm.Longitude) : string.Empty;
            _routePreview.Document.InvokeScript("addPoint", new object[] { pm.Latitude, pm.Longitude, markerText });
            foreach (bykIFv1.Point p in _trackItem.Items)
            {
                if( p.Interest || (pm.Time.AddSeconds(10) < p.Time))
                {
                    pm = p;
                    markerText = (pm.Interest) ? string.Format(format, pm.Time.ToLocalTime(), pm.Latitude, pm.Longitude) : string.Empty;
                    _routePreview.Document.InvokeScript("addPoint", new object[] { pm.Latitude, pm.Longitude, markerText });
                }
            }
            if (pm != _trackItem.Items[_trackItem.Items.Count - 1])
            {
                pm = _trackItem.Items[_trackItem.Items.Count - 1];
                markerText = (pm.Interest) ? string.Format(format, pm.Time.ToLocalTime(), pm.Latitude, pm.Longitude) : string.Empty;
                _routePreview.Document.InvokeScript("addPoint", new object[] { pm.Latitude, pm.Longitude, markerText });
            }

            // 描画を実行します。
            _routePreview.Document.InvokeScript("drawPolyline");
        }

        private void _routePreview_SizeChanged(object sender, EventArgs e)
        {
            _routePreview.Document.InvokeScript("resizeMap");
        }
    }
}
