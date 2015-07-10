using System;
using System.Collections.Generic;
using System.IO;
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
    /// VideoControl.xaml 的交互逻辑
    /// </summary>
    public partial class VideoControl : UserControl
    {
        public VideoControl(List<string> videopath)
        {
            InitializeComponent();
            Player.IsEnabled = false;
            Stop.IsEnabled = false;
            mListpath = videopath;
        }

        public VideoControl(string pipename)
        {
            InitializeComponent();
            
        }

        public void ShowContent()
        {
            if (mListpath==null||mListpath.Count <= 0)
                return;

            string videopath = mListpath.ElementAt(0);
            bfirst = true;
            string basepath = System.AppDomain.CurrentDomain.BaseDirectory;

            if (videopath == null || videopath.Length <= 0 || !File.Exists(basepath + "\\" + videopath))
            {
                MessageBox.Show("加载视频出错！", "错误消息");
                return;
            }
            else
            {
                Player.IsEnabled = false;
                Stop.IsEnabled = true;
                if (bfirst)
                {
                    progress.Visibility = Visibility.Visible;
                    bfirst = false;
                }
                try
                {
                    timeline.Source = new Uri(videopath, UriKind.RelativeOrAbsolute);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "错误消息");
                    return;
                }
                timeline.BeginTime = new TimeSpan(0, 0, 0, 0);
                Mediapalyer.Volume = 1;
                Mediapalyer.Clock = timeline.CreateClock(true) as MediaClock;
                timeslider.Maximum = Mediapalyer.NaturalDuration.TimeSpan.TotalMilliseconds;
                storyboard.Stop(this);

            }
        }
        private void timeslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int slidervalue = (int)timeslider.Value;
            TimeSpan span = new TimeSpan(0, 0, 0, slidervalue);
            storyboard.SeekAlignedToLastTick(span);
        }

        private void Player_Click(object sender, RoutedEventArgs e)
        {
            timeline.BeginTime = new TimeSpan(0, 0, 0, 0);
            if (Mediapalyer.Clock.CurrentState == System.Windows.Media.Animation.ClockState.Stopped)
            {
                storyboard.Begin(this, true);
            }
            else if (Mediapalyer.Clock.CurrentGlobalSpeed == 0)
            {
                storyboard.Resume(this);
            }

            Player.IsEnabled = false;
            Stop.IsEnabled = true;
            timeslider.Value = 0;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            storyboard.Pause(this);
            Player.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        private void Mediapalyer_MediaOpened(object sender, RoutedEventArgs e)
        {
            progress.Visibility = Visibility.Hidden;
        }

        private void Mediapalyer_MediaEnded(object sender, RoutedEventArgs e)
        {
            timeslider.Value = 0;
            Player.IsEnabled = true;
            Stop.IsEnabled = false;
        }

        private void timeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (Mediapalyer.Clock.CurrentTime != null)
                timeslider.Value = Mediapalyer.Clock.CurrentTime.Value.TotalMinutes;
        }

        private List<string> mListpath;
        private bool bfirst = true;

    }
}
