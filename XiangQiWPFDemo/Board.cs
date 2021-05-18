using System;
using System.Collections.Generic;
using System.Media;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace XiangQiWPFDemo
{
    internal static class Board
    {
        private static Canvas _canvas;
        private static TimeSpan _interval = TimeSpan.FromMilliseconds(250);

        private static readonly int[,] _defaultBoard = {
        { 5, 4, 3, 2, 1, 2, 3, 4, 5 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 6, 0, 0, 0, 0, 0, 6, 0 },
        { 7, 0, 7, 0, 7, 0, 7, 0, 7 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        {-7, 0,-7, 0,-7, 0,-7, 0,-7 },
        { 0,-6, 0, 0, 0, 0, 0,-6, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        {-5,-4,-3,-2,-1,-2,-3,-4,-5 }, };

        public static double UnitWidth;
        public static double UnitHeight;
        public static Chessman Focused;
        public static Image FocusBoxRed;
        public static Image FocusBoxBlue;

        public static int[,] CurrentBoard;
        public static List<Chessman> AllChessmans;

        public static void SetFENBoard(string fen)
        {
            ClearBoard();
            string _cmtdict = "PCRNBAK kabnrcp";
            MainWindow.Turn = fen[fen.Length - 1] == 'b' ? -2 : -1;
            var lines = fen.Substring(0, fen.Length - 2).Split('/');
            var arr = lines.Select(str =>
              {
                  for (int i = 1; i < 10; i++)
                  {
                      str = str.Replace(i.ToString(), " ".PadLeft(i));
                  }
                  return str;
              }).ToArray();
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (arr[r][c]!=' ')
                    {
                        SetChessman(_cmtdict.IndexOf(arr[r][c]) - 7, r, c);
                    }
                }
            }
        }

        public static void PlayMoveSound()
        {
            _sound.SoundLocation = "Sounds/move.wav";
            _sound.Play();
        }
        public static void PlayClickSound()
        {
            _sound.SoundLocation = "Sounds/click.wav";
            _sound.Play();
        }
        public static void Initialize(Canvas canvas, Image fboxr, Image fboxb)
        {
            UnitWidth = canvas.ActualWidth / 9;
            UnitHeight = canvas.ActualHeight / 10;
            _canvas = canvas;
            CurrentBoard = new int[10, 9];
            AllChessmans = new List<Chessman>();
            FocusBoxRed = fboxr;
            FocusBoxBlue = fboxb;
        }

        public static void SetDefaultBoard()
        {
            ClearBoard();
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (_defaultBoard[r, c] != 0)
                    {
                        SetChessman(_defaultBoard[r, c], r, c);
                    }
                }
            }
        }

        public static void ClearBoard()
        {
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    CurrentBoard[r, c] = 0;
                }
            }
            foreach (var cm in AllChessmans)
            {
                _canvas.Children.Remove(cm.Image);
            }
            AllChessmans.Clear();
            FocusBoxRed.Visibility = System.Windows.Visibility.Hidden;
            FocusBoxBlue.Visibility = System.Windows.Visibility.Hidden;
            Focused = null;
        }

        private static DoubleAnimation _animSize = new DoubleAnimation(20, 50, _interval);

        public static bool SetChessman(int nType, int row, int column, bool anim = true)
        {
            Chessman chessman = new Chessman(nType, row, column);
            if (chessman.IsVaild())
            {
                CurrentBoard[row, column] = nType;
                AllChessmans.Add(chessman);
                Canvas.SetLeft(chessman.Image, column * UnitWidth);
                Canvas.SetTop(chessman.Image, row * UnitHeight);
                _canvas.Children.Add(chessman.Image);
                if (anim)
                {
                    chessman.Image.BeginAnimation(Image.WidthProperty, _animSize);
                    chessman.Image.BeginAnimation(Image.HeightProperty, _animSize);
                }
                return true;
            }
            return false;
        }

        public static string RowToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    strBuilder.Append(Chessman.ToString(CurrentBoard[r, c]));
                }
                strBuilder.AppendLine();
            }
            return strBuilder.ToString();
        }

        public static void Focus(Chessman chessman)
        {
            Focused = chessman;
            PlayClickSound();
            FocusBoxRed.Visibility = System.Windows.Visibility.Hidden;
            FocusBoxBlue.Visibility = System.Windows.Visibility.Hidden;
            if (chessman.IsRed)
            {
                Canvas.SetLeft(FocusBoxRed, chessman.Column * UnitWidth);
                Canvas.SetTop(FocusBoxRed, chessman.Row * UnitHeight);
                FocusBoxRed.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Canvas.SetLeft(FocusBoxBlue, chessman.Column * UnitWidth);
                Canvas.SetTop(FocusBoxBlue, chessman.Row * UnitHeight);
                FocusBoxBlue.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public static bool ExistsChessman(int row, int column)
        {
            if (row < 0 || row > 9 || column < 0 || column > 8)
            {
                return false;
            }
            return CurrentBoard[row, column] != 0;
        }

        public static bool ExistsChessman(RowColumn rc)
        {
            return ExistsChessman(rc.Row, rc.Column);
        }

        public static void RemoveChessman(int row, int column)
        {
            CurrentBoard[row, column] = 0;
            var chessman = GetChessman(row, column);
            _canvas.Children.Remove(chessman.Image);
            AllChessmans.Remove(chessman);
        }

        public static Chessman GetChessman(int row, int column)
        {
            return AllChessmans.Find(cm => cm.Row == row && cm.Column == column);
        }

        private static DoubleAnimation _animLeft = new DoubleAnimation(0, 0, _interval);
        private static DoubleAnimation _animTop = new DoubleAnimation(0, 0, _interval);


        public static string GetFENCode()
        {
            string result = "";
            string _cmtdict = "PCRNBAK kabnrcp";
            for (int i = 0; i < 10; i++)
            {
                int blank = 0;
                for (int j = 0; j < 9; j++)
                {
                    if (CurrentBoard[i, j] != 0)
                    {
                        if (blank != 0)
                        {
                            result += blank;
                            blank = 0;
                        }
                        result += _cmtdict[CurrentBoard[i, j] + 7];
                    }
                    else
                    {
                        blank++;
                    }
                }
                if (blank != 0)
                    result += blank;

                result += "/";
            }

            return result.Substring(0, result.Length - 1) + (MainWindow.Turn % 2 == 0 ? " w" : " b");
        }

        public static SoundPlayer _sound = new SoundPlayer();

        public static void Update(Chessman chessman)
        {
            _canvas.Children.Remove(chessman.Image);
            _canvas.Children.Add(chessman.Image);
            _animLeft.From = Canvas.GetLeft(chessman.Image);
            _animLeft.To = chessman.Column * UnitWidth;
            _animTop.From = Canvas.GetTop(chessman.Image);
            _animTop.To = chessman.Row * UnitHeight;
            chessman.Image.BeginAnimation(Canvas.LeftProperty, _animLeft);
            chessman.Image.BeginAnimation(Canvas.TopProperty, _animTop);
            PlayMoveSound();
        }
    }
}