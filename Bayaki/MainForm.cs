using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Reflection;
using System.Xml;

namespace Bayaki
{
    public partial class MainForm : Form
    {
        private string _workPath;
        private List<TrackItemSummary> _locations;
        private List<JPEGFileItem> _images;
        private List<string> _pluginPath;

        private enum EXPORT_FORMAT
        {
            INVALID,
            GPX,
            CSV,
            KML
        };

        public MainForm()
        {
            InitializeComponent();

            // メニュー処理簡略
            _exportGPX.Tag = EXPORT_FORMAT.GPX;
            _exportCSV.Tag = EXPORT_FORMAT.CSV;
            _exportKML.Tag = EXPORT_FORMAT.KML;

            // とりあえず作業フォルダを固定する（あとで設定できるよにしようとは思う）
            _workPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string locations = System.IO.Path.Combine(_workPath, "BayakiSummary.dat");
            if (File.Exists(locations))
            {
                try
                {
                    using (var stream = new FileStream(locations, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        _locations = bf.Deserialize(stream) as List<TrackItemSummary>;
                    }
                }
                catch
                {
                    // 読み出せないよ？
                    File.Delete(locations);
                }
            }
            if( null == _locations)
            {
                _locations = new List<TrackItemSummary>();
            }
            _images = new List<JPEGFileItem>();
            _pluginPath = new List<string>();

            // ツールバーにGPS情報取得プラグインを追加する
            AddToolbar(new GPXLoaderv1());
            string plugins = System.IO.Path.Combine(_workPath, "BayakiPlugins.dat");
            if (File.Exists(plugins))
            {
                try
                {
                    List<string> loadWork = null;
                    using (var stream = new FileStream(plugins, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        loadWork = bf.Deserialize(stream) as List<string>;
                    }
                    if(null != loadWork)
                    {
                        foreach( string filePath in loadWork)
                        {
                            LoadPlugin(filePath);
                        }
                    }
                }
                catch
                {
                    // 読み出せないよ？
                    File.Delete(locations);
                }
            }
        }

        private void SerializePluginList()
        {
            // 新しいリストが追加されたので、保存します。
            string plugins = System.IO.Path.Combine(_workPath, "BayakiPlugins.dat");

            if (0 < _pluginPath.Count)
            {
                using (var stream = new FileStream(plugins, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, _pluginPath);
                }
            }
            else
            {
                // 保存すべきものがない場合削除します
                File.Delete(plugins);
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();
        }

        private void Initialize()
        {
            TrackItemSummary.SavePath = _workPath;

            _previewMap.DocumentText = Properties.Resources.googlemapsHTML;

            UpdateLocationList();
        }

        private void AddToolbar( bykIFv1.PlugInInterface plugin)
        {
            ToolStripItem item = new ToolStripButton(plugin.Icon);
            item.ToolTipText = plugin.Name;
            item.Tag = plugin;
            item.Click += Item_Click;

            _locationToolbar.Items.Add(item);
        }

        private void UpdateLocationList()
        {
            try
            {
                _locationSources.BeginUpdate();
                _locationSources.Items.Clear();

                foreach( TrackItemSummary track in _locations)
                {
                    ListViewItem viewItem = track.GetListViewItem();
                    _locationSources.Items.Add(viewItem);
                }
            }
            finally
            {
                _locationSources.EndUpdate();
            }

        }

        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (null == item) return;

            bykIFv1.PlugInInterface plugin = item.Tag as bykIFv1.PlugInInterface;
            bykIFv1.TrackItem[] results = null;
            try
            {
                results = plugin.GetTrackItems(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("plugin[{0}]でエラーが発生しました。\n\n詳細\n{1}", plugin.Name, ex.Message), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (null == results) return;

            bool blNewTracks = false;
            foreach (bykIFv1.TrackItem track in results)
            {
                if (null == track) continue;
                if (1 > track.Items.Count) continue;

                System.Diagnostics.Debug.Print(string.Format("create:{1} name:{0}", track.Name, track.CreateTime));

                TrackItemSummary summary = new TrackItemSummary(track);
                _locations.Add(summary);
                blNewTracks = true;
            }

            if (blNewTracks)
            {
                SerializeLocationList();
                UpdateLocationList();

                // 読み込んでいるイメージに対して再度位置情報のマッチングを実施する
                ReMatching();
            }
        }

        private void ReMatching()
        {
            foreach(ListViewItem item in _targets.Items)
            {
                JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                if (null == jpegItem) continue;
                if (null != jpegItem.NewLocation) continue;

                jpegItem.NewLocation = FindPoint(jpegItem.DateTimeOriginal);
                if( null != jpegItem.NewLocation)
                {
                    _update.Enabled = true;
                    item.Checked = true;
                }
            }
        }

        private void SerializeLocationList()
        {
            // 新しいリストが追加されたので、保存します。
            string locations = System.IO.Path.Combine(_workPath, "BayakiSummary.dat");

            if (0 < _locations.Count)
            {
                using (var stream = new FileStream(locations, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, _locations);
                }
            }
            else
            {
                // 保存すべきものがない場合削除します
                File.Delete(locations);
            }

        }

        private void _dropCover_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    string ext = System.IO.Path.GetExtension(d);
                    if( 0 == string.Compare( ext, ".jpg", true) || 0 == string.Compare(ext, ".jpeg", true))
                    {
                        if( System.IO.File.Exists(d))
                        {
                            e.Effect = DragDropEffects.Copy;
                            return;
                        }
                    }
                }
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

        private bykIFv1.Point FindPoint(DateTime dt)
        {
            foreach(TrackItemSummary summary in _locations)
            {
                if(summary.IsContein(dt))
                {
                    return summary.GetPoint(dt);
                }
            }
            return null;
        }

        private void PreviewDropFiles(string[] dropFiles, bool add)
        {
            try
            {
                _targets.BeginUpdate();
                if (!add)
                {
                    _targets.Clear();
                    _targets.LargeImageList.Images.Clear();
                }
                _targets.LargeImageList.TransparentColor = Color.Transparent;


                DateTime debugTime = DateTime.Now;
                TimeSpan debugSpan;
                bool match = false;
                foreach (string dropFile in dropFiles)
                {
                    // すでに登録されているものを二重登録しない
                    match = false;
                    foreach( ListViewItem item in _targets.Items)
                    {
                        JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                        if (null == jpegItem) continue;

                        if ( dropFile == jpegItem.FilePath)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match) continue;

                    using (Image bmp = Bitmap.FromFile(dropFile))
                    {
                        if (null != bmp)
                        {
                            debugTime = DateTime.Now;
                            int index = _targets.LargeImageList.Images.Count;
                            _targets.LargeImageList.Images.Add(stretchImage(bmp, _targets.LargeImageList.ImageSize, _targets.LargeImageList.TransparentColor));
                            string fileName = System.IO.Path.GetFileName(dropFile);
                            debugSpan = DateTime.Now - debugTime;
                            System.Diagnostics.Debug.Print(string.Format("stretchImage:{0}", debugSpan.TotalMilliseconds));

                            debugTime = DateTime.Now;
                            JPEGFileItem jpegItem = new JPEGFileItem(dropFile);
                            debugSpan = DateTime.Now - debugTime;
                            System.Diagnostics.Debug.Print(string.Format("JPEGFileItem:{0}", debugSpan.TotalMilliseconds));

                            debugTime = DateTime.Now;
                            jpegItem.NewLocation = FindPoint(jpegItem.DateTimeOriginal);
                            debugSpan = DateTime.Now - debugTime;
                            System.Diagnostics.Debug.Print(string.Format("FindPoint:{0}", debugSpan.TotalMilliseconds));

                            ListViewItem item = new ListViewItem(fileName);
                            item.ImageIndex = index;
                            item.Tag = jpegItem;

                            if (null != jpegItem.NewLocation)
                            {
                                _update.Enabled = true;
                                item.Checked = true;
                            }
                            else
                            {
                                item.Checked = false;
                            }

                            _targets.Items.Add(item);
                        }
                    }
                }
            }
            finally
            {
                _targets.EndUpdate();
            }
        }

        private void _targets_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                PreviewDropFiles(dropFiles, true);
            }
        }

        private void _dropCover_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                PreviewDropFiles(dropFiles, false);

                _dropCover.Visible = false;
            }
        }

        private void _targets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (null == lv) return;

            if (0 >= lv.SelectedItems.Count) return;

            ListViewItem item = lv.SelectedItems[0];
            JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
            if (null == jpegItem) return;

            //Image bmp = Bitmap.FromFile(jpegItem.FilePath);
            Image bmp = null;
            using (FileStream fs = new FileStream(jpegItem.FilePath, FileMode.Open, FileAccess.Read))
            {
                bmp = Bitmap.FromStream(fs);
                fs.Close();
            }

            _previewImage.Image = bmp;
            switch (jpegItem.Orientation)
            {
                case Orientation.Orientation_90:
                    _previewImage.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case Orientation.Orientation_180:
                    _previewImage.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case Orientation.Orientation_270:
                    _previewImage.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            if (null != jpegItem.CurrentLocation)
            {
                _previewMap.Url = new Uri(string.Format("javascript:movePos({0},{1});", jpegItem.CurrentLocation.Longitude, jpegItem.CurrentLocation.Latitude));
                _previewMap.Visible = true;
            }
            else
            {
                _previewMap.Url = new Uri("javascript:resetMaker();");
                _previewMap.Visible = false;
            }
        }

        private void _deleteTrackItem_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            menuItem.Enabled = (0 < _locationSources.SelectedItems.Count);
        }

        private void _renameTarckItem_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            menuItem.Enabled = (0 < _locationSources.SelectedItems.Count);
        }

        private void _upPriority_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            menuItem.Enabled = (1 == _locationSources.SelectedIndices.Count && 1 < _locationSources.SelectedIndices[0]);
        }

        private void _downPriority_Paint(object sender, PaintEventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            menuItem.Enabled = (1 == _locationSources.SelectedIndices.Count && (_locationSources.Items.Count - 1) > _locationSources.SelectedIndices[0]);
        }

        private void _deleteTrackItem_Click(object sender, EventArgs e)
        {
            if (0 >= _locationSources.SelectedItems.Count) return;
            if (DialogResult.OK != MessageBox.Show(Properties.Resources.MSG1, this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) return;

            foreach (ListViewItem item in _locationSources.SelectedItems)
            {
                TrackItemSummary summary = item.Tag as TrackItemSummary;
                if (null == summary) continue;

                _locationSources.Items.Remove(item);
                _locations.Remove(summary);
                summary.Remove();
            }
            SerializeLocationList();
        }

        private void _upPriority_Click(object sender, EventArgs e)
        {
            if (1 != _locationSources.SelectedItems.Count) return;

            ListViewItem lvItem = _locationSources.SelectedItems[0];
            TrackItemSummary summary = lvItem.Tag as TrackItemSummary;

            if (null == summary) return;

            int index = _locations.IndexOf(summary);
            if (0 >= index) return;

            _locations.Remove(summary);
            _locations.Insert(index - 1, summary);
        }

        private void _downPriority_Click(object sender, EventArgs e)
        {
            if (1 != _locationSources.SelectedItems.Count) return;

            ListViewItem lvItem = _locationSources.SelectedItems[0];
            TrackItemSummary summary = lvItem.Tag as TrackItemSummary;

            if (null == summary) return;

            int index = _locations.IndexOf(summary);
            if (_locations.Count >= index) return;

            _locations.Remove(summary);
            _locations.Insert(index + 1, summary);
        }

        private void SetPropertyValue(Image bmp, int ID, short Type, byte[] value)
        {
            foreach(PropertyItem item in bmp.PropertyItems)
            {
                if( item.Id == ID)
                {
                    item.Type = Type;
                    item.Value = value;
                    item.Len = value.Length;
                    bmp.SetPropertyItem(item);
                    return;
                }
            }
            System.Drawing.Imaging.PropertyItem p1 = bmp.PropertyItems[0];
            // 緯度
            p1.Id = ID;
            p1.Type = Type;
            p1.Value = value;
            p1.Len = value.Length;
            bmp.SetPropertyItem(p1);
        }

        private void _update_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem lvItem in _targets.Items)
            {
                // チェックされていないのは保存対象外
                if (!lvItem.Checked) continue;

                JPEGFileItem item = lvItem.Tag as JPEGFileItem;
                if (null == item) continue;

                Image bmp = null;
                FileStream fs = new FileStream(item.FilePath, FileMode.Open, FileAccess.Read, FileShare.Write);
                bmp = Bitmap.FromStream(fs);

                if (null == bmp) continue;

                // 書き込みデータに変換する
                PointConvertor converter = new PointConvertor(item.NewLocation);

                // 緯度
                SetPropertyValue(bmp, 1, 2, converter.LatitudeMark);
                SetPropertyValue(bmp, 2, 5, converter.Latitude);

                // 経度
                SetPropertyValue(bmp, 3, 2, converter.LongitudeMark);
                SetPropertyValue(bmp, 4, 5, converter.Longtude);

                // 高度基準
                SetPropertyValue(bmp, 5, 7, converter.AltitudeRef);

                // 高度
                SetPropertyValue(bmp, 6, 5, converter.Altitude);

                // 一時ファイルに保存します
                string workPath = Path.GetTempFileName();

                // 保存する
                bmp.Save(workPath);

                // 保存するためにクローズする
                bmp.Dispose();
                fs.Close();

                // 元ファイルを削除します。
                File.Delete(item.FilePath);

                // 一時ファイルを移動します。
                File.Move(workPath, item.FilePath);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "プラグイン選択";
            of.DefaultExt = ".dll";
            of.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            of.Filter = "byk plugin(*.dll)|*.dll|all(*.*)|*.*";
            of.Multiselect = false;
            if (DialogResult.OK == of.ShowDialog(this))
            {
                string filePath = of.FileName;

                LoadPlugin( filePath);
            }
        }

        private void LoadPlugin( string filePath)
        {
            bykIFv1.PlugInInterface plugin;
            Assembly asm = Assembly.LoadFrom(filePath);
            AssemblyTitleAttribute asmttl = asm.GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single() as AssemblyTitleAttribute;

            bool findPlugin = false;
            foreach (Type tp in asm.GetTypes())
            {
                try
                {
                    if (tp.IsInterface) continue;

                    plugin = Activator.CreateInstance(tp) as bykIFv1.PlugInInterface;
                    if (null == plugin) continue;

                    findPlugin = true;

                    AddToolbar(plugin);
                }
                catch
                {
                    continue;
                }
            }

            if (findPlugin)
            {
                _pluginPath.Add(filePath);
                SerializePluginList();
            }
        }

        private void _locationToolbarContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ToolStripMenuItem item = null;

            removeToolStripMenuItem.DropDownItems.Clear();
            foreach (string filePath in _pluginPath)
            {
                Assembly asm = Assembly.LoadFrom(filePath);
                AssemblyTitleAttribute asmttl = asm.GetCustomAttributes(typeof(AssemblyTitleAttribute), true).Single() as AssemblyTitleAttribute;

                item = new ToolStripMenuItem(asmttl.Title);
                item.Click += RemovePlugin_Click;
                item.Tag = filePath;

                removeToolStripMenuItem.DropDownItems.Add(item);
            }
            removeToolStripMenuItem.Enabled = (0 < removeToolStripMenuItem.DropDownItems.Count);
        }

        private void RemovePlugin_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (null == item) return;

            string targetPath = item.Tag as string;
            if (null == targetPath) return;

            List<ToolStripItem> delTarget = new List<ToolStripItem>();

            bykIFv1.PlugInInterface plugin;
            foreach (ToolStripItem toolBarItem in _locationToolbar.Items)
            {
                plugin = toolBarItem.Tag as bykIFv1.PlugInInterface;
                if (null == plugin) return;

                Type tp = plugin.GetType();
                if (tp.Assembly.Location == targetPath)
                {
                    delTarget.Add(toolBarItem);
                }
            }
            foreach (ToolStripItem toolBarItem in delTarget)
            {
                _locationToolbar.Items.Remove(toolBarItem);
            }
            _pluginPath.Remove(targetPath);
            SerializePluginList();
        }

        delegate void ExportMethod(string filePath, bykIFv1.TrackItem trackItem);

        private void ExportGPX( string filePath, bykIFv1.TrackItem trackItem)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter xw = XmlWriter.Create(filePath, settings))
            {
                xw.WriteStartElement("gpx");
                {
                    xw.WriteAttributeString("version", "1.0");
                    xw.WriteAttributeString("creator", "Bayaki");

                    xw.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    xw.WriteAttributeString("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd");

                    // 作成時間
                    xw.WriteElementString("time", trackItem.CreateTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    xw.WriteStartElement("bounds");
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
                        xw.WriteAttributeString("minlat", minlat.ToString());
                        xw.WriteAttributeString("minlon", minlon.ToString());
                        xw.WriteAttributeString("maxlat", maxlat.ToString());
                        xw.WriteAttributeString("maxlon", maxlon.ToString());
                    }
                    xw.WriteEndElement();

                    // waypointを出力します
                    int index = 0;
                    foreach (bykIFv1.Point point in trackItem.Items)
                    {
                        ++index;
                        if ( point.Interest)
                        {
                            xw.WriteStartElement("wpt");
                            {
                                xw.WriteAttributeString("lat", point.Latitude.ToString());
                                xw.WriteAttributeString("lon", point.Longitude.ToString());

                                xw.WriteElementString("ele", point.Elevation.ToString());
                                xw.WriteElementString("time", point.Time.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                                xw.WriteElementString("speed", point.Speed.ToString());
                                xw.WriteElementString("name", string.Format("PT{0:D4}", index));
                            }
                            xw.WriteEndElement();
                        }
                    }

                    xw.WriteStartElement("trk");
                    {
                        xw.WriteStartElement("trkseg");
                        {
                            index = 0;
                            foreach (bykIFv1.Point point in trackItem.Items)
                            {
                                xw.WriteStartElement("trkpt");
                                {
                                    xw.WriteAttributeString("lat", point.Latitude.ToString());
                                    xw.WriteAttributeString("lon", point.Longitude.ToString());

                                    xw.WriteElementString("ele", point.Elevation.ToString());
                                    xw.WriteElementString("time", point.Time.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                                    xw.WriteElementString("speed", point.Speed.ToString());
                                    xw.WriteElementString("name", string.Format("PT{0:D4}", ++index));
                                }
                                xw.WriteEndElement();
                            }
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
        }

        private void _export_Click(object sender, EventArgs e)
        {
            if( 1 != _locationSources.SelectedItems.Count)return;
            ListViewItem item = _locationSources.SelectedItems[0];
            TrackItemSummary trackItemSummary = item.Tag as TrackItemSummary;
            if (null == trackItemSummary) return;

            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            if (null == menu) return;

            ExportMethod exportMethod = null;
            EXPORT_FORMAT format = (EXPORT_FORMAT)menu.Tag;
            switch(format)
            {
                case EXPORT_FORMAT.GPX:
                    _exportFileDialog.DefaultExt = ".gpx";
                    _exportFileDialog.Filter = "GPX Files|*.gpx|ALL Files|*.*";
                    _exportFileDialog.FileName = trackItemSummary.Name;
                    exportMethod = new ExportMethod(ExportGPX);
                    break;
                default:
                    return;
            }

            if( DialogResult.OK == _exportFileDialog.ShowDialog(this))
            {
                bykIFv1.TrackItem trackItem = trackItemSummary.TrackItem;
                if (null != exportMethod)
                    exportMethod(_exportFileDialog.FileName, trackItem);
            }
        }
    }
}
