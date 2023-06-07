using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Globalization;

namespace HypotheticalComputingMachineApp.ValidationRules
{
    public class HCMFlagRegisterValidationRule : ValidationRule
    {
        private Regex flagsRule;

        public HCMFlagRegisterValidationRule(int flagsCount)
        {
            flagsRule = new Regex("^[01]{" + flagsCount + "}$", RegexOptions.Compiled);

            return;
        }

        public override ValidationResult Validate(object value, CultureInfo ci)
        {
            if (!flagsRule.IsMatch((string)value)) return new ValidationResult(false, "Регистр флагов принимает только 0 и 1.");

            return ValidationResult.ValidResult;
        }
    }
}
