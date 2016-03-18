using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bykIFv1;

namespace Bayaki
{
    class KMLLoaderv1 : bykIFv1.PlugInInterface
    {
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

            }
            return null;
        }
    }
}
