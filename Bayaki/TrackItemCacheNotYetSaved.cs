using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bykIFv1;
using System.IO;

namespace Bayaki
{
    class TrackItemCacheNotYetSaved : ITrackItemCache
    {
        private string _fileName;
        private bykIFv1.TrackItem _item;

        public TrackItemCacheNotYetSaved(bykIFv1.TrackItem item)
        {
            this._item = item;
            this._fileName = string.Empty;
        }

        public TrackItem GetTrackItem(out ITrackItemCache nextStatus)
        {
            nextStatus = this;
            return this._item;
        }

        public string SaveTrackItem(out ITrackItemCache nextStatus, string path)
        {
            // ファイルを作成してみて保存パス名を決める
            string fileName = string.Empty;
            for (int index = 1; index < int.MaxValue; ++index)
            {
                fileName = string.Format("TrackItem{0:D4}.dat", index);
                try
                {
                    string savePath = Path.Combine(path, fileName);
                    using (Stream stream = new FileStream(savePath, FileMode.CreateNew))
                    {
                        stream.SetLength(0);
                        stream.Flush();
                        using (TrackItemWriter tiw = new TrackItemWriter(stream))
                        {
                            // 保存します
                            tiw.Write(_item);

                            // ファイルが作成できたからこれをファイル名にします。
                            _fileName = fileName;
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
            nextStatus = new TrackItemCacheLoaded(_item, _fileName);
            return _fileName;
        }

        public void UpdateTrackItem(out ITrackItemCache nextStatus, string path)
        {
            nextStatus = this;

            // 保存されていないのですから、ここでは更新しません。
            return;
        }
    }
}
