using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PipeNetManager.common
{
    class VectorLine
    {
        public VectorLine(Point sp, Point ep)
        {
            StartPoint = sp;
            EndPoint = ep;
        }
        public Point StartPoint;            //矢量线条的起始点，对应物理屏幕
        public Point EndPoint;              //矢量线条的终止点，对应物理屏幕
    }
}
