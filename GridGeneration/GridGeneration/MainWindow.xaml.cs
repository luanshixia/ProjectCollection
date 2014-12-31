using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GridGenerationLib;

namespace GridGeneration
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        int _cols = 20;
        int _rows = 20;
        NodeGrid _grid;
        CityGrid _city;
        bool _startNewPath = true;
        Node _start;
        List<FrameworkElement> _steps = new List<FrameworkElement>();
        TextBlock[,] _labels;
        Rectangle[,] _cellRects;

        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
        }

        private void NewGame()
        {
            _grid = new NodeGrid(_cols, _rows);
            _city = new CityGrid(_cols, _rows);
            _city.InitializeCells();
            _labels = new TextBlock[_cols, _rows];
            _cellRects = new Rectangle[_cols, _rows];

            boardGrid.Width = _cols;
            boardGrid.Height = _rows;
            boardGrid.ColumnDefinitions.Clear();
            boardGrid.RowDefinitions.Clear();
            Enumerable.Range(0, _cols).ToList().ForEach(x => boardGrid.ColumnDefinitions.Add(new ColumnDefinition()));
            Enumerable.Range(0, _rows).ToList().ForEach(x => boardGrid.RowDefinitions.Add(new RowDefinition()));

            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    CityCell cell = _city.Cells[i, j];

                    Rectangle cellRect = new Rectangle();
                    cellRect.Fill = GetGradientBrush(CityGrid.GetColorOfType(cell.Type));
                    Grid.SetRow(cellRect, j);
                    Grid.SetColumn(cellRect, i);
                    boardGrid.Children.Add(cellRect);
                    _cellRects[i, j] = cellRect;

                    TextBlock label = new TextBlock { Text = cell.Score.ToString(), FontSize = 0.3, Foreground = Brushes.Black };
                    Grid.SetRow(label, j);
                    Grid.SetColumn(label, i);
                    boardGrid.Children.Add(label);
                    _labels[i, j] = label;
                }
            }
        }

        private LinearGradientBrush GetGradientBrush(Color color)
        {
            Color color0 = color;
            color0.R = (byte)(color.R + (255 - color.R) / 2);
            color0.G = (byte)(color.G + (255 - color.G) / 2);
            color0.B = (byte)(color.R + (255 - color.B) / 2);
            return new LinearGradientBrush(color0, color, 45);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void Window_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {

        }

        private void boardGrid_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            var cell = e.OriginalSource as UIElement; // mod
            int row = Grid.GetRow(cell);
            int col = Grid.GetColumn(cell);

            if (_startNewPath)
            {
                ClearSteps();
                _start = _grid.nodes[col, row];
                DrawStep(col, row, Brushes.Black);
            }
            else
            {
                AStar astar = new AStar(_grid, _start.x, _start.y, col, row);
                astar.search();
                foreach (var step in astar.path)
                {
                    DrawStep(step.x, step.y, Brushes.Gray);
                }
                DrawStep(col, row, Brushes.Black);
                DrawStep(_start.x, _start.y, Brushes.Black);
            }
            _startNewPath = !_startNewPath;
        }

        private void DrawStep(int col, int row, Brush fill)
        {
            double hintSize = 0.3;
            Ellipse step = new Ellipse { Width = hintSize, Height = hintSize, Fill = fill };
            Grid.SetRow(step, row);
            Grid.SetColumn(step, col);
            boardGrid.Children.Add(step);
            _steps.Add(step);
        }

        private void ClearSteps()
        {
            _steps.ForEach(x => boardGrid.Children.Remove(x));
        }

        public void Run(int n)
        {
            _city.Run(n);
            UpdateGameBoard();
        }

        public void UpdateGameBoard()
        {
            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    CityCell cell = _city.Cells[i, j];
                    _cellRects[i, j].Fill = GetGradientBrush(CityGrid.GetColorOfType(cell.Type));
                    _labels[i, j].Text = cell.Score.ToString();
                }
            }
        }

        private void Window_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                this.Run(1);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            NewGameWindow ngw = new NewGameWindow { Owner = this };
            if (ngw.ShowDialog() == true)
            {
                _cols = ngw.ColCount;
                _rows = ngw.RowCount;
                NewGame();
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
