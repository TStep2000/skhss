using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKHSS.Models;
using SKHSS.Models.PageModels;
using SKHSS.Authentication;
using System.Web.Security;
using SKHSS.Helpers;
using System.IO;
using System.Drawing;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;
using System.Net.Mail;

namespace SKHSS.Controllers
{
    public class MembersController : Controller
    {
        SKHSSMembershipProvider sp = new SKHSSMembershipProvider();
        private static Random rand = new Random();
        private bool ThumbnailCallback()
        {
            return false;
        }

        #region MemberPages
            public ActionResult Index()
            {
                ViewBag.MemberPageID = 1;
                AccountPageModel model;
                UserLogin ul = Data.GetLogin(User.Identity.Name);
                if (Global.IsInRole(ul, Definitions.ParentRole))
                {
                    Parent p = Data.GetLogin(User.Identity.Name).Parent;
                    if (p == null){
                        p = SaveModel.CreateDefaultParent(ul);
                    }
                    model = new AccountPageModel(p);
                }
                else
                {
                    model = new AccountPageModel(ul);
                }

                return View(model);
            }
            public String MyAlbums(String id)
            {
                SKHSSUser user = sp.GetUser(id,id);
                List<Album> albums = Data.db.Albums.Where(m => m.ALB_USR_RecordID == user.USR_RecordID).ToList();
                String[] s = new String[albums.Count];
            
                for (int i = 0; i < albums.Count; i++)
                {
                    s[i] = albums[i].ALB_RecordID.ToString();
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                String result = js.Serialize(s);
                return result;
            }
            public ActionResult RenderAlbum(String id)
            {
                Guid guid = Guid.Parse(id);
                Album a = Data.db.Albums.Single(m => m.ALB_RecordID == guid);
                return PartialView("DisplayTemplates/Album", a);
            }
            public ActionResult Teams()
            {
                if (!Data.GetLogin(User.Identity.Name).USR_Approved && !Global.IsInRoleOrHigher(User.Identity.Name, Definitions.CoachRole))
                {
                    return RedirectToAction("Index");
                }
                ViewBag.MemberPageID = 2;
                List<Team> dbteams = Data.db.Teams.Where(m=>m.TEM_Enabled).ToList();
                return View("Shared/Teams", dbteams);
            }
            public ActionResult Team(Int32 id)
            {
                if (!Data.GetLogin(User.Identity.Name).USR_Approved && !Global.IsInRoleOrHigher(User.Identity.Name, Definitions.CoachRole))
                {
                    return RedirectToAction("Index");
                }
                ViewBag.MemberPageID = 2;
                Team model = Data.db.Teams.Single(m => m.TEM_TeamID == id);
                List<Picture> PictureList = Data.db.Pictures.Where(m => m.PIC_TeamID == id).ToList();
                if (PictureList.Count >= 6)
                {
                    PictureList = PictureList.GetRange(0, 6);
                }
                ViewBag.PictureList = PictureList;
                switch (id)
                {
                    case 0: ViewBag.tag = "peewee"; break;
                    case 1: ViewBag.tag = "middler"; break;
                    case 2: ViewBag.tag = "intermediate"; break;
                    case 3: ViewBag.tag = "junior"; break;
                    case 4: ViewBag.tag = "Senior"; break;
                    case 5: ViewBag.tag = "girl"; break;
                    default: ViewBag.tag = "none";
                    break;
                }
                return View("Shared/Team", model);
            }

            #region GalleryPages
                public ActionResult Pictures()
                {
                    SKHSSEntities db = new SKHSSEntities();
                    ViewBag.MemberPageID = 3;
                    Guid userid = sp.GetUser(User.Identity.Name, User.Identity.Name).USR_RecordID;
                    GalleryModel model = new GalleryModel();
                    model.MyAlbumList = Data.db.Albums.Where(m => m.ALB_USR_RecordID == userid).ToList();
                    for (var i = 0; i < 6; i++)
                    {
                        IQueryable<Picture> pics = db.Pictures.Where(m => m.PIC_TeamID == i && m.PIC_Private == false);
                        model.TeamsList.Add(pics.ToList());
                    }
                    model.PublicAlbums = Data.db.Albums.Where(m => m.ALB_Private == false).ToList();
                    return View("Shared/Pictures", model);
                }
                public ActionResult EditAlbum(Guid? id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    Album model = new Album();
                    if (id.HasValue)
                    {
                        model = Data.db.Albums.Single(m => m.ALB_RecordID == id.Value);
                    }
                    return View("Shared/EditAlbum", model);
                }
                public ActionResult Album(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    Int32 c = -1;
                    Int32.TryParse(id, out c);
                    Guid guid = Guid.Empty;
                    Guid.TryParse(id, out guid);
                    List<Picture> pics = new List<Picture>();
                    SKHSSUser user = sp.GetUser(User.Identity.Name, User.Identity.Name);
                    if (guid != Guid.Empty || (c > -1 && c < 7))
                    {
                        Album a;
                        Guid userid = user.USR_RecordID;
                        if (guid != Guid.Empty)
                        {
                            pics = db.Pictures.Where(m => m.PIC_ALB_RecordID == guid).ToList();
                            a = Data.db.Albums.Single(m => m.ALB_RecordID == guid);
                            ViewBag.Owned = a.ALB_USR_RecordID == userid;
                            ViewBag.AlbumName = a.ALB_Title;
                            ViewBag.AlbumDesc = a.ALB_Description;
                            ViewBag.AlbumID = a.ALB_RecordID;
                        }
                        else
                        {
                            pics = db.Pictures.Where(m => m.PIC_TeamID == c && m.PIC_Private == false).ToList();
                            //ViewBag.AlbumName = Global.TeamName(c);
                            ViewBag.AlbumDesc = "";
                            ViewBag.Owned = false;
                        }
                        ViewBag.Crash = false;
                    }
                    else
                    {
                        ViewBag.Owned = false;
                        ViewBag.Crash = true;
                    }
                    ViewBag.UserRole = user.UserLogin.USR_ROL_RoleID;
                    return View("Shared/Album", pics);
                }
                [HttpPost]
                public String ChangeCoachPic(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    try
                    {
                        id = id.Substring(id.IndexOf("0000"));
                        Guid guid = db.Pictures.Single(m => m.PIC_Filename == id).PIC_RecordID;
                        SKHSSUser user = sp.GetUser(User.Identity.Name, User.Identity.Name);
                        Coach c = db.Coaches.Single(m => m.CCH_USR_RecordID == user.UserLogin.USR_RecordID);
                        c.CCH_PIC_RecordID = guid;
                        Data.SaveDB();
                    }
                    catch 
                    {
                        result = "!error";
                    }
                    return result;
                }
                [HttpPost]
                public String ChangeTeamPic(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    try
                    {
                        id = id.Substring(id.IndexOf("0000"));
                        Guid guid = db.Pictures.Single(m => m.PIC_Filename == id).PIC_RecordID;
                        SKHSSUser user = sp.GetUser(User.Identity.Name, User.Identity.Name);
                        Coach c = db.Coaches.Single(m => m.CCH_USR_RecordID == user.UserLogin.USR_RecordID);
                        Team t = db.Teams.Single(m => m.TEM_TeamID == c.CCH_TEM_TeamID);
                        t.TEM_PIC_RecordID = guid;
                        Data.SaveDB();
                    }
                    catch 
                    {
                        result = "!error";
                    }
                    return result;
                }
                [HttpPost]
                public String UploadAlbum()
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    try
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        String JSON = Request.Form["data"];
                        Album albumModel = js.Deserialize<Album>(JSON);
                        if (albumModel.ALB_RecordID == null)
                        {
                            Album newAlbum = new Album();
                            Guid albumID = Guid.NewGuid();
                            newAlbum.ALB_RecordID = albumID;
                            newAlbum.ALB_USR_RecordID = sp.GetUser(User.Identity.Name, User.Identity.Name).USR_RecordID;
                            newAlbum.ALB_Title = albumModel.ALB_Title;
                            newAlbum.ALB_Description = albumModel.ALB_Description;
                            Data.db.Albums.Add(newAlbum);
                            Data.SaveDB();
                            result = albumID.ToString();
                        }
                        else
                        {
                            //Guid albumID = Guid.Parse(albumModel.ALB_RecordID);
                            Album editAlbum = Data.db.Albums.Single(m => m.ALB_RecordID == albumModel.ALB_RecordID);
                            editAlbum.ALB_Title = albumModel.ALB_Title;
                            editAlbum.ALB_Description = albumModel.ALB_Description;
                            Data.SaveDB();
                            result = albumModel.ALB_RecordID.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        result = "!Alb Save Error: " + e.InnerException;
                    }
                    return result;
                }
                [HttpPost]
                public String DeleteAlbum(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    Boolean over = Boolean.Parse((Request.Form["over"] == null) ? "False" : Request.Form["over"]);
                    try
                    {
                        Guid guid = Guid.Parse(id);
                        Album a = Data.db.Albums.Single(m => m.ALB_RecordID == guid);
                        List<Picture> pics = db.Pictures.Where(m => m.PIC_ALB_RecordID == guid).ToList();
                        foreach (Picture p in pics)
                        {
                            String res = used(p.PIC_RecordID);
                            if (!over && res != "")
                            {
                                throw new Exception(res);
                            }
                            db.Pictures.Remove(p);
                        }
                        Data.db.Albums.Remove(a);
                        Data.SaveDB();
                    }
                    catch (Exception e)
                    {
                        if (e.Message.StartsWith("~"))
                        {
                            result = e.Message;
                        }
                        else
                        {
                            result = "! Alb delete error: " + e.InnerException;
                        }
                    }
                    return result;
                }
                [HttpPost]
                public String UploadPicture()
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    String uploadresult = "";
                    PictureModel pictureModel = new PictureModel();
                    try
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        String JSON = Request.Form["data"];
                        String ALB_RecordID = Request.Form["name"];
                        pictureModel = js.Deserialize<PictureModel>(JSON);
                        if (pictureModel.Filename.EndsWith(".png"))
                        {
                            throw new Exception(".png files not supported");
                        }
                        if (pictureModel.Filename != "")
                        {
                            uploadresult = UploadImage(pictureModel.PictureData.Substring(23), pictureModel.Filename);
                            if (uploadresult.StartsWith("!"))
                            {
                                throw new Exception(uploadresult);
                            }
                            String newName = uploadresult.Substring(uploadresult.LastIndexOf(@"\") + 1);
                            Picture newPicture = new Picture();
                            newPicture.PIC_RecordID = Guid.NewGuid();
                            newPicture.PIC_ALB_RecordID = Guid.Parse(ALB_RecordID);
                            newPicture.PIC_Filename = newName;
                            newPicture.PIC_TeamID = Int32.Parse(pictureModel.Team);
                            newPicture.PIC_Caption = pictureModel.Caption;
                            db.Pictures.Add(newPicture);
                            Data.SaveDB();
                        }
                        else
                        {
                            Guid guid = Guid.Parse(pictureModel.PIC_RecordID);
                            Picture editPicture = db.Pictures.Single(m => m.PIC_RecordID == guid);
                            editPicture.PIC_Caption = pictureModel.Caption;
                            editPicture.PIC_TeamID = Int32.Parse(pictureModel.Team);
                            Data.SaveDB();
                        }
                    }
                    catch (Exception e)
                    {
                        result = "!Pic Save Error: " + e.Message + "/" + pictureModel.Filename;
                        if (!e.Message.StartsWith("!"))
                        {
                            result += " || " + CancelImage(uploadresult);
                        }
                    }
                    return result;
                }
                public String CancelImage(String name)
                {
                    String result = "";
                    try
                    {
                        System.IO.File.Delete(name);
                        result = "Rollback: success";
                    }
                    catch(Exception e)
                    {
                        result = "!Roll Error: " + e.Message;
                    }
                    return result;
                }
                public String UploadImage(String data, String name)
                {
                    String result = "";
                    try
                    {
                        String pathstr = "~/Content/uploads";
                        String foldernum = "0000" + rand.Next(0, 9) + "/";
                        String filenum = Guid.NewGuid().ToString();
                        String fileext = name.Substring(name.LastIndexOf("."));
                        Int32 thumbWidth = 200;
                        if (!Directory.Exists(Path.Combine(Server.MapPath(pathstr), foldernum)))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath(pathstr), foldernum));
                        }
                        if (!Directory.Exists(Path.Combine(Server.MapPath(pathstr), foldernum, "thumbnails/")))
                        {
                            Directory.CreateDirectory(Path.Combine(Server.MapPath(pathstr), foldernum, "thumbnails/"));
                        }
                        String save = Path.Combine(Server.MapPath(pathstr), foldernum, filenum + fileext);
                        String savethumb = Path.Combine(Server.MapPath(pathstr), foldernum + "thumbnails/", filenum + fileext);
                        Byte[] imgdata = new Byte[data.Length];
                        imgdata = Convert.FromBase64String(data);
                        System.IO.File.WriteAllBytes(save, imgdata);

                        System.Drawing.Image origimg = System.Drawing.Image.FromFile(save);

                        Int32 newheight = (Int32)(((Double)thumbWidth / origimg.Width) * origimg.Height);
                        System.Drawing.Image thumimg = origimg.GetThumbnailImage(thumbWidth, newheight, null, new IntPtr());
                        origimg.Dispose();

                        thumimg.Save(savethumb);
                        thumimg.Dispose();

                        result = save;
                    }
                    catch (Exception e)
                    {
                        result = "!Upl. Error: " + e.Message + "/" + name;
                    }
                    return result;
                }
                public string FixBase64ForImage(string Image)
                {
                    System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
                    sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
                    return sbText.ToString();
                }
                [HttpPost]
                public String DeletePicture(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    try
                    {
                        String pathstr = "~/Content/uploads";
                        Guid guid = Guid.Parse(id);
                        Picture pic = db.Pictures.Single(m => m.PIC_RecordID == guid);
                        Boolean over = Boolean.Parse((Request.Form["over"] == null) ? "False" : Request.Form["over"]);
                        String use = used(guid);
                        if (use=="~c")
                        {
                            if (over)
                            {
                                List<Coach> coaches = db.Coaches.Where(m => m.CCH_PIC_RecordID == guid).ToList();
                                foreach (Coach c in coaches)
                                {
                                    c.CCH_PIC_RecordID = null;
                                }
                                Data.SaveDB();
                            }
                            else
                            {
                                throw new Exception("~c");
                            }
                        }
                        if (use == "~t")
                        {
                            if (over)
                            {
                                List<Team> teams = db.Teams.Where(m => m.TEM_PIC_RecordID == guid).ToList();
                                foreach (Team t in teams)
                                {
                                    t.TEM_PIC_RecordID = null;
                                }
                                Data.SaveDB();
                            }
                            else
                            {
                                throw new Exception("~t");
                            }
                        }
                        System.IO.File.Delete(Path.Combine(Server.MapPath(pathstr), pic.PIC_Filename));
                        db.Pictures.Remove(pic);
                        Data.SaveDB();
                
                    }
                    catch (Exception e)
                    {
                        if (e.Message.StartsWith("~"))
                        {
                            result = e.Message;
                        }
                        else
                        {
                            result = "! DeleteError: " + e.InnerException;
                        }
                    }
                    return result;
                }
                public String used(Guid guid)
            {
                SKHSSEntities db = new SKHSSEntities();
                String result = "";
                if (db.Coaches.Any(m => m.CCH_PIC_RecordID == guid))
                {
                    result = "~c";
                }
                if (db.Teams.Any(m => m.TEM_PIC_RecordID == guid))
                {
                    result = "~t";
                }
                return result;
            }
            #endregion
        #endregion

        #region ParentPages
            public ActionResult Register()
            {
                RegisterPageModel model = new RegisterPageModel(Data.GetLogin(User.Identity.Name).Parent);
                ViewBag.ShirtSizeList = Definitions.GetShirtSizeList();

                ViewBag.State = 0;
                Order ord = Data.GetCurrentOrder(model.Parent);
                ViewBag.nul = ord == null;
                if (model.RegChildren.Count == 0)
                {
                    ViewBag.State = 0;
                }
                else if (ViewBag.nul)
                {
                    ViewBag.State = 1;
                }
                else if (Global.OrderComplete(ord))
                {
                    ViewBag.State = 3;
                }
                else
                {
                    ViewBag.State = 2;
                }
                ViewBag.NumReg = model.RegChildren.Where(m => m.Reg == true).Count();
                ViewBag.NumUni = model.RegChildren.Where(m => m.Uni == true).Count();

                ViewBag.tcur = Global.CombPrice(ViewBag.NumReg, ViewBag.NumUni);

                ViewBag.treg = Data.GetCurrentTeammates(model.Parent).Count * Definitions.RegistrationPrice;
                ViewBag.max = Definitions.FamilyMaxiumum;
                ViewBag.tsub = 0;
                ViewBag.tgrand = 0;
                ViewBag.tpaid = 0;
                ViewBag.Paid = false;
                if (ViewBag.nul != true)
                {
                    ViewBag.Paid = Global.OrderComplete(ord);
                    foreach (Payment p in ord.Payments.ToList())
                    {
                        ViewBag.tsub += Global.CombPrice(p.PMT_Registrations, p.PMT_Uniforms);
                    }
                    if (ord.Payments.Any(m => m.PMT_LateFee == true))
                    {
                        ViewBag.tsub += 10;
                    }
                    ViewBag.tgrand = ord.Payments.Sum(m => m.PMT_Total);
                    ViewBag.tpaid = ord.Payments.Where(m => m.PMT_Paid).Sum(m => m.PMT_Total);
                }

                return View("Parent/Register", model);
            }
        #endregion

        #region CoachPages
            public ActionResult MyTeam()
            {
                if (!Global.IsInRole(User.Identity.Name, Definitions.CoachRole))
                {
                    return RedirectToAction("Index");
                }
                Guid ulg = Data.GetLogin(User.Identity.Name).USR_RecordID;
                Int32 n = Data.GetCH(m => m.CCH_USR_RecordID == ulg).CCH_TEM_TeamID;
                return RedirectToAction("Team", new { id = n });
            }
        #endregion

        #region AdministratorPages
            public ActionResult ActivateUsers()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.MeRole, Definitions.PresRole }))
                {
                    return RedirectToAction("Index");
                }
                List<UserLogin> uls = Data.GetULs(m => m.USR_Activated == false && (m.USR_Email == "" || m.USR_Email == null)).Where(Predicates.IsInRole(Definitions.ParentRole)).ToList();
                if (Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                {
                    uls = Data.GetULs(m => m.USR_Activated == false).Where(Predicates.IsInRole(Definitions.ParentRole)).OrderBy(m => m.USR_Email).ToList();
                }
                return View("Admin/ActivateUsers", uls);
            }
            public ActionResult Registrations()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.PresRole, Definitions.WebAdminRole, Definitions.MeRole }))
                {
                    return RedirectToAction("Index");
                }
                Boolean me = Global.IsInRole(User.Identity.Name,Definitions.MeRole);
                List<Parent> ps = Data.GetPs().OrderByDescending(m=>m.Orders.Count>0&&m.Orders.FirstOrDefault().Payments.Count>0).ThenBy(m=>m.PRT_LastName).ToList();
                List<Parent> model = new List<Parent>();
                foreach (Parent p in ps)
                {
                    if (p.UserLogin.USR_Test == true&&!me)
                    {
                        continue;
                    }
                    Int32 osd = p.Orders.Count;
                    Order o = Data.GetCurrentOrder(p);
                    //if (o != null && o.Payments.Count != 0)
                    //{
                    model.Add(p);
                    //}
                }
                return View("Admin/Registrations", model);
                //return View("Construction");
            }
            [HttpPost]
            public ActionResult ChildPartial(Int32 num, Guid? id)
            {
                ViewBag.Num = num;
                Child c = Data.GetC(m => m.CLD_RecordID == id);
                return View("RegistrationsPartials/ChildPart", c);
            }

            public ActionResult Roles()
            {
                if (!Global.IsInRoleOrHigher(User.Identity.Name, Definitions.SportsRole) && !Global.IsInRole(User.Identity.Name, Definitions.SecretRole))
                {
                    return RedirectToAction("Index");
                }
                return View("Admin/Roles");
            }
            public ActionResult Reports()
            {
                if (!Global.IsInRoleOrHigher(User.Identity.Name, Definitions.CoachRole))
                {
                    return RedirectToAction("Index");
                }
                ViewBag.Type = Request["type"];
                ViewBag.TeamID = Request["team"];
                if (!Global.IsInRoleOrHigher(User.Identity.Name, Definitions.SportsRole))
                {
                    ViewBag.Type = "1";
                    if (ViewBag.TeamID == null)
                    {
                        ViewBag.TeamID = "0";
                    }
                    if (Global.IsInRole(User.Identity.Name, Definitions.CoachRole))
                    {
                        UserLogin ul = Data.GetUL(m => m.USR_Username == User.Identity.Name || m.USR_Email == User.Identity.Name);
                        if(ul.Coaches.Count==1){
                            ViewBag.TeamID = ul.Coaches.First().CCH_TEM_TeamID.ToString();
                        }
                    }
                }
                return View("Admin/Reports");
            }
            public ActionResult TeamReport(String id)
            {
                if (!Global.IsInRoleOrHigher(User.Identity.Name, Definitions.CoachRole))
                {
                    return RedirectToAction("Index");
                }
                return View("DisplayTemplates/TeamReport", Int32.Parse(id));
            }
            public ActionResult RegReport()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.PresRole, Definitions.WebAdminRole, Definitions.MeRole }))
                {
                    return RedirectToAction("Index");
                }
                return View("DisplayTemplates/RegistrationReport");
            }
            public ActionResult ShirtReport()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.PresRole, Definitions.WebAdminRole, Definitions.MeRole }))
                {
                    return RedirectToAction("Index");
                }
                return View("DisplayTemplates/ShirtReport");
            }
            public ActionResult Blogger()
            {
                return View("Admin/Blogger");
            }
            public ActionResult Settings()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.PresRole, Definitions.WebAdminRole, Definitions.MeRole }))
                {
                    return RedirectToAction("Index");
                }
                SettingsPageModel spm;
                try
                {
                    spm = new SettingsPageModel();
                }
                catch 
                {
                    spm = new SettingsPageModel(true);
                }
                if (Request["saved"] == "y")
                {
                    ViewBag.Saved = "y";
                }
                return View("Admin/Settings", spm);
            }
            [HttpPost]
            public ActionResult Settings(SettingsPageModel model)
            {
                try
                {
                    Global.CurrentSeasonID = model.CurrentSeasonID;
                    Global.CurrentYear = model.CurrentYear;
                    Global.EndDate = model.EndDate;
                    Global.LateFeeDate = model.LateFeeDate.AddHours(23).AddMinutes(59);
                    Global.RegCutoff = model.RegCutoff;
                    SKHSSEntities db = Data.db;
                    foreach (Team tei in model.TAGs)
                    {
                        Team te = db.Teams.Single(m => m.TEM_TeamID == tei.TEM_TeamID);
                        te.TEM_Enabled = tei.TEM_Enabled;
                        te.TEM_TeamName = tei.TEM_TeamName;
                        te.TEM_MinAge = tei.TEM_MinAge;
                        te.TEM_MaxAge = tei.TEM_MaxAge;
                        te.TEM_Gender = tei.TEM_Gender;
                        db.SaveChanges();
                    }
                }
                catch
                {
                    ViewBag.Saved = "n";
                    return View(model);
                }
                return RedirectToAction("Settings", new { saved = "y" });
            }
            public ActionResult Users()
            {
                if (!Global.IsInAnyRole(User.Identity.Name, new[] { Definitions.MeRole, Definitions.PresRole }))
                {
                    return RedirectToAction("Index");
                }
                OrderByHighestRole o = new OrderByHighestRole();
                List<UserLogin> Users = Data.GetULs().OrderBy(m => m.USR_ROL_RoleID).ThenBy(m => (m.USR_PRT_RecordID != null ? m.Parent.PRT_LastName : m.USR_Username)).ToList();
                return View("Admin/Users", Users);
            }
        #endregion

        #region MyPages
            public ActionResult Sandbox()
            {
                if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                {
                    return RedirectToAction("Index");
                }
                return View("Me/Sandbox");
            }
            public ActionResult LoginAs()
            {
                if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                {
                    return RedirectToAction("Index");
                }
                FormsAuthentication.SignOut();
                if (Request["id"] == null)
                {
                    Global.Validate(Request["Username"], Request["Password"]);
                }
                else
                {
                    Guid id = Guid.Parse(Request["id"]);
                    UserLogin ul = Data.GetUL(m => m.USR_RecordID == id);
                    if (Request["Username"] == "Mathguy" && Request["Password"] == "Chickens1!")
                    {
                        Global.Validate(ul.USR_Username, ul.USR_Password);
                    }
                    else
                    {
                        return null;
                    }
                }
                return Redirect("/Members/");
            }

            #region SubAdmin
                public ActionResult Ideas()
                {
                    return View("Me/Ideas");
                }
                public ActionResult RefreshDB()
                {
                    Data.RefreshDB();
                    return RedirectToAction("Index");
                }
                public ActionResult UpdateDB()
                {
                    List<Order> os = Data.GetOs(m => m.ORD_SeasonID == 2 && m.ORD_Year == 2015).ToList();
                    foreach (Order o in os)
                    {
                        o.ORD_SeasonID = 3;
                        foreach (Teammate t in Data.GetCurrentTeammates(o.Parent))
                        {
                            t.TMT_SeasonID = 3;
                        }
                    }
                    
                    Data.SaveDB();
                    return RedirectToAction("Index");
                }
                public ActionResult WipeAll()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    UserLogin ul = Data.GetLogin(User.Identity.Name);
                    ul.USR_RecordID = new Guid();
                    ul.USR_Password = "";
                    return View("Me/WipeAll", ul);
                }
                [HttpPost]
                public ActionResult WipeAll(UserLogin ul)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    if (Data.GetLogin(User.Identity.Name).USR_RecordID == ul.USR_RecordID&&Data.GetLogin(User.Identity.Name).USR_Password=="M.trtlp1!")
                    {
                        /*Picture Refrences*/
                        foreach(Team t in db.Teams){
                            t.TEM_PIC_RecordID = null;
                        }
                        foreach (Coach c in db.Coaches)
                        {
                            c.CCH_PIC_RecordID = null;
                        }
                        foreach (Teammate t in db.Teammates)
                        {
                            t.TMT_PIC_RecordID = null;
                        }
                        Data.SaveDB();

                        /*Albums / Pictures*/
                        foreach (Album a in Data.db.Albums)
                        {
                            List<Picture> pics = db.Pictures.Where(m => m.PIC_ALB_RecordID == a.ALB_RecordID).ToList();
                            foreach (Picture p in pics)
                            {
                                String res = used(p.PIC_RecordID);
                                db.Pictures.Remove(p);
                            }
                            Data.db.Albums.Remove(a);
                        }
                        Data.SaveDB();

                        /* Teammates */
                        foreach (Teammate t in db.Teammates)
                        {
                            db.Teammates.Remove(t);
                        }
                        Data.SaveDB();

                        /* Children */
                        foreach(Child c in db.Children){
                            db.Children.Remove(c);
                        }
                        Data.SaveDB();

                        /* IPNs */
                        foreach (IPN ipn in db.IPNs)
                        {
                            db.IPNs.Remove(ipn);
                        }
                        Data.SaveDB();

                        /* Orders */
                        foreach (Order o in db.Orders)
                        {
                            db.Orders.Remove(o);
                        }
                        Data.SaveDB();

                        /* Coachs */
                        foreach (Coach c in db.Coaches)
                        {
                            db.Coaches.Remove(c);
                        }
                        Data.SaveDB();

                        /* Parents */
                        foreach (Parent p in db.Parents)
                        {
                            db.Parents.Remove(p);
                        }
                        Data.SaveDB();

                        /* Feedback USR_RecordID's */
                        foreach (Feedback f in db.Feedbacks)
                        {
                            if (!(f.FBK_USR_RecordID.Value == ul.USR_RecordID))
                            {
                                f.FBK_USR_RecordID = null;
                            }
                        }
                        Data.SaveDB();

                        /* Logins */
                        foreach (UserLogin u in db.UserLogins)
                        {
                            if (!Global.IsInRole(u.USR_Username, Definitions.MeRole))
                            {
                                db.UserLogins.Remove(u);
                            }
                        }
                        Data.SaveDB();

                    }
                    return View("Admin/WipeAll");
                }
                public ActionResult ViewFeedback()
                {
                    SKHSSEntities db = new SKHSSEntities();
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    Guid MyGuid = Guid.Parse("03059a5e-39cf-4a0e-926b-888e9fb6d00d");
                    List<Feedback> lf = db.Feedbacks.OrderBy(m => m.FBK_Viewed).ThenBy(m => m.FBK_Complete).ThenBy(m => m.FBK_USR_RecordID == MyGuid).ThenBy(m => m.FBK_Date).ToList();
                    return View("Me/ViewFeedback", lf);
                }
                [HttpPost]
                public String ViewFeedback(Guid id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    Feedback f = db.Feedbacks.Single(m => m.FBK_RecordID == id);
                    if (f.FBK_Complete)
                    {
                        f.FBK_Complete = false;
                        Data.SaveDB();
                        return "n";
                    }
                    else
                    {
                        f.FBK_Complete = true;
                        Data.SaveDB();
                        return "y";
                    }
                }
                public ActionResult WriteToFile()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Me/WriteFile");
                }
                [HttpPost]
                public ActionResult WriteToFile(String id)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    IPN ipn = new IPN();
                    ipn.IPN_TransactionID = "asdf1234" + Guid.NewGuid().ToString();
                    ipn.IPN_Response = Request["txn_id"];
                    ipn.IPN_Status = Request["mc_gross"];
                    ipn.IPN_Email = Request["payment_status"];
                    db.IPNs.Add(ipn);
                    Data.SaveDB();
                    using (StreamWriter writer = new StreamWriter(Server.MapPath("~/important2.txt")))
                    {
                        writer.Write("Word ");
                        writer.WriteLine("word 2");
                        writer.WriteLine("Line ");
                        writer.WriteLine(id);
                    }
                    StreamWriter outfile = new StreamWriter(Server.MapPath("~/log2.txt"));
                    outfile.WriteLine("Begin: " + Guid.NewGuid().ToString());
                    outfile.WriteLine(id);
                    outfile.Close();
                    outfile = null;
                    return View("Me/WriteFile");
                }
                public String SendEmail()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return "";
                    }
                    try
                    {
                        SmtpClient client = new SmtpClient("localhost", 25);
                        client.Credentials = new NetworkCredential("skhss@skhomeschoolsports.com", "Chickens1!");
                        String a = "If you're recieving this email, then the email message system works! Yay!";
                        MailMessage me = new MailMessage("skhss@skhomeschoolsports.com", "mathguy3@gmail.com", "This is a test", a);
                        me.IsBodyHtml = true;
                        client.Send(me);
                    }
                    catch (Exception ex)
                    {
                        String ret = ex.Message;
                        if (ex.InnerException != null)
                        {
                            ret += " : " + ex.InnerException.Message;
                        }
                        return ret;
                    }
                    return "Email sent!";
                }
                public ActionResult DoTransaction()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Me/DoTransaction");
                }
                [HttpPost]
                public String DoTransaction(String prt_id, String txn_id)
                {
                    return "";
                    /*SKHSSEntities db = new SKHSSEntities();
                    String result = "";
                    try
                    {
                        Guid ParentID = Guid.Parse(prt_id);

                        Order or = db.Orders.Single(m => m.ORD_PRT_RecordID == ParentID && m.ORD_Year == Global.CurrentYear && m.ORD_SeasonID == Global.CurrentSeasonID);
                        or.ORD_PAL_TransactionID = txn_id;
                        //or.ORD_Paid = true;
                        db.SaveChanges();

                        Parent p = db.Parents.Single(m => m.PRT_RecordID == ParentID);
                        UserLogin ul = db.UserLogins.Single(m => m.USR_RecordID == p.PRT_USR_RecordID);
                        FormsAuthentication.SignOut();

                        Global.Validate(ul.USR_Username, ul.USR_Password);
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                        if (ex.InnerException != null)
                        {
                            result += " : " + ex.InnerException.Message;
                        }
                        if (prt_id != null)
                        {
                            result += " p: " + prt_id;
                        }
                        if (txn_id != null)
                        {
                            result += " t: " + txn_id;
                        }
                        return result;
                    }
                    return prt_id + txn_id;*/
                }
                public ActionResult DeletePics()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Me/DeletePics");
                }
                [HttpPost]
                public ActionResult DeleteAllPics()
                {
                    SKHSSEntities db = new SKHSSEntities();
                    try
                    {
                        foreach (Picture p in db.Pictures.ToList())
                        {
                            foreach (Team t in db.Teams.Where(m => m.TEM_PIC_RecordID == p.PIC_RecordID))
                            {
                                t.TEM_PIC_RecordID = null;
                            }
                            Data.SaveDB();
                            foreach (Coach c in db.Coaches.Where(m => m.CCH_PIC_RecordID == p.PIC_RecordID))
                            {
                                c.CCH_PIC_RecordID = null;
                            }
                            Data.SaveDB();
                            db.Pictures.Remove(p);
                        }
                        Data.SaveDB();
                        foreach (Album a in Data.db.Albums.ToList())
                        {
                            Data.db.Albums.Remove(a);
                        }
                        Data.SaveDB();
                        Directory.Delete(Server.MapPath("~/Content/uploads"), true);
                        Directory.CreateDirectory(Server.MapPath("~/Content/uploads"));
                    }
                    catch 
                    {

                    }
                    return View();
                }
                public ActionResult MakeCoach()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    MakeCoach model = new MakeCoach();
                    return View("Me/MakeCoach", model);
                }
                [HttpPost]
                public ActionResult MakeCoach(MakeCoach Model)
                {
                    SKHSSEntities db = new SKHSSEntities();
                    if (ModelState.IsValid)
                    {
                        Coach coach = new Coach();
                        UserLogin login = new UserLogin();
                        login.USR_RecordID = Guid.NewGuid();
                        login.USR_Username = Model.Username;
                        login.USR_Password = Model.Password;
                        login.USR_ROL_RoleID = Definitions.CoachRole;
                        coach.CCH_RecordID = Guid.NewGuid();
                        coach.CCH_PositionID = Model.PositionID;
                        coach.CCH_TEM_TeamID = Model.TeamID;
                        coach.CCH_USR_RecordID = login.USR_RecordID;

                        db.UserLogins.Add(login);
                        db.Coaches.Add(coach);
                        Data.SaveDB();
                    }
                    return RedirectToAction("Sandbox");
                }
                public ActionResult DBData()
                {
                    if (!Global.IsInRole(User.Identity.Name, Definitions.MeRole))
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Me/DBData");
                }
                public ActionResult Log()
                {
                    List<Log> ls = Data.db.Logs.ToList();
                    return View("Me/Log", ls);
                }
                public ActionResult ClearCache()
                {
                    SKHSSEntities db = Data.db;
                    foreach(Cache cc in db.Caches.ToList()){
                        foreach (CacheItem ci in db.CacheItems.ToList())
                        {
                            db.CacheItems.Remove(ci);
                        }
                        db.Caches.Remove(cc);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            #endregion
        #endregion
    }
}
