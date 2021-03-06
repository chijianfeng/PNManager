﻿using DBCtrl.DBClass;
using DBCtrl.DBRW;
using GIS.Arc;
using PipeMessage.eMap;
using PipeNetManager.common;
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
    /// WastePipes.xaml 的交互逻辑
    /// </summary>
    public partial class WastePipes : BaseControl
    {
        public WastePipes(WasteJuncs wj)
        {
            InitializeComponent(); 
            wastejunc = wj;
            WastePipeGrid.Margin = new Thickness(0, 0, 0, 0);
            state = new WastePipeState(this);
            
        }

        public void InitPipes()
        {
            if (((App)System.Windows.Application.Current).arcmap == null)                     //数据为空，从数据库中导入数据
            {
                ArcMap map = new ArcMap();
                map.LoadWasterCover();
                map.LoadWasterPipe();
            }
            listWastes = ((App)System.Windows.Application.Current).arcmap.WastePipeList;

            mListVLine = new List<VectorLine>(listWastes.Count + Constants.PIPEBUFFERSIZE);
            addpipes();
        }

       

        CUSInfo FindUSInfo(List<CUSInfo> usinfolist,int pipeId)
        {
            CUSInfo info = null;
            info = usinfolist.Find(us => us.PipeID == pipeId);
            return info;
        }

        //增加污水管道
        void addpipes()
        {
            for (int i = 0; i < listWastes.Count; i++)
            {
                mListVLine.Add(new VectorLine(state.Mercator2Screen(listWastes[i].Start.Location) , 
                                              state.Mercator2Screen(listWastes[i].End.Location)));
            }
        }

        
        public void AddPipes() {
            state.AddWastePipes(listWastes, mListVLine);
        }

        public void AddWastePipe(WastePipe pipe)
        {
            listWastes.Add(pipe);
            mListVLine.Add(new VectorLine(pipe.Start.Location, pipe.End.Location));
        }

        public void DelWastePipe(WastePipe pipe)
        {
            int index = 0;
            foreach (WastePipe rp in listWastes)
            {
                if (pipe.Name.Equals(rp.Name))
                {
                    break;
                }
                index++;
            }
            if (index < listWastes.Count)
            {
                listWastes.RemoveAt(index);
                mListVLine.RemoveAt(index);
            }
            
        }
        void UpdatePipes()
        {
            Task.Factory.StartNew((Obj) =>
            {

                Parallel.For(0, (int)Obj, i =>          //计算位置
                {
                    mListVLine[i].StartPoint = state.Mercator2Screen(listWastes[i].Start.Location);
                    mListVLine[i].EndPoint = state.Mercator2Screen(listWastes[i].End.Location);
                });

            }, listWastes.Count).ContinueWith(ant =>
            {               //更新到图层中
                state.UpdatePipes(mListVLine);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            this.WastePipeGrid.Margin = App.MoveRect;                        //更新相对位置
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
            state.OnMouseMove(sender, e);
            if (!IsViewMove || !IsMousedown)
                return;
            Grid CurGrid = this.WastePipeGrid;
            CurGrid.Margin = App.MoveRect;
        }

        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (IsViewMove)                                 //移动操作
            {
                IsMousedown = true;
                return;
            }
            if (IsZoomIn)                                    //放大操作
            {
                if (App.Cur_Level_Index > App.TotalLevels||IsHidden)
                    return;
                UpdatePipes();
                return;
            }
            if (IsZoomOut)                                   //缩小操作
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
            if(mode==IState.ADDMODE)
                wastejunc.SetOperationMode(IState.RELATEDMODE);      //操作关联
        }

        public  WasteJuncs wastejunc = null;                    //污水检查井绑定
        List<WastePipe> listWastes = null;                      //污水管道集合

        WastePipeState state = null;                            //操作 

        bool IsMousedown = false;                              //鼠标是否按下

        List<VectorLine> mListVLine;                            //实际屏幕坐标
    }
}
