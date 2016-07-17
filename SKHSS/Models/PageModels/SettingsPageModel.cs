using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKHSS.Helpers;
namespace SKHSS.Models.PageModels
{
    public class SettingsPageModel
    {
        public Int32 CurrentSeasonID { get; set; }
        public Int32 CurrentYear { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LateFeeDate { get; set; }
        public DateTime RegCutoff { get; set; }
        public List<Team> TAGs { get; set; }
        public SettingsPageModel()
        {
            CurrentSeasonID = Global.CurrentSeasonID;
            CurrentYear = Global.CurrentYear;
            EndDate = Global.EndDate;
            LateFeeDate = Global.LateFeeDate;
            RegCutoff = Global.RegCutoff;
            TAGs = Data.GetTMs().ToList();
        }
        public SettingsPageModel(Boolean safe)
        {
            CurrentSeasonID = -1;
            CurrentYear = -1;
            EndDate = DateTime.Now;
            LateFeeDate = DateTime.Now;
            RegCutoff = DateTime.Now;
            TAGs = new List<Team>();
        }
    }
}