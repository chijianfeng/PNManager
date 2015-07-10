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
    /// <summary>
    /// USInfo.xaml 的交互逻辑
    /// </summary>
    public partial class USInfo : UserControl
    {
        private string mPipeName;
        public USInfo(string name)
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
            mPipeName = pipename;
            ObservableCollection<Mesage> datas = GetData();
            DG1.DataContext = datas;
        }

        private ObservableCollection<Mesage> GetData()
        {
            var Datas = new ObservableCollection<Mesage>();

            SelectCmd scmd = new SelectCmd();
            PipeRev pr = new PipeRev();
            pr.PipeName = mPipeName;
            scmd.SetReceiver(pr);
            scmd.Execute();
            if (pr.ListUS == null || pr.ListUS.Count <= 0)
                return null;

            CUSInfo us = pr.ListUS.ElementAt(0);

            Datas.Add(new Mesage { ItemName = "排水管标识码", ValueName = pr.PipeName });
            Datas.Add(new Mesage { ItemName = "作业编号", ValueName = us.JobID });
            Datas.Add(new Mesage { ItemName = "检测日期", ValueName = Convert.ToString(us.DetectDate) });
            Datas.Add(new Mesage { ItemName = "检测单位", ValueName = us.DetectDep });
            Datas.Add(new Mesage { ItemName = "检测操作人员", ValueName = us.Detect_Person });
            Datas.Add(new Mesage { ItemName = "检测单位联系方式", ValueName = us.Contacts });
            Datas.Add(new Mesage { ItemName = "检测方法", ValueName = GetCheckMethod(us.Detect_Method) });
            Datas.Add(new Mesage { ItemName = "检测方向", ValueName = GetCheckDir(us.Detect_Dir) });
            Datas.Add(new Mesage { ItemName = "封堵情况", ValueName = us.Pipe_Stop });
            Datas.Add(new Mesage { ItemName = "功能性缺失", ValueName = GetFuncDef(us.Func_Defect) });
            Datas.Add(new Mesage { ItemName = "功能性缺失等级", ValueName = GetClass(us.Func_Class) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷", ValueName = GetStructDef(us.Strcut_Defect) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷等级", ValueName = GetClass(us.Struct_Class) });
            Datas.Add(new Mesage { ItemName = "修复指数RI", ValueName = Convert.ToString(us.Repair_Index) });
            Datas.Add(new Mesage { ItemName = "养护指数MI", ValueName = Convert.ToString(us.Matain_Index) });
            Datas.Add(new Mesage { ItemName = "缺陷描述", ValueName = us.Problem });
            Datas.Add(new Mesage { ItemName = "检测影像文件的文件", ValueName = us.Video_Filename });
            Datas.Add(new Mesage { ItemName = "数据填报单位", ValueName = Convert.ToString(us.ReportDept) });
            Datas.Add(new Mesage { ItemName = "填报日期", ValueName = Convert.ToString(us.ReportDate) });
            Datas.Add(new Mesage { ItemName = "数据是否完整", ValueName = bool2str(us.DataIsFull) });
            Datas.Add(new Mesage { ItemName = "数据缺失原因", ValueName = us.LoseReason });
            Datas.Add(new Mesage { ItemName = "备注", ValueName = us.Remark });
            return Datas;
        }

        private string bool2str(bool b)
        {
            if (b)
                return "是";
            else
                return "否";
        }


        private string GetFuncDef(int i)
        {
            i = i - 1;
            string[] pat = { @"无缺陷", @"沉积", @"结垢", @"障碍物", @"残墙坝根", @"树根", @"浮渣", @"封堵" };
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }

        private string GetStructDef(int i)
        {
            string[] pat = { @"无缺陷", @"破裂", @"变形", @"腐蚀", @"错口", @"起伏", @"脱节", @"接口材料脱落",
                           @"支管暗接", @"异物穿入", @"渗漏"};
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }

        private string GetClass(int i)
        {
            string[] pat = { @"I", @"Ⅱ", @"Ⅲ", @"Ⅳ", @"Ⅴ", @"Ⅵ", @"Ⅶ", @"Ⅷ",
                           @"Ⅸ"};
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "";

            return pat[i];
        }

        private string GetCheckMethod(int i)
        {
            string[] pat = { @"CCTV", @"声纳", @"潜望镜" };
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }

        private string GetCheckDir(int i)
        {
            string[] pat = { @"与水流向一致", @"与水流方向不一致" };
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }
        
    }
}
