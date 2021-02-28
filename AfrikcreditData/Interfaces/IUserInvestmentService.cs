using AfrikcreditData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Interfaces
{
    public interface IUserInvestmentService
    {
        bool CreateInvestment(long investmentPlanId, long userId, out string message);
        List<UserInvestment> GetAllUserInvestments(long userId);
        List<Investment> GetAllInvestments();
        List<UserInvestment> GetAllMaturedUserInvestments();
        bool SetUserInvestmentToPaid(long userInvestmentId, out string message);
        bool PlaceWithdrawal(long userInvestmentId, out string message);
        bool WithdrawToWallet(long userInvestmentId, out string message);
    }
}
