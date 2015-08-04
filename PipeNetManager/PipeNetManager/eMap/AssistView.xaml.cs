using PipeNetManager.utils.Assist;
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

namespace PipeNetManager.eMap
{
    /// <summary>
    /// AssistView.xaml 实现长度测量，面积测量的呈现效果
    /// </summary>
    public partial class AssistView : BaseControl
    {
        private BaseAction mAction;
        public AssistView()
        {
            InitializeComponent();
        }

        public AssistView(BaseAction action)
        {
            InitializeComponent();
            mAction = action;
            if(mAction!=null)
                mAction.setCanvas(this.AssistCanvas);
        }

        public void SetAction(BaseAction action)
        {
            if (action == null) return;
            mAction = action;
            mAction.setCanvas(this.AssistCanvas);
        }

        public BaseAction GetAction()
        {
            return mAction;
        }

        public override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mAction == null) return;
            mAction.OnMouseUp(sender, e);
        }

        public override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mAction == null) return;
            mAction.OnMouseMove(sender, e);
        }

        public override void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (mAction == null) return;
            mAction.OnMouseLeftDown(sender, e);
        }

        public override void OnViewOriginal(object sender, RoutedEventArgs e)
        {
            if (mAction == null) return;
            mAction.OnViewOriginal(sender, e);
        }

        public  override void OnMouseRightDown(object sender, MouseButtonEventArgs e)
        {
            if (mAction == null) return;
            mAction.OnMouseRightDown(sender, e);
        }

        public override void SetOperationMode(int mode)
        {
           
        }


    }
}
