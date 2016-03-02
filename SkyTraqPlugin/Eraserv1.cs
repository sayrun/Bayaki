using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bykIFv1;
using System.Windows.Forms;
using System.Drawing;

namespace SkyTraqPlugin
{
    public class Eraserv1 : bykIFv1.PlugInInterface
    {
        public Eraserv1()
        {
        }

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.Eraser_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.Eraser_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            EraseDataForm eraseForm = new EraseDataForm();

            eraseForm.ShowDialog(owner);

            return new TrackItem[] { };
        }
    }
}
