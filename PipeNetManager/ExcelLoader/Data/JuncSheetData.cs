using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelOper.Data
{
    class JuncSheetData
    {
        /// <summary>
        /// 路名
        /// </summary>
        public string RoadName
        {
            set;
            get;
        }
        
        /// <summary>
        /// 测绘编号/检查井标识码
        /// </summary>
        public string JuncName
        {
            set;
            get;
        }

        /// <summary>
        /// 检查井类别
        /// </summary>
        public string JuncCategory
        {
            set;
            get;
        }
        /// <summary>
        /// 检查井类型
        /// </summary>
        public string JuncType
        {
            set;
            get;
        }
        /// <summary>
        /// 检查井形式
        /// </summary>
        public string JuncStyle
        {
            set;
            get;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            set;
            get;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            set;
            get;
        }
        /// <summary>
        /// 地面高程H
        /// </summary>
        public double Height
        {
            set;
            get;
        }
      
        /// <summary>
        /// 检查井井深
        /// </summary>
        public double Depth
        {
            set;
            get;
        }
        /// <summary>
        /// 校验的CCTV检测编号
        /// </summary>
        public string CCTV_CheckCode
        {
            set;
            get;
        }
        /// <summary>
        /// 有无暗管
        /// </summary>
        public bool JuncDarkjoint
        {
            set;
            get;
        }
        /// <summary>
        /// 污水直排
        /// </summary>
        public bool SewageLine
        {
            set;
            get;
        }
        /// <summary>
        /// 井盖错误
        /// </summary>
        public bool JuncError
        {
            set;
            get;
        }

        /// <summary>
        /// 井口至管顶距离
        /// </summary>
        public double[] Dis = new double[8];
        /// <summary>
        /// 数据来源
        /// </summary>
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
