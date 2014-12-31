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

namespace BlackWhite
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _size = 8;
        private Board _board; // = new Board(8);
        private Piece _color = Piece.Black;
        private List<Ellipse> _pieces = new List<Ellipse>();
        private List<Ellipse> _possibles = new List<Ellipse>();
        private MouseButtonEventHandler cell_Clicked;
        private MouseButtonEventHandler cell_Clicked_old;
        private GameAi _ai;
        private Piece _opponent;
        private string message = "Welcome to BnW! ";

        public MainWindow()
        {
            InitializeComponent();

            _board = new Board(_size);
            cell_Clicked = SinglePlayerHandler;
            cell_Clicked_old = SinglePlayerHandler;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void NewGame()
        {
            _board = new Board(_size);
            _ai = new GameAi(_board, Piece.White);
            _opponent = _color == Piece.Black ? Piece.White : Piece.Black;
            _color = Piece.Black; // new 20110728
            //cell_Clicked = SinglePlayerHandler;

            boardGrid.MouseLeftButtonDown -= cell_Clicked_old;
            boardGrid.MouseLeftButtonDown += cell_Clicked; // new
            boardGrid.ColumnDefinitions.Clear();
            boardGrid.RowDefinitions.Clear();
            Enumerable.Range(0, _size).ToList().ForEach(x => boardGrid.ColumnDefinitions.Add(new ColumnDefinition()));
            Enumerable.Range(0, _size).ToList().ForEach(x => boardGrid.RowDefinitions.Add(new RowDefinition()));

            LinearGradientBrush lgb = new LinearGradientBrush(Colors.Orange, Colors.Peru, 45);

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Rectangle cellBackground = new Rectangle();
                    cellBackground.Fill = lgb;
                    //cellBackground.MouseLeftButtonDown += cell_Clicked;
                    Grid.SetRow(cellBackground, i);
                    Grid.SetColumn(cellBackground, j);
                    boardGrid.Children.Add(cellBackground);
                }
            }

            DrawPieces();
            DrawPossibleSteps();
        }

        private void ChangeColor()
        {
            _color = _color == Piece.Black ? Piece.White : Piece.Black;
        }

        private void DoublePlayerHandler(object sender, RoutedEventArgs e)
        {
            var cell = e.OriginalSource as UIElement; // mod
            int row = Grid.GetRow(cell);
            int col = Grid.GetColumn(cell);

            if (_board.StepReal(_color, Tuple.Create(row, col)) > 0)
            {
                message = "Good luck!";

                DrawPieces();
                DrawPossibleSteps();
                ChangeColor();
            }
        }

        private void SinglePlayerHandler(object sender, RoutedEventArgs e)
        {
            var cell = e.OriginalSource as UIElement; // mod
            int row = Grid.GetRow(cell);
            int col = Grid.GetColumn(cell);

            var validPosList = _board.GetPossibleSteps(_color);
            if (!validPosList.Any(x => x.Item1 == row && x.Item2 == col))
            {
                return;
            }

            if (_board.StepReal(_color, Tuple.Create(row, col)) > 0)
            {
                // 绘制人走棋后的棋盘
                ChangeColor();
                DrawPieces();
                DrawPossibleSteps();
                message = "Computer is thinking...";
                UpdateStatusBar();

                // 电脑思考0.5秒后走棋
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                timer.Tick += (sender1, e1) =>
                {
                    timer.Stop();
                    System.Threading.Thread.Sleep(500);

                    ChangeColor();
                    message = "Good luck!";
                    var aiStep = _ai.Step();
                    _board.StepReal(_opponent, aiStep);
                    DrawPieces();
                    DrawPossibleSteps();
                };
                timer.Start();
            }
        }

        private void DrawPieces()
        {
            _pieces.ForEach(x => boardGrid.Children.Remove(x));
            _pieces.Clear();

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Piece color = _board[i, j];
                    if (color != Piece.Empty)
                    {
                        Ellipse piece = new Ellipse();
                        if (color == Piece.Black)
                        {
                            piece.Fill = Brushes.Black;
                        }
                        else
                        {
                            piece.Fill = Brushes.White;
                        }
                        Grid.SetRow(piece, i);
                        Grid.SetColumn(piece, j);
                        _pieces.Add(piece);
                        boardGrid.Children.Add(piece);
                    }
                }
            }

            UpdateStatusBar();
        }

        private void DrawPossibleSteps()
        {
            _possibles.ForEach(x => boardGrid.Children.Remove(x));
            _possibles.Clear();

            double hintSize = boardGrid.Height / _board.Size / 5;

            var possiblePosList = _board.GetPossibleSteps(_color);
            foreach (var pos in possiblePosList)
            {
                Ellipse possible = new Ellipse { Width = hintSize, Height = hintSize, Opacity = 0.6 };
                if (_color == Piece.Black)
                {
                    possible.Fill = Brushes.Black;
                }
                else
                {
                    possible.Fill = Brushes.White;
                }
                Grid.SetRow(possible, pos.Item1);
                Grid.SetColumn(possible, pos.Item2);
                _possibles.Add(possible);
                boardGrid.Children.Add(possible);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double size = Math.Min(root.ActualHeight, root.ActualWidth) - 100;
            boardGrid.Height = size;
            boardGrid.Width = size;

            DrawPossibleSteps();
        }

        private void UpdateStatusBar()
        {
            info.Text = string.Format("Black={0}, White={1} | Current={2} | {3}", _board.GetCount(Piece.Black), _board.GetCount(Piece.White), _color, message);
        }

        private void Exit_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void QuickNew_Clicked(object sender, RoutedEventArgs e)
        {
            if (TaskDialog.Show("Are you sure to discard current game?", "Open your eyes round", "BnW", TaskDialogButtons.Yes | TaskDialogButtons.No, TaskDialogIcon.SecurityWarning) == TaskDialogResult.Yes)
            {
                NewGame();
            }
        }

        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            string s = "BnW 0.9\n  -- Tiny Black and White Chess Game\nDesigned by WY in Shanghai in 2011\nPowered by Microsoft WPF";
            //MessageBox.Show(s, "About BnW", MessageBoxButton.OK, MessageBoxImage.Information);
            TaskDialog.Show(s, "Let me tell you...", "About BnW", TaskDialogButtons.OK, TaskDialogIcon.SecuritySuccess);
        }

        private void New_Clicked(object sender, RoutedEventArgs e)
        {
            NewGameWindow ngw = new NewGameWindow();
            if (ngw.ShowDialog() == true)
            {
                if (TaskDialog.Show("Are you sure to discard current game?", "Open your eyes round", "BnW", TaskDialogButtons.Yes | TaskDialogButtons.No, TaskDialogIcon.SecurityWarning) == TaskDialogResult.Yes)
                {
                    _size = (int)ngw.num1.Value;
                    if (ngw.rbSingle.IsChecked == true)
                    {
                        cell_Clicked_old = cell_Clicked;
                        cell_Clicked = SinglePlayerHandler;
                    }
                    else
                    {
                        cell_Clicked_old = cell_Clicked;
                        cell_Clicked = DoublePlayerHandler;
                    }
                    NewGame();
                }
            }
        }
    }
}
