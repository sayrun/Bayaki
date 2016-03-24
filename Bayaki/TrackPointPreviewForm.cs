using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Bayaki
{
    public partial class TrackPointPreviewForm : Form
    {
        private bykIFv1.TrackItem _trackItem;
        private MapControl _previewMap;

        public TrackPointPreviewForm(bykIFv1.TrackItem trackItem)
        {
            InitializeComponent();

            _trackItem = trackItem;
            _previewMap = new MapControl(_mapView);
        }

        private void TrackPointPreviewForm_Load(object sender, EventArgs e)
        {
            this.Text = _trackItem.Name;
        }

        private void _routePreview_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // 初期化してあげます
            _previewMap.Initialize();
            _previewMap.clearPoint();

            // パス情報を追加していきます
            bykIFv1.Point pm = _trackItem.Items[0];
            string markerText = string.Empty;

            _previewMap.addPoint( pm.Latitude, pm.Longitude, string.Format( Properties.Resources.MSG7, pm.Time.ToLocalTime()));
            foreach (bykIFv1.Point p in _trackItem.Items)
            {
                if( p.Interest || (pm.Time.AddSeconds(6) < p.Time))
                {
                    pm = p;
                    if (pm == _trackItem.Items[_trackItem.Items.Count - 1])
                    {
                        markerText = string.Format(Properties.Resources.MSG8, pm.Time.ToLocalTime());
                    }
                    else
                    {
                        markerText = (pm.Interest) ? pm.Time.ToLocalTime().ToString() : string.Empty;
                    }
                    _previewMap.addPoint(pm.Latitude, pm.Longitude, markerText);
                }
            }
            if (pm != _trackItem.Items[_trackItem.Items.Count - 1])
            {
                pm = _trackItem.Items[_trackItem.Items.Count - 1];
                _previewMap.addPoint(pm.Latitude, pm.Longitude, string.Format(Properties.Resources.MSG8, pm.Time.ToLocalTime()));
            }

            // 描画を実行します。
            _previewMap.drawPolyline();
        }

        private void _routePreview_SizeChanged(object sender, EventArgs e)
        {
            _previewMap.resizeMap();
        }
    }
}
