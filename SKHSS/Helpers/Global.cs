using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKHSS.Authentication;
using System.Web.Security;
using SKHSS.Models;
using SKHSS.Models.PageModels;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Net;
using System.Linq.Expressions;
using System.Data.Entity;

namespace SKHSS.Helpers
{
    public class Global
    {
        
        #region DatabaseValues

        private static Int32 _CurrentYear = -1;
        public static Int32 CurrentYear
        {
            get
            { 
                if(_CurrentYear == -1){
                    _CurrentYear = Int32.Parse(Data.GetSysVar("CurrentYear"));
                }
                return _CurrentYear;
            }
            set
            {
                Data.SetSysVar("CurrentYear", value.ToString());
                _CurrentYear = value;
            }
        }
        private static Int32 _CurrentSeasonID = -1;
        public static Int32 CurrentSeasonID
        {
            get
            {
                if(_CurrentSeasonID == -1){
                    _CurrentSeasonID = Int32.Parse(Data.GetSysVar("CurrentSeasonID"));
                }
                return _CurrentSeasonID;
            }
            set
            {
                Data.SetSysVar("CurrentSeasonID", value.ToString());
                _CurrentSeasonID = value;
            }
        }
        private static DateTime? _LateFeeDate = null;
        public static DateTime LateFeeDate
        {
            get 
            { 
                if(_LateFeeDate == null){
                    _LateFeeDate = DateTime.Parse(Data.GetSysVar("LateFeeDate"));
                }
                return _LateFeeDate.Value; 
            }
            set
            {
                Data.SetSysVar("LateFeeDate", value.ToString());
                _LateFeeDate = value;
            }
        }
        private static DateTime? _RegCutoff = null;
        public static DateTime RegCutoff
        {
            get 
            {
                if (_RegCutoff == null)
                {
                    _RegCutoff = DateTime.Parse(Data.GetSysVar("RegCutoff"));
                }
                return _RegCutoff.Value; 
            }
            set
            {
                Data.SetSysVar("RegCutoff", value.ToString());
                _RegCutoff = value;
            }
        }
        private static DateTime? _EndDate = null;
        public static DateTime EndDate
        {
            get
            {
                if (_EndDate == null)
                {
                    _EndDate = DateTime.Parse(Data.GetSysVar("EndDate"));
                }
                return _EndDate.Value;
            }
            set
            {
                Data.SetSysVar("EndDate", value.ToString());
                _EndDate = value;
            }
        }
        
        public static String SiteRoot
        {
            get
            {
                //Return variable declaration
                var appPath = string.Empty;

                //Getting the current context of HTTP request
                var context = HttpContext.Current;

                //Checking the current context content
                if (context != null)
                {
                    //Formatting the fully qualified website url/name
                    appPath = string.Format("{0}://{1}{2}{3}",
                                            context.Request.Url.Scheme,
                                            context.Request.Url.Host,
                                            context.Request.Url.Port == 80
                                                ? string.Empty
                                                : ":" + context.Request.Url.Port,
                                            context.Request.ApplicationPath);
                }

                if (!appPath.EndsWith("/"))
                    appPath += "/";

                return appPath;
            }
        }

        #endregion

        #region InfoMethods

            public static Boolean IsLateFee
            {
                get
                {
                    return DateTime.Now.CompareTo(LateFeeDate) > 0;
                }
            }
            public static Boolean IsRegCutoff
            {
                get
                {
                    return DateTime.Now.CompareTo(RegCutoff) >= 0;
                }
            }

            public static String ParentNames(Parent p, Boolean and = false, Boolean first = false, Boolean last = true, Boolean reverse = false, Int32 parent = 0)
            {
                Boolean dad = p.PRT_FatherName != null && p.PRT_FatherName != "";
                Boolean mom = p.PRT_MotherName != null && p.PRT_MotherName != "";
                String sDad = (dad?(first ? p.PRT_FatherName[0].ToString() : p.PRT_FatherName):"");
                String sMom = (mom?(first ? p.PRT_MotherName[0].ToString() : p.PRT_MotherName):"");
                String sAnd = (and ? "&" : "and");
                String form = (last ? (reverse ? "{1}, {0}" : "{0} {1}") : "{0}");

                String s1 = (parent == 0 ? (dad & mom ? sDad + " " + sAnd + " " + sMom : (dad ? sDad : sMom)) : (parent == 1 ? sDad : sMom));
                String s2 = p.PRT_LastName;
                return String.Format(form, s1, s2);
            }
            public static String ParentName(Coach c, Boolean reverse = true, Boolean first = false, Boolean last = true)
            {
                return ParentNames(c.UserLogin.Parent, false, first, last, reverse, c.CCH_PRTID ? 2 : 1);
            }
            public static String ChildLastName(Child c)
            {
                if(c.CLD_LastName==null||c.CLD_LastName==""){
                    return c.Parent.PRT_LastName;
                } else {
                    return c.CLD_LastName;
                }
            }
            public static Int32 CombPrice(Int32 r, Int32 u)
            {
                Int32 rt = (r * Definitions.RegistrationPrice);
                Int32 ut = (u * Definitions.UniformPrice);
                Int32 tot = rt + ut;
                return tot;
            }
            public static Int32 PaymentTotal(Payment pm)
            {
                Int32 rt = (pm.PMT_Registrations * Definitions.RegistrationPrice);
                if (rt > Definitions.FamilyMaxiumum) { rt = Definitions.FamilyMaxiumum; }
                Int32 ut = (pm.PMT_Uniforms * Definitions.UniformPrice);
                Int32 LateFee = (pm.PMT_LateFee ? 10 : 0);
                return rt + ut + LateFee;
            }
            public static Int32 OrderTotal(Order o)
            {
                if (o != null && o.Payments.Count > 0)
                {
                    Int32 r = o.Payments.Sum(m => m.PMT_Registrations);
                    Int32 u = o.Payments.Sum(m => m.PMT_Uniforms);
                    Boolean lf = o.Payments.Any(m => m.PMT_LateFee);
                    Int32 rt = r * Definitions.RegistrationPrice;
                    Int32 ut = u * Definitions.UniformPrice;
                    rt = rt > Definitions.FamilyMaxiumum ? 160 : rt;
                    Int32 tot = rt + ut;
                    tot += (lf && r != 0) ? 10 : 0;
                    return tot;
                }
                else
                {
                    return 0;
                }
            }
            public static Int32 TotalPrice(Int32 r, Int32 u, Order o)
            {
                if (o != null && o.Payments.Count > 0)
                {
                    Payment pm = Data.GetCurrentPayment(o);
                    Int32 rt = r * Definitions.RegistrationPrice;
                    Int32 ut = u * Definitions.UniformPrice;
                    Int32 rtot = (o.Payments.Sum(m => m.PMT_Registrations) - r) * Definitions.RegistrationPrice;
                    rt = rtot > Definitions.FamilyMaxiumum ? 0 : (rtot + rt > Definitions.FamilyMaxiumum ? rtot + rt - Definitions.FamilyMaxiumum : rt);
                    Int32 tot = rt + ut;
                    tot += (pm.PMT_LateFee && r != 0) ? 10 : 0;
                    return tot;
                }
                else
                {
                    return 0;
                }
            }

            public static Boolean PaymentComplete(Payment pm)
            {
                return (pm != null && pm.PMT_Registrations > 0 && pm.PMT_Paid);
            }
            public static Boolean OrderComplete(Order o)
            {
                if (o != null)
                {
                    return PaymentComplete(o.Payments.SingleOrDefault(Predicates.CurrentPayment()));
                }
                else
                {
                    return false;
                }
            }
            public static Boolean OrderComplete(UserLogin ul)
            {
                Order o = Data.GetCurrentOrder(ul.Parent);
                return OrderComplete(o);
            }

            public static String ThumbnailFilename(String Filename)
            {
                return Filename.Substring(0, 6) + "thumbnails/" + Filename.Substring(6);
            }

            public static String FancyPhone(Int32 Phone)
            {
                return FancyPhone(Phone.ToString());
            }
            public static String FancyPhone(String Phone)
            {
                return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6, 4);
            }
            public static String StripPhone(String Phone)
            {
                String rt = "";
                for (int i = 0; i < Phone.Length; i++)
                {
                    if (Char.IsDigit(Phone[i]))
                        rt += Phone[i];
                }
                return rt;
            }

            public static Int32 Age(DateTime birthdate)
            {
                if (birthdate.DayOfYear < DateTime.Now.DayOfYear)
                {
                    return (DateTime.Now.Year - birthdate.Year);
                }
                return (DateTime.Now.Year - birthdate.Year - 1);
            }
            public static Int32 AgeOnReg(DateTime birthdate)
            {
                if (birthdate.DayOfYear < EndDate.DayOfYear)
                {
                    return (EndDate.Year - birthdate.Year);
                }
                return (EndDate.Year - birthdate.Year - 1);
            }

            public static Int32 GetTeamMin(Int32 TeamID)
            {
                return Data.db.Teams.Single(m => m.TEM_TeamID == TeamID).TEM_MinAge;
            }
            public static void SetTeamMin(Int32 TeamID, Int32 Value)
            {
                Team te = Data.db.Teams.Single(m => m.TEM_TeamID == TeamID);
                te.TEM_MinAge = Value;
            }
            public static Int32 GetTeamMax(Int32 TeamID)
            {
                return Data.db.Teams.Single(m => m.TEM_TeamID == TeamID).TEM_MaxAge;
            }
            public static void SetTeamMax(Int32 TeamID, Int32 Value)
            {
                Team te = Data.db.Teams.Single(m => m.TEM_TeamID == TeamID);
                te.TEM_MaxAge = Value;
            }

            /*public static String TeamName(DateTime birthdate)
            {
                return TeamName(TeamID(birthdate));
            }
            public static String TeamName(Int32 TeamID)
            {
                String team = "";
                switch (TeamID)
                {
                    case 0:
                        team = "PeeWees";
                        break;
                    case 1:
                        team = "Middlers";
                        break;
                    case 2:
                        team = "Intermediates";
                        break;
                    case 3:
                        team = "Juniors";
                        break;
                    case 4:
                        team = "Seniors";
                        break;
                    case 5:
                        team = "Girls Team";
                        break;
                    default:
                        team = "None";
                        break;
                }
                return team;
            }*/
            public static List<Team> AllTeams(DateTime birthdate, bool? Gender = null)
            {
                Int32 age = AgeOnReg(birthdate);
                return AllTeams(age, Gender);
            }
            public static List<Team> AllTeams(Int32 age, bool? Gender = null)
            {
                return Data.db.Teams.Where(m => m.TEM_MinAge <= age && m.TEM_MaxAge >= age && (!m.TEM_Gender.HasValue || m.TEM_Gender.Value == Gender) && m.TEM_Enabled).OrderBy(m => m.TEM_Gender.HasValue).ToList();
            }
            public static int TeamID(DateTime birthdate, bool? Gender = null)
            {
                Int32 age = AgeOnReg(birthdate);
                return TeamID(age, Gender);
            }
            public static int TeamID(Int32 age, bool? Gender = null)
            {
                List<Team> match = Data.db.Teams.Where(m => m.TEM_MinAge <= age && m.TEM_MaxAge >= age && m.TEM_Enabled).ToList();
                if (match.Any(m => m.TEM_Gender == Gender))
                {
                    return match.Where(m => m.TEM_Gender == Gender).First().TEM_TeamID;
                }
                else if (match.Any(m => !m.TEM_Gender.HasValue))
                {
                    return match.First().TEM_TeamID;
                }
                return -1;
            }

        #endregion

        #region UtilityMethods

            public static Boolean SendEmail(String from, String to, String subject, String message)
            {
                try
                {
                    SmtpClient client = new SmtpClient("localhost", 25);
                    client.Credentials = new NetworkCredential("skhss@skhomeschoolsports.com", "Chickens1!");
                    MailMessage me = new MailMessage(from, to, subject, message);
                    me.IsBodyHtml = true;
                    client.Send(me);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            public static String Validate(String useremail, String password)
            {
                SKHSSMembershipProvider sp = new SKHSSMembershipProvider();
                if (useremail == "")
                {
                    return "Username cannot be blank";
                }
                if (password == "")
                {
                    return "Password cannot be blank";
                }
                SKHSSUser login;
                try
                {
                    login = sp.GetUser(useremail, useremail);
                }
                catch (Exception e)
                {
                    return "Username/Email - " + e.Message;
                }
                if (login == null) { return "Username/Email was not found"; }
                if (login.Password.CompareTo(password) != 0) { return "Password is incorrect"; }
                if (login.UserLogin.USR_Activated != true) { return "account-not-activated" + (login.UserLogin.USR_Email == "" ? "#noemail" : ""); }
                FormsAuthentication.SetAuthCookie(login.Username, true);
                return "loggedin";
            }
            public static void Log(String text)
            {
                SKHSSEntities db = Data.db;
                Log l = new Log();
                l.LOG_RecordID = Guid.NewGuid();
                l.LOG_Text = text;
                db.Logs.Add(l);
                db.SaveChanges();
            }
            public static RegisterPageModel CreateReg(Payment pm)
            {
                RegisterPageModel rp = new RegisterPageModel();
                Parent p = new Parent();
                p.PRT_RecordID = pm.Order.ORD_PRT_RecordID;
                rp.Parent = p;
                rp.Medical = true;
                rp.Volunteer = pm.Order.ORD_Volunteer;
                foreach (Teammate t in pm.Teammates)
                {
                    RegisterPageChild rpc = new RegisterPageChild();
                    rpc.Reg = true;
                    rpc.Uni = t.TMT_ShirtID.HasValue;
                    rpc.UniSize = (rpc.Uni ? t.TMT_ShirtID.Value : -1);
                    rpc.TeamSelect = t.TMT_TEM_TeamID;
                    Child c = new Child();
                    c.CLD_RecordID = t.TMT_CLD_RecordID;
                    rpc.Child = c;
                    rp.RegChildren.Add(rpc);
                }
                return rp;
            }
            public static RegisterPageModel CreateReg(ObjectModelParent psave)
            {
                Guid ParentID = Guid.Parse(psave.ParentRecordID);
                RegisterPageModel rp = new RegisterPageModel();
                Parent p = Data.GetP(m => m.PRT_RecordID == ParentID);
                rp.Parent = p;
                foreach (ObjectModelChild oc in psave.Children)
                {
                    Guid ChildID = Guid.Empty;
                    Guid.TryParse(oc.ChildRecordID, out ChildID);
                    RegisterPageChild pc = new RegisterPageChild();
                    ObjectModelTeammate ot = oc.Teammates.FirstOrDefault();
                    pc.Reg = ot != null;
                    if (pc.Reg)
                    {
                        pc.Uni = ot.ShirtID != "-1";
                        pc.UniSize = Int32.Parse(ot.ShirtID);
                        pc.TeamSelect = Int32.Parse(ot.TeamID);
                        if (ChildID != Guid.Empty)
                        {pc.Child = Data.GetC(m => m.CLD_RecordID == ChildID);}
                    }
                    rp.RegChildren.Add(pc);
                }
                return rp;
            }
            public static RegisterPageModel CreateReg(Parent pt)
            {
                RegisterPageModel rp = new RegisterPageModel();
                Parent p = Data.GetP(m => m.PRT_RecordID == pt.PRT_RecordID);
                rp.Parent = p;
                foreach (Child c in p.Children)
                {
                    RegisterPageChild rpc = new RegisterPageChild();
                    Teammate t = Data.GetCurrentTeammate(c);
                    rpc.Reg = t != null;
                    if (rpc.Reg)
                    {
                        rpc.Uni = t.TMT_ShirtID.HasValue;
                        if (rpc.Uni)
                        {rpc.UniSize = t.TMT_ShirtID.Value;}
                        rpc.TeamSelect = t.TMT_TEM_TeamID;
                        rpc.Child = c;
                    }
                    rp.RegChildren.Add(rpc);
                }
                return rp;
            }

        #endregion

        #region Roles

            public static Int32 HighestRole(String UserIdentity)
            {
                return Int32.Parse(Data.GetLogin(UserIdentity).USR_ROL_RoleID.ToString().Last().ToString());
            }
            public static Boolean IsInRole(UserLogin ul, Int32 Role)
            {
                return ul.USR_ROL_RoleID.ToString().Contains(Role.ToString());
            }
            public static Boolean IsInRole(String ui, Int32 Role)
            {
                if (ui == null || ui == "")
                {
                    return false;
                }
                return IsInRole(Data.GetLogin(ui), Role);
            }

            public static Boolean IsInAnyRole(UserLogin ul, Int32[] Roles)
            {
                String ri = ul.USR_ROL_RoleID.ToString();
                foreach (Int32 r in Roles)
                {
                    if (IsInRole(ul, r))
                    {
                        return true;
                    }
                }
                return false;
            }
            public static Boolean IsInAnyRole(String ui, Int32[] Roles)
            {
                if (ui == null || ui == "")
                {
                    return false;
                }
                return IsInAnyRole(Data.GetLogin(ui), Roles);
            }

            public static Boolean IsInRoleOrHigher(UserLogin ul, Int32 Role)
            {
                if (Int32.Parse(ul.USR_ROL_RoleID.ToString().Last().ToString()) <= Role)
                {
                    return true;
                }
                return false;
            }
            public static Boolean IsInRoleOrHigher(String ui, Int32 Role)
            {
                if (ui == null || ui == "")
                {
                    return false;
                }
                return IsInRoleOrHigher(Data.GetLogin(ui), Role);
            }

            public static Boolean IsInRoleOrLower(UserLogin ul, Int32 Role)
            {
                if (Int32.Parse(ul.USR_ROL_RoleID.ToString()[0].ToString()) >= Role)
                {
                    return true;
                }
                return false;
            }
            public static Boolean IsInRoleOrLower(String ui, Int32 Role)
            {
                if (ui == null || ui == "")
                {
                    return false;
                }
                return IsInRoleOrLower(Data.GetLogin(ui), Role);
            }

            public static void AddRole(Guid ui, Int32 Role)
            {
                UserLogin ul = Data.GetUL(m => m.USR_RecordID == ui);
                String sRole = ul.USR_ROL_RoleID.ToString();
                if (sRole.IndexOf(Role.ToString()) == -1)
                {
                    for (int i = sRole.Length - 1; i >= 0; i--)
                    {
                        if (Int32.Parse(sRole[i].ToString()) > Role)
                        {
                            ul.USR_ROL_RoleID = Int32.Parse(sRole.Insert(i + 1, Role.ToString()));
                        }
                        else if(i == 0)
                        {
                            ul.USR_ROL_RoleID = Int32.Parse(sRole.Insert(i, Role.ToString()));
                        }
                    }
                    Data.SaveDB();
                }
            }
            public static void RemoveRole(Guid ui, Int32 Role)
            {
                UserLogin ul = Data.GetUL(m => m.USR_RecordID == ui);
                String sRole = ul.USR_ROL_RoleID.ToString();
                sRole = sRole.Replace(Role.ToString(), "");
                ul.USR_ROL_RoleID = Int32.Parse(sRole);
                Data.SaveDB();
            }

        #endregion
    }
}