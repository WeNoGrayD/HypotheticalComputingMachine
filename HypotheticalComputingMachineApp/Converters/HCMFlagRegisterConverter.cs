using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using HypotheticalComputingMachineClassLib;

namespace HypotheticalComputingMachineApp.Converters
{
    public class HCMFlagRegisterConverter : IValueConverter
    {
        public HCMFlagRegisterConverter(HCMViewModel viewModel)
        {
            return;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool[] flags = (bool[])value;
            string flagsStr = "";

            for (int i = 0; i < flags.Length; i++)
            {
                flagsStr += flags[i] ? '1' : '0';
            }

            return flagsStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string flagsStr = (string)value;
            bool[] flags = new bool[flagsStr.Length];

            for (int i = 0; i < flags.Length; i++)
            {
                flags[i] = flagsStr[i] == '1';
            }

            return flags;
        }
    }
}
