using PipeNetManager.eMap;
using PipeNetManager.Login;
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

namespace PipeNetManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度

            double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度

            this.Width = x;//设置窗体宽度

            this.Height = y;//设置窗体高度

            LoginWnd loginwnd = new LoginWnd();
            this.stackpanl.Children.Add(loginwnd);
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(PageChange));
        }

        void PageChange(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.ToString().Equals("PipeNetManager.Login.LoginWnd"))
            {
                LoadWait wait = new LoadWait();
                this.stackpanl.Children.Clear();
                this.stackpanl.Children.Add(wait);
            }
            else if (e.OriginalSource.ToString().Equals("PipeNetManager.eMap.Mapctl"))
            {
                this.Grid1.Children.Remove(textBlock1);     //移除textblock

                Mapctl eMap = e.OriginalSource as Mapctl;
                eMap.AddContent();
                this.Grid1.Children.Remove(textBlock1);     //移除textblock
                this.stackpanl.Children.Clear();
                this.stackpanl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this.stackpanl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                this.stackpanl.Children.Add(eMap);
            }
        }
    }
}
