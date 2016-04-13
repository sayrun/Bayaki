using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bykIFv1;
using System.IO;

namespace Bayaki
{
    class TrackItemCacheModified : ITrackItemCache
    {
        private bykIFv1.TrackItem _item;
        private string _fileName;

        public TrackItemCacheModified(bykIFv1.TrackItem item, string fileName)
        {
            _item = item;
            _fileName = fileName;
        }
        public TrackItem GetTrackItem(out ITrackItemCache nextStatus)
        {
            nextStatus = this;
            return _item;
        }

        public string SaveTrackItem(out ITrackItemCache nextStatus, string path)
        {
            // 上書き保存
            string savaPath = Path.Combine(path, this._fileName);
            using (Stream stream = new FileStream(savaPath, FileMode.Create, FileAccess.Write))
            {
                stream.SetLength(0);
                stream.Flush();
                using (TrackItemWriter tiw = new TrackItemWriter(stream))
                {
                    // 保存します
                    tiw.Write(_item);
                }
            }

            nextStatus = new TrackItemCacheLoaded(_item, _fileName);

            return _fileName;
        }

        public void UpdateTrackItem(out ITrackItemCache nextStatus, string path)
        {
            nextStatus = this;
            // 処理なし
            return;
        }
    }
}
