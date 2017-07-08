using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphControlLibrary
{
    public partial class GraphControl: UserControl
    {
        private readonly List<GraphSet> _items;
        private Bitmap _bitmap = null;

        public delegate string Data2Text(Single x, Single y);

        public GraphControl()
        {
            InitializeComponent();

            _items = new List<GraphSet>();
        }

        public List<GraphSet> Items
        {
            get
            {
                return _items;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            _bitmap = new Bitmap(base.ClientRectangle.Width, base.ClientRectangle.Height, base.CreateGraphics());
            DrawGraph();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (null != _bitmap)
            {
                e.Graphics.DrawImage(_bitmap, 0, 0);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);


            System.Diagnostics.Debug.Print("X={0}, Y={1}", e.X, e.Y);

            StringBuilder sb = new StringBuilder();
            foreach (GraphSet gset in _items)
            {
                Single xwidth = gset.XScale.Max.Value - gset.XScale.Min.Value;
                Single yheight = gset.YScale.Max.Value - gset.YScale.Min.Value;

                foreach (PointData pd in gset.Items)
                {
                    Single sx = ((_bitmap.Width - 1) * (pd.X - gset.XScale.Min.Value)) / xwidth;
                    if (sx >= e.X)
                    {
                        if (0 < sb.Length)
                        {
                            sb.Append("\n");
                        }
                        sb.Append(string.Format("{0}", gset.YScale.ConvertFrom(pd.Y)));
                        if ( 0 < gset.YScale.Title.Length)
                        {
                            sb.Append(string.Format("({0})", gset.YScale.Title));
                        }

                        sb.Append(string.Format("-{0}", gset.XScale.ConvertFrom(pd.X)));
                        break;
                    }
                }
            }
            string s = sb.ToString();
            //toolTip1.Show(s, this);
        }

        public void BegineUpdate()
        {

        }

        public void EndUpdate()
        {
            DrawGraph();

            this.Invalidate();
        }

        private void DrawGraph()
        {
            Graphics gs = Graphics.FromImage(_bitmap);

            gs.FillRectangle(Brushes.White, 0, 0, _bitmap.Width -1, _bitmap.Height-1);
            gs.DrawRectangle(Pens.DarkGray, 0, 0, _bitmap.Width-1, _bitmap.Height-1);

            foreach (GraphSet gset in _items)
            {
                if (false == gset.DrawScale)
                    continue;

                foreach (ScaleData xs in gset.XScale.Items)
                {
                    if (gset.XScale.Min.Value > xs.Value) throw new Exception("X座標で、最小値よりもメモリが小さい");
                    if (gset.XScale.Max.Value < xs.Value) throw new Exception("X座標で、最大値よりもメモリが大きい");
                }
                foreach (ScaleData ys in gset.YScale.Items)
                {
                    if (gset.YScale.Min.Value > ys.Value) throw new Exception("Y座標で、最小値よりもメモリが小さい");
                    if (gset.YScale.Max.Value < ys.Value) throw new Exception("Y座標で、最大値よりもメモリが大きい");
                }


                // X座標を描画
                if( 0 < gset.XScale.Items.Count)
                {
                    Single xwidth = gset.XScale.Max.Value - gset.XScale.Min.Value;
                    Single widthWork;
                    int xTarget;
                    foreach (ScaleData xs in gset.XScale.Items)
                    {
                        widthWork = base.ClientRectangle.Width;
                        widthWork *= ((xs.Value - gset.XScale.Min.Value) / xwidth);

                        xTarget = (int)widthWork;

                        gs.DrawLine(Pens.DarkGray, xTarget, 0, xTarget, _bitmap.Height);
                        gs.DrawString(xs.Text, SystemFonts.MenuFont, Brushes.DarkGray, new Point(xTarget, 0));
                    }
                }

                // Y軸を描画
                if( 0 < gset.YScale.Items.Count)
                {
                    Single yheight = gset.YScale.Max.Value - gset.YScale.Min.Value;
                    Single heightWork;
                    int yTarget;
                    foreach (ScaleData ys in gset.YScale.Items)
                    {
                        heightWork = base.ClientRectangle.Height    ;
                        heightWork *= ((ys.Value - gset.YScale.Min.Value) / yheight);

                        yTarget = _bitmap.Height - (int)heightWork;

                        gs.DrawLine(Pens.DarkGray, 0, yTarget, _bitmap.Width, yTarget);
                        gs.DrawString(ys.Text, SystemFonts.MenuFont, Brushes.DarkGray, new Point(0, yTarget));
                    }

                }

                //グラフ線を描画:2点以上必要なので
                if(1 < gset.Items.Count)
                {
                    Single xwidth = gset.XScale.Max.Value - gset.XScale.Min.Value;
                    Single yheight = gset.YScale.Max.Value - gset.YScale.Min.Value;
                    List<PointF> points = new List<PointF>();

                    Single sx;
                    Single sy;
                    foreach (PointData pd in gset.Items)
                    {
                        sx = ((_bitmap.Width - 1) * (pd.X - gset.XScale.Min.Value)) / xwidth;
                        sy = _bitmap.Height - (((_bitmap.Height - 1) * (pd.Y - gset.YScale.Min.Value)) / yheight);

                        points.Add(new PointF(sx, sy));
                    }

                    Pen p = new Pen(Color.Red, 2);
                    gs.DrawLines(p, points.ToArray());
                }

            }
        }
    }
}
