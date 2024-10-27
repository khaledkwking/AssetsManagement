using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using nsAlienRFID2;
namespace AssetsManagement.Controllers
{
    public class HandoverOrdersController : Controller
    {


        public int Qty = 0;
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }
        [UserPermissionAttribute(AllowFeature = "HandOver Orders", AllowPermission = "Accessing")]
        public ActionResult HandoverOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            HandoverOrdersViewModel model = new HandoverOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            List<HandoverOrders> HandoverOrdersList = unitWork.HandoverOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending(m => m.OverOrderId).ToList();
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "HandoverOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "HandoverOrderId" ? "HandoverOrderDate" : "StoreId";

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
                HandoverOrdersList = unitWork.HandoverOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "HandoverOrderId":
                    HandoverOrdersList = HandoverOrdersList.OrderByDescending(stu => stu.OverOrderId).ToList();
                    break;
                case "HandoverOrderDate":
                    HandoverOrdersList = HandoverOrdersList.OrderByDescending(stu => stu.OverOrderDate).ToList();
                    break;
                case "StoreId":
                    HandoverOrdersList = HandoverOrdersList.OrderByDescending(stu => stu.StoreId).ToList();
                    break;
                default:
                    HandoverOrdersList = HandoverOrdersList.OrderByDescending(stu => stu.OverOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.HandoverOrders = HandoverOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (HandoverOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditHandoverOrder")]
        public ActionResult AddEditHandoverOrder(string Id = null)
        {
      
            HandoverOrdersViewModel model = new HandoverOrdersViewModel();
            try
            {
                //model.ItemsPopulateList(model.FromEmpId, null,1);
                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("HandOver Orders", "Adding"))
                    {
                        ModelState.Clear();
                        HandoverOrders UnitRecord = new HandoverOrders();
                        UnitRecord.OverOrderDate = DateTime.Today;
                        model.SelectedItem = UnitRecord;
                        model.DisplayMode = "ReadOnly";
                        List<HandOverOrdersDetails> HandoverOrdersDetialsList = new List<HandOverOrdersDetails>();
                        model.HandOverOrdersDetails = HandoverOrdersDetialsList;
                        model.SelectedItem.OrganizedFlag = 1; // personal HandoverOrder
                                                              //return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");
                    }
                }
                else
                {
                    if (this.HasPermission("HandOver Orders", "Updating"))
                    {
                        // Edit record (view in Edit mode)
                        int OrderId = int.Parse(Id);

                        //model.Buildings = unitWork.HandoverOrdersManager.GetNotDelAll().OrderBy(m => m.HandoverOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.HandoverOrdersManager.GetById(OrderId);
                        model.DisplayMode = "ReadWrite";
                        List<HandOverOrdersDetails> HandoverOrdersDetailsList = unitWork.HandOverOrdersDetailsManager.GetByOrderId(OrderId);
                        model.HandOverOrdersDetails = HandoverOrdersDetailsList;
                        if (model.SelectedItem == null) { return View("_error"); }
                        // ...
                        //return View( model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            return GetData(model, "", "", "", 0, true);

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(HandoverOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, false));
        }
        private bool SaveHandoverOrder(HandoverOrdersViewModel Mymodel)
        {
            bool ret = true;
            string ItemName = "";
            try
            {
                if (CheckEnoughQty(Mymodel, ref ItemName))
                {
                    //checked user select invetory
                    if (Mymodel.SelectedItem.StoreId != null)
                    {
                        long StoreId = Mymodel.SelectedItem.StoreId.GetValueOrDefault();
                    }
                    else
                    {
                        ret = false; // end flag
                    }
                    if (Mymodel.HandOverOrdersDetails.Count == 0)
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                        TempData["Message"] = null;
                    }
                    ret = CheckOtherItems(Mymodel, ref ItemName);



                    if (ret)
                    {
                        if (ModelState.IsValid)
                        {
                            if (Mymodel.SelectedItem.OverOrderId == 0)
                            {
                                // insert new record
                                HandoverOrders HandoverOrdersbj = unitWork.HandoverOrdersManager.Add(Mymodel.SelectedItem);
                                if (HandoverOrdersbj != null)
                                {
                                    Mymodel.HandOverOrdersDetails = Mymodel.HandOverOrdersDetails;
                                    TempData["Message"] = "Success";
                                    TempData["warningMessage"] = null;
                                }
                                else
                                {
                                    TempData["Message"] = null;
                                }
                                //ModelState.Clear();
                            }
                            else
                            {
                                bool bret = unitWork.HandoverOrdersManager.Update(Mymodel.SelectedItem);
                                // update record
                            }

                            if (SaveOrderItems(Mymodel))
                            {
                                TempData["Message"] = "Success";
                                TempData["warningMessage"] = null;
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
                    TempData["warningMessage"] = Resources.DefaultResource.InSufficientQtyMsg + " " + ItemName;
                    ret = false;

                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            return ret;
        }
        // check enough quantity to out
        private bool CheckEnoughQty(HandoverOrdersViewModel Mymodel, ref string ItemName)
        {
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.HandOverOrdersDetails)
            {
                int DiffQty = item.InStoreQty.GetValueOrDefault() - item.Qty.GetValueOrDefault();
                if (DiffQty < 0)
                {
                    ItemName = item.ItemId.ToString();
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        private bool CheckOtherItems(HandoverOrdersViewModel Mymodel, ref string ItemName)
        {
            bool ret = true;
            if (Mymodel.SelectedItem.ReasonId < 4 )
            {
                int CurEmpId=0;
                long CurRoomId=0;
                int CurDeptId=0;
                int Count = 0;
                    // check same employee and room
                    int i = 0;
                foreach (var item in Mymodel.HandOverOrdersDetails)
                { 
                    if (i==0)
                    {
                                 CurRoomId = item.StoreId.GetValueOrDefault();
                                 CurDeptId = item.DeptId.GetValueOrDefault();
                                 CurEmpId = item.EmpId.GetValueOrDefault();
                    }

                    Item_tbl Curobj = unitWork.ItemsManager.GetById(item.ItemId);
                    int ItemEmpId = item.EmpId.GetValueOrDefault();// Curobj.Emp_Id.GetValueOrDefault();
                    long ItemRoomId = item.StoreId.GetValueOrDefault(); //Curobj.Room_Id.GetValueOrDefault();
                    int ItemDeptId = item.DeptId.GetValueOrDefault();// Curobj.Depart_Id.GetValueOrDefault();
                    if (CurRoomId == ItemRoomId && CurEmpId == ItemEmpId && CurDeptId == ItemDeptId)
                    {
                        if (Curobj.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected.GetValueOrDefault())
                        {
                                    Count = Count + 1;
                        }

                    }
                    else
                    {
                                ret = false;
                                TempData["warningMessage"] = Resources.DefaultResource.HandOverEmpAndRoomCheckMsg;
                                TempData["Message"] = null;
                                break;
                    }
                }
                List<tbl_ItemsStock> objList = unitWork.ItemsStockManager.GetByParam(null, CurDeptId, CurRoomId, CurEmpId,null);
                objList = objList.Where(c => c.Item_tbl.CatSub_tbl.Category_tbl.CatMain_tbl.IsSelected == true).ToList();
                if (objList.Count != Count)
                {
                  ret = true;
                  TempData["warningMessage"] = Resources.DefaultResource.HandOverUncompletedItemsMsg;
                  TempData["Message"] = null;
                    Mymodel.SelectedItem.Active = false;

                }
                else
                {
                    Mymodel.SelectedItem.Active = true;
                }

                   
               
            }
            return ret;
        }
        private bool SaveOrderItems(HandoverOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.HandOverOrdersDetails)
            {
                item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                if (!item.tbl_ItemsStock.Item_tbl.CountableFlag)
                {
                    item.Qty = 1;
                }
                if (item.OverOrderDetId  == 0)
                {
                    // insert new record
                    item.OverOrderId = Mymodel.SelectedItem.OverOrderId;
                    HandOverOrdersDetails HandoverOrderItemsbj = unitWork.HandOverOrdersDetailsManager.Add(item);
                    if (HandoverOrderItemsbj != null)
                    {
                        //Mymodel.HandoverOrderDetails = Mymodel.HandoverOrderDetails;
                        TempData["Message"] = "Success";
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                    //ModelState.Clear();
                }
                else
                {
                    item.OverOrderId = Mymodel.SelectedItem.OverOrderId;
                    bool bret = unitWork.HandOverOrdersDetailsManager.Update(item);
                    // update record
                    if (bret)
                    {
                        TempData["Message"] = "Success";
                        ret = true;
                    }
                    else
                    {
                        TempData["Message"] = null;
                        ret = false;
                    }
                }
            }
            return ret;

        }
        [HttpPost]
        public ActionResult AddEditHandoverOrder(HandoverOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            bool ScanFlag = false;
            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];


            if (Mymodel.PostFlag == 3)
            {
                if (Mymodel.ReaderType == 2)
                {
                    if (!string.IsNullOrEmpty(Mymodel.Barcode))
                    {
                        long SelStoreId = Mymodel.SelectedItem.StoreId.GetValueOrDefault();
                        if (SelStoreId > 0)
                        {
                            tbl_ItemsStock existing = unitWork.ItemsStockManager.GetByBarcodeOutSide(Mymodel.Barcode, SelStoreId);
                            if (existing != null) // item barcode is existing
                            {
                                long ItemId = existing.Item_Id;
                                string ItemRFID = existing.Item_RFID;
                                long StoreId = existing.Room_Id.GetValueOrDefault();
                                long StockId = existing.StockId;
                                string Barcode = existing.Item_BarCode;
                                int InStoreQty = existing.ItemQty.GetValueOrDefault();
                                int EmpId = existing.Emp_Id.GetValueOrDefault(); 
                                int DeptId= existing.Depart_Id.GetValueOrDefault();
                                if (StoreId != Mymodel.SelectedItem.StoreId)
                                {
                                    List<HandOverOrdersDetails> RowsHandoverOrdersDetials = new List<HandOverOrdersDetails>();
                                    if (Mymodel.HandOverOrdersDetails != null)
                                    {
                                        RowsHandoverOrdersDetials = Mymodel.HandOverOrdersDetails.Where(c => c.StockId == StockId).ToList();
                                    }
                                    else
                                    {
                                        Mymodel.HandOverOrdersDetails = new List<HandOverOrdersDetails>();
                                    }
                                    if (RowsHandoverOrdersDetials.Count == 0)
                                    {
                                        HandOverOrdersDetails NewItem = new HandOverOrdersDetails();
                                        NewItem.ItemId = ItemId;
                                        NewItem.Item_RFID = ItemRFID;
                                        NewItem.StockId = StockId;
                                        if (EmpId > 0)
                                        {
                                            NewItem.EmpId = EmpId;
                                        }
                                        if (DeptId > 0)
                                        {
                                            NewItem.DeptId = DeptId;
                                        }
                                        NewItem.Qty = 1;
                                        NewItem.InStoreQty = InStoreQty;
                                        NewItem.StoreId = StoreId;
                                        Mymodel.HandOverOrdersDetails.Add(NewItem);
                                        Mymodel.HandOverOrdersDetails = unitWork.HandOverOrdersDetailsManager.GetAllDetails(Mymodel.HandOverOrdersDetails);
                                    }
                                    else
                                    {
                                        TempData["ScannerMessage"] = Resources.DefaultResource.DuplicateItemMsg;
                                    }
                                }
                                else
                                {
                                    TempData["ScannerMessage"] = Resources.DefaultResource.InvalidItemBarCode;
                                }
                            }
                            else
                            {
                                TempData["ScannerMessage"] = Resources.DefaultResource.ItemNotFound;
                            }
                        }
                        else
                        {
                            TempData["ScannerMessage"] = Resources.DefaultResource.ChooseInventoryMsg;
                        }
                    }
                }

            }
            else if (SaveValue == "CmdSave")
            {
                if (SaveHandoverOrder(Mymodel))
                {
                    return RedirectToAction("AddEditHandoverOrder", new { id = Mymodel.SelectedItem.OverOrderId });
                }
                else
                {
                    ScanFlag = true;
                    //return RedirectToAction("AddEditHandoverOrder", Mymodel);
                }
            }
            // if make scan item from scan device
            else if (ScanValue == "Scan" || Mymodel.DisplayMode == "StoreId")
            {

                ScanFlag = true;
            }
            else if (AddValue == "AddItem")
            {
                int i = 0;
                foreach (var item in Mymodel.ItemsScanCheckList)
                {
                    if (item.IsSelected)
                    {
                        long ItemId = Mymodel.ScanItems[i].Item_Id;
                        string ItemRFID = Mymodel.ScanItems[i].Item_RFID;
                        long StoreId = Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                        int? EmpId = Mymodel.ScanItems[i].Emp_Id;
                        int? DeptId = Mymodel.ScanItems[i].Depart_Id;
                        long StockId = Mymodel.ScanItems[i].StockId;
                        string BarCode = Mymodel.ScanItems[i].Item_BarCode;
                       
                        if (StoreId != Mymodel.SelectedItem.StoreId)
                        {
                                List<HandOverOrdersDetails> RowsHandoverOrdersDetials = new List<HandOverOrdersDetails>();
                                if (Mymodel.HandOverOrdersDetails != null)
                                {
                                    RowsHandoverOrdersDetials = Mymodel.HandOverOrdersDetails.Where(c => c.StockId == StockId).ToList();
                                }
                                else
                                {
                                    Mymodel.HandOverOrdersDetails = new List<HandOverOrdersDetails>();
                                }
                                if (RowsHandoverOrdersDetials.Count == 0)
                                {
                                    HandOverOrdersDetails NewItem = new HandOverOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.EmpId = EmpId;
                                    NewItem.DeptId = DeptId;
                                    NewItem.StoreId = StoreId;
                                    NewItem.StockId = StockId;
                                    NewItem.Qty = 1;
                                    //NewItem.Item_BarCode = BarCode;
                                    NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;

                                    Mymodel.HandOverOrdersDetails.Add(NewItem);


                                    
                                }
                            
                            ScanFlag = true;
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.CheckItemsInsideInventoryMsg;
                        }
                    }
                    i = ++i;
                    
                }

                Mymodel.HandOverOrdersDetails=unitWork.HandOverOrdersDetailsManager.GetAllDetails(Mymodel.HandOverOrdersDetails);

            }
            else if (DelValue == "CmdDel")
            {
                if (Id != null)
                {
                    int index = Id.GetValueOrDefault();
                    if (Mymodel.HandOverOrdersDetails != null)
                    {
                        long HandoverOrderDetId = Mymodel.HandOverOrdersDetails[index].OverOrderDetId;
                        long ItemId = Mymodel.HandOverOrdersDetails[index].ItemId.GetValueOrDefault();
                        string Item_RFID = Mymodel.HandOverOrdersDetails[index].Item_RFID;

                        HandOverOrdersDetails existing = unitWork.HandOverOrdersDetailsManager.GetById(HandoverOrderDetId);
                        if (existing != null)
                        {
                            unitWork.HandOverOrdersDetailsManager.Delete(existing);
                        }
                        Mymodel.HandOverOrdersDetails.RemoveAt(index);
                        ScanFlag = true;
                    }

                }

            }

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No,  ScanFlag));
        }
        public ActionResult GetData(HandoverOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, bool ScanFlag)
        {

            HandoverOrdersViewModel model = new HandoverOrdersViewModel();
        
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();

            if (Mymodel.ReaderType == 0) // action to do in form for assets
            {
                model.ReaderType = 1;
            }
            else
            {
                model.ReaderType = Mymodel.ReaderType;
            }
            model.SelectedItem = Mymodel.SelectedItem;
            model.Barcode = Mymodel.Barcode;
            int userId = SesssionUser.GetCurrentUserId();
            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            //Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
            model.Inventories = unitWork.RoomsManager.GetUserInventories(userId).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            model.HandOverReasons = unitWork.tbLookupsManager.GetByValue(1).ToList().Select(option => new SelectListItem
            {
                Text = option.LookupStringAr ,
                Value = option.LookupID.ToString()
            });
            model.Employees = unitWork.EmployeesManager.GetDelAll().ToList().Select(option => new SelectListItem
            {
                Text = option.FULL_NAME_AR,
                Value = option.Id.ToString()
            });
            
            //List<Item_tbl> Items = new List<Item_tbl>();
            //if (model.ReaderType == 2)
            //{
            //    Items = unitWork.ItemsManager.GetNotDelAllByType().ToList();
            //}
            //else
            //{
            //    Items = unitWork.ItemsManager.GetNotDelAll().ToList();
            //}

            //model.Items = Items.Select(option => new SelectListItem
            //{
            //    Text = option.Item_Name,
            //    Value = option.Item_Id.ToString()
            //});

            if (ScanFlag)
            {
                //check inventory
                if (model.Inventories.Count() > 0 && Mymodel.SelectedItem.StoreId == null)
                {
                    //model.SelectedItem.StoreId = long.Parse(model.Inventories.First().Value);
                }
                
                // if user choose inventory connecto to reader
                if (Mymodel.SelectedItem.StoreId != null)
                {
                    Room_tbl Roomobj = new Room_tbl();
                    Roomobj = unitWork.RoomsManager.GetById(Mymodel.SelectedItem.StoreId);
                    if (Roomobj != null)
                    {
                        if (Roomobj.ReaderId != null)
                        {
                            if (Mymodel.DisplayMode == "StoreId")
                            {
                                model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                            }
                        }
                        if (model.ReaderType == 1)
                        {
                            model.IPAddress = Roomobj.IPAddress;
                            model.TcpFlag = Roomobj.TcpFlag.GetValueOrDefault();
                            // scan from reader
                            List<tbl_ItemsStock> ScanItemsList = ScanReader(model.IPAddress, model.TcpFlag, Mymodel.SelectedItem.StoreId);
                            ScanItemsList.OrderBy(c => c.StockId);
                            model.ScanItems = ScanItemsList;//.ToPagedList(No_Of_Page, Size_Of_Page);
                            List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                            //int i = 0;
                            foreach (var item in ScanItemsList)
                            {
                                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                                bool IsSelected = false;
                                CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString(), ItemId = item.Item_Id };
                                ItemsCheckBoxList.Add(CheckItem);
                                //model.ScanItems[i].VmDepartments = item.VmDepartments;
                                //model.ScanItems[i].VmEmployees = item.VmEmployees;
                                //i++;
                            }
                            
                            model.ItemsScanCheckList = ItemsCheckBoxList;
                        }
                        else
                        {

                        }
                    }

                }
                else
                {
                    TempData["ScannerMessage"] = Resources.DefaultResource.ChooseInventoryMsg;
                }
            }
            else
            {


                model.ScanItems = Mymodel.ScanItems;
                int j = 0;
                if (Mymodel.ScanItems != null)
                {
                    foreach (var Scanitem in Mymodel.ScanItems)
                    {
                        if (Mymodel.ItemsScanCheckList[j].ItemId != 0)
                        {

                        }
                        Scanitem.Item_tbl = unitWork.ItemsManager.GetById(Scanitem.Item_Id);
                        Scanitem.Room_tbl = unitWork.RoomsManager.GetById(Scanitem.Room_Id);
                        j++;

                    }
                    model.ItemsScanCheckList = Mymodel.ItemsScanCheckList;

                    if (Mymodel.ReaderType == 2 && string.IsNullOrEmpty(Mymodel.Barcode))
                    {
                        model.ScanItems.Clear();
                        model.ItemsScanCheckList.Clear();
                    }
                }
            }
            foreach (var item in Mymodel.HandOverOrdersDetails)
            {
                item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                if (item.VmEmployees == null)
                {
                    item.VmEmployees = unitWork.EmployeesManager.GetEmployeeByEmpId(item.EmpId.GetValueOrDefault()).FirstOrDefault();
                }
                if (item.VmDepartments == null)
                { 
                    item.VmDepartments = unitWork.DepartmentManager.GetDeptById (item.DeptId.GetValueOrDefault()).FirstOrDefault();
                }
                item.Room_tbl = unitWork.RoomsManager.GetById(item.StoreId);
                //item.tbl_ItemsStock.Item_tbl = unitWork.ItemsManager.GetById(item.ItemId);
            }
            model.HandOverOrdersDetails = Mymodel.HandOverOrdersDetails;
            ModelState.Clear();
            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditHandoverOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid

        }

        private List<tbl_ItemsStock> ScanReader(string IpAddress, bool TcpFlag, long ? StoreId)
        {
            GlobalsCls globalClass = new GlobalsCls();
            //mReader = new clsReader();
            //mReaderInfo = mReader.ReaderSettings;
            string ReaderStatus = "";
            bool ret = globalClass.Connect(IpAddress, TcpFlag, ref ReaderStatus);
            if (!ret)
            {
                TempData["ScannerMessage"] = Resources.DefaultResource.UnSuccessConnect;
            }
            else
            {
                TempData["ScannerMessage"] = null;
            }
            List<tbl_ItemsStock> ItemsStockListAll;
            ItemsStockListAll = globalClass.ParsHandOverInOrderTages(StoreId);
           

            ItemsStockListAll = ItemsStockListAll.Where(p => p.Item_tbl.CountableFlag  == false ).ToList();
            globalClass.DisconnectReader();
            return ItemsStockListAll;
            //timer1.Start();
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            HandoverOrders existing = unitWork.HandoverOrdersManager.GetById(Id);
            if (existing != null)
            {
                unitWork.HandoverOrdersManager.Delete(existing);
                DeleteDetails(Id); // delete order items details

                HandoverOrdersViewModel model = new HandoverOrdersViewModel();
                model.HandoverOrders = unitWork.HandoverOrdersManager.GetNotDelAll().OrderByDescending(m => m.OverOrderId ).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedItem = null;
                model.DisplayMode = "";

                return RedirectToAction("HandoverOrderList");
            }
            else
            {
                return RedirectToAction("HandoverOrderList");
            }
        }
        private void DeleteDetails(int OrderId)
        {
            List<HandOverOrdersDetails> HandoverOrdersDetailsList = unitWork.HandOverOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in HandoverOrdersDetailsList)
            {
                HandOverOrdersDetails existing = unitWork.HandOverOrdersDetailsManager.GetById(item.OverOrderDetId);
                if (existing != null)
                {
                    unitWork.HandOverOrdersDetailsManager.Delete(existing);

                }
            }
        }

    }
}