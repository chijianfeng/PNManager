using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelOper.Data
{
    class PipeSheetData
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
        /// 管道类别
        /// </summary>
        public string PipeCategory
        {
            set;
            get;
        }

        /// <summary>
        /// 起点编码
        /// </summary>
        public string InCode
        {
            set;
            get;
        }

        /// <summary>
        /// 终点编码
        /// </summary>
        public string OutCode
        {
            set;
            get;
        }
        /// <summary>
        /// 起点管顶标高
        /// </summary>
        public double In_UpEle
        {
            set;
            get;
        }
        /// <summary>
        /// 起点管底标高
        /// </summary>
        public double In_BottomEle
        {
            set;
            get;
        }
        /// <summary>
        /// 终点管顶标高
        /// </summary>
        public double Out_UpEle
        {
            set;
            get;
        }
        /// <summary>
        /// 终点管底标高
        /// </summary>
        public double Out_BottomEle
        {
            set;
            get;
        }
        /// <summary>
        /// 起点实测管径
        /// </summary>
        public double In_R
        {
            set;
            get;
        }
        /// <summary>
        /// 终点实测管径
        /// </summary>
        public double Out_R
        {
            set;
            get;
        }
        /// <summary>
        /// 断面形式
        /// </summary>
        public string ShapeType
        {
            set;
            get;
        }
        /// <summary>
        /// 断面数据
        /// </summary>
        public string ShapeData
        {
            set;
            get;
        }
        /// <summary>
        /// 管道材质
        /// </summary>
        public string Matrial
        {
            set;
            get;
        }
        /// <summary>
        /// 管顶糙率
        /// </summary>
        public double Roughness
        {
            set;
            get;
        }

        public string DataSource
        {
            set;
            get;
        }

        /// <summary>
        /// 数据获取时间
        /// </summary>
        public DateTime DataTime
        {
            set;
            get;
        }
        /// <summary>
        /// 填报单位
        /// </summary>
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
