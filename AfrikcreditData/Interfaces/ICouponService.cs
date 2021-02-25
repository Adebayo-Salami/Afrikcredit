using AfrikcreditData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Interfaces
{
    public interface ICouponService
    {
        bool Create(double value, long userId, out string message);
        Coupon GetCouponDetails(string couponCode, out string message);
        List<Coupon> GetAvailableCoupons();
    }
}
