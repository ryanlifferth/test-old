using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.GoodTesting
{
    public class Account
    {
        public DateTime DueDate { get; set; }
        public Customer Customer { get; set; }
        public double Balance { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public bool IsVip { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
    }

}
