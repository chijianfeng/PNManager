using DBCtrl.DBClass;
using DBCtrl.DBRW;
using GIS.Arc;
using PipeMessage.eMap;
using PipeNetManager.common;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    /// RainJuncs.xaml 的交互逻辑
    /// </summary>
    public partial class RainJuncs : BaseControl
    {
        public RainJuncs()
        {
            InitializeComponent();
            RainGrid.Margin = new Thickness(-0, -0, 0, 0);                          //初始化相对位置
            state = new RainJuncState(this);
        }

        //初始化相关变量---》》》》》》》》》》进行坐标转换加速
        public void InitRainJuncs()
        {

            if (((App)System.Windows.Application.Current).arcmap == null)
            {
                //加载雨水检查井
                ArcMap map = new ArcMap();
                map.LoadRainCover();
                listRains = ((App)System.Windows.Application.Current).arcmap.RainCoverList;
            }
            else
            {
                listRains = ((App)System.Windows.Application.Current).arcmap.RainCoverList;
            }
           
            //将点坐标进行保存
            mListScreenpoint = new List<Point>(listRains.Count+Constants.JUNCBUFFERSIZE);
            addjuncs();
        }

        //向图层添加检查井
        private void addjuncs()
        {
            for (int i = 0; i < listRains.Count;i++ )
            {
                mListScreenpoint.Add(state.Mercator2Screen(listRains.ElementAt(i).Location));
            }
        }

        public void AddJuncs() { 
            state.AddRainJunc(listRains, mListScreenpoint);
        }

        public void AddJunc(RainCover c)           //添加雨水检查井
        {
            listRains.Add(c);
            //计算点的坐标
            mListScreenpoint.Add(state.Mercator2Screen(c.Location));
         
        }

        public void DelJunc(RainCover c)
        {
            int index = 0;
            foreach(RainCover tmpc in listRains)
            {
                if(c.Name.Equals(tmpc.Name))
                {
                    break;
                }
                index++;
            }
            if (index < listRains.Count)
            {
                listRains.RemoveAt(index);
                mListScreenpoint.RemoveAt(index);
            }
        }

        /// <summary>
        ///在现有的检查井中寻找最接近的点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public RainCover FindClosedCover(Point p)
        {
            RainCover cover = null;
            double dis = App.StrokeThinkness;
            for (int i = 0; i < listRains.Count;i++ )
            {
                if (Math.Abs(mListScreenpoint.ElementAt(i).X - p.X) > dis || Math.Abs(mListScreenpoint.ElementAt(i).Y - p.Y) > dis)
                    continue;
                double d = Math.Sqrt((mListScreenpoint.ElementAt(i).X - p.X) * (mListScreenpoint.ElementAt(i).X - p.X) +
                    (mListScreenpoint.ElementAt(i).Y - p.Y) * (mListScreenpoint.ElementAt(i).Y - p.Y));                       //计算距离
                if (dis > d)
                {
                    dis = d;
                    cover = listRains[i];
                }
            }
            return cover;
        }

        //更新检查井--》》》》》》》进行加速
        private void UpdateRainJuncs()
        {
            //计算相对位置
            Task.Factory.StartNew<int>((Obj) =>
            {
                Parallel.For(0, (int)Obj, i =>              //并行计算
                {
                    mListScreenpoint[i] = state.Mercator2Screen(listRains.ElementAt(i).Location);
                });
                return 0;
            }, listRains.Count).ContinueWith(ant =>
            {
                state.UpdateJuncPos(mListScreenpoint);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            this.RainGrid.Margin = App.MoveRect;
        }
        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            UpdateRainJuncs();
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
            {
                state.OnMouseMove(sender, e);                       //默认处理
                return; 
            }
            Grid CurGrid = this.RainGrid;
            CurGrid.Margin = App.MoveRect;
        }

        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if(IsViewMove)
            {
                IsMousedown = true;
                return;
            }
            if (IsZoomIn)            //放大操作
            {
                if (App.Cur_Level_Index > App.TotalLevels||IsHidden)    //达到最大放大级别或者该图层为隐藏
                    return;
                UpdateRainJuncs();
                return;
            }
            if (IsZoomOut)          //缩小
            {
                if (App.Cur_Level_Index < 0||IsHidden)
                    return;
                UpdateRainJuncs();
                return;
            }
            state.OnMouseDown(sender, e);                               //其他操作，删除，选择操作等
        }

        public new void Update()                                           //强制更新检查井位置
        {
            UpdateRainJuncs();
        }
        public override void SetOperationMode(int mode)
        {
            if (state == null)
                return;
            state.CurrentMode = mode;
        }

        public List<RainCover> listRains = null;               //雨水检查井集合

        private List<Point> mListScreenpoint = null;            //屏幕上物理坐标

        RainJuncState state = null;                            //操作 

        bool IsMousedown = false;                              //鼠标是否按下
    }
}
