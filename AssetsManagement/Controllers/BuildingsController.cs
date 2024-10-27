using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace AssetsManagement.Controllers
{
    //[Authorize]

    public class BuildingsController : Controller
    {

        // GET: Buildings
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        [UserPermissionAttribute(AllowFeature = "Building", AllowPermission = "Accessing")]
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            bool islogin = User.Identity.IsAuthenticated;
            string username = User.Identity.Name;
            var profileData = Session["UserProfile"] as SesssionUser;

            
            int userId = SesssionUser.GetCurrentUserId();
            tbUsersPages UsersPages = unitWork.UsersManager.UserPermission(userId, "Building"); // is not system admin
            if (UsersPages != null)
            {
                ViewBag.Accessing = UsersPages.Accessing;
                ViewBag.Adding = UsersPages.Adding;
                ViewBag.Updating = UsersPages.Updating;
                ViewBag.Deleting = UsersPages.Deleting;
                ViewBag.Importing = UsersPages.Importing;
            }
            tbUsersPages UsersPagesFloors = unitWork.UsersManager.UserPermission(userId, "Floors"); // is not system admin
            if (UsersPages != null)
            {
                ViewBag.FloorsAccessing = UsersPagesFloors.Accessing;

            }

            //List<Car> carList = null;
            BuildingsViewModel model = new BuildingsViewModel();
            List<Building_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).ToList();
            model.SelectedBuilding = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Building_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Building_Id" ? "Building_Name" : "Building_NameEn";

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
                UnitList = unitWork.BuildingsManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "Building_Id":
                    UnitList = UnitList.OrderByDescending(stu => stu.Building_Id).ToList();
                    break;
                case "Building_Name":
                    UnitList = UnitList.OrderByDescending(stu => stu.Building_Name).ToList();
                    break;
                case "Building_NameEn":
                    UnitList = UnitList.OrderByDescending(stu => stu.Building_NameEn).ToList();
                    break;
                default:
                    UnitList = UnitList.OrderByDescending(stu => stu.Building_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Buildings = UnitList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UnitList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        //[HttpPost]

        //public ActionResult New()
        //{

        //    BuildingsViewModel model = new BuildingsViewModel();
        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).ToPagedList(No_Of_Page, Size_Of_Page);

        //    model.SelectedBuilding = null;
        //    model.DisplayMode = "WriteOnly";
        //    return PartialView("EditUnitModal", model.SelectedBuilding);
        //    // return View("Index", model);

        //}
        //[HttpPost]
        //public ActionResult Insert(Building_tbl obj)
        //{
        //    BuildingsViewModel model = new BuildingsViewModel();
        //    unitWork.BuildingsManager.Add(obj);
        //    unitWork.BuildingsManager.Update(obj);

        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
        //    model.SelectedBuilding = unitWork.BuildingsManager.GetById(obj.Building_Id);  //officeList..GetById((obj.Unit_Id);
        //    model.DisplayMode = "ReadOnly";
        //    return View("Index", model);

        //}
        //[HttpPost]
        //public ActionResult Select(string id)
        //{

        //    BuildingsViewModel model = new BuildingsViewModel();
        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
        //    int Unit_Id = int.Parse(id);
        //    model.SelectedBuilding = unitWork.BuildingsManager.GetById(Unit_Id);
        //    model.DisplayMode = "ReadOnly";
        //    return View("Index", model);
        //}
        //[HttpPost]
        //public ActionResult Edit(string id)
        //{
        //    int Unit_Id = int.Parse(id);
        //    BuildingsViewModel model = new BuildingsViewModel();
        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
        //    model.SelectedBuilding = unitWork.BuildingsManager.GetById(Unit_Id);
        //    model.DisplayMode = "ReadWrite";
        //    return View("Index", model);

        //}
        //[HttpPost]
        //public ActionResult Update(Building_tbl obj)
        //{

        //    Building_tbl existing = unitWork.BuildingsManager.GetById(obj.Building_Id);
        //    existing.Building_Id = obj.Building_Id;
        //    existing.Building_Name = obj.Building_Name;
        //    existing.Building_NameEn = obj.Building_NameEn;
        //    existing.Building_Code = obj.Building_Code;

        //    //existing.UpdateBy = 1;
        //    //existing.UpdateOn =

        //    unitWork.BuildingsManager.Update(existing);

        //    BuildingsViewModel model = new BuildingsViewModel();
        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);

        //    model.SelectedBuilding = existing;
        //    model.DisplayMode = "ReadOnly";
        //    return View("Index", model);

        //}

        //[HttpPost]
        //public ActionResult Cancel(string id)
        //{
        //    int Unit_Id = int.Parse(id);
        //    BuildingsViewModel model = new BuildingsViewModel();
        //    model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;
        //    model.SelectedBuilding = unitWork.BuildingsManager.GetById(Unit_Id);
        //    model.DisplayMode = "ReadOnly";
        //    return View("Index", model);
        //}


        [UserPermissionAttribute(AllowFeature = "Building", AllowPermission = "Deleting")]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            int Unit_Id = int.Parse(id);
            Building_tbl existing = unitWork.BuildingsManager.GetById(Unit_Id);
            if (existing != null)
            {
                unitWork.BuildingsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                BuildingsViewModel model = new BuildingsViewModel();
                model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedBuilding = null;
                model.DisplayMode = "";
                return RedirectToAction("Index");//View("Index", model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Route("SaveBuilding")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveBuilding(Building_tbl Unit)
        {
            Building_tbl existing = Unit;
            if (ModelState.IsValid)
            {

                if (Unit.Building_Id == 0)
                {
                    // insert new record
                    Building_tbl Unitobj = unitWork.BuildingsManager.Add(existing);
                    if (Unitobj != null)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                    ModelState.Clear();
                }
                else
                {
                    existing = unitWork.BuildingsManager.GetById(Unit.Building_Id);
                    existing.Building_Id = Unit.Building_Id;
                    existing.Building_Name = Unit.Building_Name;
                    existing.Building_NameEn = Unit.Building_NameEn;
                    existing.Building_Code = Unit.Building_Code;
                    bool ret = unitWork.BuildingsManager.Update(existing);
                    // update record

                    if (ret)
                    {
                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                }

            }
            //Unit_tbl existing = unitWork.BuildingsManager.GetById(obj.Unit_Id);
            //existing.Unit_Id = obj.Unit_Id;
            //existing.Unit_Name = obj.Unit_Name;
            //existing.Unit_NameEn = obj.Unit_NameEn;

            //unitWork.BuildingsManager.Update(existing);

            BuildingsViewModel model = new BuildingsViewModel();
            model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);

            model.SelectedBuilding = existing;
            model.DisplayMode = "ReadOnly";
            //return View("Index", model);
            return RedirectToAction("Index");
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }

        
        [ActionName("AddEdit")]
        //[UserPermissionAttribute(AllowFeature = "Building", AllowPermission = "Adding")]     
        public ActionResult AddEdit(string id = null)
        {
            
            BuildingsViewModel model = new BuildingsViewModel();
            if (String.IsNullOrEmpty(id))
            {

                Building_tbl UnitRecord = new Building_tbl();
                model.SelectedBuilding = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddModal", model.SelectedBuilding);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Buildings = unitWork.BuildingsManager.GetNotDelAll().OrderBy(m => m.Building_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedBuilding = unitWork.BuildingsManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedBuilding == null) { return View("_error"); }
                // ...
                return PartialView("EditAddModal", model.SelectedBuilding);
            }

        }



        // Start of Floors module
        [UserPermissionAttribute(AllowFeature = "Floors", AllowPermission = "Accessing")]
        public ActionResult FloorsList(string BuildingId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            
            int userId = SesssionUser.GetCurrentUserId();
            tbUsersPages UsersPages = unitWork.UsersManager.UserPermission(userId, "Floors"); // is not system admin
            if (UsersPages!=null)
            {
                ViewBag.Accessing = UsersPages.Accessing;
                ViewBag.Adding = UsersPages.Adding ;
                ViewBag.Updating = UsersPages.Updating;
                ViewBag.Deleting = UsersPages.Deleting;
                ViewBag.Importing = UsersPages.Importing;
            }
            tbUsersPages UsersPagesRoom = unitWork.UsersManager.UserPermission(userId, "Rooms"); // is not system admin
            if (UsersPages != null)
            {
                ViewBag.RoomAccessing = UsersPagesRoom.Accessing;

            }
            if (String.IsNullOrEmpty(BuildingId))
            {
                return View();
            }

            ViewBag.BuildingId = BuildingId.ToString();
            //List<Car> carList = null;
            int Id = int.Parse(BuildingId);
            Building_tbl ObjBuilding = unitWork.BuildingsManager.GetById(Id);
            if (ObjBuilding != null)
            {
                if (ObjBuilding != null)
                    ViewBag.BuildingName = ObjBuilding.Building_Name;
            }

            FloorsViewModel model = new FloorsViewModel();
            List<Floor_tbl> FloorsList = unitWork.FloorsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).ToList();

            FloorsList = FloorsList.Where(r => r.Building_Id == Id).ToList(); //.FirstOrDefault();
            //var flrs = from f in FloorsList orderby f.Building_Id, f.Floor_Id select f;
            //if (flrs != null)
            //{
            //    if (flrs.Count() >= 0)
            //    {
            //        _floors = flrs.ToList();
            //    }
            //}
            model.Floors = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Floor_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Floor_Id" ? "Floor_Name" : "Floor_NameEn";

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
                FloorsList = unitWork.FloorsManager.GetCastByUnitName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ID":
                    FloorsList = FloorsList.OrderByDescending(stu => stu.Building_Id).ToList();
                    break;
                case "Floor_Code":
                    FloorsList = FloorsList.OrderBy(stu => stu.Floor_Code).ToList();
                    break;
                case "Floor_Name":
                    FloorsList = FloorsList.OrderBy(stu => stu.Floor_Name).ToList();
                    break;
                case "Floor_NameEn":
                    FloorsList = FloorsList.OrderByDescending(stu => stu.Floor_NameEn).ToList();
                    break;
                default:
                    FloorsList = FloorsList.OrderBy(stu => stu.Building_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Floors = FloorsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (FloorsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteFloor")]
        public ActionResult DeleteFloor(string id)
        {
            int Id = int.Parse(id);
            Floor_tbl existing = unitWork.FloorsManager.GetById(Id);
            if (existing != null)
            {
                unitWork.FloorsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                FloorsViewModel model = new FloorsViewModel();
                model.Floors = unitWork.FloorsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedFloor = null;
                model.DisplayMode = "";

                return RedirectToAction("FloorsList", new { BuildingId = existing.Building_Id });
            }
            else
            {
                return RedirectToAction("FloorsList");
            }
        }


        [HttpGet]
        [ActionName("AddEditFloor")]
        public ActionResult AddEditFloor(string id = null, string BuildingId = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetNotDelAll();
            //var CurStatusList = unitWork.StatusManager.GetNotDelAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            FloorsViewModel model = new FloorsViewModel();

            if (String.IsNullOrEmpty(id))
            {
                Floor_tbl FloorRecord = new Floor_tbl();
                if (BuildingId != null)
                {
                    FloorRecord.Building_Id = int.Parse(BuildingId);
                }
                model.SelectedFloor = FloorRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddFloorModal", model.SelectedFloor);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Floors = unitWork.FloorsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedFloor = unitWork.FloorsManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedFloor == null) { return View("_error"); }
                // ...
                return PartialView("EditAddFloorModal", model.SelectedFloor);
            }

        }

        [Route("SaveFloor")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveFloor(Floor_tbl Floor)
        {
            Floor_tbl existing = Floor;
            try
            {
                if (ModelState.IsValid)
                {

                    if (Floor.Floor_Id == 0)
                    {
                        // insert new record

                        Floor_tbl Floorobj = unitWork.FloorsManager.Add(existing);
                        if (Floorobj != null)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.FloorsManager.GetById(Floor.Floor_Id);
                        existing.Building_Id = Floor.Building_Id;
                        existing.Floor_Id = Floor.Floor_Id;
                        existing.Floor_Name = Floor.Floor_Name;
                        existing.Floor_NameEn = Floor.Floor_NameEn;
                        existing.Floor_Code = Floor.Floor_Code;
                        bool ret = unitWork.FloorsManager.Update(existing);
                        // update record

                        if (ret)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                    }

                }
                //Unit_tbl existing = unitWork.BuildingsManager.GetById(obj.Unit_Id);
                //existing.Unit_Id = obj.Unit_Id;
                //existing.Unit_Name = obj.Unit_Name;
                //existing.Unit_NameEn = obj.Unit_NameEn;

                //unitWork.BuildingsManager.Update(existing);

                FloorsViewModel model = new FloorsViewModel();
                model.Floors = unitWork.FloorsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedFloor = existing;
                model.DisplayMode = "ReadOnly";
                //return View("FloorsList", model,   );
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("FloorsList", new { BuildingId = existing.Building_Id });
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }
        // End of Floors Module
       

        // Start of Rooms module
        [UserPermissionAttribute(AllowFeature = "Rooms", AllowPermission = "Accessing")]
       
        public ActionResult RoomsList(int? StoreFlag, string BuildingId, string FloorId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, int? ShowEmptyFlag)
        {
            RoomsViewModel model = new RoomsViewModel();
            List<Room_tbl> RoomList;
            ViewBag.StoreFlag = StoreFlag;
            model.CurrentStoreFlag = StoreFlag;
            model.ShowEmptyFlag = ShowEmptyFlag;

            var allIRoomStatusList = unitWork.tbLookupsManager.GetByValue(2).ToList();
            //if (ShowEmptyFlag == null)
            //{
            //    model.ShowEmptyFlag = allIRoomStatusList.FirstOrDefault().LookupID;
            //}
            //else
            //{
            //    model.ShowEmptyFlag = ShowEmptyFlag.GetValueOrDefault();
            //    //model.ShowEmptyFlag = ShowEmptyFlag.GetValueOrDefault();
            //}
            model.RoomStatus = new SelectList(allIRoomStatusList, "LookupID", "LookupStringAr", model.ShowEmptyFlag);


            if (StoreFlag == 1)
            {
                int userId = SesssionUser.GetCurrentUserId();
                RoomList = unitWork.RoomsManager.GetUserInventories(userId).OrderBy(m => m.Floor_Id).ToList();
                RoomList = RoomList.Where(r => r.StoreFlag == true).ToList(); //.FirstOrDefault();
                Room_tbl Room = RoomList.FirstOrDefault();
                FloorId = Room.Floor_Id.ToString();
                int Id = int.Parse(FloorId);
                Floor_tbl Floor = unitWork.FloorsManager.GetById(Id);

                //ViewBag.FloorName = Floor.Floor_Name;
                ViewBag.FloorId = FloorId.ToString();

                BuildingId = Floor.Building_Id.ToString();
            }
            else
            {
                if (String.IsNullOrEmpty(FloorId))
                {
                    return View();
                }
                ViewBag.FloorId = FloorId.ToString();
                int Id = int.Parse(FloorId);

                Floor_tbl ObjFloor = unitWork.FloorsManager.GetById(Id);
                if (ObjFloor != null)
                {
                    ViewBag.FloorName = ObjFloor.Floor_Name;

                }
                if (model.ShowEmptyFlag== 5)
                {
                    RoomList = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).ToList();
                    RoomList = RoomList.Where(r => r.Floor_Id == Id && r.busyRoom ==false).ToList(); //.FirstOrDefault();
                    //model.ShowEmptyFlag = 2;
                    ViewBag.Acative = true;
                }
                else if (model.ShowEmptyFlag == 6)
                {
                    RoomList = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).ToList();
                    RoomList = RoomList.Where(r => r.Floor_Id == Id && r.busyRoom == true).ToList(); //.FirstOrDefault();
                    //model.ShowEmptyFlag = 3;
                    ViewBag.Acative = true;
                }
               else if (!String.IsNullOrEmpty(Search_Data))
                {

                    RoomList = unitWork.RoomsManager.GetCastByUnitName(Search_Data);

                }
                else
                {
                    RoomList = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).ToList();
                    RoomList = RoomList.Where(r => r.Floor_Id == Id).ToList(); //.FirstOrDefault();
                    model.ShowEmptyFlag = 1 ;
                    ViewBag.Acative = false;
                }

            }

            ViewBag.BuildingId = BuildingId.ToString();
            model.CurBulidingId = BuildingId;
            model.CurFloorId  = FloorId;

            int CurBuildingId = int.Parse(BuildingId);
            Building_tbl ObjBuilding = unitWork.BuildingsManager.GetById(CurBuildingId);
            if (ObjBuilding != null && StoreFlag != 1)
            {
                ViewBag.BuildingName = ObjBuilding.Building_Name;
            }

            model.Rooms = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Room_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Room_Id" ? "Room_Name" : "Room_NameEn";

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
            
            switch (Sorting_Order)
            {
                case "Building_Id":
                    RoomList = RoomList.OrderByDescending(stu => stu.Floor_Id).ToList();
                    break;
                case "Floor_Name":
                    RoomList = RoomList.OrderByDescending(stu => stu.Room_Name).ToList();
                    break;
                case "Floor_NameEn":
                    RoomList = RoomList.OrderByDescending(stu => stu.Room_NameEn).ToList();
                    break;
                default:
                    RoomList = RoomList.OrderBy(stu => stu.Floor_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Rooms = RoomList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (RoomList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }
        [HttpPost]
        public ActionResult RoomsList(RoomsViewModel model)
        {
            int? StoreFlag;
            string CurBuildingId;
            string CurFloorId;
            string Sorting_Order;
            string Search_Data;
            string Filter_Value;
            int? Page_No;
            int? ShowEmptyFlag;
            StoreFlag = model.CurrentStoreFlag;
            CurBuildingId = model.CurBulidingId;
            CurFloorId = model.CurFloorId;
            Sorting_Order = model.Sorting_Order;
            Search_Data = model.Search_Data;
            Filter_Value = model.Filter_Value;
            Page_No = model.PageNumber ;
            ShowEmptyFlag = model.ShowEmptyFlag;
            return RedirectToAction("RoomsList", new { StoreFlag= StoreFlag, BuildingId= CurBuildingId, FloorId= CurFloorId, Sorting_Order= Sorting_Order, Search_Data= Search_Data, Filter_Value= Filter_Value, Page_No= Page_No, ShowEmptyFlag= ShowEmptyFlag });

        }
        [HttpPost]
        [ActionName("DeleteRoom")]
        public ActionResult DeleteRoom(string id, string StoreFlag)
        {
            int Id = int.Parse(id);
            Room_tbl existing = unitWork.RoomsManager.GetById(Id);
            if (existing != null)
            {
                unitWork.RoomsManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                Floor_tbl Floorobj = unitWork.FloorsManager.GetById(existing.Floor_Id);
                int Building_Id = 0;
                if (Floorobj != null)
                {
                    Building_Id = Floorobj.Building_Id;
                }

                RoomsViewModel model = new RoomsViewModel();
                model.Rooms = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Floor_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.Rooms = null;
                model.DisplayMode = "";
                string CurStoreFlag = StoreFlag;

                return RedirectToAction("RoomsList", new { StoreFlag = CurStoreFlag, BuildingId = Building_Id, FloorId = existing.Floor_Id });
            }
            else
            {
                return RedirectToAction("RoomsList");
            }
        }




        [HttpGet]
        [ActionName("AddEditRoom")]
        public ActionResult AddEditRoom(string id = null, string FloorId = null, string StoreFlag = null)
        {

            RoomsViewModel model = new RoomsViewModel();
            if (StoreFlag != null)
            {
                model.CurrentStoreFlag = int.Parse(StoreFlag);
            }
            model.Depatments = unitWork.DepartmentManager.GetAll().Select(option => new SelectListItem
            {
                Text = option.Name,
                Value = option.Id.ToString()
            });

            var allIRoomTypesList = unitWork.tbLookupsManager.GetByValue(3).ToList();
            model.RoomTypes = new SelectList(allIRoomTypesList, "LookupID", "LookupStringAr", model.ShowEmptyFlag);




            if (String.IsNullOrEmpty(id))
            {
                Room_tbl RoomRecord = new Room_tbl();

                if (FloorId != null)
                {
                    RoomRecord.Floor_Id = int.Parse(FloorId);
                }
                model.SelectedRoom = RoomRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddRoomModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Rooms = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Room_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedRoom = unitWork.RoomsManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedRoom == null) { return View("_error"); }
                // ...
                return PartialView("EditAddRoomModal", model);
            }

        }

        [Route("SaveRoom")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveRoom(RoomsViewModel Room)
        {
            Room_tbl existing = Room.SelectedRoom;
            string CurStoreFlag = Room.CurrentStoreFlag.ToString();
            int Building_Id = 0;
            try
            {
               
                if (ModelState.IsValid)
                {
                    if (Room.SelectedRoom.Room_Id == 0)
                    {
                        // insert new record

                        Room_tbl Roomobj = unitWork.RoomsManager.Add(existing);
                        if (Roomobj != null)
                        {
                            TempData["Message"] = "Success";
                            //unitWork.RoomsManager.SaveLog("Rooms", int.Parse(Roomobj.Room_Id.ToString()), "Insert", SesssionUser.GetCurrentUserId());
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                    else
                    {
                        existing = unitWork.RoomsManager.GetById(Room.SelectedRoom.Room_Id);
                        existing.Floor_Id = Room.SelectedRoom.Floor_Id;
                        existing.Room_Id = Room.SelectedRoom.Room_Id;
                        existing.Room_Name = Room.SelectedRoom.Room_Name;
                        existing.Room_NameEn = Room.SelectedRoom.Room_NameEn;
                        existing.Room_Code = Room.SelectedRoom.Room_Code;
                        existing.Room_BarCode = Room.SelectedRoom.Room_BarCode;
                        existing.Room_RFID = Room.SelectedRoom.Room_RFID;
                        existing.DeptQuota = Room.SelectedRoom.DeptQuota;
                        existing.StoreFlag = Room.SelectedRoom.StoreFlag;
                        existing.IPAddress = Room.SelectedRoom.IPAddress;
                        existing.RoomTypeId = Room.SelectedRoom.RoomTypeId ;

                        bool ret = unitWork.RoomsManager.Update(existing);
                        // update record

                        if (ret)
                        {
                            TempData["Message"] = "Success";
                            //unitWork.RoomsManager.SaveLog("Rooms", int.Parse(existing.Room_Id.ToString()), "Update", SesssionUser.GetCurrentUserId());
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                    }

                }



                //if (existing.StoreFlag) { CurStoreFlag = "1"; } else   { CurStoreFlag = "0"; }

                Floor_tbl Floorobj = unitWork.FloorsManager.GetById(existing.Floor_Id);
               
                if (Floorobj != null)
                {
                    Building_Id = Floorobj.Building_Id;
                }
                RoomsViewModel model = new RoomsViewModel();
                model.Rooms = unitWork.RoomsManager.GetNotDelAll().OrderBy(m => m.Room_Id).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedRoom = existing;
                model.DisplayMode = "ReadOnly";
            }
            catch (Exception ex)
            {
                TempData["Message"] = Resources.DefaultResource.ErrorMessage;

            }
            return RedirectToAction("RoomsList", new { StoreFlag = CurStoreFlag, BuildingId = Building_Id, FloorId = existing.Floor_Id });

        }
        // End of Rooms Module

        public ActionResult ShowBulidingReport()
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "Report1.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View();
            }
            
           List<Building_tbl> cm = unitWork.BuildingsManager.GetAll().ToList ();
          
            ReportDataSource rd = new ReportDataSource("DataSet2", cm);
           lr.DataSources.Add(rd);



            var reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportPath = path;
           // Server.MapPath("~/Reports/Reception/PatientMoneyReceipt.rdlc");

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(rd);
            reportViewer.LocalReport.Refresh();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.AsyncRendering = false;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.ReportViewer = reportViewer;
            return View();
            
        }
        [HttpGet]
        [ActionName("ShowRoomEmployees")]
        public ActionResult ShowRoomEmployees(string id = null)
        {

            EmpRoomsViewModel model = new EmpRoomsViewModel();
            if (String.IsNullOrEmpty(id))
            {

                //OutOrdersDetails OutOrdersDetailsRecord = new OutOrdersDetails();
                //model.SelectedUnit = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("RoomEmpModal", model.EmpRooms);
            }
            else
            {
                // Edit record (view in Edit mode)
                long OrderId = long.Parse(id);
                int Size_Of_Page = 30;
                model.EmpRooms = unitWork.EmpRoomsManager.GetNotDelAll (id).OrderBy(m => m.EmpId).ToPagedList(No_Of_Page, Size_Of_Page);

                //model.OutOrders = OutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);

                //model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                model.DisplayMode = "ReadOnly";
                //if (model.SelectedUnit == null) { return View("_error"); }
                // ...
                return PartialView("RoomEmpModal", model);
            }

        }
    }
}
