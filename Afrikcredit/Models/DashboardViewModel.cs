using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Models
{
    public class DashboardViewModel
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public List<AfrikcreditData.Models.Notification> Notifications { get; set; }
        public string PhoneNumber { get; set; }
        public double Balance { get; set; }
        public List<AfrikcreditData.Models.UserInvestment> UserInvestments { get; set; }
        public List<AfrikcreditData.Models.Investment> AvailableInvestments { get; set; }
        public string CouponCode { get; set; }
    }
}
