using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using PayCompute.Entity;
using PayCompute.Persistence;

namespace PayCompute.Services.Implementation
{
    public class PayComputationService : IPayComputationService
    {
        private decimal contractualEarnings;
        private decimal overtimeHours;
        private readonly ApplicationDbContext _context;

        public PayComputationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(PaymentRecord paymentRecord)
        {
            await _context.PaymentRecords.AddAsync(paymentRecord);
            await _context.SaveChangesAsync();
        }

        public PaymentRecord GetById(int id) =>
            _context
                .PaymentRecords
                .SingleOrDefault(pay =>
                    pay.Id == id);

        public IEnumerable<PaymentRecord> GetAll() => _context.PaymentRecords.OrderBy(p => p.EmployeeId);

        public IEnumerable<SelectListItem> GetAllTaxYear()
        {
            var allTaxYear = _context.TaxYears
                .Select(taxYear => new SelectListItem
                {
                    Text = taxYear.YearOfTax,
                    Value = taxYear.Id.ToString()
                });
            return allTaxYear;
        }

        public decimal OvertimeHours(decimal hoursWorked, decimal contractualHours)
        {
            if (hoursWorked <= contractualHours)
            {
                overtimeHours = 0.00m;
            }
            else if (hoursWorked > contractualHours)
            {
                overtimeHours = hoursWorked - contractualHours;
            }

            return overtimeHours;

        }

        public decimal ContractualEarnings(decimal contractualHours, decimal hoursWorked, decimal hourlyRate)
        {
            if (hoursWorked < contractualHours)
            {
                contractualEarnings = hoursWorked * hourlyRate;
            }
            else
            {
                contractualEarnings = contractualHours * hourlyRate;
            }

            return contractualEarnings;
        }

        public decimal OvertimeRate(decimal hourlyRate) => hourlyRate*1.5

        public decimal OvertimeEarnings(decimal overtimeRate, decimal overtimeHours) =>
            overtimeHours * overtimeRate;

        public decimal TotalEarnings(decimal overtimeEarnings, decimal contractualEarnings)
        {
            throw new NotImplementedException();
        }

        public decimal TotalDeduction(decimal tax, decimal nic, decimal studentLoanRepayment, decimal unionFees)
        {
            throw new NotImplementedException();
        }

        public decimal NetPay(decimal totalEarnings, decimal totalDeduction) =>
            totalEarnings - totalDeduction;
    }
}
