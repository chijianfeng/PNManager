using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PipeNetManager.utils
{
    class AnimationUtil
    {
        public static  void ScaleEasingAnimation(FrameworkElement element)
        {
            ScaleTransform scale = new ScaleTransform();
            element.RenderTransform = scale;
            element.RenderTransformOrigin = new Point(0.5, 0.5);//定义圆心位置
            EasingFunctionBase easing = new ElasticEase()
            {
                EasingMode = EasingMode.EaseOut,            //公式
                Oscillations = 1,                           //滑过动画目标的次数
                Springiness = 10                             //弹簧刚度
            };
            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                From = 0,                                 //起始值
                To = 1,                                     //结束值
                EasingFunction = easing,                    //缓动函数
                Duration = new TimeSpan(0, 0, 0, 1, 200)    //动画播放时间
            };
            AnimationClock clock = scaleAnimation.CreateClock();
            scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clock);
            scale.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clock);
        }
    }
}
