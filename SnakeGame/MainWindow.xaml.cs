﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool gameRunning = false;
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new Dictionary<GridValue, ImageSource>()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food }
        };
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        public MainWindow()
        {
            gameState = new GameState(rows,cols);
            InitializeComponent();
            gridImages = SetUpGrid();

        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            ShowGameOver();
            gameState = new GameState(rows,cols);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                default:
                    break;
            }
        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        public async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(120);
                gameState.Move();
                Draw();
            }
        }
        public Image[,] SetUpGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c< cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            ScoreText.Text = $"Score: {gameState.Score}";
            DrawGrid();
        }

        

        private void DrawGrid()
        {
            for(int r = 0; r< rows; r++)
            {
                for(int c = 0; c< cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                }
            }
        }

        private async Task ShowCountDown()
        {
            for(int i = 3; i > 0; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000);
            }
        }
    
        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(100);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to start";
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.GetSnakePosition());
            for(int i = 0; i< positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = Images.DeadBody;
                gridImages[pos.Row, pos.Column].Source = source;
                await Task.Delay(50);
            }
        }
    }
}
