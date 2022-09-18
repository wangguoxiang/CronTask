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

namespace CronTask.MVVM.Views
{
    /// <summary>
    /// HttpsCheckerView.xaml 的交互逻辑
    /// </summary>
    public partial class HttpsCheckerView : Window
    {
        public HttpsCheckerView()
        {
            InitializeComponent();
        }


        void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        #region <!-- Metro Window Style -->
        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void PART_HELP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Window Metro Theme v1.0\n.");
        }
        #endregion

        private void HttpsControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
