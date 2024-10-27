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
    public class UsersController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();

        // Start of Main Items module
        [UserPermissionAttribute(AllowFeature = "Users", AllowPermission = "Accessing")]
        public ActionResult UsersList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            UsersViewModel model = new UsersViewModel();
            List<tbUsers> UsersList = unitWork.UsersManager.GetNotDelAll().OrderBy(m => m.UserID).ToList();
            model.SelectedUser = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "UserID" : "";
            ViewBag.SortingModel = Sorting_Order == "UserID" ? "UsersName" : "FullName";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            if (!String.IsNullOrEmpty(Search_Data))
            {

                UsersList = unitWork.UsersManager.GetCastByName(Search_Data);

            }
            switch (Sorting_Order)
            {
                case "UserID":
                    UsersList = UsersList.OrderBy(stu => stu.UserID).ToList();
                    break;
                case "FullName":
                    UsersList = UsersList.OrderBy(stu => stu.FullName).ToList();
                    break;
                case "UsersName":
                    UsersList = UsersList.OrderBy(stu => stu.UserName).ToList();
                    break;
                default:
                    UsersList = UsersList.OrderBy(stu => stu.UserID).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.PageNumber = No_Of_Page;
            model.Sorting_Order = Sorting_Order;
            model.Users = UsersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UsersList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }

        [HttpPost]
        [ActionName("DeleteUser")]
        public ActionResult DeleteUser(string id)
        {
            int Id = int.Parse(id);
            tbUsers existing = unitWork.UsersManager.GetById(Id);
            if (existing != null)
            {
                unitWork.UsersManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                UsersViewModel model = new UsersViewModel();
                //model.Users = unitWork.UsersManager.GetNotDelAll().OrderBy(m => m.UserID ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedUser = null;
                model.DisplayMode = "";

                return RedirectToAction("UsersList");
            }
            else
            {
                return RedirectToAction("UsersList");
            }
        }


        [HttpGet]
        [ActionName("AddEditUser")]
        public ActionResult AddEditUser(string id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetNotDelAll();
            //var CurStatusList = unitWork.StatusManager.GetNotDelAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            UsersViewModel model = new UsersViewModel();
            model.Employees = unitWork.EmployeesManager.GetNotDelAll().Select(option => new SelectListItem
            {
                Text = option.FULL_NAME_AR,
                Value = option.Id.ToString()
            });

            model.Roles = unitWork.UsersRolesManager.GetNotDelAll().Select(option => new SelectListItem
            {
                Text = option.RoleNameAr,
                Value = option.RoleID.ToString()
            });
            if (String.IsNullOrEmpty(id))
            {
                if (this.HasPermission("Users", "Adding"))
                {
                    tbUsers ObjRecord = new tbUsers();
                    model.SelectedUser = ObjRecord;
                    model.DisplayMode = "ReadOnly";
                    return PartialView("EditAddUserModal", model);
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }
            }
            else
            {
                if (this.HasPermission("Users", "Updating"))
                {
                    // Edit record (view in Edit mode)
                    int EmpId = int.Parse(id);

                    //model.Users = unitWork.UsersManager.GetNotDelAll().OrderBy(m => m.UserID).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);


                    model.SelectedUser = unitWork.UsersManager.GetByUserId(EmpId);
                    model.DisplayMode = "ReadWrite";
                    if (model.SelectedUser == null) { return View("_error"); }
                    // ...
                    return PartialView("EditAddUserModal", model);
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }
            }

        }

        [Route("SaveUser")]
        //[ValidateAntiForgeryToken]
        [HttpPost]

        public ActionResult SaveUser(UsersViewModel ModelItem)
        {
            TempData["warningMessage"] = null;
            tbUsers existing = ModelItem.SelectedUser;
            //if (ModelState.IsValid)
            //{
            List<vwEmployees> Employee = unitWork.EmployeesManager.GetEmployeeByEmpId(ModelItem.SelectedUser.EmpId.GetValueOrDefault());
            tbUsers CurUser = unitWork.UsersManager.GetUserByEmpId(ModelItem.SelectedUser.EmpId.GetValueOrDefault());

            if (Employee.Count > 0)
            {

                existing.EmpId = Employee.FirstOrDefault().Id;
                existing.Password = ModelItem.SelectedUser.Password;
                existing.Email = Employee.FirstOrDefault().Email;
                string UserName = "";
                if (Employee.FirstOrDefault().Email != null)
                {
                    int index = Employee.FirstOrDefault().Email.IndexOf("@");
                    UserName = Employee.FirstOrDefault().Email.Substring(0, index);
                }
                existing.FullName = Employee.FirstOrDefault().FULL_NAME_AR;
                existing.UserName = UserName;
                existing.FingerPrintID = Employee.FirstOrDefault().Fingerprint_Id;
                existing.IsDeleted = false;
                existing.IsActive = ModelItem.SelectedUser.IsActive;
                existing.RoleID = ModelItem.SelectedUser.RoleID;

                if (ModelItem.SelectedUser.UserID == 0)
                {
                    if (CurUser != null)
                    {
                        TempData["warningMessage"] = Resources.DefaultResource.DuplicatedUserMsg;
                        TempData["Message"] = null;

                    }
                    else
                    {
                        // insert new record
                        tbUsers Userobj = unitWork.UsersManager.Add(existing);
                        if (Userobj != null)
                        {
                            TempData["Message"] = "Success";
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        ModelState.Clear();
                    }
                }
                else
                {

                    tbUsers CurUserobj = unitWork.UsersManager.GetById(ModelItem.SelectedUser.UserID);


                    // update record
                    CurUserobj.EmpId = existing.EmpId;
                    CurUserobj.Password = existing.Password;
                    CurUserobj.Email = existing.Email;
                    CurUserobj.FullName = existing.FullName;
                    CurUserobj.UserName = existing.UserName;
                    CurUserobj.FingerPrintID = existing.FingerPrintID;
                    CurUserobj.IsDeleted = existing.IsDeleted;
                    CurUserobj.IsActive = existing.IsActive;
                    CurUserobj.RoleID = existing.RoleID;

                    bool ret = unitWork.UsersManager.Update(CurUserobj);

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
            else
            {
                TempData["warningMessage"] = Resources.DefaultResource.MustSelectEmployee;
                TempData["Message"] = null;
            }
            return RedirectToAction("UsersList");

        }
        // End of Main Category Module------------------------------------------------------------------------------

        private void GetUserPage(int UserId)
        {
            List<tbSystemPages> SystemPagesList = new List<tbSystemPages>();
            SystemPagesList = unitWork.SystemPagesManager.GetNotDelAll();

            foreach (tbSystemPages obj in SystemPagesList)
            {
                tbUsersPages UserPages = unitWork.UsersPagesManager.GetById(UserId, obj.PageID);
                if (UserPages == null)
                {
                    tbUsersPages UserPageEnity = new tbUsersPages();
                    UserPageEnity.PageID = obj.PageID;
                    UserPageEnity.UserID = UserId;
                    UserPageEnity.Accessing = false;
                    UserPageEnity.Adding = false;
                    UserPageEnity.Updating = false;
                    UserPageEnity.Deleting = false;
                    unitWork.UsersPagesManager.Add(UserPageEnity);
                }
            }

        }
        [UserPermissionAttribute(AllowFeature = "Permissions", AllowPermission = "Accessing")]
        public ActionResult UsersPagesAssignment(string UserId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            UsersPagesViewModel model = new UsersPagesViewModel();
            int CurUserId = 0;

            if (!String.IsNullOrEmpty(UserId))
            {
                CurUserId = int.Parse(UserId);
                model.CurUserId = CurUserId;
                ViewBag.UserId = CurUserId;
                tbUsers objUser = unitWork.UsersManager.GetById(CurUserId);
                ViewBag.UserName = objUser.FullName;
            }
            GetUserPage(CurUserId); // get all user Depts
            List<tbUsersPages> UsersPagesList = unitWork.UsersPagesManager.GetByUserId(CurUserId).OrderBy(m => m.PageID).ToList();
            model.SelectedUsersPages = null;
            //model.UsersPageslist = UsersPagesList;


            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "PageID" : "";
            ViewBag.SortingModel = Sorting_Order == "UserID" ? "UserID" : "PageID";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            if (!String.IsNullOrEmpty(Search_Data))
            {

                UsersPagesList = unitWork.UsersPagesManager.GetCastByName(CurUserId, Search_Data);

            }
            switch (Sorting_Order)
            {
                case "UserID":
                    UsersPagesList = UsersPagesList.OrderBy(stu => stu.UserID).ToList();
                    break;
                case "PageID":
                    UsersPagesList = UsersPagesList.OrderBy(stu => stu.PageID).ToList();
                    break;
                case "UpdateOn":
                    UsersPagesList = UsersPagesList.OrderBy(stu => stu.UpdateOn).ToList();
                    break;
                default:
                    UsersPagesList = UsersPagesList.OrderBy(stu => stu.UserID).ToList();
                    //carList =
                    break;
            }
            List<CheckBoxListPermissionItem> ItemsCheckBoxList = new List<CheckBoxListPermissionItem>();
            foreach (var item in UsersPagesList)
            {
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                CheckBoxListPermissionItem CheckItem = new CheckBoxListPermissionItem() { IsAccessSelected = item.Accessing.GetValueOrDefault(),
                    IsAddSelected = item.Adding.GetValueOrDefault(),
                    IsUpdateSelected = item.Updating.GetValueOrDefault(),
                    IsDeleteSelected = item.Deleting.GetValueOrDefault(),
                    PageId = item.PageID, Name = "Item" + item.PageID.ToString() };
                ItemsCheckBoxList.Add(CheckItem);
            }
            model.ItemsPagesCheckList = ItemsCheckBoxList;

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.UsersPages = UsersPagesList;

            //.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UsersPagesList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }
        [Route("UsersPagesAssignment")]

        [HttpPost]
        public ActionResult UsersPagesAssignment(UsersPagesViewModel ModelItem)
        {
            //List<tbSystemPages> SystemPagesList = new List<tbSystemPages>();

            //List<tbUsersPages> UsersPagesList =  unitWork.UsersPagesManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.PageID).ToList();

            ModelItem.UsersPages = ModelItem.UsersPages;//.ToPagedList(No_Of_Page, Size_Of_Page);
            ModelItem.ItemsPagesCheckList = ModelItem.ItemsPagesCheckList;
            //SystemPagesList = unitWork.SystemPagesManager.GetNotDelAll(); 

            if (ModelItem.UsersPages.Count > 0)
            {

                Boolean ret = false;
                if (ModelState.IsValid)
                {
                    int i = 0;
                    foreach (tbUsersPages obj in ModelItem.UsersPages)
                    {
                        tbUsersPages UserPage = unitWork.UsersPagesManager.GetById(obj.UserID, obj.PageID);
                        if (UserPage != null)
                        {
                            UserPage.Accessing = ModelItem.ItemsPagesCheckList[i].IsAccessSelected;
                            UserPage.Adding = ModelItem.ItemsPagesCheckList[i].IsAddSelected;
                            UserPage.Updating = ModelItem.ItemsPagesCheckList[i].IsUpdateSelected;
                            UserPage.Deleting = ModelItem.ItemsPagesCheckList[i].IsDeleteSelected;
                            unitWork.UsersPagesManager.Update(UserPage);
                        }
                        else
                        {
                            tbUsersPages UserPageEnity = new tbUsersPages();
                            UserPageEnity.PageID = obj.PageID;
                            UserPageEnity.UserID = ModelItem.CurUserId.GetValueOrDefault();
                            UserPageEnity.Accessing = false;
                            UserPageEnity.Adding = false;
                            UserPageEnity.Updating = false;
                            UserPageEnity.Deleting = false;
                            unitWork.UsersPagesManager.Add(UserPageEnity);
                        }
                        i++;
                    }
                    ret = true;

                }

                if (ret)
                {
                    TempData["Message"] = "Success";
                }
                else
                {
                    TempData["Message"] = null;
                }
            }


            //unitWork.BuildingsManager.Update(existing);

            UsersPagesViewModel model = new UsersPagesViewModel();
            model = ModelItem;
            model.UsersPages = unitWork.UsersPagesManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.PageID).ToList();
            //.Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.CurUserId = ModelItem.CurUserId;


            //model.SelectedUsersPages  = existing;
            model.DisplayMode = "ReadOnly";
            return RedirectToAction("UsersPagesAssignment", new { UserId = model.CurUserId });
            //return View(model,);
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }

        private void GetUserStore(int UserId)
        {
            List<Room_tbl> StoreslList = new List<Room_tbl>();
            StoreslList = unitWork.RoomsManager.GetInventoriesAll();

            foreach (Room_tbl obj in StoreslList)
            {
                tbUsersStores UserStores = unitWork.UsersStoresManager.GetByUserIdAndStoreId(UserId, obj.Room_Id);
                if (UserStores == null)
                {
                    tbUsersStores UserPageEnity = new tbUsersStores();
                    UserPageEnity.StoreID = obj.Room_Id;
                    UserPageEnity.UserID = UserId;
                    UserPageEnity.Accessing = false;
                    UserPageEnity.Adding = false;
                    UserPageEnity.Updating = false;
                    UserPageEnity.Deleting = false;
                    unitWork.UsersStoresManager.Add(UserPageEnity);
                }
            }

        }
        public ActionResult UsersStoresAssignment(string UserId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            UsersStoresViewModel model = new UsersStoresViewModel();
            int CurUserId = 0;

            if (!String.IsNullOrEmpty(UserId))
            {
                CurUserId = int.Parse(UserId);
                model.CurUserId = CurUserId;
                ViewBag.UserId = CurUserId;
                tbUsers objUser = unitWork.UsersManager.GetById(CurUserId);
                ViewBag.UserName = objUser.FullName;
            }
            GetUserStore(CurUserId);
            List<tbUsersStores> UsersStoresList = unitWork.UsersStoresManager.GetByUserId(CurUserId).OrderBy(m => m.StoreID).ToList();
            model.SelectedUsersStores = null;
            //model.UsersPageslist = UsersPagesList;


            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "PageID" : "";
            ViewBag.SortingModel = Sorting_Order == "UserID" ? "UserID" : "PageID";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            if (!String.IsNullOrEmpty(Search_Data))
            {

                UsersStoresList = unitWork.UsersStoresManager.GetCastByName(CurUserId, Search_Data);

            }
            switch (Sorting_Order)
            {
                case "UserID":
                    UsersStoresList = UsersStoresList.OrderBy(stu => stu.UserID).ToList();
                    break;
                case "PageID":
                    UsersStoresList = UsersStoresList.OrderBy(stu => stu.StoreID).ToList();
                    break;
                case "UpdateOn":
                    UsersStoresList = UsersStoresList.OrderBy(stu => stu.UpdateOn).ToList();
                    break;
                default:
                    UsersStoresList = UsersStoresList.OrderBy(stu => stu.UserID).ToList();
                    //carList =
                    break;
            }
            List<CheckBoxListStoresPermissionItem> ItemsCheckBoxList = new List<CheckBoxListStoresPermissionItem>();
            foreach (var item in UsersStoresList)
            {
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                CheckBoxListStoresPermissionItem CheckItem = new CheckBoxListStoresPermissionItem()
                {
                    IsAccessSelected = item.Accessing.GetValueOrDefault(),
                    IsAddSelected = item.Adding.GetValueOrDefault(),
                    IsUpdateSelected = item.Updating.GetValueOrDefault(),
                    IsDeleteSelected = item.Deleting.GetValueOrDefault(),
                    StoreId = item.StoreID,
                    Name = "Item" + item.StoreID.ToString()
                };
                ItemsCheckBoxList.Add(CheckItem);
            }
            model.ItemsStoresCheckList = ItemsCheckBoxList;

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.UsersStores = UsersStoresList;

            //.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UsersStoresList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }
        [Route("UsersStoresAssignment")]

        [HttpPost]
        public ActionResult UsersStoresAssignment(UsersStoresViewModel ModelItem)
        {
            //List<tbSystemPages> SystemPagesList = new List<tbSystemPages>();
            //List<tbUsersPages> UsersPagesList =  unitWork.UsersPagesManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.PageID).ToList();

            ModelItem.UsersStores = ModelItem.UsersStores;//.ToPagedList(No_Of_Page, Size_Of_Page);
            ModelItem.ItemsStoresCheckList = ModelItem.ItemsStoresCheckList;
            //SystemPagesList = unitWork.SystemPagesManager.GetNotDelAll(); 

            if (ModelItem.UsersStores.Count > 0)
            {

                Boolean ret = false;
                if (ModelState.IsValid)
                {
                    int i = 0;
                    foreach (tbUsersStores obj in ModelItem.UsersStores)
                    {
                        tbUsersStores UserStore = unitWork.UsersStoresManager.GetByUserIdAndStoreId(obj.UserID, obj.StoreID);
                        if (UserStore != null)
                        {
                            UserStore.Accessing = ModelItem.ItemsStoresCheckList[i].IsAccessSelected;
                            UserStore.Adding = ModelItem.ItemsStoresCheckList[i].IsAddSelected;
                            UserStore.Updating = ModelItem.ItemsStoresCheckList[i].IsUpdateSelected;
                            UserStore.Deleting = ModelItem.ItemsStoresCheckList[i].IsDeleteSelected;
                            unitWork.UsersStoresManager.Update(UserStore);
                        }
                        else
                        {
                            tbUsersStores UserPageEnity = new tbUsersStores();
                            UserPageEnity.StoreID = obj.StoreID;
                            UserPageEnity.UserID = ModelItem.CurUserId.GetValueOrDefault();
                            UserPageEnity.Accessing = false;
                            UserPageEnity.Adding = false;
                            UserPageEnity.Updating = false;
                            UserPageEnity.Deleting = false;
                            unitWork.UsersStoresManager.Add(UserPageEnity);
                        }
                        i++;
                    }
                    ret = true;

                }

                if (ret)
                {
                    TempData["Message"] = "Success";
                }
                else
                {
                    TempData["Message"] = null;
                }
            }


            //unitWork.BuildingsManager.Update(existing);

            UsersStoresViewModel model = new UsersStoresViewModel();
            model = ModelItem;
            model.UsersStores = unitWork.UsersStoresManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.StoreID).ToList();
            //.Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.CurUserId = ModelItem.CurUserId;


            //model.SelectedUsersPages  = existing;
            model.DisplayMode = "ReadOnly";
            //return View(model);
            return RedirectToAction("UsersStoresAssignment", new { UserId = model.CurUserId });
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }


        public ActionResult UsersMainCatsAssignment(string UserId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            UsersMainCatsViewModel model = new UsersMainCatsViewModel();
            int CurUserId = 0;

            if (!String.IsNullOrEmpty(UserId))
            {
                CurUserId = int.Parse(UserId);
                model.CurUserId = CurUserId;
                ViewBag.UserId = CurUserId;
                tbUsers objUser = unitWork.UsersManager.GetById(CurUserId);
                ViewBag.UserName = objUser.FullName;
            }
            GetUserMainCats(CurUserId);
            List<tbUsersMainCats> UsersMainCatsList = unitWork.UsersMainCatsManager.GetByUserId(CurUserId).OrderBy(m => m.CatMain_Id).ToList();
            model.SelectedUsersMainCats = null;
            //model.UsersPageslist = UsersPagesList;


            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "PageID" : "";
            ViewBag.SortingModel = Sorting_Order == "UserID" ? "UserID" : "PageID";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            if (!String.IsNullOrEmpty(Search_Data))
            {

                UsersMainCatsList = unitWork.UsersMainCatsManager.GetCastByName(CurUserId, Search_Data);

            }
            switch (Sorting_Order)
            {
                case "UserID":
                    UsersMainCatsList = UsersMainCatsList.OrderBy(stu => stu.UserID).ToList();
                    break;
                case "PageID":
                    UsersMainCatsList = UsersMainCatsList.OrderBy(stu => stu.CatMain_Id).ToList();
                    break;
                case "UpdateOn":
                    UsersMainCatsList = UsersMainCatsList.OrderBy(stu => stu.UpdateOn).ToList();
                    break;
                default:
                    UsersMainCatsList = UsersMainCatsList.OrderBy(stu => stu.UserID).ToList();
                    //carList =
                    break;
            }
            List<CheckBoxListMainCatsPermissionItem> ItemsCheckBoxList = new List<CheckBoxListMainCatsPermissionItem>();
            foreach (var item in UsersMainCatsList)
            {
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                CheckBoxListMainCatsPermissionItem CheckItem = new CheckBoxListMainCatsPermissionItem()
                {
                    IsAccessSelected = item.Accessing.GetValueOrDefault(),
                    IsAddSelected = item.Adding.GetValueOrDefault(),
                    IsUpdateSelected = item.Updating.GetValueOrDefault(),
                    IsDeleteSelected = item.Deleting.GetValueOrDefault(),
                    CatMain_Id = item.CatMain_Id.GetValueOrDefault(),
                    Name = "Item" + item.CatMain_Id.ToString()
                };
                ItemsCheckBoxList.Add(CheckItem);
            }
            model.ItemsMainCatsCheckList = ItemsCheckBoxList;

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.UsersMainCats = UsersMainCatsList;

            //.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UsersMainCatsList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }
        [Route("UsersStoresAssignment")]

        [HttpPost]
        public ActionResult UsersMainCatsAssignment(UsersMainCatsViewModel ModelItem)
        {
            //List<tbSystemPages> SystemPagesList = new List<tbSystemPages>();
            //List<tbUsersPages> UsersPagesList =  unitWork.UsersPagesManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.PageID).ToList();

            ModelItem.UsersMainCats = ModelItem.UsersMainCats;//.ToPagedList(No_Of_Page, Size_Of_Page);
            ModelItem.ItemsMainCatsCheckList = ModelItem.ItemsMainCatsCheckList;
            //SystemPagesList = unitWork.SystemPagesManager.GetNotDelAll(); 

            if (ModelItem.UsersMainCats.Count > 0)
            {

                Boolean ret = false;
                if (ModelState.IsValid)
                {
                    int i = 0;
                    foreach (tbUsersMainCats obj in ModelItem.UsersMainCats)
                    {
                        tbUsersMainCats UsersMainCats = unitWork.UsersMainCatsManager.GetByUserIdAndCatMainId(obj.UserID, obj.CatMain_Id);
                        if (UsersMainCats != null)
                        {
                            UsersMainCats.Accessing = ModelItem.ItemsMainCatsCheckList[i].IsAccessSelected;
                            UsersMainCats.Adding = ModelItem.ItemsMainCatsCheckList[i].IsAddSelected;
                            UsersMainCats.Updating = ModelItem.ItemsMainCatsCheckList[i].IsUpdateSelected;
                            UsersMainCats.Deleting = ModelItem.ItemsMainCatsCheckList[i].IsDeleteSelected;
                            unitWork.UsersMainCatsManager.Update(UsersMainCats);
                        }
                        else
                        {
                            tbUsersMainCats UserPageEnity = new tbUsersMainCats();
                            UserPageEnity.CatMain_Id = obj.CatMain_Id;
                            UserPageEnity.UserID = ModelItem.CurUserId.GetValueOrDefault();
                            UserPageEnity.Accessing = false;
                            UserPageEnity.Adding = false;
                            UserPageEnity.Updating = false;
                            UserPageEnity.Deleting = false;
                            unitWork.UsersMainCatsManager.Add(UserPageEnity);
                        }
                        i++;
                    }
                    ret = true;

                }

                if (ret)
                {
                    TempData["Message"] = "Success";
                }
                else
                {
                    TempData["Message"] = null;
                }
            }


            //unitWork.BuildingsManager.Update(existing);

            UsersMainCatsViewModel model = new UsersMainCatsViewModel();
            model = ModelItem;
            model.UsersMainCats = unitWork.UsersMainCatsManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.CatMain_Id).ToList();
            //.Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.CurUserId = ModelItem.CurUserId;


            //model.SelectedUsersPages  = existing;
            model.DisplayMode = "ReadOnly";
            //return View(model);
            return RedirectToAction("UsersMainCatsAssignment", new { UserId = model.CurUserId });
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }
        private void GetUserMainCats (int UserId)
        {
            List<CatMain_tbl > CatMain_tbllList = new List<CatMain_tbl>();
            CatMain_tbllList = unitWork.CatMainManager.GetNotDelAll();

            foreach (CatMain_tbl obj in CatMain_tbllList)
            {
                tbUsersMainCats UserStores = unitWork.UsersMainCatsManager.GetByUserIdAndCatMainId(UserId, obj.CatMain_Id);
                if (UserStores == null)
                {
                    tbUsersMainCats UserPageEnity = new tbUsersMainCats();
                    UserPageEnity.CatMain_Id = obj.CatMain_Id;
                    UserPageEnity.UserID = UserId;
                    UserPageEnity.Accessing = false;
                    UserPageEnity.Adding = false;
                    UserPageEnity.Updating = false;
                    UserPageEnity.Deleting = false;
                    unitWork.UsersMainCatsManager.Add(UserPageEnity);
                }
            }

        }

        // user depts part
        private void GetUserDept(int UserId)
        {
            List<vwDepartments> DeptslList = new List<vwDepartments>();
            DeptslList = unitWork.DepartmentManager.GetNotDelAll();

            foreach (vwDepartments obj in DeptslList)
            {
                tbUsersDepts UserDepts = unitWork.UsersDeptsManager.GetByUserIdAndDeptId(UserId, obj.Id );
                if (UserDepts == null)
                {
                    tbUsersDepts UserPageEnity = new tbUsersDepts();
                    UserPageEnity.DeptID = obj.Id;
                    UserPageEnity.UserID = UserId;
                    UserPageEnity.Accessing = false;
                    UserPageEnity.Adding = false;
                    UserPageEnity.Updating = false;
                    UserPageEnity.Deleting = false;
                    unitWork.UsersDeptsManager.Add(UserPageEnity);
                }
            }

        }
        public ActionResult UsersDeptsAssignment(string UserId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            UsersDeptsViewModel model = new UsersDeptsViewModel();
            int CurUserId = 0;

            if (!String.IsNullOrEmpty(UserId))
            {
                CurUserId = int.Parse(UserId);
                model.CurUserId = CurUserId;
                ViewBag.UserId = CurUserId;
                tbUsers objUser = unitWork.UsersManager.GetById(CurUserId);
                ViewBag.UserName = objUser.FullName;
            }
            GetUserDept(CurUserId);
            List<tbUsersDepts> UsersDeptsList = unitWork.UsersDeptsManager.GetByUserId(CurUserId).OrderBy(m => m.DeptID).ToList();
            model.SelectedUsersDepts = null;
            //model.UsersPageslist = UsersPagesList;


            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "DeptID" : "";
            ViewBag.SortingModel = Sorting_Order == "UserID" ? "UserID" : "DeptID";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            if (!String.IsNullOrEmpty(Search_Data))
            {

                UsersDeptsList = unitWork.UsersDeptsManager.GetCastByName(CurUserId, Search_Data);

            }
            switch (Sorting_Order)
            {
                case "UserID":
                    UsersDeptsList = UsersDeptsList.OrderBy(stu => stu.UserID).ToList();
                    break;
                case "DeptID":
                    UsersDeptsList = UsersDeptsList.OrderBy(stu => stu.DeptID).ToList();
                    break;
                case "UpdateOn":
                    UsersDeptsList = UsersDeptsList.OrderBy(stu => stu.UpdateOn).ToList();
                    break;
                default:
                    UsersDeptsList = UsersDeptsList.OrderBy(stu => stu.UserID).ToList();
                    //carList =
                    break;
            }
            List<CheckBoxListDeptsPermissionItem> ItemsCheckBoxList = new List<CheckBoxListDeptsPermissionItem>();
            foreach (var item in UsersDeptsList)
            {
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                CheckBoxListDeptsPermissionItem CheckItem = new CheckBoxListDeptsPermissionItem()
                {
                    IsAccessSelected = item.Accessing.GetValueOrDefault(),
                    IsAddSelected = item.Adding.GetValueOrDefault(),
                    IsUpdateSelected = item.Updating.GetValueOrDefault(),
                    IsDeleteSelected = item.Deleting.GetValueOrDefault(),
                    DeptID = item.DeptID,
                    Name = "Dept" + item.DeptID.ToString()
                };
                ItemsCheckBoxList.Add(CheckItem);
            }
            model.ItemsDeptsCheckList = ItemsCheckBoxList;

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.UsersDepts = UsersDeptsList;

            //.ToPagedList(No_Of_Page, Size_Of_Page);
            if (UsersDeptsList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }

        }
        [Route("UsersDeptsAssignment")]

        [HttpPost]
        public ActionResult UsersDeptsAssignment(UsersDeptsViewModel ModelItem)
        {
            //List<tbSystemPages> SystemPagesList = new List<tbSystemPages>();
            //List<tbUsersPages> UsersPagesList =  unitWork.UsersPagesManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.PageID).ToList();

            ModelItem.UsersDepts = ModelItem.UsersDepts;//.ToPagedList(No_Of_Page, Size_Of_Page);
            ModelItem.ItemsDeptsCheckList = ModelItem.ItemsDeptsCheckList;
            //SystemPagesList = unitWork.SystemPagesManager.GetNotDelAll(); 

            if (ModelItem.UsersDepts.Count > 0)
            {

                Boolean ret = false;
                if (ModelState.IsValid)
                {
                    int i = 0;
                    foreach (tbUsersDepts obj in ModelItem.UsersDepts )
                    {
                        tbUsersDepts UserDept = unitWork.UsersDeptsManager.GetByUserIdAndDeptId(obj.UserID, obj.DeptID);
                        if (UserDept != null)
                        {
                            UserDept.Accessing = ModelItem.ItemsDeptsCheckList[i].IsAccessSelected;
                            UserDept.Adding = ModelItem.ItemsDeptsCheckList[i].IsAddSelected;
                            UserDept.Updating = ModelItem.ItemsDeptsCheckList[i].IsUpdateSelected;
                            UserDept.Deleting = ModelItem.ItemsDeptsCheckList[i].IsDeleteSelected;
                            unitWork.UsersDeptsManager.Update(UserDept);
                        }
                        else
                        {
                            tbUsersDepts UserPageEnity = new tbUsersDepts();
                            UserPageEnity.DeptID  = obj.DeptID;
                            UserPageEnity.UserID = ModelItem.CurUserId.GetValueOrDefault();
                            UserPageEnity.Accessing = false;
                            UserPageEnity.Adding = false;
                            UserPageEnity.Updating = false;
                            UserPageEnity.Deleting = false;
                            unitWork.UsersDeptsManager.Add(UserPageEnity);
                        }
                        i++;
                    }
                    ret = true;

                }

                if (ret)
                {
                    TempData["Message"] = "Success";
                }
                else
                {
                    TempData["Message"] = null;
                }
            }


            //unitWork.BuildingsManager.Update(existing);

            UsersDeptsViewModel model = new UsersDeptsViewModel();
            model = ModelItem;
            model.UsersDepts = unitWork.UsersDeptsManager.GetByUserId(ModelItem.CurUserId.GetValueOrDefault()).OrderBy(m => m.DeptID).ToList();
            //.Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.CurUserId = ModelItem.CurUserId;


            //model.SelectedUsersPages  = existing;
            model.DisplayMode = "ReadOnly";
            //return View(model);
            return RedirectToAction("UsersDeptsAssignment", new { UserId = model.CurUserId });
            //Car Cars = unitWork.CarManager.GetNotDelAll().FirstOrDefault();
            //ViewBag.CarModelId = new SelectList(unitWork.modelmanager.GetNotDelAll(), "ModelId", "ModelDesc", Car.CarModeltype);
            //ViewBag.CarStatusId = new SelectList(unitWork.StatusManager.GetNotDelAll(), "CarStausId", "CarStatusDesc", Car.CarModeltype);

        }

    }
}