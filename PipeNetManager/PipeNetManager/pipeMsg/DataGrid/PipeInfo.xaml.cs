using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PipeNetManager.pipeMsg.DataGrid
{

    public enum OrderStatus { None, New, Processing, Shipped, Received };

    public class Mesage
    {
        public string ItemName { get; set; }
        public string ValueName { get; set; }
    }

    /// <summary>
    /// PipeInfo.xaml 的交互逻辑
    /// </summary>
    public partial class PipeInfo : UserControl
    {
        private string mPipename;
        public PipeInfo(string name)
        {
            InitializeComponent();
            ShowContent(name);
        }

        public void ShowContent(string pipename)
        {
            if (pipename == null && pipename.Length <= 0)
            {
                MessageBox.Show("Load Data Failed!", "错误消息");
                return;
            }
            mPipename = pipename;
            ObservableCollection<Mesage> datas = GetData();
            DG1.DataContext = datas;
        }

        private ObservableCollection<Mesage> GetData()
        {
            var Datas = new ObservableCollection<Mesage>();

            SelectCmd scmd = new SelectCmd();
            PipeRev pr = new PipeRev();
            pr.PipeName = mPipename;
            scmd.SetReceiver(pr);
            scmd.Execute();
            if (pr.ListPipe == null || pr.ListPipe.Count <= 0)
                return null;
            CPipeInfo pi = pr.ListPipe.ElementAt(0);
            CPipeExtInfo pei = pr.ListPipeExt.ElementAt(0);


            Datas.Add(new Mesage { ItemName = "排水管标识码", ValueName = pi.PipeName });
            Datas.Add(new Mesage { ItemName = "排水系统编码/路名", ValueName = pei.Lane_Way });
            Datas.Add(new Mesage { ItemName = "管道类别", ValueName = GetCategoryName(pi.Pipe_Category) });
            Datas.Add(new Mesage { ItemName = "起点编码", ValueName = GetJuncName(pi.In_JunID) });
            Datas.Add(new Mesage { ItemName = "终点编码", ValueName = GetJuncName(pi.Out_JunID) });
            Datas.Add(new Mesage { ItemName = "起点管顶标高", ValueName = Convert.ToString(pi.In_UpEle) });
            Datas.Add(new Mesage { ItemName = "起点管底标高", ValueName = Convert.ToString(pi.In_BottomEle) });
            Datas.Add(new Mesage { ItemName = "终点管顶标高", ValueName = Convert.ToString(pi.Out_UpEle) });
            Datas.Add(new Mesage { ItemName = "终点管底标高", ValueName = Convert.ToString(pi.Out_BottomEle) });
            Datas.Add(new Mesage { ItemName = "起点实测管径", ValueName = Convert.ToString(pi.Shape_Data1) });
            Datas.Add(new Mesage { ItemName = "终点实测管径", ValueName = Convert.ToString(pi.Shape_Data2) });
            Datas.Add(new Mesage { ItemName = "断面形式", ValueName = GetShapeType(pi.ShapeType) });
            Datas.Add(new Mesage { ItemName = "断面数据", ValueName = pi.ShapeData });
            Datas.Add(new Mesage { ItemName = "管道材质", ValueName = Getmaterial(pi.Material) });
            Datas.Add(new Mesage { ItemName = "管顶糙率", ValueName = Convert.ToString(pi.Roughness) });
            Datas.Add(new Mesage { ItemName = "数据来源", ValueName = GetDataSource(pi.DataSource) });
            Datas.Add(new Mesage { ItemName = "数据获取时间", ValueName = Convert.ToString(pi.Record_Date) });
            Datas.Add(new Mesage { ItemName = "填报单位", ValueName = pi.ReportDept });
            Datas.Add(new Mesage { ItemName = "填报日期", ValueName = Convert.ToString(pi.ReportDate) });
            Datas.Add(new Mesage { ItemName = "数据是否完整", ValueName = bool2str(pei.DataIsFull) });
            Datas.Add(new Mesage { ItemName = "数据缺失原因", ValueName = pei.LoseReason });
            Datas.Add(new Mesage { ItemName = "备注", ValueName = pei.Remark });
            return Datas;
        }

        private string GetCategoryName(int i)
        {
            i = i - 1;
            string[] pat = { @"雨水", @"污水", @"合流" };
            if (i < 0 || i > 2)
                return "其他";
            else
                return pat[i];
        }

        private string GetType(int i)
        {
            i = i - 1;
            string[] pat = { @"排水井", @"接户井", @"闸阀井", @"溢流井", @"倒虹井", @"透气井" ,
                           @"压力井", @"检测井", @"拍门井", @"截流井", @"水封井", @"跌水井" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            else
                return pat[i];
        }

        private string GetStyle(int i)
        {
            i = i - 1;
            string[] pat = { @"一通",@"二通直",@"二通转",
                             @"三通",@"四通",@"五通",@"五通以上",@"暗井",@"侧立型",@"平面型",@"出水口" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            else
                return pat[i];
        }

        private string GetJuncName(int id)
        {
            JuncRev jr = new JuncRev();
            jr.ID = id;
            SelectCmd scmd = new SelectCmd();
            scmd.SetReceiver(jr);
            scmd.Execute();
            CJuncInfo ji;
            if (jr.ListJunc != null && jr.ListJunc.Count >= 0)
                ji = jr.ListJunc.ElementAt(0);
            else
                return "";
            return ji.JuncName;
        }

        private string bool2str(bool b)
        {
            if (b)
                return "是";
            else
                return "否";
        }

        private string GetDataSource(int i)
        {
            i = i - 1;
            string[] pat = { @"设计图", @"竣工图", @"现场测绘", @"人工估计" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            return pat[i];
        }

        private string Getmaterial(int i)
        {
            i = i - 1;
            string[] pat = { @"混凝土管",@"钢筋混凝土管",@"陶土管",
                             @"PE（聚乙烯）管",@"HDPE(高密度聚乙烯）管",
                            @"UPVC管",@"铸铁管",@"玻璃钢夹沙管",@"钢管",@"石棉水泥管" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            return pat[i];
        }

        private string GetShapeType(int i)
        {
            i = i - 1;
            string[] pat = { @"圆形", @"梯形", @"三角形", @"椭圆形", @"矩形", @"不规则形状" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            return pat[i];
        }
    }
}
