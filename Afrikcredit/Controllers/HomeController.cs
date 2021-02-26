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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index(string username = "", string password = "")
        {
            string _displayMessage = HttpContext.Session.GetString("DisplayMessage");

            HomePageViewModel viewModel = new HomePageViewModel()
            {
                DisplayMessage = _displayMessage,
            };

            HttpContext.Session.SetString("DisplayMessage", String.Empty);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Login(HomePageViewModel data)
        {
            if (String.IsNullOrWhiteSpace(data.Username))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input your username.");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.Password))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input your password");
                return RedirectToAction("Index", "Home");
            }

            User loggedInUser = _userService.Login(data.Username, data.Password, out string message);
            if (loggedInUser == null)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Index", "Home");
            }

            //Setting Authentication Session
            HttpContext.Session.SetString("AuthorizationToken", loggedInUser.AuthenticationToken);
            HttpContext.Session.SetString("UserID", loggedInUser.Id.ToString());

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult Register(HomePageViewModel data)
        {
            if (String.IsNullOrWhiteSpace(data.Username))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input username");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.Password))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input password");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.BankName))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input bank name");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.AccountNumber))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input account number");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.ReTypePassword))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly retype password");
                return RedirectToAction("Index", "Home");
            }

            if (String.IsNullOrWhiteSpace(data.PhoneNumber))
            {
                HttpContext.Session.SetString("DisplayMessage", "Kindly input your phone numnber");
                return RedirectToAction("Index", "Home");
            }

            if (data.Password != data.ReTypePassword)
            {
                HttpContext.Session.SetString("DisplayMessage", "Passwords do not match.");
                return RedirectToAction("Index", "Home");
            }

            bool isRegistered = _userService.Register(data.Username, data.Password, data.AccountNumber, data.BankName, data.ReferralCode, data.PhoneNumber, out string message);
            if (!isRegistered)
            {
                HttpContext.Session.SetString("DisplayMessage", message);
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("DisplayMessage", "Registration Successful!, Kindly Login Now.");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            try
            {
                //Check Authentication
                string userId = HttpContext.Session.GetString("UserID");
                string authenticationToken = HttpContext.Session.GetString("AuthorizationToken");

                bool userLogged = _userService.CheckUserAuthentication(Convert.ToInt64(userId), authenticationToken, out User loggedUser);
                if (userLogged)
                {
                    if (!_userService.LogoutUser(loggedUser, out string message))
                    {
                        throw new Exception(message);
                    }
                }

            }
            catch (Exception err)
            {
                _logger.LogError("An error occurred at Logout " + err);
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
