using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelCoach
    {
        public String CoachRecordID { get; set; }
        public String TeamID { get; set; }
        public String PositionID { get; set; }
        public String PicID { get; set; }
        public String PRTID { get; set; }
        public String Mode { get; set; }
        public ObjectModelCoach()
        {
        }
    }
}