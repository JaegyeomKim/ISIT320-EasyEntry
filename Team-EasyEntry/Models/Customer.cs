using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Team_EasyEntry.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstShotDate { get; set; }
        public string FirstShotName { get; set; }
        public string SecondShotDate { get; set; }
        public string SecondShotName { get; set; }
        public string ThirdShotDate { get; set; }
        public string ThirdShotName { get; set; }

        public Customer()
        {

        }

    }
}
