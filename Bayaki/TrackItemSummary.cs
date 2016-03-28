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
        public readonly DateTime From;
        public readonly DateTime To;
        public readonly int PointCount;
        public readonly string Description;
        public string _name;

        private string _saveFileName;

        private static string _savePath;

        private const int DATA_VERSION = 0x0100;

        bykIFv1.TrackItem _item;

        public TrackItemSummary( bykIFv1.TrackItem track)
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
            _item = track;

            _saveFileName = string.Empty;
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
                    bykIFv1.TrackItem item = this.TrackItem;
                    item.Name = value;

                    // ファイル名が設定されているなら更新が必要
                    if (0  < _saveFileName.Length)
                    {
                        // 名前を更新します。
                        using (Stream stream = new FileStream(Path.Combine(_savePath, _saveFileName), FileMode.Open))
                        {
                            stream.SetLength(0);
                            stream.Flush();
                            using (TrackItemWriter tiw = new TrackItemWriter(stream))
                            {
                                // 保存します
                                tiw.Write(_item);
                            }
                        }
                    }
                }
            }
        }

        private void SafeTrackItem()
        {
            // ファイルを作成してみて保存パス名を決める
            _saveFileName = string.Empty;
            string fileName = string.Empty;
            for (int index = 1; index < int.MaxValue; ++index)
            {
                fileName = string.Format("TrackItem{0:D4}.dat", index);
                try
                {
                    string savePath = Path.Combine(_savePath, fileName);
                    using (Stream stream = new FileStream(Path.Combine(_savePath, fileName), FileMode.CreateNew))
                    {
                        stream.SetLength(0);
                        stream.Flush();
                        using (TrackItemWriter tiw = new TrackItemWriter(stream))
                        {
                            // 保存します
                            tiw.Write(_item);

                            // ファイルが作成できたからこれをファイル名にします。
                            _saveFileName = fileName;
                            break;
                        }
                    }
                }
                catch (IOException)
                {
                    // ファイル名が作成できないみたいだらから
                    continue;
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
            _item = track;

            _saveFileName = filePath;
        }

        public TrackItemSummary(SerializationInfo info, StreamingContext context)
        {
            int version = info.GetInt32("Version");
            PointCount = info.GetInt32("PointCount");
            From = info.GetDateTime("From");
            To = info.GetDateTime("To");
            _name = info.GetString("Name");
            Description = info.GetString("Description");
            _saveFileName = info.GetString("saveFileName");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // 持っているデータを保存します
            if (0 >= _saveFileName.Length && null != _item)
            {
                SafeTrackItem();
            }

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

        private void RestoreTrackItem()
        {
            if (null == _item)
            {
                string locations = System.IO.Path.Combine(_savePath, _saveFileName);
                using (TrackItemReader tir = new TrackItemReader(locations))
                {
                    _item = tir.Read();

                    // 変更された名前を設定する
                    _item.Name = this._name;
                }
            }
        }

        public bykIFv1.TrackItem TrackItem
        {
            get
            {
                RestoreTrackItem();
                return _item;
            }
        }

        public bykIFv1.Point GetPoint(DateTime targetDate, double margineSeconds)
        {
            // 含まれない場合NULLを返すよ
            if (!IsContein(targetDate, margineSeconds)) return null;

            // 読み込まれていない場合読み込む
            RestoreTrackItem();

            // 指定された時間をUTCに変換する
            TimeSpan diff = System.TimeZoneInfo.Local.GetUtcOffset(targetDate);
            DateTime utcTime = targetDate.Subtract(diff);
            bykIFv1.Point orless = null;
            foreach ( bykIFv1.Point pnt in _item.Items)
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
            if (2 < _item.Items.Count)
            {
                bykIFv1.Point ormore = _item.Items[_item.Items.Count - 1];

                TimeSpan s3 = utcTime - ormore.Time;
                if (s3.TotalSeconds <= margineSeconds)
                {
                    return ormore;
                }
            }

            return null;
        }

        public ListViewItem GetListViewItem()
        {
            ListViewItem viewItem = new ListViewItem(_name);
            viewItem.SubItems.Add(From.ToString("yyyy/MM/dd HH:mm:ss"));
            viewItem.SubItems.Add(To.ToString("yyyy/MM/dd HH:mm:ss"));
            viewItem.SubItems.Add(PointCount.ToString());
            if ( null != Description && 0 <= Description.Length)
            {
                viewItem.ToolTipText = Description;
            }
            viewItem.Tag = this;

            return viewItem;
        }

        public void Remove()
        {
            string locations = System.IO.Path.Combine(_savePath, _saveFileName);
            File.Delete(locations);
        }
    }
}
