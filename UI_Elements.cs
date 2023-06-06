using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TriggerDiscipline
{
    public abstract class GenericInterfaceElement : IObject
    {
        public float[] flyoutFrames = new float[16];
        public SolidColorBrush fillBrush = new() { Color = Colors.Black };
        public readonly SolidColorBrush strokeBrush = new() { Color = Colors.White };

        public List<GenericInterfaceElement> children = new();

        public Action? onClick;
        public enum ElementState
        {
            INIT,
            NORMAL,
            HOVERED,
            PRESSED,
            RELEASED
        }
        public ElementState currentState = ElementState.RELEASED;

        public Point elementCenter;
        public int height;
        public int width;

        public int animationCounter = 0;

        public GenericInterfaceElement(Point center, int w, int h)
        {
            elementCenter = center;
            height = h;
            width = w;

            Compute();
        }

        public void Compute()
        {
            for (int animFrame = 0; animFrame < flyoutFrames.Length; animFrame++)
            {
                flyoutFrames[animFrame] = (1f / flyoutFrames.Length) * animFrame;
            }
        }

        public void InteractionHandler(Point point, bool clicked = false)
        {
            if (point.isInSquare(elementCenter, height, width))
            {
                currentState = ElementState.HOVERED;
                if (clicked)
                {
                    currentState = ElementState.PRESSED;
                    onClick?.Invoke();
                }
            }
            else
            {
                currentState = ElementState.NORMAL;
            }
        }

        public void AnimationUpdate()
        {
            switch (currentState)
            {
                case ElementState.NORMAL:
                    animationCounter -= animationCounter > flyoutFrames.Length - 1 ? 1 : 0;
                    break;
                case ElementState.HOVERED:
                    animationCounter += animationCounter < 255 ? 1 : 0;
                    break;
                case ElementState.PRESSED:
                    animationCounter = 15;
                    currentState = ElementState.RELEASED;
                    break;
                case ElementState.RELEASED:
                    animationCounter -= animationCounter > 0 ? 1 : 0;
                    break;
                case ElementState.INIT:
                    if (animationCounter < flyoutFrames.Length - 1)
                    {
                        animationCounter++;
                    }
                    else
                    {
                        currentState = ElementState.NORMAL;
                    }
                    break;
            }
        }

        public abstract void FixedUpdate();

        public abstract void Update();

        public abstract void Destroy();

        public void HideElement()
        {
            foreach (GenericInterfaceElement child in children)
            {
                child.HideElement();
            }
            currentState = ElementState.PRESSED;
        }

        public void ShowElement()
        {
            animationCounter = 0;
            foreach (GenericInterfaceElement child in children)
            {
                child.ShowElement();
            }
            currentState = ElementState.INIT;
        }
    }

    public class Dialog : GenericInterfaceElement
    {
        public Rectangle rect = new() { Stroke = new SolidColorBrush() { Color = Colors.White }, Fill = new SolidColorBrush() { Color = Colors.Black } };

        public Dialog(Point center, int w, int h) : base(center, w, h)
        {
            rect.Height = h;
            rect.Width = w;
            MainWindow.canvasRef.Children.Add(rect);
        }

        public override void Destroy()
        {
            foreach (GenericInterfaceElement child in children)
            {
                if (child.GetType() == typeof(Button))
                {
                    child.Destroy();
                }
            }
            MainWindow.canvasRef.Children.Remove(rect);
        }

        public override void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            AnimationUpdate();
            foreach (GenericInterfaceElement child in children)
            {
                child.Update();
            }

            if (currentState == ElementState.INIT || currentState == ElementState.RELEASED)
            {
                Point scaledCenter = (Point)(elementCenter - elementCenter.Scale(flyoutFrames[animationCounter]));
                Canvas.SetTop(rect, scaledCenter.X);
                Canvas.SetLeft(rect, scaledCenter.Y);
                rect.Width = flyoutFrames[animationCounter] * width;
                rect.Height = flyoutFrames[animationCounter] * height;
            }
            else
            {
                rect.Width = width;
                rect.Height = height;
            }
        }
    }

    /// <summary>
    /// Кнопка
    /// </summary>
    public class Button : GenericInterfaceElement
    {
        public Rectangle rect = new() { Stroke = new SolidColorBrush() { Color = Colors.White }, Fill = new SolidColorBrush() { Color = Colors.Black } };
        public List<GenericInterfaceElement> elements = new();
        public Label caption;
        public Point parentPoint;
        public Button(Point center, int w, int h, Point parent = new()) : base(center, w, h)
        {
            parentPoint = parent;
            rect.Height = h;
            rect.Width = w;
            MainWindow.canvasRef.Children.Add(rect);
            caption = new Label();
            caption.Content = "Restart?";
            caption.Height = h;
            caption.Width = w;
            MainWindow.canvasRef.Children.Add(caption);

            
        }

        public override void FixedUpdate()
        {

        }

        public override void Destroy()
        {
            foreach (GenericInterfaceElement child in children)
            {
                if (child.GetType() == typeof(Button))
                {
                    child.Destroy();
                }
            }
            MainWindow.canvasRef.Children.Remove(rect);
        }


        public override void Update()
        {
            AnimationUpdate();
            foreach (GenericInterfaceElement element in elements)
            {
                element.Update();
            }
            if (currentState == ElementState.INIT || currentState == ElementState.RELEASED)
            {
                float targetW = flyoutFrames[animationCounter] * width;
                float targetH = flyoutFrames[animationCounter] * height; 
                Point scaledCenter = (Point)(elementCenter - new Point(targetH / 2, targetW / 2));
                scaledCenter = scaledCenter.Sum(parentPoint);
                Canvas.SetTop(rect, scaledCenter.X);
                Canvas.SetLeft(rect, scaledCenter.Y);
                Canvas.SetTop(caption, scaledCenter.X);
                Canvas.SetLeft(caption, scaledCenter.Y);
                rect.Width = targetW;
                rect.Height = targetH;
            }
            if (currentState == ElementState.HOVERED || currentState == ElementState.NORMAL)
            {
                fillBrush.Color = Color.FromArgb(Convert.ToByte(animationCounter), 255, 255, 255);
            }
        }
    }
}
