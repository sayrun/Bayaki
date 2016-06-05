using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    [Serializable]
    class TrackItemSummary : ISerializable
    {
        public readonly UInt32 ID;

        public readonly DateTime From;
        public readonly DateTime To;
        public readonly int PointCount;
        public readonly string Description;
        public string _name;

        private string _saveFileName;

        private static string _savePath;
        private static UInt32 IDseed = 0;

        private const int DATA_VERSION = 0x0100;

        ITrackItemCache _trackItemProxy;

        public TrackItemSummary( bykIFv1.TrackItem track)
        {
            track.Normalize();

            ID = GenerateID();

            PointCount = track.Items.Count;
            From = track.Items[0].Time;
            TimeSpan span = System.TimeZoneInfo.Local.GetUtcOffset(From);
            From = From.Add(span);

            To = track.Items[PointCount - 1].Time;
            span = System.TimeZoneInfo.Local.GetUtcOffset(To);
            To = To.Add(span);

            Description = track.Description;

            _name = track.Name;
            _trackItemProxy = new TrackItemCacheNotYetSaved( track);

            _saveFileName = string.Empty;
        }

        /// <summary>
        /// 処理用の管理IDを生成する
        /// ここでは、起動中に有効であることが分かればよいから、生成は連番で。
        /// </summary>
        /// <returns></returns>
        private static UInt32 GenerateID()
        {
            ++IDseed;

            return IDseed;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (this._name != value)
                {
                    this._name = value;

                    bykIFv1.TrackItem item = _trackItemProxy.GetTrackItem(out _trackItemProxy);
                    item.Name = value;

                    _trackItemProxy.UpdateTrackItem(out _trackItemProxy, _savePath);
                }
            }
        }

        /// <summary>
        /// リカバリー用コンストラクタ
        /// </summary>
        /// <param name="track">リカバリーデータ</param>
        /// <param name="filePath">リカバリー元のファイル</param>
        public TrackItemSummary(bykIFv1.TrackItem track, string filePath)
        {
            track.Normalize();

            PointCount = track.Items.Count;
            From = track.Items[0].Time;
            TimeSpan span = System.TimeZoneInfo.Local.GetUtcOffset(From);
            From = From.Add(span);

            To = track.Items[PointCount - 1].Time;
            span = System.TimeZoneInfo.Local.GetUtcOffset(To);
            To = To.Add(span);

            Description = track.Description;

            _name = track.Name;
            _trackItemProxy = new TrackItemCacheLoaded(track, filePath);

            _saveFileName = filePath;
        }

        public TrackItemSummary(SerializationInfo info, StreamingContext context)
        {
            ID = GenerateID();

            int version = info.GetInt32("Version");
            PointCount = info.GetInt32("PointCount");
            From = info.GetDateTime("From");
            To = info.GetDateTime("To");
            _name = info.GetString("Name");
            Description = info.GetString("Description");
            _saveFileName = info.GetString("saveFileName");

            _trackItemProxy = new TrackItemCacheNotLoaded(_savePath, _saveFileName);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // 持っているデータを保存します
            _saveFileName = _trackItemProxy.SaveTrackItem(out _trackItemProxy, _savePath);

            info.AddValue("Version", DATA_VERSION);
            info.AddValue("PointCount", PointCount);
            info.AddValue("From", From);
            info.AddValue("To", To);
            info.AddValue("Name", _name);
            info.AddValue("Description", Description);
            info.AddValue("saveFileName", _saveFileName);
        }

        public static string SavePath
        {
            get
            {
                return _savePath;
            }
            set
            {
                _savePath = value;
            }
        }

        public bool IsContein(DateTime targetDate, double margineSeconds)
        {
            return (From.AddSeconds(-1 * margineSeconds) <= targetDate && To.AddSeconds(margineSeconds) >= targetDate);
        }

        public bykIFv1.TrackItem TrackItem
        {
            get
            {
                bykIFv1.TrackItem result = _trackItemProxy.GetTrackItem(out _trackItemProxy);

                return result;
            }
        }

        public bykIFv1.Point GetPoint(DateTime targetDate, double margineSeconds, double margineDstaince)
        {
            // 含まれない場合NULLを返すよ
            if (!IsContein(targetDate, margineSeconds)) return null;

            // 指定された時間をUTCに変換する
            TimeSpan diff = System.TimeZoneInfo.Local.GetUtcOffset(targetDate);
            DateTime utcTime = targetDate.Subtract(diff);
            bykIFv1.Point orless = null;
            bykIFv1.TrackItem trackItem = _trackItemProxy.GetTrackItem(out _trackItemProxy);
            foreach ( bykIFv1.Point pnt in trackItem.Items)
            {
                if (utcTime == pnt.Time)
                {
                    return pnt;
                }

                if (utcTime > pnt.Time)
                {
                    orless = pnt;
                }

                // ソートされているから省略する
                if (utcTime < pnt.Time)
                {
                    if( null != orless)
                    {
                        TimeSpan s1 = utcTime - orless.Time;
                        TimeSpan s2 = pnt.Time - utcTime;
                        if( s1 < s2)
                        {
                            if( s1.TotalSeconds <= margineSeconds)
                            {
                                return orless;
                            }
                        }
                        else
                        {
                            if (s2.TotalSeconds <= margineSeconds)
                            {
                                return pnt;
                            }
                        }

                        // 前回値の二点間の距離をみるよ
                        double d = Distance(orless, pnt);
                        if (d < margineDstaince)
                        {
                            // 前後の距離が近いので、時間のブレは大きいが、位置情報として採用する
                            // 近い方
                            if (s1 < s2)
                            {
                                return orless;
                            }
                            else
                            {
                                return pnt;
                            }


                        }
                    }
                    else
                    {
                        TimeSpan s2 = pnt.Time - utcTime;
                        if (s2.TotalSeconds <= margineSeconds)
                        {
                            return pnt;
                        }
                    }
                    break;
                }
            }

            // 最終の点よりも少しあとを再処理する
            if (2 < trackItem.Items.Count)
            {
                bykIFv1.Point ormore = trackItem.Items[trackItem.Items.Count - 1];

                TimeSpan s3 = utcTime - ormore.Time;
                if (0 <= s3.TotalSeconds)
                {
                    if (s3.TotalSeconds <= margineSeconds)
                    {
                        return ormore;
                    }
                }
            }

            return null;
        }

        public void Remove()
        {
            string locations = System.IO.Path.Combine(_savePath, _saveFileName);
            File.Delete(locations);
        }

        /// <summary>
        /// 二点間の距離を求める(ヒュベニの公式)
        /// ※
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        private double Distance(bykIFv1.Point pt1, bykIFv1.Point pt2 )
        {
            const double PI = 3.1415926535898;
            // a = 6,378,137
            const double a = 6378137;
            // f = 1 / 298.257 223 563
            const double f = 1 / 298.257223563;
            // b = a - ( a * f)
            const double b = a - (a * f);

            double x1 = (pt1.Longitude * PI) / 180;
            double x2 = (pt2.Longitude * PI) / 180;
            double y1 = (pt1.Latitude * PI) / 180;
            double y2 = (pt2.Latitude * PI) / 180;

            // e = √((a^2 - b^2) / a^2)
            //double e = Math.Sqrt((Math.Pow(a, 2) + Math.Pow(b, 2)) / Math.Pow(a, 2));
            const double e = 1.4118447577583941;
            // μy = (y1 + y2) / 2
            double uy = (y1 + y2) / 2;
            // W = √(1-(e^2 * sin(μy)^2))
            double W = Math.Sqrt(1 - (Math.Pow(e, 2) * Math.Pow(Math.Sin((uy * PI) / 180), 2)));
            // N = a / W
            double N = a / W;
            // M = a * (1 - e^2) / W^3
            double M = (a * (1 - Math.Pow(e, 2))) / Math.Pow(W, 3);
            // dy = y1 - y2
            double dy = y1 - y2;
            // dx = x1 - x2
            double dx = x1 - x2;

            // d = √((dy*M)^2 + (dx*N*cos μy)^2)
            double d = Math.Sqrt(Math.Pow(dy * M, 2) + Math.Pow(dx * N * Math.Cos(uy), 2));

            return d;

        }
    }
}
