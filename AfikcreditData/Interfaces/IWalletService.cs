using System;
using System.Collections.Generic;
using System.Text;

namespace AfikcreditData.Interfaces
{
    public interface IWalletService
    {
        bool FundWallet(long userId, string couponCode, out string message);
        bool TransferFunds(long userId, double amount, string receiverEmailAddress, out string message);
        bool UpdateWalletInfo(long userId, string bankName, string accountNumber, out string message);
    }
}
