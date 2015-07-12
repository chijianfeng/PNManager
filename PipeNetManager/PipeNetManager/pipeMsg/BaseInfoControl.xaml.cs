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
        private PipeInfo mPipeInfo = null;
        private USInfo   mUsInfo = null;

        private Callback mCB;
        public BaseInfoControl(string name , Callback cb)
        {
            InitializeComponent();
            mPipeName = name;
            ShowContent(name);
            mCB = cb;
        }

        private void ShowContent(string name)
        {
            this.stackpanel.Children.Clear();
            if (mPipeInfo == null) {
                mPipeInfo = new PipeInfo(name);
            }
            this.stackpanel.Children.Add(mPipeInfo);
        }

        private void USInfo(object sender, RoutedEventArgs e)           //内窥数据
        {
            this.stackpanel.Children.Clear();
            if (mUsInfo == null)
            {
                mUsInfo = new USInfo(mPipeName);
            }
            this.stackpanel.Children.Add(mUsInfo);
        }

        private void BaseInfo(object sender, RoutedEventArgs e)         //管道基本数据
        {
            this.stackpanel.Children.Clear();
            if (mPipeInfo == null)
            {
                mPipeInfo = new PipeInfo(mPipeName);
            }
            this.stackpanel.Children.Add(mPipeInfo);
        }

        private string mPipeName;

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (mPipeInfo != null && mPipeInfo.DoSave() || (mUsInfo != null && mUsInfo.DoSave()))
            {
                MessageBox.Show("保存成功");
                if (mCB != null)
                {
                    mCB.CloseWindow();
                }
            }
            else
            {
                MessageBox.Show("保存失败！");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
