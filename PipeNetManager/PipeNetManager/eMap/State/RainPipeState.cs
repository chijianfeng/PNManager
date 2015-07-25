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
    class RainPipeState:PipeState
    {
        public RainPipeState(RainPipes rp) : base(rp.RainPipeCanvas) 
        {
            rainpipes = rp;
        }
        /// <summary>
        /// 添加多个雨水管道
        /// </summary>
        /// <param name="listpipe"></param>
        /// <param name="sps"></param>
        /// <param name="eps"></param>
        public void AddRainPipes(List<RainPipe> listpipe , List<VectorLine> list)
        {
            int index = 0;
            foreach (RainPipe pipe in listpipe)
            {
                Path path = new Path();
                path.Stroke = pipe.GetColorBrush();
                //添加带方向的管道

                path.Data = DrawPipe(list[index].StartPoint, list[index].EndPoint);
                index++;
                
                path.StrokeThickness = App.StrokeThinkness;
                path.ToolTip = pipe;
                context.Children.Add(path);
                listpath.Add(path);
            }
        }

        public override int AddPipe2Data(Pipe pipe , Cover injunc , Cover outjunc)
        {
            rainpipes.AddRainPipe((RainPipe)pipe);
            //插入后台数据库
            return InsterDb(pipe , injunc , outjunc);
        }

        public override void DelPipeFromData(Pipe pipe)
        {
            rainpipes.DelRainPipe((RainPipe)pipe);
            DeleteDb(pipe);
        }

        public new  void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentMode == ADDMODE)
            {
                Point cp = e.GetPosition(context);                     //获取相关坐标
                cp.X = cp.X + 7 - App.StrokeThinkness / 2;
                cp.Y = cp.Y + 7 - App.StrokeThinkness / 2;             //设置为中心
                Cover c = rainpipes.rainjunc.FindClosedCover(cp);      //检测最近点位
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
                    mStartPoint.Y = Mercator2ScreenY(mStartJunc.Location.Y) +App.StrokeThinkness / 2;

                    mMovingPath = new Path();
                    mMovingPath.Stroke = colorCenter.Seleted_Fill_Color;
                    mMovingPath.Data = DrawPipe(mStartPoint, mStartPoint);
                    mMovingPath.StrokeThickness = App.StrokeThinkness/2;
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

                    RainPipe pipe = new RainPipe(mStartJunc.Name + "-" + mEndJunc.Name, "双击查看信息", mStartJunc, mEndJunc);
                    pipe.Start.Location = GetMercator(mStartPoint);
                    pipe.End.Location = GetMercator(mEndPoint);

                    PipeAddCommand cmd = new PipeAddCommand(this ,pipe , mStartJunc, mEndJunc);
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

        private RainPipes rainpipes = null;
    }
}
