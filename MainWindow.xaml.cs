using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfConstellation.Shapes;

namespace WpfConstellation
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Ellipse> Points { get; set; }
        public ObservableCollection<Line> Lines { get; set; }

        public MainWindow()
        {
            Points = new ObservableCollection<Ellipse>();
            Lines = new ObservableCollection<Line>();
            DataContext = this;
            InitializeComponent();

            for (var i = 0; i <= 50; i++)
            {
                var circle = CircleSpawner.Spawn();
                var shape = circle.Shape;
                Canvas.SetTop(shape, StaticRandom.Next(0, (int) Height));
                Canvas.SetLeft(shape, StaticRandom.Next(0, (int) Width));
                Points.Add(shape);

                var line = new Line()
                           {
                               X1 = 0,
                               Y1 = 0,
                               Fill = Brushes.LightYellow,
                               Stroke = Brushes.Cornsilk,
                           };
                var xBinding = new Binding()
                               {
                                   Path = new PropertyPath("Canvas.LeftProperty"),
                                   Source = shape
                               };
                var yBinding = new Binding()
                               {
                                   Path = new PropertyPath("Canvas.LeftProperty"),
                                   Source = shape
                               };

                BindingOperations.SetBinding(line, Line.X2Property,xBinding);
                BindingOperations.SetBinding(line, Line.Y2Property,yBinding);

                Lines.Add(line);
                AnimationArea.Children.Add(shape);
                AnimationArea.Children.Add(line);

                var timer = new Timer();
                timer.Interval = 5000;
                timer.Elapsed += (sender, args) =>
                                 {
                                     var test = "test";
                                 };
                    timer.Start();
            }

            /*
            var connectorsThread = new Thread(() =>
                                              {
                                                  Dispatcher.Invoke(() =>
                                                                    {
                                                                        while (true)
                                                                        {
                                                                            foreach (var line in Lines)
                                                                            {
                                                                                AnimationArea.Children.Remove(line);
                                                                            }

                                                                            Lines.Clear();
                                                                            foreach (var ellipsis in Points)
                                                                            {
                                                                                var yPosition = Canvas.GetTop(ellipsis);
                                                                                var xPosition = Canvas.GetLeft(ellipsis);
                                                                                var line = new Line()
                                                                                           {
                                                                                               X1 = 0,
                                                                                               X2 = xPosition,
                                                                                               Y1 = 0,
                                                                                               Y2 = yPosition,
                                                                                               Fill = Brushes.Cornsilk,
                                                                                               Stroke = Brushes.Cornsilk,
                                                                                           };
                                                                                Lines.Add(line);
                                                                                AnimationArea.Children.Add(line);
                                                                            }
                                                                        }
                                                                    });
                                              });
            connectorsThread.Start();
            */
        }
    }
}