using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public double Balance { get; set; }
    }
}
