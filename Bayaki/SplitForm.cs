using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bayaki
{
    public partial class SplitForm : Form
    {
        bykIFv1.TrackItem _track;
        List<bykIFv1.TrackItem> _items;

        public SplitForm(bykIFv1.TrackItem track)
        {
            InitializeComponent();

            _track = track;
            _items = new List<bykIFv1.TrackItem>();
        }

        public IList<bykIFv1.TrackItem> Items
        {
            get
            {
                return _items;
            }
        }


        private void byDateTime_CheckedChanged(object sender, EventArgs e)
        {
            if (byDateTime.Checked)
            {
                timeValue.Enabled = true;
                distanceValue.Enabled = false;
                preview.Enabled = true;
            }
        }

        private void byDistance_CheckedChanged(object sender, EventArgs e)
        {
            if (byDistance.Checked)
            {
                distanceValue.Enabled = true;
                timeValue.Enabled = false;
                preview.Enabled = true;
            }
        }

        private void preview_Click(object sender, EventArgs e)
        {
            _items.Clear();
            if (byDateTime.Checked)
            {
                int splitCount = 0;
                bykIFv1.TrackItem newItem = null;

                DateTime prevValue = DateTime.MinValue;
                TimeSpan ts;
                foreach (bykIFv1.Point p in _track.Items)
                {
                    ts = p.Time - prevValue;
                    if ((double)timeValue.Value <= ts.TotalHours)
                    {
                        ++splitCount;
                        // 名前は元のデータを利用する
                        newItem = new bykIFv1.TrackItem(string.Format("{0}({1})", _track.Name, splitCount), _track.CreateTime);
                        // 情報があれば利用する
                        if (0 < _track.Description.Length)
                        {
                            newItem.Description = string.Format("{0}({1})", _track.Description, splitCount);
                        }
                        _items.Add(newItem);
                    }
                    newItem.Items.Add(p);
                    prevValue = p.Time;
                }
            }
            else if(byDistance.Checked)
            {
                int splitCount = 0;
                bykIFv1.TrackItem newItem = null;

                // KMをMにしますよ
                double distanceValueMeter = (double)distanceValue.Value * 1000;

                bykIFv1.Point from = null;
                double distance = 0;
                foreach (bykIFv1.Point p in _track.Items)
                {
                    distance = (null == from) ? distanceValueMeter : PointDistance.Distance(from, p);
                    if (distanceValueMeter <= distance)
                    {
                        ++splitCount;
                        // 名前は元のデータを利用する
                        newItem = new bykIFv1.TrackItem(string.Format("{0}({1})", _track.Name, splitCount), _track.CreateTime);
                        // 情報があれば利用する
                        if (0 < _track.Description.Length)
                        {
                            newItem.Description = string.Format("{0}({1})", _track.Description, splitCount);
                        }
                        _items.Add(newItem);
                    }
                    newItem.Items.Add(p);
                    from = p;
                }
            }

            _splitItems.BeginUpdate();
            try
            {
                _splitItems.Items.Clear();
                foreach (var v in _items)
                {
                    ListViewItem item = new ListViewItem(v.Name);
                    DateTime dtFrom = v.Items[0].Time;
                    TimeSpan span = System.TimeZoneInfo.Local.GetUtcOffset(dtFrom);
                    dtFrom = dtFrom.Add(span);
                    item.SubItems.Add(dtFrom.ToString());

                    DateTime dtTo = v.Items[v.Items.Count - 1].Time;
                    span = System.TimeZoneInfo.Local.GetUtcOffset(dtTo);
                    dtTo = dtTo.Add(span);
                    item.SubItems.Add(dtTo.ToString());
                    item.SubItems.Add(v.Items.Count.ToString());
                    item.Tag = v;
                    _splitItems.Items.Add(item);
                }
                _splitItems.Enabled = true;
                _OK.Enabled = true;
            }
            finally
            {
                _splitItems.EndUpdate();
            }
        }

        private void _splitItems_DoubleClick(object sender, EventArgs e)
        {
            if (1 != _splitItems.SelectedItems.Count) return;
            bykIFv1.TrackItem trackItem = _splitItems.SelectedItems[0].Tag as bykIFv1.TrackItem;
            if (null == trackItem) return;

            TrackPointPreviewForm tpf = new TrackPointPreviewForm(trackItem);

            tpf.Show(this);
        }

        private void _OK_Click(object sender, EventArgs e)
        {
            if (1 >= _items.Count)
            {
                // 分割されなかった。
                DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SplitForm_Load(object sender, EventArgs e)
        {
            byDateTime.Checked = true;
        }
    }
}
