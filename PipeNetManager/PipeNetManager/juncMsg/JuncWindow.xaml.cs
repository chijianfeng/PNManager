﻿using PipeNetManager.Login;
using PipeNetManager.utils;
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
        private BaseContent mBasic;
        private int p;
        public JuncWindow(string name)
        {
            InitializeComponent();
            this.Junc_Name.Text = "\t" + name + "检查井信息";
            mBasic = new BaseContent(name);
            this.Stackpanel1.Children.Add(mBasic);

            //check authority
            bool b = AuthControl.AUTH_ROOT == AuthControl.getInstance().getAuth();
            Button_Cancle.IsEnabled = b;
            Button_Save.IsEnabled = b;

            AnimationUtil.ScaleEasingAnimation(this);
        }

        public JuncWindow(int id)
        {
            InitializeComponent();
            this.Junc_Name.Text = "\t" + id + " 检查井信息";
            mBasic = new BaseContent(id);
            this.Stackpanel1.Children.Add(mBasic);

            //check authority
            bool b = AuthControl.AUTH_ROOT == AuthControl.getInstance().getAuth();
            Button_Cancle.IsEnabled = b;
            Button_Save.IsEnabled = b;

            AnimationUtil.ScaleEasingAnimation(this);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (mBasic.DoSave())
            {
                MessageBox.Show("修改成功");
                this.Close();
            }
        }

        private void Button_Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
