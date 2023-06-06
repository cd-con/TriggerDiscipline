using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace TriggerDiscipline
{
    public class TriggerDotController : IObject
    {
        public int frameCounter;
        public List<TriggerDot> dotStorage = new List<TriggerDot>();
        public List<TriggerDot> deathQueue = new List<TriggerDot>();
        public List<Point> clickQueue = new List<Point>();

        public Action clickSuccseedAction;

        public int difficulty = 1;

        public TriggerDotController()
        {
            Compute();
        }

        private PointCollection[] introAnimation = new PointCollection[32]; 
        private PointCollection[] loopAnimation = new PointCollection[360];
        // Реализовано через систему частиц
        // private PointCollection[] outroAnimation = new PointCollection[360];
        public void Compute()
        {
            // Запекаем анимациию появления
            for (int introFrame = 0; introFrame < introAnimation.Length; introFrame++)
            {
                introAnimation[introFrame] = BakeFrame(introFrame,  (32 / introAnimation.Length) * introFrame);
            }

            // Запекаем анимацию вращения
            for (int loopFrame = 0; loopFrame < loopAnimation.Length; loopFrame++)
            {
                loopAnimation[loopFrame] = BakeFrame(loopFrame, 32);
            }

            // Обнуляем глобальный таймер анимациий после генерации кадров
            frameCounter = 0;
        }

        public void Update()
        {
            // Нажимаем
            foreach (Point clickPoint in clickQueue)
            {
                foreach (TriggerDot dot in dotStorage)
                {
                    MainWindow.debuggerRef.Content = clickPoint.DistanceTo(dot.center);
                    if (clickPoint.isInCircle(dot.center, 900))
                    {
                        deathQueue.Add(dot);

                        if (clickSuccseedAction != null)
                            clickSuccseedAction.Invoke();
                    }
                }
            }
            clickQueue.Clear();

            // Удаляем
            if (deathQueue.Count > 0)
            {
                dotStorage = dotStorage.Except(deathQueue).ToList();

                foreach (TriggerDot dot in deathQueue)
                {
                    MainWindow.canvasRef.Children.Remove(dot.poly);
                    MainWindow.canvasRef.Children.Remove(dot.timeLeftCircle);
                }
                
                deathQueue.Clear();
            }

            // Добaвляем
            if (dotStorage.Count < difficulty)
            {
                TriggerDot newDot = new(this, new(App.globalRandom.Next(100, (int)MainWindow.windowSize.Y - 100), App.globalRandom.Next(100, (int)MainWindow.windowSize.X - 100)));
                MainWindow.canvasRef.Children.Add(newDot.poly);
                MainWindow.canvasRef.Children.Add(newDot.timeLeftCircle);
                dotStorage.Add(newDot);
            }

            foreach (TriggerDot dot in dotStorage)
            {
                dot.Update();
            }

            difficulty = (int)(frameCounter / 500f) + 1;

            frameCounter++;
        }

        public void Destroy(TriggerDot dot)
        {
            deathQueue.Add(dot);
        }

        public void Click(Point clickPoint)
        {
            clickQueue.Add(clickPoint);
        }

        public void Clear()
        {
            deathQueue = dotStorage;
        }

        private PointCollection BakeFrame(float angle, float diam)
        {
            PointCollection frame = new PointCollection();

            for (int vertex = 0; vertex <= 6; vertex++)
            {
                frame.Add(new(Math.Sin(vertex + (angle / 10)) * diam, Math.Cos(vertex + (angle / 10)) * diam));
            }

            return frame;
        }

        public PointCollection GetFrame(int frame)
        {
            return frame < 32 ? introAnimation[frame] : loopAnimation[frame % loopAnimation.Length];
        }
    }

    public class TriggerDot : IObject
    {
        private TriggerDotController parent;
        public Polygon poly;
        public Ellipse timeLeftCircle;
        public Point center;

        private int timeLeftInit = 107;
        public int timeLeft;
        
        private SolidColorBrush polyBrush = new() { Color = Colors.White };
        private SolidColorBrush circleBrush = new() { Color = Color.FromArgb(64, 255, 255, 255) };

        public TriggerDot(TriggerDotController storageReference, Point position) {
            parent = storageReference;
            timeLeft = timeLeftInit;
            center = position;
            poly = new() { Stroke = polyBrush };
            timeLeftCircle = new() { Fill = circleBrush };
            

        }

        public void Compute()
        {
            // No computing today
        }

        public void Update()
        {
            poly.Points = parent.GetFrame(timeLeftInit - timeLeft).ShiftSum(center);

            double currDiam = (32 * 2) + 32 * (timeLeft / 32d);
            timeLeftCircle.Height = currDiam;
            timeLeftCircle.Width = currDiam;
            Canvas.SetTop(timeLeftCircle, center.Y - (currDiam / 2));
            Canvas.SetLeft(timeLeftCircle, center.X - (currDiam / 2));

            timeLeft--;

            if (timeLeft == 0)
            {
                SoundManager.instance.missPlayer.Play();
                parent.Destroy(this);
            }
        }
    }
}
