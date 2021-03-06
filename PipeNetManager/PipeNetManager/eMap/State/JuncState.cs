﻿using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
using DBCtrl.DBRW;
using GIS.Arc;
using GIS.Map;
using PipeMessage.eMap;
using PipeNetManager.common;
using PipeNetManager.juncMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PipeNetManager.eMap.State
{
     public  class JuncState:IState
     {
        public JuncState(Canvas canvas):base(canvas){

            animationcanvas = new Canvas();
            canvas.Children.Add(animationcanvas);           //用于显示动画特效

        }

        protected Canvas animationcanvas = null;
        /// <summary>
        /// 选择对象
        /// </summary>
        /// <param name="path"></param>
        public override void SelectShape(Path path)
        {
            if (path == null) return;
            if(SelectPath!=null)
            {
                SelectPath.Stroke = colorCenter.UnSelected_Border_Color;
            }
            if (path != SelectPath)
            {
                path.Stroke = colorCenter.Selected_Border_Color;
                SelectPath = path;
            }
            else
                SelectPath = null;
        }

        //添加单个检查井
        /// <summary>
        /// 增加井盖
        /// </summary>
        /// <param name="cover"></param>
        /// <param name="cp"></param>
        public void AddJunc(Cover cover, Point cp)
        {
            Path path = new Path();
            path.Fill = cover.GetColorBrush();
            path.Stroke = colorCenter.UnSelected_Border_Color;
            EllipseGeometry eg = new EllipseGeometry();            
            eg.Center = cp;
            eg.RadiusX = App.StrokeThinkness;
            eg.RadiusY = App.StrokeThinkness;
            path.Data = eg;
            path.ToolTip = cover;
            
            context.Children.Add(path);
            listpath.Add(path);
        }

        public Path AddJunc(Cover cover)
        {
            Path path = new Path();
            path.Fill = cover.GetColorBrush();
            path.Stroke = colorCenter.UnSelected_Border_Color;
            EllipseGeometry eg = new EllipseGeometry();
            eg.Center = Mercator2Screen(cover.Location);
            eg.RadiusX = App.StrokeThinkness;
            eg.RadiusY = App.StrokeThinkness;
            path.Data = eg;
            path.ToolTip = cover;

            context.Children.Add(path);
            listpath.Add(path);
            return path;
        }

        /// <summary>
        /// 创建正方形
        /// </summary>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        protected Path CreatRect(Point p , double r)
        {
            Path path = new Path();
            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(p.X - r, p.Y - r, 2*r, 2*r);
            path.StrokeThickness = App.StrokeThinkness / 2;
            path.Stroke = colorCenter.Selected_Border_Color;
            path.Data = rg;
            return path;
        }

        /// <summary>
        /// 删除检查井
        /// </summary>
        /// <param name="path"></param>
        public bool DelJunc(Path path)
        {
            if (path == null) return false;
            path.Stroke = colorCenter.Selected_Border_Color;
            string msg = "是否删除选中对象?";
            string title = "删除";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(msg, title, buttons, icon);
            if (result == MessageBoxResult.Yes)
            {
                context.Children.Remove(path);
                listpath.Remove(path);
                return true;
            }
            else
            {
                path.Stroke = colorCenter.UnSelected_Border_Color;
                return false; 
            }
        }

        public void delJunc(Path path)
        {
            if (path == null) return;
            context.Children.Remove(path);
            listpath.Remove(path);
        }

        //更新检查井位置
        public void UpdateJuncPos(float[] px, float[] py)
        {
            for (int i = 0; i < listpath.Count; i++)
            {
                ((EllipseGeometry)(listpath[i].Data)).Center = new Point(px[i], py[i]);
                ((EllipseGeometry)(listpath[i].Data)).RadiusX = App.StrokeThinkness;
                ((EllipseGeometry)(listpath[i].Data)).RadiusY = App.StrokeThinkness;
            }
        }

        public void UpdateJuncPos(List<Point> list)
        {
            int i = 0;
            foreach (Point p in list)
            {
                ((EllipseGeometry)(listpath[i].Data)).Center = p;
                ((EllipseGeometry)(listpath[i].Data)).RadiusX = App.StrokeThinkness;
                ((EllipseGeometry)(listpath[i].Data)).RadiusY = App.StrokeThinkness;
                i++;
            }
        }

        public virtual int AddJunc2Data(Cover c) { return 0; }

        public virtual void DelJuncFromData(Cover c) { }

        //鼠标按下后，出现基本信息
        public override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Path path = e.Source as Path;
            if (path==null)
                return;

            SelectShape(path);                              //设置焦点
           
            //若是双击
            if (e.ClickCount >= 2)
            {
                 Cover cover = path.ToolTip as Cover;
                 if (cover == null) return;
                 JuncWindow juncWnd;
                 if (cover.juncInfo.JuncName == null || cover.juncInfo.JuncName.Length <= 0)
                 {
                      juncWnd = new JuncWindow(cover.juncInfo.ID);
                 }
                 else
                 {
                      juncWnd = new JuncWindow(cover.Name);
                 }
                 juncWnd.ShowDialog();
            }
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            
        }

         //返回插入junction的id
        protected int InsterDB(Cover c)
        {
            if (c == null) return 0;
            //change the coordinate Mercator to WGS84
            Coords.Point p = new Coords.Point();
            p.x = c.Location.X;
            p.y = c.Location.Y;
            p = Coords.Mercator2WGS84(p);
            //store to 
            CJuncInfo info = new CJuncInfo();
            info.X_Coor = p.x + Constants.COOR_X_OFFSET;
            info.Y_Coor = p.y + Constants.COOR_Y_OFFSET;

            info.Junc_Category = c.juncInfo.Junc_Category;

            //数据库操作
            TJuncInfo juncinfo = new TJuncInfo(App._dbpath, App.PassWord);
            TJuncExtInfo juncextinfo = new TJuncExtInfo(App._dbpath, App.PassWord);
            if (!juncinfo.Insert_JuncInfo(ref info)) {
                return 0;
            }
            CJuncExtInfo extinfo = new CJuncExtInfo();
            extinfo.JuncID = info.ID;
            juncextinfo.Insert_JuncExtInfo(ref extinfo);
            return info.ID;
        }

        protected void DelDB(Cover c)
        {
            if (c == null) return;
            CJuncInfo info = c.juncInfo;

            DeleteCmd cmd = new DeleteCmd();
            JuncRev rev = new JuncRev();
            rev.ListJunc = new List<CJuncInfo>();
            rev.ListJunc.Add(info);

            cmd.SetReceiver(rev);
            cmd.Execute();
        }
    }
}
