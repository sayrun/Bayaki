using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bykIFv1;

namespace Bayaki
{
    class SkytraqDownloaderv1 : bykIFv1.PlugInInterface
    {
        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.SkytraqDownloader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.SkytraqDownloader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            throw new NotImplementedException();
        }
    }
}
