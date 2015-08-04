using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PipeNetManager.utils
{
    class ExceptionProcess : Exception
    {
        private string mCauseReson;
        public ExceptionProcess(string s){
            mCauseReson = s;
        }

        public string getReson()
        {
            return mCauseReson;
        }

        public override string ToString()
        {
            return mCauseReson;
        }
    }
}
