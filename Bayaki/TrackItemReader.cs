using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bayaki
{
    class TrackItemReader : IDisposable
    {
        private XmlReader _xmlReader;

        public TrackItemReader( string filePath)
        {
            _xmlReader = System.Xml.XmlReader.Create(filePath);
        }

        public TrackItemReader(XmlReader xr)
        {
            _xmlReader = xr;
        }

        public void Dispose()
        {
            if( null != _xmlReader)
            {
                _xmlReader.Close();
                _xmlReader = null;
            }
        }

        public bykIFv1.TrackItem Read()
        {
            bykIFv1.TrackItem result = null;
            List<bykIFv1.Point> points = new List<bykIFv1.Point>();

            DateTime createTime = DateTime.Now;
            string name = string.Empty;

            List<DateTime> wayPoints = new List<DateTime>();
            while (_xmlReader.Read())
            {
                switch (_xmlReader.NodeType)
                {
                    case System.Xml.XmlNodeType.Element:
                        if (0 == string.Compare(_xmlReader.Name, "time", true))
                        {
                            string sTime = _xmlReader.ReadString();
                            DateTime dt;
                            if (DateTime.TryParse(sTime, out dt))
                            {
                                createTime = dt;
                            }
                        }
                        else if (0 == string.Compare(_xmlReader.Name, "name", true))
                        {
                            name = _xmlReader.ReadString();
                        }
                        else if (0 == string.Compare(_xmlReader.Name, "wpt", true))
                        {
                            if (!_xmlReader.IsEmptyElement)
                            {
                                string lon = _xmlReader.GetAttribute("lon");
                                string lat = _xmlReader.GetAttribute("lat");

                                string sEle = string.Empty;
                                string sTime = string.Empty;
                                string sSpeed = string.Empty;

                                while (_xmlReader.Read())
                                {
                                    switch (_xmlReader.NodeType)
                                    {
                                        case System.Xml.XmlNodeType.Element:
                                            if (0 == string.Compare(_xmlReader.Name, "ele", true))
                                            {
                                                sEle = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "time", true))
                                            {
                                                sTime = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "speed", true))
                                            {
                                                sSpeed = _xmlReader.ReadString();
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

                                DateTime dt;
                                if (DateTime.TryParse(sTime, out dt))
                                {
                                    wayPoints.Add(dt);
                                }
                            }
                        }
                        else if (0 == string.Compare(_xmlReader.Name, "trkpt", true))
                        {
                            if (!_xmlReader.IsEmptyElement)
                            {
                                string lon = _xmlReader.GetAttribute("lon");
                                string lat = _xmlReader.GetAttribute("lat");

                                string sEle = string.Empty;
                                string sTime = string.Empty;
                                string sSpeed = string.Empty;

                                while (_xmlReader.Read())
                                {
                                    switch (_xmlReader.NodeType)
                                    {
                                        case System.Xml.XmlNodeType.Element:
                                            if (0 == string.Compare(_xmlReader.Name, "ele", true))
                                            {
                                                sEle = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "time", true))
                                            {
                                                sTime = _xmlReader.ReadString();
                                            }
                                            else if (0 == string.Compare(_xmlReader.Name, "speed", true))
                                            {
                                                sSpeed = _xmlReader.ReadString();
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

                                DateTime dt;
                                if (DateTime.TryParse(sTime, out dt))
                                {
                                    bool wayPoint = (0 <= wayPoints.IndexOf(dt));

                                    bykIFv1.Point item = new bykIFv1.Point(dt.ToUniversalTime(), double.Parse(lat), double.Parse(lon), double.Parse(sEle), double.Parse(sSpeed), wayPoint);
                                    points.Add(item);
                                }
                            }
                        }
                        break;
                }
            }

            if( 0 >= name.Length)
            {
                name = "location data";
            }
            // 応答を作成しますよ
            result = new bykIFv1.TrackItem(name, createTime);
            result.Items = points;

            return result;
        }
    }
}
