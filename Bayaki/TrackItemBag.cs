using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    internal delegate void BagChanged(TrackItemBag sender);

    internal class TrackItemBag
    {
        private List<TrackItemSummary> _locations;
        private string _savePath;
        private const int MAX_TIMEDIFF = (60 * 20); // 根拠ないけど、20分以上差異があれば採用しない

        public event BagChanged OnChanged;

        public TrackItemBag(string savePath)
        {
            _locations = new List<TrackItemSummary>();
            _savePath = savePath;
            TrackItemSummary.SavePath = savePath;
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
                OnChanged(this);
            }
            else
            {
                // ファイルがないけど、リカバリーを試みよう
                DirectoryInfo di = new DirectoryInfo(_savePath);
                foreach (FileInfo fi in di.GetFiles("TrackItem*.dat"))
                {
                    try
                    {
                        using (TrackItemReader tir = new TrackItemReader(fi.FullName))
                        {
                            bykIFv1.TrackItem item = tir.Read();
                            if (null != item)
                            {
                                TrackItemSummary summary = new TrackItemSummary(item, fi.FullName);

                                _locations.Add(summary);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 読み込みでエラーが発生する可能性は、この場合では無視する。
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }

                if (0 < _locations.Count)
                {
                    Save();
                    OnChanged(this);
                }
            }
        }

        public IEnumerable<TrackItemSummary> Items
        {
            get
            {
                return _locations;
            }
        }

        public UInt32 AddItem(bykIFv1.TrackItem track)
        {
            TrackItemSummary tiSum = new TrackItemSummary(track);
            _locations.Add(tiSum);
            OnChanged(this);

            return tiSum.ID;
        }

        public void Remove(UInt32 ID)
        {
            foreach( TrackItemSummary item in _locations)
            {
                if( item.ID == ID)
                {
                    _locations.Remove(item);
                    // 関連情報も消す
                    item.Remove();

                    OnChanged(this);

                    break;
                }
            }
        }

        public void Up( UInt32 ID)
        {
            for( int index = 0; index < _locations.Count; ++index)
            {
                if(ID == _locations[index].ID)
                {
                    TrackItemSummary item = _locations[index];

                    _locations.Remove(item);
                    _locations.Insert(index - 1, item);

                    OnChanged(this);

                    break;
                }
            }
        }

        public void Down(UInt32 ID)
        {
            for (int index = 0; index < _locations.Count; ++index)
            {
                if (ID == _locations[index].ID)
                {
                    TrackItemSummary item = _locations[index];

                    _locations.Remove(item);
                    _locations.Insert(index + 1, item);

                    OnChanged(this);

                    break;
                }
            }
        }

        public bykIFv1.Point FindPoint(DateTime localTime)
        {
            return FindPointForeach(localTime);
        }

        private bykIFv1.Point FindPointForeach(DateTime localTime)
        {
            // 複数の位置情報元で一致したら、近いデータを採用する
            // 比較用にUTCにする
            DateTime utcdt = localTime.Subtract(System.TimeZoneInfo.Local.GetUtcOffset(localTime));

            bykIFv1.Point result = null;
            double diff = 0;
            double diffOld = double.MaxValue;

            foreach (TrackItemSummary summary in _locations)
            {
                if (summary.IsContein(localTime, MAX_TIMEDIFF))
                {
                    bykIFv1.Point point = summary.GetPoint(localTime, MAX_TIMEDIFF);
                    if (null != point)
                    {
                        TimeSpan s = point.Time - utcdt;
                        diff = Math.Abs(s.TotalSeconds);

                        if (diffOld > diff)
                        {
                            result = point;
                            diffOld = diff;
                        }
                    }
                }
            }
            
            return result;
        }

        private bykIFv1.Point FindPointLinQ(DateTime localTime)
        {
            // 複数の位置情報元で一致したら、近いデータを採用する
            // 比較用にUTCにする
            DateTime utcdt = localTime.Subtract(System.TimeZoneInfo.Local.GetUtcOffset(localTime));

            var points = from d in _locations
                         where (null != d.GetPoint(localTime, MAX_TIMEDIFF))
                         select new { Point = d.GetPoint(localTime, MAX_TIMEDIFF), Distance = Math.Abs((d.GetPoint(localTime, MAX_TIMEDIFF).Time - utcdt).TotalSeconds) };

            // 一致しないならNULLを返す
            if (0 >= points.Count()) return null;

            var hoge = from dd in points
                       orderby dd.Distance
                       select dd.Point;

            return hoge.First();
        }
    }
}
