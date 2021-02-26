using AfrikcreditData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Interfaces
{
    public interface IUserService
    {
        bool Register(string email, string password, string accountNumber, string bankName, string referral, out string message);
        User Login(string email, string password, out string message);
        User GetUserByEmail(string email, out string message);
        User GetUserByID(long userId, out string message);
        bool CheckUserAuthentication(long userId, string authenticationToken, out User loggedUser);
        bool LogoutUser(User user, out string message);
        string RecoverPassword(string userEmail, string ans1, string ans2, string ans3, out string message);
        List<Transaction> GetUserTransactions(long userId);
        int GetTotalUsersReferredCount(string userEmail);
        int GetTotalUserRegistered();
        int GetTotalUsersEngagedInInvestment();
        bool DeactivateUser(string userEmail, out string message);
        bool ReActivateUser(string userEmail, out string message);
        bool UpdatePasswordInfo(long userId, string oldPassword, string newPassword, string retypePassword, out string message);
        bool UpdateUserInfo(long userId, string firstName, string lastName, string address, string city, string country, string postalCode, string aboutMe, string bankName, string accountNumber, string secretAns1, string secretAns2, string secretAns3, out string message);
    }
}
