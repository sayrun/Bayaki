using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bykIFv1;
using System.IO;

namespace Bayaki
{
    class TrackItemCacheLoaded : ITrackItemCache
    {
        private bykIFv1.TrackItem _item;
        private string _fileName;

        public TrackItemCacheLoaded(bykIFv1.TrackItem item, string fileName)
        {
            this._item = item;
            this._fileName = fileName;
        }

        public TrackItem GetTrackItem(out ITrackItemCache nextStatus)
        {
            nextStatus = this;

            return this._item;
        }

        public string SaveTrackItem(out ITrackItemCache nextStatus, string path)
        {
            nextStatus = this;

            // もともとあるものをロードされているので、ここでは保存しない

            return _fileName;
        }

        public void UpdateTrackItem(out ITrackItemCache nextStatus, string path)
        {
            nextStatus = new TrackItemCacheModified(_item, _fileName);
            // 処理なし
            return;
        }
    }
}
