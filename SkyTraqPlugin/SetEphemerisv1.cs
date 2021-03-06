﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bykIFv1;

namespace SkyTraqPlugin
{
    public class SetEphemerisv1 : bykIFv1.PlugInInterface
    {
        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.SetEphemeris_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.SetEphemeris_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            SetEphemerisForm form = new SetEphemerisForm();

            form.ShowDialog(owner);

            return new TrackItem[] { };
        }
    }
}
