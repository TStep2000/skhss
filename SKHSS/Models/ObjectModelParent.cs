using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelParent
    {
        public String ParentRecordID { get; set; }
        public String FatherName { get; set; }
        public String MotherName { get; set; }
        public String LastName { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Zipcode { get; set; }
        public String FatherEmail { get; set; }
        public String MotherEmail { get; set; }
        public String FatherPhone { get; set; }
        public String MotherPhone { get; set; }
        public String PrimaryEmail { get; set; }
        public String PrimaryPhone { get; set; }
        
        public String Approve { get; set; }
        public String Mode { get; set; }
        public List<ObjectModelChild> Children { get; set; }
        public List<ObjectModelOrder> Orders { get; set; }
    }
}