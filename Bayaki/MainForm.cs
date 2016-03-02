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

namespace Bayaki
{
    public partial class MainForm : Form
    {
        private string _workPath;
        private List<TrackItemSummary> _locations;
        private List<JPEGFileItem> _images;

        public MainForm()
        {

            InitializeComponent();

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
            else
            {
                _locations = new List<TrackItemSummary>();
            }
            _images = new List<JPEGFileItem>();
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

            AddToolbar(new GPXLoaderv1());

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
                bykIFv1.PlugInInterface plugin;
                Assembly asm = Assembly.LoadFrom(of.FileName);
                foreach (var t in asm.GetTypes())
                {
                    try
                    {
                        if (t.IsInterface) continue;

                        plugin = Activator.CreateInstance(t) as bykIFv1.PlugInInterface;
                        if (null == plugin) continue;

                        ToolStripItem item = new ToolStripButton(plugin.Icon);
                        item.ToolTipText = plugin.Name;
                        item.Tag = plugin;
                        item.Click += Item_Click;

                        _locationToolbar.Items.Add(item);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
    }
}
