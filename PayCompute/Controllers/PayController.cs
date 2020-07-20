using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayCompute.Services;

namespace PayCompute.Controllers
{
    public class PayController : Controller
    {
        private readonly IPayComputationService _payComputationService;

        public PayController(IPayComputationService payComputationService)
        {
            _payComputationService = payComputationService;
        }
    }
}
