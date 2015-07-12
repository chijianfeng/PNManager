using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
using DBCtrl.DBRW;
using PipeNetManager.utils;
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

        private CPipeInfo mPipeInfo;
        private CPipeExtInfo mPipeExtInfo;

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

        public bool DoSave()
        {
            ObservableCollection<Mesage> datas = (ObservableCollection<Mesage>)DG1.DataContext;
            foreach (Mesage msg in datas)
            {
                try
                {
                    ParseData(msg);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                    string str = "";
                    if (ex is ExceptionProcess)
                    {
                        str += ((ExceptionProcess)ex).getReson() + "\n";
                    }
                    MessageBox.Show(str + "设置数据格式错误：" + msg.ItemName + "  " + msg.ValueName);
                    return false;
                }
            }
            //此处直接调用数据库操作接口，绕过中间层
            TPipeInfo pipeinfo = new TPipeInfo(App._dbpath, App.PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(App._dbpath, App.PassWord);

            List<CPipeInfo> pipelist = new List<CPipeInfo>();
            pipelist.Add(mPipeInfo);

            List<CPipeExtInfo> extlist = new List<CPipeExtInfo>();
            extlist.Add(mPipeExtInfo);
            bool b1 = pipeinfo.Update_PipeInfo(pipelist);
            bool b2 = pipextinfo.Update_PipeExtInfo(extlist);
            return b1&b2;
        }

        private void ParseData(Mesage msg)
        {
            if (msg.ItemName.Equals("排水管标识码"))
            {
                mPipeInfo.PipeName = msg.ValueName;
            }
            else if (msg.ItemName.Equals("排水系统编码/路名"))
            {
                mPipeExtInfo.Lane_Way = msg.ValueName;
            }
            else if (msg.ItemName.Equals("管道类别"))
            {
                mPipeInfo.Pipe_Category = GetCategory(msg.ValueName);
            }
            else if (msg.ItemName.Equals("起点编码"))
            {
                mPipeInfo.In_JunID = GetJuncId(msg.ValueName);
            }
            else if (msg.ItemName.Equals("终点编码"))
            {
                mPipeInfo.Out_JunID = GetJuncId(msg.ValueName);
            }
            else if (msg.ItemName.Equals("起点管顶标高"))
            {
                mPipeInfo.In_UpEle = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("起点管底标高"))
            {
                mPipeInfo.In_BottomEle = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("终点管顶标高"))
            {
                mPipeInfo.Out_UpEle = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("终点管底标高"))
            {
                mPipeInfo.Out_BottomEle = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("起点实测管径"))
            {
                mPipeInfo.Shape_Data1 = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("终点实测管径"))
            {
                mPipeInfo.Shape_Data2 = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("断面形式"))
            {
                mPipeInfo.ShapeType = GetShapeType(msg.ValueName);
            }
            else if (msg.ItemName.Equals("断面数据"))
            {
                mPipeInfo.ShapeData = msg.ValueName;
            }
            else if (msg.ItemName.Equals("管道材质"))
            {
                mPipeInfo.Material = Getmaterial(msg.ValueName);
            }
            else if (msg.ItemName.Equals("管顶糙率"))
            {
                mPipeInfo.Roughness = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据来源"))
            {
                mPipeInfo.DataSource = GetDataSource(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据获取时间"))
            {
                mPipeInfo.Record_Date = ValueConvert.str2time(msg.ValueName);
            }
            else if (msg.ItemName.Equals("填报单位"))
            {
                mPipeInfo.ReportDept = msg.ValueName;
            }
            else if (msg.ItemName.Equals("填报日期"))
            {
                mPipeInfo.ReportDate = ValueConvert.str2time(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据是否完整"))
            {
                mPipeExtInfo.DataIsFull = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据缺失原因"))
            {
                mPipeExtInfo.LoseReason = msg.ValueName;
            }
            else if (msg.ItemName.Equals("备注"))
            {
                mPipeExtInfo.Remark = msg.ValueName;
            }
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
            mPipeInfo  = pr.ListPipe.ElementAt(0);
            mPipeExtInfo = pr.ListPipeExt.ElementAt(0);


            Datas.Add(new Mesage { ItemName = "排水管标识码", ValueName = mPipeInfo.PipeName });
            Datas.Add(new Mesage { ItemName = "排水系统编码/路名", ValueName = mPipeExtInfo.Lane_Way });
            Datas.Add(new Mesage { ItemName = "管道类别", ValueName = GetCategoryName(mPipeInfo.Pipe_Category) });
            Datas.Add(new Mesage { ItemName = "起点编码", ValueName = GetJuncName(mPipeInfo.In_JunID) });
            Datas.Add(new Mesage { ItemName = "终点编码", ValueName = GetJuncName(mPipeInfo.Out_JunID) });
            Datas.Add(new Mesage { ItemName = "起点管顶标高", ValueName = Convert.ToString(mPipeInfo.In_UpEle) });
            Datas.Add(new Mesage { ItemName = "起点管底标高", ValueName = Convert.ToString(mPipeInfo.In_BottomEle) });
            Datas.Add(new Mesage { ItemName = "终点管顶标高", ValueName = Convert.ToString(mPipeInfo.Out_UpEle) });
            Datas.Add(new Mesage { ItemName = "终点管底标高", ValueName = Convert.ToString(mPipeInfo.Out_BottomEle) });
            Datas.Add(new Mesage { ItemName = "起点实测管径", ValueName = Convert.ToString(mPipeInfo.Shape_Data1) });
            Datas.Add(new Mesage { ItemName = "终点实测管径", ValueName = Convert.ToString(mPipeInfo.Shape_Data2) });
            Datas.Add(new Mesage { ItemName = "断面形式", ValueName = GetShapeType(mPipeInfo.ShapeType) });
            Datas.Add(new Mesage { ItemName = "断面数据", ValueName = mPipeInfo.ShapeData });
            Datas.Add(new Mesage { ItemName = "管道材质", ValueName = Getmaterial(mPipeInfo.Material) });
            Datas.Add(new Mesage { ItemName = "管顶糙率", ValueName = Convert.ToString(mPipeInfo.Roughness) });
            Datas.Add(new Mesage { ItemName = "数据来源", ValueName = GetDataSource(mPipeInfo.DataSource) });
            Datas.Add(new Mesage { ItemName = "数据获取时间", ValueName = Convert.ToString(mPipeInfo.Record_Date) });
            Datas.Add(new Mesage { ItemName = "填报单位", ValueName = mPipeInfo.ReportDept });
            Datas.Add(new Mesage { ItemName = "填报日期", ValueName = Convert.ToString(mPipeInfo.ReportDate) });
            Datas.Add(new Mesage { ItemName = "数据是否完整", ValueName = bool2str(mPipeExtInfo.DataIsFull) });
            Datas.Add(new Mesage { ItemName = "数据缺失原因", ValueName = mPipeExtInfo.LoseReason });
            Datas.Add(new Mesage { ItemName = "备注", ValueName = mPipeExtInfo.Remark });
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

        private int GetCategory(string name)
        {
            string[] pat = { @"雨水", @"污水", @"合流", @"其他" };
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 雨水 ， 污水 ，合流 ，其他");
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

        private int GetJuncId(string name)
        {
            JuncRev jr = new JuncRev();
            jr.JuncName = name;
            SelectCmd scmd = new SelectCmd();
            scmd.SetReceiver(jr);
            scmd.Execute();
            CJuncInfo ji;
            if (jr.ListJunc != null && jr.ListJunc.Count >= 0)
                ji = jr.ListJunc.ElementAt(0);
            else
                throw new ExceptionProcess("找不到对应检查井");
            return ji.ID;
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

        private int GetDataSource(String name)
        {
            string[] pat = { @"设计图", @"竣工图", @"现场测绘", @"人工估计", @"其他" };
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 设计图 ， 竣工图 ，现场测绘 ，人工估计, 其他");
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

        private int Getmaterial(string name) 
        {
            string[] pat = { @"混凝土管",@"钢筋混凝土管",@"陶土管",
                             @"PE（聚乙烯）管",@"HDPE(高密度聚乙烯）管",
                            @"UPVC管",@"铸铁管",@"玻璃钢夹沙管",@"钢管",@"石棉水泥管" ,@"其他"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 混凝土管 ,钢筋混凝土管 ，陶土管 ，PE（聚乙烯）管,...,其他");
        }

        private string GetShapeType(int i)
        {
            i = i - 1;
            string[] pat = { @"圆形", @"梯形", @"三角形", @"椭圆形", @"矩形", @"不规则形状" };
            if (i < 0 || i >= pat.Count())
                return "其他";
            return pat[i];
        }

        private int GetShapeType(string name)
        {
            string[] pat = { @"圆形", @"梯形", @"三角形", @"椭圆形", @"矩形", @"不规则形状", @"其他" };
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 圆形 ,梯形 ，三角形 ，椭圆形,矩形,不规则形状,其他");
        }
    }
}
