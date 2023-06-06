using System.Windows;
using System.Windows.Media;

namespace TriggerDiscipline
{
    public static class Utility
    {
        //public static double DistanceTo(this Point from, Point to) =>  Math.Sqrt(Math.Pow(from.X - to.X, 2) - Math.Pow(from.Y - to.Y, 2) + 0.05);
        public static double DistanceTo(this Point from, Point to) => (to - from).LengthSquared;

        public static bool isInCircle(this Point from, Point source, float diam) => from.DistanceTo(source) <= diam;

        public static bool isInSquare(this Point from, Point origin, int h, int w)
        {
            Point topLeft = new Point(origin.X - (h / 2), origin.Y - (w / 2));
            Point bottomRight = new Point(origin.X + (h / 2), origin.Y + (w / 2));
            return from.Y > topLeft.Y && from.Y < bottomRight.Y && from.X > topLeft.X && from.X < bottomRight.X;
        }
        public static PointCollection ShiftSum(this PointCollection source, Point shift)
        {
            PointCollection sauce = new();
            for (int i = 0; i < source.Count; i++)
            {
                sauce.Add(source[i].Sum(shift));
            }
            return sauce;
        }

        public static Point Sum(this Point A, Point B) => new(A.X + B.X, A.Y + B.Y);

        public static Point Scale(this Point A, double factor) => new(A.X * factor, A.Y * factor);
    }
}
