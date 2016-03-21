using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GoogleMapTimelinePlugin
{
    public partial class DownloadForm : Form
    {
        private double _maxSpeed = 340.0;

        public DownloadForm()
        {
            InitializeComponent();
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime target = dateTimePicker1.Value;

            string s = string.Format("https://accounts.google.com/ServiceLogin?hl=ja&passive=true&continue=https://www.google.com/maps/timeline/kml?authuser=0&pb=!1m8!1m3!1i{0}!2i{1}!3i{2}!2m3!1i{0}!2i{1}!3i{2}", target.Year, target.Month - 1, target.Day);

            webBrowser1.Navigate(new Uri(s));
        }

        bykIFv1.TrackItem _data;

        private void webBrowser1_FileDownload(object sender, EventArgs e)
        {
            try
            {
                using (XmlReader xr = XmlReader.Create(webBrowser1.DocumentStream))
                {
                    _data = Read(xr);
                }
            }
            catch
            {
                return;
            }
        }

        public bykIFv1.TrackItem TrackItem
        {
            get
            {
                return _data;
            }
        }

        private bykIFv1.TrackItem Read(XmlReader _xmlReader)
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

                                                                            double maxDistance = _maxSpeed * s.TotalSeconds;

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

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Segments[e.Url.Segments.Length - 1].EndsWith(".kml"))
            {
                string filename = e.Url.Segments[e.Url.Segments.Length - 1];

                System.Net.WebClient client = new System.Net.WebClient();
                client.DownloadFileAsync(e.Url, "c:\\hoge.txt");

                e.Cancel = true;
            }

            System.Diagnostics.Debug.Print("uri = {0}", e.Url);
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
