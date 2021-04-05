using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Models
{
    public class UserInvestment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Investment Investment { get; set; }
        public DateTime DateInvested { get; set; }
        public InvestmentStatus InvestmentStatus { get; set; }
        public bool IsDeactivated { get; set; }
        public DateTime DeactivationDate { get; set; }
        public bool IsWithdrawing { get; set; }
        public DateTime DateWithdrawalPlaced { get; set; }
        public double AmountToWithdraw { get; set; }
    }
}
