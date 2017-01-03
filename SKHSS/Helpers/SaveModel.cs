using SKHSS.Authentication;
using SKHSS.Models;
using SKHSS.Models.PageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace SKHSS.Helpers
{
    public class SaveModel
    {
        private static JavaScriptSerializer js = new JavaScriptSerializer(new SimpleTypeResolver());
        public static UserLogin SaveObject(String data)
        {
            ObjectModel a = js.Deserialize<ObjectModel>(data);
            return SaveObject(a);
        }
        public static UserLogin SaveObject(ObjectModel save)
        {
            UserLogin result = new UserLogin();
            SKHSSMembershipProvider sp = new SKHSSMembershipProvider();
            Boolean newuser = false;
            Boolean uu = false;
            switch(save.Mode){
                case "Delete":
                    DeleteUser(save);
                break;
                case "Activate":
                    Guid arid = Guid.Parse(save.UserRecordID);
                    UserLogin acl = Data.GetUL(m => m.USR_RecordID == arid);
                    acl.USR_Activated = Boolean.Parse(save.Activated);
                    Data.SaveDB();
                break;
                case "Approve":
                    Guid brid = Guid.Parse(save.UserRecordID);
                    UserLogin apl = Data.GetUL(m => m.USR_RecordID == brid);
                    List<Teammate> ts = Data.GetCurrentTeammates(apl.Parent);
                    foreach (Teammate t in ts)
                    {
                        t.TMT_Accepted = Boolean.Parse(save.Approved);
                    }
                    apl.USR_Approved = Boolean.Parse(save.Approved);
                    Data.SaveDB();
                break;
                case "Paid":
                    Guid crid = Guid.Parse(save.UserRecordID);
                    UserLogin pdl = Data.GetUL(m => m.USR_RecordID == crid);
                    Payment cur = Data.GetCurrentPayment(pdl.Parent);
                    if(cur==null){
                        Fix.FixError(Data.GetCurrentOrder(pdl.Parent).ORD_RecordID,2);
                        cur = Data.GetCurrentPayment(pdl.Parent);
                    }
                    Guid pmrid = cur.PMT_RecordID;
                    Payment pm = Data.GetPM(m => m.PMT_RecordID == pmrid);
                    pm.PMT_Paid = Boolean.Parse(save.Paid);
                    Data.SaveDB();
                break;
                default:
                {
                    Guid UserRecordID = Guid.Empty;
                    Guid.TryParse(save.UserRecordID, out UserRecordID);
                    if (UserRecordID == Guid.Empty)
                    {
                        newuser = true;
                    }
                    if (!v(save.RoleID)) { save.RoleID = Definitions.ParentRole.ToString(); }
                    //if (v(save.Phone)) { save.Phone = Global.StripPhone(save.Phone); }
                    if (save.Mode == "AdminCreate")
                    {
                        #region AdminCreate
                        save.Username = save.Parent.LastName.Substring(0, 4) + (v(save.Parent.FatherName) ? save.Parent.FatherName : save.Parent.MotherName);
                        save.Username = save.Username.Replace(" ", "");
                        if (save.Username.Length > 15)
                        {
                            save.Username = save.Username.Substring(0, 15);
                        }
                        Boolean u = true;
                        Int32 i = 0;
                        String iu = "";
                        while (u)
                        {
                            u = Data.GetULs().Any(m => m.USR_Username == (save.Username + iu));
                            if (u)
                            {
                                iu = i.ToString();
                                i++;
                            }
                        }
                        save.Username += iu;
                        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        var random = new Random();
                        save.Password = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                        #endregion
                    }
                    if (v(save.Parent.PrimaryEmail)) { save.PrimaryEmail = save.Parent.PrimaryEmail; }
                    if (v(save.Parent.PrimaryPhone)) { save.PrimaryPhone = save.Parent.PrimaryPhone; }
                    if (v(save.PrimaryEmail)) { save.PrimaryEmail = Boolean.Parse(save.PrimaryEmail)?save.Parent.MotherEmail:save.Parent.FatherEmail; } else { }
                    if (v(save.PrimaryPhone)) { save.PrimaryPhone = Boolean.Parse(save.PrimaryPhone)?save.Parent.MotherPhone:save.Parent.FatherPhone; } else { }
                    UserLogin l = (newuser ? sp.CreateUser(save.Username, save.Password, save.PrimaryEmail, save.PrimaryPhone, Int32.Parse(save.RoleID)).UserLogin : Data.GetUL(m => m.USR_RecordID == UserRecordID));
                    result = l;
                    UserRecordID = l.USR_RecordID;
                    l = Data.GetUL(m => m.USR_RecordID == UserRecordID);
                    if (v(save.Activated)) { l.USR_Activated = Boolean.Parse(save.Activated); }
                    if (v(save.Approved)) { l.USR_Approved = Boolean.Parse(save.Approved); }
                    if (v(save.Temp)) { l.USR_Temp = Boolean.Parse(save.Temp); }
                    if (v(save.Test)) { l.USR_Test = Boolean.Parse(save.Test); }
                    if (save.Mode == "AdminCreate")
                    {
                        l.USR_Temp = true;
                        if (save.Parent != null)
                        {
                            save.Parent.Mode = "AdminCreate";
                        }
                    }
                    Data.SaveDB();
                    if (!newuser)
                    {
                        if (v(save.RoleID)) { l.USR_ROL_RoleID = Int32.Parse(save.RoleID); }
                        if (v(save.Username)) {
                            if (HttpContext.Current.User.Identity.Name == l.USR_Username)
                            {
                                uu = true;
                            }
                            l.USR_Username = save.Username;
                        }
                        //if (v(save.Email)) { l.USR_Email = save.Email; }
                        if (v(save.Password)) { l.USR_Password = save.Password; }
                        /*if (v(save.Phone))
                        {
                            if (save.Phone.Length == 10) { l.USR_Phone = save.Phone; }
                            else { l.USR_Phone = save.Phone.Substring(1, 3) + save.Phone.Substring(6, 3) + save.Phone.Substring(10, 4); }
                        }*/
                        Data.SaveDB();
                    }
                    if (v(save.Parent))
                    {
                        SaveObjectParent(save.Parent, UserRecordID);
                    }
                    Data.SaveDB();
                    break;
                }
            }
            if (newuser && (!v(save.Mode) || save.Mode != "NoActivation"))
            {
                SendActivation(result);
            }
            if (uu)
            {
                System.Web.Security.FormsAuthentication.SignOut();
                Global.Validate(save.Username, Data.GetUL(m => m.USR_Username == save.Username).USR_Password);
            }
            Data.SaveDB();
            return result;
        }
        public static Parent SaveObjectParent(String data, Guid UserRecordID = default(Guid))
        {
            ObjectModelParent a = js.Deserialize<ObjectModelParent>(data);
            return SaveObjectParent(a, UserRecordID);
        }
        public static Parent SaveObjectParent(ObjectModelParent psave, Guid UserRecordID = default(Guid))
        {
            Parent result = new Parent();
            Guid ParentRecordID = Guid.Empty;
            Guid.TryParse(psave.ParentRecordID, out ParentRecordID);
            if (v(psave.Mode) && psave.Mode == "AdminReg")
            {
                SaveAdminReg(psave);
            }
            else if (v(psave.Mode) && psave.Mode == "Delete")
            {
                DeleteObjectParent(psave);
            }
            else
            {
                Boolean newparent = false;
                if (ParentRecordID == Guid.Empty && UserRecordID != Guid.Empty)
                {
                    newparent = true;
                }
                Parent p = ((ParentRecordID == Guid.Empty) ? new Parent() : Data.GetP(m => m.PRT_RecordID == ParentRecordID));
                result = p;

                if (v(psave.FatherName,true)) { 
                    p.PRT_FatherName = psave.FatherName; 
                }
                Contact fc = p.Contacts.SingleOrDefault(m=>m.CON_PRTID==false);
                if (!d(psave.FatherEmail) || !d(psave.FatherPhone)){fc = fc ?? new Contact();}
                else if (d(psave.FatherEmail) && d(psave.FatherPhone)){Data.GetCTs().Remove(fc);}
                if (v(psave.FatherEmail, true)) { fc.CON_Email = psave.FatherEmail; }
                if (v(psave.FatherPhone, true)) { fc.CON_Phone = psave.FatherPhone; }

                if (v(psave.MotherName, true)) { p.PRT_MotherName = psave.MotherName; }
                Contact mc = p.Contacts.SingleOrDefault(m => m.CON_PRTID == true);
                if (!d(psave.MotherEmail) || !d(psave.MotherPhone)) { mc = mc ?? new Contact(); }
                else if (d(psave.MotherEmail) && d(psave.MotherPhone)) { Data.GetCTs().Remove(mc); }

                if (v(psave.MotherEmail, true)){ mc.CON_Email = psave.MotherEmail; }
                if (v(psave.MotherPhone, true)){ mc.CON_Phone = psave.MotherPhone; }

                if (v(psave.PrimaryEmail)) { p.PRT_PrimaryEmail =  psave.PrimaryEmail; }
                if (v(psave.PrimaryPhone)) { p.PRT_PrimaryPhone =  psave.PrimaryPhone; }

                if (v(psave.LastName)) { p.PRT_LastName = psave.LastName; }
                if (v(psave.Address)) { p.PRT_Address = psave.Address; }
                if (v(psave.City)) { p.PRT_City = psave.City; }
                if (v(psave.Zipcode)) { p.PRT_Zipcode = psave.Zipcode; }
                if (newparent)
                {
                    p.PRT_USR_RecordID = UserRecordID;
                    p.PRT_RecordID = Guid.NewGuid();
                    ParentRecordID = p.PRT_RecordID;
                    Data.GetUL(m => m.USR_RecordID == UserRecordID).USR_PRT_RecordID = ParentRecordID;
                    Data.GetPs().Add(p);
                    Data.SaveDB();
                }
                /*if (psave.Mode == "AdminCreate")
                {
                    psave.ParentRecordID = ParentRecordID.ToString();
                    DefaultOrder(psave);
                }*/
                if (v(psave.Approve))
                {
                    Order o = p.Orders.Single(Predicates.CurrentOrder());
                    o.ORD_Approved = true;
                    p.UserLogin.USR_Approved = true;
                }
                Data.SaveDB();

                if (psave.Children != null && psave.Mode != "NoRec")
                {
                    foreach (ObjectModelChild oc in psave.Children)
                    {
                        /*if (psave.Mode == "AdminCreate")
                        {
                            oc.Mode = "AdminCreate";
                        }*/
                        Guid ChildID = SaveObjectChild(oc, ParentRecordID).CLD_RecordID;
                    }
                }
                /*if (psave.Mode == "AdminCreate")
                {
                    SaveObjectReg(Global.CreateReg(p), true);
                }*/
            }
            return result;
        }
        public static Child SaveObjectChild(String data, Guid ParentRecordID = default(Guid))
        {
            ObjectModelChild a = js.Deserialize<ObjectModelChild>(data);
            return SaveObjectChild(a, ParentRecordID);
        }
        public static Child SaveObjectChild(ObjectModelChild oc, Guid ParentRecordID = default(Guid))
        {
            Child result = new Child();
            if (v(oc.Mode) && oc.Mode == "Delete")
            {
                DeleteObjectChild(oc);
            }
            else
            {
                Boolean newchild = false;
                Guid ChildRecordID = Guid.Empty;
                Guid.TryParse(oc.ChildRecordID, out ChildRecordID);
                if (ChildRecordID == Guid.Empty)
                {
                    ChildRecordID = Guid.NewGuid();
                    newchild = true;
                }
                Child c = (newchild ? new Child() : Data.GetC(m => m.CLD_RecordID == ChildRecordID));
                result = c;
                if (v(oc.FirstName)) { c.CLD_FirstName = oc.FirstName; }
                if (v(oc.LastName,true)) { c.CLD_LastName = oc.LastName; }
                if (v(oc.Birthdate)) { c.CLD_Birthdate = DateTime.Parse(oc.Birthdate); }
                if (v(oc.Gender)) { c.CLD_Gender = (oc.Gender == "True" || oc.Gender == "true" || oc.Gender == "1" || oc.Gender == "Female" || oc.Gender == "female" ? true : false); }
                if (v(oc.Notes,true)) { 
                    c.CLD_Notes = oc.Notes; 
                }
                if (newchild)
                {
                    c.CLD_PRT_RecordID = ParentRecordID;
                    c.CLD_RecordID = Guid.NewGuid();
                    ChildRecordID = c.CLD_RecordID;
                    Data.GetCs().Add(c);
                }
                Data.SaveDB();
                if (oc.Teammates != null && oc.Mode != "NoRec")
                {
                    foreach (ObjectModelTeammate ot in oc.Teammates)
                    {
                        SaveObjectTeammate(ot, ChildRecordID);
                    }
                }
                
            }
            return result;
        }
        public static Teammate SaveObjectTeammate(String data, Guid ChildRecordID = default(Guid))
        {
            ObjectModelTeammate a = js.Deserialize<ObjectModelTeammate>(data);
            return SaveObjectTeammate(a, ChildRecordID);
        }
        public static Teammate SaveObjectTeammate(ObjectModelTeammate ot, Guid ChildRecordID = default(Guid))
        {
            Teammate result = new Teammate();
            if (v(ot.Mode) && ot.Mode == "Delete")
            {
                DeleteObjectTeammate(ot);
            }
            else
            {
                Boolean newteammate = false;
                Guid TeammateRecordID = Guid.Empty;
                Guid.TryParse(ot.TeammateRecordID, out TeammateRecordID);
                if (TeammateRecordID == Guid.Empty)
                {
                    TeammateRecordID = Guid.NewGuid();
                    newteammate = true;
                }
                Teammate t = (newteammate ? new Teammate() : Data.GetT(m => m.TMT_RecordID == TeammateRecordID));
                
                result = t;
                if (v(ot.Year)) { t.TMT_Year = Int32.Parse(ot.Year); }
                if (v(ot.SeasonID)) { t.TMT_SeasonID = Int32.Parse(ot.SeasonID); }
                if (v(ot.TeamID)) { t.TMT_TEM_TeamID = Int32.Parse(ot.TeamID); }
                if (v(ot.ShirtID) && ot.ShirtID != "-1") { t.TMT_ShirtID = Int32.Parse(ot.ShirtID); }
                else if(ot.ShirtID == "-1") { t.TMT_ShirtID = null; }
                if (v(ot.Accepted)) { t.TMT_Accepted = Boolean.Parse(ot.Accepted); }
                if (v(ot.PicID)) { t.TMT_PIC_RecordID = Guid.Parse(ot.PicID); }
                if (newteammate)
                {
                    t.TMT_PMT_RecordID = Data.GetCurrentPayment(Data.GetCurrentOrder(Data.GetC(m => m.CLD_RecordID == ChildRecordID).Parent)).PMT_RecordID;
                    t.TMT_CLD_RecordID = ChildRecordID;
                    t.TMT_RecordID = Guid.NewGuid();
                    Data.GetTs().Add(t);
                }
                Data.SaveDB();
            }
            return result;
        }
        public static Coach SaveObjectCoach(String data, Guid UserRecordID = default(Guid))
        {
            ObjectModelCoach a = js.Deserialize<ObjectModelCoach>(data);
            return SaveObjectCoach(a, UserRecordID);
        }
        public static Coach SaveObjectCoach(ObjectModelCoach oc, Guid UserRecordID = default(Guid))
        {
            Coach result = new Coach();
            if (v(oc.Mode) && oc.Mode == "Delete")
            {
                DeleteObjectCoach(oc);
            }
            else
            {
                Boolean prtid = Int32.Parse(oc.PRTID) == 1;
                Int32 teamid = Int32.Parse(oc.TeamID);
                if (Data.GetCH(m => m.CCH_USR_RecordID == UserRecordID && m.CCH_PRTID == prtid && m.CCH_TEM_TeamID == teamid) != null)
                {
                    return new Coach();
                }
                Boolean newcoach = false;
                Guid CoachRecordID = Guid.Empty;
                Guid.TryParse(oc.CoachRecordID, out CoachRecordID);
                if (CoachRecordID == Guid.Empty)
                {
                    CoachRecordID = Guid.NewGuid();
                    newcoach = true;
                }
                Coach c = (newcoach ? new Coach() : Data.GetCH(m => m.CCH_RecordID == CoachRecordID));
                result = c;
                if (v(oc.TeamID)) { c.CCH_TEM_TeamID = Int32.Parse(oc.TeamID); }
                if (v(oc.PicID)) { c.CCH_PIC_RecordID = Guid.Parse(oc.PicID); }
                if (v(oc.PRTID)) { c.CCH_PRTID = Int32.Parse(oc.PRTID) == 1; }
                if (v(oc.PositionID)) { c.CCH_PositionID = Int32.Parse(oc.PositionID); }

                if (newcoach)
                {
                    c.CCH_USR_RecordID = UserRecordID;
                    c.CCH_RecordID = CoachRecordID;
                    Global.AddRole(UserRecordID, Definitions.CoachRole);
                    Data.GetCHs().Add(c);
                }
                Data.SaveDB();
            }
            return result;
        }

        public static Order SaveObjectOrder(String data, Guid ParentRecordID = default(Guid))
        {
            ObjectModelOrder a = js.Deserialize<ObjectModelOrder>(data);
            return SaveObjectOrder(a, ParentRecordID);
        }
        public static Order SaveObjectOrder(ObjectModelOrder oo, Guid ParentRecordID = default(Guid))
        {
            Order result = new Order();
            if (v(oo.Mode) && oo.Mode == "Delete")
            {
                //DeleteObjectOrder(oc);
            }
            else
            {
                Boolean neworder = false;
                Guid OrderRecordID = Guid.Empty;
                Guid.TryParse(oo.OrderRecordID, out OrderRecordID);
                if (OrderRecordID == Guid.Empty)
                {
                    OrderRecordID = Guid.NewGuid();
                    neworder = true;
                }
                Order o = (neworder ? new Order() : Data.GetO(m => m.ORD_RecordID == OrderRecordID));
                result = o;
                if (v(oo.SeasonID)) { o.ORD_SeasonID= Int32.Parse(oo.SeasonID); }
                if (v(oo.Year)) { o.ORD_Year = Int32.Parse(oo.Year); }
                if (v(oo.Date)) { o.ORD_Date = DateTime.Parse(oo.Date); }
                if (v(oo.Volunteer)) { o.ORD_Volunteer = Int32.Parse(oo.Volunteer); }
                if (v(oo.Approved)) { o.ORD_Approved = Boolean.Parse(oo.Approved); }

                if (neworder)
                {
                    o.ORD_PRT_RecordID = ParentRecordID;
                    o.ORD_RecordID = Guid.NewGuid();
                    Data.GetOs().Add(o);
                }
                Data.SaveDB();
            }
            return result;
        }
        public static Payment SaveObjectPayment(String data, Guid OrderRecordID = default(Guid))
        {
            ObjectModelPayment a = js.Deserialize<ObjectModelPayment>(data);
            return SaveObjectPayment(a, OrderRecordID);
        }
        public static Payment SaveObjectPayment(ObjectModelPayment opm, Guid OrderRecordID = default(Guid))
        {
            Payment result = new Payment();
            if (v(opm.Mode) && opm.Mode == "Delete")
            {
                //DeleteObjectOrder(oc);
            }
            else
            {
                Boolean newpayment = false;
                Guid PaymentRecordID = Guid.Empty;
                Guid.TryParse(opm.PaymentRecordID, out PaymentRecordID);
                if (PaymentRecordID == Guid.Empty)
                {
                    PaymentRecordID = Guid.NewGuid();
                    newpayment = true;
                }
                Payment pm = (newpayment ? new Payment() : Data.GetPM(m => m.PMT_RecordID == PaymentRecordID));
                result = pm;
                if (v(opm.Current)) { pm.PMT_Current = Boolean.Parse(opm.Current); }
                if (v(opm.Date)) { pm.PMT_Date = DateTime.Parse(opm.Date); }
                if (v(opm.Registrations)) { pm.PMT_Registrations = Int32.Parse(opm.Registrations); }
                if (v(opm.Uniforms)) { pm.PMT_Uniforms = Int32.Parse(opm.Uniforms); }
                if (v(opm.Total)) { pm.PMT_Total = Int32.Parse(opm.Total); }
                if (v(opm.LateFee)) { pm.PMT_LateFee = Boolean.Parse(opm.LateFee); }
                if (v(opm.Paid)) { pm.PMT_Paid = Boolean.Parse(opm.Paid); }
                if (v(opm.TransactionID)) { pm.PMT_IPN_TransactionID = opm.TransactionID; }
                if (v(opm.TransactionDate)) { pm.PMT_TransactionDate = DateTime.Parse(opm.TransactionDate); }

                if (newpayment)
                {
                    pm.PMT_ORD_RecordID = OrderRecordID;
                    pm.PMT_RecordID = Guid.NewGuid();
                    
                    Data.GetPMs().Add(pm);
                }
                Data.SaveDB();
            }
            return result;
        }

        public static Cache SaveCache(String data)
        {
            CacheModel a = js.Deserialize<CacheModel>(data);
            return SaveCache(a);
        }
        public static Cache SaveCache(CacheModel model)
        {
            //Guid g = new Guid();
            DeleteCache(model.CAC_Type, Data.db);
            Cache c = new Cache();
            c = new Cache();
            c.CAC_RecordID = Guid.NewGuid();
            c.CAC_Date = DateTime.Now;
            c.CAC_Type = model.CAC_Type;
            Data.GetCCs().Add(c);
            Data.SaveDB();
            foreach (CacheItemModel cim in model.items)
            {
                CacheItem ci = new CacheItem();
                ci.CCI_CAC_RecordID = c.CAC_RecordID;
                ci.CCI_RecordID = Guid.NewGuid();
                ci.CCI_Title = cim.title;
                ci.CCI_Published = DateTime.Parse(cim.published);
                ci.CCI_Content = cim.content;
                ci.CCI_Labels = String.Join(",", cim.labels);
                Data.GetCCIs().Add(ci);
            }
            Data.SaveDB();
            return c;
        }
        public static Parent SaveObjectReg(String data)
        {
            RegisterPageModel a = js.Deserialize<RegisterPageModel>(data);
            return SaveObjectReg(a);
        }
        public static Parent SaveObjectReg(RegisterPageModel rp, Boolean AdminCreate = false)
        {
            Order o = Data.GetCurrentOrder(rp.Parent);
            if (rp.RegChildren.Count(m => m.Reg == true) > 0)
            {
                if (o == null)
                {
                    o = new Order();
                    o.ORD_RecordID = Guid.NewGuid();
                    o.ORD_PRT_RecordID = rp.Parent.PRT_RecordID;
                    o.ORD_Date = DateTime.Now;
                    o.ORD_SeasonID = Global.CurrentSeasonID;
                    o.ORD_Year = Global.CurrentYear;
                    Data.GetOs().Add(o);
                    Data.SaveDB();
                }
                o.ORD_Volunteer = rp.Volunteer;
                Data.SaveDB();
                Payment pm = Data.GetCurrentPayment(o);
                if (pm == null || pm.PMT_Paid)
                {
                    if(pm != null && pm.PMT_Paid){
                        pm.PMT_Current = false;
                        Data.SaveDB();
                        Data.RefreshDB();
                    }
                    pm = new Payment();
                    pm.PMT_RecordID = Guid.NewGuid();
                    pm.PMT_ORD_RecordID = o.ORD_RecordID;
                    pm.PMT_Date = DateTime.Now;
                    pm.PMT_Current = true;
                    Data.GetPMs().Add(pm);
                    Data.SaveDB();
                }
                pm.PMT_Registrations = rp.RegChildren.Count(m => m.Reg == true);
                pm.PMT_Uniforms = rp.RegChildren.Count(m => m.Uni == true && m.Reg == true);
                pm.PMT_LateFee = (!Global.IsLateFee ? false : (o.Payments.Any(m => m.PMT_LateFee == true) ? (o.Payments.Count(m => m.PMT_LateFee == true) > 1 ? false : pm.PMT_LateFee) : true));
                pm.PMT_Total = Global.PaymentTotal(pm);
                Data.SaveDB();
                if (!AdminCreate)
                {
                    foreach (RegisterPageChild rc in rp.RegChildren)
                    {
                        Teammate t = Data.GetCurrentTeammate(rc.Child);
                        Child c = Data.GetC(m => m.CLD_RecordID == rc.Child.CLD_RecordID);
                        if (rc.Reg)
                        {
                            if (t == null)
                            {
                                t = new Teammate();
                                t.TMT_RecordID = Guid.NewGuid();
                                t.TMT_CLD_RecordID = rc.Child.CLD_RecordID;
                                t.TMT_PMT_RecordID = pm.PMT_RecordID;
                                t.TMT_SeasonID = Global.CurrentSeasonID;
                                t.TMT_Year = Global.CurrentYear;
                                Data.GetTs().Add(t);
                                Data.SaveDB();
                            }
                            t.TMT_ShirtID = (rc.Uni ? rc.UniSize : (Int32?)null);
                            Int32 age = Global.AgeOnReg(c.CLD_Birthdate);
                            if (rc.TeamSelect == -1) { rc.TeamSelect = Global.TeamID(c.CLD_Birthdate); }
                            t.TMT_TEM_TeamID = rc.TeamSelect;
                            Data.SaveDB();
                        }
                        else if (t != null && !t.Payment.PMT_Paid)
                        {
                            RecursiveDeleteTeammate(t);
                        }
                    }
                }
            }
            else
            {
                if (o != null && o.Payments.Count(m => m.PMT_Paid == true) == 0)
                {
                    RecursiveDeleteOrder(o);
                }
                else if(o != null)
                {
                    Payment pm = Data.GetCurrentPayment(o);
                    if (pm == null)
                    {
                        pm = o.Payments.OrderByDescending(m => m.PMT_TransactionDate).First();
                        pm.PMT_Current = true;
                        Data.SaveDB();
                    }
                    else if (!pm.PMT_Paid)
                    {
                        RecursiveDeletePayment(pm);
                    }
                }
            }
            return rp.Parent;
        }

        public static void SaveAdminReg(ObjectModelParent psave)
        {
            Guid prid = Guid.Parse(psave.ParentRecordID);
            Parent p = Data.GetP(m => m.PRT_RecordID == prid);
            if(psave.Children==null){psave.Children = new List<ObjectModelChild>();}
            //Delete deleted teammates
            foreach (Teammate t in Data.GetCurrentTeammates(p).ToList()) //Might not work if no current teammates
            {
                String crid = t.TMT_CLD_RecordID.ToString();
                if (psave.Children.Count==0||!psave.Children.Any(m => m.ChildRecordID == crid))
                {
                    SDeleteObjectTeammate(t.TMT_RecordID);
                }
            }
            Order o1 = Data.GetCurrentOrder(p);
            if (o1 == null){o1 = CreateDefaultOrder(p);}
            Payment pm1 = Data.GetCurrentPayment(p);
            if (pm1 == null){pm1 = CreateDefaultPayment(o1);}
            //Create new childs/Teammates if necessary and save old childs/teammates
            if (psave.Children.Count > 0)
            {
                foreach (ObjectModelChild oc in psave.Children.ToList())
                {
                    //oc.Teammates.First().Accepted = true.ToString();
                    SaveObjectChild(oc, p.PRT_RecordID);
                }
            }

            Data.RefreshDB();

            Order o = Data.GetCurrentOrder(p);
            Boolean lf = o.Payments.Any(m => m.PMT_LateFee == true);
            if (o != null)
            {
                foreach (Payment pm in o.Payments.ToList())
                {
                    if (pm.Teammates.Count == 0)
                    {
                        SDeleteObjectPayment(pm.PMT_RecordID);
                        o.Payments.Remove(pm);
                    }
                    else
                    {
                        pm.PMT_Registrations = pm.Teammates.Count;
                        pm.PMT_Uniforms = pm.Teammates.Count(m => m.TMT_ShirtID != null);
                        pm.PMT_LateFee = lf; // may have to recalculate whether this ORDER needs a late fee.
                        #region late fee data 
                        //(!Global.IsLateFee ? false : (o.Payments.Any(m => m.PMT_LateFee == true) ? (o.Payments.Count(m => m.PMT_LateFee == true) > 1 ? false : pm.PMT_LateFee) : true));
                        #endregion
                        pm.PMT_Total = Global.PaymentTotal(pm);
                    }
                }
                if (o.Payments.Count == 0)
                {
                    SDeleteObjectOrder(o.ORD_RecordID);
                }
            }
            Data.SaveDB();
            Data.RefreshDB();
        }

        public static void DefaultOrder(ObjectModelParent psave)
        {
            Guid ParentID = Guid.Empty;
            Guid.TryParse(psave.ParentRecordID, out ParentID);
            Parent p = Data.GetP(m => m.PRT_RecordID == ParentID);
            Order o = Data.GetCurrentOrder(p);
            if (o == null)
            {
                o = new Order();
                o.ORD_RecordID = Guid.NewGuid();
                o.ORD_PRT_RecordID = ParentID;
                o.ORD_Date = DateTime.Now;
                o.ORD_SeasonID = Global.CurrentSeasonID;
                o.ORD_Year = Global.CurrentYear;
                Data.GetOs().Add(o);
                Data.SaveDB();
            }
            Payment pm = Data.GetCurrentPayment(o);
            if (pm == null)
            {
                pm = new Payment();
                pm.PMT_RecordID = Guid.NewGuid();
                pm.PMT_ORD_RecordID = o.ORD_RecordID;
                pm.PMT_Current = true;
                Data.GetPMs().Add(pm);
                Data.SaveDB();
            }
            /*else if(pm.PMT_Paid)
            {
                pm.PMT_Paid = false;
                Data.SaveDB();
            }*/
        }

        private static void DeleteUser(ObjectModel obj)
        {
            Guid UserId = Guid.Parse(obj.UserRecordID);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == UserId);
            Boolean wasnull = ul.USR_PRT_RecordID == null;
            ul.USR_PRT_RecordID = null;
            if (!wasnull){Data.SaveDB();}
            Parent pa = Data.GetP(m => m.PRT_USR_RecordID == UserId);
            if (pa != null)
            {
                ObjectModelParent op = new ObjectModelParent();
                op.ParentRecordID = pa.PRT_RecordID.ToString();
                DeleteObjectParent(op);
            }
            List<Coach> cos = Data.GetCHs(m => m.CCH_USR_RecordID == UserId).ToList();
            foreach(Coach co in cos)
            {
                ObjectModelCoach oc = new ObjectModelCoach();
                oc.CoachRecordID = co.CCH_RecordID.ToString();
                DeleteObjectCoach(oc);
            }
            List<Feedback> fbs = Data.db.Feedbacks.Where(m => m.FBK_USR_RecordID == UserId).ToList();
            foreach (Feedback fb in fbs)
            {
                fb.FBK_USR_RecordID = null;
            }
            Data.SaveDB();
            List<Album> albs = Data.db.Albums.Where(m => m.ALB_USR_RecordID == UserId).ToList();
            foreach (Album a in albs)
            {
                List<Picture> pics = Data.db.Pictures.Where(m => m.PIC_ALB_RecordID == a.ALB_RecordID).ToList();
                foreach (Picture p in pics)
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath("~/Content/uploads/") + p.PIC_Filename);
                    Data.db.Pictures.Remove(p);
                }
                Data.db.Albums.Remove(a);
            }
            Data.GetULs().Remove(Data.GetUL(m => m.USR_RecordID == UserId));
            Data.SaveDB();
            Data.RefreshDB();
        }
        private static void DeleteObjectParent(ObjectModelParent obj)
        {
            Guid ParentRecordID = Guid.Parse(obj.ParentRecordID);
            List<Child> cs = Data.GetCs(m => m.CLD_PRT_RecordID == ParentRecordID).ToList();
            foreach (Child c in cs)
            {
                ObjectModelChild oc = new ObjectModelChild();
                oc.ChildRecordID = c.CLD_RecordID.ToString();
                DeleteObjectChild(oc);
            }
            List<Order> os = Data.GetOs(m => m.ORD_PRT_RecordID == ParentRecordID).ToList();
            foreach (Order order in os)
            {
                foreach (Payment pm in order.Payments.ToList())
                {
                    Data.GetPMs().Remove(pm);
                }
                Data.GetOs().Remove(order);
            }
            Data.GetPs().Remove(Data.GetP(m => m.PRT_RecordID == ParentRecordID));
            Data.SaveDB();
        }
        private static void DeleteObjectChild(ObjectModelChild obj)
        {
            Guid ChildId = Guid.Parse(obj.ChildRecordID);
            List<Teammate> ts = Data.GetTs(m => m.TMT_CLD_RecordID == ChildId).ToList();
            foreach (Teammate t in ts)
            {
                ObjectModelTeammate ot = new ObjectModelTeammate();
                ot.TeammateRecordID = t.TMT_RecordID.ToString();
                DeleteObjectTeammate(ot);
            }

            Data.GetCs().Remove(Data.GetC(m => m.CLD_RecordID == ChildId));
            Data.SaveDB();
        }
        private static void DeleteObjectTeammate(ObjectModelTeammate obj)
        {
            Guid TeammateId = Guid.Parse(obj.TeammateRecordID);
            Data.GetTs().Remove(Data.GetT(m => m.TMT_RecordID == TeammateId));
            Data.SaveDB();
        }
        private static void DeleteObjectCoach(ObjectModelCoach obj)
        {
            Guid CoachId = Guid.Parse(obj.CoachRecordID);
            Data.GetCHs().Remove(Data.GetCH(m => m.CCH_RecordID == CoachId));
            Data.SaveDB();
        }

        private static void DeleteCache(String Type, SKHSSEntities db)
        {
            List<Cache> cs = db.Caches.Where(m => m.CAC_Type == Type).ToList();
            foreach (Cache c in cs)
            {
                List<CacheItem> cis = db.CacheItems.Where(m => m.CCI_CAC_RecordID == c.CAC_RecordID).ToList();
                foreach(CacheItem ci in cis){
                    db.CacheItems.Remove(ci);
                }
                db.Caches.Remove(c);
            }
            db.SaveChanges();
            Data.RefreshDB();
        }

        #region CreateDefaults // USE SPARINGLY
        public static Parent CreateDefaultParent(UserLogin iul)
        {
            UserLogin ul = Data.GetUL(m=>m.USR_RecordID == iul.USR_RecordID);
            Parent p = new Parent();
            p.PRT_RecordID = Guid.NewGuid();
            p.PRT_USR_RecordID = ul.USR_RecordID;
            p.PRT_FatherName = "";
            p.PRT_LastName = "";
            p.PRT_Address = "";
            p.PRT_City = "";
            p.PRT_Zipcode = "00000";
            ul.USR_PRT_RecordID = p.PRT_RecordID;
            Data.GetPs().Add(p);
            Data.SaveDB();
            return p;
        }
        public static Order CreateDefaultOrder(Parent inp)
        {
            Order o = new Order();
            o.ORD_RecordID = Guid.NewGuid();
            o.ORD_PRT_RecordID = inp.PRT_RecordID;
            o.ORD_Date = DateTime.Now;
            o.ORD_SeasonID = Global.CurrentSeasonID;
            o.ORD_Year = Global.CurrentYear;
            Data.GetOs().Add(o);
            Data.SaveDB();
            return o;
        }
        public static Payment CreateDefaultPayment(Order ino)
        {
            Payment pm = new Payment();
            pm.PMT_RecordID = Guid.NewGuid();
            pm.PMT_ORD_RecordID = ino.ORD_RecordID;
            pm.PMT_Current = true;
            pm.PMT_Date = DateTime.Now;
            pm.PMT_Paid = false;
            pm.PMT_Registrations = 0;
            pm.PMT_Uniforms = 0;
            pm.PMT_Total = 0;
            pm.PMT_LateFee = false;
            Data.GetPMs().Add(pm);
            Data.SaveDB();
            return pm;
        }
        #endregion

        #region StraightDeletes // USE SPARINGLY

        public static void SDeleteUser(String UserRecordID)
        {
            Guid UserId = Guid.Parse(UserRecordID);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == UserId);
            Data.GetULs().Remove(Data.GetUL(m => m.USR_RecordID == UserId));
            Data.SaveDB();
        }
        public static void SDeleteObjectParent(String ParentRecordID)
        {
            Guid ParentID = Guid.Parse(ParentRecordID);
            Data.GetPs().Remove(Data.GetP(m => m.PRT_RecordID == ParentID));
            Data.SaveDB();
        }
        public static void SDeleteObjectOrder(String OrderRecordID)
        {
            SDeleteObjectOrder(Guid.Parse(OrderRecordID));
        }
        public static void SDeleteObjectOrder(Guid OrderRecordID)
        {
            Data.GetOs().Remove(Data.GetO(m => m.ORD_RecordID == OrderRecordID));
            Data.SaveDB();
        }
        public static void SDeleteObjectPayment(String PaymentRecordID)
        {
            SDeleteObjectPayment(Guid.Parse(PaymentRecordID));
        }
        public static void SDeleteObjectPayment(Guid PaymentRecordID)
        {
            Data.GetPMs().Remove(Data.GetPM(m => m.PMT_RecordID == PaymentRecordID));
            Data.SaveDB();
        }
        public static void SDeleteObjectChild(String ChildRecordID)
        {
            Guid ChildId = Guid.Parse(ChildRecordID);
            Data.GetCs().Remove(Data.GetC(m => m.CLD_RecordID == ChildId));
            Data.SaveDB();
        }
        public static void SDeleteObjectTeammate(String TeammateRecordID)
        {
            SDeleteObjectTeammate(Guid.Parse(TeammateRecordID));
        }
        public static void SDeleteObjectTeammate(Guid TeammateRecordID)
        {
            Data.GetTs().Remove(Data.GetT(m => m.TMT_RecordID == TeammateRecordID));
            Data.SaveDB();
        }
        public static void SDeleteObjectCoach(String CoachRecordID)
        {
            Guid CoachId = Guid.Parse(CoachRecordID);
            Data.GetCHs().Remove(Data.GetCH(m => m.CCH_RecordID == CoachId));
            Data.SaveDB();
        }
        #endregion

        #region RecursiveDeletes
        public static void RecursiveDeleteUser(String id)
        {
            Guid urid = Guid.Parse(id);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == urid);
            RecursiveDeleteUser(ul);
        }
        public static void RecursiveDeleteUser(UserLogin ul)
        {
            if (ul.USR_PRT_RecordID.HasValue)
            {
                Parent p = ul.Parent;
                Data.GetUL(m => m.USR_RecordID == ul.USR_RecordID).USR_PRT_RecordID = null;
                Data.SaveDB();
                RecursiveDeleteParent(p);
            }
            foreach (Coach ch in ul.Coaches.ToList())
            {
                RecursiveDeleteCoach(ch);
            }
            Data.GetULs().Remove(Data.GetUL(m => m.USR_RecordID == ul.USR_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeleteParent(String id)
        {
            Guid prid = Guid.Parse(id);
            Parent p = Data.GetP(m => m.PRT_RecordID == prid);
            RecursiveDeleteParent(p);
        }
        public static void RecursiveDeleteParent(Parent p)
        {
            foreach (Order o in p.Orders.ToList())
            {
                RecursiveDeleteOrder(o);
            }
            foreach (Child c in p.Children.ToList())
            {
                RecursiveDeleteChild(c);
            }
            Data.GetPs().Remove(Data.GetP(m => m.PRT_RecordID == p.PRT_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeleteOrder(String id)
        {
            Guid orid = Guid.Parse(id);
            Order o = Data.GetO(m => m.ORD_RecordID == orid);
            RecursiveDeleteOrder(o);
        }
        public static void RecursiveDeleteOrder(Order o)
        {
            foreach (Payment p in o.Payments.ToList())
            {
                RecursiveDeletePayment(p);
            }
            Data.GetOs().Remove(Data.GetO(m => m.ORD_RecordID == o.ORD_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeletePayment(String id)
        {
            Guid pmrid = Guid.Parse(id);
            Payment pm = Data.GetPM(m => m.PMT_RecordID == pmrid);
            RecursiveDeletePayment(pm);
        }
        public static void RecursiveDeletePayment(Payment pm)
        {
            foreach (Teammate t in pm.Teammates.ToList())
            {
                RecursiveDeleteTeammate(t);
            }
            Data.GetPMs().Remove(Data.GetPM(m => m.PMT_RecordID == pm.PMT_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeleteChild(String id)
        {
            Guid crid = Guid.Parse(id);
            Child c = Data.GetC(m => m.CLD_RecordID == crid);
            RecursiveDeleteChild(c);
        }
        public static void RecursiveDeleteChild(Child c)
        {
            foreach (Teammate t in c.Teammates.ToList())
            {
                RecursiveDeleteTeammate(t);
            }
            Data.GetCs().Remove(Data.GetC(m => m.CLD_RecordID == c.CLD_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeleteTeammate(String id)
        {
            Guid trid = Guid.Parse(id);
            Teammate t = Data.GetT(m => m.TMT_RecordID == trid);
            RecursiveDeleteTeammate(t);
        }
        public static void RecursiveDeleteTeammate(Teammate t)
        {
            Guid pmrid = t.TMT_PMT_RecordID;
            Data.GetTs().Remove(Data.GetT(m => m.TMT_RecordID == t.TMT_RecordID));
            Data.SaveDB();
        }
        public static void RecursiveDeleteCoach(String id)
        {
            Guid chrid = Guid.Parse(id);
            Coach ch = Data.GetCH(m => m.CCH_RecordID == chrid);
            RecursiveDeleteCoach(ch);
        }
        public static void RecursiveDeleteCoach(Coach ch)
        {
            Data.GetCHs().Remove(Data.GetCH(m => m.CCH_RecordID == ch.CCH_RecordID));
            Data.SaveDB();
        }
        #endregion

        #region v

        private static Boolean v(ObjectModel a)
        {
            return (a != null && (v(a.Parent) || /*v(a.Email) || v(a.Phone) ||*/ v(a.Password) || v(a.RoleID) || v(a.Username)));
        }
        private static Boolean v(ObjectModelParent a)
        {
            return (a != null && (v(a.Children) || v(a.Address) || v(a.City) || v(a.FatherName) || v(a.FatherName) || v(a.FatherEmail) || v(a.FatherPhone) || v(a.MotherName) || v(a.MotherEmail) || v(a.MotherPhone) || v(a.Zipcode)));
        }
        private static Boolean v(List<ObjectModelChild> a)
        {
            Boolean b = (a != null && a.Count != 0);
            Boolean c = false;
            if (b)
            {
                foreach (ObjectModelChild oc in a)
                {
                    if (v(oc))
                    {
                        c = true;
                        break;
                    }
                }
            }
            return (b && c);
        }
        private static Boolean v(ObjectModelChild a)
        {
            return (a != null && (v(a.Teammates) || v(a.Birthdate) || v(a.FirstName)));
        }
        private static Boolean v(List<ObjectModelTeammate> a)
        {
            Boolean b = (a != null && a.Count != 0);
            Boolean c = false;
            if (b)
            {
                foreach (ObjectModelTeammate ot in a)
                {
                    if (v(ot))
                    {
                        c = true;
                        break;
                    }
                }
            }
            return (b && c);
        }
        private static Boolean v(ObjectModelTeammate a)
        {
            return (a != null && (v(a.Accepted) || v(a.PicID) || v(a.SeasonID) || v(a.ShirtID) || v(a.TeamID) || v(a.Year)));
        }
        private static Boolean v(String a,Boolean b=false)
        {
            return (a != null && (a.Trim() != ""||b));
        }
        private static Boolean d(String a, Boolean b = false)
        {
            return (a != null && a.Trim() == "" );
        }

        #endregion

        public static void SendActivation(UserLogin ul)
        {
            String body = "Thank you for registering with the South Kitsap Homeschool Sports Club!<br/> Just click this link to activate your account. <br/><br/><a href='" + Global.SiteRoot + "Home/Activate/" + ul.USR_RecordID + "' style='font-size:20px;'>Activate</a><br/><br/> If the link doesn't work, copy and paste this into the browser url: " + Global.SiteRoot + "/Home/Activate/" + ul.USR_RecordID;
            Global.SendEmail("skhss@skhomeschoolsports.com", ul.USR_Email, "Activation For SKHSS Club Website Account", body);
        }
    }




}