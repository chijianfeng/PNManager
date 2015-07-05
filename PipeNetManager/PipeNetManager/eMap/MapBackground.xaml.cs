﻿using GIS.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Threading;

namespace PipeNetManager.eMap
{
    /// <summary>
    /// MapBackground.xaml 的交互逻辑
    /// </summary>
    public partial class MapBackground : BaseControl
    {
        public MapBackground():base()
        {
            InitializeComponent();

            InitBackGroundMapGrid();                //初始化地图

            App.Cur_Level = detail.Levels[0];       //最初显示最小层级
            App.Cur_Level_Index = 0;
            
            //最初显示地图位置
            Ind_Row = App.Cur_Level.FTile.Row + 1;   //最初显示地图
            Ind_Column = App.Cur_Level.FTile.Column - 1;

            App.TotalLevels = detail.Level_Count;    //总层级数

            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column);
        }
        // 加载器
        Loader loader = new Loader();                //瓦片图加载器
        // 地图详情
        Detail detail = new Detail();
        // 背景Image对象
        List<Image> Images = new List<Image>();

        //默认地图，用于加载过程中间状态
        BitmapImage mDefaultImg = new BitmapImage();

        System.Threading.CancellationTokenSource cancleToken = new System.Threading.CancellationTokenSource();

        static Boolean mCanbeCancle = false;

        Point Movepos = new Point();                 //移动点

        int Ind_Row;
        int Ind_Column;

        bool IsMousedown = false;

        double mTop = -256;                           //地图偏移距离
        double mLeft = -256;

        Point refp = new Point();                   //用于缩放偏移参考点

        Thickness MoveMargin;

        void InitBackGroundMapGrid()
        {
            MapPanel.Margin = new Thickness(-0, -0, 0, 0);                          //初始布局

            for (int i = 0; i < Level.Total_Row; i++)                               //设置显示位置
            {
                for (int j = 0; j < Level.Total_Column; j++)
                {
                    Image image = new Image();
                    MapPanel.Children.Add(image);
                    Grid.SetRow(image, i);
                    Grid.SetColumn(image, j);
                    Images.Add(image);
                }
            }
            detail = loader.LoadMapDetail();
            String Abs_File_Name = System.IO.Path.GetFullPath(Level.GetDefault_Path());
            byte[] bs = File.ReadAllBytes(Abs_File_Name);
            MemoryStream str = new MemoryStream(bs);
            mDefaultImg.BeginInit();
            mDefaultImg.StreamSource = str;
            mDefaultImg.EndInit();
        }

        private void UpdateMap(Level lvl, int x, int y)
        {
            App.Tiles = lvl.GetTiles_M(x, y);
            //进行后台地图加载，利用Task Parallel 进行并行加载，.net 4.0以上版本支持
            //first stop task
            var token = cancleToken.Token;
            Task task = null;
            if (mCanbeCancle){
                cancleToken.Cancel();
            }
            try
            {
             task = Task.Factory.StartNew<MemoryStream[]>((Obj) =>
             {
                mCanbeCancle = true;
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                MemoryStream[] maps = new MemoryStream[64];
                Parallel.For(0, (int)Obj, i =>
                {
                    String Abs_File_Name = System.IO.Path.GetFullPath(App.Tiles[i].Filename);
                    byte[] bs = File.ReadAllBytes(Abs_File_Name);
                    MemoryStream str = new MemoryStream(bs);
                    maps[i] = str;
                });
                return maps;
            }, Images.Count).ContinueWith(ant =>
             {
                 mCanbeCancle = false;
                 for (int i = 0; i < Images.Count; i++)              //同步显示到界面上
                 {
                     BitmapImage img = new BitmapImage();
                     img.BeginInit();
                     img.StreamSource = ant.Result[i];
                     img.EndInit();
                     Images[i].Source = img;
                 }
                 MapPanel.Margin = MoveMargin;
             }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                if(task.IsCanceled)
                for (int i = 0; i < Images.Count; i++)              //同步显示到界面上
                {
                    Images[i].Source = mDefaultImg;
                }
            }
        }

        private void ProcessZoomIn(int row, int column)
        {
            if (App.Cur_Level_Index+1  > detail.Level_Count - 1)       //已经放大到最大级别，不能继续放大
            {
                MessageBox.Show("已到放大到最大级别！");
                return;
            }
            App.StrokeThinkness++;
            App.Cur_Level = detail.Levels[++App.Cur_Level_Index];       //设置当前缩放级别
            Ind_Row = (Ind_Row + row) * 2 - 1 - row; 
            Ind_Column = (Ind_Column + column) * 2 - 1 - column;
            if (-refp.X + MapPanel.Margin.Left < mLeft)                  //左侧超出边界
            {
                MoveMargin = new Thickness(-refp.X + MapPanel.Margin.Left - mLeft, MapPanel.Margin.Top, 0, 0);
                ++Ind_Column;
            }
            else if (-refp.Y + MapPanel.Margin.Top < mTop)               //上面超出边界
            {
                MoveMargin = new Thickness(MapPanel.Margin.Left, -refp.Y + MapPanel.Margin.Top - mTop, 0, 0);
                ++Ind_Row;
            }
            else
                MoveMargin = new Thickness(-refp.X + MapPanel.Margin.Left, -refp.Y + MapPanel.Margin.Top, 0, 0);
            App.MoveRect = MoveMargin;                                  //图层同步
            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column);              //启动更新地图
        }

        private void ProcessZoomOut(int row, int column)
        {
            if (App.Cur_Level_Index <1)
            {
                MessageBox.Show("已到缩小到到最小级别！");
                return;
            }
            App.StrokeThinkness--;
            App.Cur_Level = detail.Levels[--App.Cur_Level_Index];
            if((Ind_Row+row)%2==0&&(Ind_Column+column)%2==0)             //右下角子图
            {
                refp.X = -(-refp.X / 2 - mLeft / 2);
                refp.Y = -(-refp.Y / 2 - mTop / 2);
            }
            else if((Ind_Row+row)%2==0&&(Ind_Column+column)%2!=0)       //左下角子图
            {
                refp.X = refp.X / 2;
                refp.Y = mTop / 2 + refp.Y / 2;
            }
            else if((Ind_Row+row)%2!=0&&(Ind_Column+column)%2==0)       //右上角子图
            {
                refp.X = refp.X / 2 + mLeft / 2;
                refp.Y = refp.Y / 2 ;
            }
            else//左上角子图
            {
                refp.X = refp.X / 2;
                refp.Y = refp.Y / 2;
            }
            Ind_Row = (Ind_Row +1 +row) / 2  - row;
            Ind_Column = (Ind_Column +1+column) / 2  - column;
            
            if (MapPanel.Margin.Left + refp.X > 0)
            {
               MoveMargin = new Thickness(MapPanel.Margin.Left + refp.X + mLeft, MapPanel.Margin.Top, 0, 0);
                --Ind_Column;
            }
            else if (MapPanel.Margin.Left + refp.X < mLeft)
            {
                MoveMargin = new Thickness(MapPanel.Margin.Left + refp.X - mLeft, MapPanel.Margin.Top, 0, 0);
                ++Ind_Column;
            }
            else if (MapPanel.Margin.Top + refp.Y > 0)
            {
                MoveMargin = new Thickness(MapPanel.Margin.Left, MapPanel.Margin.Top + refp.Y + mTop, 0, 0);
                --Ind_Row;
            }
            else if (MapPanel.Margin.Top + refp.Y < mTop)
            {
                MoveMargin = new Thickness(MapPanel.Margin.Left, MapPanel.Margin.Top + refp.Y - mTop, 0, 0);
                ++Ind_Row;
            }
            else
                MoveMargin = new Thickness(MapPanel.Margin.Left + refp.X, MapPanel.Margin.Top + refp.Y, 0, 0);
            App.MoveRect = MoveMargin;                              //同步
            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column);          //更新地图
            
        }

        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            MoveMargin = App.MoveRect = new Thickness(-0, -0, 0, 0);

            App.Cur_Level = detail.Levels[0];   //最初显示最小层级
            App.Cur_Level_Index = 0;
            Ind_Row = App.Cur_Level.FTile.Row + 1;
            Ind_Column = App.Cur_Level.FTile.Column - 1;
            App.StrokeThinkness = 1;

            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column);
        }

        public override void OnMouseUp(object sender, MouseButtonEventArgs e)           //鼠标释放事件
        {
            if(IsViewMove)
            {
                FrameworkElement ele = sender as FrameworkElement;
                IsMousedown = false;
                ele.ReleaseMouseCapture();
               
                //更新地图

            }
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)               //鼠标移动事件
        {
            if (!IsViewMove||!IsMousedown)
                return;
            Grid currEle = this.MapPanel;
            double xPos = e.GetPosition(null).X - Movepos.X + currEle.Margin.Left;
            double yPos = e.GetPosition(null).Y - Movepos.Y + currEle.Margin.Top;
            
            App.MoveRect = new Thickness(e.GetPosition(null).X - Movepos.X + App.MoveRect.Left,
               e.GetPosition(null).Y - Movepos.Y + App.MoveRect.Top, 0, 0);
            if(xPos>0)
            {
                MoveMargin = new Thickness(xPos + mLeft, yPos, 0, 0);
                UpdateMap(App.Cur_Level, Ind_Row, --Ind_Column);
            }
            else if (xPos < mLeft)
            {
                MoveMargin = new Thickness(xPos - mLeft, yPos, 0, 0);
                UpdateMap(App.Cur_Level, Ind_Row, ++Ind_Column);
            }
            else if (yPos > 0)
            {
                MoveMargin = new Thickness(xPos, yPos + mTop, 0, 0);
                UpdateMap(App.Cur_Level, --Ind_Row, Ind_Column);
            }
            else if (yPos < mTop)
            {
                MoveMargin = new Thickness(xPos, yPos - mTop, 0, 0);
                UpdateMap(App.Cur_Level, ++Ind_Row, Ind_Column);
            }
            else
            {
                currEle.Margin = new Thickness(xPos, yPos, 0, 0); 
            }
            Movepos = e.GetPosition(null);
        }

        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)     //鼠标左键按下事件
        {
            if (IsViewMove)                                                             //判断是否是移动事件
            {
                IsMousedown = true;
            }
            else
            {
                Point p = e.GetPosition(MapPanel);
                int Column = (int)p.X / 256;
                int Row = (int)p.Y / 256;
                if (Column > 0)
                    refp.X = p.X- 256 * (Column );
                else
                    refp.X = p.X;
                if (Row > 0)
                    refp.Y = p.Y - 256 * (Row );
                else
                    refp.Y = p.Y;
                if (IsZoomIn)
                {
                    ProcessZoomIn(Row, Column);
                }
                if (IsZoomOut)
                    ProcessZoomOut(Row , Column);
                return; 
            }
            FrameworkElement fEle = sender as FrameworkElement;
            Movepos = e.GetPosition(null);
            fEle.CaptureMouse();
        }
        public override void SetOperationMode(int mode)
        {
            
        }
    }
}