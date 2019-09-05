using Constellation.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Constellation.ValueConverter
{
    public class NodeToPathDataConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] is Node node && node.ConnectedPoints != null)
            {
                // Create a StreamGeometry to draw line(s) from the current to the next node(s).
                StreamGeometry geometry = new StreamGeometry();
                using (StreamGeometryContext ctx = geometry.Open())
                {
                    if (node.ConnectedPoints.Any())
                    {
                        var currentPoint = new Point(0, 0);
                        foreach (var nextNode in node.ConnectedPoints)
                        {
                            ctx.BeginFigure(currentPoint, true, true);

                            var endPoint = new Point(nextNode.X - node.X, nextNode.Y - node.Y);
                            ctx.LineTo(endPoint, true, false);
                            currentPoint = endPoint;
                        }
                        ctx.LineTo(new Point(0, 0), true, false);
                    }

                    geometry.FillRule = FillRule.EvenOdd;
                }
                geometry.Freeze();
                return geometry;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
