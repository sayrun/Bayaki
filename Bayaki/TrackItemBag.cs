using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    internal class TrackItemBag
    {
        private List<TrackItemSummary> _locations;
        private string _savePath;
        private const int MAX_TIMEDIFF = (60 * 20); // 根拠ないけど、20分以上差異があれば採用しない


        public TrackItemBag(string savePath)
        {
            _locations = new List<TrackItemSummary>();
            _savePath = savePath;
        }

        public void Save()
        {
            string locations = System.IO.Path.Combine(_savePath, "BayakiSummary.dat");
            if (0 < _locations.Count)
            {
                using (var stream = new FileStream(locations, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, _locations);
                }
            }
            else
            {
                // 保存すべきものがない場合削除します
                File.Delete(locations);
            }
        }

        public void Load()
        {
            // 場所情報のサマリーを取得する
            string locations = System.IO.Path.Combine(_savePath, "BayakiSummary.dat");
            if (File.Exists(locations))
            {
                using (var stream = new FileStream(locations, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    _locations = bf.Deserialize(stream) as List<TrackItemSummary>;
                }
            }
            else
            {
                // ファイルがないけど、リカバリーを試みよう
                _locations = TrackItemSummary.RecoveryRead();
                if (0 < _locations.Count)
                {
                    Save();
                }
            }
        }

        public ListViewItem[] GetListViewItems()
        {
            ListViewItem[] items = new ListViewItem[_locations.Count];

            for( int index = 0; index < _locations.Count; ++index)
            {
                items[index] = _locations[index].GetListViewItem();
            }

            return items;
        }

        public void AddItem(TrackItemSummary item)
        {
            _locations.Add(item);
        }

        public void RemoveItem(TrackItemSummary item)
        {
            _locations.Remove(item);
            // 関連情報も消す
            item.Remove();
        }

        public void Up(TrackItemSummary item)
        {
            int index = _locations.IndexOf(item);
            if (0 >= index) return;

            _locations.Remove(item);
            _locations.Insert(index - 1, item);
        }

        public void Down(TrackItemSummary item)
        {
            int index = _locations.IndexOf(item);
            if (_locations.Count <= index) return;

            _locations.Remove(item);
            _locations.Insert(index + 1, item);

        }

        public bykIFv1.Point FindPoint(DateTime localTime)
        {
            List<bykIFv1.Point> points = new List<bykIFv1.Point>();
            foreach (TrackItemSummary summary in _locations)
            {
                if (summary.IsContein(localTime))
                {
                    points.Add(summary.GetPoint(localTime));
                }
            }

            // 一致しないならNULLを返す
            if (0 >= points.Count) return null;

            // 一致したものが１つならそれを返す
            if (1 == points.Count) return points[0];

            // 複数の位置情報元で一致したら、近いデータを採用する
            // 比較用にUTCにする
            DateTime utcdt = localTime.Subtract(System.TimeZoneInfo.Local.GetUtcOffset(localTime));
            // 複数の結果が得られたら、より時間の小さいほう
            bykIFv1.Point result = null;
            double diff = 0;
            double diffOld = double.MaxValue;
            foreach (bykIFv1.Point pnt in points)
            {
                TimeSpan s = pnt.Time - utcdt;
                diff = Math.Abs(s.TotalSeconds);
                if (diff >= MAX_TIMEDIFF) continue;
                if (diffOld > diff)
                {
                    result = pnt;
                    diffOld = diff;
                }
            }
            return result;
        }
    }
}
