﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayCompute.Entity;
using PayCompute.Models;
using PayCompute.Services;

namespace PayCompute.Controllers
{
    public class PayController : Controller
    {
        private readonly IPayComputationService _payComputationService;
        private readonly IEmployeeService _employeeService;
        private readonly ITaxService _taxService;
        private readonly INationalInsuranceContributionService _nationalInsuranceContributionService;
        private decimal _overtimeHrs;
        private decimal _contractualEarnings;
        private decimal _overtimeEarnings;
        private decimal _totalEarnings;
        private int _employeeId;
        private decimal _tax;
        private decimal _nationalInsurance;
        private decimal _unionFee;
        private decimal _studentLoan;
        private decimal _totalDeduction;
        private int _taxYearId;
        private decimal _hourlyRate;

        public PayController(IPayComputationService payComputationService,
            IEmployeeService employeeService,
            ITaxService taxService,
            INationalInsuranceContributionService nationalInsuranceContributionService)
        {
            _payComputationService = payComputationService;
            _employeeService = employeeService;
            _taxService = taxService;
            _nationalInsuranceContributionService = nationalInsuranceContributionService;
        }

        public IActionResult Index()
        {
            var payRecords = _payComputationService
                .GetAll()
                .Select(pay => new PaymentRecordIndexViewModel
                {
                    Id = pay.Id,
                    EmployeeId = pay.EmployeeId,
                    FullName = pay.FullName,
                    PayDate = pay.PayDate,
                    PayMonth = pay.PayMonth,
                    TaxYearId = pay.TaxYearId,
                    Year = _payComputationService.GetTaxYearById(pay.TaxYearId).YearOfTax,
                    TotalEarnings = pay.TotalEarnings,
                    TotalDeduction = pay.TotalDeduction,
                    NetPayment = pay.NetPayment,
                    Employee = pay.Employee
                });
            return View(payRecords);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.employees = _employeeService.GetAllEmployeesForPayroll();
            ViewBag.taxYears = _payComputationService.GetAllTaxYear();
            var model = new PaymentRecordCreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentRecordCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var payRecord = new PaymentRecord
                {
                    Id = model.Id,
                    EmployeeId = _employeeId = model.EmployeeId,
                    FullName = _employeeService.GetById(model.EmployeeId).FullName,
                    NiNo = _employeeService.GetById(model.EmployeeId).NationalInsuranceNo,
                    PayDate = model.PayDate,
                    PayMonth = model.PayMonth,
                    TaxYearId = model.TaxYearId,
                    TaxCode = model.TaxCode,
                    HourlyRate = model.HourlyRate,
                    HoursWorked = model.HoursWorked,
                    ContractualHours = _contractualEarnings = model.ContractualHours,
                    OvertimeHours = _overtimeHrs = _payComputationService.OvertimeHours(model.HoursWorked, model.ContractualHours),
                    ContractualEarnings = _payComputationService.ContractualEarnings(model.ContractualHours, model.HoursWorked, model.HourlyRate),
                    OvertimeEarnings = _overtimeEarnings = _payComputationService.OvertimeEarnings(_payComputationService.OvertimeRate(model.HourlyRate), _overtimeHrs),
                    TotalEarnings = _totalEarnings = _payComputationService.TotalEarnings(_overtimeEarnings, _contractualEarnings),
                    Tax = _tax = _taxService.TaxAmount(_totalEarnings),
                    UnionFee = _unionFee = _employeeService.UnionFees(_employeeId),
                    SLC = _studentLoan = _employeeService.StudentLoanRepaymentAmount(_employeeId, _totalEarnings),
                    NIC = _nationalInsurance = _nationalInsuranceContributionService.NIContribution(_totalEarnings),
                    TotalDeduction = _totalDeduction = _payComputationService.TotalDeduction(_tax, _nationalInsurance, _studentLoan, _unionFee),
                    NetPayment = _payComputationService.NetPay(_totalEarnings, _totalDeduction)
                };
                await _payComputationService.CreateAsync(payRecord);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.employees = _employeeService.GetAllEmployeesForPayroll();
            ViewBag.taxYears = _payComputationService.GetAllTaxYear();
            return View();
        }

        public IActionResult Detail(int id)
        {
            var paymentRecord = _payComputationService.GetById(id);
            if (paymentRecord == null)
            {
                return NotFound();
            }

            var model = new PaymentRecordDetailViewModel
            {
                Id = paymentRecord.Id,
                EmployeeId = paymentRecord.EmployeeId,
                FullName = paymentRecord.FullName,
                NiNo = paymentRecord.NiNo,
                PayDate = paymentRecord.PayDate,
                PayMonth = paymentRecord.PayMonth,
                TaxYearId = _taxYearId = paymentRecord.TaxYearId,
                Year = _payComputationService.GetTaxYearById(_taxYearId).YearOfTax,
                TaxCode = paymentRecord.TaxCode,
                HourlyRate = _hourlyRate = paymentRecord.HourlyRate,
                HoursWorked = paymentRecord.HoursWorked,
                ContractualHours = paymentRecord.ContractualHours,
                OvertimeHours = paymentRecord.OvertimeHours,
                OvertimeRate = _payComputationService.OvertimeRate(_hourlyRate),
                ContractualEarnings = paymentRecord.ContractualEarnings,
                OvertimeEarnings = paymentRecord.OvertimeEarnings,
                Tax = paymentRecord.Tax,
                NIC = paymentRecord.NIC,
                UnionFee = paymentRecord.UnionFee,
                SLC = paymentRecord.SLC,
                TotalEarnings = paymentRecord.TotalEarnings,
                TotalDeduction = paymentRecord.TotalDeduction,
                Employee = paymentRecord.Employee,
                TaxYear = paymentRecord.TaxYear,
                NetPayment = paymentRecord.NetPayment
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Payslip(int id)
        {
            var paymentRecord = _payComputationService.GetById(id);
            if (paymentRecord == null)
            {
                return NotFound();
            }

            var model = new PaymentRecordDetailViewModel
            {
                Id = paymentRecord.Id,
                EmployeeId = paymentRecord.EmployeeId,
                FullName = paymentRecord.FullName,
                NiNo = paymentRecord.NiNo,
                PayDate = paymentRecord.PayDate,
                PayMonth = paymentRecord.PayMonth,
                TaxYearId = _taxYearId = paymentRecord.TaxYearId,
                Year = _payComputationService.GetTaxYearById(_taxYearId).YearOfTax,
                TaxCode = paymentRecord.TaxCode,
                HourlyRate = _hourlyRate = paymentRecord.HourlyRate,
                HoursWorked = paymentRecord.HoursWorked,
                ContractualHours = paymentRecord.ContractualHours,
                OvertimeHours = paymentRecord.OvertimeHours,
                OvertimeRate = _payComputationService.OvertimeRate(_hourlyRate),
                ContractualEarnings = paymentRecord.ContractualEarnings,
                OvertimeEarnings = paymentRecord.OvertimeEarnings,
                Tax = paymentRecord.Tax,
                NIC = paymentRecord.NIC,
                UnionFee = paymentRecord.UnionFee,
                SLC = paymentRecord.SLC,
                TotalEarnings = paymentRecord.TotalEarnings,
                TotalDeduction = paymentRecord.TotalDeduction,
                Employee = paymentRecord.Employee,
                TaxYear = paymentRecord.TaxYear,
                NetPayment = paymentRecord.NetPayment
            };
            return View(model);
        }
    }
}
