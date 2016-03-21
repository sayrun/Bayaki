using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bykIFv1;
using System.Device.Location;

namespace Bayaki
{
    class KMLLoaderv1 : bykIFv1.PlugInInterface
    {
        private const double MAX_SPEED = 340.0; // 音速(m/s)

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.KMLLoader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.KMLLoader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "select KML file";
            dialog.Filter = "kml file(*.kml)|*.kml|all file(*.*)|*.*||";
            dialog.DefaultExt = ".kml";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = true;

            if (DialogResult.OK == dialog.ShowDialog(owner))
            {
                List<TrackItem> result = new List<TrackItem>();
                foreach (string targetPath in dialog.FileNames)
                {
                    TrackItem item = LoadKmlFile(targetPath);
                    if (null != item)
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

        private static TrackItem LoadKmlFile(string filePath)
        {
            bykIFv1.Point olditem = null;

            TrackItem result = new TrackItem(filePath, DateTime.Now);

            result.Description = string.Format("from [{0}]", filePath);

            using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(new System.IO.StreamReader(filePath)))
            {
                List<DateTime> wayPoints = new List<DateTime>();
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case System.Xml.XmlNodeType.Element:
                            if (0 == string.Compare(xr.Name, "Placemark", true))
                            {
                                if (!xr.IsEmptyElement)
                                {
                                    string datetime = string.Empty;
                                    string coord = string.Empty;

                                    while (xr.Read())
                                    {
                                        switch (xr.NodeType)
                                        {
                                            case System.Xml.XmlNodeType.Element:
                                                if (0 == string.Compare(xr.Name, "name", true))
                                                {
                                                    result.Name = xr.ReadString();
                                                }
                                                else if (0 == string.Compare(xr.Name, "description", true))
                                                {
                                                    result.Description = xr.ReadString();
                                                }
                                                else if (0 == string.Compare(xr.Name, "gx:Track", true))
                                                {
                                                    if (!xr.IsEmptyElement)
                                                    {
                                                        while (xr.Read())
                                                        {
                                                            switch (xr.NodeType)
                                                            {
                                                                case System.Xml.XmlNodeType.Element:
                                                                    if (0 == string.Compare(xr.Name, "when", true))
                                                                    {
                                                                        datetime = xr.ReadString();
                                                                    }
                                                                    else if (0 == string.Compare(xr.Name, "gx:coord", true))
                                                                    {
                                                                        coord = xr.ReadString();

                                                                        string[] coords = coord.Split(' ');

                                                                        DateTime dt;
                                                                        if (DateTime.TryParse(datetime, out dt))
                                                                        {
                                                                            double lon = double.Parse(coords[1]);
                                                                            double lat = double.Parse(coords[0]);
                                                                            double ele = (coords.Length > 2) ? double.Parse(coords[2]) : double.NaN;

                                                                            bykIFv1.Point item = new bykIFv1.Point(dt.ToUniversalTime(), lon, lat, ele, 0, false);

                                                                            bool skip = false;
                                                                            if( null != olditem)
                                                                            {
                                                                                TimeSpan s = item.Time - olditem.Time;

                                                                                double maxDistance = MAX_SPEED * s.TotalSeconds;

                                                                                // 音速を超える移動は破棄する
                                                                                skip = (maxDistance < item.Location.GetDistanceTo(olditem.Location));
                                                                            }
                                                                            if (false == skip)
                                                                            {
                                                                                result.Items.Add(item);
                                                                                olditem = item;
                                                                            }
                                                                        }

                                                                        datetime = string.Empty;
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
                                                            }
                                                        }
                                                    }
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
