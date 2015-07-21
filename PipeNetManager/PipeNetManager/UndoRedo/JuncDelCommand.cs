using GIS.Arc;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PipeNetManager.UndoRedo
{
    class JuncDelCommand : BaseCmd
    {
        private JuncState mState;   //real operator

        private Path mPath;         //real data

        public JuncDelCommand(JuncState state, Path path)
        {
            mState = state;
            mPath = path;
        }
        public void Excute()
        {
            if (mState == null)
                return;
            if (mState.DelJunc(mPath)){
                Cover c = mPath.ToolTip as Cover;
                mState.DelJuncFromData(c);
            }
        }

        public void Undo()
        {
            if (mState == null)
                return;
            Cover c = mPath.ToolTip as Cover;
            mPath = mState.AddJunc(c);
            mState.AddJunc2Data(c);
        }

        public void Redo()
        {
            if (mState == null) return;
            if (mPath == null) return;
            mState.delJunc(mPath);
            Cover c = mPath.ToolTip as Cover;
            mState.DelJuncFromData(c);
        }
    }
}
