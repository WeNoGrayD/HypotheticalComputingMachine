using HypotheticalComputingMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HypotheticalComputingMachineApp
{
    /// <summary>
    /// Логика взаимодействия для ScreenSaver.xaml
    /// </summary>
    public partial class ScreenSaver : Window
    {
        public ScreenSaver()
        {
            InitializeComponent();
            this.Loaded += ShowMainWindow;
        }

        private void ShowMainWindow(object sender, RoutedEventArgs e)
        {
            Timer t = new Timer((_) => new HCMWindow().Show());
            t.
        }
    }
}
