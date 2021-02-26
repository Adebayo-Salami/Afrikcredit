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

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(viewModel);
        }

        public IActionResult Admin(string _couponCode = "", string _couponCreator = "", string _couponDateCreated = "", string _couponDateUsed = "", string _couponUser = "", double _couponValue = 0, bool _couponUserDeactivated = false)
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
                AvailableCoupons = _couponService.GetAvailableCoupons(),
                AllMaturedUserInvestments = _userInvestmentService.GetAllMaturedUserInvestments(),
                TotalUsersRegisteredOnPlatform = _userService.GetTotalUserRegistered(),
                TotalUsersWithActiveInvestment = _userService.GetTotalUsersEngagedInInvestment(),
            };

            if (!String.IsNullOrWhiteSpace(_couponCode))
            {
                viewModel.CouponCode = _couponCode;
                viewModel.CouponCreator = _couponCreator;
                viewModel.CouponDateCreated = _couponDateCreated;
                viewModel.CouponDateUsed = _couponDateUsed;
                viewModel.CouponUser = _couponUser;
                viewModel.CouponValue = _couponValue;
                viewModel.IsCouponUserDeactivated = _couponUserDeactivated;
            }

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult UpdateProfile(string password = "", string phone = "", string address = "", string bank = "", string acct = "")
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

            bool IsUpdated = _userService.UpdateUserInfo(loggedUser.Id, password, phone, address, bank, acct, out string message);
            if (!IsUpdated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Profile", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "Update Successful.");
            return RedirectToAction("Profile", "Dashboard");
        }

        [HttpGet]
        public IActionResult GenerateCoupon(string amount)
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

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Your not authorized to view that page.");
                return RedirectToAction("Index", "Dashboard");
            }

            double amt = 0;
            try
            {
                amt = Convert.ToDouble(amount);
                if (amt <= 0)
                {
                    throw new Exception("Err");
                }
            }
            catch
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly input a valid amount.");
                return RedirectToAction("Admin", "Dashboard");
            }

            bool IsGenerated = _couponService.Create(amt, loggedUser.Id, out string message);
            if (!IsGenerated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Admin", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "Your coupon has been generated successfully.");
            return RedirectToAction("Admin", "Dashboard");
        }

        [HttpGet]
        public IActionResult CheckCouponStatus(string couponCode)
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

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Your not authorized to view that page.");
                return RedirectToAction("Index", "Dashboard");
            }

            Coupon couponDetails = _couponService.GetCouponDetails(couponCode, out string message);
            if (couponDetails == null)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Admin", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "Your coupon data has been fetched successfully.");
            return RedirectToAction("Admin", "Dashboard", new { _couponCode = couponDetails.Code, _couponCreator = (couponDetails.CreatedBy == null) ? "None" : couponDetails.CreatedBy.Username, _couponDateCreated = couponDetails.DateCreated.ToString(), _couponDateUsed = ((DateTime.Now - couponDetails.DateUsed).TotalDays > 1000) ? "Not Used" : couponDetails.DateUsed.ToString(), _couponUser = (couponDetails.UsedBy == null) ? "None" : couponDetails.UsedBy.Username, _couponValue = couponDetails.Value, _couponUserDeactivated = (couponDetails.UsedBy == null) ? false : couponDetails.UsedBy.IsDeactivated });
        }

        public IActionResult ReactivateCouponUser(string userEmail)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly login. Your session has expired.");
                return RedirectToAction("Index", "Home");
            }

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Your not authorized to view that page.");
                return RedirectToAction("Index", "Dashboard");
            }

            bool IsReactivated = _userService.ReActivateUser(userEmail, out string message);
            if (!IsReactivated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Admin", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "User with username " + userEmail + " has been reactivated successfully.");
            return RedirectToAction("Admin", "Dashboard");
        }

        public IActionResult DeactivateCouponUser(string userEmail)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly login. Your session has expired.");
                return RedirectToAction("Index", "Home");
            }

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Your not authorized to view that page.");
                return RedirectToAction("Index", "Dashboard");
            }

            bool IsDeactivated = _userService.DeactivateUser(userEmail, out string message);
            if (!IsDeactivated)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Admin", "Dashboard");
            }

            HttpContext.Session.SetString("DisplayMessage", "User with username " + userEmail + " has been deactivated successfully.");
            return RedirectToAction("Admin", "Dashboard");
        }

        public IActionResult SetUserInvestmentToPaid(long userInvestmentId)
        {
            //Check Authentication
            string Id = HttpContext.Session.GetString("UserID");
            string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

            bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(Id), authenticationToken, out User loggedUser);
            if (!userLogged)
            {
                HttpContext.Session.SetString("DisplayMessage", "Please, Kindly login. Your session has expired.");
                return RedirectToAction("Index", "Home");
            }

            if (!loggedUser.isAdmin)
            {
                HttpContext.Session.SetString("DisplayMessage", "Your not authorized to view that page.");
                return RedirectToAction("Index", "Dashboard");
            }

            if (userInvestmentId <= 0)
            {
                HttpContext.Session.SetString("DisplayMessage", "Invalid User Investment ID.");
            }
            else
            {
                bool isUserPaid = _userInvestmentService.SetUserInvestmentToPaid(userInvestmentId, out string message);
                if (isUserPaid)
                {
                    HttpContext.Session.SetString("DisplayMessage", "User updated to paid.");
                }
                else
                {
                    HttpContext.Session.SetString("DisplayMessage", message);
                }
            }

            return RedirectToAction("Admin", "Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
