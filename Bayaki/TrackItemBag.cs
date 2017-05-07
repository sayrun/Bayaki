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
        private const int MAX_DISTDIFF = (80 * MAX_TIMEDIFF);   // 根拠ないけど、最大誤差時間＊80m範囲は近い時間を採用させる

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

        public UInt32 AddItem( UInt32 id, bykIFv1.TrackItem track)
        {
            TrackItemSummary tiSum = new TrackItemSummary(track);
            int index = _locations.FindIndex(x => x.ID == id);
            if (0 > index)
            {
                _locations.Add(tiSum);
            }
            else
            {
                _locations.Insert(index, tiSum);
            }

            return tiSum.ID;
        }

        public UInt32 AddItem(bykIFv1.TrackItem track)
        {
            TrackItemSummary tiSum = new TrackItemSummary(track);
            if (0 == _locations.Count)
            {
                _locations.Add(tiSum);
            }
            else
            {
                _locations.Insert(0, tiSum);
            }

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
                    bykIFv1.Point point = summary.GetPoint(localTime, MAX_TIMEDIFF, MAX_DISTDIFF);
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
                         where (null != d.GetPoint(localTime, MAX_TIMEDIFF, MAX_DISTDIFF))
                         select new { Point = d.GetPoint(localTime, MAX_TIMEDIFF, MAX_DISTDIFF), Distance = Math.Abs((d.GetPoint(localTime, MAX_TIMEDIFF, MAX_DISTDIFF).Time - utcdt).TotalSeconds) };

            // 一致しないならNULLを返す
            if (0 >= points.Count()) return null;

            var hoge = from dd in points
                       orderby dd.Distance
                       select dd.Point;

            return hoge.First();
        }
    }
}
