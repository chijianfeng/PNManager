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
    /// <summary>
    /// USInfo.xaml 的交互逻辑
    /// </summary>
    public partial class USInfo : UserControl
    {
        private CUSInfo mUSinfo;
        public USInfo(string name)
        {
            InitializeComponent();
            ShowContent(name);
        }

        public USInfo(int id)
        {
            InitializeComponent();
            ShowContent(id);
        }

        private void ShowContent(int id)
        {
            ObservableCollection<Mesage> datas = GetData(id);
            DG1.DataContext = datas;
        }

        public void ShowContent(string pipename)
        {
            if (pipename == null && pipename.Length <= 0)
            {
                MessageBox.Show("Load Data Failed!", "错误消息");
                return;
            }
            ObservableCollection<Mesage> datas = GetData(pipename);
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

            TUSInfo usinfo = new TUSInfo(App._dbpath, App.PassWord);
            List<CUSInfo> list = new List<CUSInfo>();
            list.Add(mUSinfo);
            return usinfo.Update_USInfo(list);
        }

        private void ParseData(Mesage msg)
        {
            if (msg.ItemName.Equals("作业编号"))
            {
                mUSinfo.JobID = msg.ValueName;
            }
            else if (msg.ItemName.Equals("检测日期")) 
            {
                mUSinfo.DetectDate = ValueConvert.str2time(msg.ValueName);
            }
            else if (msg.ItemName.Equals("检测单位"))
            {
                mUSinfo.DetectDep = msg.ValueName;
            }
            else if (msg.ItemName.Equals("检测操作人员"))
            {
                mUSinfo.Detect_Person = msg.ValueName;
            }
            else if (msg.ItemName.Equals("检测单位联系方式"))
            {
                mUSinfo.Contacts = msg.ValueName;
            }
            else if (msg.ItemName.Equals("检测方法"))
            {
                mUSinfo.Detect_Method = GetCheckMethod(msg.ValueName);
            }
            else if (msg.ItemName.Equals("检测方向"))
            {
                mUSinfo.Detect_Dir = GetCheckDir(msg.ValueName);
            }
            else if (msg.ItemName.Equals("封堵情况"))
            {
                mUSinfo.Pipe_Stop = msg.ValueName;
            }
            else if (msg.ItemName.Equals("功能性缺失"))
            {
                mUSinfo.Func_Defect = GetFunDef(msg.ValueName);
            }
            else if (msg.ItemName.Equals("功能性缺失等级"))
            {
                mUSinfo.Func_Class = GetClass(msg.ValueName);
            }
            else if (msg.ItemName.Equals("结构性缺陷"))
            {
                mUSinfo.Strcut_Defect = GetStructDef(msg.ValueName);
            }
            else if (msg.ItemName.Equals("结构性缺陷等级"))
            {
                mUSinfo.Struct_Class = GetClass(msg.ValueName);
            }
            else if (msg.ItemName.Equals("修复指数RI"))
            {
                mUSinfo.Repair_Index = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("养护指数MI"))
            {
                mUSinfo.Matain_Index = ValueConvert.str2double(msg.ValueName);
            }
            else if (msg.ItemName.Equals("缺陷描述"))
            {
                mUSinfo.Problem = msg.ValueName;
            }
            else if (msg.ItemName.Equals("检测影像文件的文件"))
            {
                mUSinfo.Video_Filename = msg.ValueName;
            }
            else if (msg.ItemName.Equals("数据填报单位"))
            {
                mUSinfo.ReportDept = msg.ValueName;
            }
            else if (msg.ItemName.Equals("填报日期"))
            {
                mUSinfo.ReportDate = ValueConvert.str2time(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据是否完整"))
            {
                mUSinfo.DataIsFull = ValueConvert.str2bool(msg.ValueName);
            }
            else if (msg.ItemName.Equals("数据缺失原因"))
            {
                mUSinfo.LoseReason = msg.ValueName;
            }
            else if (msg.ItemName.Equals("备注"))
            {
                mUSinfo.Remark = msg.ValueName;
            }
        }

        private ObservableCollection<Mesage> GetData(string pipename)
        {
            var Datas = new ObservableCollection<Mesage>();

            SelectCmd scmd = new SelectCmd();
            PipeRev pr = new PipeRev();
            pr.PipeName = pipename;
            scmd.SetReceiver(pr);
            scmd.Execute();
            if (pr.ListUS == null || pr.ListUS.Count <= 0)
                return null;

            mUSinfo = pr.ListUS.ElementAt(0);

            Datas.Add(new Mesage { ItemName = "排水管标识码", ValueName = pr.PipeName });
            Datas.Add(new Mesage { ItemName = "作业编号", ValueName = mUSinfo.JobID });
            Datas.Add(new Mesage { ItemName = "检测日期", ValueName = Convert.ToString(mUSinfo.DetectDate) });
            Datas.Add(new Mesage { ItemName = "检测单位", ValueName = mUSinfo.DetectDep });
            Datas.Add(new Mesage { ItemName = "检测操作人员", ValueName = mUSinfo.Detect_Person });
            Datas.Add(new Mesage { ItemName = "检测单位联系方式", ValueName = mUSinfo.Contacts });
            Datas.Add(new Mesage { ItemName = "检测方法", ValueName = GetCheckMethod(mUSinfo.Detect_Method) });
            Datas.Add(new Mesage { ItemName = "检测方向", ValueName = GetCheckDir(mUSinfo.Detect_Dir) });
            Datas.Add(new Mesage { ItemName = "封堵情况", ValueName = mUSinfo.Pipe_Stop });
            Datas.Add(new Mesage { ItemName = "功能性缺失", ValueName = GetFuncDef(mUSinfo.Func_Defect) });
            Datas.Add(new Mesage { ItemName = "功能性缺失等级", ValueName = GetClass(mUSinfo.Func_Class) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷", ValueName = GetStructDef(mUSinfo.Strcut_Defect) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷等级", ValueName = GetClass(mUSinfo.Struct_Class) });
            Datas.Add(new Mesage { ItemName = "修复指数RI", ValueName = Convert.ToString(mUSinfo.Repair_Index) });
            Datas.Add(new Mesage { ItemName = "养护指数MI", ValueName = Convert.ToString(mUSinfo.Matain_Index) });
            Datas.Add(new Mesage { ItemName = "缺陷描述", ValueName = mUSinfo.Problem });
            Datas.Add(new Mesage { ItemName = "检测影像文件的文件", ValueName = mUSinfo.Video_Filename });
            Datas.Add(new Mesage { ItemName = "数据填报单位", ValueName = Convert.ToString(mUSinfo.ReportDept) });
            Datas.Add(new Mesage { ItemName = "填报日期", ValueName = Convert.ToString(mUSinfo.ReportDate) });
            Datas.Add(new Mesage { ItemName = "数据是否完整", ValueName = bool2str(mUSinfo.DataIsFull) });
            Datas.Add(new Mesage { ItemName = "数据缺失原因", ValueName = mUSinfo.LoseReason });
            Datas.Add(new Mesage { ItemName = "备注", ValueName = mUSinfo.Remark });
            return Datas;
        }

        private ObservableCollection<Mesage> GetData(int id)
        {
            var Datas = new ObservableCollection<Mesage>();

            TUSInfo tusinfo = new TUSInfo(App._dbpath, App.PassWord);
            mUSinfo = tusinfo.Sel_USInfoByPipeid(id);
            if (mUSinfo == null)
            {
                MessageBox.Show("读取内窥数据失败");
                return null;
            }

            Datas.Add(new Mesage { ItemName = "排水管标识码", ValueName = "-" });
            Datas.Add(new Mesage { ItemName = "作业编号", ValueName = mUSinfo.JobID });
            Datas.Add(new Mesage { ItemName = "检测日期", ValueName = Convert.ToString(mUSinfo.DetectDate) });
            Datas.Add(new Mesage { ItemName = "检测单位", ValueName = mUSinfo.DetectDep });
            Datas.Add(new Mesage { ItemName = "检测操作人员", ValueName = mUSinfo.Detect_Person });
            Datas.Add(new Mesage { ItemName = "检测单位联系方式", ValueName = mUSinfo.Contacts });
            Datas.Add(new Mesage { ItemName = "检测方法", ValueName = GetCheckMethod(mUSinfo.Detect_Method) });
            Datas.Add(new Mesage { ItemName = "检测方向", ValueName = GetCheckDir(mUSinfo.Detect_Dir) });
            Datas.Add(new Mesage { ItemName = "封堵情况", ValueName = mUSinfo.Pipe_Stop });
            Datas.Add(new Mesage { ItemName = "功能性缺失", ValueName = GetFuncDef(mUSinfo.Func_Defect) });
            Datas.Add(new Mesage { ItemName = "功能性缺失等级", ValueName = GetClass(mUSinfo.Func_Class) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷", ValueName = GetStructDef(mUSinfo.Strcut_Defect) });
            Datas.Add(new Mesage { ItemName = "结构性缺陷等级", ValueName = GetClass(mUSinfo.Struct_Class) });
            Datas.Add(new Mesage { ItemName = "修复指数RI", ValueName = Convert.ToString(mUSinfo.Repair_Index) });
            Datas.Add(new Mesage { ItemName = "养护指数MI", ValueName = Convert.ToString(mUSinfo.Matain_Index) });
            Datas.Add(new Mesage { ItemName = "缺陷描述", ValueName = mUSinfo.Problem });
            Datas.Add(new Mesage { ItemName = "检测影像文件的文件", ValueName = mUSinfo.Video_Filename });
            Datas.Add(new Mesage { ItemName = "数据填报单位", ValueName = Convert.ToString(mUSinfo.ReportDept) });
            Datas.Add(new Mesage { ItemName = "填报日期", ValueName = Convert.ToString(mUSinfo.ReportDate) });
            Datas.Add(new Mesage { ItemName = "数据是否完整", ValueName = bool2str(mUSinfo.DataIsFull) });
            Datas.Add(new Mesage { ItemName = "数据缺失原因", ValueName = mUSinfo.LoseReason });
            Datas.Add(new Mesage { ItemName = "备注", ValueName = mUSinfo.Remark });
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

        private int GetFunDef(string name)
        {
            string[] pat = { @"无缺陷", @"沉积", @"结垢", @"障碍物", @"残墙坝根", @"树根", @"浮渣", @"封堵", @"其他" };
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 无缺陷 ,沉积,结垢,障碍物, 残墙坝根,...,其他");
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

        private int GetStructDef(string name)
        {
            string[] pat = { @"无缺陷", @"破裂", @"变形", @"腐蚀", @"错口", @"起伏", @"脱节", @"接口材料脱落",
                           @"支管暗接", @"异物穿入", @"渗漏",@"其他"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 无缺陷 ,破裂,变形,腐蚀, 起伏,...,其他");
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

        private int GetClass(string name)
        {
            string[] pat = { @"I", @"Ⅱ", @"Ⅲ", @"Ⅳ", @"Ⅴ", @"Ⅵ", @"Ⅶ", @"Ⅷ",
                           @"Ⅸ"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: I ,Ⅱ,Ⅲ, Ⅳ , Ⅴ,...");
        }

        private string GetCheckMethod(int i)
        {
            string[] pat = { @"CCTV", @"声纳", @"潜望镜" };
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }

        private int GetCheckMethod(string name)
        {
            string[] pat = { @"CCTV", @"声纳", @"潜望镜" ,@"其他"};
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: CCTV ,声纳 ，潜望镜 ,其他");
        }

        private string GetCheckDir(int i)
        {
            string[] pat = { @"与水流向一致", @"与水流方向不一致" };
            i = i - 1;
            if (i < 0 || i >= pat.Count())
                return "其他";

            return pat[i];
        }

        private int GetCheckDir(string name)
        {
            string[] pat = { @"与水流向一致", @"与水流方向不一致" , @"其他" };
            int count = 1;
            foreach (string p in pat)
            {
                if (p.Equals(name))
                    return count;
                else
                    count++;
            }
            throw new ExceptionProcess("输入类型必须是: 与水流向一致 ,与水流方向不一致 ,其他");
        }
        
    }
}
