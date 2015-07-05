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
using System.Windows.Shapes;

namespace PipeNetManager.juncMsg
{
    /// <summary>
    /// JuncWindow.xaml 的交互逻辑
    /// </summary>
    public partial class JuncWindow : Window
    {
        public JuncWindow(string name)
        {
            InitializeComponent();
            this.Junc_Name.Text = "\t" + name + "检查井信息";
            BaseContent basic = new BaseContent(name);
            this.Stackpanel1.Children.Add(basic);
        }


    }
}
