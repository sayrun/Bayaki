using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bykIFv1;

namespace SkyTraqPlugin
{
    public class Configurev1 : bykIFv1.PlugInInterface
    {
        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.Configure_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.Configure_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            ConfiurationForm cf = new ConfiurationForm();

            cf.ShowDialog(owner);

            return new TrackItem[] { };
        }
    }
}
