using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AfikcreditData.Models;
using AfikcreditData.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using AfikcreditData;

namespace AfrikcreditServices
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly AfrikcreditDataContext _context;

        public UserService(IConfiguration configuration, AfrikcreditDataContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public bool CheckUserAuthentication(long userId, string authenticationToken, out User loggedUser)
        {
            loggedUser = null;
            bool result = false;

            try
            {
                if (userId <= 0)
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(authenticationToken))
                {
                    return false;
                }

                loggedUser = _context.Users.Include(x => x.Wallet).FirstOrDefault(user => user.Id == userId && user.AuthenticationToken == authenticationToken);
                if (loggedUser == null)
                {
                    return false;
                }

                if (loggedUser.IsDeactivated)
                {
                    return false;
                }

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public User GetUserByEmail(string email, out string message)
        {
            message = String.Empty;
            User user = null;

            try
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    message = "User Email Is Required";
                    return null;
                }

                user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.EmailAddress == email);
                if (user == null)
                {
                    message = "User with email " + email + " does not exists";
                }
            }
            catch (Exception error)
            {
                user = null;
                message = error.Message;
            }

            return user;
        }

        public User GetUserByID(long userId, out string message)
        {
            message = String.Empty;
            User user = null;

            try
            {
                if (userId <= 0)
                {
                    message = "Invalid User ID";
                    return null;
                }

                user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "User with ID " + userId + " does not exist";
                }
            }
            catch (Exception error)
            {
                user = null;
                message = error.Message;
            }

            return user;
        }

        public User Login(string email, string password, out string message)
        {
            message = String.Empty;
            User loggedUser = null;

            try
            {
                if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password))
                {
                    message = "Incomplete Credentials";
                    return null;
                }

                loggedUser = _context.Users.Include(x => x.Wallet).FirstOrDefault(user => user.EmailAddress == email && user.Password == password);
                if (loggedUser == null)
                {
                    message = "No User with this credential exists";
                    return null;
                }

                if (loggedUser.IsDeactivated)
                {
                    message = "Apologies, your account has been deactivated. Kindly contact your admin.";
                    return null;
                }

                string tokenString = GenerateJSONWebToken(loggedUser);
                loggedUser.AuthenticationToken = tokenString;
                loggedUser.LastLoginDate = DateTime.Now;
                _context.Users.Update(loggedUser);
                _context.SaveChanges();
            }
            catch (Exception error)
            {
                message = error.Message;
                loggedUser = null;
            }

            return loggedUser;
        }

        private string GenerateJSONWebToken(User loggedUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, loggedUser.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.EmailAddress),
                new Claim("Id", loggedUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool LogoutUser(User user, out string message)
        {
            try
            {
                user.AuthenticationToken = String.Empty;
                _context.Users.Update(user);
                _context.SaveChanges();
                message = String.Empty;
                return true;
            }
            catch (Exception error)
            {
                message = error.Message;
                return false;
            }
        }

        public bool Register(string email, string password, string accountNumber, string bankName, string referral, string ans1, string ans2, string ans3, out string message)
        {
            message = String.Empty;
            bool result = false;

            try
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    message = "Email Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(password))
                {
                    message = "Password Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(accountNumber))
                {
                    message = "Account Number Is Required";
                    return false;
                }

                if (String.IsNullOrWhiteSpace(bankName))
                {
                    message = "Bank Name Is Required";
                    return false;
                }

                if (CheckIfEmailExists(email))
                {
                    message = "User with this Email already exists";
                    return false;
                }

                if (!String.IsNullOrWhiteSpace(referral))
                {
                    if (!CheckIfEmailExists(referral))
                    {
                        message = "Referral with email " + referral + " does not exists. Please note that you can register without referral by leaving the referral field blank.";
                        return false;
                    }
                    else
                    {
                        //User ReferralUser = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.EmailAddress == referral);
                        //ReferralUser.Wallet.Balance = ReferralUser.Wallet.Balance + 1000;
                        //Transaction transaction = new Transaction()
                        //{
                        //    Amount = 1000,
                        //    DateOfTransaction = DateTime.Now,
                        //    TransactionDescription = "Referral Bonus of 1000 added to wallet for referring user " + email,
                        //    TransactionType = TransactionType.Credit,
                        //    User = ReferralUser
                        //};
                        //_context.Transactions.Add(transaction);
                        //_context.Wallets.Update(ReferralUser.Wallet);
                    }
                }

                //if (String.IsNullOrWhiteSpace(ans1) || String.IsNullOrWhiteSpace(ans2) || String.IsNullOrWhiteSpace(ans3))
                //{
                //    message = "Apologies, All Secret Answers are required and compulsory, as this will be needed for password recovery.";
                //    return result;
                //}

                User userRegistering = new User()
                {
                    EmailAddress = email,
                    Password = password,
                    DateJoined = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    ReferredBy = referral,
                    //SecretAnswer1 = ans1,
                    //SecretAnswer2 = ans2,
                    //SecretAnswer3 = ans3,
                    Wallet = new Wallet()
                    {
                        AccountNumber = accountNumber,
                        BankName = bankName
                    },
                };

                _context.Users.Add(userRegistering);
                _context.SaveChanges();
                result = true;
            }
            catch (Exception error)
            {
                result = false;
                message = error.Message;
            }

            return result;
        }

        private bool CheckIfEmailExists(string email)
        {
            return _context.Users.Any(x => x.EmailAddress == email);
        }

        public string RecoverPassword(string userEmail, string ans1, string ans2, string ans3, out string message)
        {
            string result = String.Empty;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(userEmail))
                {
                    message = "User Email Is Required to retrieve password";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.SecretAnswer1.ToLower() == ans1 && x.SecretAnswer2.ToLower() == ans2 && x.SecretAnswer3.ToLower() == ans3 && x.EmailAddress == userEmail);
                if (user == null)
                {
                    message = "Password Recovery failed, Please ensure you input the correct answers (Your secret ans must match the secret ans provided at the point of account registration).";
                    return result;
                }

                result = "Your account password is " + user.Password;
            }
            catch (Exception error)
            {
                message = error.Message;
                result = String.Empty;
            }

            return result;
        }

        public List<Transaction> GetUserTransactions(long userId)
        {
            return _context.Transactions.Include(x => x.User).Where(x => x.User.Id == userId).OrderByDescending(x => x.DateOfTransaction).ToList();
        }

        public int GetTotalUsersReferredCount(string userEmail)
        {
            return _context.Users.Where(x => x.ReferredBy == userEmail).Count();
        }

        public int GetTotalUserRegistered()
        {
            return _context.Users.Count();
        }

        public int GetTotalUsersEngagedInInvestment()
        {
            int result = 0;

            try
            {
                List<User> allUsersWithActiveInvestment = new List<User>();
                List<UserInvestment> allUserInvestments = _context.UserInvestments.Include(x => x.User).Include(x => x.Investment).Where(x => x.InvestmentStatus == InvestmentStatus.Pending).ToList();
                foreach (var userinvestment in allUserInvestments)
                {
                    allUsersWithActiveInvestment.Add(userinvestment.User);
                }
                result = allUsersWithActiveInvestment.Distinct().Count();
            }
            catch { }

            return result;
        }

        public bool DeactivateUser(string userEmail, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(userEmail))
                {
                    message = "User Email Is Required";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.EmailAddress == userEmail);
                if (user == null)
                {
                    message = "Error, No user with the email " + userEmail + "exists.";
                    return result;
                }

                if (user.IsDeactivated)
                {
                    message = "Error, User profile is already deactivated.";
                    return result;
                }

                List<UserInvestment> userInvestments = _context.UserInvestments.Include(x => x.User).Where(x => x.User == user).ToList();
                foreach (var userInvestment in userInvestments)
                {
                    userInvestment.IsDeactivated = true;
                    userInvestment.DeactivationDate = DateTime.Now;
                    _context.UserInvestments.Update(userInvestment);
                }

                user.IsDeactivated = true;
                _context.Users.Update(user);
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

        public bool ReActivateUser(string userEmail, out string message)
        {
            bool result = false;
            message = String.Empty;

            try
            {
                if (String.IsNullOrWhiteSpace(userEmail))
                {
                    message = "User Email Is Required";
                    return result;
                }

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.EmailAddress == userEmail);
                if (user == null)
                {
                    message = "Error, No user with the email " + userEmail + "exists.";
                    return result;
                }

                if (!user.IsDeactivated)
                {
                    message = "Error, User profile is already activated.";
                    return result;
                }

                List<UserInvestment> userInvestments = _context.UserInvestments.Include(x => x.User).Where(x => x.User == user).ToList();
                foreach (var userInvestment in userInvestments)
                {
                    TimeSpan checkDeactivationTimeSpan = DateTime.Now - userInvestment.DeactivationDate;
                    if (checkDeactivationTimeSpan.TotalDays < 365)
                    {
                        userInvestment.IsDeactivated = false;
                        TimeSpan timeSpan = userInvestment.DeactivationDate - userInvestment.DateInvested;
                        userInvestment.DateInvested = userInvestment.DateInvested.AddHours(timeSpan.TotalHours);
                        _context.UserInvestments.Update(userInvestment);
                    }
                }

                user.IsDeactivated = false;
                _context.Users.Update(user);
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

        public bool UpdatePasswordInfo(long userId, string oldPassword, string newPassword, string retypePassword, out string message)
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

                if (String.IsNullOrWhiteSpace(oldPassword))
                {
                    message = "Old Password is required.";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(newPassword))
                {
                    message = "New Password is required.";
                    return result;
                }

                if (String.IsNullOrWhiteSpace(retypePassword))
                {
                    message = "Retyped Password is required.";
                    return result;
                }

                if (newPassword != retypePassword)
                {
                    message = "Apologies, Passwords do not match.";
                    return result;
                }

                User user = _context.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "Error, No user account is associated with the passed ID";
                    return result;
                }

                if (user.Password != oldPassword)
                {
                    message = "Apologies, The old password specified does not match with the account current password.";
                    return result;
                }

                user.Password = newPassword;
                _context.Users.Update(user);
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

        public bool UpdateUserInfo(long userId, string firstName, string lastName, string address, string city, string country, string postalCode, string aboutMe, string bankName, string accountNumber, string secretAns1, string secretAns2, string secretAns3, out string message)
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

                User user = _context.Users.Include(x => x.Wallet).FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    message = "Errro, No user with the specified ID exists.";
                    return result;
                }

                if (!String.IsNullOrWhiteSpace(firstName))
                {
                    user.FirstName = firstName;
                }

                if (!String.IsNullOrWhiteSpace(lastName))
                {
                    user.LastName = lastName;
                }

                if (!String.IsNullOrWhiteSpace(address))
                {
                    user.Address = address;
                }

                if (!String.IsNullOrWhiteSpace(city))
                {
                    user.City = city;
                }

                if (!String.IsNullOrWhiteSpace(country))
                {
                    user.Country = country;
                }

                if (!String.IsNullOrWhiteSpace(postalCode))
                {
                    user.PostalCode = postalCode;
                }

                if (!String.IsNullOrWhiteSpace(aboutMe))
                {
                    user.AboutUser = aboutMe;
                }

                if (!String.IsNullOrWhiteSpace(secretAns1))
                {
                    user.SecretAnswer1 = secretAns1;
                }

                if (!String.IsNullOrWhiteSpace(secretAns2))
                {
                    user.SecretAnswer2 = secretAns2;
                }

                if (!String.IsNullOrWhiteSpace(secretAns3))
                {
                    user.SecretAnswer3 = secretAns3;
                }

                if (!String.IsNullOrWhiteSpace(bankName) || !String.IsNullOrWhiteSpace(accountNumber))
                {
                    if (user.Wallet == null)
                    {
                        message = "Error, This user is not linked to any wallet account.";
                        return result;
                    }

                    if (!String.IsNullOrWhiteSpace(bankName))
                    {
                        user.Wallet.BankName = bankName;
                    }

                    if (!String.IsNullOrWhiteSpace(accountNumber))
                    {
                        user.Wallet.AccountNumber = accountNumber;
                    }

                    _context.Wallets.Update(user.Wallet);
                }

                _context.Users.Update(user);
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
