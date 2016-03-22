using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GoogleMapTimelinePlugin
{
    public partial class DownloadForm : Form
    {
        bykIFv1.TrackItem _trackItem;
        private double _maxSpeed = 340.0;
        private string _SID = string.Empty;
        private string _APISID = string.Empty;
        private string _SAPISID = string.Empty;
        private string _OGPC = string.Empty;

        public DownloadForm()
        {
            InitializeComponent();
        }

        private void DownloadForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            string s = @"https://accounts.google.com/ServiceLogin?hl=ja&passive=true&continue=https://www.google.com/";
            webBrowser1.Navigate(new Uri(s));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime target = dateTimePicker1.Value;

                string s = string.Format("https://www.google.com/maps/timeline/kml?authuser=0&pb=!1m8!1m3!1i{0}!2i{1}!3i{2}!2m3!1i{0}!2i{1}!3i{2}", target.Year, target.Month - 1, target.Day);

                // HTTPリクエストを作成
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(s);
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("OGPC", this._OGPC, "/", @"www.google.com"));
                request.CookieContainer.Add(new Cookie("SID", this._SID, "/", @"www.google.com"));
                request.CookieContainer.Add(new Cookie("APISID", this._APISID, "/", @"www.google.com"));
                request.CookieContainer.Add(new Cookie("SAPISID", this._SAPISID, "/", @"www.google.com"));
                // HTTPレスポンスを取得
                HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
                // レスポンスのストリームを取得
                using (Stream responcestream = responce.GetResponseStream())
                {
                    // XMLリーダーを作成
                    using (XmlReader reader = XmlReader.Create(responcestream))
                    {
                        _trackItem = Read(reader);
                    }
                }
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void webBrowser1_FileDownload(object sender, EventArgs e)
        {
            try
            {
                string sCookie =  webBrowser1.Document.Cookie;
                System.Diagnostics.Debug.Print("Cookie={0}", sCookie);

                Regex regex = new Regex(@"([^= ]+)=(.+)", RegexOptions.Compiled);
                foreach (string item in sCookie.Split(';'))
                {
                    if (regex.IsMatch(item))
                    {
                        Match match = regex.Match(item);
                        switch (match.Groups[1].Value)
                        {
                           case "SID":
                                 _SID = match.Groups[2].Value;

                                System.Diagnostics.Debug.Print("SID={0}", _SID);
                                button1.Enabled = true;
                                 break;
                            case "APISID":
                                _APISID = match.Groups[2].Value;
                                System.Diagnostics.Debug.Print("APISID={0}", _APISID);
                                break;
                            case "SAPISID":
                                _SAPISID = match.Groups[2].Value;
                                System.Diagnostics.Debug.Print("SAPISID={0}", _SAPISID);
                                break;
                            case "OGPC":
                                _OGPC = match.Groups[2].Value;
                                System.Diagnostics.Debug.Print("OGPC={0}", _OGPC);
                                break;

                        }
                    }
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
                return _trackItem;
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
    }
}
