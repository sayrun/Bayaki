using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bykIFv1;
using System.Windows.Forms;

namespace SkyTraqPlugin
{
    public class Downloaderv1 : bykIFv1.PlugInInterface
    {
        public Downloaderv1()
        {

        }

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.Downloader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.Downloader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            throw new NotImplementedException("まだ実装していないです");
        }
    }
}
