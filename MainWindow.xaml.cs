using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TriggerDiscipline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public class TriggerDot
    {
        public Polygon poly;
        public Ellipse timeLeftCircle;
        public Point center;
        public int num = 0; // Хз зачем надо, мб потом пригодится
        public int diam = 32;
        public int timeLeft = 75;

        public TriggerDot(Canvas canvas, Point centerPoint)
        {
            poly = new Polygon() { Stroke = new SolidColorBrush() { Color = Colors.White } };
            timeLeftCircle = new() { Fill = new SolidColorBrush() { Color = Color.FromArgb(64, 255, 255, 255) } };
            center = centerPoint;
            Update(0);
        }

        public void Update(float angle)
        {
            double currDiam = (diam * 2) + diam * (timeLeft / 25d);
            timeLeftCircle.Height = currDiam;
            timeLeftCircle.Width = currDiam;
            Canvas.SetTop(timeLeftCircle, center.Y - (currDiam / 2));
            Canvas.SetLeft(timeLeftCircle, center.X - (currDiam / 2));

            poly.Points.Clear();
            for (int vertex = 0; vertex <= 6; vertex++)
            {
                poly.Points.Add(new(Math.Sin(vertex + (angle / 10)) * diam + center.X, Math.Cos(vertex + (angle / 10)) * diam + center.Y));
            }
            timeLeft--;
        }

        public bool isPointInRect(Point from) => from.DistanceTo(center) <= diam;
    }

    public static class Utility
    {
        public static double DistanceTo(this Point from, Point to) => Math.Sqrt(Math.Pow(from.X - to.X, 2) - Math.Pow(from.Y - to.Y, 2));

    }

    public partial class MainWindow : Window
    {
        public List<TriggerDot> dotsList = new();
        SoundPlayer clickPlayer = new(Application.GetResourceStream(new Uri("/resources/sounds/click.wav", UriKind.Relative)).Stream);
        SoundPlayer missPlayer = new(Application.GetResourceStream(new Uri("/resources/sounds/miss.wav", UriKind.Relative)).Stream);
        public long frameCounter = 0;
        public int score = 0;
        public double accuracy = 100;
        public int lives = 1000;
        Line timerLine;

        public MainWindow()
        {
            InitializeComponent();
            clickPlayer.Load();
            missPlayer.Load();
            Task.Factory.StartNew(GameLoop);
        }

        Random rnd = new Random();
        public void GameLoop()
        {
            Stopwatch updateTimer = Stopwatch.StartNew();

            Dispatcher.Invoke(() =>
            {
                timerLine = new Line{ Stroke = new SolidColorBrush() { Color = Colors.White }, StrokeThickness = 15 };
                timerLine.X1 = 0;
                timerLine.Y1 = 0;
                timerLine.Y2 = 0;
                timerLine.X2 = Width;
                MainCanvas.Children.Add(timerLine);
            });
           
            while (true)
            {
                if(updateTimer.ElapsedMilliseconds > 16)
                {

                    if (dotsList.Count == 0)
                    {
                        try
                        {
                            Dispatcher.Invoke(() =>
                        {
                            var dot = new TriggerDot(MainCanvas, new(rnd.Next(64, (int)Width - 64), rnd.Next(64, (int)Height - 64)));
                            MainCanvas.Children.Add(dot.poly);
                            MainCanvas.Children.Add(dot.timeLeftCircle);
                            dotsList.Add(dot);
                        });
                        }
                        catch
                        {
                            // LOLOL
                        }
                    }

                    try
                    {
                        foreach (TriggerDot dot in dotsList)
                        {
                            try
                            {
                                Dispatcher.Invoke(() => {
                                    dot.Update(frameCounter % 360); // , Convert.ToUInt32((360 - frameCounter % 360)) / 4
                                    timerLine.X2 = (Width / 1000) * lives; 
                                    
                                    if (dot.timeLeft <= 0)
                                    {
                                        missPlayer.Play();
                                        DestroyDot(dot);
                                    }
                                });
                                
                            }
                            catch
                            {
                                // LOLOL
                            }
                        }
                    }catch (Exception e)
                    {
                        //
                    }
                    frameCounter++;

                    lives -= 2;//* (int)(frameCounter / 500);
                    
                    if (lives <= 0)
                    {
                        MessageBox.Show($"Looser! Score: {score}");
                        break;
                    }

                    updateTimer.Restart();
                }
            }
        }

        private void DestroyDot(TriggerDot dot)
        {
            dotsList.Remove(dot);
            MainCanvas.Children.Remove(dot.poly);
            MainCanvas.Children.Remove(dot.timeLeftCircle);
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = Mouse.GetPosition(MainCanvas);
                foreach (TriggerDot dot in dotsList)
                {
                    if (dot.isPointInRect(p))
                    {
                        //SystemSounds.Beep.Play();
                        clickPlayer.Play();
                        lives += 125;
                        Score.Content = $"Score: {++score}";
                        //accuracy = (accuracy + p.DistanceTo(dot.center) - dot.diam) / score;
                        //Accuracy.Content = $"Acc.: {accuracy}";
                        DestroyDot(dot);
                        break;
                    }
                }
            }
        }
    }
}
