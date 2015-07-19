using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeNetManager.common
{
    enum PIPETYPE
    {
        PIPE_RAIN = 1,
        PIPE_WASTE = 2,
        PIPE_COLLABORATE = 3,
        PIPE_OTHER = 4
    }

    enum JUNCTYPE
    {
        [IntegerValue(1)]
        JUNC_RAIN = 1,

        [IntegerValue(2)]
        JUNC_WASTE = 2,

        [IntegerValue(3)]
        JUNC_COLLABORATE = 3,

        [IntegerValue(4)]
        JUNC_OTHER = 4,
    }

    public class IntegerValue : System.Attribute{
        private int _v;
        public IntegerValue(int value) 
        { 
            _v = value; 
        }
        public int Value
        {
            get { return _v; }
        } 
    }
    class Constants
    {
        public const  String PIPENONENAME = "-";
    }
}
