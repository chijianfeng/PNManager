using DBCtrl.DBClass;
using DBCtrl.DBRW;
using GIS.Arc;
using PipeMessage.eMap;
using PipeNetManager.common;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
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
    /// WasteJuncs.xaml 的交互逻辑
    /// </summary>
    public partial class WasteJuncs : BaseControl
    {
        public WasteJuncs()
        {
            InitializeComponent();
            WasteGrid.Margin = new Thickness(-0, -0, 0, 0);
            state = new WasteJuncState(this);
        }

        public void InitWasteJuncs()
        {
            if (((App)System.Windows.Application.Current).arcmap == null)
            {
                //加载雨水检查井
                ArcMap map = new ArcMap();
                map.LoadWasterCover();
                listWaste = ((App)System.Windows.Application.Current).arcmap.WasterCoverList;
            }
            else
            {
                listWaste = ((App)System.Windows.Application.Current).arcmap.WasterCoverList;
            }
            //将点坐标进行保存
            mListScreenpoint = new List<Point>(listWaste.Count + Constants.JUNCBUFFERSIZE);

            addjuncs();                     //添加污水井
        }

        private  void addjuncs()
        {
            for (int i = 0; i < listWaste.Count; i++) {
                mListScreenpoint.Add(state.Mercator2Screen(listWaste.ElementAt(i).Location));
            }
        }

        public void AddJuncs() {
            state.AddWasteJunc(listWaste, mListScreenpoint);
        }

        public void AddWasteJunc(WasteCover wc)
        {
            listWaste.Add(wc);
            mListScreenpoint.Add(state.Mercator2Screen(wc.Location));
        }

        public void DelWasteJunc(WasteCover c)
        {
            int index = 0;
            foreach (WasteCover tmpc in listWaste)
            {
                if (c.juncInfo.ID.Equals(tmpc.juncInfo.ID))
                {
                    break;
                }
                index++;
            }
            if (index < listWaste.Count)
                listWaste.RemoveAt(index);
        }

        public WasteCover FindClosedCover(Point p)
        {
            WasteCover cover = null;
            double dis = App.StrokeThinkness;
            for (int i = 0; i < listWaste.Count; i++)
            {
                if (Math.Abs(mListScreenpoint.ElementAt(i).X - p.X) > dis || Math.Abs(mListScreenpoint.ElementAt(i).Y - p.Y) > dis)
                    continue;
                double d = Math.Sqrt((mListScreenpoint.ElementAt(i).X - p.X) * (mListScreenpoint.ElementAt(i).X - p.X) +
                    (mListScreenpoint.ElementAt(i).Y - p.Y) * (mListScreenpoint.ElementAt(i).Y - p.Y));                       //计算距离
                if (dis > d)
                {
                    dis = d;
                    cover = listWaste[i];
                }
            }
            return cover;
        }

        private void UpdateWasteJuncs()
        {
            //计算相对位置
            Task.Factory.StartNew<int>((Obj) =>
            {
                Parallel.For(0, (int)Obj, i =>              //并行计算
                {
                    mListScreenpoint[i] = state.Mercator2Screen(listWaste.ElementAt(i).Location);
                });
                return 0;
            }, listWaste.Count).ContinueWith(ant =>
            {
                state.UpdateJuncPos(mListScreenpoint);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            this.WasteGrid.Margin = App.MoveRect;
        }

        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            UpdateWasteJuncs();
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
            Grid CurGrid = this.WasteGrid;
            CurGrid.Margin = App.MoveRect;
        }
        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (IsViewMove)
            {
                IsMousedown = true;
                return;
            }
            
            if (IsZoomIn)            //放大或缩小操作
            {
                if (App.Cur_Level_Index  > App.TotalLevels||IsHidden)
                        return;
                 UpdateWasteJuncs();
                    return;
            }
           if (IsZoomOut)
           {
              if (App.Cur_Level_Index < 0||IsHidden)
                    return;
              UpdateWasteJuncs();
              return;
           }
            state.OnMouseDown(sender, e);                   //其他操作，删除，选择操作等
        }

        public new void Update()
        {
            UpdateWasteJuncs();
        }

        public override void SetOperationMode(int mode)
        {
            if (null == state)
                return;
            state.CurrentMode = mode;
        }

        public List<WasteCover> listWaste = null;

        private List<Point> mListScreenpoint = null;            //屏幕上物理坐标

        WasteJuncState  state = null;                           //操作 

        bool IsMousedown = false;                              //鼠标是否按下
    }
}
