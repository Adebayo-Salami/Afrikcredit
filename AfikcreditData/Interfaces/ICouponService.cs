using AfikcreditData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfikcreditData.Interfaces
{
    public interface ICouponService
    {
        bool Create(double value, long userId, out string message);
        Coupon GetCouponDetails(string couponCode, out string message);
        List<Coupon> GetAvailableCoupons();
    }
}
