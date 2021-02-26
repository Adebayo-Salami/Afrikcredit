using Afrikcredit.Models;
using AfrikcreditData.Interfaces;
using AfrikcreditData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IUserService _userService;
        private readonly IUserInvestmentService _userInvestmentService;
        private readonly IWalletService _walletService;
        private readonly ICouponService _couponService;
        private readonly INotificationService _notificationService;
        public DashboardController(ILogger<DashboardController> logger, IUserService userService, IUserInvestmentService userInvestmentService, IWalletService walletService, ICouponService couponService, INotificationService notificationService)
        {
            _logger = logger;
            _userService = userService;
            _userInvestmentService = userInvestmentService;
            _walletService = walletService;
            _couponService = couponService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");

            //Check Authentication
            string userId = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly login. Your session has expired.");
                return RedirectToAction("Index", "Home");
            }

            DashboardViewModel viewModel = new DashboardViewModel()
            {
                DisplayMessage = _displayMessage,
                Username = loggedUser.Username,
                IsAdmin = loggedUser.isAdmin,
            };

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(viewModel);
        }
    }
}
