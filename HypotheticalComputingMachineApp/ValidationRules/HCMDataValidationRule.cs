using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Data;
using HypotheticalComputingMachineApp.Converters;

namespace HypotheticalComputingMachineApp.ValidationRules
{
    public abstract class HCMDataValidationRule : ValidationRule
    {
        public HCMDataConverter Converter;

        protected Dictionary<DisplayingNumberStyles, (Regex Rule, string ErrorMessage)> _validationRules;

        protected const string _binaryNumberStyleValidationErrorMessage = "В строке должны присутствовать только 0 и 1!";

        public HCMDataValidationRule(HCMDataConverter converter)
        {
            Converter = converter;

            _validationRules = new Dictionary<DisplayingNumberStyles, (Regex Rule, string ErrorMessage)>();
            _validationRules.Add(DisplayingNumberStyles.Integer,
                (new Regex(@"^[-+]?\d*$", RegexOptions.Compiled), 
                 "В строке должны присутствовать только десятичные цифры, опционально - знак числа!"));
            _validationRules.Add(DisplayingNumberStyles.Hex,
                (new Regex(@"^(\d|[AaBbCcDdEeFf])*h$", RegexOptions.Compiled), 
                 "В строке должны присутствовать только десятичные цифры и латинские буквы от A до F (любого регистра)!"));

            return;
        }

        public override ValidationResult Validate(object value, CultureInfo ci)
        {
            (Regex Rule, string ErrorMessage) validationRule = _validationRules[Converter.DisplayingNumberStyle];
            if (!validationRule.Rule.IsMatch((string)value)) return new ValidationResult(false, validationRule.ErrorMessage);

            (bool Success, int Result) intermediateConverting = Converter.ConvertBackToInt32(value);
            if (!intermediateConverting.Success)
                return new ValidationResult(false, "Неправильно введёное значение! Возможно, число слишком велико по модулю.");

            var validationResult = ValidateAfterParsing(intermediateConverting.Result);

            return new ValidationResult(validationResult.IsValid, validationResult.ErrorMessage);
        }

        protected abstract (bool IsValid, string ErrorMessage) ValidateAfterParsing(int data);
    }

    public class HCMHalfByteValidationRule : HCMDataValidationRule
    {
        public HCMHalfByteValidationRule(HCMDataConverter converter)
            : base(converter)
        {
            _validationRules.Add(DisplayingNumberStyles.Binary, 
                (new Regex("^[01]{4}b$", RegexOptions.Compiled), _binaryNumberStyleValidationErrorMessage));

            return;
        }

        protected override (bool IsValid, string ErrorMessage) ValidateAfterParsing(int data)
        {
            if (data < 0)
                return (false, "Значение не может быть меньше 0!");
            if (data > 15)
                return (false, "Значение не может быть больше 15!");

            return (true, null);
        }
    }

    public class HCMByteValidationRule : HCMDataValidationRule
    {
        public HCMByteValidationRule(HCMDataConverter converter)
            : base(converter)
        {
            _validationRules.Add(DisplayingNumberStyles.Binary,
                (new Regex("^[01]{8}b$", RegexOptions.Compiled), _binaryNumberStyleValidationErrorMessage));

            return;
        }

        protected override (bool IsValid, string ErrorMessage) ValidateAfterParsing(int data)
        {
            if (data < 0)
                return (false, "Значение не может быть меньше 0!");
            if (data > 255)
                return (false, "Значение не может быть больше 255!");

            return (true, null);
        }
    }

    public class HCMDWordValidationRule : HCMDataValidationRule
    {
        public HCMDWordValidationRule(HCMDataConverter converter)
            : base(converter)
        {
            _validationRules.Add(DisplayingNumberStyles.Binary,
                (new Regex(@"^[01]{8}\s[01]{8}\s[01]{8}\s[01]{8}b$", RegexOptions.Compiled),
                 _binaryNumberStyleValidationErrorMessage));

            return;
        }

        protected override (bool IsValid, string ErrorMessage) ValidateAfterParsing(int data)
        {
            return (true, null);
        }
    }
}
