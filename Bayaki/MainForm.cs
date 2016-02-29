using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

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
            bykIFv1.TrackItem[] results = plugin.GetTrackItems(this);
            if (null == results) return;

            foreach (bykIFv1.TrackItem track in results)
            {
                if (null == track) continue;
                if (1 > track.Items.Count) continue;

                System.Diagnostics.Debug.Print(string.Format("create:{1} name:{0}", track.Name, track.CreateTime));

                TrackItemSummary summary = new TrackItemSummary(track);
                _locations.Add(summary);
            }

            SerializeLocationList();

            UpdateLocationList();
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

        private void _dropCover_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                _targets.BeginUpdate();
                _targets.Clear();
                _targets.LargeImageList.Images.Clear();
                _targets.LargeImageList.TransparentColor = Color.Transparent;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // ドラッグ中のファイルやディレクトリの取得
                    string[] dropFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                    foreach (string dropFile in dropFiles)
                    {
                        using (Image bmp = Bitmap.FromFile(dropFile))
                        {
                            if (null != bmp)
                            {
                                int index = _targets.LargeImageList.Images.Count;
                                _targets.LargeImageList.Images.Add(stretchImage(bmp, _targets.LargeImageList.ImageSize, _targets.LargeImageList.TransparentColor));
                                string fileName = System.IO.Path.GetFileName(dropFile);

                                JPEGFileItem jpegItem = new JPEGFileItem(dropFile);
                                jpegItem.NewLocation = FindPoint(jpegItem.DateTimeOriginal);

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
                _dropCover.Visible = false;
            }
            finally
            {
                _targets.EndUpdate();
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


            /*
            //FileStream オブジェクトを使用し、画像を読み込む
            System.IO.FileStream fs = new System.IO.FileStream(jpegItem.FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            Image LoadImage = Image.FromStream(fs);
            fs.Close();
            */

            Image bmp = Bitmap.FromFile(jpegItem.FilePath);
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

        private void _update_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem lvItem in _targets.Items)
            {
                JPEGFileItem item = lvItem.Tag as JPEGFileItem;
                if (null == item) continue;

                File.Delete(item.FilePath);
                Image bmp = Bitmap.FromFile(item.FilePath);

                /*
                System.IO.FileInfo fi = new FileInfo(item.FilePath);
                byte[] readArea = new byte[fi.Length];

                using (FileStream fs = new FileStream(item.FilePath, FileMode.Open))
                {
                    fs.Read(readArea, 0, (int)fi.Length);
                }

                Bitmap bmp = null;
                using (MemoryStream ms = new MemoryStream(readArea))
                {
                    bmp = new Bitmap(ms);
                }*/

            if (null == bmp) continue;

                PointConvertor converter = new PointConvertor(item.NewLocation);

                System.Drawing.Imaging.PropertyItem p1 = bmp.PropertyItems[0];
                // 緯度
                p1.Id = 1;
                p1.Type = 2;
                p1.Value = converter.LatitudeMark;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                p1.Id = 2;
                p1.Type = 5;
                p1.Value = converter.Latitude;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                // 経度
                p1.Id = 3;
                p1.Type = 2;
                p1.Value = converter.LongitudeMark;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                p1.Id = 4;
                p1.Type = 5;
                p1.Value = converter.Longtude;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                // 高度基準
                p1.Id = 5;
                p1.Type = 7;
                p1.Value = converter.AltitudeRef;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                // 高度
                p1.Id = 6;
                p1.Type = 5;
                p1.Value = converter.Altitude;
                p1.Len = p1.Value.Length;
                bmp.SetPropertyItem(p1);

                string hoge = @"C:\Users\Tomo\Documents\Bluetooth 交換フォルダ\新しいフォルダー\hoge.jpg";
                // 保存する
                bmp.Save(item.FilePath);
            }
        }
    }
}
