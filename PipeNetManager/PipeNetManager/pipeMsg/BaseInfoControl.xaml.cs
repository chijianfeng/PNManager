using PipeNetManager.pipeMsg.DataGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PipeNetManager.pipeMsg
{
    /// <summary>
    /// BaseInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class BaseInfoControl : UserControl
    {
        public BaseInfoControl(string name)
        {
            InitializeComponent();
            mPipeName = name;
            ShowContent(name);
        }

        private void ShowContent(string name)
        {
            this.stackpanel.Children.Clear();
            PipeInfo info = new PipeInfo(name);
            this.stackpanel.Children.Add(info);
        }

        private void USInfo(object sender, RoutedEventArgs e)           //内窥数据
        {
            this.stackpanel.Children.Clear();
            USInfo info = new USInfo(mPipeName);
            this.stackpanel.Children.Add(info);
        }

        private void BaseInfo(object sender, RoutedEventArgs e)         //管道基本数据
        {
            this.stackpanel.Children.Clear();
            PipeInfo info = new PipeInfo(mPipeName);
            this.stackpanel.Children.Add(info);
        }

        private string mPipeName;
    }
}
