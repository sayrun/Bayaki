﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
        private WebBrowserObserver _observer;

        private Color TRANS_COLOR = Color.White;

        private const int MAX_TIMEDIFF = (60 * 20); // 根拠ないけど、20分以上差異があれば採用しない

        private const string DIRECTORY_DATAFOLDER = "Bayaki Folder";

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

            // Exifの元ネタ作成用画像をセットする
            UpdateJpegFile.SetHeaderSeed(Properties.Resources.HeaderSeed);

            // データ保存用フォルダを作成
            _workPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DIRECTORY_DATAFOLDER);

            _locations = null;
            _images = new List<JPEGFileItem>();
            _pluginPath = new List<string>();

            // ツールバーにGPS情報取得プラグインを追加する
            AddToolbar(new GPXLoaderv1());
            AddToolbar(new KMLLoaderv1());
 
            // 地図情報をJavascriptからもらう        
            _observer = new WebBrowserObserver();
            _observer.OnMakerDrag += _observer_OnMakerDrag;
            _previewMap.ObjectForScripting = _observer;
        }

        private void _observer_OnMakerDrag(double lat, double lon)
        {
            if (1 != _targets.SelectedItems.Count) return;

            ListViewItem item = _targets.SelectedItems[0];
            JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
            if (null == jpegItem) return;

            System.Diagnostics.Debug.Print("new pos {0},{1}", lat, lon);

            bykIFv1.Point point = jpegItem.NewLocation;
            bykIFv1.Point pnew;
            if( null != point)
            {
                pnew = new bykIFv1.Point(point.Time, lat, lon, double.NaN, 0, false);
            }
            else
            {
                pnew = new bykIFv1.Point(jpegItem.DateTimeOriginal, lat, lon, double.NaN, 0, false);
            }
            jpegItem.NewLocation = pnew;
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
#if _MAP_YAHOO
            _previewMap.DocumentText = Properties.Resources.yahoomapsHTML;
#else
#if _MAP_GOOGLE
            _previewMap.DocumentText = Properties.Resources.googlemapsHTML;
#endif
#endif

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
                LocationMatching();
            }
        }

        private void LocationMatching()
        {
            foreach(ListViewItem item in _targets.Items)
            {
                JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                if (null == jpegItem) continue;

                jpegItem.NewLocation = FindPoint(jpegItem.DateTimeOriginal);
                if( null != jpegItem.NewLocation)
                {
                    item.Checked = true;

                    if (jpegItem.NewLocation.Time != jpegItem.DateTimeOriginal.ToUniversalTime())
                    {
                        item.ForeColor = Color.Pink;
                    }
                    else
                    {
                        item.ForeColor = Color.Black;
                    }
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

        private bykIFv1.Point FindPoint(DateTime dt)
        {

            List<bykIFv1.Point> points = new List<bykIFv1.Point>();
            // 総当たりで位置情報を取得
            foreach (TrackItemSummary summary in _locations)
            {
                if (summary.IsContein(dt))
                {
                    points.Add(summary.GetPoint(dt));
                }
            }

            if( 1 == points.Count)
            {
                return points[0];
            }
            else
            {
                // 比較用にUTCにする
                DateTime utcdt = dt.Subtract(System.TimeZoneInfo.Local.GetUtcOffset(dt));
                // 複数の結果が得られたら、より時間の小さいほう
                bykIFv1.Point result = null;
                double diff = 0;
                double diffOld = double.MaxValue;
                foreach(bykIFv1.Point pnt in points)
                {
                    TimeSpan s = pnt.Time - utcdt;
                    diff = Math.Abs(s.TotalSeconds);
                    if (diff >= MAX_TIMEDIFF) continue;
                    if( diffOld > diff)
                    {
                        result = pnt;
                        diffOld = diff;
                    }
                }
                return result;
            }
        }

        private void LoadDropFiles(string[] dropFiles, bool add)
        {
            try
            {
                _targets.BeginUpdate();
                if (!add)
                {
                    _targets.Clear();
                    _targets.LargeImageList.Images.Clear();
                }
                _targets.LargeImageList.TransparentColor = TRANS_COLOR;

                List<string> targetFiles = new List<string>();
                foreach(string filePath in dropFiles)
                {
                    // すでに登録されているものを二重登録しない
                    bool match = false;
                    foreach (ListViewItem item in _targets.Items)
                    {
                        JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                        if (null == jpegItem) continue;

                        if (filePath == jpegItem.FilePath)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match) continue;

                    targetFiles.Add(filePath);
                }

                LoadJpegFile ljf = new LoadJpegFile(_targets.LargeImageList.ImageSize, TRANS_COLOR);
                NowProcessingForm<string> npf = new NowProcessingForm<string>(ljf, targetFiles.ToArray());

                if( DialogResult.OK == npf.ShowDialog(this))
                {
                    foreach( ListViewItem item in ljf.Items)
                    {
                        JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                        if (null != jpegItem)
                        {
                            jpegItem.NewLocation = FindPoint(jpegItem.DateTimeOriginal);
                            if (null != jpegItem.NewLocation)
                            {
                                item.Checked = true;

                                if(jpegItem.NewLocation.Time != jpegItem.DateTimeOriginal.ToUniversalTime())
                                {
                                    item.ForeColor = Color.Pink;
                                }
                                else
                                {
                                    item.ForeColor = Color.Black;
                                }
                            }
                            else
                            {
                                item.Checked = false;

                                if( null != jpegItem.CurrentLocation)
                                {
                                    item.ForeColor = Color.Blue;
                                }
                            }

                            item.ImageIndex = _targets.LargeImageList.Images.Count;
                            _targets.LargeImageList.Images.Add(jpegItem.ThumNail);

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

                LoadDropFiles(dropFiles, true);
            }
        }

        private void _dropCover_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                LoadDropFiles(dropFiles, false);

                _dropCover.Visible = false;
            }
        }

        private void _targets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = sender as ListView;
            if (null == lv) return;

            if (0 >= lv.SelectedItems.Count)
            {
                _previewImage.Image = null;
                _previewMap.Document.InvokeScript("resetMarker");
                return;
            }

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

            if (null != jpegItem.NewLocation)
            {
                _previewMap.Document.InvokeScript("movePos", new object[] { jpegItem.NewLocation.Latitude, jpegItem.NewLocation.Longitude });
            }
            else
            {
                if (null != jpegItem.CurrentLocation)
                {
                    _previewMap.Document.InvokeScript("movePos", new object[] { jpegItem.CurrentLocation.Latitude, jpegItem.CurrentLocation.Longitude });
                }
                else
                {
                    _previewMap.Document.InvokeScript("resetMarker");
                }
            }
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

        private void _update_Click(object sender, EventArgs e)
        {
            List<JPEGFileItem> items = new List<JPEGFileItem>();
            foreach(ListViewItem lvItem in _targets.Items)
            {
                // チェックされていないのは保存対象外
                if (!lvItem.Checked) continue;

                JPEGFileItem item = lvItem.Tag as JPEGFileItem;
                if (null == item) continue;

                items.Add(item);
            }

            NowProcessingForm<JPEGFileItem> npf = new NowProcessingForm<JPEGFileItem>(new UpdateJpegFile(), items.ToArray());
            if( DialogResult.OK == npf.ShowDialog(this))
            {
                MessageBox.Show("更新しました", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            using (TrackItemWriter tiw = new TrackItemWriter(filePath))
            {
                tiw.Write(trackItem);
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

        private void _renameTarckItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = _locationSources.SelectedItems[0];
            item.BeginEdit();
        }

        private void _locationSources_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListView lv = sender as ListView;
            if( null == lv)
            {
                e.CancelEdit = true;
                return;
            }

            if (e.Label != null)
            {
                if( 0 == e.Label.Length)
                {
                    e.CancelEdit = true;
                    return;
                }

                int index = e.Item;

                ListViewItem item = lv.Items[index];
                TrackItemSummary trackItem = item.Tag as TrackItemSummary;
                if( null == trackItem)
                {
                    e.CancelEdit = true;
                    return;
                }
                trackItem.Name = e.Label;
                SerializeLocationList();

            }
        }

        private void _routePreview_Click(object sender, EventArgs e)
        {
            if (1 != _locationSources.SelectedItems.Count) return;
            ListViewItem lvItem = _locationSources.SelectedItems[0];
            TrackItemSummary trackSummary = lvItem.Tag as TrackItemSummary;
            if (null == trackSummary) return;

            bykIFv1.TrackItem trackItem = trackSummary.TrackItem;

            trackItem.Name = trackSummary.Name;
            TrackPointPreviewForm tpf = new TrackPointPreviewForm(trackItem);

            tpf.Show(this);
        }

        private void _previewMap_SizeChanged(object sender, EventArgs e)
        {
            _previewMap.Document.InvokeScript("resizeMap");
        }

        private void _previewMap_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // 初期化してあげます
            _previewMap.Document.InvokeScript("Initialize");
        }

        private void _previewImageContextMenu_Opening(object sender, CancelEventArgs e)
        {
            bool removeLocation = false;
            bool addLocation = false;
            if( 1 == _targets.SelectedItems.Count)
            {
                ListViewItem item = _targets.SelectedItems[0];
                JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                if( null != jpegItem)
                {
                    if( null != jpegItem.NewLocation || null != jpegItem.CurrentLocation)
                    {
                        removeLocation = true;
                    }
                    else
                    {
                        addLocation = true;
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
            _removeLocationToolStripMenuItem.Enabled = removeLocation;
            _addLocationToolStripMenuItem.Enabled = addLocation;
        }

        private void _removeLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if( DialogResult.Yes == MessageBox.Show("位置情報を削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (1 != _targets.SelectedItems.Count) return;

                ListViewItem item = _targets.SelectedItems[0];
                JPEGFileItem jpegItem = item.Tag as JPEGFileItem;
                if (null == jpegItem) return;

                jpegItem.RemoveLocation();

                _previewMap.Document.InvokeScript("resetMarker");
            }
        }

        private void _addLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _previewMap.Document.InvokeScript("dropMarker");
        }

        private void _locationContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if( 0 >= _locationSources.SelectedItems.Count)
            {
                e.Cancel = true;
            }

            _deleteTrackItem.Enabled = (0 < _locationSources.SelectedItems.Count);
            _renameTarckItem.Enabled = (0 < _locationSources.SelectedItems.Count);
            _exportTrackItem.Enabled = (0 < _locationSources.SelectedItems.Count);
            _routePreview.Enabled = (0 < _locationSources.SelectedItems.Count);
            _upPriority.Enabled = (1 == _locationSources.SelectedIndices.Count && 0 < _locationSources.SelectedIndices[0]);
            _downPriority.Enabled = (1 == _locationSources.SelectedIndices.Count && (_locationSources.Items.Count - 1) > _locationSources.SelectedIndices[0]);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 保存先フォルダがないなら作る
            if( !System.IO.Directory.Exists(_workPath))
            {
                System.IO.Directory.CreateDirectory(_workPath);
            }

            // 場所情報のサマリーを取得する
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
            if (null == _locations)
            {
                _locations = new List<TrackItemSummary>();
            }

            // プラグインの情報を読み出し
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
                    if (null != loadWork)
                    {
                        foreach (string filePath in loadWork)
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

        private void _targets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            _update.Enabled = (0 < _targets.CheckedItems.Count);
        }
    }
}
