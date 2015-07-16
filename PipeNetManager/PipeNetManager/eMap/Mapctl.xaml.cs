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
using System.Runtime.InteropServices;
using System.Windows.Resources;
using GIS.Map;
using PipeMessage.eMap;
using PipeNetManager.Login;
using System.IO;

namespace PipeNetManager.eMap
{
    /// <summary>
    /// Mapctl.xaml 的交互逻辑
    /// </summary>
    public partial class Mapctl : UserControl
    {
        private RainJuncs mRainjunc;
        private WasteJuncs mWastejunc;
        private RainPipes mRainpipe;
        private WastePipes mWastepipe;
        private MapBackground mBackground;
        public Mapctl()
        {
            InitializeComponent();
            TextState.Loaded += new RoutedEventHandler(textLoaded);
            mBackground = new MapBackground();                     //创建地图背景图层
            mRainjunc = new RainJuncs();
            mWastejunc = new WasteJuncs();
            mRainpipe = new RainPipes(mRainjunc);
            mWastepipe = new WastePipes(mWastejunc);
        }

        //初始化数据
        public void InitBackground() {

            if (mBackground != null) {
                mBackground.InitBackGroundMapGrid();
            }
        }

        public void InitRainJuncState()
        {
            if (mRainjunc != null) {
                mRainjunc.InitRainJuncs();
            }
        }

        public void InitWasteJuncState()
        {
            if (mWastejunc != null) {
                mWastejunc.InitWasteJuncs();
            }
        }

        public void InitRainpipeState()
        {
            if (mRainpipe != null) {
                mRainpipe.InitPipes();
            }
        }

        public void InitWastepipeState() {
            if (mWastepipe != null) {
                mWastepipe.InitPipes();
            }
        }

        //创建实际的图层内容
        public void CreateContent() {
            mBackground.CreateContent();
            mRainjunc.AddJuncs();
            mRainpipe.AddPipes();
            mWastejunc.AddJuncs();
            mWastepipe.AddPipes();
        }

        private void textLoaded(object sender, RoutedEventArgs e)
        {
            TextState.Text = "丽水城南水阁区块排水管网健康查询系统      当前用户：" + AuthControl.getInstance().UserName 
                                + "         " + AuthControl.getInstance().getLoginTime();
            TextState.TextAlignment = TextAlignment.Center;
            //AddContent();
        }

        private List<BaseControl> listLayer = new List<BaseControl>();          //创建图层集合
        public void AddContent()
        {
            this.MapGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.MapGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.MapGrid.Children.Add(mBackground);
            listLayer.Add(mBackground);                                          //保存到图层中，便于管理

            this.MapGrid.Children.Add(mRainpipe);                                //添加雨水管道图层
            listLayer.Add(mRainpipe);


            this.MapGrid.Children.Add(mWastepipe);                              //添加污水管道图层
            listLayer.Add(mWastepipe);


            this.MapGrid.Children.Add(mRainjunc);                               //添加雨水检查井图层
            listLayer.Add(mRainjunc);


            this.MapGrid.Children.Add(mWastejunc);
            listLayer.Add(mWastejunc);

            //check authority
            EnableButton(AuthControl.getInstance().getAuth() == AuthControl.AUTH_ROOT);

        }

        private void EnableButton(bool b) {
            Button_Del.IsEnabled = b;
            Button_RainCover.IsEnabled = b;
            Button_RainPipe.IsEnabled = b;
            Button_WasteCover.IsEnabled = b;
            Button_WastePipe.IsEnabled = b;
        }

        //message
        private Cursor CreateCur(string path)
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri(path, UriKind.Relative));

            Cursor customCursor = new Cursor(sri.Stream);
            return customCursor;
        }
        /*
         * 移动消息处理
         */
        private void OnMoveMap(object sender, RoutedEventArgs e)
        {
            MapGrid.Cursor = CreateCur("/Assets/Move.cur");                    //表示移动状态
            View_ZoomIn.IsChecked = false;
            View_ZoomOut.IsChecked = false;
            VIew_Orignal.IsChecked = false;
            View_Select.IsChecked = false;
            View_Move.IsChecked = true;
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
        }

        private void OnSelect(object sender, RoutedEventArgs e)
        {
            MapGrid.Cursor = Cursors.Arrow;
            View_ZoomIn.IsChecked = false;
            View_ZoomOut.IsChecked = false;
            VIew_Orignal.IsChecked = false;
            View_Move.IsChecked = false;
            View_Select.IsChecked = true;
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
        }
        //放大操作
        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            MapGrid.Cursor = CreateCur("/Assets/zoomin.cur");
            //其他选项变成非选中
            View_Move.IsChecked = false;
            View_ZoomOut.IsChecked = false;
            VIew_Orignal.IsChecked = false;
            View_Select.IsChecked = false;
            View_ZoomIn.IsChecked = true;
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
        }

        //缩小操作
        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            //修改鼠标状态
            MapGrid.Cursor = CreateCur("/Assets/zoomout.cur");
            //其他选项变成非选中
            View_Move.IsChecked = false;
            View_ZoomIn.IsChecked = false;
            VIew_Orignal.IsChecked = false;
            View_Select.IsChecked = false;
            View_ZoomOut.IsChecked = true;
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
        }

        //是否显示雨水检查井
        private void View_Show_Rainjunc_Click(object sender, RoutedEventArgs e)
        {
            int index = 3;
            if(View_Show_Rainjunc.IsChecked)            //选中
            {
                View_Show_Rainjunc.IsChecked = false;
                this.MapGrid.Children[index+1].Visibility = Visibility.Collapsed;
                listLayer.ElementAt(index).IsHidden = true;
            }
            else
            {
                View_Show_Rainjunc.IsChecked = true;
                this.MapGrid.Children[index+1].Visibility = Visibility.Visible;
                listLayer.ElementAt(index).IsHidden = false;
                RainJuncs mJunc = listLayer.ElementAt(index) as RainJuncs;
                mJunc.Update();
            }
        }

        //是否显示污水检查井
        private void View_Show_Wastejunc_Click(object sender, RoutedEventArgs e)
        {
            int index = 4;
            if(View_Show_Wastejunc.IsChecked)
            {
                View_Show_Wastejunc.IsChecked = false;
                this.MapGrid.Children[index+1].Visibility = Visibility.Hidden;
                listLayer.ElementAt(index).IsHidden = true;
            }
            else
            {
                View_Show_Wastejunc.IsChecked = true;
                this.MapGrid.Children[index+1].Visibility = Visibility.Visible;
                listLayer.ElementAt(index).IsHidden = false;
                WasteJuncs mJunc = listLayer.ElementAt(index) as WasteJuncs;
                mJunc.Update();
            }
        }
        //是否显示雨水管道图层
        private void View_Show_Rainpipe_Click(object sender, RoutedEventArgs e)
        {
            int index = 1;
            if(View_Show_Rainpipe.IsChecked)
            {
                View_Show_Rainpipe.IsChecked = false;
                this.MapGrid.Children[index+1].Visibility = Visibility.Hidden;
                listLayer.ElementAt(index).IsHidden = true;
            }
            else
            {
                View_Show_Rainpipe.IsChecked = true;
                this.MapGrid.Children[index+1].Visibility = Visibility.Visible;
                listLayer.ElementAt(index).IsHidden = false;
                RainPipes pipes = listLayer.ElementAt(index) as RainPipes;
                pipes.Update();
            }
        }
        //是否显示污水管道图层
        private void View_Show_Wastepipe_Click(object sender, RoutedEventArgs e)
        {
            int index = 2;
            if(View_Show_Wastepipe.IsChecked)
            {
                View_Show_Wastepipe.IsChecked = false;
                this.MapGrid.Children[index + 1].Visibility = Visibility.Hidden;
                listLayer.ElementAt(index).IsHidden = true;
            }
            else
            {
                View_Show_Wastepipe.IsChecked = true;
                this.MapGrid.Children[index + 1].Visibility = Visibility.Visible;
                listLayer.ElementAt(index).IsHidden = false;
                WastePipes pipes = listLayer.ElementAt(index) as WastePipes;
                pipes.Update();
            }
        }

        //内部消息事件，适用于各个图层
        private void MapGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in listLayer)                                    //每个图层接收消息
            {
                item.OnMouseUp(sender, e);
            }
        }

        private void MapGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in listLayer)                                    //每个图层接收消息
            {
               // if(!item.IsHidden)
                item.OnMouseLeftDown(sender, e);
            }
        }

        private void MapGrid_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var item in listLayer)                                    //每个图层接收消息
            {
               // if (!item.IsHidden)
                item.OnMouseMove(sender, e);
            }
            //更新信息
            Point p = e.GetPosition(MapGrid);
            if (p.X < 0)
                return;
            int Column = (int)p.X / 256;
            int Row = (int)p.Y / 256;

            double dx = p.X - Column * 256;
            double dy = p.Y - Row * 256;

            Tile tile = App.Tiles[Row * Level.Total_Column + Column];
            double x = tile.X + tile.Dx * dx;
            double y = tile.Y - tile.Dy * dy;
            Coords.Point point;
            point.x = x; point.y = y;
            Coords.Point mp = Coords.Mercator2WGS84(point);
            String Msg = "";
            Msg += "经纬度坐标：\nX:" + mp.x.ToString("0.00000000") + "\nY:" + mp.y.ToString("0.00000000") + "\n";
            Lbl_Detail.Content = Msg;
        }

        //显示原始尺寸
        private void VIew_Orignal_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listLayer)                                    //每个图层接收消息
            {
                //if (!item.IsHidden)
                item.OnViewOriginal(sender, e);
            }
            MapGrid.Cursor = Cursors.Arrow;
            View_Move.IsChecked = false;
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }

        //添加雨水检查井
        private void Edit_Add_RainJunc_Click(object sender, RoutedEventArgs e)
        {
            int index = 3;
            if(!View_Show_Rainjunc.IsChecked)            //未选中
            {
                MessageBox.Show("雨水检查井图层为隐藏，无法操作");
                return;
            }
            MapGrid.Cursor = CreateCur("/Assets/add.cur");                    //表示添加状态
            for (int i = 0; i < listLayer.Count;i++ )
            {
                if (i == index)
                    continue;
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
            listLayer.ElementAt(index).SetOperationMode(IState.ADDMODE);      //进入添加状态
            View_Move.IsChecked = false;                                      //其他状态变为不可用
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }

        //添加污水检查井
        private void Edit_Add_WasteJunc_Click(object sender, RoutedEventArgs e)
        {
            int index = 4;
            if(!View_Show_Wastejunc.IsChecked)
            {
                MessageBox.Show("污水检查井图层为隐藏，无法操作");
                return;
            }
            MapGrid.Cursor = CreateCur("/Assets/add.cur");                    //表示添加状态
            for (int i = 0; i < listLayer.Count; i++)
            {
                if (i == index)
                    continue;
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
            listLayer.ElementAt(index).SetOperationMode(IState.ADDMODE);      //进入添加状态
            View_Move.IsChecked = false;                                      //其他状态变为不可用
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }

        private void Edit_Add_WastePipe_Click(object sender, RoutedEventArgs e)
        {
            int index = 2;
            if(!View_Show_Wastepipe.IsChecked)
            {
                MessageBox.Show("污水管道图层为隐藏，无法操作");
                return;
            }
            MapGrid.Cursor = CreateCur("/Assets/add.cur");                    //表示添加状态
            for (int i = 0; i < listLayer.Count; i++)
            {
                if (i == index)
                    continue;
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
            listLayer.ElementAt(index).SetOperationMode(IState.ADDMODE);      //进入添加状态
            View_Move.IsChecked = false;                                      //其他状态变为不可用
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }

        private void Edit_Add_RainPipe_Click(object sender, RoutedEventArgs e)
        {
            int index = 1;
            if (!View_Show_Rainpipe.IsChecked)
            {
                MessageBox.Show("雨水管道图层为隐藏，无法操作");
                return;
            }
            MapGrid.Cursor = CreateCur("/Assets/add.cur");                    //表示添加状态
            for (int i = 0; i < listLayer.Count; i++)
            {
                if (i == index)
                    continue;
                listLayer.ElementAt(i).SetOperationMode(IState.SELECTMODE);   //进入选择状态
            }
            listLayer.ElementAt(index).SetOperationMode(IState.ADDMODE);      //进入添加状态
            View_Move.IsChecked = false;                                      //其他状态变为不可用
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Del_Click(object sender, RoutedEventArgs e)
        {
            MapGrid.Cursor = CreateCur("/Assets/Del.cur");                    //表示delete状态
            for (int i = 0; i < listLayer.Count; i++)
            {
                listLayer.ElementAt(i).SetOperationMode(IState.DELMODE);      //进入选择状态
            }
            View_Move.IsChecked = false;                                      //其他状态变为不可用
            View_ZoomOut.IsChecked = false;
            View_ZoomIn.IsChecked = false;
        }

        /// <summary>
        /// 调起导入数据window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_LoadData_Click(object sender, RoutedEventArgs e)
        {
            string loadpath = "/ExcelLoader.exe";
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + loadpath))
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + loadpath);
            }
            else
            {
                MessageBox.Show("无法启动导入数据程序");
            }
           
        }
    }
}
