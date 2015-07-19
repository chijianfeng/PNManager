using GIS.Arc;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PipeNetManager.UndoRedo
{
    public class JuncAddCommand : BaseCmd
    {
        private JuncState mState;   //real operator
        private Cover mCover;       //real data

        private Path mPath;
        public JuncAddCommand(JuncState state , Cover c)
        {
            mState = state;
            mCover = c;
        }

        public void Excute()
        {
            if (mState == null)
                return;
            mPath =  mState.AddJunc(mCover);
            mState.AddJunc2Data(mCover);
        }

        public void Undo()          //delete the junction
        {
            if (mState == null) return;
            if (mPath == null) return;
            mState.delJunc(mPath);
            mState.DelJuncFromData(mCover);
        }

        public void Redo()          //add the junction
        {
            Excute();
        }

    }
}
