using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeNetManager.UndoRedo
{
    //该类是所有命令的基类,
    public interface  BaseCmd
    {
        //执行
        void Excute();

        void Undo();

        void Redo();
    }
}
