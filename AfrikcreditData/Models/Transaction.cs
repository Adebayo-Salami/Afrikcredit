using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public User User { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public string TransactionDescription { get; set; }
    }
}
