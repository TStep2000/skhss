using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class ObjectModelTeammate
    {
        public String TeammateRecordID { get; set; }
        public String Year { get; set; }
        public String SeasonID { get; set; }
        public String TeamID { get; set; }
        public String ShirtID { get; set; }

        public String PicID { get; set; }

        public String Accepted { get; set; }
        public String Mode { get; set; }
        public ObjectModelTeammate()
        {
        }
    }
}