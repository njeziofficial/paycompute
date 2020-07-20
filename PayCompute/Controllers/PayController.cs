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
        private decimal overtimeHrs;
        private decimal contractualEarnings;
        private decimal overtimeEarnings;
        private decimal totalEarnings;
        private int employeeId;

        public PayController(IPayComputationService payComputationService,
            IEmployeeService employeeService,
            ITaxService taxService)
        {
            _payComputationService = payComputationService;
            _employeeService = employeeService;
            _taxService = taxService;
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
        public IActionResult Create(PaymentRecordCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var payRecord = new PaymentRecord
                {
                    Id = model.Id,
                    EmployeeId=employeeId = model.EmployeeId,
                    FullName = _employeeService.GetById(model.EmployeeId).FullName,
                    NiNo = _employeeService.GetById(model.EmployeeId).NationalInsuranceNo,
                    PayDate = model.PayDate,
                    PayMonth = model.PayMonth,
                    TaxYearId = model.TaxYearId,
                    TaxCode = model.TaxCode,
                    HourlyRate = model.HourlyRate,
                    HoursWorked = model.HoursWorked,
                    ContractualHours = contractualEarnings = model.ContractualHours,
                    OvertimeHours = overtimeHrs = _payComputationService.OvertimeHours(model.HoursWorked, model.ContractualHours),
                    ContractualEarnings = _payComputationService.ContractualEarnings(model.ContractualHours, model.HoursWorked, model.HourlyRate),
                    OvertimeEarnings = overtimeEarnings = _payComputationService.OvertimeEarnings(_payComputationService.OvertimeRate(model.HourlyRate), overtimeHrs),
                    TotalEarnings=totalEarnings = _payComputationService.TotalEarnings(overtimeEarnings, contractualEarnings),
                    Tax =_taxService.TaxAmount(totalEarnings),
                    UnionFee = _employeeService.UnionFees(employeeId),
                    SLC = _employeeService.StudentLoanRepaymentAmount(employeeId, totalEarnings),
                    
                };
            }
            return View();
        }
    }
}
