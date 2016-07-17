using SKHSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Helpers
{
    public class Fix
    {
        public static Boolean FixError(Guid id, Int32 error)
        {
            switch (error)
            {
                case 1:
                    return Fix.FixError1(id);
                case 2:
                    return Fix.FixError2(id);
                case 3:
                    return Fix.FixError3(id);
                case 4:
                    return Fix.FixError4(id);
                default:
                    return false;
            }
        }
        //Error 1 Parent without parent role
        private static Boolean FixError1(Guid urid)
        {
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == urid);
            Global.AddRole(ul.USR_RecordID, Definitions.ParentRole);
            Data.SaveDB();
            return true;
        }

        //Error 2 Order with payments, but no current payment
        private static Boolean FixError2(Guid orid)
        {
            Order o = Data.GetO(m => m.ORD_RecordID == orid);
            Payment pm = o.Payments.OrderBy(m => m.PMT_Date.Value).First();
            pm.PMT_Current = true;
            Data.SaveDB();
            return true;
        }

        //Error 3 Payment with no date/empty date
        private static Boolean FixError3(Guid pmrid)
        {
            Payment pm = Data.GetPM(m => m.PMT_RecordID == pmrid);
            pm.PMT_Date = DateTime.Now;
            Data.SaveDB();
            return true;
        }

        //Error 4 Payment with no date/empty date
        private static Boolean FixError4(Guid trid)
        {
            Teammate t = Data.GetT(m => m.TMT_RecordID == trid);
            t.TMT_TEM_TeamID = Global.TeamID(t.Child.CLD_Birthdate, t.Child.CLD_Gender);
            Data.SaveDB();
            return true;
        }
    }
}