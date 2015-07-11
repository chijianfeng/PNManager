using PipeNetManager.eMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PipeNetManager.Login
{
    /// <summary>
    /// LoadWait.xaml 的交互逻辑
    /// </summary>
    public partial class LoadWait : UserControl
    {
        private Mapctl mMap;
        public LoadWait()
        {
            InitializeComponent();
            mMap = new Mapctl();
            Dowork();
        }

        private void Longtaskwork(object sender, DoWorkEventArgs e)                                 //invoke the long task 
        {
            if (((App)System.Windows.Application.Current).arcmap == null)
            {
                backworkthread.ReportProgress(0 ,"正在导入雨水检查井数据，0%");
                ((App)System.Windows.Application.Current).arcmap = new GIS.Arc.ArcMap();
                ((App)System.Windows.Application.Current).arcmap.LoadRainCover();
                mMap.InitRainJuncState();
                backworkthread.ReportProgress(25 , "正在导入污水检查井数据，25%");
                
                 ((App)System.Windows.Application.Current).arcmap.LoadWasterCover();
                 mMap.InitWasteJuncState();
                 backworkthread.ReportProgress(50, "正在导入雨水管道数据，50%");

                 ((App)System.Windows.Application.Current).arcmap.LoadRainPipe();
                 mMap.InitRainpipeState();
                 backworkthread.ReportProgress(75, "正在导入污水管道数据，75%");

                 ((App)System.Windows.Application.Current).arcmap.LoadWasterPipe();
                 mMap.InitWastepipeState();
                 backworkthread.ReportProgress(85, "导入数据完成，加载背景地图...85%");

                 mMap.InitBackground();
                 backworkthread.ReportProgress(99, "导入数据完成，正在初始化...99%");
            }
        }

        private void LongtaskComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            //NavigationCommands.GoToPage.Execute("/eMap/Page_Map.xaml", this);
            App currentApp = (App)Application.Current;
            RoutedEventArgs newEventArgs = new RoutedEventArgs(Button.ClickEvent);

            HintText.Text = "导入数据完成，正在初始化...99%";

            mMap.CreateContent();

            newEventArgs.Source = mMap;
            currentApp.MainWindow.RaiseEvent(newEventArgs);
        }

        private void Task_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e) {
            HintText.Text = e.UserState.ToString();
        }

        private void Dowork()
        {
            backworkthread = new BackgroundWorker();
            backworkthread.WorkerSupportsCancellation = true;
            backworkthread.DoWork += new DoWorkEventHandler(Longtaskwork);
            backworkthread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LongtaskComplete);
            backworkthread.ProgressChanged += Task_ProgressChanged;
            backworkthread.WorkerReportsProgress = true;
            backworkthread.RunWorkerAsync();
        }

        private BackgroundWorker backworkthread;
    }
}
