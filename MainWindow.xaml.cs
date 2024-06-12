using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;

namespace Had
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Direction { Up, Down, Left, Right };
        private Direction currentDirection = Direction.Right;
        private DispatcherTimer gameTimer;
        private List<Rectangle> snakeParts;
        private Rectangle food;
        private Random rand;
        private bool isPaused = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            snakeParts = new List<Rectangle>();
            rand = new Random();

            StartNewGame();
        }

        private void StartNewGame()
        {
            snakeParts.Clear();
            GameCanvas.Children.Clear();
            currentDirection = Direction.Right;

            Rectangle head = new Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green
            };
            snakeParts.Add(head);
            GameCanvas.Children.Add(head);
            Canvas.SetLeft(head, 100);
            Canvas.SetTop(head, 100);

            DrawFood();
            gameTimer.Start();
        }

        private void DrawFood()
        {
            food = new Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red
            };
            GameCanvas.Children.Add(food);
            PositionFood();
        }

        private void PositionFood()
        {
            int maxX = (int)(GameCanvas.ActualWidth / 20);
            int maxY = (int)(GameCanvas.ActualHeight / 20);
            Canvas.SetLeft(food, rand.Next(0, maxX) * 20);
            Canvas.SetTop(food, rand.Next(0, maxY) * 20);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused) return;

            for (int i = snakeParts.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(snakeParts[i], Canvas.GetLeft(snakeParts[i - 1]));
                Canvas.SetTop(snakeParts[i], Canvas.GetTop(snakeParts[i - 1]));
            }

            double left = Canvas.GetLeft(snakeParts[0]);
            double top = Canvas.GetTop(snakeParts[0]);

            switch (currentDirection)
            {
                case Direction.Up:
                    Canvas.SetTop(snakeParts[0], top - 20);
                    break;
                case Direction.Down:
                    Canvas.SetTop(snakeParts[0], top + 20);
                    break;
                case Direction.Left:
                    Canvas.SetLeft(snakeParts[0], left - 20);
                    break;
                case Direction.Right:
                    Canvas.SetLeft(snakeParts[0], left + 20);
                    break;
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {
            double headLeft = Canvas.GetLeft(snakeParts[0]);
            double headTop = Canvas.GetTop(snakeParts[0]);

            if (headLeft < 0 || headTop < 0 || headLeft >= GameCanvas.ActualWidth || headTop >= GameCanvas.ActualHeight)
            {
                GameOver();
            }

            for (int i = 1; i < snakeParts.Count; i++)
            {
                if (headLeft == Canvas.GetLeft(snakeParts[i]) && headTop == Canvas.GetTop(snakeParts[i]))
                {
                    GameOver();
                }
            }

            if (headLeft == Canvas.GetLeft(food) && headTop == Canvas.GetTop(food))
            {
                GrowSnake();
                PositionFood();
            }
        }

        private void GrowSnake()
        {
            Rectangle newPart = new Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green
            };
            snakeParts.Add(newPart);
            GameCanvas.Children.Add(newPart);
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! Press OK to start a new game.");
            StartNewGame();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                isPaused = !isPaused;
                return;
            }

            if (isPaused) return;

            switch (e.Key)
            {
                case Key.Up:
                    if (currentDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (currentDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;
            }
        }
    }
}
