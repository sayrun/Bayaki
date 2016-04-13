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

        public bykIFv1.Point GetPoint(DateTime targetDate, double margineSeconds)
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
                if(utcTime > pnt.Time)
                {
                    orless = pnt;
                }
                if(utcTime == pnt.Time)
                {
                    return pnt;
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
                if (s3.TotalSeconds <= margineSeconds)
                {
                    return ormore;
                }
            }

            return null;
        }

        public void Remove()
        {
            string locations = System.IO.Path.Combine(_savePath, _saveFileName);
            File.Delete(locations);
        }
    }
}
