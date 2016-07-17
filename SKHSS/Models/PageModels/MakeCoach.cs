using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class MakeCoach
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public Int32 TeamID { get; set; }
        public Int32 PositionID { get; set; }

        public MakeCoach()
        {

        }
    }
}