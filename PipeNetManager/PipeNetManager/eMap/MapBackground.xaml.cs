using GIS.Map;
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
        public MapBackground()
            : base()
        {
            InitializeComponent();

            MapPanel.Margin = new Thickness(-0, -0, 0, 0);                          //初始布局
        }
        // 加载器
        Loader loader = new Loader();                //瓦片图加载器
        // 地图详情
        Detail detail = new Detail();
        // 背景Image对象
        List<Image> Images = new List<Image>();

        //默认地图，用于加载过程中间状态
        BitmapImage mDefaultImg = new BitmapImage();


        Point Movepos = new Point();                 //移动点

        int Ind_Row;
        int Ind_Column;

        bool IsMousedown = false;

        double mTop = -256;                           //地图偏移距离
        double mLeft = -256;

        Point refp = new Point();                     //用于缩放偏移参考点

        Thickness MoveMargin;

        MemoryStream[] mMaps = new MemoryStream[64];  //background map cache
        MemoryStream mDefalutcache = null;

        public void InitBackGroundMapGrid()
        {
            detail = loader.LoadMapDetail();        //加载地图基本信息
            App.Cur_Level = detail.Levels[0];       //最初显示最小层级
            App.Cur_Level_Index = 0;

                                                     //最初显示地图位置
            Ind_Row = App.Cur_Level.FTile.Row + 1;   //最初显示地图
            Ind_Column = App.Cur_Level.FTile.Column - 1;
            App.Tiles = App.Cur_Level.GetTiles_M(Ind_Row, Ind_Column);

            App.TotalLevels = detail.Level_Count;    //总层级数

            String Abs_File_Name = System.IO.Path.GetFullPath(Level.GetDefault_Path());
            byte[] bs = File.ReadAllBytes(Abs_File_Name);
            mDefalutcache = new MemoryStream(bs);

            //load bitmap to cache
            for (int i = 0; i < Level.Total_Column* Level.Total_Row; i++)
            {
                Abs_File_Name = System.IO.Path.GetFullPath(App.Tiles[i].Filename);
                byte[] bts = File.ReadAllBytes(Abs_File_Name);
                MemoryStream img = new MemoryStream(bts);
                mMaps[i] = img;
            }
        }

        //创建实际地图图层
        public void CreateContent()
        {
            initBgMap();
        }

        void initBgMap()
        {
            mDefaultImg.BeginInit();
            mDefaultImg.StreamSource = mDefalutcache;
            mDefaultImg.EndInit();

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
            for (int i = 0; i < Images.Count; i++)                                  //同步显示到界面上
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = mMaps[i];
                img.EndInit();
                Images[i].Source = img;
            }
        }

        private void update(Level lvl, int x, int y) {

            App.Tiles = lvl.GetTiles_M(x, y);
            //进行后台地图加载，利用Task Parallel 进行并行加载，.net 4.0以上版本支持
            Task.Factory.StartNew<MemoryStream[]>((Obj) =>
            {
                Parallel.For(0, (int)Obj, i =>
                {
                    String Abs_File_Name = System.IO.Path.GetFullPath(App.Tiles[i].Filename);
                    byte[] bs = File.ReadAllBytes(Abs_File_Name);
                    mMaps[i] = new MemoryStream(bs);
                });
                return mMaps;
            }, Images.Count).ContinueWith(ant =>
            {
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

        /// <summary>
        /// update the map
        /// </summary>
        /// <param name="lvl">显示地图层级</param>
        /// <param name="x">左上角显示的第一行</param>
        /// <param name="y">左上角显示的第一列</param>
        /// <param name="dirx">偏移x方向位置</param>
        /// <param name="diry">偏移y方向位置</param>
        private void UpdateMap(Level lvl, int x, int y, int c, int r)
        {
            if (c == 0 && r == 0)
            {
                update(lvl, x, y);
                return;
            }
            int i = 0;
            int j = 0;
            int R = r * -1;
            int C = c * -1;
            if (c <= 0 && r < 0)
            {          //move from right-bottom to top-left
                for (i = 0; i < Level.Total_Row; i++)
                {
                    for (j = 0; j < Level.Total_Column; j++)
                    {
                        if (i + R < Level.Total_Row && j + C < Level.Total_Column)
                        {
                            Images[i * Level.Total_Column + j].Source = Images[(i + R) * Level.Total_Column + j + C].Source;
                        }
                        else
                        {
                            Images[i * Level.Total_Column + j].Source = loadSource(lvl, x + i, y + j);
                        }
                    }
                }
            }
            else if (c > 0 && r <= 0)
            {
                for (i = 0; i < Level.Total_Row; i++)
                {
                    for (j = Level.Total_Column - 1; j >= 0; j--)
                    {
                        if (C + j >= 0 && i + R < Level.Total_Row)
                        {
                            Images[i * Level.Total_Column + j].Source = Images[(i + R) * Level.Total_Column + j + C].Source;
                        }
                        else
                        {
                            Images[i * Level.Total_Column + j].Source = loadSource(lvl, x + i, y + j);
                        }
                    }
                }
            }
            else if (c >= 0 && r > 0)
            {
                for (i = Level.Total_Row - 1; i >= 0; i--)
                {
                    for (j = Level.Total_Column - 1; j >= 0; j--)
                    {
                        if (C + j >= 0 && i + R >= 0)
                        {
                            Images[i * Level.Total_Column + j].Source = Images[(i + R) * Level.Total_Column + j + C].Source;
                        }
                        else
                        {
                            Images[i * Level.Total_Column + j].Source = loadSource(lvl, x + i, y + j);
                        }
                    }
                }
            }
            else
            {
                for (i = Level.Total_Row - 1; i >= 0; i--)
                {
                    for (j = 0; j < Level.Total_Column; j++)
                    {
                        if (C + j < Level.Total_Column && i + R >= 0)
                        {
                            Images[i * Level.Total_Column + j].Source = Images[(i + R) * Level.Total_Column + j + C].Source;
                        }
                        else
                        {
                            Images[i * Level.Total_Column + j].Source = loadSource(lvl, x + i, y + j);
                        }
                    }
                }
            }
            MapPanel.Margin = MoveMargin;
        }

        private BitmapSource loadSource(Level lvl, int x, int y)
        {
            Tile tile = lvl.getTile(x, y);
            String Abs_File_Name = System.IO.Path.GetFullPath(tile.Filename);
            return new BitmapImage(new Uri(Abs_File_Name, UriKind.RelativeOrAbsolute));
        }

        private void ProcessZoomIn(int row, int column)
        {
            if (App.Cur_Level_Index + 1 > detail.Level_Count - 1)       //已经放大到最大级别，不能继续放大
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
            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column, 0, 0);              //启动更新地图
        }

        private void ProcessZoomOut(int row, int column)
        {
            if (App.Cur_Level_Index < 1)
            {
                MessageBox.Show("已到缩小到到最小级别！");
                return;
            }
            App.StrokeThinkness--;
            App.Cur_Level = detail.Levels[--App.Cur_Level_Index];
            if ((Ind_Row + row) % 2 == 0 && (Ind_Column + column) % 2 == 0)             //右下角子图
            {
                refp.X = -(-refp.X / 2 - mLeft / 2);
                refp.Y = -(-refp.Y / 2 - mTop / 2);
            }
            else if ((Ind_Row + row) % 2 == 0 && (Ind_Column + column) % 2 != 0)       //左下角子图
            {
                refp.X = refp.X / 2;
                refp.Y = mTop / 2 + refp.Y / 2;
            }
            else if ((Ind_Row + row) % 2 != 0 && (Ind_Column + column) % 2 == 0)       //右上角子图
            {
                refp.X = refp.X / 2 + mLeft / 2;
                refp.Y = refp.Y / 2;
            }
            else//左上角子图
            {
                refp.X = refp.X / 2;
                refp.Y = refp.Y / 2;
            }
            Ind_Row = (Ind_Row + 1 + row) / 2 - row;
            Ind_Column = (Ind_Column + 1 + column) / 2 - column;

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
            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column, 0, 0);          //更新地图

        }

        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            MoveMargin = App.MoveRect = new Thickness(-0, -0, 0, 0);

            App.Cur_Level = detail.Levels[0];   //最初显示最小层级
            App.Cur_Level_Index = 0;
            Ind_Row = App.Cur_Level.FTile.Row + 1;
            Ind_Column = App.Cur_Level.FTile.Column - 1;
            App.StrokeThinkness = 1;

            update(App.Cur_Level, Ind_Row, Ind_Column);
        }

        public override void OnMouseUp(object sender, MouseButtonEventArgs e)           //鼠标释放事件
        {
            if (IsViewMove)
            {
                FrameworkElement ele = sender as FrameworkElement;
                IsMousedown = false;
                ele.ReleaseMouseCapture();

                //更新地图

            }
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)               //鼠标移动事件
        {
            if (!IsViewMove || !IsMousedown)
                return;
            int dirx = 0;
            int diry = 0;
            Grid currEle = this.MapPanel;
            double xPos = e.GetPosition(null).X - Movepos.X + currEle.Margin.Left;
            double yPos = e.GetPosition(null).Y - Movepos.Y + currEle.Margin.Top;

            App.MoveRect = new Thickness(e.GetPosition(null).X - Movepos.X + App.MoveRect.Left,
               e.GetPosition(null).Y - Movepos.Y + App.MoveRect.Top, 0, 0);
            if (xPos > 0)
            {
                dirx = (int)(xPos / Math.Abs(mLeft)) + 1;
                MoveMargin = new Thickness(xPos + dirx * mLeft, yPos, 0, 0);
            }
            else if (xPos < mLeft)
            {
                dirx = (int)(xPos / Math.Abs(mLeft));
                MoveMargin = new Thickness(xPos + dirx * mLeft, yPos, 0, 0);
            }
            else if (yPos > 0)
            {
                diry = (int)(yPos / Math.Abs(mTop)) + 1;
                MoveMargin = new Thickness(xPos, yPos + diry * mTop, 0, 0);
            }
            else if (yPos < mTop)
            {
                diry = (int)(yPos / Math.Abs(mTop));
                MoveMargin = new Thickness(xPos, yPos + diry * mTop, 0, 0);
            }
            else
            {
                currEle.Margin = new Thickness(xPos, yPos, 0, 0);
                Movepos = e.GetPosition(null);
                return;
            }
            Ind_Column = Ind_Column - dirx;
            Ind_Row = Ind_Row - diry;
            UpdateMap(App.Cur_Level, Ind_Row, Ind_Column, dirx, diry);
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
                    refp.X = p.X - 256 * (Column);
                else
                    refp.X = p.X;
                if (Row > 0)
                    refp.Y = p.Y - 256 * (Row);
                else
                    refp.Y = p.Y;
                if (IsZoomIn)
                {
                    ProcessZoomIn(Row, Column);
                }
                if (IsZoomOut)
                    ProcessZoomOut(Row, Column);
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
