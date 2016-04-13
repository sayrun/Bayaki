using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bayaki
{
    interface ITrackItemCache
    {
        bykIFv1.TrackItem GetTrackItem(out ITrackItemCache nextStatus);
        string SaveTrackItem(out ITrackItemCache nextStatus, string path);
        void UpdateTrackItem(out ITrackItemCache nextStatus, string path);
    }
}
