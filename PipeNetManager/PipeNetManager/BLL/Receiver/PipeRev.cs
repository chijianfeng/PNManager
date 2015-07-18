using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBCtrl.DBRW;
using DBCtrl.DBClass;

namespace BLL.Receiver
{
    public class PipeRev : BasicRev
    {
        /// <summary>
        /// 管道基本信息
        /// </summary>
        public List<CPipeInfo> ListPipe
        {
            set;
            get;
        }
        /// <summary>
        /// 管道附加信息
        /// </summary>
        public List<CPipeExtInfo> ListPipeExt
        {
            set;
            get;
        }

        /// <summary>
        /// 管道内壁检查信息
        /// </summary>
        public List<CUSInfo> ListUS
        {
            set;
            get;
        }

        /// <summary>
        /// 管道名称
        /// </summary>
        public string PipeName
        {
            set;
            get;
        }

        public PipeRev()
        {
            ListPipe = null;
            ListPipeExt = null;
        }

        public override bool Docmd(string cmd)
        {
            if (cmd.Equals("Load"))
            {
                return DoLoad();
            }
            else if (cmd.Equals("Select"))
            {
                return DoSelect();
            }
            else if (cmd.Equals("Update"))
            {
                return DoUpdate();
            }
            else if (cmd.Equals("Insert"))
            {
                return DoInsert();
            }
            else if (cmd.Equals("Delete"))
            {
                return DoDelete();
            }
            else if (cmd.Equals("Clear"))
            {
                return DoClear();
            }
            else if (cmd.Equals("QuickInsert"))
            {
                return DoQuickInsert();
            }
            return true;
        }

      

        /// <summary>
        /// 导入管道信息，附加信息;但不会导入管道检测信息，管道日志，图片，报告，视频信息
        /// </summary>
        /// <returns></returns>
        private bool DoLoad()
        {
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);
            pipeinfo.OpenDB();
            pipextinfo.OpenDB();
            usinfo.OpenDB();

            ListPipe = pipeinfo.Load_PipeInfo();
            ListPipeExt = pipextinfo.Load_PipeExtInfo();
            ListUS = usinfo.Load_USInfo();

            pipeinfo.CloseDB();
            pipextinfo.CloseDB();
            usinfo.CloseDB();
            if (ListPipe == null || ListPipeExt == null||ListUS==null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 根据管道名称获取管道信息，附加信息;管道检测信息，管道日志，图片，报告，视频信息
        /// </summary>
        /// <returns></returns>
        private bool DoSelect()
        {
            if (PipeName == null || PipeName.Length <= 0)
                return false;
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);

            ListPipe = pipeinfo.Sel_PipeInfo(PipeName);
            if (ListPipe == null || ListPipe.Count <= 0)
                return false;

            ListPipeExt = new List<CPipeExtInfo>();
            ListUS = new List<CUSInfo>();

            foreach(CPipeInfo pipe in ListPipe)
            {
                int id = pipe.ID;
                List<CPipeExtInfo> list1 = pipextinfo.Sel_PipeExtInfo(id);
                if (list1 != null && list1.Count > 0)
                {
                    ListPipeExt.Add(list1.ElementAt(0));
                }
                List<CUSInfo> list2 = usinfo.Sel_USInfo(id);
                if (list2 != null && list2.Count > 0)
                {
                    ListUS.Add(list2.ElementAt(0));
                }
            }
            return true;
        }

        /// <summary>
        /// 更新管道信息，对现有数据更新，如不更新其他数据则设为null
        /// </summary>
        /// <returns></returns>
        private bool DoUpdate()
        {
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);

            pipeinfo.Update_PipeInfo(ListPipe);
            pipextinfo.Update_PipeExtInfo(ListPipeExt);
            usinfo.Update_USInfo(ListUS);
            return true;
        }

        /// <summary>
        /// 插入管道信息，和附加信息，管道检测信息，管道日志，图片，报告，视频信息
        /// </summary>
        /// <returns></returns>
        private bool DoInsert()
        {
            if (ListPipe == null)
                return false;
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);
            

            pipeinfo.OpenDB();
            pipextinfo.OpenDB();
            usinfo.OpenDB();
            

            int i = 0;
            foreach (CPipeInfo pipe in ListPipe)
            {
                CPipeInfo tmp = pipe;
                //插入附加信息
                CPipeExtInfo extmp = null;
                if (!pipeinfo.Insert_PipeInfo(ref tmp))
                {
                    continue;
                }
                if (ListPipeExt == null || ListPipeExt.Count == 0)
                {
                    extmp = new CPipeExtInfo();
                }
                else
                {
                    if (i < ListPipeExt.Count)
                        extmp = ListPipeExt.ElementAt(i);
                    else
                        extmp = new CPipeExtInfo();
                }
                extmp.PipeID = tmp.ID;
                pipextinfo.Insert_PipeExtInfo(ref extmp);

                //插入管道检测信息
                CUSInfo ustmp = null;
                if (ListUS == null || ListUS.Count == 0)
                {
                    ustmp = new CUSInfo();
                }
                else
                {
                    if (i < ListUS.Count)
                        ustmp = ListUS.ElementAt(i);
                    else
                        ustmp = new CUSInfo();
                }
                ustmp.PipeID = tmp.ID;
                usinfo.Insert_USInfo(ref ustmp);
                i++;
            }

            //close the db connection
            pipeinfo.CloseDB();
            pipextinfo.CloseDB();
            usinfo.CloseDB();
           

            return true;
        }
        private bool DoQuickInsert()
        {
            if (ListPipe == null||ListPipe.Count==0)
            {
                if (ListUS != null && ListUS.Count > 0)
                    return InsertUs();
                else
                    return false;
            }
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            pipeinfo.OpenDB();
            pipextinfo.OpenDB();

            List<int> listid = new List<int>();

            if (!pipeinfo.Insert_PipeInfo(ListPipe, ref listid))
                return false;

            int nCount = 0;
            List<CPipeExtInfo> newllist = new List<CPipeExtInfo>();
            foreach (CPipeExtInfo pipe in ListPipeExt)
            {
                pipe.PipeID = listid.ElementAt(nCount++);
                newllist.Add(pipe);
            }
            if (!pipextinfo.Insert_PipeExtInfo(ListPipeExt))
                return false;
            pipeinfo.CloseDB();
            pipextinfo.CloseDB();
            return true;
        }

        private bool InsertUs()
        {
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);

            usinfo.OpenDB();
            bool ret = usinfo.Insert_USInfo(ListUS);
            usinfo.CloseDB();
            return ret;
        }
        
        /// <summary>
        /// 删除管道的基本信息，同时删除其他关联信息,
        /// :附加信息，管道检测信息，管道日志，图片，报告，视频信息
        /// </summary>
        /// <returns></returns>
        private bool DoDelete()
        {
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);

            if (ListPipe == null || ListPipe.Count == 0)
                return false;

            foreach (CPipeInfo pipe in ListPipe)
            {
                pipeinfo.Delete_PipeInfo(pipe);
                
                CPipeExtInfo ext = null;
                ListPipeExt = pipextinfo.Sel_PipeExtInfo(pipe.ID);
                if (ListPipeExt != null && ListPipeExt.Count > 0)
                    ext = ListPipeExt.ElementAt(0);
                pipextinfo.Delete_PipeExtInfo(ext);

                CUSInfo us = null;
                ListUS = usinfo.Sel_USInfo(pipe.ID);
                if (ListUS!=null && ListUS.Count > 0)
                    us = ListUS.ElementAt(0);
                usinfo.Delete_USInfo(us);
            }

            return true;
        }

        private bool DoClear()
        {
            TPipeInfo pipeinfo = new TPipeInfo(_dbpath, PassWord);
            TPipeExtInfo pipextinfo = new TPipeExtInfo(_dbpath, PassWord);
            TUSInfo usinfo = new TUSInfo(_dbpath, PassWord);

            pipeinfo.Clear_PipeInfo();
            pipextinfo.Clear_PipeExtInfo();
            usinfo.Clear_USInfo();
            return true;
        }
    }
}
