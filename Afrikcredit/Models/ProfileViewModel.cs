using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public List<AfrikcreditData.Models.Notification> Notifications { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Address { get; set; }
    }
}
