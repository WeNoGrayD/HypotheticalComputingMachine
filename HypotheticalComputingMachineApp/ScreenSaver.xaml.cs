using HypotheticalComputingMachine;
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
using System.Timers;

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
            this.Loaded += TurnOnTimer;
        }

        public void ShowMainWindow()
        {
            new HCMWindow().Show();
            this.Close();

            return;
        }

        private void TurnOnTimer(object sender, RoutedEventArgs e)
        {
            Timer t = new Timer(TimeSpan.FromSeconds(3));
            t.Elapsed += (_, _) =>
            {
                this.Dispatcher.Invoke(ShowMainWindow);
            };
            t.AutoReset = false;
            t.Enabled = true;
            t.Start();
        }
    }
}
