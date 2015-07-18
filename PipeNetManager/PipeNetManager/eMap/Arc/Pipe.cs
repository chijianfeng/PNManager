using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DBCtrl.DBClass;
using PipeNetManager.common;

namespace GIS.Arc
{
    /// <summary>
    /// 管道
    /// </summary>
    public class Pipe : IRelatedShape
    {
        public static int NUM;
        public CPipeInfo pipeInfo = null;
        public CPipeExtInfo pipeExtInfo = null;
        public CUSInfo UsInfo = null;


        public Pipe(IConcreteShape start, IConcreteShape end)
        {
            this.Name = "管道" + NUM++;
            this.Info = "无";
            this.Start = start;
            this.End = end;
        }

        public Pipe(String name, String info, IConcreteShape start, IConcreteShape end)
        {
            this.Name = name;
            this.Info = info;
            this.Start = start;
            this.End = end;
        }

        public override string ToString()
        {
            String msg = base.ToString();
            return msg;
        }
    }

    /// <summary>
    /// 雨水管道
    /// </summary>
    public class RainPipe : Pipe
    {
        public RainPipe(IConcreteShape start, IConcreteShape end)
            : base(start, end)
        {
            if (pipeInfo == null)
            {
                pipeInfo = new CPipeInfo();
                pipeInfo.Pipe_Category = (int)PIPETYPE.PIPE_RAIN;
            }
        }

        public RainPipe(String name, String info, IConcreteShape start, IConcreteShape end)
            : base(name, info, start, end)
        {
            if (pipeInfo == null)
            {
                pipeInfo = new CPipeInfo();
                pipeInfo.Pipe_Category = (int)PIPETYPE.PIPE_RAIN;
            }
        }

        public override SolidColorBrush GetColorBrush()
        {
            if (UsInfo != null)
            {
                Color c = ColorCenter.GetInstance().GetRainDefectPipeColor(UsInfo.Struct_Class);
                return new SolidColorBrush(c);
            }
            else
                return ColorCenter.GetInstance().Pipe_Rain_Fill_Color;
        }
    }

    /// <summary>
    /// 污水管道
    /// </summary>
    public class WastePipe : Pipe
    {
        public WastePipe(IConcreteShape start, IConcreteShape end)
            : base(start, end)
        {
            if (pipeInfo == null)
            {
                pipeInfo = new CPipeInfo();
                pipeInfo.Pipe_Category = (int)PIPETYPE.PIPE_WASTE;
            }
        }
        public WastePipe(String name, String info, IConcreteShape start, IConcreteShape end)
            : base(name, info, start, end)
        {
            if (pipeInfo == null)
            {
                pipeInfo = new CPipeInfo();
                pipeInfo.Pipe_Category = (int)PIPETYPE.PIPE_WASTE;
            }
        }
        public override SolidColorBrush GetColorBrush()
        {
            if (UsInfo != null)
            {
                Color c = ColorCenter.GetInstance().GetWasteDefectPipeColor(UsInfo.Struct_Class);
                return new SolidColorBrush(c);
            }
            else
                return ColorCenter.GetInstance().Pipe_Waste_Fill_Color;

        }

    }
}
