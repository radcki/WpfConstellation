using Constellation.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Constellation.ViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace Constellation.ValueConverter
{
    public class NodeToPathStrokeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] is Node node && node.ConnectedPoints != null && node.ConnectedPoints.Any())
            {
                var stroke = new SolidColorBrush();
                var distance = node.ConnectedPoints
                                   .Select(next => (new Point(0, 0) - new Point(next.X - node.X, next.Y - node.Y)).LengthSquared)
                                   .Average();

                stroke.Color = Color.FromRgb(255, 255, 255);
                var viewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                stroke.Opacity = Math.Min(1, 1 - (distance / viewModel.ConnectionDistance));
                if (parameter is string opacityMultiplier)
                {
                    var multiplier = double.Parse(opacityMultiplier, CultureInfo.InvariantCulture);
                    stroke.Opacity = Math.Max(0, Math.Min(1, stroke.Opacity * multiplier));
                }
                return stroke;
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}