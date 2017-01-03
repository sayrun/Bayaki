using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bayaki
{
    class KMLReader : IDisposable
    {
        private XmlReader _xmlReader;
        private readonly double _maxSpeed;

        public KMLReader(string filePath, double maxDistance)
        {
            _xmlReader = XmlReader.Create(filePath);
            _maxSpeed = maxDistance;
        }

        public KMLReader(XmlReader xr, double maxDistance)
        {
            _xmlReader = xr;
            _maxSpeed = maxDistance;
        }

        public void Dispose()
        {
            if (null != _xmlReader)
            {
                _xmlReader.Close();
                _xmlReader = null;
            }
        }

        public bykIFv1.TrackItem Read()
        {
            bykIFv1.Point olditem = null;

            bykIFv1.TrackItem result = new bykIFv1.TrackItem("KML data", DateTime.Now);


            List<DateTime> wayPoints = new List<DateTime>();
            while (_xmlReader.Read())
            {
                switch (_xmlReader.NodeType)
                {
                    case System.Xml.XmlNodeType.Element:
                        if (0 == string.Compare(_xmlReader.Name, "Placemark", true))
                        {
                            if (!_xmlReader.IsEmptyElement)
                            {
                                string datetime = string.Empty;
                                string coord = string.Empty;

                                while (_xmlReader.Read())
                                {
                                    switch (_xmlReader.NodeType)
                                    {
                                        case System.Xml.XmlNodeType.Element:
                                            if (0 == string.Compare(_xmlReader.Name, "name", true))
                                            {
                                                result.Name = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "description", true))
                                            {
                                                result.Description = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "gx:Track", true))
                                            {
                                                if (!_xmlReader.IsEmptyElement)
                                                {
                                                    while (_xmlReader.Read())
                                                    {
                                                        switch (_xmlReader.NodeType)
                                                        {
                                                            case System.Xml.XmlNodeType.Element:
                                                                if (0 == string.Compare(_xmlReader.Name, "when", true))
                                                                {
                                                                    datetime = _xmlReader.ReadString();
                                                                }
                                                                else if (0 == string.Compare(_xmlReader.Name, "gx:coord", true))
                                                                {
                                                                    coord = _xmlReader.ReadString();

                                                                    string[] coords = coord.Split(' ');

                                                                    DateTime dt;
                                                                    if (DateTime.TryParse(datetime, out dt))
                                                                    {
                                                                        double lon = double.Parse(coords[1]);
                                                                        double lat = double.Parse(coords[0]);
                                                                        double ele = (coords.Length > 2) ? double.Parse(coords[2]) : double.NaN;

                                                                        bykIFv1.Point item = new bykIFv1.Point(dt.ToUniversalTime(), lon, lat, ele, 0, false);

                                                                        bool skip = false;
                                                                        if (null != olditem)
                                                                        {
                                                                            TimeSpan s = item.Time - olditem.Time;

                                                                            double maxDistance = Math.Abs( _maxSpeed * s.TotalSeconds);

                                                                            // 音速を超える移動は破棄する
                                                                            skip = (maxDistance < item.Location.GetDistanceTo(olditem.Location));
                                                                        }
                                                                        if (false == skip)
                                                                        {
                                                                            result.Items.Add(item);
                                                                        }
                                                                        olditem = item;
                                                                    }

                                                                    datetime = string.Empty;
                                                                }
                                                                else
                                                                {
                                                                    if (!_xmlReader.IsEmptyElement)
                                                                    {
                                                                        while (_xmlReader.Read())
                                                                            if (_xmlReader.NodeType == System.Xml.XmlNodeType.EndElement) break;
                                                                    }
                                                                }
                                                                continue;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!_xmlReader.IsEmptyElement)
                                                {
                                                    while (_xmlReader.Read())
                                                        if (_xmlReader.NodeType == System.Xml.XmlNodeType.EndElement) break;
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

            return result;
        }

    }
}
