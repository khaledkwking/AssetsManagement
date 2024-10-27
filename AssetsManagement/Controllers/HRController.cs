using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
namespace AssetsManagement.Controllers
{
    public class HRController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        // GET: HR
        public ActionResult Index()
        {
            return View();
        }
        [UserPermissionAttribute(AllowFeature = "Departments", AllowPermission = "Accessing")]
        public ActionResult Departements(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string Id = null)
        {
            bool islogin = User.Identity.IsAuthenticated;
            string username = User.Identity.Name;
            var profileData = Session["UserProfile"] as SesssionUser;

            //List<Car> carList = null;
            DepartementsViewModel model = new DepartementsViewModel();
            List<vwDepartments> UnitList;
            if (String.IsNullOrEmpty(Id))
            {
                UnitList = unitWork.DepartmentManager.GetNotDelAll().OrderBy(m => m.Id).ToList();
            }
            else
            {
                UnitList = unitWork.DepartmentManager.GetParentId(int.Parse (Id)).OrderBy(m => m.Id).ToList();
            }
                model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Id" ? "Name" : "Name";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
                UnitList = unitWork.DepartmentManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ID":
                    UnitList = UnitList.OrderByDescending(stu => stu.Id).ToList();
                    break;
                case "DepartementName":
                    UnitList = UnitList.OrderBy(stu => stu.Name).ToList();
                    break;
                case "ManagerName":
                    UnitList = UnitList.OrderBy(stu => stu.ManagerFullNameEn).ToList();
                    break;
                default:
                    UnitList = UnitList.OrderBy(stu => stu.Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Departements = UnitList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UnitList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [UserPermissionAttribute(AllowFeature = "Employees", AllowPermission = "Accessing")]
        public ActionResult Employees(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,Boolean ? AcativeFlag)
        {
            bool islogin = User.Identity.IsAuthenticated;
            string username = User.Identity.Name;
            var profileData = Session["UserProfile"] as SesssionUser;

            //List<Car> carList = null;
            EmployeesViewModel model = new EmployeesViewModel();
            model.AcativeFlag = AcativeFlag.GetValueOrDefault();
            List<vwEmployees> UnitList;
            if (model.AcativeFlag == true)
            {
                UnitList = unitWork.EmployeesManager.GetDelAll().OrderBy(m => m.Id).ToList();
                model.AcativeFlag = true;
                ViewBag.Acative = true;
            }
            else
            {
                UnitList = unitWork.EmployeesManager.GetNotDelAll().OrderBy(m => m.Id).ToList();
                model.AcativeFlag = false;
                ViewBag.Acative = false;
            }
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Id" ? "Name" : "Name";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
                UnitList = unitWork.EmployeesManager.GetCastByUnitName(Search_Data, model.AcativeFlag);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ID":
                    UnitList = UnitList.OrderByDescending(stu => stu.Id).ToList();
                    break;
                case "TitleAr":
                    UnitList = UnitList.OrderByDescending(stu => stu.FULL_NAME_AR).ToList();
                    break;
                case "Title":
                    UnitList = UnitList.OrderByDescending(stu => stu.FULL_NAME_En).ToList();
                    break;
                case "Email":
                    UnitList = UnitList.OrderByDescending(stu => stu.Email).ToList();
                    break;
                case "DeptTitle":
                    UnitList = UnitList.OrderByDescending(stu => stu.DeptTitle).ToList();
                    break;
                case "JobTitle":
                    UnitList = UnitList.OrderByDescending(stu => stu.JobTitle).ToList();
                    break;
                case "FingerNo":
                    UnitList = UnitList.OrderByDescending(stu => stu.Fingerprint_Id).ToList();
                    break;
                case "CivilId":
                    UnitList = UnitList.OrderByDescending(stu => stu.Civil_Id).ToList();
                    break;
                  default:
                    UnitList = UnitList.OrderByDescending(stu => stu.Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Employees = UnitList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UnitList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
    }
}