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
        Single maxSpeed;

        public TrackPointPreviewForm(bykIFv1.TrackItem trackItem)
        {
            InitializeComponent();

            _trackItem = trackItem;
            maxSpeed = 200;
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
            SetGraphData();
        }

        internal class HogeX : GraphControlLibrary.IDataFormater
        {
            private DateTime _dt;
            public HogeX(DateTime dt)
            {
                _dt = dt.ToLocalTime();
            }
            public string ConvertFrom(float d)
            {
                DateTime date = _dt.AddSeconds(d);
                return date.ToString();
            }
        }

        internal class HogeY : GraphControlLibrary.IDataFormater
        {
            public string ConvertFrom(float d)
            {
                return string.Format("{0:F2} km/h", d);
            }
        }

        private void SetGraphData()
        {
            bykIFv1.Point start = _trackItem.Items[0];
            bykIFv1.Point end = _trackItem.Items[_trackItem.Items.Count - 1];


            List<GraphControlLibrary.PointData> speedList = new List<GraphControlLibrary.PointData>();

            {
                double range = 60;
                {
                    TimeSpan ts = _trackItem.Items[_trackItem.Items.Count - 1].Time - _trackItem.Items[0].Time;
                    if( 1 >= ts.TotalMinutes)
                    {
                        range = 1;
                    }
                }

                // １分平均（データが1分以下なら平均しないよ）
                for (DateTime time = start.Time; time <= end.Time; time = time.AddSeconds(60))
                {
                    DateTime work = time.AddSeconds(60);
                    // 60分いないのデータ一覧を取得
                    var q = _trackItem.Items.Where(x => time <= x.Time && work > x.Time).OrderBy(x => x.Time);

                    // 取得したデータ一覧で速度を求める
                    bykIFv1.Point p1 = null;
                    double speed = 0;
                    int itemCount = 0;
                    foreach (bykIFv1.Point p in q)
                    {
                        if (null == p1)
                        {
                            // 最初の一湖なので、データを取り出すだけね。
                            speed = p.Speed;
                            itemCount = 1;
                        }
                        else
                        {
                            // 前回値との差分から速度をみるよ
                            double speedSrc = GetSpeed(p, p1);
                            if (0 < speedSrc)
                            {
                                speed += speedSrc;
                                ++itemCount;
                            }
                        }
                        p1 = p;
                    }
                    TimeSpan s2 = time - start.Time;

                    // 速度を平均とkm/hへ変換する
                    if (0 < itemCount)
                    {
                        speed /= itemCount;
                        // m/sからkm/hへ
                        speed *= 3600;
                        speed /= 1000;
                    }
                    else
                    {
                        speed = 0;
                    }

                    // 登録
                    speedList.Add(new GraphControlLibrary.PointData((Single)s2.TotalSeconds, (Single)speed));
                }
            }

            TimeSpan s = end.Time - start.Time;
            Single TotalSeconds = (Single)s.TotalSeconds;

            GraphControlLibrary.ScaleData xMin = new GraphControlLibrary.ScaleData(start.Time.ToString(), 0);
            GraphControlLibrary.ScaleData xMax = new GraphControlLibrary.ScaleData(end.Time.ToString(), TotalSeconds);
            GraphControlLibrary.ScaleSet ss = new GraphControlLibrary.ScaleSet( "時間", "s", xMin, xMax, new HogeX(start.Time));

            if( 1 <= s.TotalDays)
            {
                // 一日以上の差があるので、0:00にだけ線を引く
                DateTime scale = new DateTime(start.Time.Year, start.Time.Month, start.Time.Day, 0, 0, 0);
                while(end.Time > scale)
                {
                    scale = scale.AddDays(1);
                    TimeSpan s2 = scale - start.Time;
                    if (end.Time > scale)
                    {
                        ss.Items.Add(new GraphControlLibrary.ScaleData(scale.ToLocalTime().ToString("yyyy/MM/dd"), (Single)s2.TotalSeconds));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if( 1 <= s.TotalHours)
                {
                    // 一時間以上差があるので、x:00に線を引く
                    DateTime scale = new DateTime(start.Time.Year, start.Time.Month, start.Time.Day, start.Time.Hour, 0, 0);
                    while (end.Time > scale)
                    {
                        scale = scale.AddHours(1);
                        TimeSpan s2 = scale - start.Time;
                        if (end.Time > scale)
                        {
                            if (0 >= ss.Items.Count)
                            {
                                ss.Items.Add(new GraphControlLibrary.ScaleData(scale.ToLocalTime().ToString("yyyy/MM/dd HH:mm"), (Single)s2.TotalSeconds));
                            }
                            else
                            {
                                ss.Items.Add(new GraphControlLibrary.ScaleData(scale.ToLocalTime().ToString("HH:mm"), (Single)s2.TotalSeconds));
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if( 1 <= s.TotalMinutes)
                    {
                        // 1分以上差があるので、5分ごとに線を引く
                        DateTime scale = new DateTime(start.Time.Year, start.Time.Month, start.Time.Day, start.Time.Hour, start.Time.Minute, 0);
                        while (end.Time > scale)
                        {
                            scale = scale.AddMinutes(5);
                            TimeSpan s2 = scale - start.Time;
                            if (end.Time > scale)
                            {
                                if (0 >= ss.Items.Count)
                                {
                                    ss.Items.Add(new GraphControlLibrary.ScaleData(scale.ToLocalTime().ToString("yyyy/MM/dd HH:mm"), (Single)s2.TotalSeconds));
                                }
                                else
                                {
                                    ss.Items.Add(new GraphControlLibrary.ScaleData(scale.ToLocalTime().ToString("HH:mm"), (Single)s2.TotalSeconds));
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            GraphControlLibrary.ScaleSet ss2 = new GraphControlLibrary.ScaleSet("時速", "km/h", new GraphControlLibrary.ScaleData("0 km", 0), new GraphControlLibrary.ScaleData(string.Format("{0:F2}", maxSpeed), (float)maxSpeed), new HogeY());
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed / 2), (Single)(maxSpeed / 2)));
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed), (Single)(maxSpeed)));
            GraphControlLibrary.GraphSet gset = new GraphControlLibrary.GraphSet(ss, ss2, true);

            gset.Items.AddRange(speedList);

            _graphView.BegineUpdate();

            _graphView.Items.Add(gset);

            _graphView.EndUpdate();
        }

        private class Speedrange
        {
            public readonly int Range;
            public int Count;

            public Speedrange( int r)
            {
                Range = r;
                Count = 0;
            }
        };

        #region 速度を設定する
        private double GetSpeed(bykIFv1.Point current, bykIFv1.Point prev)
        {
            if (0 < current.Speed) return current.Speed;

            // 10分以上差があるなら、時速算出しない
            TimeSpan ts = current.Time - prev.Time;
            if (ts.TotalMinutes >= 10) return current.Speed;

            double dis = PointDistance.Distance(current, prev);
            double result = dis / ts.TotalSeconds;

            return result;
        }
        #endregion


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

        private void rangeXXkmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item) return;

            maxSpeed = (int)item.Tag;

            GraphControlLibrary.GraphSet gset = _graphView.Items[0];

            GraphControlLibrary.ScaleSet ss2 = new GraphControlLibrary.ScaleSet("時速", "km/h", new GraphControlLibrary.ScaleData("0 km", 0), new GraphControlLibrary.ScaleData(string.Format("{0:F2}", maxSpeed), (float)maxSpeed), new HogeY());
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed / 2), (Single)(maxSpeed / 2)));
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed), (Single)(maxSpeed)));
            GraphControlLibrary.GraphSet newGset = new GraphControlLibrary.GraphSet(gset.XScale, ss2, true);

            newGset.Items.AddRange(gset.Items);

            _graphView.BegineUpdate();

            _graphView.Items.Clear();
            _graphView.Items.Add(newGset);

            _graphView.EndUpdate();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Single work;
            foreach (ToolStripMenuItem item in contextMenuStrip1.Items)
            {
                work = (int)item.Tag;
                item.Checked = (maxSpeed == work);
            }
        }
    }
}
