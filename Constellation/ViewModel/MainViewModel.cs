using Constellation.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Activities.Presentation.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using WpfConstellation;

namespace Constellation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int? _limitConnections;
        private readonly Stopwatch _stopwatch;
        private double _loopFrequency;

        public ICommand AddPoint => new RelayCommand(() =>
                                                     {
                                                         var newPoint = new Node
                                                                        {
                                                                            X = StaticRandom.Next(0, MaxX),
                                                                            Y = StaticRandom.Next(0, MaxY),
                                                                        };
                                                         Points.Add(newPoint);
                                                         RaisePropertyChanged(() => PointsCount);
                                                     }, () => true);

        public ICommand RemovePoint => new RelayCommand(() =>
                                                        {
                                                            Points.Remove(Points.First());
                                                            RaisePropertyChanged(() => PointsCount);
                                                        }, () => Points.Count > 0);

        public ICommand RaiseConnectionLimit => new RelayCommand(() => LimitConnections++, () => true);

        public ICommand LowerConnectionLimit => new RelayCommand(() =>
                                                                 {
                                                                     if (LimitConnections > 0)
                                                                     {
                                                                         LimitConnections--;
                                                                     }
                                                                 }, () => LimitConnections > 0);

        #region Constructors

        public MainViewModel()
        {
            Drag = 1;
            CollisionEnabled = true;
            ConnectionDistance = 10000;
            LimitConnections = 4;
            MaxX = 1200;
            MaxY = 600;
            _stopwatch = Stopwatch.StartNew();
            Points = new ObservableCollection<Node>();
            PointsCount = 90;
            
            //ComponentDispatcher.ThreadIdle += Animate;
            CompositionTarget.Rendering += Animate;

        }

        #endregion

        #region Properties

        public ObservableCollection<Node> Points { get; set; }

        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public double Drag { get; set; }

        public double LoopFrequency
        {
            get => _loopFrequency;
            set
            {
                _loopFrequency = value;
                RaisePropertyChanged();
            }
        }

        public int PointsCount
        {
            get => Points.Count;
            set
            {
                while (value > Points.Count)
                {
                    AddPoint.Execute(null);
                }

                while (value < Points.Count)
                {
                    RemovePoint.Execute(null);
                }

                RaisePropertyChanged();
            }
        }

        public int? LimitConnections
        {
            get => _limitConnections;
            set
            {
                _limitConnections = value;
                RaisePropertyChanged();
            }
        }

        public int ConnectionDistance { get; set; }

        public bool CollisionEnabled { get; set; }

        #endregion

        #region Methods

        private void MovePoints()
        {
            for (var i = 0; i < Points.Count; i++)
            {
                var node = Points[i];

                var connected = new List<Node>();
                for (var k = i + 1; k < Points.Count; k++)
                {
                    var otherNode = Points[k];
                    var squareDistance = otherNode.SquareDistanceTo(node);

                    if (LimitConnections == null
                        || (LimitConnections > connected.Count
                            && otherNode.X > node.X
                            && squareDistance < ConnectionDistance))
                    {
                        connected.Add(otherNode);
                    }

                    if (CollisionEnabled && squareDistance < Math.Pow(node.Radius + otherNode.Radius, 2))
                    {
                        ResolveCollision(otherNode, node);
                    }
                }

                node.ConnectedPoints = connected;
                MoveNode(node);
            }
        }

        private void MoveNode(Node node)
        {
            var maxX = MaxX - (SystemParameters.WindowNonClientFrameThickness.Left +
                               SystemParameters.WindowNonClientFrameThickness.Right +
                               SystemParameters.WindowResizeBorderThickness.Left +
                               SystemParameters.WindowResizeBorderThickness.Right);

            var maxY = MaxY - (SystemParameters.WindowNonClientFrameThickness.Top +
                               SystemParameters.WindowNonClientFrameThickness.Bottom +
                               SystemParameters.WindowResizeBorderThickness.Top +
                               SystemParameters.WindowResizeBorderThickness.Bottom);

            node.Vector *= Drag;
            // Avoid going out of screen even for very high speeds
            node.X = Math.Min(maxX, Math.Max(0, node.X + node.Vector.X));
            node.Y = Math.Min(maxY, Math.Max(0, node.Y + node.Vector.Y));

            if (node.X >= maxX)
            {
                node.Vector.X = -node.Vector.X; // Bounce from side of screen
            }
            else if (node.X + node.Vector.X <= 0)
            {
                node.Vector.X = -node.Vector.X;
            }

            if (node.Y >= maxY)
            {
                node.Vector.Y = -node.Vector.Y;
            }
            else if (node.Y + node.Vector.Y <= 0)
            {
                node.Vector.Y = -node.Vector.Y;
            }
        }

        public void ResolveCollision(Node n1, Node n2)
        {
            double collisionAngle = Math.Atan2((n2.Y - n1.Y), (n2.X - n1.X));

            double speed1 = n1.Vector.Length;
            double speed2 = n2.Vector.Length;

            double direction_1 = Math.Atan2(n1.Vector.Y, n1.Vector.X);
            double direction_2 = Math.Atan2(n2.Vector.Y, n2.Vector.X);
            double newXspeed1 = speed1 * Math.Cos(direction_1 - collisionAngle);
            double newYspeed1 = speed1 * Math.Sin(direction_1 - collisionAngle);
            double newXspeed2 = speed2 * Math.Cos(direction_2 - collisionAngle);
            double newYspeed2 = speed2 * Math.Sin(direction_2 - collisionAngle);


            double finalXspeed1 = ((n1.Mass - n2.Mass) * newXspeed1 + (n2.Mass + n2.Mass) * newXspeed2) / (n1.Mass + n2.Mass);
            double finalXspeed2 = ((n1.Mass + n1.Mass) * newXspeed1 + (n2.Mass - n1.Mass) * newXspeed2) / (n1.Mass + n2.Mass);
            double finalYspeed1 = newYspeed1;
            double finalYspeed2 = newYspeed2;

            double cosAngle = Math.Cos(collisionAngle);
            double sinAngle = Math.Sin(collisionAngle);
            n1.Vector.X = cosAngle * finalXspeed1 - sinAngle * finalYspeed1;
            n1.Vector.Y = sinAngle * finalXspeed1 + cosAngle * finalYspeed1;
            n2.Vector.X = cosAngle * finalXspeed2 - sinAngle * finalYspeed2;
            n2.Vector.Y = sinAngle * finalXspeed2 + cosAngle * finalYspeed2;

            Vector pos1 = new Vector(n1.X, n1.Y);
            Vector pos2 = new Vector(n2.X, n2.Y);

            // get the mtd
            Vector posDiff = pos1 - pos2;
            double d = posDiff.Length;

            // minimum translation distance to push balls apart after intersecting
            Vector mtd = posDiff * (((n1.Radius + n2.Radius) - d) / d);

            // resolve intersection --
            // computing inverse mass quantities
            double im1 = 1 / n1.Mass;
            double im2 = 1 / n2.Mass;

            // push-pull them apart based off their mass
            pos1 = pos1 + mtd * (im1 / (im1 + im2));
            pos2 = pos2 - mtd * (im2 / (im1 + im2));
            n1.X = pos1.X;
            n1.Y = pos1.Y;
            n2.X = pos2.X;
            n2.Y = pos2.Y;


            if (((n1.X + n1.Radius) >= MaxX) | ((n1.X - n1.Radius) <= 0))
                n1.Vector.X = -1 * n1.Vector.X;

            if (((n1.Y + n1.Radius) >= MaxY) | ((n1.Y - n1.Radius) <= 0))
                n1.Vector.Y = -1 * n1.Vector.Y;

            if (((n2.X + n2.Radius) >= MaxX) | ((n2.X - n2.Radius) <= 0))
                n2.Vector.X = -1 * n2.Vector.X;

            if (((n2.Y + n2.Radius) >= MaxY) | ((n2.Y - n2.Radius) <= 0))
                n2.Vector.Y = -1 * n2.Vector.Y;
        }

        #endregion

        private void Animate(object sender, EventArgs e)
        {
            /* equalize frames to ~30fps*/
            if (_stopwatch.Elapsed.TotalMilliseconds < 1000d / 31)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds((1000d / 30) - _stopwatch.Elapsed.TotalMilliseconds));
            }
            LoopFrequency = Math.Round(1000 / _stopwatch.Elapsed.TotalMilliseconds, 1);
            _stopwatch.Restart();
            MovePoints();

            RaisePropertyChanged(() => Points);
        }
    }
}