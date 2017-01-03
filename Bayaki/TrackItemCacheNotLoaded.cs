using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bykIFv1;

namespace Bayaki
{
    class TrackItemCacheNotLoaded : ITrackItemCache
    {
        private string _savePath;
        private string _fileName;
        private bykIFv1.TrackItem _item;

        public TrackItemCacheNotLoaded( string path, string fileName)
        {
            _savePath = path;
            _fileName = fileName;
        }

        public TrackItem GetTrackItem(out ITrackItemCache nextStatus)
        {
            string filePath = System.IO.Path.Combine(_savePath, _fileName);
            using (TrackItemReader tir = new TrackItemReader(filePath))
            {
                _item = tir.Read();
            }

            nextStatus = new TrackItemCacheLoaded(_item, _fileName);
            return _item;
        }

        public string SaveTrackItem(out ITrackItemCache nextStatus, string path)
        {
            nextStatus = this;

            return _fileName;
        }

        public void UpdateTrackItem(out ITrackItemCache nextStatus, string path)
        {
            throw new Exception("未ロードから変更へは状態を更新できません。");
        }
    }
}
