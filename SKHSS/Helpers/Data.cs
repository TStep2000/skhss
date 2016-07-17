using SKHSS.Authentication;
using SKHSS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SKHSS.Helpers
{
    public class Data
    {
        public static SKHSSEntities _db = new SKHSSEntities();
        public static SKHSSEntities db { get { return new SKHSSEntities(); } }
        public static void RefreshDB() { _db = new SKHSSEntities(); }
        public static UserLogin GetUL(Expression<Func<UserLogin, bool>> predicate)
        {
            return _db.UserLogins.SingleOrDefault(predicate);
        }
        public static UserLogin pGetUL(Func<UserLogin, bool> predicate)
        {
            return _db.UserLogins.SingleOrDefault(predicate);
        }
        public static IEnumerable<UserLogin> GetULs(Expression<Func<UserLogin, bool>> predicate)
        {
            return _db.UserLogins.Where(predicate);
        }
        public static IEnumerable<UserLogin> pGetULs(Func<UserLogin, bool> predicate)
        {
            return _db.UserLogins.Where(predicate);
        }
        public static DbSet<UserLogin> GetULs()
        {
            return _db.UserLogins;
        }

        public static Parent GetP(Expression<Func<Parent, bool>> predicate)
        {
            return _db.Parents.SingleOrDefault(predicate);
        }
        public static Parent pGetP(Func<Parent, bool> predicate)
        {
            return _db.Parents.SingleOrDefault(predicate);
        }
        public static IEnumerable<Parent> GetPs(Expression<Func<Parent, bool>> predicate)
        {
            return _db.Parents.Where(predicate);
        }
        public static IEnumerable<Parent> pGetPs(Func<Parent, bool> predicate)
        {
            return _db.Parents.Where(predicate);
        }
        public static DbSet<Parent> GetPs()
        {
            return _db.Parents;
        }

        public static Child GetC(Expression<Func<Child, bool>> predicate)
        {
            return _db.Children.SingleOrDefault(predicate);
        }
        public static Child pGetC(Func<Child, bool> predicate)
        {
            return _db.Children.SingleOrDefault(predicate);
        }
        public static IQueryable<Child> GetCs(Expression<Func<Child, bool>> predicate)
        {
            return _db.Children.Where(predicate);
        }
        public static IEnumerable<Child> pGetCs(Func<Child, bool> predicate)
        {
            return _db.Children.Where(predicate);
        }
        public static DbSet<Child> GetCs()
        {
            return _db.Children;
        }

        public static Teammate GetT(Expression<Func<Teammate, bool>> predicate)
        {
            return _db.Teammates.SingleOrDefault(predicate);
        }
        public static Teammate pGetT(Func<Teammate, bool> predicate)
        {
            return _db.Teammates.SingleOrDefault(predicate);
        }
        public static IQueryable<Teammate> GetTs(Expression<Func<Teammate, bool>> predicate)
        {
            return _db.Teammates.Where(predicate);
        }
        public static IEnumerable<Teammate> pGetTs(Func<Teammate, bool> predicate)
        {
            return _db.Teammates.Where(predicate);
        }
        public static DbSet<Teammate> GetTs()
        {
            return _db.Teammates;
        }

        public static Team GetTM(Expression<Func<Team, bool>> predicate)
        {
            return _db.Teams.SingleOrDefault(predicate);
        }
        public static Team pGetTM(Func<Team, bool> predicate)
        {
            return _db.Teams.SingleOrDefault(predicate);
        }
        public static IQueryable<Team> GetTMs(Expression<Func<Team, bool>> predicate)
        {
            return _db.Teams.Where(predicate);
        }
        public static IEnumerable<Team> pGetTMs(Func<Team, bool> predicate)
        {
            return _db.Teams.Where(predicate);
        }
        public static DbSet<Team> GetTMs()
        {
            return _db.Teams;
        }

        public static Order GetO(Expression<Func<Order, bool>> predicate)
        {
            return _db.Orders.SingleOrDefault(predicate);
        }
        public static Order pGetO(Func<Order, bool> predicate)
        {
            return _db.Orders.SingleOrDefault(predicate);
        }
        public static IQueryable<Order> GetOs(Expression<Func<Order, bool>> predicate)
        {
            return _db.Orders.Where(predicate);
        }
        public static IEnumerable<Order> pGetOs(Func<Order, bool> predicate)
        {
            return _db.Orders.Where(predicate);
        }
        public static DbSet<Order> GetOs()
        {
            return _db.Orders;
        }

        public static Payment GetPM(Expression<Func<Payment, bool>> predicate)
        {
            return _db.Payments.SingleOrDefault(predicate);
        }
        public static Payment pGetPM(Func<Payment, bool> predicate)
        {
            return _db.Payments.SingleOrDefault(predicate);
        }
        public static IQueryable<Payment> GetPMs(Expression<Func<Payment, bool>> predicate)
        {
            return _db.Payments.Where(predicate);
        }
        public static IEnumerable<Payment> pGetPMs(Func<Payment, bool> predicate)
        {
            return _db.Payments.Where(predicate);
        }
        public static DbSet<Payment> GetPMs()
        {
            return _db.Payments;
        }

        public static Cache GetCC(Expression<Func<Cache, bool>> predicate)
        {
            return _db.Caches.SingleOrDefault(predicate);
        }
        public static Cache pGetCC(Func<Cache, bool> predicate)
        {
            return _db.Caches.SingleOrDefault(predicate);
        }
        public static IQueryable<Cache> GetCCs(Expression<Func<Cache, bool>> predicate)
        {
            return _db.Caches.Where(predicate);
        }
        public static IEnumerable<Cache> pGetCCs(Func<Cache, bool> predicate)
        {
            return _db.Caches.Where(predicate);
        }
        public static DbSet<Cache> GetCCs()
        {
            return _db.Caches;
        }

        public static CacheItem GetCCI(Expression<Func<CacheItem, bool>> predicate)
        {
            return _db.CacheItems.SingleOrDefault(predicate);
        }
        public static CacheItem pGetCCI(Func<CacheItem, bool> predicate)
        {
            return _db.CacheItems.SingleOrDefault(predicate);
        }
        public static IQueryable<CacheItem> GetCCIs(Expression<Func<CacheItem, bool>> predicate)
        {
            return _db.CacheItems.Where(predicate);
        }
        public static IEnumerable<CacheItem> pGetCCIs(Func<CacheItem, bool> predicate)
        {
            return _db.CacheItems.Where(predicate);
        }
        public static DbSet<CacheItem> GetCCIs()
        {
            return _db.CacheItems;
        }

        public static Coach GetCH(Expression<Func<Coach, bool>> predicate)
        {
            return _db.Coaches.SingleOrDefault(predicate);
        }
        public static Coach pGetCH(Func<Coach, bool> predicate)
        {
            return _db.Coaches.SingleOrDefault(predicate);
        }
        public static IQueryable<Coach> GetCHs(Expression<Func<Coach, bool>> predicate)
        {
            return _db.Coaches.Where(predicate);
        }
        public static IEnumerable<Coach> pGetCHs(Func<Coach, bool> predicate)
        {
            return _db.Coaches.Where(predicate);
        }
        public static DbSet<Coach> GetCHs()
        {
            return _db.Coaches;
        }

        public static Contact GetCT(Expression<Func<Contact, bool>> predicate)
        {
            return _db.Contacts.SingleOrDefault(predicate);
        }
        public static Contact pGetCT(Func<Contact, bool> predicate)
        {
            return _db.Contacts.SingleOrDefault(predicate);
        }
        public static IQueryable<Contact> GetCTs(Expression<Func<Contact, bool>> predicate)
        {
            return _db.Contacts.Where(predicate);
        }
        public static IEnumerable<Contact> pGetCTs(Func<Contact, bool> predicate)
        {
            return _db.Contacts.Where(predicate);
        }
        public static DbSet<Contact> GetCTs()
        {
            return _db.Contacts;
        }

        public static void SaveDB()
        {
            _db.SaveChanges();
        }

        #region ObjectMethods-------------------------------------------------------------------------------------------------------

        public static String GetPrimaryEmail(Guid UserRecordID)
        {
            Boolean primary = db.Parents.Single(m=>m.PRT_USR_RecordID==UserRecordID).PRT_PrimaryEmail.Value;                                                    //||waiting on changes
            return db.Contacts.Single(m => m.CON_PRTID == primary).CON_Email;
        }
        public static String GetPrimaryPhone(Guid UserRecordID)
        {
            Boolean primary = db.Parents.Single(m => m.PRT_USR_RecordID == UserRecordID).PRT_PrimaryPhone.Value;                                                //||
            return db.Contacts.Single(m => m.CON_PRTID == primary).CON_Phone;
        }
        public static String GetSysVar(String Name)
        {
            return db.SysVars.Single(m => m.GBL_ID == Name).GBL_Data;
        }
        public static void SetSysVar(String Name, String Value)
        {
            SysVar s = _db.SysVars.SingleOrDefault(m => m.GBL_ID == Name);
            if (s == null)
            {
                s = new SysVar();
                s.GBL_ID = Name;
            }
            s.GBL_Data = Value;
            SaveDB();
        }
        public static Coach GetCoach(String UserIdentity)
        {
            if (UserIdentity == "") { return null; }
            Coach c = new Coach();
            if (Global.IsInRole(UserIdentity, Definitions.CoachRole))
            {
                Guid ulg = GetLogin(UserIdentity).USR_RecordID;
                c = GetCH(m => m.CCH_USR_RecordID == ulg);
            }
            else
            {
                throw new Exception("Only for coach role");
            }
            return c;
        }
        public static UserLogin GetLogin(String UserIdentity)
        {
            if (UserIdentity == "") { return null; }
            SKHSSUser sku = new SKHSSMembershipProvider().GetUser(UserIdentity, UserIdentity);
            return sku != null ? sku.UserLogin : null;
        }
        public static List<UserLogin> GetLoginsByRole(Int32 Role)
        {
            return _db.UserLogins.ToList().FindAll(delegate(UserLogin ul) { return Global.IsInRole(ul, Role); });
        }

        public static Order GetCurrentOrder(Parent p)
        {
            return GetO(m => m.ORD_PRT_RecordID == p.PRT_RecordID && m.ORD_SeasonID == Global.CurrentSeasonID && m.ORD_Year == Global.CurrentYear);
        }
        public static Payment GetCurrentPayment(Parent p)
        {
            return GetCurrentOrder(p).Payments.SingleOrDefault(m=>m.PMT_Current == true);
        }
        public static Payment GetCurrentPayment(Order o)
        {
            return GetPM(m => m.PMT_ORD_RecordID == o.ORD_RecordID && m.PMT_Current == true);
        }
        public static Teammate GetCurrentTeammate(Child c)
        {
            return GetT(m => m.TMT_CLD_RecordID == c.CLD_RecordID && m.TMT_SeasonID == Global.CurrentSeasonID && m.TMT_Year == Global.CurrentYear);
        }
        public static List<Teammate> GetCurrentTeammates(Parent p)
        {
            return GetTs(m => m.Child.CLD_PRT_RecordID == p.PRT_RecordID && m.TMT_SeasonID == Global.CurrentSeasonID && m.TMT_Year == Global.CurrentYear).ToList();
        }
        public static List<Teammate> GetAllCurrentTeammates()
        {
            return GetTs(m => m.TMT_SeasonID == Global.CurrentSeasonID && m.TMT_Year == Global.CurrentYear).ToList();
        }

        #endregion
    }
}