using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKHSS.Helpers;
using SKHSS.Models;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using SKHSS.Authentication;
using SKHSS.Models.PageModels;

namespace SKHSS.Controllers
{
    public class AJAXController : Controller
    {
        SKHSSMembershipProvider sp = new SKHSSMembershipProvider();
        JavaScriptSerializer js = new JavaScriptSerializer(new SimpleTypeResolver());
        String username = "mathguy";
        String password = "M.trtlp1!";
        public ActionResult Index()
        {
            return View();
        }

        #region Content
            public string RenderRazorViewToString(string viewName, object model)
            {
                ViewBag.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                             viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                                 ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            public String Script()
            {
                ViewBag.SaveData = !Request.QueryString["a"].Equals("0");
                ViewBag.DisableBackspace = !Request.QueryString["b"].Equals("0");
                ViewBag.Members = !Request.QueryString["members"].Equals("0");
                String page = Request.QueryString["page"];
                try { page = (page != "" ? RenderRazorViewToString("Scripts/" + page + "SC", null).Replace("<script>", "").Replace("</script>", "") : ""); }
                catch { page = ""; }
                Response.ContentType = "text/javascript";
                return RenderRazorViewToString("Script", null).Replace("<script>", "").Replace("</script>", "") + page;
            }
            public String Style()
            {
                ViewBag.RelativeHeight = !Request.QueryString["a"].Equals("0");
                ViewBag.Members = !Request.QueryString["members"].Equals("0");
                String page = Request.QueryString["page"];
                try {page = (page != "" ? RenderRazorViewToString("Styles/" + page + "ST", null).Replace("<style>", "").Replace("</style>", "") : ""); }
                catch { page = ""; }
                Response.ContentType = "text/css";
                return RenderRazorViewToString("Style", null).Replace("<style>", "").Replace("</style>", "") + page;
            }
        #endregion

        #region Pages

        [HttpPost]
        public String Login(String useremail, String password)
        {
            String result = Global.Validate(useremail, password);
            if (result == "loggedin")
            {
                return "loggedin#" + sp.GetUser(useremail, useremail).UserLogin.USR_ROL_RoleID + "#";
            }
            else
            {
                if (result.Contains("account-not-activated"))
                {
                    return result;
                }
                return "Error - " + result + "#";
                
            }
        }
        public ActionResult GetAction(String name, String data)
            {
                return View(viewName: name, model: data);
            }

        #endregion

        #region globalapi

            [HttpPost]
            public Int32 Age(DateTime data)
            {
                return Global.Age(data);
            }
            [HttpPost]
            public Int32 AgeOnReg(DateTime data)
            {
                return Global.AgeOnReg(data);
            }
            [HttpPost]
            public String TeamName(DateTime data)
            {
                Int32 id = Global.TeamID(data);
                return Data.GetTM(m=>m.TEM_TeamID == id).TEM_TeamName;
            }
            [HttpPost]
            public Int32 TeamID(Int32 data, bool? Gender = null)
            {
                return Global.TeamID(data, Gender);
            }
            [HttpPost]
            public String SendActivationEmail(String data)
            {
                UserLogin ul = Data.GetLogin(data);
                SaveModel.SendActivation(ul);
                return "success";
            }

        #endregion

        #region objectvalidation
            [HttpPost]
            public Boolean ValidateBirthdate(String data)
            {
                //return js.Serialize(Validation.ValidateObject(Request["data"], Request["id"])) + "||&||" + Request["data"];
                return Validation._BirthdateIsValid(data);
            }
        #endregion

        #region objectsaves

            [HttpPost]
            public String SaveObject()
            {
                Dictionary<String,List<RegError>> val = Validation.ValidateObject(js.Deserialize<ObjectModel>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObject(Request["data"]).USR_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveParentObject()
            {
                Guid UserRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out UserRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidateParentObject(js.Deserialize<ObjectModelParent>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectParent(Request["data"], UserRecordID).PRT_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveOrderObject()
            {
                Guid ParentRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out ParentRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidateOrderObject(js.Deserialize<ObjectModelOrder>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectOrder(Request["data"], ParentRecordID).ORD_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SavePaymentObject()
            {
                Guid OrderRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out OrderRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidatePaymentObject(js.Deserialize<ObjectModelPayment>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectPayment(Request["data"], OrderRecordID).PMT_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveChildObject()
            {
                Guid ParentRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out ParentRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidateChildObject(js.Deserialize<ObjectModelChild>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectChild(Request["data"], ParentRecordID).CLD_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveTeammateObject()
            {
                Guid ChildRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out ChildRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidateTeammateObject(js.Deserialize<ObjectModelTeammate>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectTeammate(Request["data"], ChildRecordID).TMT_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveCoachObject()
            {
                Guid UserRecordID = Guid.Empty;
                Guid.TryParse(Request["lvlid"], out UserRecordID);
                Dictionary<String, List<RegError>> val = Validation.ValidateCoachObject(js.Deserialize<ObjectModelCoach>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectCoach(Request["data"], UserRecordID).CCH_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost]
            public String SaveRegObject()
            {
                Dictionary<String, List<RegError>> val = Validation.ValidateRegObject(js.Deserialize<RegisterPageModel>(Request["data"]), Request["contid"]);
                String ret = (val.Count == 0 ? SaveModel.SaveObjectReg(Request["data"]).PRT_RecordID.ToString() : "~" + js.Serialize(val));
                Data.RefreshDB();
                return ret;
            }
            [HttpPost, ValidateInput(false)]
            public String SaveCache()
            {
                try
                {
                    return SaveModel.SaveCache(Request["data"]).CAC_RecordID.ToString();
                }
                catch
                {
                    return "";
                }
            }

        #endregion

        #region objectdeletes
            [HttpPost]
            public String DeleteObject()
            {
                if (Request["username"] == username)
                {
                    if(Request["password"] == password){
                        SaveModel.RecursiveDeleteUser(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }

            }
            [HttpPost]
            public String DeleteParentObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeleteParent(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            [HttpPost]
            public String DeleteOrderObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeleteOrder(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            [HttpPost]
            public String DeletePaymentObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeletePayment(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            [HttpPost]
            public String DeleteChildObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeleteChild(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            [HttpPost]
            public String DeleteTeammateObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeleteTeammate(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            [HttpPost]
            public String DeleteCoachObject()
            {
                if (Request["username"] == username)
                {
                    if (Request["password"] == password)
                    {
                        SaveModel.RecursiveDeleteCoach(Request["id"]);
                        Data.RefreshDB();
                        return "";
                    }
                    else
                    {
                        return "p";
                    }
                }
                else
                {
                    return "u";
                }
            }
            #region Recursive
                [HttpPost]
                public String RDeleteObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty; Guid.TryParse(Request["urid"], out urid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    if (id != Guid.Empty)
                    {
                        UserLogin ul = Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!");
                        if (ov || urid == ul.USR_RecordID && id != ul.USR_RecordID)
                        {
                            SaveModel.RecursiveDeleteUser(Data.GetUL(m => m.USR_RecordID == id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else{return "f";}
                }
                [HttpPost]
                public String RDeleteParentObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty; Guid.TryParse(Request["urid"], out urid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    if (id != Guid.Empty)
                    {
                        if (ov || urid == Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!").USR_RecordID)
                        {
                            SaveModel.RecursiveDeleteParent(Data.GetP(m => m.PRT_RecordID == id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
                [HttpPost]
                public String RDeleteOrderObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty; Guid.TryParse(Request["urid"], out urid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    if (id != Guid.Empty)
                    {
                        if (ov || urid == Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!").USR_RecordID)
                        {
                            SaveModel.RecursiveDeleteOrder(Data.GetO(m => m.ORD_RecordID == id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
                [HttpPost]
                public String RDeletePaymentObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty; Guid.TryParse(Request["urid"], out urid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    if (id != Guid.Empty)
                    {
                        if (ov || urid == Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!").USR_RecordID)
                        {
                            SaveModel.RecursiveDeletePayment(Data.GetPM(m => m.PMT_RecordID == id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
                [HttpPost]
                public String RDeleteChildObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid prid = Guid.Empty; Guid.TryParse(Request["prid"], out prid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    if (id != Guid.Empty)
                    {
                        Child c = Data.GetC(m => m.CLD_RecordID == id);
                        UserLogin ul = Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "Chickens1!");
                        if (ov || c.CLD_PRT_RecordID == prid || ul.USR_RecordID == prid)
                        {
                            SaveModel.RecursiveDeleteChild(c);
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
                [HttpPost]
                public String RDeleteTeammateObject()
                {
                    Guid id = Guid.Empty; Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty; Guid.TryParse(Request["urid"], out urid);
                    Boolean ov = false; Boolean.TryParse(Request["override"], out ov);
                    Boolean updatepayment = false; Boolean.TryParse(Request["updatepayment"], out updatepayment);
                    if (id != Guid.Empty)
                    {
                        if (ov || urid == Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!").USR_RecordID)
                        {
                            SaveModel.RecursiveDeleteTeammate(Data.GetT(m => m.TMT_RecordID== id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
                [HttpPost]
                public String RDeleteCoachObject()
                {

                    Guid id = Guid.Empty;
                    Guid.TryParse(Request["id"], out id);
                    Guid urid = Guid.Empty;
                    Guid.TryParse(Request["urid"], out id);
                    if (id != Guid.Empty)
                    {
                        if (urid == Data.GetUL(m => (m.USR_Username == "mathguy" || m.USR_Username == "Mathguy") && m.USR_Password == "M.trtlp1!").USR_RecordID)
                        {
                            SaveModel.RecursiveDeleteCoach(Data.GetCH(m => m.CCH_RecordID == id));
                        }
                        Data.RefreshDB();
                        return "s";
                    }
                    else { return "f"; }
                }
            #endregion

            [HttpPost]
            public String DeleteLog()
            {
                SKHSSEntities db = Data.db;
                List<Log> ls = db.Logs.ToList();
                foreach (Log l in ls)
                {
                    db.Logs.Remove(l);
                }
                db.SaveChanges();
                return "s";
            }
        #endregion

        #region objectrequests

            [HttpPost]
        public String GetUser(String data)
        {
            Guid g = Guid.Parse(data);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == g);
            return "{\"UserRecordID\":\"" + ul.USR_RecordID + "\",\"Username\":\"" + ul.USR_Username + "\",\"Email\":\"" + ul.USR_Email + "\",\"RoleID\":\"" + ul.USR_ROL_RoleID + "\",\"UserID\":\"" + ul.USR_Email + "\",\"Activated\":\"" + ul.USR_Activated + "\",\"Phone\":\"" + ul.USR_Phone + "\",\"Parent\":[" + GetParent(ul.Parent.PRT_RecordID.ToString()) + "]}";
        }
        [HttpPost]
        public String GetParent(String data)
        {
            Guid g = Guid.Parse(data);
            Parent p = Data.GetP(m => m.PRT_RecordID == g);
            String ret = "{\"ParentRecordID\":\"" + p.PRT_RecordID + "\",\"FatherName\":\"" + p.PRT_FatherName + "\",\"MotherName\":\"" + p.PRT_MotherName + "\",\"LastName\":\"" + p.PRT_LastName + "\",\"Address\":\"" + p.PRT_Address + "\",\"City\":\"" + p.PRT_City + "\",\"Zipcode\":\"" + p.PRT_Zipcode + "\",\"Children\":[";
            List<Child> cs = p.Children.OrderByDescending(m => m.CLD_Birthdate).ToList();
            for (Int32 i = 0; i < cs.Count;i++ )
            {
                ret += GetChild(cs[i].CLD_RecordID.ToString()) + (i < p.Children.Count - 1 ? "," : "");
            }
            ret.Remove(ret.Length - 2);
            ret +="]}";
            return ret;
        }
        [HttpPost]
        public String GetChild(String data)
        {
            Guid g = Guid.Parse(data);
            Child c = Data.GetC(m => m.CLD_RecordID == g);
            String ret = "{\"ChildRecordID\":\"" + c.CLD_RecordID + "\",\"FirstName\":\"" + c.CLD_FirstName + "\",\"Birthdate\":\"" + c.CLD_Birthdate + "\",\"Notes\":\"" + c.CLD_Notes + "\",\"Gender\":\"" + c.CLD_Gender + "\",\"ParentRecordID\":\"" + c.CLD_PRT_RecordID + "\",\"Teammates\":[";
            List<Teammate> cs = c.Teammates.OrderByDescending(m => m.TMT_Year).ThenByDescending(m => m.TMT_SeasonID).ToList();
            for (Int32 i = 0; i < cs.Count;i++ )
            {
                ret += GetTeammate(cs[i].TMT_RecordID.ToString()) + (i < c.Teammates.Count - 1 ? "," : "");
            }
            ret.Remove(ret.Length - 2);
            ret +="]}";
            return ret;
        }
        [HttpPost]
        public String GetTeammate(String data)
        {
            Guid g = Guid.Parse(data);
            Teammate t = Data.GetT(m => m.TMT_RecordID == g);
            return "{\"TeammateRecordID\":\"" + t.TMT_RecordID + "\",\"Year\":\"" + t.TMT_Year + "\",\"SeasonID\":\"" + t.TMT_SeasonID + "\",\"TeamID\":\"" + t.TMT_TEM_TeamID + "\",\"ShirtID\":\"" + t.TMT_ShirtID + "\",\"Accepted\":\"" + t.TMT_Accepted + "\",\"PicID\":\"" + t.TMT_PIC_RecordID + "\",\"ChildRecordID\":\"" + t.TMT_CLD_RecordID + "\"}";
        }

    #endregion

        #region datarequests
            [HttpPost]
            public ActionResult GetUserLoginData(Guid? id)
            {
                return View("Data/UserLogins", id);
            }
            [HttpPost]
            public ActionResult GetUserLoginsData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/UserLogins", id);
            }
            [HttpPost]
            public ActionResult GetParentData(Guid? id)
            {
                return View("Data/Parents", id);
            }
            [HttpPost]
            public ActionResult GetParentsData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Parents", id);
            }
            [HttpPost]
            public ActionResult GetOrderData(Guid? id)
            {
                return View("Data/Orders", id);
            }
            [HttpPost]
            public ActionResult GetOrdersData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Orders", id);
            }
            [HttpPost]
            public ActionResult GetPaymentData(Guid? id)
            {
                return View("Data/Payments", id);
            }
            /*[HttpPost]
            public ActionResult GetPaymentsData()
            {
                ViewBag.All = true;
                return View("Data/Payments");
            }*/
            [HttpPost]
            public ActionResult GetPaymentsData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Payments", id);
            }
            [HttpPost]
            public ActionResult GetChildData(Guid? id)
            {
                return View("Data/Children", id);
            }
            [HttpPost]
            public ActionResult GetChildrenData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Children", id);
            }
            [HttpPost]
            public ActionResult GetTeammateData(Guid? id)
            {
                return View("Data/Teammates", id);
            }
            [HttpPost]
            public ActionResult GetTeammatesData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Teammates", id);
            }
            [HttpPost]
            public ActionResult GetTeamData(Guid? id)
            {
                return View("Data/Teams", id);
            }
            [HttpPost]
            public ActionResult GetTeamsData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Teams", id);
            }
            [HttpPost]
            public ActionResult GetCoachData(Guid? id)
            {
                return View("Data/Coaches", id);
            }
            [HttpPost]
            public ActionResult GetCoachesData(Guid? id, String type)
            {
                if (id == null)
                {
                    ViewBag.All = true;
                }
                ViewBag.Type = type;
                return View("Data/Coaches", id);
            }
        #endregion

            #region Fix
        [HttpPost]
            public Boolean FixError(Guid id, Int32 error)
            {
                return Fix.FixError(id, error);
            }
            #endregion

            #region paypal

            [HttpPost]
        public String IPNResponse()
        {
            return "VERIFIED";
        }
        [HttpPost]
        public String IPN()
        {
            SKHSSEntities db = new SKHSSEntities();
            IPN ipn = new IPN();
            ipn.IPN_PMT_RecordID = Request["custom"];
            if (db.IPNs.SingleOrDefault(m => m.IPN_PMT_RecordID == ipn.IPN_PMT_RecordID) == null)
            {
                ipn.IPN_TransactionID = Request["txn_id"];
                Decimal d = new Decimal(0.02);
                Decimal.TryParse(Request["mc_gross"], out d);
                ipn.IPN_Amount = d;
                ipn.IPN_Status = Request["payment_status"];
                ipn.IPN_Email = Request["receiver_email"];
                ipn.IPN_PayerEmail = Request["payer_email"];
                ipn.IPN_PayerLastName = Request["last_name"];
                ipn.IPN_RecieptDate = DateTime.Now;
                db.IPNs.Add(ipn);
                db.SaveChanges();
                /*}
                catch (Exception ex)
                {
                    ipn = new IPN();
                    ipn.IPN_TransactionID = Guid.NewGuid().ToString();
                    ipn.IPN_Response = ex.Message;
                    if (ex.InnerException != null)
                    {
                        ipn.IPN_Status = ex.InnerException.Message;
                    }
                    db.IPNs.Add(ipn);
                    db.SaveChanges();
                }*/
                try
                {
                    Guid pmrid = Guid.Parse(ipn.IPN_PMT_RecordID);
                    Payment pmt = db.Payments.SingleOrDefault(m => m.PMT_RecordID == pmrid);
                    if (pmt != null && ipn.IPN_Email.ToLower() == "skhssc@hotmail.com".ToLower() && ipn.IPN_Status == "Completed" && pmt.PMT_Paid == false && (pmt.PMT_Total == Decimal.ToInt32(ipn.IPN_Amount.Value) || ipn.IPN_Amount.Value == (Decimal)0.01))
                    {
                        pmt.PMT_Paid = true;
                        pmt.PMT_IPN_TransactionID = ipn.IPN_TransactionID;
                        pmt.PMT_TransactionDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        ipn.IPN_Response = "failed";
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ipn.IPN_Error = true;
                    ipn.IPN_Response = ex.Message;
                    if (ex.InnerException != null)
                    {
                        ipn.IPN_Status = ex.InnerException.Message;
                    }
                    db.SaveChanges();
                }
            }
            return "";
        }
        string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox)
    {
        String siteRoot = Request.Url.AbsoluteUri;
        if (Request.Url.Query != "")
        {
            siteRoot = siteRoot.Replace(Request.Url.Query, String.Empty);
        }
        siteRoot = siteRoot.Replace(Request.Url.AbsolutePath, String.Empty);
        string paypalUrl = useSandbox ? siteRoot + "/AJAX/IPNResponse" : "https://www.paypal.com/cgi-bin/webscr";

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

        // Set values for the request back
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";

        byte[] param = Request.BinaryRead(Request.ContentLength);
        string strRequest = Encoding.ASCII.GetString(param);

        StringBuilder sb = new StringBuilder();
        sb.Append(strRequest);

        foreach (string key in formVals.Keys)
        {
            sb.AppendFormat("&{0}={1}", key, formVals[key]);
        }
        strRequest += sb.ToString();
        req.ContentLength = strRequest.Length;

        //Send the request to PayPal and get the response
        string response = "";
        using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
        {

            streamOut.Write(strRequest);
            streamOut.Close();
            using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = streamIn.ReadToEnd();
            }
        }

        return response;
    }

        #endregion

    }
}
