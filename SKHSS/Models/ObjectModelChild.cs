using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelChild
    {
        public String ChildRecordID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Birthdate { get; set; }
        public String Gender { get; set; }
        public String Notes { get; set; }

        public String Mode { get; set; }
        public List<ObjectModelTeammate> Teammates { get; set; }
        
        public ObjectModelChild()
        {

        }
    }
}