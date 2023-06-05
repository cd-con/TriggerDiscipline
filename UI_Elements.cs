using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TriggerDiscipline
{
    public class CanvasButton : IObject
    {
        public bool hovered = false;

        SolidColorBrush fillBrush = new() { Color = Colors.Black };

        public Rectangle buttonRect;
        public Point center;
        public Point targetSize;
        public double proportionFac;

        public IObject.ObjectState state = IObject.ObjectState.INTRO;
        private int animCounter = 0;

        public CanvasButton(Point centerPoint, Point buttonScale)
        {
            buttonRect = new Rectangle() { Fill = fillBrush, Stroke = new SolidColorBrush() { Color = Colors.White } };
            center = centerPoint;
            Canvas.SetTop(buttonRect, centerPoint.Y - buttonScale.Y / 2);
            Canvas.SetLeft(buttonRect, centerPoint.X - buttonScale.X / 2);
            targetSize = buttonScale;
            //proportionFac = dialogScale.X / dialogScale.Y;
        }

        public void Intro()
        {
            buttonRect.Width = targetSize.X;
            buttonRect.Height = targetSize.Y;
            state = IObject.ObjectState.LOOP;
        }

        public void Outro()
        {
            buttonRect.Width = 0;
            buttonRect.Height = 0;
        }

        public void Update()
        {
            switch (state)
            {
                case IObject.ObjectState.INTRO:
                    Intro();
                    break;
                case IObject.ObjectState.LOOP:

                    if (hovered)
                    {
                        animCounter += animCounter < 250 ? 25 : 0;
                    }
                    else
                    {
                        animCounter -= animCounter > 0 ? 25 : 0;
                    }
                    fillBrush.Color = Color.FromArgb(Convert.ToByte(animCounter), 255, 255, 255);
                    break;
                case IObject.ObjectState.OUTRO:
                    Outro();
                    break;
            }
        }

        /// <summary>
        /// Нажатие на кнопку.
        /// </summary>
        /// <param name="clickPoint"></param>
        public bool Click(Point clickPoint)
        {
            Point topLeft = new Point(center.X - (targetSize.X / 2), center.Y - (targetSize.Y / 2));
            Point bottomRight = new Point(center.X + (targetSize.X / 2), center.Y + (targetSize.Y / 2));
            // Pi*dec
            return (clickPoint.Y > topLeft.Y && clickPoint.Y < bottomRight.Y) && (clickPoint.X > topLeft.X && clickPoint.X < bottomRight.X);
        }
    }

    public class CanvasMessageBox : IObject
    {
        public Rectangle boxRect;
        public Point center;
        public Point targetSize;
        public double proportionFac;

        public List<CanvasButton> buttons = new List<CanvasButton>();

        public IObject.ObjectState state = IObject.ObjectState.INTRO;
        private int animCounter = 0;

        public CanvasMessageBox(Point centerPoint, Point dialogScale)
        {
            boxRect = new Rectangle() { Stroke = new SolidColorBrush { Color = Colors.White }, Fill = new SolidColorBrush { Color = Colors.DarkSlateGray } };
            Canvas.SetTop(boxRect, centerPoint.Y - dialogScale.Y / 2);
            Canvas.SetLeft(boxRect, centerPoint.X - dialogScale.X / 2);
            targetSize = dialogScale;

            // Test button
            buttons.Add(new CanvasButton(centerPoint, new(50, 25)));

            //proportionFac = dialogScale.X / dialogScale.Y;
        }



        public void Intro()
        {
            //if (animCounter <= 64)
            //{
            //polyBrush.Color = Color.FromArgb(Convert.ToByte((frame - intro_animation_end) * -25.5), 255, 255, 255);

            //UpdateAnimation(animCounter % 360);
            //}
            //else
            //{
            boxRect.Width = targetSize.Y;
            boxRect.Height = targetSize.X;
            state = IObject.ObjectState.LOOP;
            foreach (CanvasButton btn in buttons)
            {
                btn.state = IObject.ObjectState.INTRO;
                btn.Update();
            }
            //}
        }

        public void Outro()
        {
            boxRect.Width = 0;
            boxRect.Height = 0;

            foreach (CanvasButton btn in buttons)
            {
                btn.state = IObject.ObjectState.OUTRO;
                btn.Update();
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
                    //UpdateAnimation(animCounter % 360);
                    break;
                case IObject.ObjectState.OUTRO:
                    Outro();
                    break;
            }

            animCounter++;
        }
    }

}
