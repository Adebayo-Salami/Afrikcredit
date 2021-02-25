using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AfrikcreditData.Models;
using AfrikcreditData.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using AfrikcreditData;

namespace AfrikcreditServices
{
    public class CouponService : ICouponService
    {
        private readonly IConfiguration _configuration;
        private readonly AfrikcreditDataContext _context;

        public CouponService(IConfiguration configuration, AfrikcreditDataContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool Create(double value, long userId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (value <= 0)
                {
                    message = "Invalid Coupon Value, Coupon value must be greater than zero";
                    return result;
                }

                if (userId <= 0)
                {
                    message = "Invalid User ID, Kindly Contact your admin.";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "No User with the passed ID exists.";
                    return result;
                }

                if (!user.isAdmin)
                {
                    message = "Apologies, only admin users can create a coupon code";
                    return result;
                }

                //Generate Unique Coupon Code
                string uniqueCouponCode = GenerateUniqueCouponCode();
                Coupon coupon = new Coupon()
                {
                    Code = uniqueCouponCode,
                    CreatedBy = user,
                    DateCreated = DateTime.Now,
                    Value = value
                };

                _context.Coupons.Add(coupon);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception error)
            {
                message = error.Message;
                result = false;
            }

            return result;
        }

        private string GenerateUniqueCouponCode()
        {
            string code;
            do
            {
                code = Random("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            }
            while (_context.Coupons.FirstOrDefault(x => x.Code == code) != null);

            return code;
        }

        public string Random(string chars, int length = 5)
        {
            var randomString = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
                randomString.Append(chars[random.Next(chars.Length)]);

            return randomString.ToString();
        }

        public Coupon GetCouponDetails(string couponCode, out string message)
        {
            Coupon result = null;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(couponCode))
                {
                    message = "Error, Coupon Code Is Required.";
                    return result;
                }

                result = _context.Coupons.Include(x => x.UsedBy).Include(x => x.CreatedBy).FirstOrDefault(x => x.Code == couponCode);
                if (result == null) message = "No Coupon with the specified code exists";
            }
            catch (Exception error)
            {
                message = error.Message;
                result = null;
            }

            return result;
        }

        public List<Coupon> GetAvailableCoupons()
        {
            return _context.Coupons.Include(x => x.CreatedBy).Include(x => x.UsedBy).Where(x => x.UsedBy == null).ToList();
        }
    }
}
