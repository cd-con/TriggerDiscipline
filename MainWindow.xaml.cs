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
        public static Point windowSize;

        public static Canvas canvasRef;
        public static Label accuracyRef;

        SoundManager manager = new();
        TriggerDotController dotCtrl = new();

        public long frameCounter = 0;
        public int score = 0;
        public double accuracy = 100;
        private int clickCount;
        public int lives = 1000;

        Line timerLine;
        Dialog? newGameOver;

        public MainWindow()
        {
            InitializeComponent();

            canvasRef = MainCanvas;
            accuracyRef = Accuracy;

            MainCanvas.Children.Add(debugEl);

            windowSize = new(Height, Width);
            SizeChanged += ((object sender, SizeChangedEventArgs e) =>
            {
                windowSize.X = e.NewSize.Height;
                windowSize.Y = e.NewSize.Width;
            });
            Task.Factory.StartNew(GameLoop);
        }

        public void GameLoop()
        {
            Stopwatch updateTimer = Stopwatch.StartNew();

            dotCtrl.clickSuccseedAction = () =>
            {
                lives += lives < 1000 ? 125 - (dotCtrl.difficulty * 5) : 0;
                manager.clickPlayer.Play();
                Score.Content = $"Score: {++score}";
            };

            Dispatcher.Invoke(() =>
            {
                /*gameOverBox = new(, new(200, 200));

                gameOverBox.state = IObject.ObjectState.OUTRO; // Сбрасываем состояние окна, чтобы изначально не отображалось
                gameOverBox.Update();
                MainCanvas.Children.Add(gameOverBox.boxRect);
                foreach (CanvasButton btn in gameOverBox.buttons)
                {
                    MainCanvas.Children.Add(btn.buttonRect);
                }*/

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
                try
                {
                    // Fixed update
                    if (updateTimer.ElapsedMilliseconds > 16)
                    {
                        if (state == GameState.GAME_LOOP)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                //accuracyRef.Content = $"Acc.:{(clickCount + 1) / (score + 1) * 100.0}%";
                                timerLine.X2 = (windowSize.Y / 1000) * lives;
                                dotCtrl.Update();
                                lives -= 2 * dotCtrl.difficulty;

                                if (lives <= 0)
                                {

                                    newGameOver = new Dialog(new(windowSize.Y / 2, windowSize.X / 2), 200, 200);
                                    newGameOver.ShowElement();
                                    state = GameState.GAME_END;
                                }
                            });
                        }

                        if (state == GameState.GAME_END)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                newGameOver?.Update();
                                //gameOverBox.center = new(windowSize.Y / 2, windowSize.X / 2);
                                //gameOverBox.Update();
                                /*foreach (CanvasButton btn in gameOverBox.buttons)
                                {
                                    btn.Update();
                                }*/
                            });
                        }

                        
                        updateTimer.Restart();
                    }
                }
                // Ловим отмену задачи, чтобы многократно не тыкать в отладчике
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(MainCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (state == GameState.GAME_LOOP)
                {
                    clickCount++;
                    dotCtrl.Click(p);
                }
                if (state == GameState.GAME_END)
                {
                    /*foreach (CanvasButton button in gameOverBox.buttons)
                    {
                        
                        if (button.Click(p))
                        {
                            gameOverBox.state = IObject.ObjectState.OUTRO;
                            gameOverBox.Update();
                            manager.clickPlayer.Play();
                            RestartGame();
                        }
                    }*/
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
                /*foreach (CanvasButton button in gameOverBox.buttons)
                {
                    button.hovered = button.Click(p);
                }*/
            }
        }

        private void RestartGame()
        {
            dotCtrl.Clear();
            dotCtrl.difficulty = 1;
            dotCtrl.frameCounter = 0;
            score = 0;
            lives = 1000;
            state = GameState.GAME_LOOP;
        }
    }
}
