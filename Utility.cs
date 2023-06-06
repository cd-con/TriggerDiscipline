using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TriggerDiscipline
{
    public static class Utility
    {
        public static double DistanceTo(this Point from, Point to) => (to - from).LengthSquared;

        public static bool isInCircle(this Point from, Point source, float diam) => from.DistanceTo(source) <= diam;

        public static bool isInSquare(this Point point, Point origin, int h, int w)
        {
            Point topLeft = new Point(origin.X, origin.Y);
            Point bottomRight = new Point(origin.X + h, origin.Y + + w);

            // Debug
            MainWindow.accuracyRef.Content = point.X > topLeft.X && point.X < bottomRight.Y;

            return (point.X > topLeft.X && point.X < bottomRight.X) && (point.Y > topLeft.Y && point.Y < bottomRight.Y);
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
        public static Point ScaleFun(Point A, double factor) => new(A.X * factor, A.Y * factor);
    }
}
