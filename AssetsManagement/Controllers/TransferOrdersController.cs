using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using nsAlienRFID2;
using System.IO;
using System.Configuration;

namespace AssetsManagement.Controllers
{
    public class TransferOrdersController : Controller
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
        public ActionResult OrdersSearch(TransferOrdersViewModel model, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            //TransferOrdersViewModel model = new TransferOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            List<TransferOrders> TransferOrdersList = unitWork.TransferOrdersManager.GetNotDelAllByUserId (userId).OrderByDescending(m => m.TransferOrderId).ToList();
            if (! String.IsNullOrEmpty(model.OrderType))
            {
                TransferOrdersList = TransferOrdersList.Where(p => p.ConfirmFlag == int.Parse(model.OrderType)).ToList();
                ViewBag.OrderType = model.OrderType;
            }
            else
            {
                ViewBag.OrderType = "";
            }
           
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "TransferOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "TransferOrderId" ? "TransferOrderDate" : "StoreId";

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
                TransferOrdersList = unitWork.TransferOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "TransferOrderId":
                    TransferOrdersList = TransferOrdersList.OrderByDescending(stu => stu.TransferOrderId).ToList();
                    break;
                case "TransferOrderDate":
                    TransferOrdersList = TransferOrdersList.OrderBy(stu => stu.TransferOrderDate).ToList();
                    break;
                case "StoreId":
                    TransferOrdersList = TransferOrdersList.OrderByDescending(stu => stu.StoreId_To).ToList();
                    break;
                default:
                    TransferOrdersList = TransferOrdersList.OrderByDescending(stu => stu.TransferOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.TransferOrders = TransferOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (TransferOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [UserPermissionAttribute(AllowFeature = "Transfer orders", AllowPermission = "Accessing")]
        public ActionResult TransferOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            TransferOrdersViewModel model = new TransferOrdersViewModel();
            return OrdersSearch(model, Sorting_Order,  Search_Data,  Filter_Value, Page_No);

        }
        [HttpPost]
        public ActionResult TransferOrderList(TransferOrdersViewModel model, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return OrdersSearch(model, Sorting_Order, Search_Data, Filter_Value, Page_No);
        }

        
        [HttpGet]
        [ActionName("AddEditTransferOrder")]
        public ActionResult AddEditTransferOrder(string Id = null, int ScanFlag = 0)
        {
            
            TransferOrdersViewModel model = new TransferOrdersViewModel();

            try
            {
                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("Transfer orders", "Adding"))
                    {
                        ModelState.Clear();
                        TransferOrders UnitRecord = new TransferOrders();
                        UnitRecord.TransferOrderDate = DateTime.Today;
                        model.SelectedItem = UnitRecord;
                        model.DisplayMode = "ReadOnly";
                        model.EditMode = "Add";

                        List<TransferOrdersDetails> TransferOrdersDetailsList = new List<TransferOrdersDetails>();
                        model.TransferOrdersDetails = TransferOrdersDetailsList;
                        model.SelectedItem.ConfirmFlag = 0; // personal TransferOrder
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");

                    }
                    //return View(model);
                }
                else
                {
                    if (this.HasPermission("Transfer orders", "Updating"))
                    {
                        // Edit record (view in Edit mode)
                        int OrderId = int.Parse(Id);

                        //model.Buildings = unitWork.TransferOrdersManager.GetNotDelAll().OrderBy(m => m.TransferOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.TransferOrdersManager.GetById(OrderId);
                        model.DisplayMode = "ReadWrite";
                        model.EditMode = "Edit";
                        List<TransferOrdersDetails> TransferOrdersDetailsList = unitWork.TransferOrdersDetailsManager.GetByOrderId(OrderId);
                        model.TransferOrdersDetails = TransferOrdersDetailsList;
                        if (model.SelectedItem == null) { return View("_error"); }

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
            return GetData(model, "", "", "", 0,  ScanFlag );

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(TransferOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, 0));
        }
        private bool SaveTransferOrder(TransferOrdersViewModel Mymodel)
        {
            bool ret = true;

            try
            {
                if (Mymodel.SelectedItem.StoreId_To != null && Mymodel.SelectedItem.StoreId_From != null)
                {
                    long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
                    if (Mymodel.SelectedItem.StoreId_To  == Mymodel.SelectedItem.StoreId_From)
                    {
                        TempData["warningMessage"] = Resources.DefaultResource.DiffInventoryMsg;
                        TempData["Message"] = null;
                        ret = false; // end flag
                    }
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.RequiredInventoryMsg;
                    TempData["Message"] = null;
                    ret = false; // end flag
                }

                if (Mymodel.TransferOrdersDetails.Count == 0)
                {
                    ret = false;
                    TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                    TempData["Message"] = null;
                }
                if (ret)
                {
                    //if (ModelState.IsValid)
                    //{
                    if (Mymodel.SelectedItem.TransferOrderId == 0)
                    {
                        // insert new record
                        TransferOrders TransferOrdersbj = unitWork.TransferOrdersManager.Add(Mymodel.SelectedItem);
                        if (TransferOrdersbj != null)
                        {
                            Mymodel.TransferOrdersDetails = Mymodel.TransferOrdersDetails;
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
                        bool bret = unitWork.TransferOrdersManager.Update(Mymodel.SelectedItem);
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
        private bool CheckEnoughQty(TransferOrdersViewModel Mymodel)
        {
            bool ret = true;
            foreach (var item in Mymodel.TransferOrdersDetails)
            {
                int DiffQty = item.InStoreQty.GetValueOrDefault() - item.Qty.GetValueOrDefault();
                if (DiffQty < 0)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        private bool SaveOrderItems(TransferOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.TransferOrdersDetails)
            {
                if (item.StockId == 0) // save stock item if new
                {
                    tbl_ItemsStock StockObj = new tbl_ItemsStock();
                    StockObj.Item_Id = item.ItemId.GetValueOrDefault();
                    StockObj.Item_RFID = item.Item_RFID;
                    Item_tbl obj = unitWork.ItemsManager.GetById(item.ItemId);
                    if (obj != null)
                    {
                        bool CountableFlag = obj.CountableFlag;
                        if (!CountableFlag)
                        {
                            StockObj.ItemQty = 1;
                            StockObj.Item_Serial = item.Item_BarCode;
                        }
                        else
                        {
                            StockObj.Item_BarCode = item.Item_BarCode;
                        }
                        StockObj.Room_Id = Mymodel.SelectedItem.StoreId_To;

                        tbl_ItemsStock ItemStockObj = unitWork.ItemsStockManager.Add(StockObj);
                        item.StockId = ItemStockObj.StockId;
                        item.tbl_ItemsStock = ItemStockObj;
                    }
                }
                else
                {
                    item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                    //item.tbl_ItemsStock.Room_Id = Mymodel.SelectedItem.StoreId_To;
                }
                if (item.TransferOrderDetId == 0)
                {
                    // insert new record
                    item.TransferOrderId = Mymodel.SelectedItem.TransferOrderId;
                    TransferOrdersDetails TransferOrderItemsbj = unitWork.TransferOrdersDetailsManager.Add(item);
                    if (TransferOrderItemsbj != null)
                    {
                        //Mymodel.TransferOrdersDetails = Mymodel.TransferOrdersDetails;
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
                    item.TransferOrderId = Mymodel.SelectedItem.TransferOrderId;
                    bool bret = unitWork.TransferOrdersDetailsManager.Update(item);
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
        public ActionResult AddEditTransferOrder(TransferOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, HttpPostedFileBase postedFile)
        {
            int ScanFlag = 0;
            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];
            string CmdUpload = Request.Form["CmdUpload"];

            //if (Mymodel.PostFlag == 3)
            //{
            //    if (Mymodel.ReaderType == 2) // select barcode choice
            //    {
            //        if (!string.IsNullOrEmpty(Mymodel.Barcode))
            //        {
            //            long SelStoreId = Mymodel.SelectedItem.StoreId_From.GetValueOrDefault();


            //            tbl_ItemsStock existing = unitWork.ItemsStockManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
            //            if (Mymodel.ScanItems == null) { Mymodel.ScanItems = new List<tbl_ItemsStock>(); }

            //            if (existing != null) // item barcode is existing
            //            {
            //                //long ItemId = existing.Item_Id;
            //                //string ItemRFID = existing.Item_RFID;
            //                long StoreId = existing.Room_Id.GetValueOrDefault();
            //                //long StockId = existing.StockId;
            //                //string Barcode = existing.Item_BarCode;
            //                //int InStoreQty = existing.ItemQty.GetValueOrDefault();
            //                if (StoreId == Mymodel.SelectedItem.StoreId_From)
            //                {
            //                    Mymodel.ScanItems.Add(existing);

            //                    //List<TransferOrdersDetails> RowsTransferOrdersDetails = new List<TransferOrdersDetails>();
            //                    //if (Mymodel.TransferOrdersDetails != null)
            //                    //{
            //                    //    RowsTransferOrdersDetails = Mymodel.TransferOrdersDetails.Where(c => c.StockId == StockId).ToList();
            //                    //}
            //                    //else
            //                    //{
            //                    //    Mymodel.TransferOrdersDetails = new List<TransferOrdersDetails>();
            //                    //}
            //                    //if (RowsTransferOrdersDetails.Count == 0)
            //                    //{
            //                    //    TransferOrdersDetails NewItem = new TransferOrdersDetails();
            //                    //    NewItem.ItemId = ItemId;
            //                    //    NewItem.Item_RFID = ItemRFID;
            //                    //    NewItem.StockId = StockId;
            //                    //    NewItem.Item_BarCode = Barcode;
            //                    //    NewItem.Qty = 1;
            //                    //    NewItem.InStoreQty = InStoreQty;

            //                    //    Mymodel.TransferOrdersDetails.Add(NewItem);

            //                    //}
            //                    //else
            //                    //{
            //                    //    TempData["ScannerMessage"] = Resources.DefaultResource.DuplicateItemMsg;
            //                    //}
            //                }
            //                //else
            //                //{
            //                //    TempData["ScannerMessage"] = Resources.DefaultResource.InvalidItemBarCode;
            //                //}
            //            }
            //            else
            //            {

            //                tbl_ItemsStock NewStock = new tbl_ItemsStock();
            //                NewStock.Item_BarCode = Mymodel.Barcode;
            //                NewStock.Room_Id = SelStoreId;
            //                NewStock.Item_Id = 0;
            //                Mymodel.ScanItems.Add(NewStock);



            //                //TempData["ScannerMessage"] = Resources.DefaultResource.ItemNotFound;
            //            }
            //            List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
            //            foreach (var item in Mymodel.ScanItems)
            //            {
            //                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
            //                bool IsSelected = false;
            //                CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString(), ItemId = item.Item_Id };
            //                ItemsCheckBoxList.Add(CheckItem);
            //            }
            //            Mymodel.ItemsScanCheckList = ItemsCheckBoxList;
            //        }
            //    }

            //}
            if (Mymodel.PostFlag == 3) // post flag done
            {
                if (Mymodel.ReaderType == 2) // when user using  seacrh by barcode
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
                        //tbl_ItemsStock existing = unitWork.ItemsStockManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
                        // check item is countable and has details and cost price
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
                            if (existingStock != null)
                            {
                                ItemId = existingStock.Item_Id;
                                ItemRFID = existingStock.Item_RFID;
                                StoreId = existingStock.Room_Id.GetValueOrDefault();
                                StockId = existingStock.StockId;
                                Barcode = existingStock.Item_BarCode;
                                InStoreQty = existingStock.ItemQty.GetValueOrDefault();


                                //StockDetId = existing.StockDetId;
                                //InStoreItemQty = existingStock.ItemQty;
                                success = true;
                            }
                            else
                            {
                                success = false;
                            }

                        }
                        if (success) // item barcode is existing
                        {
                            if (StoreId == Mymodel.SelectedItem.StoreId_From) // check select inventory
                            {
                                List<TransferOrdersDetails> RowsTransferOrdersDetails = new List<TransferOrdersDetails>();
                                if (Mymodel.TransferOrdersDetails != null)
                                {
                                    RowsTransferOrdersDetails = Mymodel.TransferOrdersDetails.Where(c => c.StockId == StockId && c.StockDetId == StockDetId).ToList();
                                }
                                else
                                {
                                    Mymodel.TransferOrdersDetails = new List<TransferOrdersDetails>();
                                }
                                if (RowsTransferOrdersDetails.Count == 0)
                                {
                                    TransferOrdersDetails NewItem = new TransferOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Item_BarCode = Barcode;
                                    NewItem.Qty = 1;
                                    //NewItem.StoreId = StoreId;
                                    //NewItem.CostPrice = CostPrice;
                                    NewItem.InStoreQty = InStoreQty;
                                    NewItem.StockDetId = StockDetId;
                                    NewItem.InStoreItemQty = InStoreItemQty;
                                    Mymodel.TransferOrdersDetails.Add(NewItem);

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
                }
            }
            else if (CmdUpload == "CmdUpload")
            {
                string NewFilename = "";
                if (postedFile != null)
                {
                    string CurfileName = Path.GetFileName(postedFile.FileName);
                    string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                    string FolderName = "~" + ConfigurationManager.AppSettings["TransOrdersPath"].ToString();
                    string path = Server.MapPath(FolderName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                    postedFile.SaveAs(path + NewFilename);
                    //ViewBag.ImageUrl = ConfigurationManager.AppSettings["ItemsHTMLPath"].ToString() + NewFilename;

                    if (NewFilename != "")
                    {
                        Mymodel.SelectedItem.PictureName = NewFilename;
                        TempData["warningMessage"] = Resources.DefaultResource.UploadSucessMsg;
                    }
                }


            }
            else if (SaveValue == "CmdSave")
            {

                bool ret = SaveTransferOrder(Mymodel);
                if (ret)
                {
                    return RedirectToAction("AddEditTransferOrder", new { id = Mymodel.SelectedItem.TransferOrderId, ScanFlag = 0 });
                }
                else
                {
                    //return RedirectToAction("AddEditTransferOrder", new { id = Mymodel.SelectedItem.TransferOrderId, ScanFlag = 0 });
                    return GetData(Mymodel, "", "", "", 0, 0);
                    //return RedirectToAction("AddEditTransferOrder", Mymodel);
                }

            }
            // if make scan item from scan device
            else if (Mymodel.PostFlag == 6 || Mymodel.PostFlag == 5)
            {
                ScanFlag = 1;
            }
            else if (ScanValue == "Scan" || Mymodel.DisplayMode == "StoreId_From")
            {
                ScanFlag = 1;
            }
            
            else if (AddValue == "AddItem") // add item button to inser item in right list
            {
                ScanFlag = 0;
                int i = 0;
                foreach (var item in Mymodel.ItemsScanCheckList)
                {
                    if (item.IsSelected)
                    {


                        // check new item add to Inventory
                        if (Mymodel.ScanItems[i].Item_Id == 0 && Mymodel.SelectedItem.StoreId_To != null && Mymodel.SelectedItem.StoreId_From != null)
                        {
                            if (Mymodel.ItemsScanCheckList[i].ItemId > 0) // check user select ItemName
                            {
                                Item_tbl obj = unitWork.ItemsManager.GetById(Mymodel.ItemsScanCheckList[i].ItemId);
                                if (obj != null)
                                {
                                    bool CountableFlag = obj.CountableFlag;
                                    if (!CountableFlag) { Mymodel.ScanItems[i].ItemQty = 1; }
                                    Mymodel.ScanItems[i].Item_Id = Mymodel.ItemsScanCheckList[i].ItemId;
                                    Mymodel.ScanItems[i].Room_Id = Mymodel.SelectedItem.StoreId_To;

                                    //unitWork.ItemsStockManager.Add(Mymodel.ScanItems[i]);
                                }
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.CheckItemHasStockMsg + " " + Mymodel.ScanItems[i].Item_RFID;
                                break;
                            }
                        }
                        else
                        {
                            Item_tbl obj = unitWork.ItemsManager.GetById(Mymodel.ScanItems[i].Item_Id);
                            if (obj != null)
                            {
                                bool CountableFlag = obj.CountableFlag;
                                long curStoreId = Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                                //if (!CountableFlag) // وجود صنف غير نثري واضافته مرة اخري
                                //{
                                //    TempData["warningMessage"] = Resources.DefaultResource.DuplicateUncountableItemInstock + " " + Mymodel.ScanItems[i].Item_RFID;
                                //    break;
                                //}
                                if (curStoreId != Mymodel.SelectedItem.StoreId_From) // التاكد من ان الصنف نثري و موجود فى نفس المخزن
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.CheckItemsInventoryMsg + " " + Mymodel.ScanItems[i].Item_RFID;
                                    break;
                                }
                            }

                        }




                        long ItemId = Mymodel.ScanItems[i].Item_Id;
                        string ItemRFID = Mymodel.ScanItems[i].Item_RFID;
                        long StoreId = Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                        long StockId = Mymodel.ScanItems[i].StockId;
                        string BarCode = Mymodel.ScanItems[i].Item_BarCode;
                        //if (StoreId == Mymodel.SelectedItem.StoreId_To)
                        //{
                        List<TransferOrdersDetails> RowsTransferOrdersDetails = new List<TransferOrdersDetails>();
                        if (Mymodel.TransferOrdersDetails != null)
                        {
                            //case of scanning by RFID reader
                            if (Mymodel.ReaderType == 1)
                            {
                                RowsTransferOrdersDetails = Mymodel.TransferOrdersDetails.Where(c => c.StockId == StockId && c.ItemId == ItemId && c.Item_RFID == ItemRFID).ToList();
                            }
                            else
                            {
                                RowsTransferOrdersDetails = Mymodel.TransferOrdersDetails.Where(c => c.StockId == StockId && c.ItemId == ItemId).ToList();
                            }
                        }
                        else
                        {
                            Mymodel.TransferOrdersDetails = new List<TransferOrdersDetails>();
                        }
                        if (RowsTransferOrdersDetails.Count == 0)
                        {
                            TransferOrdersDetails NewItem = new TransferOrdersDetails();
                            NewItem.ItemId = ItemId;
                            NewItem.Item_RFID = ItemRFID;
                            NewItem.StockId = StockId;
                            NewItem.Qty = 1;
                            NewItem.Item_BarCode = BarCode;
                            //NewItem.StoreId = StoreId;
                            NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;

                            Mymodel.TransferOrdersDetails.Add(NewItem);

                        }

                        //}
                        //else
                        //{
                        //    TempData["warningMessage"] = Resources.DefaultResource.CheckItemsInventoryMsg;
                        //}

                    }
                    i = ++i;
                }


            }
            else if (DelValue == "CmdDel")
            {

                if (Id != null)
                {
                    int index = Id.GetValueOrDefault();
                    if (Mymodel.TransferOrdersDetails != null)
                    {
                        long TransferOrderDetId = Mymodel.TransferOrdersDetails[index].TransferOrderDetId;
                        long ItemId = Mymodel.TransferOrdersDetails[index].ItemId.GetValueOrDefault();
                        string Item_RFID = Mymodel.TransferOrdersDetails[index].Item_RFID;


                        TransferOrdersDetails existing = unitWork.TransferOrdersDetailsManager.GetById(TransferOrderDetId);
                        if (existing != null)
                        {
                            //tbl_ItemsStock StockObj =  unitWork.ItemsStockManager.GetById(existing.StockId);
                            //if (StockObj.Room_Id == Mymodel.SelectedItem.StoreId_To)
                            //{
                            unitWork.TransferOrdersDetailsManager.Delete(existing);

                            //}
                        }
                        Mymodel.TransferOrdersDetails.RemoveAt(index);

                    }
                    //ScanFlag = true;
                }

            }
            //else if (Mymodel.ReaderType == 1)
            //{
            //    ScanFlag = 1;
            //}
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, ScanFlag));
        }

        public ActionResult GetData(TransferOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,  int? ScanFlag)
        {

            TransferOrdersViewModel model = new TransferOrdersViewModel();
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
            model.EditMode = Mymodel.EditMode;

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            //Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
            int userId = SesssionUser.GetCurrentUserId();
            model.FromStores = unitWork.RoomsManager.GetUserInventories(userId).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            model.ToStores = unitWork.RoomsManager.GetUserInventories(userId).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            //model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().ToList().Select(option => new SelectListItem
            //{
            //    Text = option.Sup_Name,
            //    Value = option.Sup_code.ToString()
            //});

            //List<Item_tbl>  Items = new List<Item_tbl>();
            //if (model.ReaderType==2) { 
            //    Items = unitWork.ItemsManager.GetNotDelAllByType().ToList(); 
            //} 
            //else { 
            //    Items = unitWork.ItemsManager.GetNotDelAll().ToList(); 
            //}
            //model.Items = Items.Select(option => new SelectListItem
            //{
            //    Text = option.Item_Name,
            //    Value = option.Item_Id.ToString()
            //});

            if (ScanFlag==1)
            {

                //check inventory
                //if (model.ToStores.Count() > 0 && Mymodel.SelectedItem.StoreId_To == null)
                //{
                //    model.SelectedItem.StoreId_To = long.Parse(model.ToStores.First().Value);
                //}
                //if (model.Suppliers.Count() > 0 && Mymodel.SelectedItem.SupplierId_From == null)
                //{
                //    model.SelectedItem.SupplierId_From = int.Parse(model.Suppliers.First().Value);
                //}
                // if user choose inventory connecto to reader
                if (Mymodel.SelectedItem.StoreId_From != null)
                {
                    Room_tbl Roomobj = new Room_tbl();
                    Roomobj = unitWork.RoomsManager.GetById(Mymodel.SelectedItem.StoreId_From);
                    if (Roomobj != null)
                    {
                        if (Roomobj.ReaderId != null)
                        {
                           
                            if (Mymodel.PostFlag != 4 && Mymodel.PostFlag != 3) // if postback reader nothing
                            {
                                if (Mymodel.DisplayMode == "SelectedItem_StoreId_From")
                                {
                                    model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                                }
                            }
                                //model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                            
                        }
                        if (model.ReaderType == 1 && ScanFlag == 1)
                        {
                            model.IPAddress = Roomobj.IPAddress;
                            model.TcpFlag = Roomobj.TcpFlag.GetValueOrDefault();
                            // scan from reader
                            List<tbl_ItemsStock> ScanItemsList = ScanReader(model.IPAddress, model.TcpFlag, Mymodel.SelectedItem.StoreId_From);
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
            foreach (var item in Mymodel.TransferOrdersDetails)
            {
                item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
            }
            model.TransferOrdersDetails = Mymodel.TransferOrdersDetails;
            ModelState.Clear();
            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditTransferOrder", "Orders", Mymodel);
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
            TransferOrders existing = unitWork.TransferOrdersManager.GetById(Id);
            if (existing != null)
            {
                unitWork.TransferOrdersManager.Delete(existing);
                DeleteDetails(Id); // delete order items

                TransferOrdersViewModel model = new TransferOrdersViewModel();
                model.TransferOrders = unitWork.TransferOrdersManager.GetNotDelAll().OrderByDescending(m => m.TransferOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                model.SelectedItem = null;
                model.DisplayMode = "";

                return RedirectToAction("TransferOrderList");
            }
            else
            {
                return RedirectToAction("TransferOrderList");
            }
        }
        private void DeleteDetails(int OrderId)
        {
            List<TransferOrdersDetails> TransferOrdersDetailsList = unitWork.TransferOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in TransferOrdersDetailsList)
            {
                TransferOrdersDetails existing = unitWork.TransferOrdersDetailsManager.GetById(item.TransferOrderDetId);
                if (existing != null)
                {
                    unitWork.TransferOrdersDetailsManager.Delete(existing);

                }
            }
        }

        public ActionResult DisplayTransferOrder(string Id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();

            TransferOrdersViewModel model = new TransferOrdersViewModel();

            if (!String.IsNullOrEmpty(Id))
            { }
            // Edit record (view in Edit mode)
            int OrderId = int.Parse(Id);

            //model.Buildings = unitWork.TransferOrdersManager.GetNotDelAll().OrderBy(m => m.TransferOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
            model.SelectedItem = unitWork.TransferOrdersManager.GetById(OrderId);
            model.DisplayMode = "ReadWrite";
            model.EditMode = "Edit";
            List<TransferOrdersDetails> TransferOrdersDetailsList = unitWork.TransferOrdersDetailsManager.GetByOrderId(OrderId);
            model.TransferOrdersDetails = TransferOrdersDetailsList;
            if (model.SelectedItem == null) { return View("_error"); }

            model.FromStores = unitWork.RoomsManager.GetInventoriesAll().ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            model.ToStores = unitWork.RoomsManager.GetInventoriesAll().ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            return View(model);
        }

        [HttpPost]
        [ActionName("DisplayTransferOrder")]
        public ActionResult DisplayTransferOrder(TransferOrdersViewModel model)
        {
            model.SelectedItem.ConfirmFlag = 1;
            bool ret = SaveTransferOrder(model);
            if (ret)
            {
                return RedirectToAction("DisplayTransferOrder", new { id = model.SelectedItem.TransferOrderId });
            }
            else
            {
                return View(model);
            }
        }

    }
    


}