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

        private Path mPath;                     //保存操作的数据

        public PipeAddCommand(PipeState state, Path path)
        {
            mState = state;
            mPath = path;
        }

        public void Excute()
        {
            
        }

        public void Undo()
        {
           
        }

        public void Redo()
        {
            
        }
    }
}
