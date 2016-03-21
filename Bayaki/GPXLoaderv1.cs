using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using bykIFv1;

namespace Bayaki
{
    class GPXLoaderv1 : bykIFv1.PlugInInterface, IDisposable
    {
        public void Dispose()
        {
            return;
        }

        public Bitmap Icon
        {
            get
            {
                return Properties.Resources.GPXLoader_ICON;
            }
        }

        public string Name
        {
            get
            {
                return Properties.Resources.GPXLoader_NAME;
            }
        }

        public TrackItem[] GetTrackItems(IWin32Window owner)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "select gpx file";
            dialog.Filter = "gpx file(*.gpx)|*.gpx|all file(*.*)|*.*||";
            dialog.DefaultExt = ".gpx";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = true;

            if (DialogResult.OK == dialog.ShowDialog(owner))
            {
                List<TrackItem> result = new List<TrackItem>();
                foreach (string targetPath in dialog.FileNames)
                {
                    TrackItem item = LoadGpxFile(targetPath);
                    if( null != item)
                    {
                        result.Add(item);
                    }
                }

                if (0 < result.Count)
                {
                    return result.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// GPXファイルの読み込み
        /// </summary>
        /// <param name="filePath">GPXファイルのパス</param>
        /// <returns>PGXから読み込んだ座標情報リスト</returns>
        private static TrackItem LoadGpxFile(string filePath)
        {
            TrackItem result = null;
            using (TrackItemReader tir = new TrackItemReader(filePath))
            {
                result = tir.Read();
            }
            return result;
        }
    }
}
