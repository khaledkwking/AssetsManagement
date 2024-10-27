using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using DAL;
using BOL;


using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web.Security;

namespace AssetsManagement.Controllers
{

    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            if (Session["UserProfile"] != null)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                string username = profileData.LoginName;
                return View("Index");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        
        public ActionResult SetLanguage(string lang)
        {
            try
            {
                if (lang == "ar")
                {
                    Session["SiteLanguage"] = "ArabicTheme";
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
                }
                else if (lang == "en")
                {
                    Session["SiteLanguage"] = "EnglishTheme";
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                }
                //return View();
                return Redirect(Request.UrlReferrer.AbsoluteUri);
                //return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string Lang)
        {
            tbUsers objUser = new tbUsers();
            if (Lang == "en")
            {
                Session["SiteLanguage"] = "EnglishTheme";
                ViewBag.SignIn = "Login to System";
                ViewBag.UserTitle = "Username";
                ViewBag.PassTitle = "Password";
                //ViewBag.lblNotes = "Default  UserName: test   and  Password: test";
            }
            else
            {
                Session["SiteLanguage"] = "ArabicTheme";
                ViewBag.SignIn = "تسجيل الدخول";
                ViewBag.UserTitle = " اسم المستخدم";
                ViewBag.PassTitle = " كلمة المرور";
                //ViewBag.lblNotes = "أسم المستخدم الافتراضى: test     كلمة المرور : test";
            }
            if (Session["SiteLanguage"] == null)
            {
                Session["SiteLanguage"] = "ArabicTheme";   
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");

            }
            else if (Session["SiteLanguage"].ToString() == "ArabicTheme")
            {
               
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
            }


            return View(objUser);
        }
          
         
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(tbUsers objUser,string Lang)
        {
            //if (ModelState.IsValid)
            //{

            if (Session["SiteLanguage"].ToString() == "ArabicTheme")
            {
                Session["SiteLanguage"] = "ArabicTheme";
                ViewBag.SignIn = "تسجيل الدخول";
                ViewBag.UserTitle = " اسم المستخدم";
                ViewBag.PassTitle = " كلمة المرور";
                
            }
            else
            {
                Session["SiteLanguage"] = "EnglishTheme";
                ViewBag.SignIn = "Login to System";
                ViewBag.UserTitle = "Username";
                ViewBag.PassTitle = "Password";
               
            }
           
            //if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

           
            // Check user in active dirtory
            //if (!sessionUser.ValidateUser()) { return View("Login"); }




            UnitOfWork uWork = new UnitOfWork();
            // check username in database
            
            var obj = uWork.UsersManager.GetEmpByPassord(objUser.UserName , objUser.Password  );
            //var obj = uWork.UsersManager.GetByUserName(objUser.UserName);
            if (obj != null)
            {
                var sessionUser = new SesssionUser();
                sessionUser.LoginName = objUser.UserName;
                sessionUser.Password = objUser.Password;
                
                sessionUser.UserId = int.Parse (obj.UserID.ToString());
                Session["UserProfile"] = sessionUser;
                Session["UserName"] = obj.UserName.ToString();
                Session["UserID"] = obj.UserID .ToString();
                Session["RoleID"] = obj.RoleID.ToString();

                sessionUser.UserId = int.Parse(obj.UserID.ToString());
                sessionUser.LoginName = obj.FullName.ToString();
               
                sessionUser=SetupFormsAuthTicket(sessionUser, false);
                //FormsAuthentication.SetAuthCookie(sessionUser.LoginName, false);

                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"]="<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + Resources.DefaultResource.Error + "  :  " + Resources.DefaultResource.MsgToLogin + "</h6></div>";
                return View("Login");
            }

            //}
            return RedirectToAction("Index", "ErrorLogs");
            //return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            if (Session["UserProfile"] != null)
            {
                //Session["UserProfile"] = null;
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Login");
        }
        private SesssionUser SetupFormsAuthTicket(SesssionUser user, bool persistanceFlag)
        {
            var userData = user.UserId.ToString(CultureInfo.InvariantCulture);

            var authTicket = new FormsAuthenticationTicket(1, //version
                                user.LoginName, // user name
                                DateTime.Now,   //creation
                                DateTime.Now.AddMinutes(30), //Expiration
                                persistanceFlag, //Persistent
                                user.UserId.ToString());
            
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie CookieLogin = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            CookieLogin.Expires = authTicket.Expiration;
            Response.Cookies.Add(CookieLogin);
            return user;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }
        //public ActionResult ChangeLang(string Lang)
        //{
        //    if (Lang == "Eng")
        //    {
        //        Session["SiteLanguage"] = "EnglishTheme";
        //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        //        //Response.Redirect(Request.RawUrl);
        //    }
        //    else
        //    {
        //        Session["SiteLanguage"] = "ArabicTheme";
        //        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("ar-KW");
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-KW");
        //        //Response.Redirect(Request.RawUrl);

        //    }
        //    return RedirectToAction("Index", "Home");
        //}

    }
}