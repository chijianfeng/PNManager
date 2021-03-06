﻿using GIS.Arc;
using PipeNetManager.UndoRedo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PipeNetManager.eMap.State
{
    class RainJuncState:JuncState
    {
        public RainJuncState(RainJuncs rj) : base(rj.RainJuncsCanvas) {

            rainjuncs = rj;
        }

        /// <summary>
        /// 添加多个检查井
        /// </summary>
        /// <param name="listjuncs"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        public void AddRainJunc(List<RainCover> listjuncs , float[] px, float[] py)
        {
            int index = 0;
            foreach(RainCover cover in listjuncs)
            {
                AddJunc(cover, new Point(px[index], py[index]));
                index++;
            }
        }

        public void AddRainJunc(List<RainCover> listjuncs, List<Point> listpoint)
        {
            int index = 0;
            foreach (RainCover cover in listjuncs)
            {
                AddJunc(cover, listpoint[index]);
                index++;
            }
        }

        /// <summary>
        /// set the new junction point to buffer and insert into database
        /// </summary>
        /// <param name="c"></param>
        public override int AddJunc2Data(Cover c) {
            rainjuncs.AddJunc((RainCover)c);
            return InsterDB(c);
        }

        public override void DelJuncFromData(Cover c)
        {
            rainjuncs.DelJunc((RainCover)c);
            //删除数据库中数据
            DelDB(c);
        }

        
        /// <summary>
        /// 响应鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(CurrentMode==ADDMODE)
            {
                Point cp = e.GetPosition(context);      //获取相关坐标
                cp.X = cp.X + 7-App.StrokeThinkness/2;
                cp.Y = cp.Y + 7-App.StrokeThinkness/2;  //设置为中心
                RainCover c = new RainCover("雨水检查井", GetMercator(cp), "双击查看详细信息");
                //添加其他相关信息
                c.Location = GetMercator(cp);

                JuncAddCommand cmd = new JuncAddCommand(this, c);
                cmd.Excute();
                CmdManager.getInstance().PushCmd(cmd);
            }
            else if(CurrentMode==DELMODE)
            {
                Path path = e.Source as Path;
                if (path == null)
                {
                    base.OnMouseDown(sender, e);          //若都不是添加或删除命令，则交给父类进行处理
                    return;
                }
                JuncDelCommand cmd = new JuncDelCommand(this, path);
                cmd.Excute();
                CmdManager.getInstance().PushCmd(cmd);
            }
            base.OnMouseDown(sender, e);                //若都不是添加或删除命令，则交给父类进行处理
        }

        /// <summary>
        /// 鼠标移动事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);
            if (CurrentMode == SELECTMODE)                                  //若是选择模式，不进行位置标记
                return;
            Point cp = e.GetPosition(context);                              //获取相对位置
            cp.X = cp.X + 7 - App.StrokeThinkness / 2;
            cp.Y = cp.Y + 7 - App.StrokeThinkness / 2;                      //设置为中心
            Cover c = rainjuncs.FindClosedCover(cp);
            if (null == c)
            {
                animationcanvas.Children.Clear();
                return; 
            }
            //转换坐标
            cp.X = ((c.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx);
            cp.Y = ((App.Tiles[0].Y - c.Location.Y) / App.Tiles[0].Dy);
            //创建选中矩形框
            if(animationcanvas.Children.Count>0)
            {
                //改变位置
                animationcanvas.Children.Clear();
                animationcanvas.Children.Add(CreatRect(cp, App.StrokeThinkness));
            }
            else
            {
                animationcanvas.Children.Add(CreatRect(cp, App.StrokeThinkness));
            }
        }

        private RainJuncs rainjuncs = null;
    }
}
