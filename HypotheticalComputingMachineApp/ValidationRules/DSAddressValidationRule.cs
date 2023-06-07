using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Globalization;

namespace HypotheticalComputingMachineApp.ValidationRules
{
    public abstract class DSAddressValidationRule : ValidationRule
    {
        private const int _minAddress = 0;

        public static int MaxAddress { set => _memoryEndAddress = value; }

        public static HCMDataValidationRule AddressGeneralValidationRule;

        protected static int _memoryStartAddress = 0;

        protected static int _memoryEndAddress;

        public abstract int AddressProtege { get; set; } 

        public DSAddressValidationRule() { return; }

        public override ValidationResult Validate(object value, CultureInfo ci)
        {
            ValidationResult validity = AddressGeneralValidationRule.Validate(value, ci);
            if (!validity.IsValid) return validity;
            int prevValue = AddressProtege;
            (_, AddressProtege) = AddressGeneralValidationRule.Converter.ConvertBackToInt32(value);
            validity = new ValidationResult(_memoryStartAddress < _memoryEndAddress, "Начальный адрес должен быть строго меньше конечного.");
            if (!validity.IsValid) AddressProtege = prevValue;
            
            return validity;
        }
    }

    public class DSStartAddressValidationRule : DSAddressValidationRule
    {
        public override int AddressProtege { get => _memoryStartAddress; set => _memoryStartAddress = value; }

        public DSStartAddressValidationRule() : base() { return; }
    }

    public class DSEndAddressValidationRule : DSAddressValidationRule
    {
        public override int AddressProtege { get => _memoryEndAddress; set => _memoryEndAddress = value; }

        public DSEndAddressValidationRule() : base() { return; }
    }
}
