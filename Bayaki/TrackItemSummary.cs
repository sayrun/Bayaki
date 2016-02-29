using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
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
        public string Name;

        private string _saveFileName;

        private static string _savePath;

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

            Name = track.Name;
            _item = track;

            _saveFileName = string.Empty;
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
                    using (Stream stream = new FileStream(Path.Combine(_savePath, fileName), FileMode.CreateNew))
                    {
                        // 保存します
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(stream, _item);

                        // ファイルが作成できたからこれをファイル名にします。
                        _saveFileName = fileName;
                        break;
                    }
                }
                catch (IOException)
                {
                    // ファイル名が作成できないみたいだらから
                    continue;
                }
            }
        }

        public TrackItemSummary(SerializationInfo info, StreamingContext context)
        {
            PointCount = info.GetInt32("PointCount");
            From = info.GetDateTime("From");
            To = info.GetDateTime("To");
            Name = info.GetString("Name");
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

            info.AddValue("PointCount", PointCount);
            info.AddValue("From", From);
            info.AddValue("To", To);
            info.AddValue("Name", Name);
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

        public bool IsContein(DateTime targetDate)
        {
            return (From <= targetDate && To >= targetDate);
        }

        public bykIFv1.Point GetPoint(DateTime targetDate)
        {
            // 含まれない場合NULLを返すよ
            if (!IsContein(targetDate)) return null;

            // 読み込まれていない場合読み込む
            if ( null == _item)
            {
                string locations = System.IO.Path.Combine(_savePath, _saveFileName);
                using (var stream = new FileStream(locations, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    _item = bf.Deserialize(stream) as bykIFv1.TrackItem;
                }
            }

            // 指定された時間をUTCに変換する
            TimeSpan diff = System.TimeZoneInfo.Local.GetUtcOffset(targetDate);
            DateTime utcTime = targetDate.Subtract(diff);
            foreach ( bykIFv1.Point pnt in _item.Items)
            {
                if(utcTime == pnt.Time)
                {
                    return pnt;
                }
                // ソートされているから省略する
                if (utcTime <= pnt.Time)
                {
                    return null;
                }
            }
            return null;
        }

        public ListViewItem GetListViewItem()
        {
            ListViewItem viewItem = new ListViewItem(From.ToString("yyyy/MM/dd HH:mm:ss"));
            viewItem.SubItems.Add(To.ToString("yyyy/MM/dd HH:mm:ss"));
            viewItem.SubItems.Add(PointCount.ToString());
            viewItem.SubItems.Add(Name);
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
