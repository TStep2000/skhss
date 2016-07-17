using SKHSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKHSS.Helpers
{
    public class Definitions
    {
        public static List<String> DateIcons = new List<String>() { "blank.png", "page.png", "trophysmall.png" };
        public static List<String> CoachPositions = new List<String>() { "Coach", "Assistant" };
        public static List<String> Roles = new List<String>() { "Me", "WebAdmin", "President", "Secretary", "Treasurer", "Sports Director", "Head Coach", "Coach", "Parent", "Guest" };
        public static List<String> Seasons = new List<String>() { "Basketball", "Baseball", "Summer", "Soccer" };
        public static List<String> ShirtSizes = new List<String>() { "Youth Small", "Youth Medium", "Youth Large", "Adult Small", "Adult Medium", "Adult Large", "Adult Extra Large", "Adult XX Large" };
        //public static List<String> Teams = new List<String>() { "PeeWees", "Middlers ", "Intermediates", "Juniors", "Jr Girls", "Seniors", "Sr Girls" };
        public static List<String> Teams
        {
            get
            {
                List<String> tms = new List<string>();
                foreach (Team t in Data.db.Teams.ToList())
                {
                    tms.Add(t.TEM_TeamName);
                }
                return tms;
            }
        }

        public static Int32 PeeWees = 0;
        public static Int32 Middlers = 1;
        public static Int32 Intermediates = 2;
        public static Int32 Juniors = 3;
        public static Int32 Seniors = 4;
        public static Int32 GirlsTeam = 5;

        public static Int32 Coach = 0;
        public static Int32 Assistant = 1;

        public static Int32 MeRole = 0;
        public static Int32 WebAdminRole = 1;
        public static Int32 PresRole = 2;
        public static Int32 SecretRole = 3;
        public static Int32 TreasRole = 4;
        public static Int32 SportsRole = 5;
        public static Int32 HeadCRole = 6;
        public static Int32 CoachRole = 7;
        public static Int32 ParentRole = 8;
        public static Int32 GuestRole = 9;
        
        public static Int32 Basketball = 0;
        public static Int32 Baseball = 1;
        public static Int32 Summer = 2;
        public static Int32 Soccer = 3;

        public static Int32 YouthSmall = 0;
        public static Int32 YouthMedium = 1;
        public static Int32 YouthLarge = 2;
        public static Int32 AdultSmall = 3;
        public static Int32 AdultMedium = 4;
        public static Int32 AdultLarge = 5;
        public static Int32 AdultExtraLarge = 6;
        public static Int32 AdultXXLarge = 7;

        public static Int32 RegistrationPrice = 40;
        public static Int32 UniformPrice = 25;
        public static Int32 FamilyMaxiumum = 160;

        public static String GetHighestRole(Int32 id)
        {
            return Roles[Int32.Parse(id.ToString().Last().ToString())];
        }
        public static List<SelectListItem> GetShirtSizeList()
        {
            List<SelectListItem> sil = new List<SelectListItem>();
            for (int i = 0; i < 8; i++)
            {
                sil.Add(new SelectListItem() { Text = ShirtSizes[i], Value = i.ToString(), Selected = false });
            }
            return sil;
        }
    }
}