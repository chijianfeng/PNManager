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

namespace PipeNetManager.juncMsg
{
    /// <summary>
    /// BaseContent.xaml 的交互逻辑
    /// </summary>
    public partial class BaseContent : UserControl
    {
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
            CJuncInfo junc = rev.ListJunc.ElementAt(0);
            CJuncExtInfo juncext = rev.ListJuncExt.ElementAt(0);

            Datas.Add(new JuncMesage { ItemName = "井盖名称", ValueName = junc.JuncName });
            Datas.Add(new JuncMesage { ItemName = "道路名称", ValueName = juncext.Lane_Way });
            Datas.Add(new JuncMesage { ItemName = "经度", ValueName = Convert.ToString(junc.X_Coor) });
            Datas.Add(new JuncMesage { ItemName = "纬度", ValueName = Convert.ToString(junc.Y_Coor) });
            Datas.Add(new JuncMesage { ItemName = "检查井类别", ValueName = GetCategoryName(junc.Junc_Category) });
            Datas.Add(new JuncMesage { ItemName = "检查井类型", ValueName = GetType(junc.Junc_Type) });
            Datas.Add(new JuncMesage { ItemName = "检查井形式", ValueName = GetStyle(junc.Junc_Style) });
            Datas.Add(new JuncMesage { ItemName = "井深", ValueName = GetDepth(junc.Depth) });

            Datas.Add(new JuncMesage { ItemName = "井盖所处的地面高程", ValueName = GetDepth(junc.Surface_Ele) });
            Datas.Add(new JuncMesage { ItemName = "数据来源", ValueName = GetDataSource(junc.DataSource) });
            Datas.Add(new JuncMesage { ItemName = "数据填报单位", ValueName = junc.ReportDept });
            Datas.Add(new JuncMesage { ItemName = "数据填报时间", ValueName = Convert.ToString(junc.Record_Date) });
            Datas.Add(new JuncMesage { ItemName = "管道是否暗接", ValueName = bool2str(junc.Junc_Darkjoint) });

            Datas.Add(new JuncMesage { ItemName = "污水是否直排", ValueName = bool2str(junc.Sewage_Line) });
            Datas.Add(new JuncMesage { ItemName = "井盖错误", ValueName = bool2str(junc.Junc_Error) });
            Datas.Add(new JuncMesage { ItemName = "CCTV检测编号", ValueName = junc.CCTV_CheckCode });
            Datas.Add(new JuncMesage { ItemName = "数据是否完整", ValueName = bool2str(junc.FullData) });
            Datas.Add(new JuncMesage { ItemName = "数据缺失原因", ValueName = junc.LoseReson });

            Datas.Add(new JuncMesage { ItemName = "A上游井口至管顶距离", ValueName = GetDepth(junc.Dis[0]) });
            Datas.Add(new JuncMesage { ItemName = "A上游井口至管底距离", ValueName = GetDepth(junc.Dis[1]) });
            Datas.Add(new JuncMesage { ItemName = "A下游井口至管顶距离", ValueName = GetDepth(junc.Dis[2]) });
            Datas.Add(new JuncMesage { ItemName = "A下游井口至管底距离", ValueName = GetDepth(junc.Dis[3]) });
            Datas.Add(new JuncMesage { ItemName = "B上游井口至管顶距离", ValueName = GetDepth(junc.Dis[4]) });
            Datas.Add(new JuncMesage { ItemName = "B上游井口至管底距离", ValueName = GetDepth(junc.Dis[5]) });
            Datas.Add(new JuncMesage { ItemName = "B下游井口至管顶距离", ValueName = GetDepth(junc.Dis[6]) });
            Datas.Add(new JuncMesage { ItemName = "B下游井口至管底距离", ValueName = GetDepth(junc.Dis[7]) });

            Datas.Add(new JuncMesage { ItemName = "备注", ValueName = juncext.Remark });


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

        private string GetDepth(double d)
        {
            if (d == null)
                return "";
            else
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
    }
}
