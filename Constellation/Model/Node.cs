using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfConstellation;

namespace Constellation.Model
{
    public class Node : ObservableObject
    {
        public Vector Vector;
        public double Mass;
        private double _x;
        private double _y;
        private double _diameter;

        public double Diameter
        {
            get => _diameter;
            set { Set("Diameter", ref _diameter, value); }
        }

        public double Radius => Diameter / 2;

        public double X
        {
            get => _x;
            set => Set("X", ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => Set("Y", ref _y, value);
        }

        public double OriginCorrection => -(Diameter / 2);

        public Node()
        {
            Diameter = StaticRandom.Next(4, 12);
            Mass = Diameter/2;
            var randX = (double)StaticRandom.Next(-300, 300) / 100;
            var randY = (double)StaticRandom.Next(-300, 300) / 100;
            Vector = new Vector(randX, randY);
        }

        
        public static Node operator+ (Node point, Vector vector)
        {
            point.X += vector.X;
            point.Y += vector.Y;
            return point;
        }

        public IEnumerable<Node> ConnectedPoints { get; set; }

        public double SquareDistanceTo(Node otherNode)
        {
            var x1 = X;
            var x2 = otherNode.X;
            var y1 = Y;
            var y2 = otherNode.Y;
            var distance = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            return distance;
        }

        
    }
}