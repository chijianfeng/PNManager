using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeNetManager.utils
{
    class ValueConvert
    {
        public static double str2double(string str)
        {
            return Double.Parse(str);
        }

        public static DateTime str2time(string str)
        {
            return DateTime.Parse(str);
        }

        public static bool str2bool(string str)
        {
            if (str.Equals("是"))
                return true;
            else if (str.Equals("否"))
                return false;
            else
                return bool.Parse(str);
        }
    }
}
