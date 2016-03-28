using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Bayaki
{
    public enum Orientation
    {
        Orientation_0,
        Orientation_90,
        Orientation_180,
        Orientation_270
    };

    class JPEGFileItem
    {
        private readonly string _filePath;

        private DateTime _targetdate;
        private bykIFv1.Point _currentLocation;
        private bykIFv1.Point _newLocation;
        private Orientation _orientation;

        private Image _thumNail;

        private bool _remove;

        public JPEGFileItem(string filePath, Size thumNailSize, Color transColor)
        {
            _filePath = filePath;

            AnalyzeExif(_filePath, thumNailSize, transColor);

            _newLocation = null;

            _remove = false;
        }

        public bool IsRemoveLocation
        {
            get
            {
                return _remove;
            }
        }

        public Image ThumNail
        {
            get
            {
                return _thumNail;
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(_filePath);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
        }

        /// <summary>
        /// 撮影時間
        /// </summary>
        public DateTime DateTimeOriginal
        {
            get
            {
                return _targetdate;
            }
        }

        public bool IsModifed
        {
            get
            {
                if (_remove) return true;
                if (null != _newLocation) return true;
                return false;
            }
        }

        public void RemoveLocation()
        {
            _remove = true;
            _newLocation = null;
        }

        public bykIFv1.Point DisplayLocation
        {
            get
            {
                if (_remove) return null;
                if (null != _newLocation) return _newLocation;
                if (null != _currentLocation) return _currentLocation;
                return null;
            }
        }

        public bykIFv1.Point CurrentLocation
        {
            get
            {
                return _currentLocation;
            }
        }

        public bykIFv1.Point NewLocation
        {
            get
            {
                return _newLocation;
            }
            set
            {
                _newLocation = value;
            }
        }

        private double ToDegree(UInt32[] source)
        {
            double result = source[0];
            result /= source[1];

            double dPar1 = 60;
            for (int index = 1; index < source.Length / 2; ++index)
            {
                double dWork = source[index * 2];
                dWork /= dPar1;
                dWork /= source[index * 2 + 1];

                result += dWork;
                dPar1 *= 60;
            }

            return result;
        }

        private void AnalyzeExif(string filePath, Size thumNailSize, Color transColor)
        {
            DateTime dt = DateTime.MinValue;
            string sNS = string.Empty;
            string sEW = string.Empty;

            UInt32[] uLon = null;
            UInt32[] uLat = null;

            using (Bitmap bmp = new Bitmap(filePath))
            {
                // 0x0001-北緯(N) or 南緯(S)(Ascii)
                // 0x0002-緯度(数値)(Rational)
                // 0x0003-東経(E) or 西経(W)(Ascii)
                // 0x0004-経度(数値)(Rational)
                // 0x0005-高度の基準(Byte)
                // 0x0006-高度(数値)(Rational)
                // 0x0007-GPS時間(原子時計の時間)(Rational)
                // 0x0008-測位に使った衛星信号(Ascii)
                // 0x0009-GPS受信機の状態(Ascii)
                // 0x000A-GPSの測位方法(Ascii)
                // 0x000B-測位の信頼性(Rational)
                // 0x000C-速度の単位(Ascii)
                // 0x000D-速度(数値)(Rational)
                foreach (System.Drawing.Imaging.PropertyItem item in bmp.PropertyItems)
                {
                    switch (item.Id)
                    {
                        // 0x0112 - 回転情報(Orientation)
                        case 0x0112:
                            switch (item.Value[0])
                            {
                                case 3:
                                    // 時計回りに180度回転しているので、180度回転して戻す
                                    _orientation = Orientation.Orientation_180;
                                    break;
                                case 6:
                                    // 時計回りに270度回転しているので、90度回転して戻す
                                    _orientation = Orientation.Orientation_90;
                                    break;
                                case 8:
                                    // 時計回りに90度回転しているので、270度回転して戻す
                                    _orientation = Orientation.Orientation_270;
                                    break;
                            }
                            break;
                        // 0x9004 - デジタルデータの作成日時
                        case 0x9004:
                            if (0 == dt.CompareTo(DateTime.MinValue))
                            {
                                string s = System.Text.Encoding.ASCII.GetString(item.Value);
                                s = s.Trim(new char[] { '\0' });
                                {
                                    s = System.Text.RegularExpressions.Regex.Replace(s,
                                            @"^(?<year>(?:\d\d)?\d\d):(?<month>\d\d?):(?<day>\d\d?)",
                                            "${year}/${month}/${day}");
                                }
                                dt = DateTime.Parse(s);
                            }
                            break;

                        // 0x9003 - 原画像データの生成日時
                        case 0x9003:
                            {
                                string s = System.Text.Encoding.ASCII.GetString(item.Value);
                                s = s.Trim(new char[] { '\0' });
                                {
                                    s = System.Text.RegularExpressions.Regex.Replace(s,
                                            @"^(?<year>(?:\d\d)?\d\d):(?<month>\d\d?):(?<day>\d\d?)",
                                            "${year}/${month}/${day}");
                                }
                                dt = DateTime.Parse(s);
                            }
                            break;

                        // 0x0001-北緯(N) or 南緯(S)(Ascii)
                        case 0x0001:
                            sNS = System.Text.Encoding.ASCII.GetString(item.Value);
                            sNS = sNS.Trim(new char[] { '\0' });
                            break;

                        // 0x0002-緯度(数値)(Rational):latitude - 緯度
                        case 0x0002:
                            {
                                if (4 <= item.Len)
                                {
                                    uLat = new UInt32[item.Len / 4];
                                    System.Buffer.BlockCopy(item.Value, 0, uLat, 0, item.Len);
                                }
                            }
                            break;

                        // 0x0003-東経(E) or 西経(W)(Ascii)
                        case 0x0003:
                            sEW = System.Text.Encoding.ASCII.GetString(item.Value);
                            sEW = sEW.Trim(new char[] { '\0' });
                            break;
                        // 0x0004-経度(数値)(Rational)：longitude - 経度
                        case 0x0004:
                            {
                                if (4 <= item.Len)
                                {
                                    uLon = new UInt32[item.Len / 4];
                                    System.Buffer.BlockCopy(item.Value, 0, uLon, 0, item.Len);
                                }
                            }
                            break;
                    }
                }

                // ついでにサムネイルを作る
                _thumNail = stretchImage(bmp, thumNailSize, transColor);
            }

            _targetdate = dt;

            if (null != uLon && null != uLat)
            {

                // 経度
                double lon = ToDegree(uLon);
                if (0 == string.Compare(sEW, "W", true))
                {
                    lon *= -1;
                }

                // 緯度
                double lat = ToDegree(uLat);
                if (0 == string.Compare(sNS, "S", true))
                {
                    lat *= -1;
                }

                _currentLocation = new bykIFv1.Point(dt, lat, lon, 0, 0, true);
            }
            else
            {
                _currentLocation = null;
            }
        }

        private Bitmap stretchImage(Image bmp, Size size, Color clr)
        {
            Bitmap result = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            Graphics gs = Graphics.FromImage(result);

            gs.FillRectangle(new SolidBrush(clr), 0, 0, size.Width, size.Height);

            float scaleW = size.Width;
            scaleW /= bmp.Width;
            float scaleH = size.Height;
            scaleH /= bmp.Height;

            float scale = Math.Min(scaleW, scaleH);
            float targetW = bmp.Width * scale;
            float targetH = bmp.Height * scale;

            gs.DrawImage(bmp, (size.Width - targetW) / 2, (size.Height - targetH) / 2, targetW, targetH);

            return result;
        }
    }
}
