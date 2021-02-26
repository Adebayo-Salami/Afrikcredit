using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Models
{
    public class AdminViewModel
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public List<AfrikcreditData.Models.Notification> Notifications { get; set; }
        public string PhoneNumber { get; set; }
        public double CouponValue { get; set; }
        public string CouponCode { get; set; }
        public string CouponDateCreated { get; set; }
        public string CouponDateUsed { get; set; }
        public string CouponCreator { get; set; }
        public string CouponUser { get; set; }
        public List<AfrikcreditData.Models.Coupon> AvailableCoupons { get; set; }
        public List<AfrikcreditData.Models.UserInvestment> AllMaturedUserInvestments { get; set; }
        public int TotalUsersRegisteredOnPlatform { get; set; }
        public int TotalUsersWithActiveInvestment { get; set; }
        public bool IsCouponUserDeactivated { get; set; }
        public string notificationMessage { get; set; }
    }
}
