using System;
using System.Collections.Generic;
using System.Text;

namespace PayCompute.Services.Implementation
{
    public class NationalInsuranceContributionService : INationalInsuranceContributionService
    {
        private decimal _niRate;
        private decimal _nic;
        public decimal NIContribution(decimal totalAmount)
        {
            if (totalAmount < 719)
            {
                //Lower Earning Limit Rate & below Primary Threshold
                _niRate = .0m;
                _nic = 0m;
            }
            else if (totalAmount >= 719 && totalAmount <= 4167)
            {//Between Primary Threshold and Upper Earning Limits
                _niRate = .12m;
                _nic = (totalAmount - 719) * _niRate;
            }
            else if (totalAmount > 4167)
            {//Above Upper Earning Limits
                _niRate = .02m;
                _niRate = ((4167 - 719) * -12m) + ((totalAmount - 4167) * _niRate);
            }

            return _nic;
        }
    }
}
