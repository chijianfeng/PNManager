using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeNetManager.UndoRedo
{
    //单例模式
    public class CmdManager
    {
        private static CmdManager mCmdManager;

        private Stack<BaseCmd> mStackDone;              //存放完成的操作
        private Stack<BaseCmd> mStackReDo;              //存放撤销的操作

        private Stack<BaseCmd> mStackTmp;
        private static int MAXCMD = 100;                //定义命令最大容量 
        private static int REMOVENUM = 10;              //超过最大量，一次移除数目

        private CmdManager() {
            mStackDone = new Stack<BaseCmd>();
            mStackReDo = new Stack<BaseCmd>();
            mStackTmp = new Stack<BaseCmd>();
        }

        public static CmdManager getInstance() {
            if (mCmdManager == null)
            {
                mCmdManager = new CmdManager();
            }
            return mCmdManager;
        }

        public void PushCmd(BaseCmd cmd)
        {
            if (mStackDone.Count >= MAXCMD)
            {
                mStackTmp.Clear();
                for (int i = 0; i < mStackDone.Count - REMOVENUM; i++)
                {
                    mStackTmp.Push(mStackDone.Pop());
                }
                mStackDone.Clear();
                for (int i = 0; i < mStackTmp.Count; i++)
                {
                    mStackDone.Push(mStackTmp.Pop());
                }
            }
            mStackDone.Push(cmd);
        }

        //取消操作，将done栈弹出，redo 栈压入
        public Boolean Undo() {
            if (mStackDone.Count <= 0)
                return false;
            BaseCmd cmd = mStackDone.Pop();
            if (mStackReDo.Count >= MAXCMD)
            {
                mStackTmp.Clear();
                for (int i = 0; i < mStackReDo.Count - REMOVENUM; i++)
                {
                    mStackTmp.Push(mStackReDo.Pop());
                }
                mStackReDo.Clear();
                for (int i = 0; i < mStackTmp.Count; i++)
                {
                    mStackReDo.Push(mStackTmp.Pop());
                }
            }
            mStackReDo.Push(cmd);

            cmd.Undo();                       //执行操作

            return true;
        }


        //重做操作 ， 将redo 栈弹出，done 栈压入
        public Boolean Redo()
        {
            if (mStackReDo.Count <= 0)
                return false;
            BaseCmd cmd = mStackReDo.Pop();
            PushCmd(cmd);
            cmd.Redo();
            return true;
        }

        public int GetDoneCmdNumber()
        {
            return mStackDone.Count;
        }

        public int GetRedoCmdNumber()
        {
            return mStackReDo.Count;
        }

    }
}
