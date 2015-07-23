using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BLL.Command;
using BLL.Receiver;
using DBCtrl.DBClass;
using DBCtrl.DBRW;
using PipeNetManager;
using PipeNetManager.common;

namespace GIS.Arc
{
    public class ArcMap
    {
        InsertCmd icmd = new InsertCmd();
        LoadCmd lcmd = new LoadCmd();
        SelectCmd scmd = new SelectCmd();
        UpdateCmd ucmd = new UpdateCmd();
        DeleteCmd dcmd = new DeleteCmd();
        ClearCmd ccmd = new ClearCmd();

        public List<RainPipe> RainPipeList { get; set; }
        public List<WastePipe> WastePipeList { get; set; }
        public List<RainCover> RainCoverList { get; set; }
        public List<WasteCover> WasterCoverList { get; set; }
        public ArcMap()
        {
            RainPipeList = new List<RainPipe>();
            WastePipeList = new List<WastePipe>();
            RainCoverList = new List<RainCover>();
            WasterCoverList = new List<WasteCover>();
        }

        public void LoadRainCover() 
        {
            TJuncInfo juninfo = new TJuncInfo(App._dbpath, App.PassWord);
            List<CJuncInfo> tmplist = juninfo.Sel_JuncInfoByCaty((int)JUNCTYPE.JUNC_RAIN);            //仅仅加载雨水检查井
            //进行坐标转换
            foreach (CJuncInfo mJunc in tmplist)
            {
                if (mJunc.X_Coor == 0)                                           //无座标
                    continue;
                RainCover cover = null;
                Point p = new Point(mJunc.X_Coor - Constants.COOR_X_OFFSET, mJunc.Y_Coor - Constants.COOR_Y_OFFSET);

                cover = new RainCover(mJunc.JuncName, GISConverter.WGS842Merator(p), mJunc.SystemID);
                cover.juncInfo = mJunc;
                RainCoverList.Add(cover);
            }
        }

        public void LoadWasterCover() {

            TJuncInfo juninfo = new TJuncInfo(App._dbpath, App.PassWord);
            List<CJuncInfo> tmplist = juninfo.Sel_JuncInfoByCaty((int)JUNCTYPE.JUNC_WASTE);            //仅仅加载污水检查井
            //进行坐标转换
            foreach (CJuncInfo mJunc in tmplist)
            {
                if (mJunc.X_Coor == 0)                                           //无座标
                    continue;
                WasteCover cover = null;
                Point p = new Point(mJunc.X_Coor - Constants.COOR_X_OFFSET, mJunc.Y_Coor - Constants.COOR_Y_OFFSET);

                cover = new WasteCover(mJunc.JuncName, GISConverter.WGS842Merator(p), mJunc.SystemID);
                cover.juncInfo = mJunc;
                WasterCoverList.Add(cover);
            }
        }

        public void LoadRainPipe() {
            TPipeInfo pipeinfo = new TPipeInfo(App._dbpath, App.PassWord);   //读取数据库
            List<CPipeInfo> pipelist = pipeinfo.Sel_PipeInfo((int)PIPETYPE.PIPE_RAIN);             //仅仅读取雨水管道

            TUSInfo usinfo = new TUSInfo(App._dbpath, App.PassWord);
            List<CUSInfo> uslist = usinfo.Load_USInfo();

            foreach (CPipeInfo info in pipelist)
            {
                RainPipe pipe = null;
                RainCover starjunc = FindStartRJunc(info);                    //找到起始点坐标
                RainCover endjunc = FindEndRJunc(info);                       //找到终止点坐标
                if (starjunc == null || endjunc == null)
                    continue;

                pipe = new RainPipe(starjunc, endjunc);

                pipe.pipeInfo = info;
                pipe.UsInfo = FindUSInfo(uslist, info.ID);
                RainPipeList.Add(pipe);
            }
        }

        public void LoadWasterPipe() {
            TPipeInfo pipeinfo = new TPipeInfo(App._dbpath, App.PassWord);   //读取数据库
            List<CPipeInfo> pipelist = pipeinfo.Sel_PipeInfo((int)PIPETYPE.PIPE_WASTE);             //仅仅读取污水管道

            //读取管道内窥数据
            TUSInfo usinfo = new TUSInfo(App._dbpath, App.PassWord);
            List<CUSInfo> uslist = usinfo.Load_USInfo();

            foreach (CPipeInfo info in pipelist)
            {
                WastePipe pipe = null;
                WasteCover starjunc = FindStartWJunc(info);                    //找到起始点坐标
                WasteCover endjunc = FindEndWJunc(info);                       //找到终止点坐标
                if (starjunc == null || endjunc == null)
                    continue;

                pipe = new WastePipe(starjunc, endjunc);

                pipe.pipeInfo = info;
                pipe.UsInfo = FindUSInfo(uslist, info.ID);
                WastePipeList.Add(pipe);
            }
        }
        private CUSInfo FindUSInfo(List<CUSInfo> usinfolist,int pipeId)
        {
            CUSInfo info = null;
            info = usinfolist.Find(us => us.PipeID == pipeId);
            return info;
        }

        public void Save()
        { 
        
        }

        RainCover FindStartRJunc(CPipeInfo cp)
        {
            RainCover c = null;
            c = RainCoverList.Find(cc => cc.juncInfo.ID == cp.In_JunID);
            return c;
        }
        RainCover FindEndRJunc(CPipeInfo cp)
        {
            RainCover c = null;
            c = RainCoverList.Find(cc => cc.juncInfo.ID == cp.Out_JunID);
            return c;
        }

        WasteCover FindStartWJunc(CPipeInfo cp)
        {
            WasteCover c = null;
            c = WasterCoverList.Find(cc => cc.juncInfo.ID == cp.In_JunID);
            return c;
        }
        WasteCover FindEndWJunc(CPipeInfo cp)
        {
            WasteCover c = null;
            c = WasterCoverList.Find(cc => cc.juncInfo.ID == cp.Out_JunID);
            return c;
        }

        /// <summary>
        /// 查找矩形坐标范围内的井盖集合
        /// </summary>
        /// <param name="Start">左上角坐标</param>
        /// <param name="End">右下角坐标</param>
        /// <returns></returns>
        public List<Cover> FindCover(Point Start,Point End)
        {
            List<Cover> list = new List<Cover>();
            foreach (Cover c in RainCoverList)
            {
                if (c.Location.X < Start.X || c.Location.X > End.X ||
                    c.Location.Y > Start.Y || c.Location.Y < End.Y)
                    continue;
                else
                    list.Add(c);
            }
            foreach (Cover c in WasterCoverList)
            {
                if (c.Location.X < Start.X || c.Location.X > End.X ||
                    c.Location.Y > Start.Y || c.Location.Y < End.Y)
                    continue;
                else
                    list.Add(c);
            }
            return list;
        }

        /// <summary>
        /// 查找与指定井盖集合相关的管道集合
        /// </summary>
        /// <param name="Covers">井盖集合</param>
        /// <returns></returns>
        public List<Pipe> FindPipe(List<Cover> Covers)
        {
            List<Pipe> list = new List<Pipe>();
            foreach (Cover c in Covers)
            {
                if (c.Out_Pipe != null)
                    list.Add(c.Out_Pipe);
            }
            return list;
        }

        public List<Cover> FindCover(List<Pipe> Pipes)
        {
            List<Cover> covers = new List<Cover>();
            foreach (Pipe p in Pipes)
            {
                if (!covers.Contains(p.Start))
                    covers.Add((Cover)p.Start);
                if (!covers.Contains(p.End))
                    covers.Add((Cover)p.End);
            }
            return covers;
        }
    }
}
