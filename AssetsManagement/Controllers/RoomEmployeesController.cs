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
    public class RoomEmployeesController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        // GET: RoomEmployees
        public ActionResult RoomEmployeesList( string RoomId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            EmpRoomsViewModel model = new EmpRoomsViewModel();
                   
            List<Emp_rooms> RoomEmpsList = unitWork.EmpRoomsManager.GetNotDelAll(RoomId).OrderBy(m => m.Id).ToList();
            model.SelectedRoomEmp = null;
      
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Id" ? "Id" : "Id";
            ViewBag.RoomId = RoomId;

            if (!String.IsNullOrEmpty(RoomId) && RoomEmpsList.Count >0)
            {
                Room_tbl ObjRoom = RoomEmpsList.FirstOrDefault().Room_tbl;
                long CurRoomId = long.Parse(RoomId);
                ViewBag.RoomId = RoomId;
                //Room_tbl ObjRoom = unitWork.RoomsManager.GetById(CurRoomId);
                if (ObjRoom != null)
                {
                    ViewBag.RoomName = ObjRoom.Room_Name;
                }

                if (ObjRoom.StoreFlag)
                {
                    ViewBag.StoreFlag = 1;
                }
                else { ViewBag.StoreFlag = 0; }

                ViewBag.FloorId = ObjRoom.Floor_Id;
                Floor_tbl ObjFloor = unitWork.FloorsManager.GetById(ObjRoom.Floor_Id);
                if (ObjFloor != null)
                {
                    ViewBag.BuildingId = ObjFloor.Building_Id;
                    Building_tbl ObjBuilding = unitWork.BuildingsManager.GetById(ObjFloor.Building_Id);
                    ViewBag.FloorName = ObjFloor.Floor_Name;
                    ViewBag.BuildingName = ObjBuilding.Building_Name;
                }

            }
        
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
               
                RoomEmpsList = unitWork.EmpRoomsManager .SearchByString(Search_Data);
               
            }
            switch (Sorting_Order)
            {
                case "ID":
                    RoomEmpsList = RoomEmpsList.OrderByDescending(stu => stu.Id).ToList();
                    break;
                case "EmpName":
                    //RoomEmpsList = RoomEmpsList.OrderByDescending(stu => stu.VmEmployees.ManagerFullNameAr).ToList();
                    break;
                case "CivilId":
                    //RoomEmpsList = RoomEmpsList.OrderByDescending(stu => stu.VmEmployees.Civil_Id).ToList();
                    break;
                default:
                    RoomEmpsList = RoomEmpsList.OrderBy(stu => stu.Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.EmpRooms = RoomEmpsList.ToPagedList(No_Of_Page, Size_Of_Page);
            
            if (RoomEmpsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }


        }



        [HttpPost]
        [ActionName("DeleteRoomEmps")]
        public ActionResult DeleteRoomEmps(string id)
        {
            int Id = int.Parse(id);
            Emp_rooms existing = unitWork.EmpRoomsManager.GetById(Id);
            if (existing != null)
            {
                List<tbl_ItemsStock> ItemsStockList = unitWork.ItemsStockManager.GetNotDelByDeptId(existing.DeptId.GetValueOrDefault(), existing.RoomId, existing.EmpId);
                if (ItemsStockList.Count == 0)
                {
                    unitWork.EmpRoomsManager.Delete(existing);
                }
                else
                {
                    TempData["ErrorMessage"] = Resources.DefaultResource.AssetsInDepartmentMsg;
                }
                //unitWork.BuildingsManager.Update(existing);
                

                //Floor_tbl Floorobj = unitWork.FloorsManager.GetById(existing.);
                //int Building_Id = 0;
                //if (Floorobj != null)
                //{
                //    Building_Id = Floorobj.Building_Id;
                //}

                EmpRoomsViewModel model = new EmpRoomsViewModel();
                model.EmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().OrderBy(m => m.Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.EmpRooms = null;
                model.DisplayMode = "";

                return RedirectToAction("RoomEmployeesList", new { RoomId = existing.RoomId });
            }
            else
            {
                return RedirectToAction("RoomEmployeesList");
            }
        }




        [HttpGet]
        [ActionName("AddEditRoomEmps")]
        public ActionResult AddEditRoomEmps(string id = null, string RoomId = null)
        {

            EmpRoomsViewModel model = new EmpRoomsViewModel();

            var allDeptsList = unitWork.DepartmentManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            var defaultDeptId = allDeptsList.Select(m => m.Id).FirstOrDefault();
            //get all Categories according to defaultCountryId
            var allEmployeesList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == defaultDeptId).ToList();
            var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();

          

            //model.Departments = unitWork.DepartmentManager.GetAll().Select(option => new SelectListItem
            //{
            //    Text = option.Name,
            //    Value = option.Id.ToString()
            //});

            //model.Employees = unitWork.EmployeesManager.GetAll().Select(option => new SelectListItem
            //{
            //    Text = option.FULL_NAME_AR,
            //    Value = option.Id.ToString()
            //});

            if (String.IsNullOrEmpty(id))
            {
                Emp_rooms RoomEmpRecord = new Emp_rooms();
                model.Departments = new SelectList(allDeptsList, "Id", "Name", defaultDeptId);
                model.Employees = new SelectList(allEmployeesList, "Id", "FULL_NAME_AR", defaultEmpId);
                if (RoomId  != null)
                {
                    RoomEmpRecord.RoomId = int.Parse(RoomId);
                }
                model.SelectedRoomEmp = RoomEmpRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddRoomEmpsModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.EmpRooms = unitWork.EmpRoomsManager.GetNotDelAll().OrderBy(m => m.Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedRoomEmp = unitWork.EmpRoomsManager.GetById(ModelId);

                model.Departments = new SelectList(allDeptsList, "Id", "Name", model.SelectedRoomEmp.DeptId);
                var EmpsList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == model.SelectedRoomEmp.DeptId).ToList();
                model.Employees = new SelectList(EmpsList, "Id", "FULL_NAME_AR", model.SelectedRoomEmp.EmpId);

                model.DisplayMode = "ReadWrite";
                if (model.SelectedRoomEmp == null) { return View("_error"); }
                // ...
                return PartialView("EditAddRoomEmpsModal", model);
            }

        }

        [Route("SaveRoomEmps")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveRoomEmps(EmpRoomsViewModel RoomEmps)
        {
            Emp_rooms existing = RoomEmps.SelectedRoomEmp ;
            if (ModelState.IsValid || RoomEmps.SelectedRoomEmp.Id >=0)
            {

                if (RoomEmps.SelectedRoomEmp.DeptId != null)
                {
                    if (RoomEmps.SelectedRoomEmp.Id == 0) // add new record
                    {
                        // insert new record

                        Emp_rooms RoomEmpobj = unitWork.EmpRoomsManager.Add(existing);
                        if (RoomEmpobj != null)
                        {
                            TempData["Message"] = Resources.DefaultResource.SaveMessage;
                        }
                        else
                        {
                            TempData["Message"] = null;
                            TempData["ErrorMessage"] = Resources.DefaultResource.QuataMessage;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.EmpRoomsManager.GetById(RoomEmps.SelectedRoomEmp.Id);
                        if (existing.DeptId != RoomEmps.SelectedRoomEmp.DeptId && existing.EmpId != null)
                        {
                            TempData["Message"] = null;
                            TempData["ErrorMessage"] = Resources.DefaultResource.ChangeEmpRoomMsg;
                        }
                        else
                        {
                            existing.RoomId = RoomEmps.SelectedRoomEmp.RoomId;
                            existing.DeptId = RoomEmps.SelectedRoomEmp.DeptId;
                            existing.EmpId = RoomEmps.SelectedRoomEmp.EmpId;
                            existing.QuotaId  = RoomEmps.SelectedRoomEmp.QuotaId;
                            bool ret = unitWork.EmpRoomsManager.Update(existing);
                            // update record
                            if (ret)
                            {
                                TempData["Message"] = Resources.DefaultResource.SaveMessage;
                            }
                            else
                            {
                                TempData["Message"] = null;

                            }
                        }
                    }
                }
                else
                {
                    TempData["Message"] = null;
                    TempData["ErrorMessage"] = Resources.DefaultResource.DepartmentErrorMessage;
                    //ModelState.AddModelError("LastName", "The last name cannot be the same as the first name.");
                }

            }
                    
          
            EmpRoomsViewModel model = new EmpRoomsViewModel();
            //model.EmpRooms  = unitWork.EmpRoomsManager.GetNotDelAll().OrderBy(m => m.Id ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.SelectedRoomEmp  = existing;
            model.DisplayMode = "ReadOnly";

            return RedirectToAction("RoomEmployeesList", new { RoomId =  existing.RoomId });

        }

        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            EmpRoomsViewModel model = new EmpRoomsViewModel();
            DepartementsViewModel DeptModel = new DepartementsViewModel();
            if (type == "DeptId") { type = "DeptId"; }
            switch (type)
            {
                case "DeptId":             
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.Employees = DeptModel.vwEmployees;
                    break;
            }
                    

            return Json(model);
        }
        // End of Rooms Module
    }
}