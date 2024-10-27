using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssetsManagement.Controllers
{
    public class UnauthorisedController : Controller
    {
        // GET: Unauthorised
        [HttpGet]
        public ActionResult Index()
        {
            //Session.Abandon();
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
               
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string  returnUrl)
        {

            //string Uri = returnUrl; //System.Web.HttpContext.Current.Request.UrlReferrer.ToString ();
            //return Redirect(Uri);
            return RedirectToAction("Index", "Home");
          
        }
    }
}