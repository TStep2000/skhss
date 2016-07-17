using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModel
    {
        public String UserRecordID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String PrimaryEmail { get; set; }
        public String PrimaryPhone { get; set; }
        public String PhoneProvider { get; set; }
        public String ResetDate { get; set; }
        public String RoleID { get; set; }

        public String Activated { get; set; }
        public String Approved { get; set; }
        public String Temp { get; set; }
        public String Test { get; set; }
        public String Paid { get; set; }
        public String Mode { get; set; }
        public String NoEmail { get; set; }
        public Boolean NoEmailCheck
        {
            get { return Boolean.Parse(NoEmail); }
            set { NoEmail = value.ToString(); }
        }
        public ObjectModelParent Parent { get; set; }
        public ObjectModel()
        {
        }
    }
}