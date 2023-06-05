using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

/// ДАМЫ И ГОСПОДА, TODO
/// 1. Собственное хранилище для TriggerDot
/// 2. Сделать универсальный класс для элементов интерфейса
/// 3. Переписать алгоритм отрисовки шестиугольников
/// 4. Частицы при разрушении
/// 5. Не сойти с ума

namespace TriggerDiscipline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    
    public static class Utility
    {
        public static double DistanceTo(this Point from, Point to)
        {
            double result = Math.Sqrt(Math.Pow(from.X - to.X, 2) - Math.Pow(from.Y - to.Y, 2) + 0.05);

            // Костыль
            if (result == double.NaN)
            {
                return 0;
            }
            return result;
        }

    }

    public partial class MainWindow : Window
    {
        public enum GameState
        {
            MENU,
            GAME_LOOP,
            GAME_END
        }

        public GameState state = GameState.GAME_LOOP;

        //public EventManager fixedUpdate = new();

        public static List<TriggerDot> dotsList = new();
        private static Canvas canvasRef;

        SoundManager manager = new();
        public long frameCounter = 0;
        public int score = 0;
        public double accuracy = 100;
        public int lives = 1000;

        Line timerLine;
        CanvasMessageBox gameOverBox;

        public MainWindow()
        {
            InitializeComponent();
            canvasRef = MainCanvas;
            MainCanvas.Children.Add(debugEl);
            Task.Factory.StartNew(GameLoop);
        }

        private void CreateDot()
        {
            if (dotsList.Count == 0)
            {
                var dot = new TriggerDot(new(rnd.Next(64, (int)Width - 64), rnd.Next(64, (int)Height - 64)));
                MainCanvas.Children.Add(dot.poly);
                MainCanvas.Children.Add(dot.timeLeftCircle);
                dotsList.Add(dot);
            }
        }

        Random rnd = new Random();
        public void GameLoop()
        {
            Stopwatch updateTimer = Stopwatch.StartNew();
            
            Dispatcher.Invoke(() =>
            {
                gameOverBox = new(new(Width / 2, Height/2), new(200, 200));

                
                gameOverBox.state = IObject.ObjectState.OUTRO; // Сбрасываем состояние окна, чтобы изначально не отображалось
                gameOverBox.Update();
                MainCanvas.Children.Add(gameOverBox.boxRect);
                foreach (CanvasButton btn in gameOverBox.buttons)
                {
                    MainCanvas.Children.Add(btn.buttonRect);
                }

                timerLine = new Line { Stroke = new SolidColorBrush() { Color = Colors.White }, StrokeThickness = 15 };
                timerLine.X1 = 0;
                timerLine.Y1 = 0;
                timerLine.Y2 = 0;
                timerLine.X2 = Width;
                MainCanvas.Children.Add(timerLine);
            });

            // Main game loop
            while (true)
            {
                // Fixed update
                if (updateTimer.ElapsedMilliseconds > 16)
                {
                    if (state == GameState.GAME_LOOP) {
                        try
                        {
                            Dispatcher.Invoke(CreateDot);
                        }
                        catch
                        {
                            // LOLOL
                        }

                        try
                        {
                            foreach (TriggerDot dot in dotsList)
                            {
                                try
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        timerLine.X2 = (Width / 1000) * lives;
                                        dot.Update();
                                    });

                                }
                                catch
                                {
                                    // LOLOL
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //
                        }
                        frameCounter++;

                        lives -= 2;

                        if (lives <= 0)
                        {
                            state = GameState.GAME_END;
                            gameOverBox.state = IObject.ObjectState.INTRO;
                        }
                    }

                    if (state == GameState.GAME_END)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            gameOverBox.Update();
                            foreach (CanvasButton btn in gameOverBox.buttons)
                            {
                                btn.Update();
                            }
                        });
                    }

                    updateTimer.Restart();
                }
            }
        }

        public static void DestroyDot(TriggerDot dot)
        {
            canvasRef.Children.Remove(dot.poly);
            canvasRef.Children.Remove(dot.timeLeftCircle);
            dotsList.Remove(dot);
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(MainCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                if (state == GameState.GAME_LOOP)
                {
                    foreach (TriggerDot dot in dotsList)
                    {
                        if (dot.isPointInRect(p))
                        {
                            //SystemSounds.Beep.Play();
                            manager.clickPlayer.Play();
                            lives += 125;
                            Score.Content = $"Score: {++score}";
                            //accuracy = (accuracy + p.DistanceTo(dot.center) - dot.diam) / score;
                            //Accuracy.Content = $"Acc.: {accuracy}";
                            dot.state = IObject.ObjectState.OUTRO;
                            break;
                        }
                    }
                }
                if (state == GameState.GAME_END)
                {
                    foreach (CanvasButton button in gameOverBox.buttons)
                    {
                        if (button.Click(p))
                        {
                            gameOverBox.state = IObject.ObjectState.OUTRO;
                            gameOverBox.Update();
                            manager.clickPlayer.Play();
                            RestartGame();
                        }
                    }
                }
            }
        }


        // DEBUG
        Ellipse debugEl = new() { Fill = new SolidColorBrush() { Color = Colors.Red }, Width = 5, Height = 5 };

        private void MainCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            
            Point p = Mouse.GetPosition(MainCanvas);
            Canvas.SetTop(debugEl, p.Y - 2.5);
            Canvas.SetLeft(debugEl, p.X - 2.5);
            Canvas.SetZIndex(debugEl, 100);
            if (state == GameState.GAME_END)
            {
                foreach (CanvasButton button in gameOverBox.buttons)
                {
                    button.hovered = button.Click(p);
                }
            }
        }

        private void RestartGame()
        {
            foreach (TriggerDot dot in dotsList)
            {
                MainCanvas.Children.Remove(dot.poly);
                MainCanvas.Children.Remove(dot.timeLeftCircle);
            }
            dotsList.Clear();
            score = 0;
            lives = 1000;
            state = GameState.GAME_LOOP;
        }
    }
}
