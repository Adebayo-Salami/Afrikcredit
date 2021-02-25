using System;
using System.Collections.Generic;
using System.Text;

namespace AfrikcreditData.Models
{
    public class Investment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public int DaysDuration { get; set; }
        public string Description { get; set; }
        public double AmountToBeGotten { get; set; }
    }
}
