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

namespace HypotheticalComputingMachineApp
{
    /// <summary>
    /// Логика взаимодействия для HelpWindows.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public bool ShallBeTerminated = false;

        public HelpWindow()
        {
            InitializeComponent();
            this.Closing += HideOnClosing;
        }

        public void HideOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ShallBeTerminated) return;

            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            return;
        }

        public void FillHelp(string helpText)
        {
            txtbHelp.Text = helpText;

            return;
        }
    }
}
