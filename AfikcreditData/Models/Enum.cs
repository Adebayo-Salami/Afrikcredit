using System;
using System.Collections.Generic;
using System.Text;

namespace AfikcreditData.Models
{
    public enum InvestmentStatus
    {
        Pending,
        Paid
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }

    public enum CouponStatus
    {
        New,
        Used
    }
}
