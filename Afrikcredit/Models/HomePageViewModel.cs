using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Models
{
    public class HomePageViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string ReTypePassword { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string DisplayMessage { get; set; }
        public string ReferralCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
