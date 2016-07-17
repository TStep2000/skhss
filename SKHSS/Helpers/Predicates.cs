using SKHSS.Authentication;
using SKHSS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Helpers
{
    public class OrderByHighestRole : IComparer<UserLogin>
    {
        public int Compare(UserLogin x, UserLogin y)
        {
            return x.USR_ROL_RoleID.ToString().First().CompareTo(y.USR_ROL_RoleID.ToString().First());
        }
    }
    public class Predicates
    {
        public static Func<Coach, Boolean> CoachByUsername(String Username)
        {
            return delegate(Coach m) { return m.CCH_USR_RecordID == new SKHSSMembershipProvider().GetUser(Username, Username).UserLogin.USR_RecordID; };
        }
        public static Func<UserLogin, Boolean> LoginByUsername(String Username)
        {
            return delegate(UserLogin m) { return m.USR_RecordID == new SKHSSMembershipProvider().GetUser(Username, Username).UserLogin.USR_RecordID; };
        }
        public static Func<Parent, Boolean> ParentByUsername(String Username)
        {
            return delegate(Parent m) { return m.PRT_USR_RecordID == new SKHSSMembershipProvider().GetUser(Username, Username).UserLogin.USR_RecordID; };
        }
        public static Func<Order, Boolean> CurrentOrder()
        {
            return delegate(Order m) { return m.ORD_Year == Global.CurrentYear && m.ORD_SeasonID == Global.CurrentSeasonID; };
        }
        public static Func<Payment, Boolean> CurrentPayment()
        {
            return delegate(Payment m) { return m.PMT_Current == true; };
        }
        public static Func<Payment, Boolean> CurrentPaymentByParent(Parent p)
        {
            return delegate(Payment m) { return m.Order.ORD_Year == Global.CurrentYear && m.Order.ORD_SeasonID == Global.CurrentSeasonID && m.Order.ORD_PRT_RecordID == p.PRT_RecordID && m.PMT_Current == true; };
        }
        public static Func<Payment, Boolean> CurrentPaymentsByParent(Parent p)
        {
            return delegate(Payment m) { return m.Order.ORD_Year == Global.CurrentYear && m.Order.ORD_SeasonID == Global.CurrentSeasonID && m.Order.ORD_PRT_RecordID == p.PRT_RecordID; };
        }
        public static Func<Teammate, Boolean> CurrentTeammate()
        {
            return delegate(Teammate m) { return m.TMT_Year == Global.CurrentYear && m.TMT_SeasonID == Global.CurrentSeasonID; };
        }
        public static Func<Teammate, Boolean> CurrentTeammatesByParent(Parent p)
        {
            return delegate(Teammate m) { return m.TMT_Year == Global.CurrentYear && m.TMT_SeasonID == Global.CurrentSeasonID && m.Child.CLD_PRT_RecordID == p.PRT_RecordID; };
        }
        public static Func<Teammate, Boolean> AllCurrentTeammates()
        {
            return delegate(Teammate m) { return m.TMT_Year == Global.CurrentYear && m.TMT_SeasonID == Global.CurrentSeasonID; };
        }

        public static Func<UserLogin, Boolean> IsInRole(Int32 Role)
        {
            return delegate(UserLogin ul) { return Global.IsInRole(ul, Role); };
        }
        public static Func<UserLogin, Boolean> IsInAnyRole(Int32[] Roles)
        {
            return delegate(UserLogin ul) { return Global.IsInAnyRole(ul, Roles); };
        }
        public static Func<UserLogin, Boolean> IsInRoleOrHigher(Int32 Role)
        {
            return delegate(UserLogin ul) { return Global.IsInRoleOrHigher(ul, Role); };
        }
        public static Func<UserLogin, Boolean> IsInRoleOrLower(Int32 Role)
        {
            return delegate(UserLogin ul) { return Global.IsInRoleOrLower(ul, Role); };
        }

        public static Func<Parent, Boolean> HasCurrentOrder()
        {
            return delegate(Parent p) {return Data.GetCurrentOrder(p) != null; };
        }

        /*public static IComparer<UserLogin> HighestRole()
        {
            return (IComparer<UserLogin>)delegate(UserLogin ul, UserLogin ul2) { return Global.HighestRole(ul.USR_Username).CompareTo(Global.HighestRole(ul2.USR_Username)); };
        }*/
    }
}