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
    public class InventoryController : Controller
    {
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        // GET: Inventory
        public ActionResult Index()
        {
            return View();
        }
        // Start of Items Stock  module
        [UserPermissionAttribute(AllowFeature = "Items Search", AllowPermission = "Accessing")]
        public ActionResult ItemsStockList(string EmpId, string RoomId, string DeptId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,string Flag, string ZeroFlag)
        {
            int ? CurEmpId=null;
            int ? CurDeptId=null;
            int ? CurRoomId=null;

            if (!String.IsNullOrEmpty(EmpId))
            {
                CurEmpId = int.Parse(EmpId);
                ViewBag.EmpId = EmpId;
                List<vwEmployees> ObjEmployees = unitWork.EmployeesManager.GetEmployeeByEmpId(CurEmpId.GetValueOrDefault());
                if (ObjEmployees != null)
                {
                    if (ObjEmployees.Count > 0)
                        ViewBag.EmpName = ObjEmployees[0].FULL_NAME_AR;
                }
            }

            if (!String.IsNullOrEmpty(RoomId))
            {
                CurRoomId = int.Parse(RoomId);
                ViewBag.RoomId = RoomId;
                Room_tbl ObjRoom = unitWork.RoomsManager.GetById(CurRoomId);
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
                }

            }
            if (!String.IsNullOrEmpty(DeptId))
            {
                CurDeptId = int.Parse(DeptId);
                ViewBag.DeptId = DeptId;
                List<vwDepartments> ObjDepartments = unitWork.DepartmentManager.GetDeptById(CurDeptId.GetValueOrDefault());
                if (ObjDepartments != null)
                {
                    if (ObjDepartments.Count > 0)
                        ViewBag.DeptName = ObjDepartments[0].Name;
                }
            }


            ItemsStockViewModel model = new ItemsStockViewModel();
            List<tbl_ItemsStock> ItemsList;
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            

            if (!String.IsNullOrEmpty(Search_Data))
            {
                if (!String.IsNullOrEmpty(RoomId))
                {
                    long StoreId = long.Parse(RoomId);
                    ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data, StoreId).OrderByDescending(m => m.Item_Id).ToList();  
                }
                else
                {
                    ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data, null).OrderByDescending(m => m.Item_Id).ToList();
                }
            }
            else
            {
                int? ItemId =null;
                if (CurEmpId==null  && CurDeptId == null && CurRoomId==null)
                {
                    ItemId = -1;
                }
                 ItemsList = unitWork.ItemsStockManager.GetByParam(ItemId, CurDeptId, CurRoomId, CurEmpId,null).OrderByDescending(m => m.Item_Id).ToList();
            }
            bool ZeroFlagVaule = false;
            if (!String.IsNullOrEmpty(ZeroFlag)) // check qty of zero value
            {
                 ZeroFlagVaule = bool.Parse(ZeroFlag);
                ViewBag.ZeroFlag = ZeroFlagVaule;
            }
            else
            {
                ViewBag.ZeroFlag = false;
                 ZeroFlagVaule = false;
            }
            if (ZeroFlagVaule)
            {
                ItemsList = ItemsList.Where(c => c.ItemQty > 0).ToList();
            }
            model.ItemsStock = null;
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";

            ViewBag.Flag = Flag;
            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;
            
            switch (Sorting_Order)
            {
                case "ID":
                    ItemsList = ItemsList.OrderBy(stu => stu.StockId).ToList();
                    break;
                case "TitleAr":
                    ItemsList = ItemsList.OrderBy(stu => stu.ItemName).ToList();
                    break;
                case "RFID":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_RFID).ToList();
                    break;
                case "ItemBarcode":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_BarCode).ToList();
                    break;
                case "SerialNo":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Serial).ToList();
                    break;
                case "Quantity":
                    ItemsList = ItemsList.OrderBy(stu => stu.ItemQty).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.StockId).ToList();
                    break;
            }
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.PageNumber = No_Of_Page;
            model.PageNumber = No_Of_Page;
            model.ItemsStock = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (ItemsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [UserPermissionAttribute(AllowFeature = "Items Search", AllowPermission = "Accessing")]
        public ActionResult ItemsSearchStockList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string Flag)
        {
        

            ItemsStockViewModel model = new ItemsStockViewModel();
            List<tbl_ItemsStock> ItemsList;
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }


            //if (!String.IsNullOrEmpty(Search_Data))
            //{
            //carList = Buildings.Where(stu => stu.Carid == 61);
            //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            //Buildings.Find()
            //if (!String.IsNullOrEmpty(RoomId))
            //{
            //    long StoreId = long.Parse(RoomId);
            //    ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data, StoreId).OrderByDescending(m => m.Item_Id).ToList();

            //}
            //else
            //{
            int userId = SesssionUser.GetCurrentUserId();
            UnitOfWork UWork = new UnitOfWork();
            List<CatMain_tbl > MainCats = UWork.CatMainManager.GetUserMainCats(userId).ToList();

            //var listOfMainCatsId = MainCats.Select(r => r.CatMain_Id).ToList();
            ItemsList = unitWork.ItemsStockManager.GetByItemName(Search_Data, MainCats).OrderByDescending(m => m.Item_Id).ToList();
            //List = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();

            //List<tbl_ItemsStock> NewList = ItemsList.Where(c => listOfMainCatsId.Contains(c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id)).ToList();

           
                //}
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            //}
            //else
            //{
            //    int? ItemId = null;
            //    if (CurEmpId == null && CurDeptId == null && CurRoomId == null)
            //    {
            //        ItemId = -1;
            //    }
            //    ItemsList = unitWork.ItemsStockManager.GetByParam(ItemId, CurDeptId, CurRoomId, CurEmpId, null).OrderByDescending(m => m.Item_Id).ToList();
            //}

            //ItemsList = ItemsList.Where(r => r.Emp_Id == CurEmpId || r.Depart_Id == CurDeptId || r.Room_Id == CurRoomId).ToList(); //.FirstOrDefault();

            model.ItemsStock = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";

            ViewBag.Flag = Flag;
            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;

            switch (Sorting_Order)
            {
                case "ID":
                    ItemsList = ItemsList.OrderBy(stu => stu.StockId).ToList();
                    break;
                case "TitleAr":
                    ItemsList = ItemsList.OrderBy(stu => stu.ItemName).ToList();
                    break;
                case "RFID":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_RFID).ToList();
                    break;
                case "ItemBarcode":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_BarCode).ToList();
                    break;
                case "SerialNo":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Serial).ToList();
                    break;
                case "Quantity":
                    ItemsList = ItemsList.OrderBy(stu => stu.ItemQty).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.StockId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.PageNumber = No_Of_Page;
            model.PageNumber = No_Of_Page;
            model.ItemsStock = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (ItemsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        [ActionName("AddEditItemStock")]
        public ActionResult AddEditItemStock(string Id = null, int? PageNumber=null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetNotDelAll();
            //var CurStatusList = unitWork.StatusManager.GetNotDelAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");

            if (Request.QueryString["RoomId"] != null)
            {
                ViewBag.RoomId = Request.QueryString["RoomId"].ToString();
                int RoomId = int.Parse(ViewBag.RoomId);
                Room_tbl Room = unitWork.RoomsManager.GetById(RoomId);
                ViewBag.RoomName = Room.Room_Name;
            }
           


                ItemsStockViewModel model = new ItemsStockViewModel();
            //model.Status = unitWork.StatusManager.GetAll().Select(option => new SelectListItem
            //{
            //    Text = option.AName,
            //    Value = option.Item_StateId.ToString()
            //});
            model.vwContracts = unitWork.VendorContractsManager.GetNotDelAll().Select(option => new SelectListItem
            {
                Text = option.ContractName,
                Value = option.ContractId.ToString()
            });

            if (String.IsNullOrEmpty(Id))
            {
                tbl_ItemsStock Record = new tbl_ItemsStock();
                Record.ItemQty = 1;
                model.SelectedItem = Record;
                model.DisplayMode = "ReadOnly";

           
                return View(model);
                // return PartialView("EditAddSupplierModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
               
                int StockId = int.Parse(Id);

                model.ItemsStock = unitWork.ItemsStockManager.GetNotDelAll().OrderBy(m => m.StockId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.ItemsStockManager.GetById(StockId);
                if (Request.QueryString["Flag"] != null)
                {
                    ViewBag.Flag = Request.QueryString["Flag"].ToString();
                    ViewBag.Item_Name = model.SelectedItem.Item_tbl.Item_Name;
                    ViewBag.CatSub_Name = model.SelectedItem.Item_tbl.CatSub_tbl.CatSub_Name;
                    ViewBag.SubCatId = model.SelectedItem.Item_tbl.CatSub_tbl.CatSub_Id;
                    ViewBag.Cat_Name = model.SelectedItem.Item_tbl.CatSub_tbl.Category_tbl.Cat_Name;
                    ViewBag.Cat_Id = model.SelectedItem.Item_tbl.CatSub_tbl.Category_tbl.Cat_Id;
                    ViewBag.CatMain_Name = model.SelectedItem.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Name;
                    ViewBag.MainCatId = model.SelectedItem.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.CatMain_Id;
                }
                if (model.SelectedItem != null)
                {
                    ViewBag.RoomId = model.SelectedItem.Room_Id;
                }
                model.DisplayMode = "ReadWrite";
                if (PageNumber != null && PageNumber!=0)
                {
                    model.PageNumber = PageNumber;
                   
                }
                else
                {
                    PageNumber = 1;
                }
                ViewBag.PageNumber = PageNumber;
                if (model.SelectedItem == null) { return View("_error"); }
                // ...
                return View(model);
                //return PartialView("EditAddSupplierModal", model);
            }

        }

        [Route("AddEditItemStock")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddEditItemStock(ItemsStockViewModel ItemsStock)
        {

            tbl_ItemsStock existing = ItemsStock.SelectedItem;
            long ItemId = existing.Item_Id;

            Item_tbl item = unitWork.ItemsManager.GetById(ItemId);
            if (item != null)
            {
                if (!item.CountableFlag && existing.ItemQty > 1) // غير نثري و كمية اكبر من 1
                {
                    // لايمكن الحفظ الصنف لانه الكمية يجب ان تكون 1
                    ViewBag.ErrorMessage = "Error";

                    return View(ItemsStock);
                }
                else
                {

                }

            }


            if (ModelState.IsValid)
            {

                if (ItemsStock.SelectedItem.StockId == 0)
                {
                    // insert new record

                    tbl_ItemsStock ItemsStockobj = unitWork.ItemsStockManager.Add(existing);

                    if (ItemsStockobj != null)
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
                    int? Qty = ItemsStock.SelectedItem.ItemQty;
                    existing = unitWork.ItemsStockManager.GetById(ItemsStock.SelectedItem.StockId);

                    existing.Item_Id = ItemsStock.SelectedItem.Item_Id;
                    existing.Item_RFID = ItemsStock.SelectedItem.Item_RFID;
                    existing.Item_BarCode = ItemsStock.SelectedItem.Item_BarCode;
                    existing.Item_Serial = ItemsStock.SelectedItem.Item_Serial;
                    existing.Item_StateId = ItemsStock.SelectedItem.Item_StateId;
                    existing.Item_Expire = ItemsStock.SelectedItem.Item_Expire;
                    existing.Location  = ItemsStock.SelectedItem.Location;
                    existing.QuotaId = ItemsStock.SelectedItem.QuotaId;
                    existing.Item_Stock_Limit = ItemsStock.SelectedItem.Item_Stock_Limit;
                    existing.Notes = ItemsStock.SelectedItem.Notes;
                    existing.ContractId  = ItemsStock.SelectedItem.ContractId;
                    existing.MainCostPrice = ItemsStock.SelectedItem.MainCostPrice;
                    existing.AssetFlag  = ItemsStock.SelectedItem.AssetFlag;

                    bool ret = unitWork.ItemsStockManager.Update(existing);
                    // update record

                    if (ret)
                    {

                        TempData["Message"] = "Success";
                    }
                    else
                    {
                        TempData["Message"] = null;
                    }
                    ItemsStock.SelectedItem.ItemQty = Qty;
                }

            }


            //return View(ItemsStock);
            return RedirectToAction("AddEditItemStock",new { Id = existing.StockId});


        }



        // End of Items stock

        // Start of Supplier  module
        public ActionResult SuppliersList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            SuppliersViewModel model = new SuppliersViewModel();
            List<Suppliers_tbl> SuppliersList = unitWork.SuppliersManager.GetNotDelAll().OrderBy(m => m.Sup_code).ToList();
            model.SelectedItem = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Sup_code" : "";
            ViewBag.SortingModel = Sorting_Order == "Sup_code" ? "Sup_Name" : "Sup_Ename";

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

                SuppliersList = unitWork.SuppliersManager.GetCastByName(Search_Data);

            }
            switch (Sorting_Order)
            {
                case "Sup_code":
                    SuppliersList = SuppliersList.OrderBy(stu => stu.Sup_code).ToList();
                    break;
                case "Sup_Name":
                    SuppliersList = SuppliersList.OrderBy(stu => stu.Sup_Name).ToList();
                    break;
                case "Sup_Ename":
                    SuppliersList = SuppliersList.OrderBy(stu => stu.Sup_Ename).ToList();
                    break;
                default:
                    SuppliersList = SuppliersList.OrderBy(stu => stu.Sup_code).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.Suppliers = SuppliersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (SuppliersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ActionName("DeleteSupplier")]
        public ActionResult DeleteSupplier(string id)
        {
            int Id = int.Parse(id);
            Suppliers_tbl existing = unitWork.SuppliersManager.GetById(Id);
            if (existing != null)
            {
                unitWork.SuppliersManager.Delete(existing);
                //unitWork.BuildingsManager.Update(existing);

                SuppliersViewModel model = new SuppliersViewModel();
                model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().OrderBy(m => m.Sup_code).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedItem = null;
                model.DisplayMode = "";

                return RedirectToAction("SuppliersList", new { SupId = existing.Sup_code });
            }
            else
            {
                return RedirectToAction("SuppliersList");
            }
        }


        [HttpGet]
        [ActionName("AddEditSupplier")]
        public ActionResult AddEditSupplier(string id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetNotDelAll();
            //var CurStatusList = unitWork.StatusManager.GetNotDelAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            SuppliersViewModel model = new SuppliersViewModel();

            if (String.IsNullOrEmpty(id))
            {
                Suppliers_tbl FloorRecord = new Suppliers_tbl();
                model.SelectedItem = FloorRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("EditAddSupplierModal", model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int ModelId = int.Parse(id);

                model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().OrderBy(m => m.Sup_code).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.SuppliersManager.GetById(ModelId);
                model.DisplayMode = "ReadWrite";
                if (model.SelectedItem == null) { return View("_error"); }
                // ...
                return PartialView("EditAddSupplierModal", model);
            }

        }

        [Route("SaveSupplier")]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveSupplier(SuppliersViewModel Supplier)
        {
            Suppliers_tbl existing = Supplier.SelectedItem;
            if (ModelState.IsValid)
            {

                if (Supplier.SelectedItem.Sup_code == 0)
                {
                    // insert new record

                    Suppliers_tbl Supplierobj = unitWork.SuppliersManager.Add(existing);
                    if (Supplierobj != null)
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
                    existing = unitWork.SuppliersManager.GetById(Supplier.SelectedItem.Sup_code);

                    existing.Sup_Name = Supplier.SelectedItem.Sup_Name;
                    existing.Sup_Ename = Supplier.SelectedItem.Sup_Ename;
                    bool ret = unitWork.SuppliersManager.Update(existing);
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

            SuppliersViewModel model = new SuppliersViewModel();
            model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().OrderBy(m => m.Sup_code).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.SelectedItem = existing;
            model.DisplayMode = "ReadOnly";
            //return View("FloorsList", model,   );
            return RedirectToAction("SuppliersList");


        }
        // End of  Supplier Module------------------------------------------------------------------------------




        //public ActionResult AssetItemsList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        //{

        //    ItemsStockViewModel model = new ItemsStockViewModel();

        //    List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
        //    List<tbl_ItemsStock> ToItemsList = ItemsList;// = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();

        //    model.FromEmpId = 0;
        //    model.ToEmpId = 0;
        //    model.searchType = 1;
        //    model.searchTypeTo = 1;
        //    model.ActionType = 1;
        //    model.ItemsPopulateList(model.FromEmpId, null, 1);
        //    model.ItemsPopulateList(model.ToEmpId, null, 2);

        //    ItemsList = ItemsList.Where(r => r.Emp_Id == model.FromEmpId && r.Depart_Id == model.FromDeptId && r.Room_Id == model.FromRoomId).ToList(); //.FirstOrDefault();                                                                                                                                          //ItemsList = ItemsList.Where(r => r.Emp_Id == CurEmpId || r.Depart_Id == CurDeptId || r.Room_Id == CurRoomId).ToList(); //.FirstOrDefault();
        //    ToItemsList = ToItemsList.Where(r => r.Emp_Id == model.ToEmpId && r.Depart_Id == model.ToDeptId && r.Room_Id == model.ToRoomId).ToList(); //.FirstOrDefault();
        //    model.ItemsStock = null;

        //    ViewBag.CurrentSortOrder = Sorting_Order;
        //    ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
        //    ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";

        //    if (Search_Data != null)
        //    {
        //        Page_No = 1;
        //    }
        //    else
        //    {
        //        Search_Data = Filter_Value;
        //    }

        //    ViewBag.FilterValue = Search_Data;

        //    if (!String.IsNullOrEmpty(Search_Data))
        //    {

        //        ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data);

        //    }
        //    switch (Sorting_Order)
        //    {
        //        case "Item_Id":
        //            ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
        //            break;
        //        case "Emp_Id":
        //            ItemsList = ItemsList.OrderBy(stu => stu.Emp_Id).ToList();
        //            break;
        //        case "Depart_Id":
        //            ItemsList = ItemsList.OrderBy(stu => stu.Depart_Id).ToList();
        //            break;
        //        default:
        //            ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
        //            //carList =
        //            break;
        //    }
        //    int Size_Of_Page = 500;
        //    int No_Of_Page = (Page_No ?? 1);
        //    model.ItemsStock = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
        //    model.ToItemsStock = ToItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
        //    if (ItemsList.Any())
        //    {
        //        return View(model);
        //    }
        //    else
        //    {
        //        return View(model);
        //    }

        //}
        [UserPermissionAttribute(AllowFeature = "Assets Transfer", AllowPermission = "Accessing")]
        public ActionResult AssetItemsList(ItemsStockViewModel Mymodel,string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string Flag)
        {
            return (GetData(Mymodel,Sorting_Order, Search_Data, Filter_Value, Page_No,Flag));
        }
        [HttpPost]
        public ActionResult AssetItemsList(ItemsStockViewModel Mymodel ,string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string Flag, int?bFlag )
        {
            try
            {
                if (Request.Form["CmdSave"] == Resources.DefaultResource.TransferAsset)
                {
                    if (Mymodel.PostFlag == 1)
                    {
                        SaveItems(Mymodel);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value,Page_No,Flag));
        }
        public ActionResult GetData(ItemsStockViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string Flag)
        {
                   
            ItemsStockViewModel model = new ItemsStockViewModel();
            List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
               
            if (Mymodel.searchType == 0 && Mymodel.searchTypeTo==0) // action to do in form fro assets
            {
                //Mymodel.FromEmpId = 0;
                //Mymodel.ToEmpId = 0;
                Mymodel.searchType = 1;
                Mymodel.searchTypeTo = 1;
                Mymodel.ActionType = 1;
            }
            
            model.FromEmpId = Mymodel.FromEmpId;
            model.FromDeptId = Mymodel.FromDeptId;
            model.FromRoomId = Mymodel.FromRoomId;
            model.DisplayMode = Mymodel.DisplayMode;
            model.searchType = Mymodel.searchType;

            model.ToEmpId = Mymodel.ToEmpId;
            model.ToDeptId = Mymodel.ToDeptId;
            model.ToRoomId = Mymodel.ToRoomId;
            model.ToDisplayMode = Mymodel.ToDisplayMode;
            model.searchTypeTo = Mymodel.searchTypeTo;

            model.ActionType = Mymodel.ActionType;

            @ViewBag.Flag = Flag;

            //model.ItemsPopulateList(Mymodel.FromEmpId, null);
            switch (Mymodel.searchType)
            {
                case 1:
                    if (Mymodel.FromEmpId != null)
                    {
                        model.setDropDrownList("FromEmpId", Mymodel.FromEmpId.GetValueOrDefault(), 1);
                    }
                    else
                    {
                        model.ItemsPopulateList(0, null, 1);
                    }

                    break;
                case 2:
                    if (Mymodel.FromRoomId != null)
                    {
                        model.setDropDrownList("FromRoomId", Mymodel.FromRoomId.GetValueOrDefault(), 1);
                    }
                    else
                    {
                        model.ItemsPopulateList(null, 0, 1);
                    }

                    break;
            }

            switch (Mymodel.searchTypeTo)
            {
                case 1:
                    if (Mymodel.ToEmpId != null)
                    {
                        model.setDropDrownList("ToEmpId", Mymodel.ToEmpId.GetValueOrDefault(), 2);
                    }
                    else
                    {
                        model.ItemsPopulateList(0, null, 2);
                    }

                    break;
                case 2:
                    if (Mymodel.ToRoomId != null)
                    {
                        model.setDropDrownList("ToRoomId", Mymodel.ToRoomId.GetValueOrDefault(), 2);
                    }
                    else
                    {
                        model.ItemsPopulateList(null, 0, 2);
                    }

                    break;
            }



            // check From Employeee
            if (model.FromEmpId == 0 || model.FromEmpId == null)
            {

                ItemsList = ItemsList.Where(r => r.Depart_Id == model.FromDeptId && r.Room_Id == model.FromRoomId).ToList(); //.FirstOrDefault();
            }
            else
            {
                ItemsList = ItemsList.Where(r => r.Emp_Id == model.FromEmpId && r.Depart_Id == model.FromDeptId && r.Room_Id == model.FromRoomId).ToList(); //.FirstOrDefault();
            }
            // Check To Employeee
            if (model.ToEmpId == 0 || model.ToEmpId == null)
            {
                ToItemsList = ToItemsList.Where(r => r.Depart_Id == model.ToDeptId && r.Room_Id == model.ToRoomId).ToList(); //.FirstOrDefault();
            }
            else
            {
                ToItemsList = ToItemsList.Where(r => r.Emp_Id == model.ToEmpId && r.Depart_Id == model.ToDeptId && r.Room_Id == model.ToRoomId).ToList(); //.FirstOrDefault();
            }
            model.ItemsStock = null;
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";

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
                ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data,null);
            }
            switch (Sorting_Order)
            {
                case "Item_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                    break;
                case "Emp_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.Emp_Id).ToList();
                    break;
                case "Depart_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.Depart_Id).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 500;
            int No_Of_Page = (Page_No ?? 1);
            model.ItemsStock = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
           
            model.ToItemsStock = ToItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
            // fill  Source items in checklist 
            model.ItemsStockCheckList.Clear();
            model.ToItemsStockCheckList.Clear();
            List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
            foreach (var item in model.ItemsStock)
            {
                bool IsSelected = false;
                if (item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl != null)
                {
                    if (item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected != null)
                    {
                        IsSelected = item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected.GetValueOrDefault();
                    }
                }
                CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id=item.StockId, Name = "Item" + item.StockId };
                ItemsCheckBoxList.Add(CheckItem);
            }
            // fill Target items in checklist 
            List<CheckBoxListItem> ToItemsCheckBoxList = new List<CheckBoxListItem>();
            foreach (var item in model.ToItemsStock)
            {
                bool IsSelected = false;
                if (item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl != null)
                { 
                    if (item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected != null)
                    {
                        IsSelected = item.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected.GetValueOrDefault();
                    }
                 }
                CheckBoxListItem CheckItem2 = new CheckBoxListItem() { IsSelected = IsSelected, ItemId = item.StockId, Id = item.StockId, Name = "ToItem" + item.StockId };
                ToItemsCheckBoxList.Add(CheckItem2);
            }
            

            model.ItemsStockCheckList = ItemsCheckBoxList;
            model.ToItemsStockCheckList = ToItemsCheckBoxList;

            if (ItemsList.Any())
            {
                return View(model);
            }
            else
            {
                return View(model);
            }
        }
              

        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AssetItemsList", "Inventory", Mymodel);
            //// Will save the data to the DB after if ModelState is valid
            
        }
       public  bool UpdateItemAsset(ItemsStockViewModel Mymodel)
        {
            bool ret = true;
            try
            {
                long TansId = SaveAssetTranfer(Mymodel);

                foreach (var item in Mymodel.ItemsStockCheckList)
                {
                    if (item.IsSelected)
                    {
                        if (Mymodel.ActionType == 3 || Mymodel.ActionType == 4)
                        {
                            unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.ToDeptId, Mymodel.ToRoomId.GetValueOrDefault(), Mymodel.ToEmpId);
                        }
                        else
                        {
                            // set selected asset to source employeee 
                            unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.FromDeptId, Mymodel.ToRoomId.GetValueOrDefault(), Mymodel.FromEmpId);
                            
                        }
                        //SaveAssetTranferDetails(item.Id, TansId, Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.FromEmpId,
                        //       Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.ToEmpId);
                    }
                    else
                    {
                        switch (Mymodel.ActionType)
                        {
                            case 1:
                                // clear employee asset from source just keep Departement and room
                                unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.FromDeptId, Mymodel.FromRoomId.GetValueOrDefault(), null);
                               // SaveAssetTranferDetails(item.Id, TansId, Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.FromEmpId,
                               //Mymodel.FromRoomId, Mymodel.FromDeptId, null); //  to Employee Null تحويل نفس الغرفة 

                                break;
                            case 2:
                                //swtich department and employee
                                unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.ToDeptId, Mymodel.FromRoomId.GetValueOrDefault(), Mymodel.ToEmpId);
                                //SaveAssetTranferDetails(item.Id, TansId, Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.FromEmpId,
                                //Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.ToEmpId); //  to Employee Null تحويل نفس الغرفة 

                                break;
                            case 3:
                                break;

                        }
                    }
                }

                // الجاني الاخر الذي يتم التحويل اليه
                // فى حالة تبديل المكاتب
                if (Mymodel.ActionType == 2)
                {
                    foreach (var item in Mymodel.ToItemsStockCheckList)
                    {
                        if (item.IsSelected)
                        {
                            // set selected asset to source employeee 
                            unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.ToDeptId, Mymodel.FromRoomId.GetValueOrDefault(), Mymodel.ToEmpId);
                            
                            //SaveAssetTranferDetails(item.Id, TansId, Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.ToEmpId,
                            //    Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.FromEmpId);
                            //selectedCheckBox += item.Name + ",";
                        }
                        else
                        {
                            switch (Mymodel.ActionType)
                            {
                                case 2:
                                    //swtich department and employee
                                    unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.FromDeptId, Mymodel.ToRoomId.GetValueOrDefault(), Mymodel.FromEmpId);
                                    
                                    //SaveAssetTranferDetails(item.Id, TansId, Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.ToEmpId,
                                    //Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.FromEmpId);

                                    break;
                            }
                        }
                    }
                } // فى حالة تبديل المكاتب

                // فى حالة نقل موظف الى غرفة فارغه
                if (Mymodel.ActionType == 1) 
                {
                    foreach (var item in Mymodel.ToItemsStockCheckList)
                    {

                        // set selected asset to source employeee 
                        unitWork.ItemsStockManager.updateDept_Room_Employee(item.Id, Mymodel.FromDeptId, Mymodel.ToRoomId.GetValueOrDefault(), Mymodel.FromEmpId);
                       
                        //SaveAssetTranferDetails(item.Id, TansId, Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.ToEmpId,
                        //        Mymodel.ToRoomId, Mymodel.ToDeptId, Mymodel.FromEmpId);
                        //selectedCheckBox += item.Name + ",";

                    }
                }

                List<tbl_ItemsStock> FromObj = unitWork.ItemsStockManager.GetNotDelByDeptId(null, Mymodel.FromRoomId.GetValueOrDefault(), null);
                foreach (var item in FromObj)
                {
                    SaveAssetTranferDetails(item.StockId, TansId, item.Room_Id ,item.Depart_Id ,item.Emp_Id, 2);
                }
                List<tbl_ItemsStock> ToObj = unitWork.ItemsStockManager.GetNotDelByDeptId(null, Mymodel.ToRoomId.GetValueOrDefault(), null);
                foreach (var item in ToObj)
                {
                    SaveAssetTranferDetails(item.StockId, TansId, item.Room_Id, item.Depart_Id, item.Emp_Id, 2);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
           
            return ret;
        }
        //check user select at least on item or not
        public bool CheckedAssetSelection(ItemsStockViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.ItemsStockCheckList)
            {
                if (item.IsSelected)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        // save data and make transfer
        [HttpPost]
        public ActionResult SaveItems(ItemsStockViewModel Mymodel)
        {
            bool ret = true;
            // Check user select assets
            if (CheckedAssetSelection(Mymodel))
            {
              
                // When the Save data
                switch (Mymodel.ActionType)
                {
                    case 1: // Move_Employee_To_Empty_Room
                        if (Mymodel.FromEmpId.GetValueOrDefault() != 0 && Mymodel.FromRoomId.GetValueOrDefault() != 0 && Mymodel.FromDeptId != 0) // check select employee and room
                        {
                            if (Mymodel.ToRoomId.GetValueOrDefault() != 0 && Mymodel.ToDeptId != 0 && Mymodel.ToEmpId.GetValueOrDefault() == 0)
                            {
                                //change department,room, and employee asset
                                //Switching Personal assets
                                UpdateItemAsset(Mymodel);

                                // Get record of Employee Rooms
                                List<Emp_rooms> EmpRoomsList;
                                EmpRoomsList = unitWork.EmpRoomsManager.GetRoomByParam(Mymodel.FromDeptId, Mymodel.FromRoomId, Mymodel.FromEmpId);
                                Emp_rooms row = EmpRoomsList.FirstOrDefault();
                                row.EmpId = null;
                                //set From employee null and keep with the same room and depatrment
                                unitWork.EmpRoomsManager.Update(row); // save data

                                //Get record of target Employee
                                EmpRoomsList = unitWork.EmpRoomsManager.GetRoomByParam(Mymodel.ToDeptId, Mymodel.ToRoomId, Mymodel.ToEmpId);
                                row = EmpRoomsList.FirstOrDefault();
                                //set To employee with Source employee and keep with the same room and depatrment
                                row.EmpId = Mymodel.FromEmpId;
                                row.DeptId = Mymodel.FromDeptId;
                                unitWork.EmpRoomsManager.Update(row); // save data
                                TempData["Message"] = Resources.DefaultResource.SwitchSucessMsg; 
                            }
                            else // display message to user to select To Room
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.EmptyRoomMsg;
                                ret = false;
                            }
                        }
                        else
                        {
                            TempData["ErroMessage"] = Resources.DefaultResource.EmptyRoomMsg;
                            ret = false;
                        }

                        break;

                    case 2:// switch staff offices
                        if (Mymodel.FromEmpId.GetValueOrDefault() != 0 && Mymodel.FromRoomId.GetValueOrDefault() != 0 && Mymodel.FromDeptId != 0) // check select employee and room
                        {
                            if (Mymodel.ToRoomId.GetValueOrDefault() != 0 && Mymodel.ToDeptId != 0 && Mymodel.ToEmpId.GetValueOrDefault() != 0)
                            {
                                //Switching Personal assets
                                UpdateItemAsset(Mymodel);
                                // Get record of From Employee Rooms
                                List<Emp_rooms> FromEmpRoomsList;
                                FromEmpRoomsList = unitWork.EmpRoomsManager.GetRoomByParam(Mymodel.FromDeptId, Mymodel.FromRoomId, Mymodel.FromEmpId);
                                Emp_rooms Fromrow = FromEmpRoomsList.FirstOrDefault();

                                // Get record of To Employee Rooms
                                List<Emp_rooms> ToEmpRoomsList = unitWork.EmpRoomsManager.GetRoomByParam(Mymodel.ToDeptId, Mymodel.ToRoomId, Mymodel.ToEmpId);
                                Emp_rooms Torow = ToEmpRoomsList.FirstOrDefault();

                                //Switch Department and employee of source Room
                                Fromrow.EmpId = Mymodel.ToEmpId;
                                Fromrow.DeptId = Mymodel.ToDeptId;

                                //Switch Department and employee of Destination Room
                                Torow.EmpId = Mymodel.FromEmpId;
                                Torow.DeptId = Mymodel.FromDeptId;

                                //set From employee null and keep with the same room and depatrment
                                unitWork.EmpRoomsManager.Update(Fromrow); // save data of Source Room
                                unitWork.EmpRoomsManager.Update(Torow); // save data of Destination Room

                                                            
                                TempData["Message"] = Resources.DefaultResource.SwitchSucessMsg;

                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.EmptyEmployeeMsg;
                            }
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.EmptyEmployeeMsg;
                        }
                        break;

                    case 3: //transfer asset between two employees
                        if (Mymodel.FromEmpId.GetValueOrDefault() != 0 && Mymodel.FromRoomId.GetValueOrDefault() != 0 && Mymodel.FromDeptId != 0) // check select employee and room
                        {
                            if (Mymodel.ToRoomId.GetValueOrDefault() != 0 && Mymodel.ToDeptId != 0 && Mymodel.ToEmpId.GetValueOrDefault() != 0)
                            {
                                //change department,room, and employee asset
                                //Switching Personal assets
                                UpdateItemAsset(Mymodel);

                            }
                        }
                         break;
                    case 4: //transfer asset between two Rooms
                        if ( Mymodel.FromRoomId.GetValueOrDefault() != 0 && Mymodel.FromDeptId != 0) // check select From room
                        {
                            if (Mymodel.ToRoomId.GetValueOrDefault() != 0 && Mymodel.ToDeptId != 0) // check select To room
                            {
                                //change department,room, and employee asset
                                //Switching Personal assets
                                UpdateItemAsset(Mymodel);

                            }
                        }
                        break;
                       
                    
                }
               
            }
            else
            {
                TempData["ErroMessage"] = Resources.DefaultResource.NoSelectionMsg;
            }
                // to the "Index" Action with the bookViewModel to display form errors
                return RedirectToAction("AssetItemsList", "Inventory", Mymodel);
            
            
        }
        // save Master data of Tranfer
        public long SaveAssetTranfer(ItemsStockViewModel Mymodel)
        {
            try
            {
                TransferAssets ObjTransferAssets = new TransferAssets ();
                ObjTransferAssets.TransDeptId_From = Mymodel.FromDeptId;
                ObjTransferAssets.TransDeptId_To = Mymodel.ToDeptId;
                ObjTransferAssets.TransEmpId_From = Mymodel.FromEmpId;
                ObjTransferAssets.TransEmpId_To = Mymodel.ToEmpId;
                ObjTransferAssets.TransRoomId_From = Mymodel.FromRoomId;
                ObjTransferAssets.TransRoomId_To = Mymodel.ToRoomId;
                ObjTransferAssets.TransDate =DateTime.Now;
                ObjTransferAssets.TransType = Mymodel.ActionType;
                unitWork.TransferAssetsManager.Add(ObjTransferAssets);

                foreach (var item in Mymodel.ItemsStockCheckList)
                {
                    SaveAssetTranferDetails(item.Id, ObjTransferAssets.TansId, Mymodel.FromRoomId, Mymodel.FromDeptId, Mymodel.FromEmpId, 1);
                }
                foreach (var item in Mymodel.ToItemsStockCheckList)
                {
                    SaveAssetTranferDetails(item.Id, ObjTransferAssets.TansId, Mymodel.ToRoomId , Mymodel.ToDeptId, Mymodel.ToEmpId, 1);
                }
                return ObjTransferAssets.TansId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public void SaveAssetTranferDetails( long stockId, long TansId,
            long? RoomId, int ? DeptId, int ? EmpId,int StatusId)
        {
            try
            {
                    TransferAssetsDetails ObjTransferAssetsDetails = new TransferAssetsDetails();
                    ObjTransferAssetsDetails.StockId = stockId;
                    ObjTransferAssetsDetails.TansId = TansId;

                    ObjTransferAssetsDetails.DeptId = DeptId;
                    ObjTransferAssetsDetails.EmpId = EmpId;
                    ObjTransferAssetsDetails.RoomId = RoomId;
                    ObjTransferAssetsDetails.StatusId  = StatusId;
                
                    unitWork.TransferAssetsDetailsManager.Add(ObjTransferAssetsDetails);                  
                
            }
            catch (Exception ex)
            {


            }
        }
        public ActionResult ItemsDamagedStockList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            DestroyOrdersDetialsViewModel model = new DestroyOrdersDetialsViewModel();
            List<DestroyOrdersDetails> ItemsList = unitWork.DestroyOrdersDetailsManager.GetNotDelAllAndDamaged (Search_Data).OrderByDescending(m => m.StockId).ToList();
            //ItemsStockViewModel model = new ItemsStockViewModel();
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAllAndDamaged(Search_Data).OrderByDescending(m => m.Item_Id).ToList();
                       
            model.DestroyOrdersDetails = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Item_Id" : "Depart_Id";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
          
            switch (Sorting_Order)
            {
                case "Item_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.tbl_ItemsStock.Item_Id).ToList();
                    break;
                case "Emp_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.tbl_ItemsStock.Emp_Id).ToList();
                    break;
                case "Depart_Id":
                    ItemsList = ItemsList.OrderBy(stu => stu.tbl_ItemsStock.Depart_Id).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.tbl_ItemsStock.Item_Id).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.DestroyOrdersDetails = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (ItemsList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }

        [UserPermissionAttribute(AllowFeature = "Items Search", AllowPermission = "Accessing")]
        public ActionResult ItemsStockContractList(ItemsStockViewModel Mymodel, string Sorting_Order, string Search_Data,string VendorId, string ContractId, string Filter_Value, int? Page_No,int? SearchFlag)
        {
            return GetContractData(Mymodel,Sorting_Order, Search_Data, Filter_Value, Page_No, SearchFlag, ContractId, VendorId);
        }
        [HttpPost]
        public ActionResult ItemsStockContractList(ItemsStockViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, int? Flag)
        {
            string ContractId=null;
            string VendorId = null;
            try
            {
               
                if (Request.Form["CmdSearch"] == Resources.DefaultResource.SearchItems)
                {
                    Flag = 1;
                    ViewBag.Flag = 1;
                   
                }
                if (Request.Form["CmdSearchItem"] == Resources.DefaultResource.Search)
                {
                    Flag = 2;
                    ViewBag.Flag = 2;
                    
                }
                ContractId = Mymodel.ContractId.ToString();
                ViewBag.ContractId = ContractId;
                VendorId = Mymodel.VendorId.ToString();
                ViewBag.VendorId = VendorId;
               
            }
            catch (Exception ex)
            {


            }
            return GetContractData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, Flag, ContractId, VendorId);
        }

        public ActionResult GetContractData(ItemsStockViewModel model, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,int ? Flag , string ContractId, string VendorId)
        {
            var allVendorsList = unitWork.VendorsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            if (ContractId != null)
            {
               ViewBag.ContractId = model.ContractId;
               Vendor_Contracts obj = unitWork.VendorContractsManager.GetById(model.ContractId);
                ViewBag.ContractName= obj.ContractName;
            }
            if (VendorId != null)
            {
                model.VendorId = model.VendorId.GetValueOrDefault();
            }
            else
            {
                model.VendorId = allVendorsList.Select(m => m.VendorId).FirstOrDefault();
            }
            ViewBag.VendorId = VendorId;
            model.vWVendors = new SelectList(allVendorsList, "VendorId", "VendorName", model.VendorId);
          

            var allContractsList = unitWork.VendorContractsManager.GetNotDelAll().Where(m => m.VendorId == model.VendorId).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.vwContracts = new SelectList(allContractsList, "ContractId", "ContractName", model.ContractId);
            
            List<tbl_ItemsStock> ItemsList = null;
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
               
            }

            if (model.ContractId !=null && Flag ==1)
            {
                ItemsList = unitWork.ItemsStockManager.GetByContractID(model.ContractId).OrderByDescending(m => m.Item_Id).ToList();
            }
            else 
            {
                ItemsList = unitWork.ItemsStockManager.GetCastByUnitName(Search_Data, null).OrderByDescending(m => m.Item_Id).ToList();

            }

            //if (!String.IsNullOrEmpty(Search_Data))
            //{
              

                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            //}

            //model.ItemsStock = null;
            if (ItemsList != null)
            {
                ViewBag.CurrentSortOrder = Sorting_Order;
                ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
                ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";


                ViewBag.FilterValue = Search_Data;
                //var carList = from stu in Buildings select stu;

                switch (Sorting_Order)
                {
                    case "Item_Id":
                        ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                        break;
                    case "Emp_Id":
                        ItemsList = ItemsList.OrderBy(stu => stu.Emp_Id).ToList();
                        break;
                    case "Depart_Id":
                        ItemsList = ItemsList.OrderBy(stu => stu.Depart_Id).ToList();
                        break;
                    default:
                        ItemsList = ItemsList.OrderBy(stu => stu.Item_Id).ToList();
                        //carList =
                        break;
                }
            

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.PageNumber = No_Of_Page;
            ViewBag.Flag = Flag;
            model.PageNumber = No_Of_Page;
            model.ItemsStock = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
        }
            
                return View(model);
            
        }
       

       
        
        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            ItemsStockViewModel model = new ItemsStockViewModel();
            DepartementsViewModel DeptModel = new DepartementsViewModel();
            if (type == "DeptId") { type = "DeptId"; }
            switch (type)
            {
                case "DeptId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    //DeptModel.vwEmployees = Resources.DefaultResource.ListChoose;
                    model.FromEmployees = DeptModel.vwEmployees;
                    //model.EmployeesTemp = DeptModel.Employees;
                    model.FromEmpId = null;

                    break;
                case "VendorId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.vwContracts = DeptModel.vwContracts;
                    break;
            }


            return Json(model);
        }


        [UserPermissionAttribute(AllowFeature = "Items Search", AllowPermission = "Accessing")]
        public ActionResult ItemsStockDetailsList(string StockId,string EmpId, string RoomId, string DeptId, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            int? CurEmpId = null;
            int? CurDeptId = null;
            int? CurRoomId = null;

            if (!String.IsNullOrEmpty(EmpId))
            {
                CurEmpId = int.Parse(EmpId);
                ViewBag.EmpId = EmpId;
                List<vwEmployees> ObjEmployees = unitWork.EmployeesManager.GetEmployeeByEmpId(CurEmpId.GetValueOrDefault());
                if (ObjEmployees != null)
                {
                    if (ObjEmployees.Count > 0)
                        ViewBag.EmpName = ObjEmployees[0].FULL_NAME_AR;
                }
            }

            if (!String.IsNullOrEmpty(RoomId))
            {
                CurRoomId = int.Parse(RoomId);
                ViewBag.RoomId = RoomId;
                Room_tbl ObjRoom = unitWork.RoomsManager.GetById(CurRoomId);
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
                }

            }
            if (!String.IsNullOrEmpty(DeptId))
            {
                CurDeptId = int.Parse(DeptId);
                ViewBag.DeptId = DeptId;
                List<vwDepartments> ObjDepartments = unitWork.DepartmentManager.GetDeptById(CurDeptId.GetValueOrDefault());
                if (ObjDepartments != null)
                {
                    if (ObjDepartments.Count > 0)
                        ViewBag.DeptName = ObjDepartments[0].Name;
                }
            }
            if (!String.IsNullOrEmpty(StockId))
            {
                int CStockId = int.Parse(StockId);
                ViewBag.stockId = CStockId;
                tbl_ItemsStock ObjItemsStock = unitWork.ItemsStockManager.GetById(CStockId);
                if (ObjItemsStock != null)
                {
                    if (ObjItemsStock!=null)
                        ViewBag.ItemName = ObjItemsStock.Item_tbl.Item_Name;
                }
            }
            ItemsStockDetailsViewModel model = new ItemsStockDetailsViewModel();
            List<tbl_ItemsStockDetails> ItemsList;
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            long CurStockId = long.Parse(StockId);
            if (!String.IsNullOrEmpty(Search_Data))
            {
                //carList = Buildings.Where(stu => stu.Carid == 61);
                //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
                //Buildings.Find()
               
                if (!String.IsNullOrEmpty(StockId))
                {
                    
                    ItemsList = unitWork.ItemsStockDetailsManager.GetCastByUnitName(Search_Data, CurStockId).OrderByDescending(m => m.StockDetId).ToList();

                }
                else
                {
                    ItemsList = unitWork.ItemsStockDetailsManager.GetCastByUnitName(Search_Data, null).OrderByDescending(m => m.StockDetId).ToList();
                }
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            else
            {
                             
                ItemsList = unitWork.ItemsStockDetailsManager.GetByStockId(CurStockId).OrderByDescending(m => m.StockDetId ).ToList();
            }

            //ItemsList = ItemsList.Where(r => r.Emp_Id == CurEmpId || r.Depart_Id == CurDeptId || r.Room_Id == CurRoomId).ToList(); //.FirstOrDefault();

            model.ItemsStockDetails = null;

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Item_Id" : "";
            ViewBag.SortingModel = Sorting_Order == "Item_Id" ? "Emp_Id" : "Depart_Id";


            ViewBag.FilterValue = Search_Data;
            //var carList = from stu in Buildings select stu;

            switch (Sorting_Order)
            {
                case "StockDetId":
                    ItemsList = ItemsList.OrderBy(stu => stu.StockDetId).ToList();
                    break;
                case "BarCode":
                    ItemsList = ItemsList.OrderBy(stu => stu.BarCode ).ToList();
                    break;
                case "ExpiredDate":
                    ItemsList = ItemsList.OrderBy(stu => stu.ExpiredDate ).ToList();
                    break;
                default:
                    ItemsList = ItemsList.OrderBy(stu => stu.StockDetId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.PageNumber = No_Of_Page;
            model.PageNumber = No_Of_Page;
            model.ItemsStockDetails = ItemsList.ToPagedList(No_Of_Page, Size_Of_Page);
           
            return View(model);
            

        }

    }
}