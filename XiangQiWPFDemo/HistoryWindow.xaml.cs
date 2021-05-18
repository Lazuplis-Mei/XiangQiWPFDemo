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
    /// HistoryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
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
    }
}
