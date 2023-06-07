using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using HypotheticalComputingMachineClassLib;
using System.Windows.Documents;

namespace HypotheticalComputingMachineApp.Converters
{
    public abstract class HCMDataConverter : IValueConverter
    {
        public DisplayingNumberStyles DisplayingNumberStyle { get; private set; }

        private int _bitness;

        public HCMDataConverter(HCMViewModel viewModel, int bitness)
        {
            viewModel.DisplayingNumberStyleChanged += (dns) => DisplayingNumberStyle = dns;
            _bitness = bitness;

            return;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                byte[] data => DisplayingNumberStyle switch
                {
                    DisplayingNumberStyles.Integer => BinaryInOutConverter.BytesToInt32(data).ToString(),
                    DisplayingNumberStyles.Hex => ConvertToHex(BinaryInOutConverter.BytesToInt32(data)),
                    DisplayingNumberStyles.Binary => ConvertFromBinary(data) + "b",
                    _ => ""
                },
                int data => DisplayingNumberStyle switch
                {
                    DisplayingNumberStyles.Integer => data.ToString(),
                    DisplayingNumberStyles.Hex => ConvertToHex(data),
                    DisplayingNumberStyles.Binary => ConvertFromBinary(data) + "b",
                    _ => ""
                }
            };
        }

        private string ConvertToHex(int data)
        {
            string formatted = string.Format("{0:X" + (_bitness >> 2) + "}h", data);

            return data >= 0 ? formatted : formatted[^((_bitness >> 2) + 1)..];
        }

        protected abstract string ConvertFromBinary(byte[] value);

        protected abstract string ConvertFromBinary(int value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = ConvertBackToInt32(value);
            int integer = converter.Result;

            return (string)parameter == "intRepr" ? integer : ConvertBackFromInt32Finally(integer);
        }

        public (bool Success, int Result)  ConvertBackToInt32(object value)
        {
            int integer;

            return DisplayingNumberStyle switch
            {
                DisplayingNumberStyles.Integer => (int.TryParse((string)value, NumberStyles.Integer, null, out integer), integer),
                DisplayingNumberStyles.Hex => (int.TryParse(((string)value)[..^1], NumberStyles.HexNumber, null, out integer), integer),
                DisplayingNumberStyles.Binary => (true, ConvertBackFromBinary(((string)value)[0..^1])),
                _ => (false, 0)
            };
        }

        protected abstract int ConvertBackFromBinary(string value);

        protected abstract object ConvertBackFromInt32Finally(int value);
    }

    public class HCMHalfByteConverter : HCMDataConverter
    {
        public HCMHalfByteConverter(HCMViewModel viewModel)
            : base(viewModel, 4)
        {
            return;
        }

        protected override string ConvertFromBinary(byte[] value)
        {
            return BinaryInOutConverter.ByteToBinaryStr(value[0])[4..];
        }

        protected override string ConvertFromBinary(int value)
        {
            return BinaryInOutConverter.ByteToBinaryStr((byte)(value & 0b1111));
        }

        protected override int ConvertBackFromBinary(string value)
        {
            return BinaryInOutConverter.BinaryStrToByte("0000" + value);
        }

        protected override object ConvertBackFromInt32Finally(int value)
        {
            return new byte[] { (byte)(value & 0b1111) };
        }
    }

    public class HCMByteConverter : HCMDataConverter
    {
        public HCMByteConverter(HCMViewModel viewModel)
            : base(viewModel, 8)
        {
            return;
        }

        protected override string ConvertFromBinary(byte[] value)
        {
            return BinaryInOutConverter.ByteToBinaryStr(value[0]);
        }

        protected override string ConvertFromBinary(int value)
        {
            return BinaryInOutConverter.ByteToBinaryStr((byte)value);
        }

        protected override int ConvertBackFromBinary(string value)
        {
            return BinaryInOutConverter.BinaryStrToByte(value);
        }

        protected override object ConvertBackFromInt32Finally(int value)
        {
            return new byte[] { (byte)value };
        }
    }

    public class HCMDWordConverter : HCMDataConverter
    {
        public HCMDWordConverter(HCMViewModel viewModel)
            : base(viewModel, 32)
        {
            return;
        }

        protected override string ConvertFromBinary(byte[] value)
        {
            return BinaryInOutConverter.BytesToLongBinaryStr(value, ' ');
        }

        protected override string ConvertFromBinary(int value)
        {
            return BinaryInOutConverter.Int32ToLongBinaryStr(value);
        }

        protected override int ConvertBackFromBinary(string value)
        {
            return BinaryInOutConverter.LongBinaryStrToInt32(value);
        }

        protected override object ConvertBackFromInt32Finally(int value)
        { 
            return BinaryInOutConverter.Int32ToBytes(value);
        }
    }
    public class HCMRKConverter : IValueConverter
    {
        private HCMDataConverter _halfByteConverter;

        private HCMDataConverter _byteConverter;

        public HCMRKConverter(
            HCMDataConverter halfByteConverter, 
            HCMDataConverter byteConverter)
        {
            _halfByteConverter = halfByteConverter;
            _byteConverter = byteConverter;

            return;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = value as byte[];

            return parameter switch
            {
                "COP" => _halfByteConverter.Convert(new byte[] { (byte)(bytes[0] >> 4) }, targetType, parameter, culture),
                "M" => _halfByteConverter.Convert(new byte[] { (byte)(bytes[0] & 0b1111) }, targetType, parameter, culture),
                "A1" => _byteConverter.Convert(new byte[] { bytes[1] }, targetType, parameter, culture),
                "A2" => _byteConverter.Convert(new byte[] { bytes[2] }, targetType, parameter, culture),
                "A3" => _byteConverter.Convert(new byte[] { bytes[3] }, targetType, parameter, culture),
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
