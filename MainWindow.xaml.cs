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
        private enum FoodType { Red, Yellow, Purple };

        private Direction currentDirection = Direction.Right;
        private DispatcherTimer gameTimer;
        private List<Rectangle> snakeParts;
        private Rectangle food;
        private Random rand;
        private bool isPaused = false;
        private bool isInverted = false;
        private int speedMultiplier = 1;

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
            if (food != null)
            {
                GameCanvas.Children.Remove(food);
            }

            FoodType foodType = GetRandomFoodType();
            food = new Rectangle
            {
                Width = 20,
                Height = 20,
                Fill = GetFoodColor(foodType),
                Tag = foodType
            };
            GameCanvas.Children.Add(food);
            PositionFood();
        }

        private FoodType GetRandomFoodType()
        {
            int value = rand.Next(0, 100);
            if (value < 60)
            {
                return FoodType.Red;
            }
            else if (value < 80)
            {
                return FoodType.Yellow;
            }
            else
            {
                return FoodType.Purple;
            }
        }

        private Brush GetFoodColor(FoodType foodType)
        {
            switch (foodType)
            {
                case FoodType.Red:
                    return Brushes.Red;
                case FoodType.Yellow:
                    return Brushes.Yellow;
                case FoodType.Purple:
                    return Brushes.Purple;
                default:
                    return Brushes.Red;
            }
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
                    Canvas.SetTop(snakeParts[0], top - 20 * speedMultiplier);
                    break;
                case Direction.Down:
                    Canvas.SetTop(snakeParts[0], top + 20 * speedMultiplier);
                    break;
                case Direction.Left:
                    Canvas.SetLeft(snakeParts[0], left - 20 * speedMultiplier);
                    break;
                case Direction.Right:
                    Canvas.SetLeft(snakeParts[0], left + 20 * speedMultiplier);
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
                FoodType foodType = (FoodType)food.Tag;
                switch (foodType)
                {
                    case FoodType.Red:
                        GrowSnake();
                        break;
                    case FoodType.Yellow:
                        SpeedBoost();
                        break;
                    case FoodType.Purple:
                        InvertControls();
                        break;
                }
                DrawFood();
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

        private void SpeedBoost()
        {
            speedMultiplier = 2;
            DispatcherTimer resetSpeedTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            resetSpeedTimer.Tick += (s, e) =>
            {
                speedMultiplier = 1;
                resetSpeedTimer.Stop();
            };
            resetSpeedTimer.Start();
        }

        private void InvertControls()
        {
            isInverted = true;
            DispatcherTimer resetInvertTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            resetInvertTimer.Tick += (s, e) =>
            {
                isInverted = false;
                resetInvertTimer.Stop();
            };
            resetInvertTimer.Start();
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
                        currentDirection = isInverted ? Direction.Down : Direction.Up;
                    break;
                case Key.Down:
                    if (currentDirection != Direction.Up)
                        currentDirection = isInverted ? Direction.Up : Direction.Down;
                    break;
                case Key.Left:
                    if (currentDirection != Direction.Right)
                        currentDirection = isInverted ? Direction.Right : Direction.Left;
                    break;
                case Key.Right:
                    if (currentDirection != Direction.Left)
                        currentDirection = isInverted ? Direction.Left : Direction.Right;
                    break;
            }
        }
    }
}