using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bayaki
{
    internal class LoadJpegFile : NowProcessingForm<string>.ProcessingStrategy
    {
        List<ListViewItem> _items;
        Size _thumNailSize;
        Color _transColor;

        public LoadJpegFile( Size thumNailSize, Color transColor)
        {
            _items = new List<ListViewItem>();

            _thumNailSize = thumNailSize;
            _transColor = transColor;
        }

        public List<ListViewItem> Items
        {
            get
            {
                return _items;
            }
        }

        public void DoProcess(string filePath)
        {
            DateTime debugTime = DateTime.Now;
            TimeSpan debugSpan;

            debugTime = DateTime.Now;
            JPEGFileItem jpegItem = new JPEGFileItem(filePath, _thumNailSize, _transColor);
            debugSpan = DateTime.Now - debugTime;
            System.Diagnostics.Debug.Print(string.Format("JPEGFileItem:{0}", debugSpan.TotalMilliseconds));

            debugTime = DateTime.Now;
            debugSpan = DateTime.Now - debugTime;
            System.Diagnostics.Debug.Print(string.Format("FindPoint:{0}", debugSpan.TotalMilliseconds));

            ListViewItem item = new ListViewItem(System.IO.Path.GetFileName(filePath));
            item.Tag = jpegItem;

            _items.Add(item);
        }
    }
}
