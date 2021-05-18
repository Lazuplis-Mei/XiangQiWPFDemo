using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace XiangQiWPFDemo
{
    internal class Chessman
    {
        public Image Image;
        public bool IsRed;
        public ChessmanType Type;
        public int Row;
        public int Column;

        private static ImageSourceConverter _imgsrcConverter = new ImageSourceConverter();

        public Chessman(int nType, int row, int column)
        {
            Row = row;
            Column = column;
            IsRed = nType < 0;
            Image = new Image();
            Type = (ChessmanType)(Math.Abs(nType));
            Image.Source = GetImgsrcAndType(nType);
            Image.Width = 50;
            Image.Height = 50;
            Image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            Image.MouseRightButtonDown += Image_MouseRightButtonDown;
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.IsLayoutMode)
            {
                Board.RemoveChessman(Row, Column);
                if (MainWindow._fencode.IsVisible)
                {
                    MainWindow._fencode.PreviewTB.Text = Board.RowToString();
                    MainWindow._fencode.CurrentTB.Text = Board.GetFENCode();
                    MainWindow._fencode.LoadTB.Clear();
                    MainWindow._fencode.LoadButton.IsEnabled = MainWindow.IsLayoutMode;
                    MainWindow._fencode.LoadTB.IsEnabled = MainWindow.IsLayoutMode;
                    MainWindow._fencode.Show();
                }
            }
        }

        public static ImageSource GetImgsrcAndType(int n_type)
        {
            var type = (ChessmanType)(Math.Abs(n_type));
            string imgsrcStr = "Images/";
            imgsrcStr += n_type < 0 ? "r_" : "b_";
            imgsrcStr += char.ToLower(type.ToString().First());
            imgsrcStr += ".png";
            return _imgsrcConverter.ConvertFromString(imgsrcStr) as ImageSource;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.Turn >=0 && (MainWindow.Turn % 2 == 0 && IsRed
                || (MainWindow.Turn % 2 == 1 && !IsRed)) && Board.Focused != this)
            {
                Board.Focus(this);
            }
            else if(!MainWindow.IsLayoutMode)
            {
                MainWindow.ImgBoard.RaiseMouseLeftDown();
            }
        }

        public static string ToString(int nType)
        {
            ChessmanType type;
            bool red = false;
            if (nType < 0)
            {
                red = true;
                type = (ChessmanType)(-nType);
            }
            else
            {
                type = (ChessmanType)nType;
            }
            switch (type)
            {
                case ChessmanType.Che:
                    return red ? "车" : "車";

                case ChessmanType.Ma:
                    return red ? "马" : "馬";

                case ChessmanType.Xiang:
                    return red ? "相" : "象";

                case ChessmanType.Shi:
                    return red ? "仕" : "士";

                case ChessmanType.Jiang:
                    return red ? "帅" : "将";

                case ChessmanType.Pao:
                    return red ? "炮" : "砲";

                case ChessmanType.Zu:
                    return red ? "兵" : "卒";

                default:
                    return "  ";
            }
        }

        private static RowColumn[] RedShiPoint = {
            new RowColumn(9, 3),
            new RowColumn(9, 5),
            new RowColumn(8, 4),
            new RowColumn(7, 3),
            new RowColumn(7, 5),};

        private static RowColumn[] BlackShiPoint = {
            new RowColumn(0, 3),
            new RowColumn(0, 5),
            new RowColumn(1, 4),
            new RowColumn(2, 3),
            new RowColumn(2, 5),};
        private static RowColumn[] RedXiangPoint = {
            new RowColumn(7, 0),
            new RowColumn(5, 2),
            new RowColumn(9, 2),
            new RowColumn(7, 4),
            new RowColumn(5, 6),
            new RowColumn(9, 6),
            new RowColumn(7, 8),};

        private static RowColumn[] BlackXiangPoint = {
            new RowColumn(2, 0),
            new RowColumn(4, 2),
            new RowColumn(0, 2),
            new RowColumn(2, 4),
            new RowColumn(4, 6),
            new RowColumn(0, 6),
            new RowColumn(2, 8),};

        public bool IsVaild()
        {
            if (Type == ChessmanType.Jiang)
            {
                if ((Row < 7 && Row > 2) || Column < 3 || Column > 5)
                {
                    return false;
                }
            }
            if (IsRed)
            {
                if (Type== ChessmanType.Shi)
                {
                    return RedShiPoint.Contains(new RowColumn(Row, Column));
                }
                else if (Type == ChessmanType.Xiang)
                {
                    return RedXiangPoint.Contains(new RowColumn(Row, Column));
                }
                else if (Type == ChessmanType.Zu)
                {
                    if (Row > 4 && Column % 2 != 0)
                    {
                        return false;
                    }
                    return Row < 7;
                }
            }
            else
            {
                if (Type == ChessmanType.Shi)
                {
                    return BlackShiPoint.Contains(new RowColumn(Row, Column));
                }
                else if (Type == ChessmanType.Xiang)
                {
                    return BlackXiangPoint.Contains(new RowColumn(Row, Column));
                }
                else if (Type == ChessmanType.Zu)
                {
                    if (Row < 5 && Column % 2 != 0)
                    {
                        return false;
                    }
                    return Row > 2;
                }
            }
            return true;
        }

        public void MoveBack(RowColumn pos)
        {
            Board.CurrentBoard[pos.Row, pos.Column] = Get_nType();
            Board.CurrentBoard[Row, Column] = 0;
            Row = pos.Row;
            Column = pos.Column;
            Board.Update(this);
        }

        public int Get_nType()
        {
            return IsRed ? -(int)Type : (int)Type;
        }

        public override string ToString()
        {
            string result = IsRed ? "红 " : "黑";
            result += ToString(Get_nType());
            result += $"[{Row + 1}行{Column + 1}列]";
            return result;
        }

        public bool TryMoveTo(Point p, out Move move)
        {
            foreach (var pos in GetPossiblePoints())
            {
                if (PointInPosition(p, pos))
                {
                    move = new Move(new RowColumn(Row, Column), pos, Get_nType());
                    if (Board.ExistsChessman(pos.Row, pos.Column))
                    {
                        move.Eaten_nType = Board.CurrentBoard[pos.Row, pos.Column];
                        Board.RemoveChessman(pos.Row, pos.Column);
                    }
                    Board.CurrentBoard[Row, Column] = 0;
                    Row = pos.Row;
                    Column = pos.Column;
                    Board.CurrentBoard[Row, Column] = Get_nType();
                    Board.Update(this);

                    return true;
                }
            }
            move = default;
            return false;
        }

        private static bool PointInPosition(Point p, RowColumn pos)
        {
            return (pos.Column * Board.UnitWidth < p.X)
                && ((pos.Column + 1) * Board.UnitWidth > p.X)
                && (pos.Row * Board.UnitHeight < p.Y)
                && ((pos.Row + 1) * Board.UnitWidth > p.Y);
        }

        public static Point GridPoint(Point p, out RowColumn rc)
        {
            rc = new RowColumn((int)(p.Y / Board.UnitHeight), (int)(p.X / Board.UnitWidth));
            rc.Column = Math.Min(Math.Max(0, rc.Column), 8);
            rc.Row = Math.Min(Math.Max(0, rc.Row), 9);
            return new Point(rc.Column * Board.UnitWidth, rc.Row * Board.UnitHeight);
        }

        private List<RowColumn> GetPossiblePoints()
        {
            switch (Type)
            {
                case ChessmanType.Che:
                    return GetChePoints();

                case ChessmanType.Ma:
                    return GetMaPoints();

                case ChessmanType.Xiang:
                    return GetXiangPoints();

                case ChessmanType.Shi:
                    return GetShiPoints();

                case ChessmanType.Jiang:
                    return GetJiangPoints();

                case ChessmanType.Pao:
                    return GetPaoPoints();

                case ChessmanType.Zu:
                    return GetZuPoints();

                default:
                    return null;
            }
        }

        private List<RowColumn> GetZuPoints()
        {
            var list = new List<RowColumn>();
            if (IsRed)
            {
                list.Add(new RowColumn(Row - 1, Column));
                if (Row < 5)
                {
                    list.Add(new RowColumn(Row, Column - 1));
                    list.Add(new RowColumn(Row, Column + 1));
                }
            }
            else
            {
                list.Add(new RowColumn(Row + 1, Column));
                if (Row > 4)
                {
                    list.Add(new RowColumn(Row, Column - 1));
                    list.Add(new RowColumn(Row, Column + 1));
                }
            }

            list.RemoveAll(IsInvalidPosition);

            return list;
        }

        private bool IsInvalidPosition(RowColumn rc)
        {
            if (rc.Column < 0 || rc.Column > 8)
            {
                return true;
            }
            if (rc.Row < 0 || rc.Row > 9)
            {
                return true;
            }
            if (Board.ExistsChessman(rc))
            {
                bool red = Board.CurrentBoard[rc.Row, rc.Column] < 0;
                if (red == IsRed)
                {
                    return true;
                }
            }
            return false;
        }

        private List<RowColumn> GetPaoPoints()
        {
            var list = new List<RowColumn>();

            bool passed = false;
            for (int r = Row - 1; r >= 0; r--)
            {
                if (CheckPaoPoint(list, ref passed, r, Column))
                {
                    break;
                }
            }

            passed = false;
            for (int r = Row + 1; r <= 9; r++)
            {
                if (CheckPaoPoint(list, ref passed, r, Column))
                {
                    break;
                }
            }

            passed = false;
            for (int c = Column - 1; c >= 0; c--)
            {
                if (CheckPaoPoint(list, ref passed, Row, c))
                {
                    break;
                }
            }

            passed = false;
            for (int c = Column + 1; c <= 8; c++)
            {
                if (CheckPaoPoint(list, ref passed, Row, c))
                {
                    break;
                }
            }

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private bool CheckPaoPoint(List<RowColumn> list, ref bool passed, int r, int c)
        {
            if (Board.ExistsChessman(r, c))
            {
                if (!passed)
                {
                    passed = true;
                }
                else
                {
                    list.Add(new RowColumn(r, c));
                    return true;
                }
            }
            else
            {
                if (!passed)
                {
                    list.Add(new RowColumn(r, c));
                }
            }
            return false;
        }

        private List<RowColumn> GetJiangPoints()
        {
            var list = new List<RowColumn>();

            if (IsRed)
            {
                if (Row - 1 > 6)
                {
                    list.Add(new RowColumn(Row - 1, Column));
                }
                if (Row + 1 > 6)
                { 
                    list.Add(new RowColumn(Row + 1, Column));
                }
                for (int i = Row-1; i >= 0; i--)
                {
                    if (Board.ExistsChessman(i,Column))
                    {
                        if (Board.CurrentBoard[i, Column] == 1)
                        {
                            list.Add(new RowColumn(i, Column));
                        }
                        break;
                    }
                }
                if (Row>6)
                {
                    if (Column - 1 > 2)
                    {
                        list.Add(new RowColumn(Row, Column - 1));
                    }

                    if (Column + 1 < 6)
                    {
                        list.Add(new RowColumn(Row, Column + 1));
                    }
                }
            }
            else
            {
                if (Row + 1 < 3)
                {
                    list.Add(new RowColumn(Row + 1, Column));
                }
                if (Row - 1 < 3)
                {
                    list.Add(new RowColumn(Row - 1, Column));
                }
                for (int i = Row+1; i <= 9; i++)
                {
                    if (Board.ExistsChessman(i, Column))
                    {
                        if (Board.CurrentBoard[i, Column] == -1)
                        {
                            list.Add(new RowColumn(i, Column));
                        }
                        break;
                    }
                }
                if (Row<3)
                {
                    if (Column - 1 > 2)
                    {
                        list.Add(new RowColumn(Row, Column - 1));
                    }

                    if (Column + 1 < 6)
                    {
                        list.Add(new RowColumn(Row, Column + 1));
                    }
                }
                
            }
            

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private List<RowColumn> GetShiPoints()
        {
            var list = new List<RowColumn>();

            if (IsRed)
            {
                if (Column - 1 > 2)
                {
                    list.Add(new RowColumn(Row + 1, Column - 1));
                    if (Row - 1 > 6)
                    {
                        list.Add(new RowColumn(Row - 1, Column - 1));
                    }
                }

                if (Column + 1 < 6)
                {
                    list.Add(new RowColumn(Row + 1, Column + 1));
                    if (Row - 1 > 6)
                    {
                        list.Add(new RowColumn(Row - 1, Column + 1));
                    }
                }
            }
            else
            {
                if (Column - 1 > 2)
                {
                    list.Add(new RowColumn(Row - 1, Column - 1));
                    if (Row + 1 < 3)
                    {
                        list.Add(new RowColumn(Row + 1, Column - 1));
                    }
                }
                if (Column + 1 < 6)
                {
                    list.Add(new RowColumn(Row - 1, Column + 1));
                    if (Row + 1 < 3)
                    {
                        list.Add(new RowColumn(Row + 1, Column + 1));
                    }
                }
            }

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private List<RowColumn> GetXiangPoints()
        {
            var list = new List<RowColumn>();

            if (IsRed)
            {
                if (!Board.ExistsChessman(Row - 1, Column - 1))
                {
                    if (Row - 2 > 4)
                    {
                        list.Add(new RowColumn(Row - 2, Column - 2));
                    }
                }
                if (!Board.ExistsChessman(Row - 1, Column + 1))
                {
                    if (Row - 2 > 4)
                    {
                        list.Add(new RowColumn(Row - 2, Column + 2));
                    }
                }
                if (!Board.ExistsChessman(Row + 1, Column - 1))
                {
                    list.Add(new RowColumn(Row + 2, Column - 2));
                }
                if (!Board.ExistsChessman(Row + 1, Column + 1))
                {
                    list.Add(new RowColumn(Row + 2, Column + 2));
                }
            }
            else
            {
                if (!Board.ExistsChessman(Row - 1, Column - 1))
                {
                    list.Add(new RowColumn(Row - 2, Column - 2));
                }
                if (!Board.ExistsChessman(Row - 1, Column + 1))
                {
                    list.Add(new RowColumn(Row - 2, Column + 2));
                }
                if (!Board.ExistsChessman(Row + 1, Column - 1))
                {
                    if (Row + 2 < 5)
                    {
                        list.Add(new RowColumn(Row + 2, Column - 2));
                    }
                }
                if (!Board.ExistsChessman(Row + 1, Column + 1))
                {
                    if (Row + 2 < 5)
                    {
                        list.Add(new RowColumn(Row + 2, Column + 2));
                    }
                }
            }

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private List<RowColumn> GetMaPoints()
        {
            var list = new List<RowColumn>();

            if (!Board.ExistsChessman(Row, Column - 1))
            {
                list.Add(new RowColumn(Row + 1, Column - 2));
                list.Add(new RowColumn(Row - 1, Column - 2));
            }
            if (!Board.ExistsChessman(Row, Column + 1))
            {
                list.Add(new RowColumn(Row + 1, Column + 2));
                list.Add(new RowColumn(Row - 1, Column + 2));
            }
            if (!Board.ExistsChessman(Row - 1, Column))
            {
                list.Add(new RowColumn(Row - 2, Column - 1));
                list.Add(new RowColumn(Row - 2, Column + 1));
            }
            if (!Board.ExistsChessman(Row + 1, Column))
            {
                list.Add(new RowColumn(Row + 2, Column - 1));
                list.Add(new RowColumn(Row + 2, Column + 1));
            }

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private List<RowColumn> GetChePoints()
        {
            var list = new List<RowColumn>();

            for (int r = Row - 1; r >= 0; r--)
            {
                if (CheckChePoint(list, r, Column))
                {
                    break;
                }
            }

            for (int r = Row + 1; r <= 9; r++)
            {
                if (CheckChePoint(list, r, Column))
                {
                    break;
                }
            }

            for (int c = Column - 1; c >= 0; c--)
            {
                if (CheckChePoint(list, Row, c))
                {
                    break;
                }
            }

            for (int c = Column + 1; c <= 8; c++)
            {
                if (CheckChePoint(list, Row, c))
                {
                    break;
                }
            }

            list.RemoveAll(IsInvalidPosition);
            return list;
        }

        private bool CheckChePoint(List<RowColumn> list, int r, int c)
        {
            if (Board.ExistsChessman(r, c))
            {
                list.Add(new RowColumn(r, c));
                return true;
            }
            else
            {
                list.Add(new RowColumn(r, c));
            }
            return false;
        }
    }
}