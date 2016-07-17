using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKHSS.Authentication;
using SKHSS.Helpers;

namespace SKHSS.Models
{
    public class PictureModel
    {
        public String Filename { get; set; }
        public String PictureData { get; set; }
        public String Caption { get; set; }
        public String Team { get; set; }
        public String PIC_RecordID { get; set; }
        public String ALB_RecordID { get; set; }
        public PictureModel()
        {
            Filename = "";
        }

        public Picture Picture
        {
            get
            {
                Picture p = new Picture();
                p.PIC_RecordID = Guid.NewGuid();
                p.PIC_Filename = Filename;
                p.PIC_Caption = Caption;
                p.PIC_TeamID = (Team!=null)?Int32.Parse(Team):-1;
                p.PIC_Year = Global.CurrentYear;
                p.PIC_SeasonID = Global.CurrentSeasonID;
                return p;
            }
        }
    }
}