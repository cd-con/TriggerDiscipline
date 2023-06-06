using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TriggerDiscipline
{
    public abstract class GenericInterfaceElement : IObject
    {
        public float[] flyoutFrames = new float[16];
        public SolidColorBrush fillBrush = new() { Color = Colors.Black };
        public readonly SolidColorBrush strokeBrush = new() { Color = Colors.White };

        public Action? onClick;
        public enum ElementState
        {
            INIT,
            NORMAL,
            HOVERED,
            PRESSED,
            RELEASED
        }
        public ElementState currentState = ElementState.INIT;

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

        public abstract void ShowElement();

        public abstract void HideElement();
    }

    public class Dialog : GenericInterfaceElement
    {
        public Rectangle rect = new();
        public Dialog(Point center, int w, int h) : base(center, w, h)
        {
            rect.Height = h;
            rect.Width = w;
            MainWindow.canvasRef.Children.Add(rect);
            Canvas.SetTop(rect, center.X);
            Canvas.SetLeft(rect, center.Y);
        }

        public override void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public override void HideElement()
        {
            currentState = ElementState.PRESSED;
            //rect.Visibility = Visibility.Hidden;
        }

        public override void ShowElement()
        {
            animationCounter = 0;
            currentState = ElementState.INIT;
            //rect.Visibility = Visibility.Visible;
        }

        public override void Update()
        {
            AnimationUpdate();
            if (currentState == ElementState.INIT || currentState == ElementState.RELEASED)
                rect.Width = flyoutFrames[animationCounter] * width;
                rect.Height = flyoutFrames[animationCounter] * height;
            //fillBrush.Color = Color.FromArgb(Convert.ToByte(animationCounter), 255, 255, 255);
        }
    }

    public class Button : GenericInterfaceElement
    {
        public Rectangle rect = new();
        public Button(Point center, int w, int h) : base(center, w, h)
        {
            MainWindow.canvasRef.Children.Add(rect);
            Canvas.SetTop(rect, center.X);
            Canvas.SetLeft(rect, center.Y);
        }

        public override void FixedUpdate()
        {
            throw new NotImplementedException();
        }

        public override void HideElement()
        {
            currentState = ElementState.PRESSED;
            //rect.Visibility = Visibility.Hidden;
        }

        public override void ShowElement()
        {
            animationCounter = 0;
            currentState = ElementState.INIT;
            
            //rect.Visibility = Visibility.Visible;
        }

        public override void Update()
        {
            AnimationUpdate();
            if (currentState == ElementState.INIT || currentState == ElementState.RELEASED)
                rect.Width = flyoutFrames[animationCounter];
            //fillBrush.Color = Color.FromArgb(Convert.ToByte(animationCounter), 255, 255, 255);
        }
    }

    /*

public class Gene : GenericInterfaceElement
{
    public bool hovered = false;



    public Rectangle buttonRect;

    private int animCounter = 0;

    public CanvasButton(Point center, int w, int h) : base(center, w, h)
    {
        buttonRect = new Rectangle() { Fill = fillBrush, Stroke = new SolidColorBrush() { Color = Colors.White } };
        Canvas.SetTop(buttonRect, elementCenter.Y - width / 2);
        Canvas.SetLeft(buttonRect, elementCenter.X - height / 2);
    }

    public override void Compute()
    {
        if (hovered)
        {
            animCounter += animCounter < 250 ? 25 : 0;
        }
        else
        {
            animCounter -= animCounter > 0 ? 25 : 0;
        }
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


                fillBrush.Color = Color.FromArgb(Convert.ToByte(animCounter), 255, 255, 255);
                break;
            case IObject.ObjectState.OUTRO:
                Outro();
                break;
        }
    }

    public void Compute()
    {
        throw new NotImplementedException();
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
        targetSize = dialogScale;
        center = centerPoint;

        // Test button
        buttons.Add(new CanvasButton(centerPoint, new(50, 25)));

        //proportionFac = dialogScale.X / dialogScale.Y;
    }

    public void Compute()
    {
        throw new NotImplementedException();
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
                Canvas.SetTop(boxRect, center.Y - targetSize.Y / 2);
                Canvas.SetLeft(boxRect, center.X - targetSize.X / 2);
                break;
            case IObject.ObjectState.OUTRO:
                Outro();
                break;
        }

        animCounter++;
    }
}*/

}
