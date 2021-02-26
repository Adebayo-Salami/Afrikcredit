using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Afrikcredit.Models
{
    public class AdminViewModel
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public string DisplayMessage { get; set; }
        public List<AfrikcreditData.Models.Notification> Notifications { get; set; }
        public string PhoneNumber { get; set; }
    }
}
