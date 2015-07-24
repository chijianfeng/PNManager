using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
using GIS.Arc;
using PipeMessage.eMap;
using PipeNetManager.common;
using PipeNetManager.pipeMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PipeNetManager.eMap.State
{
    class PipeState : IState
    {
        /*
         * this path is for drawing the pipe before saving in database.
         */
        protected Path mMovingPath = null;

        public PipeState(Canvas canvas) : base(canvas) { }

        /// <summary>
        /// 管道选择
        /// </summary>
        /// <param name="path"></param>
        public override void SelectShape(System.Windows.Shapes.Path path)
        {
            if (path == null)
            {
                return;
            }
            if (SelectPath != null)
            {
                Pipe p = (Pipe)SelectPath.ToolTip;
                SelectPath.Stroke = p.GetColorBrush();
            }
            if (path != SelectPath)
            {
                path.Stroke = colorCenter.Selected_Border_Color;
                SelectPath = path;
            }
            else
                SelectPath = null;

        }

        /// <summary>
        /// 增加管道
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public void AddPipe(Pipe pipe, Point Start, Point End)
        {
            Path path = new Path();
            path.Stroke = pipe.GetColorBrush();

            path.Data = DrawPipe(Start, End);

            path.StrokeThickness = App.StrokeThinkness * 2/3;
            path.SetValue(Canvas.ZIndexProperty, -1);
            path.ToolTip = pipe;
            context.Children.Add(path);
            listpath.Add(path);
        }

        public void AddPipe(Pipe pipe, VectorLine line)
        {
            AddPipe(pipe, line.StartPoint, line.EndPoint);
        }

        /// <summary>
        /// 更新管道
        /// </summary>
        /// <param name="sps"></param>
        /// <param name="eps"></param>
        public void UpdatePipes(Point[] sps, Point[] eps)
        {
            HeadHeight = App.StrokeThinkness;
            HeadWidth = App.StrokeThinkness * 2;
            for (int i = 0; i < listpath.Count; i++)
            {
                using (StreamGeometryContext context = ((StreamGeometry)(listpath[i].Data)).Open())
                {
                    InternalDrawArrowGeometry(context, sps[i], eps[i]);
                }
                listpath[i].StrokeThickness = App.StrokeThinkness * 2 / 3;
            }
        }

        public void UpdatePipes(List<VectorLine> list)
        {
            HeadHeight = App.StrokeThinkness;
            HeadWidth = App.StrokeThinkness * 2;
            for (int i = 0; i < listpath.Count; i++)
            {
                using (StreamGeometryContext context = ((StreamGeometry)(listpath[i].Data)).Open())
                {
                    InternalDrawArrowGeometry(context, list[i].StartPoint, list[i].EndPoint);
                }
                listpath[i].StrokeThickness = App.StrokeThinkness * 2 / 3;
            }
        }

        /// <summary>
        /// 删除管道
        /// </summary>
        /// <param name="path"></param>
        public bool DelPipe(Path path)
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

        /// <summary>
        /// 删除管道，但是不会有提示
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool delPipe(Path path)
        {
            if (path == null) return false;
            context.Children.Remove(path);
            listpath.Remove(path);
            return true;
        }

        /// <summary>
        /// 响应按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Path path = e.Source as Path;
            if (path == null)
                return;

            SelectShape(path);
            object obj = path.ToolTip;
            Pipe p = obj as Pipe;
            if (p == null)
                return;

            //double click
            if (e.ClickCount >= 2)
            {
                PipeWindow pipeWnd;
                if(p.pipeInfo.PipeName!=null&&!p.pipeInfo.PipeName.Equals(Constants.PIPENONENAME))
                    pipeWnd = new PipeWindow(p.pipeInfo.PipeName);
                else{
                    pipeWnd = new PipeWindow(p.pipeInfo.ID);
                }
                pipeWnd.ShowDialog();
            }
        }

        /// <summary>
        /// 响应鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentMode == ADDMODE&&mMovingPath!=null)
            {
                 Point newp = e.GetPosition(context);
                 using (StreamGeometryContext cnt = ((StreamGeometry)(mMovingPath.Data)).Open())
                 {
                     InternalDrawArrowGeometry(cnt, p1, newp);
                 }
            }
        }

        protected Geometry DrawPipe(Point sp, Point ep)
        {
            StreamGeometry geometry = new StreamGeometry();
            geometry.FillRule = FillRule.EvenOdd;

            using (StreamGeometryContext context = geometry.Open())
            {
                InternalDrawArrowGeometry(context, sp, ep);
            }

            return geometry;
        }

        protected void InsterDb(Pipe pipe , Cover sc , Cover ec)
        {
            CPipeInfo pipeInfo = new CPipeInfo();
            CPipeExtInfo pipeExtInfo = new CPipeExtInfo();
            CUSInfo UsInfo = new CUSInfo();
            pipeInfo.PipeName = pipe.Name;
            pipeInfo.In_JunID = sc.juncInfo.ID;
            pipeInfo.Out_JunID = ec.juncInfo.ID;

            pipeInfo.Pipe_Category = pipe.pipeInfo.Pipe_Category;
            UsInfo.Struct_Class = 0;

            InsertCmd icmd = new InsertCmd();
            PipeRev piperev = new PipeRev();

            List<CPipeInfo> listpipe = new List<CPipeInfo>();
            List<CPipeExtInfo> listpipExt = new List<CPipeExtInfo>();
            List<CUSInfo> listUsInfo = new List<CUSInfo>();

            listpipe.Add(pipeInfo);
            listpipExt.Add(pipeExtInfo);
            listUsInfo.Add(UsInfo);

            piperev.ListPipe = listpipe;
            piperev.ListPipeExt = listpipExt;
            piperev.ListUS = listUsInfo;
            icmd.SetReceiver(piperev);
            icmd.Execute();
        }

        protected void DeleteDb(Pipe pipe)
        {
            if (pipe == null) return;
            DeleteCmd dcmd = new DeleteCmd();
            PipeRev piperev = new PipeRev();
            piperev.ListPipe = new List<DBCtrl.DBClass.CPipeInfo>();
            piperev.ListPipe.Add(pipe.pipeInfo);
            dcmd.SetReceiver(piperev);
            dcmd.Execute();
        }


        private void InternalDrawArrowGeometry(StreamGeometryContext context, Point sp, Point ep)
        {
            double theta = Math.Atan2(sp.Y - ep.Y, sp.X - ep.X);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pm = new Point((sp.X + ep.X) / 2, (sp.Y + ep.Y) / 2);

            Point pt3 = new Point(
                pm.X + (HeadWidth * cost - HeadHeight * sint),
                pm.Y + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new Point(
                 pm.X + (HeadWidth * cost + HeadHeight * sint),
                pm.Y - (HeadHeight * cost - HeadWidth * sint));

            //begin draw line
            context.BeginFigure(sp, true, false);
            context.LineTo(ep, true, true);

            context.BeginFigure(pt3, true, false);
            context.LineTo(pm, true, true);
            context.LineTo(pt4, true, true);
        }

        protected bool IsDrawLine = false;              //是否划线
        protected Cover c1, c2;
        protected Point p1, p2;
        protected double HeadWidth = App.StrokeThinkness;
        protected double HeadHeight = App.StrokeThinkness;
    }
}
