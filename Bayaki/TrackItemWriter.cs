using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bayaki
{
    internal class TrackItemWriter : IDisposable
    {
        private XmlWriter _xmlWriter;

        public TrackItemWriter(string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.Encoding = Encoding.UTF8;

            _xmlWriter = XmlWriter.Create(filePath, settings);
        }

        internal TrackItemWriter(string filePath, bool indent)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = indent;
            settings.Encoding = Encoding.UTF8;

            _xmlWriter = XmlWriter.Create(filePath, settings);
        }


        public TrackItemWriter(XmlWriter xw)
        {
            _xmlWriter = xw;
        }

        public void Dispose()
        {
            if (null != _xmlWriter)
            {
                _xmlWriter.Flush();
                _xmlWriter.Close();
                _xmlWriter = null;
            }
        }

        public void Write(bykIFv1.TrackItem trackItem)
        {
            _xmlWriter.WriteStartElement("gpx");
            {
                _xmlWriter.WriteAttributeString("version", "1.0");
                _xmlWriter.WriteAttributeString("creator", "Bayaki");

                _xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                _xmlWriter.WriteAttributeString("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd");

                // 名前
                if (0 < trackItem.Name.Length)
                {
                    _xmlWriter.WriteElementString("name", trackItem.Name);
                }

                // 詳細
                if (0 < trackItem.Description.Length)
                {
                    _xmlWriter.WriteElementString("desc", trackItem.Description);
                }

                // 作成時間
                _xmlWriter.WriteElementString("time", trackItem.CreateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                _xmlWriter.WriteStartElement("bounds");
                {
                    decimal minlat = trackItem.Items[0].Latitude;
                    decimal maxlat = trackItem.Items[0].Latitude;
                    decimal minlon = trackItem.Items[0].Longitude;
                    decimal maxlon = trackItem.Items[0].Longitude;
                    foreach (bykIFv1.Point point in trackItem.Items)
                    {
                        if (point.Latitude < minlat) minlat = point.Latitude;
                        if (point.Latitude < maxlat) maxlat = point.Latitude;
                        if (point.Longitude < minlon) minlon = point.Longitude;
                        if (point.Longitude > maxlon) maxlon = point.Longitude;
                    }
                    _xmlWriter.WriteAttributeString("minlat", minlat.ToString("0.######"));
                    _xmlWriter.WriteAttributeString("minlon", minlon.ToString("0.######"));
                    _xmlWriter.WriteAttributeString("maxlat", maxlat.ToString("0.######"));
                    _xmlWriter.WriteAttributeString("maxlon", maxlon.ToString("0.######"));
                }
                _xmlWriter.WriteEndElement();

                // waypointを出力します
                int index = 0;
                foreach (bykIFv1.Point point in trackItem.Items)
                {
                    ++index;
                    if (point.Interest)
                    {
                        _xmlWriter.WriteStartElement("wpt");
                        {
                            _xmlWriter.WriteAttributeString("lat", point.Latitude.ToString("0.######"));
                            _xmlWriter.WriteAttributeString("lon", point.Longitude.ToString("0.######"));

                            _xmlWriter.WriteElementString("ele", point.Elevation.ToString());
                            _xmlWriter.WriteElementString("time", point.Time.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                            _xmlWriter.WriteElementString("speed", point.Speed.ToString("0.######"));
                            _xmlWriter.WriteElementString("name", string.Format("PT{0:D4}", index));
                        }
                        _xmlWriter.WriteEndElement();
                    }
                }

                _xmlWriter.WriteStartElement("trk");
                {
                    _xmlWriter.WriteStartElement("trkseg");
                    {
                        index = 0;
                        foreach (bykIFv1.Point point in trackItem.Items)
                        {
                            _xmlWriter.WriteStartElement("trkpt");
                            {
                                _xmlWriter.WriteAttributeString("lat", point.Latitude.ToString("0.######"));
                                _xmlWriter.WriteAttributeString("lon", point.Longitude.ToString("0.######"));

                                _xmlWriter.WriteElementString("ele", point.Elevation.ToString());
                                _xmlWriter.WriteElementString("time", point.Time.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                                _xmlWriter.WriteElementString("speed", point.Speed.ToString("0.######"));
                                _xmlWriter.WriteElementString("name", string.Format("PT{0:D4}", ++index));
                            }
                            _xmlWriter.WriteEndElement();
                        }
                    }
                    _xmlWriter.WriteEndElement();
                }
                _xmlWriter.WriteEndElement();
            }
            _xmlWriter.WriteEndElement();
        }
    }
}
