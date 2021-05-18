using System;
using System.Windows;
using System.Windows.Input;

namespace XiangQiWPFDemo
{
    internal struct RowColumn
    {
        public int Row;
        public int Column;

        public RowColumn(int r, int c)
        {
            Row = r;
            Column = c;
        }

        public override string ToString()
        {
            return $"[{Row + 1}行{Column + 1}列]";
        }
    }

    internal enum ChessmanType
    {
        Che = 5,
        Ma = 4,
        Xiang = 3,
        Shi = 2,
        Jiang = 1,
        Pao = 6,
        Zu = 7,
    }

    internal static class Extensions
    {
        public static void RaiseMouseLeftDown(this UIElement obj)
        {
            var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
            args.RoutedEvent = UIElement.MouseLeftButtonDownEvent;
            obj.RaiseEvent(args);
        }

        public static void ShowMessage(string text,string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool Ask(string text, string title)
        {
            return MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }

    internal struct Move
    {
        public RowColumn Pos1;
        public RowColumn Pos2;
        public int Eaten_nType;
        public bool RedTurn;
        private int n_Type;
        public Move(RowColumn p1, RowColumn p2, int ntype)
        {
            Pos1 = p1;
            Pos2 = p2;
            Eaten_nType = 0;
            RedTurn = ntype<0;
            n_Type = ntype;
        }

        public override string ToString()
        {
            string nums = " 一二三四五六七八九";
            string result;
            if (RedTurn)
            {
                if (Pos1.Row == Pos2.Row)
                {

                    result = Chessman.ToString(n_Type);
                    result += nums[9 - Pos1.Column];
                    result += "平";
                    result += nums[9 - Pos2.Column];
                    return result;
                }
                else if (n_Type == -4|| n_Type == -3||n_Type == -2)
                {
                    result = Chessman.ToString(n_Type);
                    result += nums[9 - Pos1.Column];
                    result += Pos2.Row < Pos1.Row ? "进" : "退";
                    result += nums[9 - Pos2.Column];
                    return result;
                }
                else
                {
                    result = Chessman.ToString(n_Type);
                    result += nums[9 - Pos1.Column];
                    result += Pos2.Row < Pos1.Row ? "进" : "退";
                    result += nums[Math.Abs(Pos1.Row - Pos2.Row)];
                    return result;
                }
            }
            else
            {
                
                if (Pos1.Row == Pos2.Row)
                {

                    result = Chessman.ToString(n_Type);
                    result += Pos1.Column + 1;
                    result += "平";
                    result += Pos2.Column + 1;
                    return result;
                }
                else if (n_Type == 4|| n_Type == 3|| n_Type == 2)
                {
                    result = Chessman.ToString(n_Type);
                    result += Pos1.Column + 1;
                    result += Pos2.Row > Pos1.Row ? "进" : "退";
                    result += Pos2.Column + 1;
                    return result;
                }
                else
                {
                    result = Chessman.ToString(n_Type);
                    result += Pos1.Column + 1;
                    result += Pos2.Row > Pos1.Row ? "进" : "退";
                    result += Math.Abs(Pos1.Row - Pos2.Row);
                    return result;
                }
            }
        }
    }
}