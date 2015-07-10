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
    /// PicControl.xaml 的交互逻辑
    /// </summary>
    public partial class PicControl : UserControl
    {
        private List<string> mListPath;
        public PicControl(List<string> listPath)
        {
            InitializeComponent();
            mListPath = listPath;
            ShowContent();
        }

        public PicControl(string name)
        {
            InitializeComponent();
            //add code for read database 
        }

        public void ShowContent()
        {
            if (mListPath == null || mListPath.Count <= 0) return;
            string picpath = mListPath.ElementAt(0);
            SetPic(picpath);
        }

        /// <summary>
        /// 设置图片位置，并进行显示
        /// </summary>
        /// <param name="path"></param>
        private void SetPic(string path)
        {
            if (path == null || path.Length < 0)
                return;

            ImageName.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }     
    }
}
