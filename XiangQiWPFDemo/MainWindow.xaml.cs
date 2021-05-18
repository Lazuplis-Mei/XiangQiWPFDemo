using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XiangQiWPFDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int Turn = -1;
        public static Image ImgBoard;
        private static Stack<Move> _historyMoves = new Stack<Move>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Turn < 0)
            {
                if (Board.AllChessmans.Count == 0)
                {
                    Board.SetDefaultBoard();
                    if (_fencode.IsVisible)
                    {
                        FENCodeButton_Click(null, null);
                    }
                }
                Turn = Turn == -1 ? 0 : 1;
                StartButton.IsEnabled = false;
                LayoutButton.IsEnabled = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Board.Initialize(canvas, FocusBoxRed, FocusBoxBlue);
            ImgBoard = Img_Board;
            nType = 7;
            SetPreviewChessman(nType);
            _fencode = new FENCodeWindow();
            _history = new HistoryWindow();
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            _historyMoves.Clear();
            HistoryButton.IsEnabled = false;
            if (Turn >= 0)
            {
                Board.ClearBoard();
                Turn = -1;
                RegretButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                LayoutButton.IsEnabled = true;
            }
            else if (LayoutButton.IsChecked == true)
            {
                LayoutButton.IsChecked = false;
            }
            else if (Board.AllChessmans.Count > 0) 
            {
                if (Extensions.Ask("棋盘上仍有棋子，确定要退出吗？","退出？"))
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
            if (_fencode.IsVisible)
            {
                FENCodeButton_Click(null, null);
            }
            if (_history.IsVisible)
            {
                HistoryButton_Click(null, null);
            }
        }

        private void Img_Board_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsLayoutMode)
            {
                var rc = (RowColumn)PreviewChessman.Tag;
                if(Board.SetChessman(nType, rc.Row, rc.Column,false))
                {
                    Board.PlayClickSound();
                    if (_fencode.IsVisible)
                    {
                        FENCodeButton_Click(null, null);
                    }
                }
            }
            else if (Board.Focused != null)
            {
                Point pos = e.GetPosition(canvas);
                var p1 = new RowColumn(Board.Focused.Row, Board.Focused.Column);
                if (Board.Focused.TryMoveTo(pos, out Move move))
                {
                    _historyMoves.Push(move);
                    if (_fencode.IsVisible)
                    {
                        FENCodeButton_Click(null, null);
                    }
                    if (_history.IsVisible)
                    {
                        HistoryButton_Click(null, null);
                    }
                    Board.Focused = null;
                    Board.FocusBoxRed.Visibility = Visibility.Hidden;
                    Board.FocusBoxBlue.Visibility = Visibility.Hidden;
                    RegretButton.IsEnabled = true;
                    HistoryButton.IsEnabled = true;
                    Turn++;
                }
            }
        }

        private void RegretButton_Click(object sender, RoutedEventArgs e)
        {
            if (_historyMoves.Count > 0)
            {
                var move = _historyMoves.Pop();
                var chessman = Board.GetChessman(move.Pos2.Row, move.Pos2.Column);
                chessman.MoveBack(move.Pos1);
                Board.Focus(chessman);
                if (move.Eaten_nType != 0)
                {
                    Board.SetChessman(move.Eaten_nType, move.Pos2.Row, move.Pos2.Column);
                }
                Turn--;
                if (Turn == 0)
                {
                    RegretButton.IsEnabled = false;
                    HistoryButton.IsEnabled = false;
                }
                if (_fencode.IsVisible)
                {
                    FENCodeButton_Click(null, null);
                }
                if (_history.IsVisible)
                {
                    HistoryButton_Click(null, null);
                }
            }
        }

        public static bool IsLayoutMode;
        private int nType;

        private void SetPreviewChessman(int n_type)
        {
            PreviewChessman.Source = Chessman.GetImgsrcAndType(n_type);
        }

        private void LayoutButton_Checked(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            Extensions.ShowMessage("通过鼠标(左/右)键(放置/移除)对应的棋子，滚轮切换棋子类型\n同时还可以使用面板中的导入功能", "布局模式已开启");
            IsLayoutMode = true;
        }

        private void LayoutButton_Unchecked(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            IsLayoutMode = false;
            PreviewChessman.Visibility = Visibility.Hidden;
        }

        private void Img_Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsLayoutMode)
            {
                PreviewChessman.Visibility = Visibility.Visible;
                var p = Chessman.GridPoint(e.GetPosition(canvas), out RowColumn rc);
                Canvas.SetLeft(PreviewChessman, p.X);
                Canvas.SetTop(PreviewChessman, p.Y);
                PreviewChessman.Tag = rc;
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                nType++;
                if (nType == 0)
                {
                    nType = 1;
                }
            }
            else
            {
                nType--;
                if (nType == 0)
                {
                    nType = -1;
                }
            }
            if (nType > 7)
            {
                nType = -7;
            }
            if (nType < -7)
            {
                nType = 7;
            }
            SetPreviewChessman(nType);
        }

        private void Img_Board_MouseLeave(object sender, MouseEventArgs e)
        {
            PreviewChessman.Visibility = Visibility.Hidden;
        }

        public static FENCodeWindow _fencode;
        private HistoryWindow _history;

        private void FENCodeButton_Click(object sender, RoutedEventArgs e)
        {
            _fencode.PreviewTB.Text = Board.RowToString();
            _fencode.CurrentTB.Text = Board.GetFENCode();
            _fencode.LoadTB.Clear();
            _fencode.LoadButton.IsEnabled = IsLayoutMode;
            _fencode.LoadTB.IsEnabled = IsLayoutMode;
            _fencode.Show();
            _fencode.Activate();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _fencode.ToBeClose = true;
            _fencode.Close();
            _history.ToBeClose = true;
            _history.Close();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _history.RedHistoryLB.Items.Clear();
            _history.BlackHistoryLB.Items.Clear();
            var moves = _historyMoves.ToArray();
            Array.Reverse(moves);
            foreach (var item in moves)
            {
                if (item.RedTurn)
                {
                    _history.RedHistoryLB.Items.Add(item.ToString());
                }
                else
                {
                    _history.BlackHistoryLB.Items.Add(item.ToString());
                }
            }
            _history.Show();
            _history.Activate();
        }
    }
}