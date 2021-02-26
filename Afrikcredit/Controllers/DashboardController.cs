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
                PhoneNumber = loggedUser.PhoneNumber,
                Notifications = _notificationService.GetAll(),
            };

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(viewModel);
        }

        public IActionResult Profile()
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

            ProfileViewModel viewModel = new ProfileViewModel()
            {
                DisplayMessage = _displayMessage,
                Username = loggedUser.Username,
                IsAdmin = loggedUser.isAdmin,
                PhoneNumber = loggedUser.PhoneNumber,
                Notifications = _notificationService.GetAll(),
                Password = loggedUser.Password,
                BankName = (loggedUser.Wallet == null) ? "None" : loggedUser.Wallet.BankName,
                AccountNumber = (loggedUser.Wallet == null) ? "None" : loggedUser.Wallet.AccountNumber,
                Address = loggedUser.Address,
            };

            return View(viewModel);
        }

        public IActionResult Admin()
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

            AdminViewModel viewModel = new AdminViewModel()
            {
                DisplayMessage = _displayMessage,
                Username = loggedUser.Username,
                IsAdmin = loggedUser.isAdmin,
                PhoneNumber = loggedUser.PhoneNumber,
                Notifications = _notificationService.GetAll(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileViewModel data)
        {
            //Check Authentication
            string userId = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");
            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly login. Your session has expired.");
                return RedirectToAction("Index", "Home");
            }

            bool IsUpdated = _userService.UpdateUserInfo(loggedUser.Id, data.Password, data.PhoneNumber, data.Address, data.BankName, data.AccountNumber, out string message);
            if (!IsUpdated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Profile", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "Update Successful.");
            return RedirectToAction("Profile", "Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
