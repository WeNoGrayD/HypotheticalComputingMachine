using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using HypotheticalComputingMachine;
using HypotheticalComputingMachineApp.Converters;
using System.Windows;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using HypotheticalComputingMachineApp.DataModels;
using HypotheticalComputingMachineApp.ValidationRules;
using System.Windows.Controls;
using HypotheticalComputingMachineClassLib.HCMModel_D1;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace HypotheticalComputingMachineApp
{
    public enum DisplayingNumberStyles
    {
        Integer = 0,
        Hex = 1,
        Binary = 2
    }

    public class HCMViewModel
    {
        private HCMWindow _view;

        private Lazy<HelpWindow> _winHelp = new Lazy<HelpWindow>();

        private HelpWindow WinHelp { get => _winHelp.Value; } 

        private IHypotheticalComputingMachineD1Model _model;

        private DisplayingNumberStyles _displayingNumberStyle;

        public DisplayingNumberStyles DisplayingNumberStyle
        {
            get { return _displayingNumberStyle; }
            set
            {
                if (value != _displayingNumberStyle)
                {
                    _displayingNumberStyle = value;
                    OnDisplayingNumberStyleChanged();
                }
            }
        }

        public event Action<DisplayingNumberStyles> DisplayingNumberStyleChanged;

        public HCMViewModel(HCMWindow view)
        {
            _view = view;
            _model = new D1Model();

            InitConvertersAndValidationRules();

            DisplayingNumberStyle = DisplayingNumberStyles.Binary;

            BindViewAndModel();
        }

        private void InitConvertersAndValidationRules()
        {
            ResourceDictionary converters = new ResourceDictionary();
            HCMDataConverter halfByteConverter = new HCMHalfByteConverter(this),
                             byteConverter = new HCMByteConverter(this),
                             dwordConverter = new HCMDWordConverter(this);
            HCMFlagRegisterConverter flagsConverter = new HCMFlagRegisterConverter(this);
            HCMRKConverter rkConverter = new HCMRKConverter(halfByteConverter, byteConverter);

            converters.Add("HalfByteConverter", halfByteConverter);
            converters.Add("ByteConverter", byteConverter);
            converters.Add("DWordConverter", dwordConverter);
            converters.Add("FlagsConverter", flagsConverter);
            converters.Add("RKConverter", rkConverter);

            _view.Resources.MergedDictionaries.Add(converters);

            ResourceDictionary validationRules = new ResourceDictionary();
            HCMDataValidationRule halfByteValidationRule = new HCMHalfByteValidationRule(halfByteConverter),
                                  byteValidationRule = new HCMByteValidationRule(byteConverter),
                                  dwordValidationRule = new HCMDWordValidationRule(dwordConverter);
            HCMFlagRegisterValidationRule flagsValidationRule = new HCMFlagRegisterValidationRule(4);
            DSAddressValidationRule startAddressValidationRule = new DSStartAddressValidationRule(),
                                    endAddressValidationRule = new DSEndAddressValidationRule();
            DSAddressValidationRule.MaxAddress = 255;
            DSAddressValidationRule.AddressGeneralValidationRule = byteValidationRule;

            validationRules.Add("VisibleMemoryStartAddressValidationRule", startAddressValidationRule);
            validationRules.Add("VisibleMemoryEndAddressValidationRule", endAddressValidationRule);
            validationRules.Add("HalfByteValidationRule", halfByteValidationRule);
            validationRules.Add("ByteValidationRule", byteValidationRule);
            validationRules.Add("DWordValidationRule", dwordValidationRule);
            validationRules.Add("FlagsValidationRule", dwordValidationRule);

            _view.Resources.MergedDictionaries.Add(validationRules);

            return;
        }

        private void AddKeyboardCommands()
        {
            _view.KeyUp += (o, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.F5: 
                        { RunProgramTilEnd(o, e); break; }
                    case System.Windows.Input.Key.F10:
                    case System.Windows.Input.Key.Enter when _model.Program.State == ProgramState.RunningByStep: 
                        { RunProgramByStep(o, e); break; }
                }
            };

            return;
        }

        private void BindViewAndModel()
        {
            _view.DataContext = _model;

            return;
        }

        private void OnDisplayingNumberStyleChanged()
        {
            DisplayingNumberStyleChanged?.Invoke(_displayingNumberStyle);

            return;
        }

        public void InitAfterInitializeComponent(List<TextBox> displays)
        {
            foreach (var display in displays)
            {
                DisplayingNumberStyleChanged += (dns) => UpdateTargetBinding(display);
            }
            DisplayingNumberStyleChanged += (dns) =>
            {
                foreach (var col in _view.dgCache.Columns)
                {
                    foreach (var item in _view.dgCache.Items)
                        UpdateTargetBinding(col.GetCellContent(item));
                }
            };
            DisplayingNumberStyleChanged += (dns) =>
            {
                foreach (var col in _view.dgDS.Columns)
                {
                    foreach (var item in _view.dgDS.Items) 
                        UpdateTargetBinding(col.GetCellContent(item));
                }
            };

            AddKeyboardCommands();

            return;
        }

        private void UpdateTargetBinding(FrameworkElement sender)
        {
            if (sender is null) return;

            switch (sender)
            {
                case TextBox:
                    {
                        sender.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        break;
                    }
                case TextBlock:
                    {
                        sender.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                        break;
                    }
                default: break;
            }

            return;
        }

        public void InvokeHelp(object sender, RoutedEventArgs e) 
        {
            string helpFilePath = "HypotheticalComputingMachineApp.Help.Help.txt";
            using (var helpFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(helpFilePath))
            using (StreamReader reader = new StreamReader(helpFile))
            {
                WinHelp.FillHelp(reader.ReadToEnd());
                WinHelp.Show();
            }

            return; 
        }

        public void ResetConfig(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                    "Вы точно хотите сбросить текущую конфигурацию ГВМ и создать новую?",
                    "Сброс конфигурации ГВМ D1",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel)
                != MessageBoxResult.OK)
            { return; }

            _view.DataContext = null;
            _model.Reset();
            _view.DataContext = _model;
        }

        public void SaveConfig(object sender, RoutedEventArgs e) 
        {
            SaveFileDialog saveConfig = new SaveFileDialog()
            {
                Title = "Открытие файла конфигурации ГВМ D1",
                AddExtension = true,
                CheckPathExists = false,
                OverwritePrompt = true,
                DefaultExt = ".txt",
                Filter = "Текстовые документы (.txt)|*.txt"
            };

            bool? dialogResult = saveConfig.ShowDialog();

            if (dialogResult != true) return;

            string configPath = saveConfig.FileName;
            _model.SaveToFile(configPath);

            return;
        }

        public void LoadConfig(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadConfig = new OpenFileDialog()
            { 
                Title = "Открытие файла конфигурации ГВМ D1",
                AddExtension = true, 
                CheckPathExists = true,
                Multiselect = false,
                DefaultExt = ".txt",
                Filter = "Текстовые документы (.txt)|*.txt"
            };

            bool? dialogResult = loadConfig.ShowDialog();

            if (dialogResult != true) return;

            _view.DataContext = null;
            string configPath = loadConfig.FileName;
            _model.LoadFromFile(configPath);

            _view.DataContext = _model;

            return;
        }

        public void RunProgramTilEnd(object sender, RoutedEventArgs e)
        {
            _model.Program.RunTilEnd();

            MessageBox.Show(
                    "Останов программы.",
                    "Программа ГВМ D1 остановлена",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);

            return;
        }

        public void RunProgramByStep(object sender, RoutedEventArgs e)
        {
            _model.Program.RunByStep();

            if (_model.Program.EOF)
                MessageBox.Show(
                    "Останов программы.",
                    "Программа ГВМ D1 остановлена",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);

            return;
        }
    }
}
