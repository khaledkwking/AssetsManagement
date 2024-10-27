using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using System.IO;
using System.Configuration;

namespace AssetsManagement.Controllers
{
    //[Authorize]
    public class ErrorController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        public ActionResult Forbidden()
        {    
                return View();
        }
        public ActionResult InternalError()
        {
            return View();
        }

        public ActionResult Oops()
        {
            return View();
        }
        public ActionResult Oops404()
        {
            return View();
        }
    }
}