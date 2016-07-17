using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelPayment
    {
        public String PaymentRecordID { get; set; }
        public String Current { get; set; }
        public String Date { get; set; }
        public String Registrations { get; set; }
        public String Uniforms { get; set; }
        public String Total { get; set; }
        public String TransactionID { get; set; }
        public String TransactionDate { get; set; }

        public String LateFee { get; set; }
        public String Paid { get; set; }
        public String Approved { get; set; }
        public String Mode { get; set; }
        public List<ObjectModelChild> Children { get; set; }
    }
}