using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bykIFv1;

namespace GPSBabelPlugin
{
    public class GPSBabelLoaderv1 : bykIFv1.PlugInInterface
    {
        public System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.GPSBabelLoader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.GPSBabelLoader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(System.Windows.Forms.IWin32Window owner)
        {
            ExecCommandForm dlg = new ExecCommandForm();

            if(System.Windows.Forms.DialogResult.OK == dlg.ShowDialog(owner))
            {
                if( null != dlg.Item)
                {
                    return new TrackItem[] { dlg.Item };
                }
            }
            // 結果なしを返す
            return new TrackItem[] { };
        }
    }
}
