using GIS.Arc;
using PipeNetManager.common;
using PipeNetManager.eMap.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace PipeNetManager.UndoRedo
{
    class PipeDelCommand : BaseCmd
    {
        private PipeState mState;               //保存数据实际操作者

        private Path mPath;         //real data

        private int mCurId;         //current id

        public PipeDelCommand(PipeState state, Path path)
        {
            mState = state;
            mPath = path;
        }

        public void Excute()
        {
            if (mState == null) return;
            if (mState.DelPipe(mPath))
            {
                Pipe pipe = mPath.ToolTip as Pipe;
                mState.DelPipeFromData(pipe);
            }
        }

        public void Undo()
        {
            if (mState == null)
                return;
            Pipe pipe = mPath.ToolTip as Pipe;
            mPath = mState.AddPipe(pipe, new VectorLine(pipe.Start.Location, pipe.End.Location));
            Cover start = new Cover();
            start.juncInfo = new DBCtrl.DBClass.CJuncInfo();
            start.juncInfo.ID = pipe.pipeInfo.In_JunID;

            Cover end = new Cover();
            end.juncInfo = new DBCtrl.DBClass.CJuncInfo();
            end.juncInfo.ID = pipe.pipeInfo.Out_JunID;
            mCurId = mState.AddPipe2Data(pipe, start, end);
        }

        public void Redo()
        {
            if (mState == null) return;
            if (mPath == null) return;

            mState.delPipe(mPath);
            Pipe pipe = mPath.ToolTip as Pipe;
            pipe.pipeInfo.ID = mCurId;
            mState.DelPipeFromData(pipe);
        }
    }
}
