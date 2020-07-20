using System;
using System.Collections.Generic;
using System.Text;

namespace PayCompute.Services.Implementation
{
   public class NationalInsuranceContributionService : INationalInsuranceContributionService
   {
       private decimal NIRate;
       private decimal NIC;
        public decimal NIContribution(decimal totalAmount)
        {
            if (totalAmount < 719)
            {
                //Lower Earning 
            }
        }
    }
}
