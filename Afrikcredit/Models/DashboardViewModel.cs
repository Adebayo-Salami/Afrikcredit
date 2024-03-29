﻿using System;
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
        public decimal AmountGained(double amtToBeGotten, int investmentPercentage, bool IsWithdrawing = false, double amountWithdrawing = 0)
        {
            if (IsWithdrawing)
            {
                return Convert.ToDecimal(amountWithdrawing);
            }
            else
            {
                return Convert.ToDecimal(investmentPercentage) / 100m * (decimal)amtToBeGotten;
            }
        }
        public int GetInvestmentMaturityPercentage(DateTime dateOfInvestment, int investmentDuration = 0)
        {
            int result = 0;

            try
            {
                TimeSpan timeSpan = DateTime.Now - dateOfInvestment;
                if (timeSpan.TotalDays > investmentDuration) return 100;
                double durationPercentage = (timeSpan.TotalDays / investmentDuration) * 100;
                result = (int)durationPercentage;
            }
            catch { }

            return result;
        }
        public string Amount { get; set; }
        public string ReceipentUsername { get; set; }
    }
}
