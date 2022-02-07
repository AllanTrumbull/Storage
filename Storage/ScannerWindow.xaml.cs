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

namespace Storage
{
    /// <summary>
    /// Логика взаимодействия для ScannerWindow.xaml
    /// </summary>
    public partial class ScannerWindow : Window
    {
        public ScannerWindow()
        {
            InitializeComponent();
            boxSN.Focus();
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //var mainWindow = ((MainWindow)Owner);
            //mainWindow.UpdateList();
        }
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        public string SN => boxSN.Text;
    }
}
