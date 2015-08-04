using GIS.Arc;
using GIS.Map;
using PipeNetManager.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PipeNetManager.utils.Assist
{
    public abstract class BaseAction
    {
        //保存描点的墨卡托坐标,支持缩放
        protected List<Point> mMPoints = new List<Point>();

        //对应物理屏幕坐标，直接参与绘图
        protected List<Point> mSPoint = new List<Point>();

        //保存绘制的路径
        protected List<Path> mPath = new List<Path>();

        private const double EARTH_RADIUS = 6378.137;//地球半径

        protected Canvas mCanvas;
        public BaseAction()
        {
        }

        public void setCanvas(Canvas canvas)
        {
            mCanvas = canvas;
        }

        public static String getType()
        {
            return "BaseAction";
        }

        //根据坐标序列画图
        protected Path createPath(Point sp, Point ep)
        {
            Path path = new Path();
            path.Stroke = ColorCenter.GetInstance().Seleted_Fill_Color;
            path.Data = DrawLine(sp, ep);
            path.StrokeThickness = App.StrokeThinkness;

            return path;
        }

        public void UpdatePath(Path path, Point sp, Point ep)
        {
            using (StreamGeometryContext cnt = ((StreamGeometry)(path.Data)).Open())
            {
                InternalDrawGeometry(cnt, sp, ep);
            }
        }

        protected Geometry DrawLine(List<Point> listp)
        {
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            using (StreamGeometryContext context = geometry.Open())
            {
                InternalDrawGeometrys(context, listp);
            }

            return geometry;
        }

        private void InternalDrawGeometrys(StreamGeometryContext context, List<Point> listp)
        {
            bool first = true;
            Point pre = new Point(0, 0);
            foreach (Point ep in listp)
            {
                Point p1 = new Point(ep.X - Constants.CALACTION_TRACELEN, ep.Y);
                Point p2 = new Point(ep.X, ep.Y - Constants.CALACTION_TRACELEN);
                Point p3 = new Point(ep.X + Constants.CALACTION_TRACELEN, ep.Y);
                Point p4 = new Point(ep.X, ep.Y + Constants.CALACTION_TRACELEN);

                //draw cross
                context.BeginFigure(p1, true, false);
                context.LineTo(p3, true, true);

                context.BeginFigure(p2, true, false);
                context.LineTo(p4, true, true);

                if (first)
                {
                    first = false;
                    pre = ep;
                }
                else
                {
                    context.BeginFigure(pre, true, false);
                    context.LineTo(ep, true, true);
                    pre = ep;
                }
            }
        }

        protected Geometry DrawLine(Point sp, Point ep)
        {
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            using (StreamGeometryContext context = geometry.Open())
            {
                InternalDrawGeometry(context, sp, ep);
            }

            return geometry;
        }

        private void InternalDrawGeometry(StreamGeometryContext context, Point sp, Point ep)
        {
            double theta = Math.Atan2(sp.Y - ep.Y, sp.X - ep.X);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point p1 = new Point(ep.X - Constants.CALACTION_TRACELEN, ep.Y);
            Point p2 = new Point(ep.X, ep.Y - Constants.CALACTION_TRACELEN);
            Point p3 = new Point(ep.X + Constants.CALACTION_TRACELEN, ep.Y);
            Point p4 = new Point(ep.X, ep.Y + Constants.CALACTION_TRACELEN);

            context.BeginFigure(sp, true, false);
            context.LineTo(ep, true, true);

            //draw cross
            context.BeginFigure(p1, true, false);
            context.LineTo(p3, true, true);

            context.BeginFigure(p2, true, false);
            context.LineTo(p4, true, true);
        }

        public abstract void OnMouseLeftDown(object sender, MouseButtonEventArgs e);

        public abstract void OnMouseUp(object sender, MouseButtonEventArgs e);

        public abstract void OnMouseMove(object sender, MouseEventArgs e);

        public abstract void OnViewOriginal(object sender, RoutedEventArgs e);

        public abstract void OnMouseRightDown(object sender, MouseButtonEventArgs e);

        protected Point GetMercator(Point p)
        {
            Point point = new Point();
            int Column = (int)p.X / 256;
            int Row = (int)p.Y / 256;

            double dx = p.X - Column * 256;
            double dy = p.Y - Row * 256;

            Tile tile = PipeNetManager.App.Tiles[Row * Level.Total_Column + Column];
            point.X = tile.X + tile.Dx * dx;
            point.Y = tile.Y - tile.Dy * dy;
            return point;
        }

        protected void DrawText(string text, Point sp, Point ep, double font_size)
        {
            Label label = new Label();
            label.Content = text;
            label.FontSize = font_size;
            mCanvas.Children.Add(label);

            Point location = new Point((sp.X + ep.X) / 2, (sp.Y + ep.Y) / 2);

            double angle = (Math.Atan2(sp.Y - ep.Y, sp.X - ep.X) + Math.PI) * 180 / Math.PI;

            if (angle != 0)
                label.LayoutTransform = new RotateTransform(angle);
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            double x = location.X;
            x -= label.DesiredSize.Width / 2;
            Canvas.SetLeft(label, x);

            double y = location.Y;
            y -= label.DesiredSize.Height / 2;
            Canvas.SetTop(label, y);
        }

        protected double WGS84Distance(Point sp, Point ep)
        {
            double radLat1 = rad(sp.X);
            double radLat2 = rad(ep.X);
            double a = radLat1 - radLat2;
            double b = rad(sp.Y) - rad(sp.Y);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
              Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 1000);
            return s;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        protected Point Mercator2WGS84(Point mercator)
        {
            Point wgs84  = new Point();
            double x = mercator.X / 20037508.34 * 180;
            double y = mercator.Y / 20037508.34 * 180;
            y = 180 / Math.PI * (2 * Math.Atan(Math.Exp(y * Math.PI / 180)) - Math.PI / 2);
            wgs84.X = x;
            wgs84.Y = y;
            return wgs84;
        }
    }
}
