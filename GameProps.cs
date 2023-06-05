using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TriggerDiscipline
{
    public class TriggerDot : IObject
    {
        public Polygon poly;
        public Ellipse timeLeftCircle;
        public Point center;

        public int diam = 32;
        public int timeLeft = 107;

        private SolidColorBrush polyBrush = new() { Color = Colors.White };
        private SolidColorBrush circleBrush = new() { Color = Color.FromArgb(64, 255, 255, 255) };

        public IObject.ObjectState state = IObject.ObjectState.INTRO;
        private int animCounter = 0;

        public TriggerDot(Point centerPoint)
        {
            poly = new Polygon() { Stroke = polyBrush };
            timeLeftCircle = new() { Fill = circleBrush };
            center = centerPoint;
            UpdateAnimation(0);
        }


        public void Intro()
        {
            if (animCounter <= 32)
            {
                //polyBrush.Color = Color.FromArgb(Convert.ToByte((frame - intro_animation_end) * -25.5), 255, 255, 255);
                diam = (animCounter);
                UpdateAnimation(animCounter % 360);
            }
            else
            {
                state = IObject.ObjectState.LOOP;
            }
        }

        public void Update()
        {
            switch (state)
            {
                case IObject.ObjectState.INTRO:
                    Intro();
                    break;
                case IObject.ObjectState.LOOP:
                    UpdateAnimation(animCounter % 360);
                    break;
                case IObject.ObjectState.OUTRO:
                    Outro();
                    break;
            }

            animCounter++;
        }
        public void Outro()
        {
            // Suicide
            MainWindow.DestroyDot(this);
        }

        private void UpdateAnimation(float angle)
        {
            double currDiam = (diam * 2) + diam * (timeLeft / 32d);
            timeLeftCircle.Height = currDiam;
            timeLeftCircle.Width = currDiam;
            Canvas.SetTop(timeLeftCircle, center.Y - (currDiam / 2));
            Canvas.SetLeft(timeLeftCircle, center.X - (currDiam / 2));

            poly.Points.Clear();
            for (int vertex = 0; vertex <= 6; vertex++)
            {
                poly.Points.Add(new(Math.Sin(vertex + (angle / 10)) * diam + center.X, Math.Cos(vertex + (angle / 10)) * diam + center.Y));
            }

            if (timeLeft == 0)
            {
                SoundManager.instance.missPlayer.Play();
                state = IObject.ObjectState.OUTRO;
            }

            timeLeft--;
        }

        public bool isPointInRect(Point from) => from.DistanceTo(center) <= diam;
    }
}
