using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AfikcreditData.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string AboutUser { get; set; }
        public bool HasPaidReferee { get; set; }
    }
}
