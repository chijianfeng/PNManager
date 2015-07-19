using GIS.Arc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PipeNetManager.common
{
    class RectAdorner : Adorner
    {
        public RectAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
           Path p = (Path)(adornedElement);
           Cover c = p.ToolTip as Cover;
           Point cp = new Point();
           cp.X = ((c.Location.X - App.Tiles[0].X) / App.Tiles[0].Dx);
           cp.Y = ((App.Tiles[0].Y - c.Location.Y) / App.Tiles[0].Dy);

            _child = new Rectangle();
            _child.Width = App.StrokeThinkness * 2;
            _child.Height = App.StrokeThinkness * 2;
            
            VisualBrush _brush = new VisualBrush(adornedElement);

            DoubleAnimation animation = new DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)));
            animation.AutoReverse = true;
            animation.RepeatBehavior =  System.Windows.Media.Animation.RepeatBehavior.Forever;
            _brush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);

            _child.Fill = _brush;
        }

        public UIElement GetElement()
        {
            return mElement;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
           
            Rect newRect = new Rect(adornedElementRect.Right - App.StrokeThinkness * 2, adornedElementRect.Bottom - App.StrokeThinkness * 2,
                adornedElementRect.Right, adornedElementRect.Bottom);
            newRect.Inflate(1, 1);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Red);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);

            //drawingContext.DrawRectangle(renderBrush, renderPen, newRect);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        private Rectangle _child = null;
      
        private UIElement mElement;
    }
}
