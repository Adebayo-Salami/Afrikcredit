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
    public class UserInvestmentService : IUserInvestmentService
    {
        private readonly IConfiguration _configuration;
        private readonly AfrikcreditDataContext _context;

        public UserInvestmentService(IConfiguration configuration, AfrikcreditDataContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool CreateInvestment(long investmentPlanId, long userId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (investmentPlanId <= 0)
                {
                    message = "Invalid Investment Plan ID";
                    return result;
                }

                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "Apologies, No user with this ID exists.";
                    return result;
                }

                if (user.Wallet == null)
                {
                    message = "Error, User does not have wallet account";
                    return result;
                }

                Investment investment = _context.Investments.FirstOrDefault(x => x.Id == investmentPlanId);
                if (investment == null)
                {
                    message = "Apologies, No Investment with this ID exists";
                    return result;
                }

                if (user.Wallet.Balance < investment.Cost)
                {
                    message = "Apologies, you do not have sufficient funds in your wallet balance to purchase this investment plan.";
                    return result;
                }

                user.Wallet.Balance = user.Wallet.Balance - investment.Cost;
                UserInvestment userInvestment = new UserInvestment()
                {
                    User = user,
                    Investment = investment,
                    InvestmentStatus = InvestmentStatus.Pending,
                    DateInvested = DateTime.Now
                };
                Transaction transaction = new Transaction()
                {
                    Amount = investment.Cost,
                    DateOfTransaction = DateTime.Now,
                    TransactionDescription = "Payment For Investment Plan " + investment.Name + ".",
                    TransactionType = TransactionType.Debit,
                    User = user
                };

                if (!String.IsNullOrWhiteSpace(user.ReferredBy) && !user.HasPaidReferee)
                {
                    User referredByUser = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Username == user.ReferredBy);
                    if (referredByUser != null)
                    {
                        if (referredByUser.Wallet != null)
                        {
                            double referralBonus = 0.03 * investment.Cost;
                            referredByUser.Wallet.Balance = referredByUser.Wallet.Balance + referralBonus;
                            Transaction transaction1 = new Transaction()
                            {
                                Amount = referralBonus,
                                DateOfTransaction = DateTime.Now,
                                TransactionDescription = "Referral Bonus of " + referralBonus + " added to wallet for user " + user.Username + " last investment.",
                                TransactionType = TransactionType.Credit,
                                User = referredByUser
                            };
                            _context.Transactions.Add(transaction1);
                            _context.Wallets.Update(referredByUser.Wallet);
                            user.HasPaidReferee = true;
                        }
                    }
                }

                _context.Wallets.Update(user.Wallet);
                _context.UserInvestments.Add(userInvestment);
                _context.Transactions.Add(transaction);
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

        public List<Investment> GetAllInvestments()
        {
            return _context.Investments.ToList();
        }

        public List<UserInvestment> GetAllMaturedUserInvestments()
        {
            List<UserInvestment> result = new List<UserInvestment>();

            try
            {
                List<UserInvestment> allUserInvestments = _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).Include(x => x.User.Wallet).Where(x => x.InvestmentStatus == InvestmentStatus.Pending && x.IsDeactivated == false && x.IsWithdrawing == true).OrderBy(x => x.DateWithdrawalPlaced).ToList();
                foreach (var userInvestment in allUserInvestments)
                {
                    TimeSpan timeSpan = DateTime.Now - userInvestment.DateInvested;
                    if (timeSpan.TotalDays >= userInvestment.Investment.DaysDuration)
                    {
                        result.Add(userInvestment);
                    }
                }
            }
            catch
            {
                result = new List<UserInvestment>();
            }

            return result;
            //return _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).Where(x => x.InvestmentStatus == InvestmentStatus.Pending && (DateTime.Now - x.DateInvested).TotalDays >= 8).OrderByDescending(x => x.DateInvested).ToList();
        }

        public List<UserInvestment> GetAllUserInvestments(long userId)
        {
            return _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).Include(x => x.User.Wallet).Where(x => x.User.Id == userId && x.InvestmentStatus == InvestmentStatus.Pending).ToList();
        }

        public bool PlaceWithdrawal(long userInvestmentId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userInvestmentId <= 0)
                {
                    message = "Invalid User Investment ID";
                    return result;
                }

                UserInvestment userInvestment = _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).FirstOrDefault(x => x.Id == userInvestmentId && x.InvestmentStatus == InvestmentStatus.Pending);
                if (userInvestment == null)
                {
                    message = "Error, No User Investment is associated to the passed ID.";
                    return result;
                }

                if (userInvestment.IsWithdrawing)
                {
                    message = "Withdrawal has already been placed for this current investment.";
                    return result;
                }

                if (userInvestment.IsDeactivated)
                {
                    message = "User Investment has been deactivated.";
                    return result;
                }

                TimeSpan investmentTimeSpan = DateTime.Now - userInvestment.DateInvested;
                if (investmentTimeSpan.TotalDays < userInvestment.Investment.DaysDuration)
                {
                    message = "Sorry, your investment is not matured.";
                    return result;
                }

                userInvestment.IsWithdrawing = true;
                userInvestment.DateWithdrawalPlaced = DateTime.Now;
                _context.UserInvestments.Update(userInvestment);
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

        public bool SetUserInvestmentToPaid(long userInvestmentId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userInvestmentId <= 0)
                {
                    message = "Invalid User Investment ID";
                    return result;
                }

                UserInvestment userInvestment = _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).FirstOrDefault(x => x.Id == userInvestmentId);
                if (userInvestment == null)
                {
                    message = "Apologies, no user investment with the specified ID exists.";
                    return result;
                }

                userInvestment.InvestmentStatus = InvestmentStatus.Paid;
                _context.UserInvestments.Update(userInvestment);
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

        public bool WithdrawToWallet(long userInvestmentId, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userInvestmentId <= 0)
                {
                    message = "Invalid User Investment ID";
                    return result;
                }

                UserInvestment userInvestment = _context.UserInvestments.Include(x => x.User).Include(x => x.User.Wallet).Include(x => x.Investment).FirstOrDefault(x => x.Id == userInvestmentId && x.InvestmentStatus == InvestmentStatus.Pending);
                if (userInvestment == null)
                {
                    message = "Error, No Pending User Investment is associated to the passed ID.";
                    return result;
                }

                //if (userInvestment.IsWithdrawing)
                //{
                //    message = "Withdrawal has already been placed for this current investment.";
                //    return result;
                //}

                if (userInvestment.IsDeactivated)
                {
                    message = "User Investment has been deactivated.";
                    return result;
                }

                TimeSpan investmentTimeSpan = DateTime.Now - userInvestment.DateInvested;
                if (investmentTimeSpan.TotalDays < userInvestment.Investment.DaysDuration)
                {
                    message = "Sorry, your investment is not matured.";
                    return result;
                }

                if(userInvestment.User.Wallet == null)
                {
                    message = "Error, No wallet is linked to user account.";
                    return result;
                }

                userInvestment.User.Wallet.Balance = userInvestment.User.Wallet.Balance + userInvestment.Investment.AmountToBeGotten;
                userInvestment.InvestmentStatus = InvestmentStatus.Paid;
                userInvestment.DateWithdrawalPlaced = DateTime.Now;
                _context.UserInvestments.Update(userInvestment);
                _context.Wallets.Update(userInvestment.User.Wallet);
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
    }
}
