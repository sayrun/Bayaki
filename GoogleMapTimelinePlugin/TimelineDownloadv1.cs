using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bykIFv1;

namespace GoogleMapTimelinePlugin
{
    public class TimelineDownloadv1 : bykIFv1.PlugInInterface
    {
        public System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.TimelineDownload_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.TimelineDownload_NAME;
            }
        }

        public TrackItem[] GetTrackItems(System.Windows.Forms.IWin32Window owner)
        {
            DownloadForm dlg = new DownloadForm();

            dlg.ShowDialog(owner);

            return new TrackItem[] { };
        }
    }
}
