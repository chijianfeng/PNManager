using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using ExcelOper.Data;
using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;

namespace ExcelOper
{
    class ExcelToDB
    {
        public ExcelToDB(DataSet ds)
        {
            dataset = ds;
        }

        public string Sr;       //start position
        public string Er;       //end position
        public int DataType;    //the Different Excel Type

        public  void LoadtoDB()
        {
            if (DataType == 1)
                LoadJuncData();
            if (DataType == 2)
                LoadPipeData();
            if (DataType == 3)
                LoadUSData();
        }

        private void LoadUSData()
        {
            if (dataset == null)
                return;
            if (Sr == null || Er == null)
                return;
            int spr = GetNumber(Sr), epr = GetNumber(Er);
            int spc = GetChar(Sr), epc = GetChar(Er);

            List<USSheetData> listsheet = new List<USSheetData>();
            for (; spr <= epr; spr++)
            {
                USSheetData sheet = new USSheetData();
                DataRow dr = dataset.Tables[0].Rows[spr - 2];
                int j = spc;
                DataColumn dc = dataset.Tables[0].Columns[j++];
                sheet.PipeName = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.RoadName = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.In_Code = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Out_Code = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.JobID = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                string tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.CheckDate = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.CheckDept = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.CheckPerson = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.CheckContact = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.CheckMethod = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.CheckDirect = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Pipe_Stop = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Func_Defect = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Func_Class = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Struct_Defect = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Struct_Class = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Repire_Index = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Matain_Index = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Problem = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Video_Filename = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.RecordDept = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.RecordTime = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.DataFull = GetBool(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.LoseReson = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Remark = dr[dc].ToString();
                listsheet.Add(sheet);
            }
            SaverTodb(listsheet);
        }


        private void LoadJuncData()
        {
            if (dataset == null)
                return;
            if (Sr == null || Er == null)
                return;
            int spr = GetNumber(Sr), epr = GetNumber(Er);
            int spc = GetChar(Sr), epc = GetChar(Er);

            List<JuncSheetData> listsheet = new List<JuncSheetData>();
            for (; spr <= epr; spr++)
            {
                JuncSheetData sheet = new JuncSheetData();
                DataRow dr = dataset.Tables[0].Rows[spr - 2];
                int j = spc;
                DataColumn dc = dataset.Tables[0].Columns[j++];
                sheet.JuncName = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.RoadName = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.JuncCategory = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.JuncType = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.JuncStyle = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Latitude = Change(dr[dc].ToString());
                dc = dataset.Tables[0].Columns[j++];
                sheet.Longitude = Change(dr[dc].ToString());

                dc = dataset.Tables[0].Columns[j++];
                string tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Height = GetFloat(tmp);
                else
                    sheet.Height = 0;

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Depth = GetFloat(tmp);
                else
                    sheet.Depth = 0;

                dc = dataset.Tables[0].Columns[j++];
                sheet.CCTV_CheckCode = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.JuncDarkjoint = GetBool(dr[dc].ToString());

                dc = dataset.Tables[0].Columns[j++];
                sheet.SewageLine = GetBool(dr[dc].ToString());
                dc = dataset.Tables[0].Columns[j++];
                sheet.JuncError = GetBool(dr[dc].ToString());

                for (int i = 0; i < 8; i++)
                {
                    dc = dataset.Tables[0].Columns[j++];
                    tmp = dr[dc].ToString();
                    if (tmp != null && tmp.Length > 0)
                        sheet.Dis[i] = Convert.ToDouble(tmp);
                }

                dc = dataset.Tables[0].Columns[j++];
                sheet.DataSource = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.DataTime = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.RecordDept = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.RecordTime = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.DataFull = GetBool(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.LoseReson = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Remark = dr[dc].ToString();
                listsheet.Add(sheet);
            }

            //save to db.
            SaverTodb(listsheet);
        }

        private void LoadPipeData()
        {
            if (dataset == null)
                return;
            if (Sr == null || Er == null)
                return;
            int spr = GetNumber(Sr), epr = GetNumber(Er);
            int spc = GetChar(Sr), epc = GetChar(Er);

            List<PipeSheetData> listsheet = new List<PipeSheetData>();
            for (; spr <= epr; spr++)
            {
                PipeSheetData sheet = new PipeSheetData();
                DataRow dr = dataset.Tables[0].Rows[spr - 2];
                int j = spc;

                DataColumn dc = dataset.Tables[0].Columns[j++];
                sheet.PipeName = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.RoadName = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.PipeCategory = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.InCode = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.OutCode = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                string tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.In_UpEle = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.In_BottomEle = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Out_UpEle = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Out_BottomEle = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.In_R = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Out_R = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.ShapeType = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.ShapeData = dr[dc].ToString();
                dc = dataset.Tables[0].Columns[j++];
                sheet.Matrial = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.Roughness = GetFloat(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.DataSource = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.DataTime = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.RecordDept = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.RecordTime = Convert.ToDateTime(tmp);

                dc = dataset.Tables[0].Columns[j++];
                tmp = dr[dc].ToString();
                if (tmp != null && tmp.Length > 0)
                    sheet.DataFull = GetBool(tmp);

                dc = dataset.Tables[0].Columns[j++];
                sheet.LoseReson = dr[dc].ToString();

                dc = dataset.Tables[0].Columns[j++];
                sheet.Remark = dr[dc].ToString();
                listsheet.Add(sheet);
            }
            SaverTodb(listsheet);
        }
        private bool SaverTodb(List<JuncSheetData> list)
        {
            if (list == null || list.Count <= 0)
                return false;

            QuickInsert qicm = new QuickInsert();
            JuncRev rev = new JuncRev();
            rev.ListJunc = new List<CJuncInfo>();
            rev.ListJuncExt = new List<CJuncExtInfo>();

            foreach (JuncSheetData data in list)
            {
                CJuncInfo info = new CJuncInfo();
                CJuncExtInfo extinfo = new CJuncExtInfo();

                extinfo.Lane_Way = data.RoadName;
                info.JuncName = data.JuncName;
                info.Junc_Category = GetCategory(data.JuncCategory);
                info.Junc_Type = GetType(data.JuncType);
                info.Y_Coor = data.Latitude;
                info.X_Coor = data.Longitude;
                info.Surface_Ele = data.Height;
                info.Junc_Style = GetStyle(data.JuncStyle);

                info.Depth = data.Depth;
                info.Junc_Darkjoint = data.JuncDarkjoint;
                info.Sewage_Line = data.SewageLine;
                info.Junc_Error = data.JuncError;

                info.CCTV_CheckCode = data.CCTV_CheckCode;
                info.DataSource = GetDataSource(data.DataSource);
                info.Record_Date = data.DataTime;

                info.ReportDept = data.RecordDept;
                info.ReportDate = data.RecordTime;

                for (int i = 0; i < 8;i++ )
                {
                    info.Dis[i] = data.Dis[i];
                }

                    info.FullData = data.DataFull;
                info.LoseReson = data.LoseReson;

                extinfo.Remark = data.Remark;
                rev.ListJunc.Add(info);
                rev.ListJuncExt.Add(extinfo);
            }
            qicm.SetReceiver(rev);
            qicm.Execute();

            return true;
        }

        private bool SaverTodb(List<PipeSheetData> list)
        {
            //get the juncinfo. then 

            if (list == null || list.Count <= 0)
                return false;
            QuickInsert icm = new QuickInsert();
            PipeRev rev = new PipeRev();
            rev.ListPipe = new List<CPipeInfo>();
            rev.ListPipeExt = new List<CPipeExtInfo>();

            LoadCmd lcmd = new LoadCmd();
            JuncRev junrev = new JuncRev();
            lcmd.SetReceiver(junrev);
            lcmd.Execute();

            string pname  = "";
            try
            {
                foreach (PipeSheetData data in list)
                {
                    CPipeInfo info = new CPipeInfo();
                    CPipeExtInfo extinfo = new CPipeExtInfo();

                    extinfo.Lane_Way = data.RoadName;
                    info.PipeName = data.PipeName;
                    info.Pipe_Category = GetCategory(data.PipeCategory);
                    info.Pipe_Len = 0;//default
                    int inid = CheckID(data.InCode, junrev.ListJunc);
                    int outid = CheckID(data.OutCode, junrev.ListJunc);
                    info.In_JunID = inid;
                    info.Out_JunID = outid;

                    info.In_UpEle = data.In_UpEle;
                    info.In_BottomEle = data.In_BottomEle;
                    info.Out_UpEle = data.Out_UpEle;
                    info.Out_BottomEle = data.Out_UpEle;

                    info.Shape_Data1 = data.In_R;
                    info.Shape_Data2 = data.Out_R;
                    info.ShapeType = GetShapeType(data.ShapeType);
                    info.ShapeData = data.ShapeData;
                    info.Material = Getmaterial(data.Matrial);

                    info.Roughness = data.Roughness;

                    info.DataSource = GetDataSource(data.DataSource);
                    info.Record_Date = data.DataTime;

                    info.ReportDept = data.RecordDept;
                    info.ReportDate = data.RecordTime;

                    extinfo.DataIsFull = data.DataFull;
                    extinfo.LoseReason = data.LoseReson;

                    extinfo.Remark = data.Remark;
                    rev.ListPipe.Add(info);
                    rev.ListPipeExt.Add(extinfo);

                    pname = data.PipeName;
                }
            } 
            catch(Exception e)
            {
                Console.WriteLine("PipeName : {0} in exception{1}",pname,e.Message);
            }
           
            icm.SetReceiver(rev);
            icm.Execute();
            return true;
        }

        private bool SaverTodb(List<USSheetData> list)
        {

            if (list == null || list.Count <= 0)
                return false;

            QuickInsert icm = new QuickInsert();
            PipeRev rev = new PipeRev();

            LoadCmd lcmd = new LoadCmd();
            lcmd.SetReceiver(rev);
            lcmd.Execute();
            string pname = "";
            try
            {
                foreach (USSheetData data in list)
                {
                    CUSInfo info = new CUSInfo();

                    info.PipeID = GetPipeID(data.PipeName , rev.ListPipe);
                    info.JobID = data.JobID;
                    info.DetectDate = data.CheckDate;

                    info.DetectDep = data.CheckDept;
                    info.Detect_Person = data.CheckPerson;
                    info.Contacts = data.CheckContact;

                    info.Detect_Method = GetCheckMethod(data.CheckMethod);
                    info.Detect_Dir = GetCheckDir(data.CheckDirect);

                    info.Pipe_Stop = data.Pipe_Stop;

                    info.Func_Defect = GetFuncDef(data.Func_Defect);
                    info.Func_Class = GetClass(data.Func_Class);

                    info.Strcut_Defect = GetStructDef(data.Struct_Defect);
                    info.Struct_Class = GetClass(data.Struct_Class);

                    if (IsDouble(data.Repire_Index))
                        info.Repair_Index = Convert.ToDouble(data.Repire_Index);
                    else
                        info.Repair_Index = 0;

                    if (IsDouble(data.Matain_Index))
                        info.Matain_Index = Convert.ToDouble(data.Matain_Index);
                    else
                        info.Matain_Index = 0;

                    info.Problem = data.Problem;
                    info.Video_Filename = data.Video_Filename;


                    info.ReportDept = data.RecordDept;
                    info.ReportDate = data.RecordTime;

                    info.DataIsFull = data.DataFull;
                    info.LoseReason = data.LoseReson;

                    info.Remark = data.Remark;
                    rev.ListUS.Add(info);

                    pname = data.PipeName;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("PipeName : {0} in exception{1}", pname, e.Message);
            }
            rev.ListPipe.Clear();
            rev.ListPipeExt.Clear();

            icm.SetReceiver(rev);
            icm.Execute();
            return true;
        }
        private  int GetNumber(string str)
        {
            string pattern = @"\D*";
            string res = System.Text.RegularExpressions.Regex.Replace(str, pattern, "");
            return Convert.ToInt32(res);
        }

        private  int GetChar(string str)
        {
            int len = 0;
            string pattern = @"\d+\w*$";
            string res = System.Text.RegularExpressions.Regex.Replace(str, pattern, "");
            if (res == null)
                return len;
            for (int i = 0; i < res.Length;i++ )
            {
                len = len * 26;
                char c = res.ElementAt(i);
                len += Convert.ToInt32(c) - 'A'+1;
            }
            return len-1;
        }

        private  double GetFloat(string str)
        {
            Regex r = new Regex(@"\d*[\d\.]\d*");
            string res = r.Match(str).Value.ToString();
            return Convert.ToDouble(res);
        }

        private double Change(string str)
        {
            Regex r = new Regex(":");
            string[] s = r.Split(str);
            if (s.Length != 3)
                return 0;
            double d0 = GetFloat(s[0]);
            double d1 = GetFloat(s[1]);
            double d2 = GetFloat(s[2]);

            return d0 + d1 / 60 + d2 / 3600;
        }

        private bool GetBool(string str)
        {
            if (str == null || str.Length <= 0)
                return false;
            if (str.Equals("1") || str.Equals("Y") || str.Equals("true"))
                return true;
            else
                return false;
        }

        private int GetCategory(string str)
        {
            string[] pat = {@"雨水", @"污水",@"合流"};
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }

        private int GetType(string str)
        {
            string[] pat = { @"排水井", @"接户井", @"闸阀井", @"溢流井", @"倒虹井", @"透气井" ,
                           @"压力井", @"检测井", @"拍门井", @"截流井", @"水封井", @"跌水井" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }

        private int GetStyle(string str)
        {
            string[] pat = { @"一通",@"二通直",@"二通转",
                             @"三通",@"四通",@"五通",@"五通以上",@"暗井",@"侧立型",@"平面型",@"出水口" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }

        private int GetShapeType(string str)
        {
            string[] pat = { @"圆形", @"梯形", @"三角形", @"椭圆形", @"矩形", @"不规则形状" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }

        private int Getmaterial(string str)
        {
            string[] pat = { @"混凝土管",@"钢筋混凝土管",@"陶土管",
                             @"PE聚乙烯管",@"HDPE高密度聚乙烯管",
                            @"UPVC管",@"铸铁管",@"玻璃钢夹沙管",@"钢管",@"石棉水泥管" };
            int i = 0;
            if (str == null || str.Length <= 0)
                return 11;
            string pat1 = @".{0,}" + @str + @".{0,}";
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(pattern, pat1))
                    return i;
            }

            return i;
        }

        private int GetDataSource(string str)
        {
            string[] pat = { @"设计图", @"竣工图", @"现场测绘", @"人工估计" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }
        
        private int CheckID(string str ,  List<CJuncInfo> list)
        {
            string pattern = @"(.{0,}" + @str + @"\D+)|(.*" + @str + "$)";
            foreach(CJuncInfo junc in list)
            {
               if (Regex.IsMatch(junc.JuncName, pattern))
               {
                   return junc.ID;
               }
            }
            return -1;
        }

        private int GetPipeID(string pipename, List<CPipeInfo> list)
        {
            
            foreach (CPipeInfo pipe in list)
            {
                string pattern = @"(.{0,}"+@pipe.PipeName + @"+\D)|(.*"+pipe.PipeName+"$)";
                if (Regex.IsMatch(pipename, pattern))
                {
                    return pipe.ID;
                }
            }
            return -1;
        }

        private int GetCheckMethod(string str)
        {
            string[] pat = { @"CCTV", @"声纳", @"潜望镜" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }

            return i;
        }

        private int GetCheckDir(string str)
        {
            string[] pat = { @"与水流向一致", @"与水流方向不一致" };
            int i = 0;
            foreach (string pattern in pat)
            {
                i++;
                if (Regex.IsMatch(str, pattern))
                    return i;
            }
            if (i >= pat.Count())
                return 1;
            return i;
        }

        private int GetFuncDef(string str)
        {
            string[] pat = { @"无缺陷", @"沉积", @"结垢", @"障碍物", @"残墙坝根", @"树根", @"浮渣", @"封堵" };
            int i = 0;
            foreach (string pattern in pat)
            {
                if (Regex.IsMatch(str, pattern))
                    return i; 
                i++;
            }

            return i;
        }

        private int GetStructDef(string str)
        {
            string[] pat = { @"无缺陷", @"破裂", @"变形", @"腐蚀", @"错口", @"起伏", @"脱节", @"接口材料脱落",
                           @"支管暗接", @"异物穿入", @"渗漏"};
            int i = 0;
            foreach (string pattern in pat)
            {
                if (Regex.IsMatch(str, pattern))
                    return i;
                i++;
            }

            return i;
        }

        private  int GetClass(string str)
        {
            string[] pat = { @"Ⅰ|I", @"Ⅱ", @"Ⅲ", @"Ⅳ", @"Ⅴ", @"Ⅵ", @"Ⅶ", @"Ⅷ",
                           @"Ⅸ"};
            int i = 0;
            foreach (string pattern in pat)
            { 
                i++;
                string p = @".{0,}" + @pattern + @".{0,}$";
                if (Regex.IsMatch(str, p))
                    return i;
               
            }
            if (i >=pat.Count())
                return 0;
            return i;
        }

        private bool IsDouble(string str)
        {
            if (str == null)
                return false;
            string pattern = @"^[0-9]+(.[0-9]{n})?$";
            return Regex.IsMatch(str, pattern);
        }

        private DataSet dataset;
    }
}
