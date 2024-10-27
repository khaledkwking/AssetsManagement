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
    public class ChangeQuantityOrdersController : Controller
    {
        // reader variables

        //-------------------------------end of reader variables---------------------------------------
        public int Qty = 0;
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }
        [UserPermissionAttribute(AllowFeature = "Change Orders", AllowPermission = "Accessing")]
        public ActionResult ChangeQuantityOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;
            ChangeQuantityOrdersViewModel model = new ChangeQuantityOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            List<ChangeQuantityOrders> ChangeQuantityOrderList = unitWork.ChangeQuantityOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending(m => m.ChangeOrderId).ToList();
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "ChangeOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "ChangeOrderId" ? "ChangeOrderDate" : "StoreId_From";

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
                ChangeQuantityOrderList = unitWork.ChangeQuantityOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "ChangeOrderId":
                    ChangeQuantityOrderList = ChangeQuantityOrderList.OrderByDescending(stu => stu.ChangeOrderId).ToList();
                    break;
                case "ChangeOrderDate":
                    ChangeQuantityOrderList = ChangeQuantityOrderList.OrderBy(stu => stu.ChangeOrderDate).ToList();
                    break;

                default:
                    ChangeQuantityOrderList = ChangeQuantityOrderList.OrderByDescending(stu => stu.ChangeOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.ChangeQuantityOrders = ChangeQuantityOrderList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (ChangeQuantityOrderList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditChangeQuantityOrder")]
        public ActionResult AddEditChangeQuantityOrder(string Id = null, int ScanFlag = 0)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            ChangeQuantityOrdersViewModel model = new ChangeQuantityOrdersViewModel();

            //model.ItemsPopulateList(model.FromEmpId, null,1);


            if (String.IsNullOrEmpty(Id))
            {
                if (this.HasPermission("Change Orders", "Adding"))
                {
                    ModelState.Clear();
                    ChangeQuantityOrders UnitRecord = new ChangeQuantityOrders();
                    UnitRecord.ChangeOrderDate = DateTime.Today;
                    model.SelectedItem = UnitRecord;
                  
                    model.EditMode = "Add";
                    model.DisplayMode = "ReadOnly";
                    List<ChangeQuantityOrdersDetails> ChangeQuantityOrdersDetailsList = new List<ChangeQuantityOrdersDetails>();
                    model.ChangeQuantityOrdersDetails = ChangeQuantityOrdersDetailsList;
                    //model.SelectedItem.TransferFlag = 1; // personal ChangeQuantityOrder
                    //return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");

                }
            }
            else
            {
                if (this.HasPermission("Change Orders", "Updating"))
                {
                    // Edit record (view in Edit mode)
                    int OrderId = int.Parse(Id);

                    //model.Buildings = unitWork.ChangeQuantityOrdersManager.GetNotDelAll().OrderBy(m => m.ChangeOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                    model.SelectedItem = unitWork.ChangeQuantityOrdersManager.GetById(OrderId);
                    model.DisplayMode = "ReadWrite";
                    model.EditMode = "Edit";
                    List<ChangeQuantityOrdersDetails> ChangeQuantityOrdersDetailsList = unitWork.ChangeQuantityOrdersDetailsManager.GetByOrderId(OrderId);
                    model.ChangeQuantityOrdersDetails = ChangeQuantityOrdersDetailsList;
                    if (model.SelectedItem == null) { return View("_error"); }
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }

            }

            return GetData(model, "", "", "", 0, ScanFlag);

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(ChangeQuantityOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, 0));
        }
        private bool SaveChangeQuantityOrder(ChangeQuantityOrdersViewModel Mymodel)
        {
            bool ret = true;
          
            string ItemName = "";
            try
            {
                if (CheckEnoughQty(Mymodel, ref ItemName))
                {
                    if (Mymodel.ChangeQuantityOrdersDetails.Count == 0)
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                        TempData["Message"] = null;
                    }
                    if (ret)
                    {
                        //if (ModelState.IsValid)
                        //{
                        if (CheckDetailsBeforeSave(Mymodel.ChangeQuantityOrdersDetails))
                        {
                            if (Mymodel.SelectedItem.ChangeOrderId == 0)
                            {
                                // insert new record
                                ChangeQuantityOrders ChangeQuantityOrdersbj = unitWork.ChangeQuantityOrdersManager.Add(Mymodel.SelectedItem);
                                if (ChangeQuantityOrdersbj != null)
                                {
                                    Mymodel.ChangeQuantityOrdersDetails = Mymodel.ChangeQuantityOrdersDetails;
                                    TempData["Message"] = "Success";
                                }
                                else
                                {
                                    TempData["Message"] = null;
                                }
                                //ModelState.Clear();
                            }
                            else
                            {
                                bool bret = unitWork.ChangeQuantityOrdersManager.Update(Mymodel.SelectedItem);
                                // update record
                            }

                            if (SaveOrderItems(Mymodel))
                            {
                                TempData["Message"] = "Success";
                            }
                            else
                            {
                                TempData["Message"] = null;
                            }
                            TempData["warningMessage"] = null;
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.DeleteItemQtyMsg;
                            ret = false;
                        }

  
                    }
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            //}
            //else
            //{
            //    TempData["warningMessage"] = Resources.DefaultResource.InSufficientQtyMsg;
            //    ret = false;

            //}
            return ret;
        }
        // check enough quantity to In
        private bool CheckEnoughQty(ChangeQuantityOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.ChangeQuantityOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                ChangeQuantityOrdersDetails ChangeQuantityOrdersDetailsObj = unWork.ChangeQuantityOrdersDetailsManager.GetById(item.ChangeOrderDetId);
                if (ChangeQuantityOrdersDetailsObj != null)
                {
                    OldQty = ChangeQuantityOrdersDetailsObj.Qty.GetValueOrDefault();
                }

                tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                if (ItemObj.CatSub_tbl.GenerateBarcodeFlag)
                {
                    if (ItemObj.CatSub_tbl.GenerateBarcodeFlag) // get current Item Quantity in inventory
                    {
                        List<tbl_ItemsStockDetails> ItemsStockDetailsList = unWork.ItemsStockDetailsManager.GetByStockDetId(item.StockDetId.GetValueOrDefault());
                        if (ItemsStockDetailsList.Count > 0)
                        {
                            TotalQty = ItemsStockDetailsList.FirstOrDefault().Qty.GetValueOrDefault();
                        }
                    }
                }
                else // get current Quantity in inventory
                {
                    TotalQty = item.tbl_ItemsStock.ItemQty.GetValueOrDefault(); //item.InStoreQty.GetValueOrDefault();
                }
                // get value of quantity in current inorder 
                int DiffQty = TotalQty+ OldQty + item.Qty.GetValueOrDefault();  // نتاكد ان الكمية الحالية لا تصبح سالب
                if (DiffQty < 0)
                {
                    ret = false;
                    ItemName = StockObj.ItemName;
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteItemQtyMsg +" "+ItemName;
                    break;
                }
            }
            return ret;
        }
        private bool SaveOrderItems(ChangeQuantityOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.ChangeQuantityOrdersDetails)
            {
                //if (item.StockId == 0) // save stock item if new
                //{
                //    tbl_ItemsStock StockObj = new tbl_ItemsStock();
                //    StockObj.Item_Id = item.ItemId.GetValueOrDefault();
                //    StockObj.Item_RFID = item.Item_RFID;
                //    Item_tbl obj = unitWork.ItemsManager.GetById(item.ItemId);
                //    if (obj != null)
                //    {
                //        //bool CountableFlag = obj.CountableFlag;
                //        //if (!CountableFlag)
                //        //{
                //        //    StockObj.ItemQty = 1;
                //        //    StockObj.Item_Serial = item.Item_BarCode;
                //        //}
                //        //else
                //        //{
                //        //    StockObj.Item_BarCode = item.Item_BarCode;
                //        //}
                //        StockObj.Room_Id = Mymodel.SelectedItem.StoreId_From;

                //        tbl_ItemsStock ItemStockObj = unitWork.ItemsStockManager.Add(StockObj);
                //        item.StockId = ItemStockObj.StockId;
                //    }
                //}
                item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                if (item.ChangeOrderDetId == 0)
                {
                    // insert new record

                    item.ChangeOrderId = Mymodel.SelectedItem.ChangeOrderId;
                    ChangeQuantityOrdersDetails ChangeQuantityOrderItemsbj = unitWork.ChangeQuantityOrdersDetailsManager.Add(item);
                    if (ChangeQuantityOrderItemsbj != null)
                    {
                        //Mymodel.ChangeQuantityOrdersDetails = Mymodel.ChangeQuantityOrdersDetails;
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
                    item.ChangeOrderId = Mymodel.SelectedItem.ChangeOrderId;
                    bool bret = unitWork.ChangeQuantityOrdersDetailsManager.Update(item);
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
        public ActionResult AddEditChangeQuantityOrder(ChangeQuantityOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            int ScanFlag = 0;
            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];


            if (Mymodel.PostFlag == 3)
            {
                if (Mymodel.ReaderType == 2) // select barcode choice
                {
                    if (!string.IsNullOrEmpty(Mymodel.Barcode))
                    {
                        long SelStoreId = Mymodel.SelectedItem.StoreId_From.GetValueOrDefault();

                        long ItemId = 0; ;
                        string ItemRFID = "";
                        long StoreId = 0;
                        long StockId = 0;
                        string Barcode = "";
                        int InStoreQty = 0;
                        decimal CostPrice = 0; ;
                        long StockDetId = 0;
                        int InStoreItemQty = 0;
                        bool success = false;

                        tbl_ItemsStockDetails existing = unitWork.ItemsStockDetailsManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
                        if (existing != null)
                        {
                            ItemId = existing.tbl_ItemsStock.Item_Id;
                            ItemRFID = existing.tbl_ItemsStock.Item_RFID;
                            StoreId = existing.tbl_ItemsStock.Room_Id.GetValueOrDefault();
                            StockId = existing.StockId.GetValueOrDefault();
                            Barcode = existing.BarCode;
                            InStoreQty = existing.tbl_ItemsStock.ItemQty.GetValueOrDefault();
                            CostPrice = existing.CostPrice.GetValueOrDefault();
                            StockDetId = existing.StockDetId;
                            InStoreItemQty = existing.Qty.GetValueOrDefault();
                            success = true;
                        }
                        else // when iten is not has detail of cost price
                        {
                            tbl_ItemsStock existingStock = unitWork.ItemsStockManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
                            //if (Mymodel.ScanItems == null) { Mymodel.ScanItems = new List<tbl_ItemsStock>(); }
                            if (existingStock != null) // item barcode is existing
                            {
                                ItemId = existingStock.Item_Id;
                                ItemRFID = existingStock.Item_RFID;
                                StoreId = existingStock.Room_Id.GetValueOrDefault();
                                StockId = existingStock.StockId;
                                Barcode = existingStock.Item_BarCode;
                                InStoreQty = existingStock.ItemQty.GetValueOrDefault();
                                success = true;
                            }
                            else
                            {
                                success = false;
                            }
                        }
                        if (success) // item barcode is existing
                        {
                            if (StoreId == Mymodel.SelectedItem.StoreId_From)
                            {
                                List<ChangeQuantityOrdersDetails> RowsChangeQuantityOrdersDetials = new List<ChangeQuantityOrdersDetails>();
                                if (Mymodel.ChangeQuantityOrdersDetails != null)
                                {
                                    RowsChangeQuantityOrdersDetials = Mymodel.ChangeQuantityOrdersDetails.Where(c => c.StockId == StockId && c.StockDetId == StockDetId).ToList();
                                }
                                else
                                {
                                    Mymodel.ChangeQuantityOrdersDetails = new List<ChangeQuantityOrdersDetails>();
                                }
                                if (RowsChangeQuantityOrdersDetials.Count == 0)
                                {
                                    ChangeQuantityOrdersDetails NewItem = new ChangeQuantityOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Item_BarCode = Barcode;
                                    NewItem.Qty = 1;
                                    NewItem.StoreId = StoreId;
                                    NewItem.InStoreQty = InStoreQty;
                                   
                                    NewItem.StockDetId = StockDetId;
                                    NewItem.InStoreItemQty = InStoreItemQty;

                                    Mymodel.ChangeQuantityOrdersDetails.Add(NewItem);
                                    TempData["ScannerMessage"] = null;
                                    
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
                    }
                    else
                    {
                        TempData["ScannerMessage"] = Resources.DefaultResource.InvalidItemBarCode;
                    }
                }
                //List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                //foreach (var item in Mymodel.ScanItems)
                //{
                //    //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                //    bool IsSelected = false;
                //    CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString(), ItemId = item.Item_Id };
                //    ItemsCheckBoxList.Add(CheckItem);
                //}
                //Mymodel.ItemsScanCheckList = ItemsCheckBoxList;
            }
                         
            else if (Mymodel.PostFlag == 6 | Mymodel.PostFlag == 5)
            {
                ScanFlag = 1;
            }
            else if (SaveValue == "CmdSave")
            {

                if (SaveChangeQuantityOrder(Mymodel))
                {
                    return RedirectToAction("AddEditChangeQuantityOrder", new { id = Mymodel.SelectedItem.ChangeOrderId, ScanFlag = 0 });
                }
                else
                {
                    //return RedirectToAction("AddEditChangeQuantityOrder", Mymodel);
                }
                //return RedirectToAction("AddEditChangeQuantityOrder", Mymodel);

            }
            // if make scan item from scan device
            else if (ScanValue == "Scan" || Mymodel.DisplayMode == "SelectedItem_StoreId_From")
            {

                ScanFlag = 1;
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
                        long StockId = Mymodel.ScanItems[i].StockId;
                        string BarCode = Mymodel.ScanItems[i].Item_BarCode;
                        if (StoreId == Mymodel.SelectedItem.StoreId_From)
                        {
                            List<ChangeQuantityOrdersDetails> RowsChangeQuantityOrdersDetails = new List<ChangeQuantityOrdersDetails>();
                            if (Mymodel.ChangeQuantityOrdersDetails != null)
                            {
                                RowsChangeQuantityOrdersDetails = Mymodel.ChangeQuantityOrdersDetails.Where(c => c.StockId == StockId).ToList();
                            }
                            else
                            {
                                Mymodel.ChangeQuantityOrdersDetails = new List<ChangeQuantityOrdersDetails>();
                            }
                            if (RowsChangeQuantityOrdersDetails.Count == 0)
                            {
                                if (Mymodel.ScanItems[i].ItemQty > 0)
                                {
                                    ChangeQuantityOrdersDetails NewItem = new ChangeQuantityOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Qty = 1;
                                    //NewItem.Item_BarCode = BarCode;
                                    NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;
                                    NewItem.StoreId = StoreId;
                                    Mymodel.ChangeQuantityOrdersDetails.Add(NewItem);
                                }
                                else
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.QtyLimitMsg;
                                }
                            }
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.CheckItemsInventoryMsg;
                        }
                    }
                    i = ++i;
                }


            }
            else if (DelValue == "CmdDel")
            {
                if (Id != null)
                {
                    int index = Id.GetValueOrDefault();
                    if (Mymodel.ChangeQuantityOrdersDetails != null)
                    {
                        long ChangeOrderDetId = Mymodel.ChangeQuantityOrdersDetails[index].ChangeOrderDetId;
                        long ItemId = Mymodel.ChangeQuantityOrdersDetails[index].ItemId.GetValueOrDefault();
                        string Item_RFID = Mymodel.ChangeQuantityOrdersDetails[index].Item_RFID;

                        Boolean DelFalg = true;
                        ChangeQuantityOrdersDetails existing = unitWork.ChangeQuantityOrdersDetailsManager.GetById(ChangeOrderDetId);

                        if (existing != null)
                        {
                            if (CheckItemDelete(existing.ChangeOrderDetId))
                            {
                                unitWork.ChangeQuantityOrdersDetailsManager.Delete(existing);
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.DeleteItemQtyMsg;
                                DelFalg = false;
                            }
                        
                        }
                        if (DelFalg)
                        {
                            Mymodel.ChangeQuantityOrdersDetails.RemoveAt(index);
                        }

                    }
                    ScanFlag = 1;
                }

            }
            //else if (Mymodel.ReaderType == 1)
            //{
            //    ScanFlag = 1;
            //}
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, ScanFlag));
        }

        public ActionResult GetData(ChangeQuantityOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, int ScanFlag)
        {

            ChangeQuantityOrdersViewModel model = new ChangeQuantityOrdersViewModel();
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();

            //if (Mymodel.SelectedItem.TransferFlag == 0) // action to do in form for assets
            //{
            //    Mymodel.SelectedItem.TransferFlag = 1;
            //}

            if (Mymodel.ReaderType == 0) // action to do in form for assets
            {
                model.ReaderType = 1;
            }
            else
            {
                model.ReaderType = Mymodel.ReaderType;
            }

            model.SelectedItem = Mymodel.SelectedItem;
            model.FromStoreId = Mymodel.SelectedItem.StoreId_From;
            model.Barcode = Mymodel.Barcode;
            model.EditMode = Mymodel.EditMode;

            int userId = SesssionUser.GetCurrentUserId();
            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            //Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
            model.FromStores = unitWork.RoomsManager.GetUserInventories(userId).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            //model.ToStores = unitWork.RoomsManager.GetInventoriesAll().ToList().Select(option => new SelectListItem
            //{
            //    Text = option.Room_Name,
            //    Value = option.Room_Id.ToString()
            //});

            //model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().ToList().Select(option => new SelectListItem
            //{
            //    Text = option.Sup_Name,
            //    Value = option.Sup_code.ToString()
            //});



            List<Item_tbl> Items = new List<Item_tbl>();
            if (model.ReaderType == 2)
            {
                Items = unitWork.ItemsManager.GetNotDelAllByType().ToList();
            }
            else
            {
                Items = unitWork.ItemsManager.GetNotDelAll().ToList();
            }
            model.Items = Items.Select(option => new SelectListItem
            {
                Text = option.Item_Name,
                Value = option.Item_Id.ToString()
            });

            if (ScanFlag==1)
            {

                // if user choose inventory connecto to reader
                if (Mymodel.SelectedItem.StoreId_From != null)
                {
                    long? StoreFromId = Mymodel.SelectedItem.StoreId_From;
                    Room_tbl Roomobj = new Room_tbl();
                    Roomobj = unitWork.RoomsManager.GetById(StoreFromId);
                    if (Roomobj != null)
                    {
                        if (Roomobj.ReaderId != null)
                        {
                            if (Mymodel.DisplayMode == "SelectedItem_StoreId_From")
                            {
                                model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                            }
                        }
                        if (model.ReaderType == 1)
                        {
                            model.IPAddress = Roomobj.IPAddress;
                            model.TcpFlag = Roomobj.TcpFlag.GetValueOrDefault();
                            // scan from reader
                            List<tbl_ItemsStock> ScanItemsList = ScanReader(model.IPAddress, model.TcpFlag, StoreFromId);
                            ScanItemsList= ScanItemsList.Where(p => p.Item_tbl.CountableFlag == true).ToList(); // filter only countable Scaned items
                          
                            ScanItemsList.OrderBy(c => c.StockId);
                            model.ScanItems = ScanItemsList;//.ToPagedList(No_Of_Page, Size_Of_Page);
                            List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                            foreach (var item in ScanItemsList)
                            {
                                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                                bool IsSelected = false;
                                CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString(), ItemId = item.Item_Id };
                                ItemsCheckBoxList.Add(CheckItem);
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

            foreach (var item in Mymodel.ChangeQuantityOrdersDetails)
            {
                item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                //item.tbl_ItemsStock.Item_tbl= unitWork.ItemsManager.GetById(item.ItemId);
            }
            model.ChangeQuantityOrdersDetails = Mymodel.ChangeQuantityOrdersDetails;
            ModelState.Clear();

            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditChangeQuantityOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid

        }

        private List<tbl_ItemsStock> ScanReader(string IpAddress, bool TcpFlag, long? StoreId)
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
            ItemsStockListAll = globalClass.ParsTages(StoreId);

            globalClass.DisconnectReader();
            return ItemsStockListAll;
            //timer1.Start();
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            ChangeQuantityOrders existing = unitWork.ChangeQuantityOrdersManager.GetById(Id);
            if (existing != null)
            {
                if (CheckDetailsBeforeDelete(existing.ChangeOrderId))
                {
                    unitWork.ChangeQuantityOrdersManager.Delete(existing);
                    DeleteDetails(Id); // delete order items

                    ChangeQuantityOrdersViewModel model = new ChangeQuantityOrdersViewModel();
                    model.ChangeQuantityOrders = unitWork.ChangeQuantityOrdersManager.GetNotDelAll().OrderByDescending(m => m.ChangeOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                    model.SelectedItem = null;
                    model.DisplayMode = "";
                    TempData["warningMessage"] = null;
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteItemQtyMsg;
                }
                return RedirectToAction("ChangeQuantityOrderList");
            }
            else
            {
                return RedirectToAction("ChangeQuantityOrderList");
            }
        }
        private void DeleteDetails(int OrderId)
        {
            List<ChangeQuantityOrdersDetails> ChangeQuantityOrdersDetailsList = unitWork.ChangeQuantityOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in ChangeQuantityOrdersDetailsList)
            {
                ChangeQuantityOrdersDetails existing = unitWork.ChangeQuantityOrdersDetailsManager.GetById(item.ChangeOrderDetId);
                if (existing != null)
                {
                    unitWork.ChangeQuantityOrdersDetailsManager.Delete(existing);

                }
            }
        }




        [HttpGet]
        [ActionName("AddEditTestOrder")]
        public ActionResult AddEditTestOrder(string Id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            ChangeQuantityOrdersViewModel model = new ChangeQuantityOrdersViewModel();

            //model.ItemsPopulateList(model.FromEmpId, null,1);


            if (String.IsNullOrEmpty(Id))
            {
                ModelState.Clear();
                ChangeQuantityOrders UnitRecord = new ChangeQuantityOrders();
                UnitRecord.ChangeOrderDate = DateTime.Today;
                model.SelectedItem = UnitRecord;
                model.DisplayMode = "ReadOnly";
                List<ChangeQuantityOrdersDetails> ChangeQuantityOrdersDetailsList = new List<ChangeQuantityOrdersDetails>();
                model.ChangeQuantityOrdersDetails = ChangeQuantityOrdersDetailsList;
                //model.SelectedItem.TransferFlag = 1; // personal ChangeQuantityOrder
                //return View(model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int OrderId = int.Parse(Id);

                //model.Buildings = unitWork.ChangeQuantityOrdersManager.GetNotDelAll().OrderBy(m => m.ChangeOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.ChangeQuantityOrdersManager.GetById(OrderId);
                model.DisplayMode = "ReadWrite";
                List<ChangeQuantityOrdersDetails> ChangeQuantityOrdersDetailsList = unitWork.ChangeQuantityOrdersDetailsManager.GetByOrderId(OrderId);
                model.ChangeQuantityOrdersDetails = ChangeQuantityOrdersDetailsList;
                if (model.SelectedItem == null) { return View("_error"); }
                // ...
                //return View( model);
            }

            return GetData(model, "", "", "", 0, 1);

        }

        private Boolean CheckItemDelete(long ChangeOrderDetId)
        {
            Boolean ret = true;
            ChangeQuantityOrdersDetails existing = unitWork.ChangeQuantityOrdersDetailsManager.GetById(ChangeOrderDetId);
            if (existing != null)
            {
                int Difference = existing.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;

                }

            }
          
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(long OrderId)
        {
            List<ChangeQuantityOrdersDetails> ChangeQuantityDetailsList = unitWork.ChangeQuantityOrdersDetailsManager.GetByOrderId(OrderId);
            Boolean ret = true;
            foreach (var item in ChangeQuantityDetailsList)
            {
                ret = CheckItemDelete(item.ChangeOrderDetId);
                if (!ret)
                {
                    ret = false;
                    break;
                }

            }
            return ret;
        }

        private Boolean CheckDetailsBeforeSave(List<ChangeQuantityOrdersDetails> ChangeQuantityDetailsList)
        {
            Boolean ret = true;
            foreach (var item in ChangeQuantityDetailsList)
            {
                int Difference = item.InStoreQty.GetValueOrDefault() + item.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;
                    break;
                }
              
            }
            return ret;
        }
    }

}