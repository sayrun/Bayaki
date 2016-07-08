﻿using System;
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

            double maxSpeed = 0;
            {
                int[] hoge = new int[] { 0, 0, 0, 0, 0 };
                // <=25/<=50/<=100/<=150/150<
                // 7/14/28/42

                foreach (bykIFv1.Point p in _trackItem.Items)
                {
                    if (0 >= p.Speed) continue;

                    if( 7 > p.Speed)
                    {
                        ++hoge[0];
                    }
                    else
                    {
                        if( 14 > p.Speed)
                        {
                            ++hoge[1];
                        }
                        else
                        {
                            if( 24 > p.Speed)
                            {
                                ++hoge[2];
                            }
                            else
                            {
                                if( 42 > p.Speed)
                                {
                                    ++hoge[3];
                                }
                                else
                                {
                                    ++hoge[4];
                                }
                            }
                        }
                    }
                }
                int maxCount = 0;
                int target = 0;
                for( int index = 0; index < hoge.Length; ++index)
                {
                    if( maxCount < hoge[index])
                    {
                        target = index;
                        maxCount = hoge[index];
                    }
                }
                switch(target)
                {
                    case 0:
                        maxSpeed = 50;
                        break;
                    case 1:
                        maxSpeed = 100;
                        break;
                    case 2:
                        maxSpeed = 200;
                        break;
                    case 3:
                        maxSpeed = 250;
                        break;
                    default:
                        maxSpeed = 300;
                        break;
                }
            }
            GraphControlLibrary.ScaleSet ss2 = new GraphControlLibrary.ScaleSet("時速", "km/h", new GraphControlLibrary.ScaleData("0 km", 0), new GraphControlLibrary.ScaleData(string.Format("{0:F2}", maxSpeed), (float)maxSpeed), new HogeY());
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed / 2), (Single)(maxSpeed / 2)));
            ss2.Items.Add(new GraphControlLibrary.ScaleData(string.Format("{0:F0}", maxSpeed), (Single)(maxSpeed)));
            GraphControlLibrary.GraphSet gset = new GraphControlLibrary.GraphSet(ss, ss2, true);


            // １分平均
            bykIFv1.Point p1 = _trackItem.Items[0];
            double speed = p1.Speed;
            int itemCount = 1;
            for ( int index = 1; index < _trackItem.Items.Count; ++index)
            {
                if( p1.Time.Year != _trackItem.Items[index].Time.Year
                    || p1.Time.Month != _trackItem.Items[index].Time.Month
                    || p1.Time.Day != _trackItem.Items[index].Time.Day
                    || p1.Time.Hour != _trackItem.Items[index].Time.Hour
                    || p1.Time.Minute != _trackItem.Items[index].Time.Minute)
                {
                    System.Diagnostics.Debug.Print("{2} - {0}/{1}", speed, itemCount, p1.Time);

                    speed /= itemCount;
                    // m/sからkm/hへ
                    speed *= 3600;
                    speed /= 1000;

                    TimeSpan s2 = p1.Time - start.Time;

                    if (0 < gset.Items.Count)
                    {
                        double hoge = (s2.TotalSeconds - gset.Items[gset.Items.Count - 1].X);
                        if (60 < (s2.TotalSeconds - gset.Items[gset.Items.Count - 1].X))
                        {
                            gset.Items.Add(new GraphControlLibrary.PointData(gset.Items[gset.Items.Count - 1].X + 1, 0));
                            if (120 < (s2.TotalSeconds - gset.Items[gset.Items.Count - 1].X))
                            {
                                gset.Items.Add(new GraphControlLibrary.PointData((Single)s2.TotalSeconds - 1, 0));
                            }
                        }
                    }

                    gset.Items.Add(new GraphControlLibrary.PointData((Single)s2.TotalSeconds, (Single)speed));

                    p1 = _trackItem.Items[index];
                    speed = p1.Speed;
                    itemCount = 1;
                }
                else
                {
                    if (0 < _trackItem.Items[index].Speed)
                    {
                        speed += _trackItem.Items[index].Speed;
                        ++itemCount;
                    }
                }
                // 最後のデータを加工する
                if (index == (_trackItem.Items.Count - 1))
                {
                    speed /= itemCount;

                    TimeSpan s2 = p1.Time - start.Time;
                    gset.Items.Add(new GraphControlLibrary.PointData((Single)s2.TotalSeconds, (Single)speed));
                }
            }

            _graphView.BegineUpdate();

            _graphView.Items.Add(gset);

            _graphView.EndUpdate();
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
