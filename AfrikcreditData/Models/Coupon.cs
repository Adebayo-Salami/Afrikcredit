using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Value { get; set; }
        public User CreatedBy { get; set; }
        public User UsedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUsed { get; set; }
    }
}
