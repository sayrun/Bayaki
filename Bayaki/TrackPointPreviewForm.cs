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

        public TrackPointPreviewForm(bykIFv1.TrackItem trackItem)
        {
                InitializeComponent();

                _trackItem = trackItem;
        }

        private void TrackPointPreviewForm_Load(object sender, EventArgs e)
        {
            this.Text = _trackItem.Name;

#if _MAP_GOOGLE
            _mapView.Initialize(MapControlLibrary.MapControl.MapProvider.GOOGLE, Properties.Resources.KEY_GOOGLE);
#else
#if _MAP_YAHOO
            _mapView.Initialize(MapControlLibrary.MapControl.MapProvider.YAHOO, Properties.Resources.KEY_YAHOO);
#else
#error      コンパイルオプションとして対象のマッププロバイダを設定してください。
#endif
#endif
            DrawRouteMap();
        }

        private void DrawRouteMap()
        {
            // 初期化してあげます
            _mapView.clearPoint();

            // パス情報を追加していきます
            bykIFv1.Point pm = _trackItem.Items[0];
            string markerText = string.Empty;

            _mapView.addPoint(pm.Latitude, pm.Longitude, string.Format(Properties.Resources.MSG7, pm.Time.ToLocalTime()));
            foreach (bykIFv1.Point p in _trackItem.Items)
            {
                if (p.Interest || (pm.Time.AddSeconds(6) < p.Time))
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
                    _mapView.addPoint(pm.Latitude, pm.Longitude, markerText);
                }
            }
            if (pm != _trackItem.Items[_trackItem.Items.Count - 1])
            {
                pm = _trackItem.Items[_trackItem.Items.Count - 1];
                _mapView.addPoint(pm.Latitude, pm.Longitude, string.Format(Properties.Resources.MSG8, pm.Time.ToLocalTime()));
            }

            // 描画を実行します。
            _mapView.drawPolyline();
        }
    }
}
