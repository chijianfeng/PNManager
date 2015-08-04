using GIS.Arc;
using GIS.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PipeNetManager.utils.Assist
{
     public  class CalLenAction : BaseAction
     {
        private Path mMovingPath;

        private bool mbCancle = true;

        public static new String getType()
        {
            return "CalLenAction";
        }

        public override void OnMouseLeftDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point newp = e.GetPosition(mCanvas);
            Point mp = GetMercator(newp);

            mMPoints.Add(mp);
            mSPoint.Add(newp);

            if (mMPoints.Count>1)
            {
                Path line = createPath(mSPoint[mSPoint.Count - 2], mSPoint[mSPoint.Count - 1]);
                mPath.Add(line);
                mCanvas.Children.Add(line);

                Point sp = Mercator2WGS84(mMPoints[mSPoint.Count - 2]);
                Point ep = Mercator2WGS84(mMPoints[mSPoint.Count - 1]);
                DrawText(WGS84Distance(sp , ep)+"m", mSPoint[mSPoint.Count - 2], mSPoint[mSPoint.Count - 1], 13);
            }
            mbCancle = false;
        }

        public override void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        public override void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mSPoint == null || mSPoint.Count <= 0) return;
            if (mbCancle) return;
            if (mMovingPath == null)
            {
                mMovingPath = createPath(mSPoint.ElementAt(mSPoint.Count - 1), e.GetPosition(mCanvas));
            }
            else
            {
                mCanvas.Children.Remove(mMovingPath);
                UpdatePath(mMovingPath, mSPoint.ElementAt(mSPoint.Count - 1), e.GetPosition(mCanvas));
            }
            mCanvas.Children.Add(mMovingPath);
        }

        public override void OnViewOriginal(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        //取消画线
        public override void OnMouseRightDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mCanvas.Children.Remove(mMovingPath);
            mMovingPath = null;
            mbCancle = true;
        }
    }
}
