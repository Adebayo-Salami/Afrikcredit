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
    public class WalletService : IWalletService
    {
        private readonly IConfiguration _configuration;
        private readonly AfrikcreditDataContext _context;

        public WalletService(IConfiguration configuration, AfrikcreditDataContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool FundWallet(long userId, string couponCode, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(couponCode))
                {
                    message = "Coupon code must not be null or empty";
                    return result;
                }

                //Check Coupon Code
                Coupon coupon = _context.Coupons.Include(x => x.CreatedBy).Include(x => x.UsedBy).FirstOrDefault(x => x.Code == couponCode);
                if (coupon == null)
                {
                    message = "Apologies, Coupon does not exists.";
                    return result;
                }

                if (coupon.UsedBy != null)
                {
                    message = "Apologies, This Coupon code has already been used.";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "Apologies, No user with this ID exists, kindly contact your admin.";
                    return result;
                }

                if (user.Wallet == null)
                {
                    message = "Error, this user does not have a wallet account.";
                    return result;
                }

                user.Wallet.Balance = user.Wallet.Balance + coupon.Value;
                coupon.UsedBy = user;
                _context.Wallets.Update(user.Wallet);
                _context.Coupons.Update(coupon);

                Transaction transaction = new Transaction()
                {
                    User = user,
                    Amount = coupon.Value,
                    DateOfTransaction = DateTime.Now,
                    TransactionType = TransactionType.Credit,
                    TransactionDescription = "Funding wallet account with coupon code " + coupon.Code + " (" + coupon.Value + ")."
                };
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

        public bool TransferFunds(long userId, double amount, string receiverEmailAddress, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                if (amount < 100)
                {
                    message = "Invalid Transfer Amount, The minimum amount that can be transferred is 100 Naira.";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(receiverEmailAddress))
                {
                    message = "Receiver Username (Username of the user sending money to, Kindly ensure that its the user account username) is required.";
                    return result;
                }

                User sender = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (sender == null)
                {
                    message = "Error, The specified ID is not linked to any user account.";
                    return result;
                }

                if (sender.Wallet == null)
                {
                    message = "Error, Sender User account does not have a wallet linked to it";
                    return result;
                }

                if (sender.Wallet.Balance < amount)
                {
                    message = "Insufficient funds in user account to transfer " + amount + " Naira. Kindly fund account and try again.";
                    return result;
                }

                if (sender.Username == receiverEmailAddress)
                {
                    message = "Apologies, your not allowed to transfer to oneself's account.";
                    return result;
                }

                User receiver = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Username == receiverEmailAddress);
                if (receiver == null)
                {
                    message = "Apologies, No user account with the specified username (" + receiverEmailAddress + ") exists on the platform.";
                    return result;
                }

                if (receiver.Wallet == null)
                {
                    message = "Error, Receiver User account does not have a wallet linked to it";
                    return result;
                }

                sender.Wallet.Balance = sender.Wallet.Balance - amount;
                receiver.Wallet.Balance = receiver.Wallet.Balance + amount;
                Transaction senderTransaction = new Transaction()
                {
                    Amount = amount,
                    DateOfTransaction = DateTime.Now,
                    TransactionDescription = amount + " naira was transferred from your account to " + receiver.Username + ".",
                    TransactionType = TransactionType.Debit,
                    User = sender,
                };
                Transaction receiverTransaction = new Transaction()
                {
                    Amount = amount,
                    DateOfTransaction = DateTime.Now,
                    TransactionDescription = amount + " naira was transferred to your account to " + sender.Username + ".",
                    TransactionType = TransactionType.Credit,
                    User = receiver,
                };

                _context.Wallets.Update(sender.Wallet);
                _context.Wallets.Update(receiver.Wallet);
                _context.Transactions.Add(senderTransaction);
                _context.Transactions.Add(receiverTransaction);
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

        public bool UpdateWalletInfo(long userId, string bankName, string accountNumber, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(bankName))
                {
                    message = "Bank Name Is Required when updating wallet info.";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(accountNumber))
                {
                    message = "Account Number Is Required when updating wallet info.";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "Error, No user account is linked to the ID specified.";
                    return result;
                }

                if (user.Wallet == null)
                {
                    message = "Error, User account does not have a wallet linked to it";
                    return result;
                }

                user.Wallet.AccountNumber = accountNumber;
                user.Wallet.BankName = bankName;
                _context.Wallets.Update(user.Wallet);
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
