using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using GIS.Arc;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using GIS.Map;
using BLL.Command;
using PipeNetManager;
using PipeNetManager.common;

namespace PipeMessage.eMap
{
    public abstract class IState
    {

        private Path path = null;

        protected Canvas context = null;

        public  int CurrentMode = SELECTMODE;                           //当前的模式

        protected  List<Path> listpath = new List<Path>();             //保存显示原始
        public Path SelectPath
        {
            get { return path; }
            set { path = value; }
        }

        public static readonly int SELECTMODE = 1;                      //选择模式
        public static readonly int ADDMODE = 2;                         //添加模式
        public static readonly int DELMODE = 3;                         //删除模式
        public static readonly int RELATEDMODE = 4;                     //关联模式

        public ColorCenter colorCenter = ColorCenter.GetInstance();

        public IState(Canvas canvas)
        {
            this.context = canvas;
        }

        /// <summary>
        /// 选择对象
        /// </summary>
        /// <param name="path"></param>
        public abstract void SelectShape(Path path);

        /// <summary>
        /// 坐标转换 
        /// </summary>
        /// <param name="p"> 屏幕上物理坐标点</param>
        /// <returns></returns>
        public Point GetMercator(Point p)
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

        /// <summary>
        /// 转换为GIS坐标
        /// </summary>
        /// <param name="p"> 屏幕上实际坐标</param>
        /// <returns></returns>
        public Point GetGIS842(Point p)
        {
            Point point = new Point();
            point = GetMercator(p);
            Coords.Point cp = new Coords.Point();
            cp.x = point.X;
            cp.y = point.Y;
            cp = Coords.Mercator2WGS84(cp);
            point.X = cp.x + Constants.COOR_X_OFFSET;
            point.Y = cp.y + Constants.COOR_Y_OFFSET;
            return point;
        }

        /// <summary>
        /// 墨卡托坐标转换为屏幕坐标
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point Mercator2Screen(Point p)
        {
            Point point = new Point();
            point.X = (float)((p.X - App.Tiles[0].X) / App.Tiles[0].Dx);
            point.Y = (float)((App.Tiles[0].Y - p.Y) / App.Tiles[0].Dy);
            return point;
        }

        public float Mercator2ScreenX(double x)
        {
            return (float)((x - App.Tiles[0].X) / App.Tiles[0].Dx);
        }

        public float Mercator2ScreenY(double y)
        {
            return (float)((App.Tiles[0].Y - y) / App.Tiles[0].Dy);
        }

        public Point GIS842toScreen(Point p)
        {
            p.X = p.X - Constants.COOR_X_OFFSET;
            p.Y = p.Y - Constants.COOR_Y_OFFSET;

            p = GISConverter.WGS842Merator(p);

            return Mercator2Screen(p);
        }

        /// <summary>
        /// 鼠标响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnMouseDown(object sender, MouseButtonEventArgs e);

        /// <summary>
        /// 响应鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnMouseMove(Object sender, MouseEventArgs e);
      
    }
}
