using ExcelOper;
using ExcelOper.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelLoader
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            loadprogress.Visibility = Visibility.Hidden;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "打开Excel文件";
            open.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.xls";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog() == true)
            {
                excelpath.Text = open.FileName;
                //read the excel basic imformation
                ExcelReader reader = new ExcelReader(open.FileName);
                foreach (SheetInfo info in reader.GetSheetlist())
                {
                    sheetlist.Add(info);
                }
                sheetcmb.ItemsSource = sheetlist;
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            if (selectsheet == null)
                return;
            Loadbutton.IsEnabled = false;
            loadprogress.Visibility = Visibility.Visible;
            reader = new ExcelReader(excelpath.Text);
            selectsheet.StartRange = Sr.Text;
            selectsheet.EndRange = Er.Text;

            backworkthread = new BackgroundWorker();
            backworkthread.WorkerSupportsCancellation = true;
            backworkthread.DoWork += new DoWorkEventHandler(longtask);
            backworkthread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LongtaskComplete);
            backworkthread.RunWorkerAsync();
        }

        private void longtask(object sender, DoWorkEventArgs e)
        {
            DataSet ds = reader.Read(selectsheet.SheetName);

            ExcelToDB edb = new ExcelToDB(ds);
            edb.Sr = selectsheet.StartRange;
            edb.Er = selectsheet.EndRange;
            edb.DataType = DataType;
            edb.LoadtoDB();
        }

        private void LongtaskComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            loadprogress.Visibility = Visibility.Hidden;
            Loadbutton.IsEnabled = true;
            MessageBox.Show("导入数据成功", "消息");
            return;
        }
        private void sheetcmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sheetcmb.SelectedItem == null)
                return;
            int index = sheetcmb.SelectedIndex;
            if (index >= 0)
                selectsheet = sheetlist.ElementAt(index);
            else
                return;
            Sr.Text = selectsheet.StartRange;
            Er.Text = selectsheet.EndRange;
        }

        private BackgroundWorker backworkthread;
        private Sheetlist sheetlist = new Sheetlist();
        private SheetInfo selectsheet = null;
        private ExcelReader reader;
        private _DataType DataType = 0;
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            DataType = _DataType.TYPE_JUNCINFO;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            DataType = _DataType.TYPE_PIPEINFO;
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            DataType = _DataType.TYPE_USINFO;
        }
    }
    enum _DataType
    {
        //检查井类型
        TYPE_JUNCINFO = 1,
        //管道类型
        TYPE_PIPEINFO = 2,
        //内窥类型
        TYPE_USINFO = 3
    }
}
