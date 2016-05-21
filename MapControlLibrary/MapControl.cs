using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapControlLibrary
{
    public delegate void NotifyMakerDrag(double lat, double lon);

    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class MapControl: WebBrowser
    {
        private IDocumentState _proxy;

        public event NotifyMakerDrag OnMakerDrag;

        public enum MapProvider
        {
            GOOGLE,
            YAHOO
        };

        private MapProvider _provider;
        private string _key;

        public MapControl()
        {
            InitializeComponent();

            _proxy = new DocumentStateNotInitialized(this);

            base.ObjectForScripting = this;
        }

        /// <summary>
        /// Mapを表示します。
        /// </summary>
        /// <param name="provider">Mapの提供元を設定します</param>
        /// <param name="key">Key(ID)を設定します</param>
        /// <param name="delay">ture時、処理メソッドがコールされるまで地図表示をしません。アクセス数低減目的。遅延描画時はDocumentに任意のHTMLを表示できます。</param>
        public void Initialize(MapProvider provider, string key, bool delay = true)
        {
            _provider = provider;
            _key = key;

            // すぐに表示する必要がある場合
            if(! delay)
            {
                lock( _proxy)
                {
                    _proxy = new DocumentStateInitalizing(this);
                    _Start();
                }
            }
        }

        internal void _Start()
        {
            base.ScriptErrorsSuppressed = true;

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                // ネットワークが利用できるので、MAPスクリプトを読み込みます
                switch (_provider)
                {
                    case MapProvider.GOOGLE:
                        base.DocumentText = Properties.Resources.googlemapsHTML.Replace("[[KEY]]", _key);
                        break;
                    case MapProvider.YAHOO:
                        base.DocumentText = Properties.Resources.yahoomapsHTML.Replace("[[KEY]]", _key);
                        break;
                    default:
                        new Exception("不正なプロバイダです");
                        return;
                }
            }
            else
            {
                // ネットワークが利用できない
                lock (_proxy)
                {
                    _proxy = new DocumentStateNetworkNotAvailable(this);
                }
            }
        }

        public MapProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        public void notifyLatLon(object lat, object lon)
        {
            double dLat = (double)lat;
            double dLon = (double)lon;

            OnMakerDrag(dLat, dLon);
        }

        public void resetMarker()
        {
            lock(_proxy)
            {
                try
                {
                    _proxy = _proxy.resetMarker();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _resetMarker()
        {
            InvokeScript("resetMarker");
        }

        public void movePos(double latitude, double longitude)
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.movePos(latitude, longitude);
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        private void InvokeScript(string functionName)
        {
            object obj = this.Document.InvokeScript(functionName);

            bool result;
            if (!bool.TryParse(obj.ToString(), out result))
            {
                result = false;
            }
            
            // javascriptが実行できなかったっぽい
            if( false == result)
            {
                throw new JavascriptInvokeException(functionName);
            }
        }

        private void InvokeScript(string functionName, object[] param)
        {
            object obj = this.Document.InvokeScript(functionName, param);

            bool result;
            if (!bool.TryParse(obj.ToString(), out result))
            {
                result = false;
            }

            // javascriptが実行できなかったっぽい
            if (false == result)
            {
                throw new JavascriptInvokeException(functionName);
            }
        }

        internal void _movePos(double latitude, double longitude)
        {
            InvokeScript("movePos", new object[] { latitude, longitude });
        }

        public void resizeMap()
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.resizeMap();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _resizeMap()
        {
            InvokeScript("resizeMap");
        }

        internal void _Initialize()
        {
            InvokeScript("Initialize");
        }

        public void dropMarker()
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.dropMarker();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _dropMarker()
        {
            InvokeScript("dropMarker");
        }

        public void clearPoint()
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.clearPoint();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _clearPoint()
        {
            InvokeScript("clearPoint");
        }

        public void addPoint(double latitude, double longitude, string title)
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.addPoint(latitude, longitude, title);
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _addPoint(double latitude, double longitude, string title)
        {
            string markerText = string.Empty;
            if (0 < title.Length)
            {
                switch (_provider)
                {
                    case MapProvider.GOOGLE:
                        markerText = string.Format("{0}\r\n{1:0.######}, {2:0.######}", title, latitude, longitude);
                        break;
                    case MapProvider.YAHOO:
                        markerText = string.Format("{0}<br>{1:0.######}, {2:0.######}", title, latitude, longitude);
                        break;
                    default:
                        throw new Exception("不正なプロバイダです");
                }
            }
            InvokeScript("addPoint", new object[] { latitude, longitude, markerText });
        }

        public void drawPolyline()
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.drawPolyline();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
        }

        internal void _drawPolyline()
        {
            InvokeScript("drawPolyline");
        }

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.initializeScript();
                }
                catch(Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
            base.OnDocumentCompleted(e);
        }

        protected override void OnResize(EventArgs e)
        {
            lock (_proxy)
            {
                try
                {
                    _proxy = _proxy.resizeMap();
                }
                catch (Exception)
                {
                    _proxy = new DocumentStateJavascriptError(this);
                }
            }
            base.OnResize(e);
        }

        internal void _SetNetUnavailableHTML()
        {
            // ネットワークが利用できないので、エラー用のHTMLを表示します。
            base.DocumentText = Properties.Resources.nonetHTML;
        }

        internal void _SetJavascriptErrorHTML()
        {
            // ネットワークが利用できないので、エラー用のHTMLを表示します。
            base.DocumentText = Properties.Resources.scriptErrorHTML;

        }
    }
}
