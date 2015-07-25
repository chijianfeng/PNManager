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
    class PipeAddCommand : BaseCmd
    {
        private PipeState mState;               //保存数据实际操作者
        private Pipe mPipe;                     //保存管道数据

        private Path mPath;                     //保存操作的数

        private Cover mInJunc;
        private Cover mOutJunc;

        public PipeAddCommand(PipeState state, Pipe pipe , Cover injunc , Cover outjunc)
        {
            mState = state;
            mPipe = pipe;
            mInJunc = injunc;
            mOutJunc = outjunc;
        }

        public void Excute()
        {
            if (mState == null || mPipe == null) return;
            //先插入到数据库，获取id，后插入图层中
            int id = mState.AddPipe2Data(mPipe , mInJunc , mOutJunc);
            mPipe.pipeInfo.ID = id;
            mPath = mState.AddPipe(mPipe, new VectorLine(mPipe.Start.Location , mPipe.End.Location));
        }

        public void Undo()
        {
            if (mState == null || mPipe == null) return;
            mState.delPipe(mPath);
            mState.DelPipeFromData(mPipe);
        }

        public void Redo()
        {
            Excute();
        }
    }
}
