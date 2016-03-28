using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Bayaki
{
    class UpdateJpegFile : NowProcessingForm<JPEGFileItem>.ProcessingStrategy
    {
        private static Image _headerSeed;

        public static void SetHeaderSeed(Image headerSeed)
        {
            _headerSeed = headerSeed;
        }

        private void SetPropertyValue(Image bmp, int ID, short Type, byte[] value)
        {
            foreach (PropertyItem item in bmp.PropertyItems)
            {
                if (item.Id == ID)
                {
                    item.Type = Type;
                    item.Value = value;
                    item.Len = value.Length;
                    bmp.SetPropertyItem(item);
                    return;
                }
            }
            System.Drawing.Imaging.PropertyItem p1 = _headerSeed.PropertyItems[0];
            // 緯度
            p1.Id = ID;
            p1.Type = Type;
            p1.Value = value;
            p1.Len = value.Length;
            bmp.SetPropertyItem(p1);
        }

        private void RemovePropertyValue(Image bmp, int ID)
        {
            // 一致するものがあるときは削除
            foreach( int localID in bmp.PropertyIdList)
            {
                if( localID == ID)
                {
                    bmp.RemovePropertyItem(ID);
                    return;
                }
            }
        }

        public void DoProcess(JPEGFileItem jpegItem)
        {
            if (!jpegItem.IsRemoveLocation)
            {
                if (jpegItem.CurrentLocation == null && jpegItem.NewLocation == null)
                {
                    // 位置情報なし→なしだから、処理しない
                    return;
                }
            }

            Image bmp = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(jpegItem.FilePath, FileMode.Open, FileAccess.Read, FileShare.Write);
                bmp = Bitmap.FromStream(fs);

                if (null == bmp) return;

                if (null != jpegItem.NewLocation)
                {

                    // 書き込みデータに変換する
                    PointConvertor converter = new PointConvertor(jpegItem.NewLocation);

                    // 緯度
                    SetPropertyValue(bmp, 1, 2, converter.LatitudeMark);
                    SetPropertyValue(bmp, 2, 5, converter.Latitude);

                    // 経度
                    SetPropertyValue(bmp, 3, 2, converter.LongitudeMark);
                    SetPropertyValue(bmp, 4, 5, converter.Longtude);

                    if (null != converter.Altitude)
                    {
                        // 高度基準
                        SetPropertyValue(bmp, 5, 7, converter.AltitudeRef);

                        // 高度
                        SetPropertyValue(bmp, 6, 5, converter.Altitude);
                    }
                    else
                    {
                        RemovePropertyValue(bmp, 5);
                        RemovePropertyValue(bmp, 6);
                    }
                }
                else
                {
                    RemovePropertyValue(bmp, 1);
                    RemovePropertyValue(bmp, 2);
                    RemovePropertyValue(bmp, 3);
                    RemovePropertyValue(bmp, 4);
                    RemovePropertyValue(bmp, 5);
                    RemovePropertyValue(bmp, 6);
                }

                // 一時ファイルに保存します
                string workPath = Path.GetTempFileName();

                // 保存する
                bmp.Save(workPath);

                // 保存するためにクローズする
                bmp.Dispose();
                fs.Close();

                // 元ファイルを削除します。
                File.Delete(jpegItem.FilePath);

                // 一時ファイルを移動します。
                File.Move(workPath, jpegItem.FilePath);
            }
            finally
            {
                if( null != fs)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
        }
    }
}
