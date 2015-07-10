using BLL.Command;
using BLL.Receiver;
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

namespace PipeNetManager.Login
{
    /// <summary>
    /// LoginWnd.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWnd : UserControl
    {
        SelectCmd scmd = new SelectCmd();
        public LoginWnd()
        {
            InitializeComponent();
            Box_Username.Loaded += new RoutedEventHandler(txtbox_Loaded);
        }

        private void txtbox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;
            txtbox.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (Box_Username.Text == null || Box_Username.Text == "")
            {
                MessageBox.Show("请输入用户名", "消息");
                return;
            }
            if (Box_Passwd.Password == null || Box_Passwd.Password == "")
            {
                MessageBox.Show("请输入密码！", "消息");
                return;
            }
            String usr = Box_Username.Text;
            String pwd = Box_Passwd.Password;

            UserRev uRev = new UserRev();
            uRev.UserName = usr;

            scmd.SetReceiver(uRev);
            scmd.Execute();
            if (uRev.ListUser.Count > 0 && uRev.ListUser[0].PassWord == pwd)
            {
                AuthControl.getInstance().setAuth(uRev.ListUser[0].UserType);
                AuthControl.getInstance().UserName = uRev.ListUser[0].UserName;

                App currentApp = (App)Application.Current;
                RoutedEventArgs newEventArgs = new RoutedEventArgs(Button.ClickEvent);
                newEventArgs.Source = this;
                currentApp.MainWindow.RaiseEvent(newEventArgs);
            }
            else
            {
                MessageBox.Show("登陆失败", "消息");

            }
        }
    }
}
