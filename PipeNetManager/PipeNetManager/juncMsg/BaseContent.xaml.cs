using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
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

namespace PipeNetManager.juncMsg
{
    /// <summary>
    /// BaseContent.xaml 的交互逻辑
    /// </summary>
    public partial class BaseContent : UserControl
    {
        private CJuncInfo mJunc;
        private CJuncExtInfo mJuncext;

        public class JuncMesage
        {
            public string ItemName { get; set; }
            public string ValueName { get; set; }
        }

        public BaseContent(string juncname)
        {
            InitializeComponent();

            if (juncname == null || juncname.Length <= 0)
            {
                MessageBox.Show("Load Data Failed!", "错误消息");
                return;
            }
            ObservableCollection<JuncMesage> datas = GetData(juncname);
            JUNCDG.DataContext = datas;
        }

        public bool DoSave()
        {
            ObservableCollection<JuncMesage> datas = (ObservableCollection<JuncMesage>)JUNCDG.DataContext;
            foreach (JuncMesage msg in datas)
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
            UpdateCmd cmd = new UpdateCmd();
            JuncRev rev = new JuncRev();
            rev.ListJunc = new List<CJuncInfo>();
            rev.ListJuncExt = new List<CJuncExtInfo>();

            rev.ListJunc.Add(mJunc);
            rev.ListJuncExt.Add(mJuncext);

            cmd.SetReceiver(rev);
            cmd.Execute();


            return true;
        }

        private void ParseData(JuncMesage msg)
        {
            if (msg.ItemName.Equals("井盖名称"))
            {
                mJunc.JuncName = msg.ValueName;
            }
            else if (msg.ItemName.Equals("道路名称"))
            {
                mJuncext.Lane_Way = msg.ValueName;
            }
            else if (msg.ItemName.Equals("经度"))
            {
                mJunc.X_Coor = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("纬度"))
            {
                mJunc.Y_Coor = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("检查井类别"))
            {
                mJunc.Junc_Category = GetCategory(msg.ValueName);
            }
            else if (msg.ItemName.Equals("检查井类型"))
            {
                mJunc.Junc_Type = GetType(msg.ValueName);
            }
            else if (msg.ItemName.Equals("检查井形式"))
            {
                mJunc.Junc_Style = GetStyle(msg.ValueName);
            }
            else if (msg.ItemName.Equals("井深"))
            {
                mJunc.Depth = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("井盖所处的地面高程"))
            {
                mJunc.Surface_Ele = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据来源"))
            {
                mJunc.DataSource = GetDataSource(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据填报单位"))
            {
                mJunc.ReportDept = msg.ValueName;
            }
            else if (msg.ItemName.Equals("数据填报时间"))
            {
                mJunc.Record_Date = ValueConvert.str2time(msg.ValueName);
            }
            else if (msg.ItemName.Equals("管道是否暗接"))
            {
                mJunc.Junc_Darkjoint = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("污水是否直排"))
            {
                mJunc.Sewage_Line = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("井盖错误"))
            {
                mJunc.Junc_Error = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("CCTV检测编号"))
            {
                mJunc.CCTV_CheckCode = msg.ValueName;
            }
            else if (msg.ItemName.Equals("数据是否完整"))
            {
                mJunc.FullData = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据缺失原因"))
            {
                mJunc.LoseReson = msg.ValueName;
            }
            else if (msg.ItemName.Equals("A上游井口至管顶距离"))
            {
                mJunc.Dis[0] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("A上游井口至管底距离"))
            {
                mJunc.Dis[1] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("A下游井口至管顶距离"))
            {
                mJunc.Dis[2] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("A下游井口至管底距离"))
            {
                mJunc.Dis[3] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("B上游井口至管顶距离"))
            {
                mJunc.Dis[4] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("B上游井口至管底距离"))
            {
                mJunc.Dis[5] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("B下游井口至管顶距离"))
            {
                mJunc.Dis[6] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("B下游井口至管底距离"))
            {
                mJunc.Dis[7] = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("备注"))
            {
                mJuncext.Remark = msg.ValueName;
            }
        }

        private ObservableCollection<JuncMesage> GetData(string juncname)
        {
            var Datas = new ObservableCollection<JuncMesage>();

            SelectCmd scmd = new SelectCmd();
            JuncRev rev = new JuncRev();

            rev.JuncName = juncname;
            scmd.SetReceiver(rev);
            scmd.Execute();
            if (rev.ListJunc == null || rev.ListJunc.Count <= 0)
                return null;
            mJunc = rev.ListJunc.ElementAt(0);
            mJuncext = rev.ListJuncExt.ElementAt(0);

            Datas.Add(new JuncMesage { ItemName = "井盖名称", ValueName = mJunc.JuncName });
            Datas.Add(new JuncMesage { ItemName = "道路名称", ValueName = mJuncext.Lane_Way });
            Datas.Add(new JuncMesage { ItemName = "经度", ValueName = Convert.ToString(mJunc.X_Coor) });
            Datas.Add(new JuncMesage { ItemName = "纬度", ValueName = Convert.ToString(mJunc.Y_Coor) });
            Datas.Add(new JuncMesage { ItemName = "检查井类别", ValueName = GetCategoryName(mJunc.Junc_Category) });
            Datas.Add(new JuncMesage { ItemName = "检查井类型", ValueName = GetType(mJunc.Junc_Type) });
            Datas.Add(new JuncMesage { ItemName = "检查井形式", ValueName = GetStyle(mJunc.Junc_Style) });
            Datas.Add(new JuncMesage { ItemName = "井深", ValueName = GetDepth(mJunc.Depth) });

            Datas.Add(new JuncMesage { ItemName = "井盖所处的地面高程", ValueName = GetDepth(mJunc.Surface_Ele) });
            Datas.Add(new JuncMesage { ItemName = "数据来源", ValueName = GetDataSource(mJunc.DataSource) });
            Datas.Add(new JuncMesage { ItemName = "数据填报单位", ValueName = mJunc.ReportDept });
            Datas.Add(new JuncMesage { ItemName = "数据填报时间", ValueName = Convert.ToString(mJunc.Record_Date) });
            Datas.Add(new JuncMesage { ItemName = "管道是否暗接", ValueName = bool2str(mJunc.Junc_Darkjoint) });

            Datas.Add(new JuncMesage { ItemName = "污水是否直排", ValueName = bool2str(mJunc.Sewage_Line) });
            Datas.Add(new JuncMesage { ItemName = "井盖错误", ValueName = bool2str(mJunc.Junc_Error) });
            Datas.Add(new JuncMesage { ItemName = "CCTV检测编号", ValueName = mJunc.CCTV_CheckCode });
            Datas.Add(new JuncMesage { ItemName = "数据是否完整", ValueName = bool2str(mJunc.FullData) });
            Datas.Add(new JuncMesage { ItemName = "数据缺失原因", ValueName = mJunc.LoseReson });

            Datas.Add(new JuncMesage { ItemName = "A上游井口至管顶距离", ValueName = GetDepth(mJunc.Dis[0]) });
            Datas.Add(new JuncMesage { ItemName = "A上游井口至管底距离", ValueName = GetDepth(mJunc.Dis[1]) });
            Datas.Add(new JuncMesage { ItemName = "A下游井口至管顶距离", ValueName = GetDepth(mJunc.Dis[2]) });
            Datas.Add(new JuncMesage { ItemName = "A下游井口至管底距离", ValueName = GetDepth(mJunc.Dis[3]) });
            Datas.Add(new JuncMesage { ItemName = "B上游井口至管顶距离", ValueName = GetDepth(mJunc.Dis[4]) });
            Datas.Add(new JuncMesage { ItemName = "B上游井口至管底距离", ValueName = GetDepth(mJunc.Dis[5]) });
            Datas.Add(new JuncMesage { ItemName = "B下游井口至管顶距离", ValueName = GetDepth(mJunc.Dis[6]) });
            Datas.Add(new JuncMesage { ItemName = "B下游井口至管底距离", ValueName = GetDepth(mJunc.Dis[7]) });

            Datas.Add(new JuncMesage { ItemName = "备注", ValueName = mJuncext.Remark });


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

        private int GetType(string name)
        {
            string[] pat = { @"排水井", @"排水井", @"排水井", @"排水井", @"倒虹井", @"透气井" ,
                           @"压力井", @"检测井", @"拍门井", @"截流井", @"水封井", @"跌水井" ,@"其他"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 排水井 ， 排水井 ，排水井 ，排水井,倒虹井,透气井,...其他");
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

        private int GetStyle(string name)
        {
            string[] pat = { @"一通",@"二通直",@"二通转",
                             @"三通",@"四通",@"五通",@"五通以上",@"暗井",@"侧立型",@"平面型",@"出水口" ,@"其他"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 一通 ， 二通直 ，二通转 ，三通,四通,五通,...其他");
        }

        private string GetDepth(double d)
        {
            return Convert.ToString(d);
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

    }
}
