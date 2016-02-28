using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bykIFv1;

namespace Bayaki
{
    class GPXLoaderv1 : bykIFv1.PlugInInterface, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.GPXLoader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.GPXLoader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "select gpx file";
            dialog.Filter = "gpx file(*.gpx)|*.gpx|all file(*.*)|*.*||";
            dialog.DefaultExt = "gpx";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = true;

            if (DialogResult.OK == dialog.ShowDialog(owner))
            {
                List<TrackItem> result = new List<TrackItem>();
                foreach (string targetPath in dialog.FileNames)
                {
                    TrackItem item = LoadGpxFile(targetPath);
                    if( null != item)
                    {
                        result.Add(item);
                    }
                }

                if (0 < result.Count)
                {
                    return result.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// GPXファイルの読み込み
        /// </summary>
        /// <param name="filePath">GPXファイルのパス</param>
        /// <returns>PGXから読み込んだ座標情報リスト</returns>
        private static TrackItem LoadGpxFile(string filePath)
        {
            TrackItem result = new TrackItem(filePath, DateTime.Now);

            result.Description = string.Format("from [{0}]", filePath);

            using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(new System.IO.StreamReader(filePath)))
            {
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:
                            if( 0 == string.Compare(xr.Name, "time", true))
                            {
                                string sTime = xr.ReadString();
                                DateTime dt;
                                if (DateTime.TryParse(sTime, out dt))
                                {
                                    result.CreateTime = dt;
                                }
                            }
                            if (0 == string.Compare(xr.Name, "name", true))
                            {
                                string name = xr.ReadString();
                                if (0 < name.Length)
                                {
                                    result.Name = name;
                                }
                            }
                            if (0 == string.Compare(xr.Name, "trkpt", true))
                            {
                                if (!xr.IsEmptyElement)
                                {
                                    string lon = xr.GetAttribute("lon");
                                    string lat = xr.GetAttribute("lat");

                                    string sEle = string.Empty;
                                    string sTime = string.Empty;
                                    string sSpeed = string.Empty;

                                    while (xr.Read())
                                    {
                                        switch (xr.NodeType)
                                        {
                                            case System.Xml.XmlNodeType.Element:
                                                if (0 == string.Compare(xr.Name, "ele", true))
                                                {
                                                    sEle = xr.ReadString();
                                                }
                                                else if (0 == string.Compare(xr.Name, "time", true))
                                                {
                                                    sTime = xr.ReadString();
                                                }
                                                else if (0 == string.Compare(xr.Name, "speed", true))
                                                {
                                                    sSpeed = xr.ReadString();
                                                }
                                                else
                                                {
                                                    if (!xr.IsEmptyElement)
                                                    {
                                                        while (xr.Read())
                                                            if (xr.NodeType == System.Xml.XmlNodeType.EndElement) break;
                                                    }
                                                }
                                                continue;

                                            case System.Xml.XmlNodeType.EndElement:
                                                break;

                                            default:
                                                continue;
                                        }
                                        break;
                                    }

                                    DateTime dt;
                                    if (DateTime.TryParse(sTime, out dt))
                                    {
                                        bykIFv1.Point item = new bykIFv1.Point(dt.ToUniversalTime(), decimal.Parse( lat), decimal.Parse(lon), decimal.Parse(sEle), decimal.Parse(sSpeed));
                                        result.Items.Add(item);
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return result;
        }
    }
}
