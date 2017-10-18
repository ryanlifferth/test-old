using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ryan.CardReader.ValidationRules
{
    public class RequiredValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value == null)
            {
                return new ValidationResult(false, "Value cannot be empty.");
            }

            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return new ValidationResult(false, "Value cannot be empty.");
                }
            }

            return ValidationResult.ValidResult;

        }
    }
}
