using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelOrder
    {
        public String OrderRecordID { get; set; }
        public String SeasonID { get; set; }
        public String Year { get; set; }
        public String Date { get; set; }
        public String Volunteer { get; set; }

        public String Approved { get; set; }
        public String Mode { get; set; }
        public List<ObjectModelPayment> Payments { get; set; }
    }
}