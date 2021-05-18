using System;
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
using System.Windows.Shapes;

namespace XiangQiWPFDemo
{
    /// <summary>
    /// FENCode.xaml 的交互逻辑
    /// </summary>
    public partial class FENCodeWindow : Window
    {
        public FENCodeWindow()
        {
            InitializeComponent();
        }

        public bool ToBeClose;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ToBeClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentTB.Text);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Board.SetFENBoard(LoadTB.Text);
            PreviewTB.Text = Board.RowToString();
            CurrentTB.Text = Board.GetFENCode();
        }
    }
}
