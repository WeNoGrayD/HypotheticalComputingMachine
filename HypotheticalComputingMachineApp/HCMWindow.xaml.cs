using HypotheticalComputingMachineApp;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HypotheticalComputingMachine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class HCMWindow : Window
    {
        private HCMViewModel _viewModel;

        public HCMWindow()
        {
            _viewModel = new HCMViewModel(this);

            InitializeComponent();

            List<TextBox> displays = new List<TextBox>()
            { txtbPA, txtbSAK, txtbRA, txtbRS, txtbRK, txtbRK_COP, txtbRK_M, txtbRK_A1, txtbRK_A2, txtbRK_A3, txtbOR1, txtbOR2, txtbACC, txtbVisibleMemoryStart, txtbVisibleMemoryEnd };
            _viewModel.InitAfterInitializeComponent(displays);

            this.Closing += OnClosing;

            return;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.OnClosing(sender, e);

            return;
        }

        public void InvokeHelp(object sender, RoutedEventArgs e) => _viewModel.InvokeHelp(sender, e);

        public void ResetConfig(object sender, RoutedEventArgs e) => _viewModel.ResetConfig(sender, e);

        public void SaveConfig(object sender, RoutedEventArgs e) => _viewModel.SaveConfig(sender, e);

        public void LoadConfig(object sender, RoutedEventArgs e) => _viewModel.LoadConfig(sender, e);

        public void RunProgramTilEnd(object sender, RoutedEventArgs e) => _viewModel.RunProgramTilEnd(sender, e);

        public void RunProgramByStep(object sender, RoutedEventArgs e) => _viewModel.RunProgramByStep(sender, e);



        public void ChangeDisplayingNumberStyleToInteger(object sender, RoutedEventArgs e)
        {
            _viewModel.DisplayingNumberStyle = DisplayingNumberStyles.Integer;
            UncheckOtherDisplayingNumberStyleSettings(sender);

            return;
        }

        public void ChangeDisplayingNumberStyleToHex(object sender, RoutedEventArgs e)
        {
            _viewModel.DisplayingNumberStyle = DisplayingNumberStyles.Hex;
            UncheckOtherDisplayingNumberStyleSettings(sender);

            return;
        }

        public void ChangeDisplayingNumberStyleToBinary(object sender, RoutedEventArgs e)
        {
            _viewModel.DisplayingNumberStyle = DisplayingNumberStyles.Binary;
            UncheckOtherDisplayingNumberStyleSettings(sender);

            return;
        }

        public void UncheckOtherDisplayingNumberStyleSettings(object sender)
        {
            MenuItem selected = sender as MenuItem;
            ContextMenu settings = selected.Parent as ContextMenu;

            foreach (var menuItem in settings.Items.OfType<MenuItem>())
                if (menuItem != selected) menuItem.IsChecked = false;

            return;
        }
    }
}
