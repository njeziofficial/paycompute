using System;
using System.Collections.Generic;
using System.Text;

namespace PayCompute.Services.Implementation
{
    public class TaxService : ITaxService
    {
        private decimal _taxRate;
        private decimal _tax;

        private static decimal CalculateValue() => 1042 * .0m;
        public decimal TaxAmount(decimal totalAmount)
        {
            if (totalAmount <= 1042)
            {
                //Tax Free Rate
                _taxRate = .0m;
                _tax = totalAmount * _taxRate;
            }
            else if (totalAmount > 1042 && totalAmount <= 3125)
            {
                //Basic Tax Rate (20%)
                _taxRate = .2m;
                //Income Tax
                _tax = CalculateValue() + ((totalAmount - 1042) * _taxRate);
            }
            else if (totalAmount > 3125 && totalAmount <= 12500)
            {
                //Higher Tax Rate (40%)
                _taxRate = .40m;
                //Income Task
                _tax = CalculateValue() + ((3125 - 1042) * .20m) + ((totalAmount - 3125) * _taxRate);
            }
            else if (totalAmount > 12500)
            {
                //Additional Tax Rate (45%)
                _taxRate = .45m;
                //Income Tax
                _tax = CalculateValue() + ((3125 - 1042) * .20m) + ((12500 - 3125) * .40m) +
                       ((totalAmount - 12500) * _taxRate);
            }

            return _tax;
        }
    }
}
