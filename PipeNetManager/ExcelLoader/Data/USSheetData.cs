using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelOper.Data
{
    class USSheetData
    {
        /// <summary>
        /// 排水管标识码
        /// </summary>
        public string PipeName
        {
            set; 
            get;
        }

        /// <summary>
        /// 排水系统编码/路名
        /// </summary>
        public string RoadName
        {
            set;
            get;
        }
        /// <summary>
        /// 起点编码
        /// </summary>
        public string In_Code
        {
            set;
            get;
        }
        /// <summary>
        /// 终点编码
        /// </summary>
        public string Out_Code
        {
            set;
            get;
        }

        /// <summary>
        /// 作业编号
        /// </summary>
        public string JobID
        {
            set;
            get;
        }

        /// <summary>
        /// 检测日期
        /// </summary>
        public DateTime CheckDate
        {
            set;
            get;
        }
        /// <summary>
        /// 检测单位
        /// </summary>
        public string CheckDept
        {
            set;
            get;
        }

        /// <summary>
        /// 检测人
        /// </summary>
        public string CheckPerson
        {
            set;
            get;
        }

        /// <summary>
        /// 检测单位联系方式
        /// </summary>
        public string CheckContact
        {
            set;
            get;
        }
        /// <summary>
        /// 检测方法
        /// </summary>
        public string CheckMethod
        {
            set;
            get;
        }

        /// <summary>
        /// 检测方向
        /// </summary>
        public string CheckDirect
        { set; get; }

        /// <summary>
        /// 封堵情况
        /// </summary>
        public string Pipe_Stop
        {
            set;
            get;
        }
        /// <summary>
        /// 功能性缺失
        /// </summary>
        public string Func_Defect
        {
            set;
            get;
        }
        /// <summary>
        /// 功能性缺陷等级
        /// </summary>
        public string Func_Class
        {
            set;
            get;
        }

        /// <summary>
        /// 结构性缺陷
        /// </summary>
        public string Struct_Defect
        {
            set;
            get;
        }

        /// <summary>
        /// 结构性缺陷等级
        /// </summary>
        public string Struct_Class
        {
            set;
            get;
        }
        /// <summary>
        /// 修复指数 RI
        /// </summary>
        public string Repire_Index
        {
            set;
            get;
        }

        /// <summary>
        /// 养护指数 MI
        /// </summary>
        public string Matain_Index
        { set; get; }
        /// <summary>
        /// 缺陷描述
        /// </summary>
        public string Problem
        {
            set;
            get;
        }
        /// <summary>
        /// 检测影像文件的文件名
        /// </summary>
        public string Video_Filename
        {
            set;
            get;
        }

        public string RecordDept
        {
            set;
            get;
        }
        /// <summary>
        /// 填报日期
        /// </summary>
        public DateTime RecordTime
        {
            set;
            get;
        }
        /// <summary>
        /// 数据是否完整
        /// </summary>
        public bool DataFull
        {
            set;
            get;
        }
        /// <summary>
        /// 数据缺失原因
        /// </summary>
        public string LoseReson
        {
            set;
            get;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set;
            get;
        }
    }
}
