using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bykIFv1;
using System.Device.Location;

namespace Bayaki
{
    class KMLLoaderv1 : bykIFv1.PlugInInterface
    {
        private const double MAX_SPEED = 340.0; // 音速(m/s)

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.KMLLoader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.KMLLoader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "select KML file";
            dialog.Filter = "kml file(*.kml)|*.kml|all file(*.*)|*.*||";
            dialog.DefaultExt = ".kml";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = true;

            if (DialogResult.OK == dialog.ShowDialog(owner))
            {
                List<TrackItem> result = new List<TrackItem>();
                foreach (string targetPath in dialog.FileNames)
                {
                    using (KMLReader kr = new KMLReader(targetPath, MAX_SPEED))
                    {
                        TrackItem item = kr.Read();
                        if (null != item)
                        {
                            result.Add(item);
                        }
                    }
                }

                if (0 < result.Count)
                {
                    return result.ToArray();
                }
            }
            return null;
        }
    }
}
