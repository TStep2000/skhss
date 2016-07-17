using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKHSS.Models;
using SKHSS.Authentication;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web.Security;
using SKHSS.Helpers;
using System.Net;
using System.Web.Script.Serialization;
using SKHSS.Models.PageModels;

namespace SKHSS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.QueryString.Count > 0)
            {
                ViewBag.login = Request.QueryString[0];
            }
            List<Cache> model = new List<Cache>();
            model.Add(Data.GetCCs(m => m.CAC_Type == "main").FirstOrDefault());
            if (model[0] == null) { model[0] = new Cache(); }
            model.Add(Data.GetCCs(m => m.CAC_Type == "news").FirstOrDefault());
            if (model[1] == null) { model[1] = new Cache(); }
            model.Add(Data.GetCCs(m => m.CAC_Type == "events").FirstOrDefault());
            if (model[2] == null) { model[2] = new Cache(); }
            return View(model);
        }
        public ActionResult News(String id)
        {
            return View(id);
        }
        public ActionResult About()
        {
            //ViewBag.PageID = 2;
            return View();
        }
        public ActionResult Calendar()
        {
            ViewBag.CurrentSeason = Definitions.Seasons[Global.CurrentSeasonID];
            ViewBag.CurrentSeasonID = Global.CurrentSeasonID;
            ViewBag.CurrentYear = Global.CurrentYear;
            List<Cache> model = new List<Cache>();
            model.Add(Data.GetCCs(m => m.CAC_Type == "calendar").FirstOrDefault());
            if (model[0] == null) { model[0] = new Cache(); }
            model.Add(Data.GetCCs(m => m.CAC_Type == "events").FirstOrDefault());
            if (model[1] == null) { model[1] = new Cache(); }
            return View(model);
        }
        public ActionResult Contact()
        {
            //ViewBag.PageID = 4;
            return View();
        }
        public ActionResult Forum()
        {
            //ViewBag.PageID = 4;
            return View();
        }
        public String ContactSubmit(String name, String email, String message)
        {
            Regex emailchk = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}");
            if (email == "" || email == null || !emailchk.IsMatch(email))
            {
                email = "[no email given]";
            }
            SmtpClient client = new SmtpClient("localhost", 25);
            client.Credentials = new NetworkCredential("skhss@skhomeschoolsports.com", "Chickens1!");
            MailMessage me = new MailMessage(email, "skhssports@gmail.com", "Message from the website", name + ", at " + email + " says: \n\n" + message);
            me.IsBodyHtml = true;
            client.Send(me);
            return "Thank you! We appreciate the correspondance!";
        }
        public ActionResult DevFeedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DevFeedback(Feedback model)
        {
            if (model.FBK_Name == null || model.FBK_Message == null || model.FBK_Email == null)
            {
                ViewBag.Error = "All fields are required";
                return View();
            }
            if (model.FBK_Message.Length > 255)
            {
                ViewBag.Error = "Max message length: 255 characters";
                return View();
            }
            model.FBK_RecordID = Guid.NewGuid();
            model.FBK_Date = DateTime.Now;
            model.FBK_Viewed = false;
            model.FBK_Complete = false;
            Data.db.Feedbacks.Add(model);
            Data.SaveDB();
            ViewBag.Result = "success";
            return View();
        }
        public ActionResult Secret()
        {
            return View();
        }
        public ActionResult PayPalComplete()
        {
            try
            {
                /*Parent p = Global.GetReadOnlyParent(User.Identity.Name);
                Payment pmt = db.Payments.SingleOrDefault(m=>m.PMT_RecordID == Request);
                pmt.PMT_IPN_TransactionID = Request["tx"];
                pmt.PMT_TrasactionDate = DateTime.Now;
                db.SaveChanges();

                IPN ipn = db.IPNs.SingleOrDefault(m => m.IPN_TransactionID == pmt.PMT_IPN_TransactionID);
                if (pmt != null && ipn != null && ipn.IPN_Email.ToLower() == "mathguy3@gmail.com".ToLower() && ipn.IPN_Status == "Completed" && pmt.ORD_Paid == false && (pmt.ORD_Total == Decimal.ToInt32(ipn.IPN_Amount.Value) || ipn.IPN_Amount.Value == (Decimal)0.02))
                {
                    pmt.PMT_Paid = true;
                    db.SaveChanges();
                    ViewBag.Complete = true;
                }*/
            }
            catch 
            {
                ViewBag.Error = true;
            }
            return View();
        }
        public ActionResult LoginMe()
        {
            if (Request.Url.ToString().Contains("http://localhost:1994/"))
            {
                FormsAuthentication.SignOut();
                Global.Validate("mathguy", "Chickens1!");
                return Redirect("/Members/");
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLogin model)
        {
            String result = Global.Validate(model.USR_Username, model.USR_Password);

            if (result == "loggedin")
            {

                return RedirectToAction("Index", "Members");
            }
            else
            {
                ViewBag.LoginError = "Error - " + result;
                if (result == "account-not-activated")
                {
                    ViewBag.LoginError = result;
                }
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public String Logout(String id)
        {
            FormsAuthentication.SignOut();
            return "loggedout";
        }
        public ActionResult SendPasswordReset()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendPasswordReset(String email)
        {
            UserLogin ul = Data.GetUL(m => m.USR_Email == email);
            if (ul != null)
            {
                ul.USR_ResetDate = DateTime.Now;
                Data.SaveDB();
                ViewBag.Result = "s";
                String sr = Global.SiteRoot;
                if (!Global.SendEmail("skhss@skhomeschoolsports.com", email, "Homeschool Sports - Password Reset", "This link will expire 24 hours after it is sent. <br/><br/><a href='" + sr + "Home/ResetPassword/" + ul.USR_RecordID + "' style='font-size:20px;'>Reset Password</a><br/><br/> If the link doesn't work, copy and paste this into your browser's address bar: " + sr + "Home/ResetPassword/" + ul.USR_RecordID))
                {
                    ViewBag.Result = "fe";
                }
            }
            else
            {
                ViewBag.Result = "f";
            }
            return View();
        }
        public ActionResult ResetPassword(String id)
        {
            Guid gid = Guid.NewGuid();
            Guid.TryParse(id, out gid);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == gid);
            if (ul == null)
            {
                ViewBag.ErrorOne = "That Account id does not match an account.";
            }
            else if (!ul.USR_ResetDate.HasValue || ul.USR_ResetDate.Value.AddDays(1).CompareTo(DateTime.Now) < 0)
            {
                ViewBag.ErrorOne = "The link for that email has already expired.";
            }
            return View(ul);
        }
        [HttpPost]
        public ActionResult ResetPassword(UserLogin ul)
        {
            String passchk = @".{5,15}";
            if (ul.USR_Username != null && ul.USR_Username == ul.USR_Password && Regex.IsMatch(ul.USR_Username, passchk))
            {
                UserLogin sl = Data.GetUL(m => m.USR_Email == ul.USR_Email);
                if (sl.USR_ResetDate.HasValue && sl.USR_ResetDate.Value.AddDays(1).CompareTo(DateTime.Now) > 0)
                {
                    sl.USR_Password = ul.USR_Password;
                    sl.USR_ResetDate = null;
                    Data.SaveDB();
                    ViewBag.Result = "s";
                }
                else
                {
                    ViewBag.ErrorOne = "The link for that email has already expired.";
                }
            }
            else if (ul.USR_Username == null)
            {
                ViewBag.Error = "Both fields are required.";
            }
            else if (!Regex.IsMatch(ul.USR_Username, passchk))
            {
                ViewBag.Error = "Password must be between 5 and 15 characters";
            }
            else if (ul.USR_Username != ul.USR_Password)
            {
                ViewBag.Error = "Passwords didn't match";
            }
            return View(ul);
        }
        public ActionResult Activate(String id)
        {
            FormsAuthentication.SignOut();
            Guid g = Guid.Empty;
            Guid.TryParse(id, out g);
            UserLogin ul = Data.GetUL(m => m.USR_RecordID == g);
            if (ul == null)
            {
                return View();
            }
            else
            {
                ul.USR_Activated = true;
                Data.SaveDB();
                Global.Validate(ul.USR_Username, ul.USR_Password);
                return View(model: "y");
            }
        }

        public ActionResult Registration()
        {
            return View();
        }
        public ActionResult ReqLogin()
        {
            return View();
        }
        [HttpPost]
        public String ReqLogin(String email = "", String phone = "")
        {
            if (email != "")
            {
                UserLogin ul = Data.GetUL(m => m.USR_Email == email);
                if (ul != null && ul.USR_Temp == true)
                {
                    Global.SendEmail("skhomeschoolsports@outlook.com", email, "Login Credentials", "You can get a new password and login using this email. Click <a href=\"\">this link</a> to select a password.");
                    return "email-success";
                }
                else
                {
                    return "email-fail";
                }
            }
            else if(phone != "")
            {
                List<UserLogin> uls = Data.GetULs(m => m.USR_Phone == phone).ToList();
                if (uls.Count > 0 && uls.Any(m=>m.USR_Temp==true))
                {
                    foreach (UserLogin ul in uls.Where(m => m.USR_Temp == true).ToList())
                    {
                        ul.USR_ResetDate = DateTime.Now;
                    }
                    Data.SaveDB();
                    return "phone-success";
                }
                else
                {
                    return "phone-fail";
                }
            }
            return "fail";
        }
        /*[HttpPost]
        public ActionResult Registration(ObjectModel model)
        {
            if (ModelState.IsValid)
            {
                if (RegistrationObjectValidates(model))
                {
                    UserLogin ul;
                    try
                    {
                        ul = SaveModel.SaveUser(model);
                    }
                    catch (ArgumentException ex)
                    {
                        ViewBag.Error = 2;
                        ViewBag.ErrorText = ex.Message;
                        ViewBag.ErrorID = ex.Data["ErrorID"];
                        return View(model);
                    }
                    FormsAuthentication.Authenticate(ul.USR_Username, ul.USR_Password);
                    if (!Request.Url.ToString().Contains("http://localhost:1994/"))
                    {
                        //SendActiviation(ul);
                    }
                    try
                    {
                        //SendActiviation(ul);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = 2;
                        if (ex.InnerException != null)
                        {
                            ViewBag.ErrorText = "Email Error: The Email may not have reached your inbox. " + ex.Message + " Inner: " + ex.InnerException.Message;
                        }
                        ViewBag.ErrorText = "Email Error: The Email may not have reached your inbox. " + ex.Message;
                        return View(model);
                    }
                    return RedirectToAction("AccountConfirmation", new { noemail = Request["noemail"] == "on" });
                }
                else
                {
                    ViewBag.Error = 1;
                    ViewBag.ErrorText = "Form did not validate.";
                }
            }
            else
            {
                ViewBag.Error = 0;
                ViewBag.ErrorText = "Form did not validate";
            }

            return View(model);
        }
        [HttpPost]
        public String AJAXRegistration(String data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer(new SimpleTypeResolver());
            ObjectModel model;
            try
            {
                model = js.Deserialize<ObjectModel>(data);
            }
            catch (Exception ex)
            {
                return "Error:0 Deserialize Error";
            }
            if (RegistrationObjectValidates(model))
            {
                UserLogin ul;
                try
                {
                    ul = SaveModel.SaveUser(model);
                }
                catch (Exception ex)
                {
                    if (ex.Data["Errors"] != null)
                    {
                        Dictionary<String, RegError> Errors = ((Dictionary<String, RegError>)ex.Data["Errors"]);
                        if (Errors.Count > 0)
                        {
                            /*String ret = "~";
                            if (Errors.Count(m => m.Value.Priority == 1) > 0)
                            {
                                ret += "1";
                                if (Errors.Count(m => m.Value.Priority == 1) > 1)
                                {
                                    ret += "These fields are required.";
                                }
                                else
                                {
                                    ret += Errors.Single(m => m.Value.Priority == 1).Value.Message;
                                }
                            }
                            else if (Errors.Count(m => m.Value.Priority >= 2) > 0)
                            {
                                RegError cur = Errors.Where(m => m.Value.Priority >= 2).First().Value;
                                ret += "2" + cur.Message + "|" + cur.id;
                            }
                            return ret;
                        }
                    }
                    if (ex.InnerException != null)
                    {
                        return "Error:x Save Error: " + ex.Message + " Inner: " + ex.InnerException.Message;
                    }
                    return "Error:x Save Error: " + ex.Message;
                }
                FormsAuthentication.Authenticate(ul.USR_Username, ul.USR_Password);
                try
                {
                    //SendActiviation(ul);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return "Error:2 Email Error: The Email may not have reached your inbox. " + ex.Message + " Inner: " + ex.InnerException.Message;
                    }
                    return "Error:2 Email Error: The Email may not have reached your inbox. " + ex.Message;
                }
                return "success";
            }
            else
            {
                return "Error:1 Registration Validation Error";
            }
        }*/

        /*public Boolean RegistrationObjectValidates(ObjectModel reg)
        {
            if (reg != null && (reg.UserRecordID == null || reg.UserRecordID == "") && (reg.Delete == null || reg.Delete == "") && reg.Parent != null && (reg.Parent.ParentRecordID == null || reg.Parent.ParentRecordID == "") && (reg.Parent.Delete == null || reg.Parent.Delete == "") && reg.Parent.Children == null)
            {
                return true;
            }
            return false;
        }*/
        
        public ActionResult AccountConfirmation()
        {
            return View();
        }
        [HttpPost]
        public String CheckUsername(String id)
        {
            String Username = id;
            SKHSSMembershipProvider pv = new SKHSSMembershipProvider();
            if (pv.GetUser(Username, null) != null)
            {
                return "Username-taken";
            }
            String usrchk = @"^[a-zA-Z0-9_-]{5,15}$";
            if (!Regex.IsMatch(Username, usrchk))
            {
                return "Username-nomatch";
            }
            return "Username-valid";
        }
        [HttpPost]
        public String CheckPhone(String id)
        {
            String Phone = Global.StripPhone(id);
            List<UserLogin> uls = Data.GetULs(m => m.USR_Phone == Phone).ToList();
            if (uls.Count > 0)
            {
                if (uls.Any(m => m.USR_Temp == true))
                {
                    return "Phone-temp";
                }
            }
            return "Phone-valid";
        }
        [HttpPost]
        public String CheckEmail(String id)
        {
            String Email = id;
            SKHSSMembershipProvider pv = new SKHSSMembershipProvider();
            SKHSSUser sku = pv.GetUser(null, Email);
            if (sku != null)
            {
                if (sku.UserLogin.USR_Temp == true)
                {
                    return "Email-temp";
                }
                else
                {
                    return "Email-taken";
                }
            }
            Regex emailchk = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}");
            if (!emailchk.IsMatch(Email))
            {
                return "Email-nomatch";
            }
            return "Email-valid";
        }
        [HttpPost]
        public String CheckPassword(String id)
        {
            String Password = id;
            String passchk = @".{5,15}";
            if (!Regex.IsMatch(Password, passchk))
            {
                return "Password-nomatch";
            }
            return "Password-valid";
        }
        public String CheckZipcode(String id)
        {
            String Zipcode = id;
            if (Zipcode.Length != 5)
            {
                return "Zipcode-nomatch";
            }
            return "Zipcode-valid";
        }
    }
}