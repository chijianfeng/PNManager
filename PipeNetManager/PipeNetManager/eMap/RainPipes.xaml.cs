using DBCtrl.DBClass;
using DBCtrl.DBRW;
using GIS.Arc;
using PipeMessage.eMap;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PipeNetManager.eMap
{
    /// <summary>
    /// RainPipes.xaml 的交互逻辑
    /// </summary>
    public partial class RainPipes : BaseControl
    {

        public RainPipes(RainJuncs rj) 
        {
            InitializeComponent();
            rainjunc = rj;                      //检查井与管道绑定
            RainPipeGrid.Margin = new Thickness(0, 0, 0, 0);
            state = new RainPipeState(this);
          
        }

       public void InitPipes()
        {
            if (((App)System.Windows.Application.Current).arcmap == null)                     //数据为空，从数据库中导入数据
            {
                ArcMap map = new ArcMap();
                map.LoadRainCover();
                map.LoadRainPipe();
            }
            listRains = ((App)System.Windows.Application.Current).arcmap.RainPipeList;

            StartPipe = new Point[listRains.Count+50];
            EndPipe = new Point[listRains.Count+50];

            addpipes();                         //图层中添加管道
        }


        public void AddPipes() {
            state.AddRainPipes(listRains, StartPipe, EndPipe);
        }

        
        CUSInfo FindUSInfo(List<CUSInfo> usinfolist, int pipeId)
        {
            CUSInfo info = null;
            info = usinfolist.Find(us => us.PipeID == pipeId);
            return info;
        }
        //增加雨水管道
        void addpipes()
        {
            //Task.Factory.StartNew((Obj) =>
            //{
            //    Parallel.For(0, (int)Obj, i =>          //计算位置
            //    {
            //        StartPipe[i].X = (listRains[i].Start.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
            //        StartPipe[i].Y = (App.Tiles[0].Y - listRains[i].Start.Location.Y) / App.Tiles[0].Dy;

            //        EndPipe[i].X = (listRains[i].End.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
            //        EndPipe[i].Y = (App.Tiles[0].Y - listRains[i].End.Location.Y) / App.Tiles[0].Dy;
            //    });

            //}, listRains.Count).ContinueWith(ant => {               //添加到图层中
            //}, TaskScheduler.FromCurrentSynchronizationContext());
            for (int i = 0; i < listRains.Count; i++) {
                StartPipe[i].X = (listRains[i].Start.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
                StartPipe[i].Y = (App.Tiles[0].Y - listRains[i].Start.Location.Y) / App.Tiles[0].Dy;

                EndPipe[i].X = (listRains[i].End.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
                EndPipe[i].Y = (App.Tiles[0].Y - listRains[i].End.Location.Y) / App.Tiles[0].Dy;
            }
        }

        public void AddRainPipe(RainPipe pipe)
        {
            listRains.Add(pipe);
            int i = listRains.Count;
            StartPipe[i].X = (listRains[i-1].Start.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
            StartPipe[i].Y = (App.Tiles[0].Y - listRains[i-1].Start.Location.Y) / App.Tiles[0].Dy;

            EndPipe[i].X = (listRains[i-1].End.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
            EndPipe[i].Y = (App.Tiles[0].Y - listRains[i-1].End.Location.Y) / App.Tiles[0].Dy;
        }

        public void DelRainPipe(RainPipe pipe)
        {
            int index = 0;
            foreach(RainPipe rp in listRains)
            {
                if (pipe.Name.Equals(rp.Name))
                {
                    break;
                }
                index++;
            }
            if (index < listRains.Count)
                listRains.RemoveAt(index);
        }

        //更新管道
        void UpdatePipes()
        {
            Task.Factory.StartNew((Obj) =>
            {

                Parallel.For(0, (int)Obj, i =>          //计算位置
                {
                    StartPipe[i].X = (listRains[i].Start.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
                    StartPipe[i].Y = (App.Tiles[0].Y - listRains[i].Start.Location.Y) / App.Tiles[0].Dy;

                    EndPipe[i].X = (listRains[i].End.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx;
                    EndPipe[i].Y = (App.Tiles[0].Y - listRains[i].End.Location.Y) / App.Tiles[0].Dy;
                });

            }, listRains.Count).ContinueWith(ant =>
            {               //更新到图层中

                state.UpdatePipes(StartPipe, EndPipe);

            }, TaskScheduler.FromCurrentSynchronizationContext());
            this.RainPipeGrid.Margin = App.MoveRect;                        //更新相对位置
        }

        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            UpdatePipes();
        }

        public override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsViewMove)
            {
                IsMousedown = false;
            }
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsViewMove || !IsMousedown)
                return;
            Grid CurGrid = this.RainPipeGrid;
            CurGrid.Margin = App.MoveRect;
        }

        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (IsViewMove)                                 //移动操作
            {
                IsMousedown = true;
                return;
            }
            if(IsZoomIn)                                    //放大操作
            {
                if (App.Cur_Level_Index > App.TotalLevels||IsHidden)
                    return;
                UpdatePipes();
                return;
            }
            if(IsZoomOut)                                   //缩小操作
            {
                if (App.Cur_Level_Index < 0||IsHidden)
                    return;
                UpdatePipes();
                return;
            }
            state.OnMouseDown(sender, e);                   //其他操作，删除，选择操作等
        }

        public new void Update()
        {
            UpdatePipes();
        }

        public override void SetOperationMode(int mode)
        {
            if (state == null)
                return;
            state.CurrentMode = mode;
            if (mode == IState.ADDMODE)
                rainjunc.SetOperationMode(IState.RELATEDMODE);  //操作关联
        }

        List<RainPipe> listRains = null;                      //雨水管道集合
        List<RainCover> listjunc = null;
        public RainJuncs rainjunc = null;

        RainPipeState state = null;                                   //操作 

        bool IsMousedown = false;                              //鼠标是否按下

        Point[] StartPipe = null;                              //起始点管道坐标
        Point[] EndPipe = null;                                //终止点管道坐标
    }
}
