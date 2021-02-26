using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AfrikcreditData.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool isAdmin { get; set; }
        public string AuthenticationToken { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string ReferredBy { get; set; }
        public string ReferralIds { get; set; }
        public Wallet Wallet { get; set; }
        public string SecretAnswer1 { get; set; }
        public string SecretAnswer2 { get; set; }
        public string SecretAnswer3 { get; set; }
        public bool IsDeactivated { get; set; }
        public string Address { get; set; }
        public bool HasPaidReferee { get; set; }
        public string PhoneNumber { get; set; }
    }
}
