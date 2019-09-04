using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfConstellation.Shapes
{
    public static class CircleSpawner
    {
        public static Circle Spawn()
        {
            return new Circle();
        }
    }

    public class Circle
    {
        public Ellipse Shape;
        public int Speed { get; set; }

        public Circle()
        {
            var size = StaticRandom.Next(3, 6);
            Shape = new Ellipse()
                    {
                        Width = size,
                        Height = size,
                        Fill = Brushes.White
                    };
            Shape.RenderTransform = AnimatedTransform(CreateAnimation(), CreateAnimation());
        }

        private DoubleAnimation CreateAnimation()
        {
            var animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            animation.EasingFunction = new SineEase();
            int max = 50;
            animation.To = StaticRandom.Next(-max, max);
            animation.IsAdditive = true;

            return animation;
        }

        private TranslateTransform AnimatedTransform(DoubleAnimation xAnimation, DoubleAnimation yAnimation)
        {
            var transform = new TranslateTransform();
            int max = 50;
            xAnimation.Completed += (sender, args) =>
                                    {
                                        xAnimation.To = StaticRandom.Next(-max, max);
                                        transform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
                                    };

            yAnimation.Completed += (sender, args) =>
                                    {
                                        yAnimation.To = StaticRandom.Next(-max, max);
                                        transform.BeginAnimation(TranslateTransform.YProperty, yAnimation);
                                    };

            transform.BeginAnimation(TranslateTransform.XProperty, xAnimation);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnimation);

            return transform;
        }
    }
}