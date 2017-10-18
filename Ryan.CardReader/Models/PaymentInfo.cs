using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.CardReader.Models
{
    public class PaymentInfo
    {

        public string CreditCardNumber { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string SecurityCode { get; set; }

    }
}
