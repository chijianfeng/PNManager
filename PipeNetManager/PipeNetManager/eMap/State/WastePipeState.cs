using GIS.Arc;
using PipeNetManager.common;
using PipeNetManager.UndoRedo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PipeNetManager.eMap.State
{
    class WastePipeState:PipeState
    {
        public WastePipeState(WastePipes wp) : base(wp.WastePipeCanvas) 
        {
            wastepipes = wp;
        }

        public void AddWastePipes(List<WastePipe> listpipe, List<VectorLine> list)
        {
            int index = 0;
            foreach (WastePipe pipe in listpipe)
            {
                Path path = new Path();
                path.Stroke = pipe.GetColorBrush();

                //添加带方向的管道

                path.Data = DrawPipe(list[index].StartPoint, list[index].EndPoint);
                index++;

                path.StrokeThickness = App.StrokeThinkness;
                path.SetValue(Canvas.ZIndexProperty, -1);
                path.ToolTip = pipe;
                path.SetValue(Canvas.ZIndexProperty, -1);
                context.Children.Add(path);

                listpath.Add(path);
            }
        }

        public override int AddPipe2Data(Pipe pipe, Cover injunc, Cover outjunc)
        {
            wastepipes.AddWastePipe((WastePipe)pipe);
            //插入后台数据库
            return InsterDb(pipe, injunc, outjunc);
        }

        public override void DelPipeFromData(Pipe pipe)
        {
            wastepipes.DelWastePipe((WastePipe)pipe);
            DeleteDb(pipe);
        }

        public new void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentMode == ADDMODE)
            {
                Point cp = e.GetPosition(context);                     //获取相关坐标
                cp.X = cp.X + 7 - App.StrokeThinkness / 2;
                cp.Y = cp.Y + 7 - App.StrokeThinkness / 2;             //设置为中心
                Cover c = wastepipes.wastejunc.FindClosedCover(cp);     //检测最近点位
                if (null == c)
                {
                    if (mMovingPath != null)
                    {
                        context.Children.Remove(mMovingPath);
                        mMovingPath = null;
                    }
                    if (IsDrawLine)
                    {
                        IsDrawLine = false;
                    }
                    return;
                }
                if (IsDrawLine == false)
                {
                    mStartJunc = c;
                    mStartPoint.X = Mercator2ScreenX(mStartJunc.Location.X) + App.StrokeThinkness / 2; //计算管道第一个点位置坐标
                    mStartPoint.Y = Mercator2ScreenY(mStartJunc.Location.Y) + App.StrokeThinkness / 2;

                    mMovingPath = new Path();
                    mMovingPath.Stroke = colorCenter.Seleted_Fill_Color;
                    mMovingPath.Data = DrawPipe(mStartPoint , mStartPoint);
                    mMovingPath.StrokeThickness = App.StrokeThinkness / 2;
                    context.Children.Add(mMovingPath);
                }
                else
                {
                    //removing the tmp moving path
                    context.Children.Remove(mMovingPath);
                    mMovingPath = null;

                    mEndJunc = c;
                    mEndPoint.X = Mercator2ScreenX(mEndJunc.Location.X) + App.StrokeThinkness / 2;
                    mEndPoint.Y = Mercator2ScreenY(mEndJunc.Location.Y) + App.StrokeThinkness / 2;

                    WastePipe pipe = new WastePipe(mStartJunc.Name + "-" + mEndJunc.Name, "双击查看信息", mStartJunc , mEndJunc);
                    pipe.Start.Location = GetMercator(mStartPoint);
                    pipe.End.Location = GetMercator(mEndPoint);

                    PipeAddCommand cmd = new PipeAddCommand(this, pipe, mStartJunc, mEndJunc);
                    cmd.Excute();
                    CmdManager.getInstance().PushCmd(cmd);
                    
                }
                IsDrawLine = !IsDrawLine;
            }
            else if (CurrentMode == DELMODE)
            {
                Path path = e.Source as Path;
                if (path == null)
                {
                    base.OnMouseDown(sender, e);          //若都不是添加或删除命令，则交给父类进行处理
                    return;
                }
                PipeDelCommand cmd = new PipeDelCommand(this, path);
                cmd.Excute();
                CmdManager.getInstance().PushCmd(cmd);
            }
            base.OnMouseDown(sender, e);                //若都不是添加或删除命令，则交给父类进行处理
        }

        private WastePipes wastepipes = null;
    }
}
