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

namespace PipeNetManager.pipeMsg
{
    /// <summary>
    /// PipeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PipeWindow : Window
    {
        public PipeWindow(string pipename)
        {
            InitializeComponent();
            if (pipename == null || pipename.Length <= 0) return;
            this.Title = pipename + "管道信息";

            BaseInfoControl infopage = new BaseInfoControl(pipename , new Notify(this));
            this.basicmsgpanel.Children.Add(infopage);

            PicControl picpage = new PicControl(pipename);
            this.picpanel.Children.Add(picpage);

            ReportControl reportpage = new ReportControl(pipename);
            this.reportpanel.Children.Add(reportpage);
            VideoControl videopage = new VideoControl(pipename);
            this.videopanel.Children.Add(videopage);
        }

        public class Notify : Callback
        {
            private PipeWindow mWnd;
            public Notify(PipeWindow wnd)
            {
                mWnd = wnd;
            }
            public void CloseWindow()
            {
                mWnd.Close();
            }
        }
        
    }
    public interface Callback
    {
         void CloseWindow();
    }
}
