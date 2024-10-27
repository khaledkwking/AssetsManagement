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
    public class DestroyOrdersController : Controller
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

        [UserPermissionAttribute(AllowFeature = "Destroy Orders", AllowPermission = "Accessing")]
        public ActionResult DestroyOrdersList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            DestroyOrdersViewModel model = new DestroyOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            List<DestroyOrders> DestroyOrdersList = unitWork.DestroyOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending(m => m.DestroyOrderId).ToList();
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "DestroyOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "DestroyOrderId" ? "DestroyOrderDate" : "StoreId_From";

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
                DestroyOrdersList = unitWork.DestroyOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "DestroyOrderId":
                    DestroyOrdersList = DestroyOrdersList.OrderByDescending(stu => stu.DestroyOrderId).ToList();
                    break;
                case "DestroyOrderDate":
                    DestroyOrdersList = DestroyOrdersList.OrderBy(stu => stu.DestroyOrderDate).ToList();
                    break;
               
                default:
                    DestroyOrdersList = DestroyOrdersList.OrderByDescending(stu => stu.DestroyOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.DestroyOrders = DestroyOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (DestroyOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditDestroyOrder")]
        public ActionResult AddEditDestroyOrder(string Id = null, int ScanFlag = 0)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            DestroyOrdersViewModel model = new DestroyOrdersViewModel();
            try
            {
                //model.ItemsPopulateList(model.FromEmpId, null,1);


                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("Destroy Orders", "Adding"))
                    {
                        ModelState.Clear();
                        DestroyOrders UnitRecord = new DestroyOrders();
                        UnitRecord.DestroyOrderDate = DateTime.Today;
                        model.SelectedItem = UnitRecord;
                        model.SelectedItem.Reason = "تالف";
                        model.EditMode = "Add";
                        model.DisplayMode = "ReadOnly";
                        List<DestroyOrdersDetails> DestroyOrdersDetailsList = new List<DestroyOrdersDetails>();
                        model.DestroyOrdersDetails = DestroyOrdersDetailsList;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");

                    }
                    //model.SelectedItem.TransferFlag = 1; // personal DestroyOrder
                    //return View(model);
                }
                else
                {
                    // Edit record (view in Edit mode)
                    if (this.HasPermission("Destroy Orders", "Updating"))
                    {
                        int OrderId = int.Parse(Id);

                        //model.Buildings = unitWork.DestroyOrdersManager.GetNotDelAll().OrderBy(m => m.DestroyOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.DestroyOrdersManager.GetById(OrderId);
                        model.DisplayMode = "ReadWrite";
                        model.EditMode = "Edit";
                        List<DestroyOrdersDetails> DestroyOrdersDetailsList = unitWork.DestroyOrdersDetailsManager.GetByOrderId(OrderId);
                        model.DestroyOrdersDetails = DestroyOrdersDetailsList;
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
            return GetData(model, "", "", "", 0, ScanFlag);

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(DestroyOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, 0));
        }
        private bool SaveDestroyOrder(DestroyOrdersViewModel Mymodel)
        {
            bool ret = true;
            string ItemName = "";
            try
            {
                //if (!CheckEnoughQty(Mymodel))
                //{
                //checked user select invetory
                //if (Mymodel.SelectedItem.StoreId_To != null)
                //{
                //    long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
                //}
                //else
                //{
                //    ret = false; // end flag
                //}
                if (Mymodel.DestroyOrdersDetails.Count == 0)
                {
                    ret = false;
                    TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                    TempData["Message"] = null;
                }
                if (!CheckEnoughQty(Mymodel, ref ItemName)) // check Qty
                {
                    TempData["warningMessage"] = Resources.DefaultResource.InSufficientQtyMsg + " " + ItemName;
                    ret = false;
                }
                if (ret)
                {
                    //if (ModelState.IsValid)
                    //{
                    if (Mymodel.SelectedItem.DestroyOrderId == 0)
                    {
                        // insert new record
                        DestroyOrders DestroyOrdersbj = unitWork.DestroyOrdersManager.Add(Mymodel.SelectedItem);
                        if (DestroyOrdersbj != null)
                        {
                            Mymodel.DestroyOrdersDetails = Mymodel.DestroyOrdersDetails;
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
                        bool bret = unitWork.DestroyOrdersManager.Update(Mymodel.SelectedItem);
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
                    //}
                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            
            return ret;
        }
        // check enough quantity to In

        //private bool CheckEnoughQty(DestroyOrdersViewModel Mymodel, ref string ItemName)
        //{
        //    bool ret = true;
        //    ItemName = "";
        //    foreach (var item in Mymodel.DestroyOrdersDetails)
        //    {
        //        Item_tbl ItemObj = unitWork.ItemsManager.GetById(item.ItemId);
        //        int TotalQty = 0;
        //        if (ItemObj.CatSub_tbl.GenerateBarcodeFlag)
        //        {
        //            TotalQty = item.InStoreItemQty.GetValueOrDefault();
        //        }
        //        else
        //        {
        //            TotalQty = item.InStoreQty.GetValueOrDefault();
        //        }


        //        int DiffQty = TotalQty - item.Qty.GetValueOrDefault();
        //        if (DiffQty < 0)
        //        {
        //            ItemName = item.ItemId.ToString();
        //            ret = false;
        //            break;
        //        }
        //    }
        //    return ret;
        //}
        private bool CheckEnoughQty(DestroyOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.DestroyOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                DestroyOrdersDetails DestroyOrdersDetailsObj = unWork.DestroyOrdersDetailsManager.GetById(item.DestroyOrderDetId);
                if (DestroyOrdersDetailsObj != null)
                {
                    OldQty = DestroyOrdersDetailsObj.Qty.GetValueOrDefault();
                }

                tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                if (ItemObj.CatSub_tbl.GenerateBarcodeFlag && item.StockDetId != 0)
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
                else // get current Quantity in item details inventory
                {
                    TotalQty = StockObj.ItemQty.GetValueOrDefault(); //item.tbl_ItemsStock.ItemQty.GetValueOrDefault(); //item.InStoreQty.GetValueOrDefault();
                }
                // get value of quantity in current inorder 
                int DiffQty = TotalQty - (item.Qty.GetValueOrDefault() - OldQty);  // نتاكد ان الكمية الحالية لا تصبح سالب
                if (DiffQty < 0)
                {
                    ret = false;
                    ItemName = StockObj.ItemName;
                    break;
                }
            }
            return ret;
        }
        private bool SaveOrderItems(DestroyOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.DestroyOrdersDetails)
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
               
                if (!item.tbl_ItemsStock.Item_tbl.CountableFlag)
                {
                    item.Qty = 1;
                }
                if (item.DestroyOrderDetId == 0)
                {
                    // insert new record
                    
                    item.DestroyOrderId = Mymodel.SelectedItem.DestroyOrderId;
                    DestroyOrdersDetails DestroyOrderItemsbj = unitWork.DestroyOrdersDetailsManager.Add(item);
                    if (DestroyOrderItemsbj != null)
                    {
                        //Mymodel.DestroyOrdersDetails = Mymodel.DestroyOrdersDetails;
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
                    item.DestroyOrderId = Mymodel.SelectedItem.DestroyOrderId;
                    bool bret = unitWork.DestroyOrdersDetailsManager.Update(item);
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
        public ActionResult AddEditDestroyOrder(DestroyOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //bool ScanFlag = false;
            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];
            string cmdsubmit = Request.Form["CmdSelStore"];
            int ScanFlag = 0;

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
                        decimal CostPrice = 0; 
                        long StockDetId = 0;
                        int InStoreItemQty = 0;
                        bool success = false;
                        
                        //if (Mymodel.ScanItems == null) { Mymodel.ScanItems = new List<tbl_ItemsStock>(); }
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
                        else
                        //if (existing != null) // item barcode is existing
                        {
                            tbl_ItemsStock existingStock = unitWork.ItemsStockManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
                            if (existingStock != null)
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
                        if (success)
                        { 
                            if (StoreId == Mymodel.SelectedItem.StoreId_From)
                            {
                                List<DestroyOrdersDetails> RowsDestroyOrdersDetials = new List<DestroyOrdersDetails>();
                                if (Mymodel.DestroyOrdersDetails != null)
                                {
                                    RowsDestroyOrdersDetials = Mymodel.DestroyOrdersDetails.Where(c => c.StockId == StockId && c.StockDetId == StockDetId).ToList();
                                }
                                else
                                {
                                    Mymodel.DestroyOrdersDetails = new List<DestroyOrdersDetails>();
                                }
                                if (RowsDestroyOrdersDetials.Count == 0)
                                {
                                    DestroyOrdersDetails NewItem = new DestroyOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Item_BarCode = Barcode;
                                    NewItem.Qty = 1;
                                    NewItem.StockDetId = StockDetId;
                                    NewItem.StoreId = StoreId;
                                    NewItem.InStoreQty = InStoreQty;
                                    NewItem.InStoreItemQty = InStoreItemQty;
                                    Mymodel.DestroyOrdersDetails.Add(NewItem);

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
                            TempData["ScannerMessage"] = Resources.DefaultResource.InvalidItemBarCode;
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
                }

            }
            else if (Mymodel.PostFlag == 6 || Mymodel.PostFlag == 5)
            {
                ScanFlag = 1;
            }
            else if (SaveValue == "CmdSave")
            {

                if (SaveDestroyOrder(Mymodel))
                {
                    return RedirectToAction("AddEditDestroyOrder", new { id = Mymodel.SelectedItem.DestroyOrderId, ScanFlag = 0 });
                }
                else
                {
                    //return RedirectToAction("AddEditDestroyOrder", new { id = Mymodel.SelectedItem.DestroyOrderId });
                }
                //return RedirectToAction("AddEditDestroyOrder", Mymodel);

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
                            List<DestroyOrdersDetails> RowsDestroyOrdersDetails = new List<DestroyOrdersDetails>();
                            if (Mymodel.DestroyOrdersDetails != null)
                            {
                                RowsDestroyOrdersDetails = Mymodel.DestroyOrdersDetails.Where(c => c.StockId == StockId).ToList();
                            }
                            else
                            {
                                Mymodel.DestroyOrdersDetails = new List<DestroyOrdersDetails>();
                            }
                            if (RowsDestroyOrdersDetails.Count == 0)
                            {
                                if (Mymodel.ScanItems[i].ItemQty > 0)
                                {
                                    DestroyOrdersDetails NewItem = new DestroyOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Qty = 1;
                                    //NewItem.Item_BarCode = BarCode;
                                    NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;
                                    NewItem.StoreId = StoreId;
                                    Mymodel.DestroyOrdersDetails.Add(NewItem);
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
                    if (Mymodel.DestroyOrdersDetails != null)
                    {
                        long DestroyOrderDetId = Mymodel.DestroyOrdersDetails[index].DestroyOrderDetId;
                        long ItemId = Mymodel.DestroyOrdersDetails[index].ItemId.GetValueOrDefault();
                        string Item_RFID = Mymodel.DestroyOrdersDetails[index].Item_RFID;

                        DestroyOrdersDetails existing = unitWork.DestroyOrdersDetailsManager.GetById(DestroyOrderDetId);
                        if (existing != null)
                        {
                            unitWork.DestroyOrdersDetailsManager.Delete(existing);
                        }
                        Mymodel.DestroyOrdersDetails.RemoveAt(index);

                    }
                    //ScanFlag = 1;
                }

            }
            else if (Mymodel.ReaderType == 1)
            {
                ScanFlag = 1;
            }
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, ScanFlag));
        }

        public ActionResult GetData(DestroyOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, int ScanFlag)
        {

            DestroyOrdersViewModel model = new DestroyOrdersViewModel();
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

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            //Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
            int userId = SesssionUser.GetCurrentUserId();
            model.FromStores = unitWork.RoomsManager.GetUserInventories(userId).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            model.ToStores = unitWork.RoomsManager.GetInventoriesAll().ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().ToList().Select(option => new SelectListItem
            {
                Text = option.Sup_Name,
                Value = option.Sup_code.ToString()
            });
            
         

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

                //check inventory
                //if (model.ToStores.Count() > 0 && Mymodel.SelectedItem.StoreId_From == null)
                //{
                //    model.SelectedItem.StoreId_From = int.Parse(model.ToStores.First().Value);
                //}
                //if (model.Suppliers.Count() > 0 && Mymodel.SelectedItem.SupplierId_From == null)
                //{
                //    model.SelectedItem.SupplierId_From = int.Parse(model.Suppliers.First().Value);
                //}
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
           
            foreach (var item in Mymodel.DestroyOrdersDetails)
            {
                item.tbl_ItemsStock  = unitWork.ItemsStockManager.GetById(item.StockId);
                //item.tbl_ItemsStock.Item_tbl= unitWork.ItemsManager.GetById(item.ItemId);
            }
            model.DestroyOrdersDetails = Mymodel.DestroyOrdersDetails;
            ModelState.Clear();

            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditDestroyOrder", "Orders", Mymodel);
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
            DestroyOrders existing = unitWork.DestroyOrdersManager.GetById(Id);
            if (existing != null)
            {
                unitWork.DestroyOrdersManager.Delete(existing);
                DeleteDetails(Id); // delete order items

                DestroyOrdersViewModel model = new DestroyOrdersViewModel();
                model.DestroyOrders = unitWork.DestroyOrdersManager.GetNotDelAll().OrderByDescending(m => m.DestroyOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedItem = null;
                model.DisplayMode = "";

                return RedirectToAction("DestroyOrdersList");
            }
            else
            {
                return RedirectToAction("DestroyOrdersList");
            }
        }
        private void DeleteDetails(int OrderId)
        {
            List<DestroyOrdersDetails> DestroyOrdersDetailsList = unitWork.DestroyOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in DestroyOrdersDetailsList)
            {
                DestroyOrdersDetails existing = unitWork.DestroyOrdersDetailsManager.GetById(item.DestroyOrderDetId);
                if (existing != null)
                {
                    unitWork.DestroyOrdersDetailsManager.Delete(existing);

                }
            }
        }



       

    }

}