using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using  nsAlienRFID2;
using System.IO;
using System.Configuration;
using Microsoft.Reporting.WebForms;

namespace AssetsManagement.Controllers
{

    public class OrdersController : Controller
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
        [HttpPost]
        public ActionResult Upload(OutOrdersViewModel Mymodel, HttpPostedFileBase postedFile)
        {
            string NewFilename = "";
            if (postedFile != null)
            {
                string CurfileName = Path.GetFileName(postedFile.FileName);
                string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                string FolderName = ConfigurationManager.AppSettings["OutOrdersPath"].ToString();
                string path = Server.MapPath(FolderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                postedFile.SaveAs(path + NewFilename);
                //ViewBag.ImageUrl = ConfigurationManager.AppSettings["ItemsHTMLPath"].ToString() + NewFilename;

            }

            if (NewFilename != "")
            {
                Mymodel.SelectedItem.PictureName = NewFilename;
            }
            return View();
        }
        [UserPermissionAttribute(AllowFeature = "OutOrders", AllowPermission = "Accessing")]
        public ActionResult OutOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            //List<Car> carList = null;

            int userId = SesssionUser.GetCurrentUserId();
            OutOrdersViewModel model = new OutOrdersViewModel();
            List<OutOrders> OutOrdersList = unitWork.OutOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending(m => m.OutOrderId).ToList();
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "OutOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "OutOrderId" ? "OutOrderDate" : "StoreId";

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
                OutOrdersList = unitWork.OutOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "OutOrderId":
                    OutOrdersList = OutOrdersList.OrderByDescending(stu => stu.OutOrderId).ToList();
                    break;
                case "OutOrderDate":
                    OutOrdersList = OutOrdersList.OrderByDescending(stu => stu.OutOrderDate).ToList();
                    break;
                case "StoreId":
                    OutOrdersList = OutOrdersList.OrderByDescending(stu => stu.StoreId).ToList();
                    break;
                default:
                    OutOrdersList = OutOrdersList.OrderByDescending(stu => stu.OutOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.OutOrders = OutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (OutOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditOutOrder")]
        public ActionResult AddEditOutOrder(string Id = null, int ScanFlag=0 )
        {
            OutOrdersViewModel model = new OutOrdersViewModel();
            try
            {        
                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("OutOrders", "Adding"))
                    {
                        ModelState.Clear();
                        OutOrders UnitRecord = new OutOrders();
                        UnitRecord.OutOrderDate = DateTime.Today;
                        model.SelectedItem = UnitRecord;
                        //model.DisplayMode = "ReadOnly";
                        model.EditMode = DataModel.AddMode;
                        List<OutOrdersDetails> OutOrdersDetialsList = new List<OutOrdersDetails>();
                        model.OutOrdersDetails = OutOrdersDetialsList;
                        model.SelectedItem.OrganizedFlag = 1; // personal outorder
                                                              //return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");

                    }
                }
                else
                {
                    if (this.HasPermission("OutOrders", "Updating"))
                    {
                        // Edit record (view in Edit mode)
                        int OrderId = int.Parse(Id);
                        //model.Buildings = unitWork.OutOrdersManager.GetNotDelAll().OrderBy(m => m.OutOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.OutOrdersManager.GetById(OrderId);
                        //model.DisplayMode = "ReadWrite";
                        model.EditMode = DataModel.EditMode;
                        model.CurOrderDate = model.SelectedItem.OutOrderDate;
                        List<OutOrdersDetails> OutOrdersDetailsList = unitWork.OutOrdersDetailsManager.GetByOrderId(OrderId);
                        model.OutOrdersDetails = OutOrdersDetailsList;
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
            catch( Exception ex)
            {
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;
               
            }
            return GetData(model, "", "", "", 0, ScanFlag);

        }
        [HttpPost]
        public ActionResult AddEditOutOrder(OutOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, HttpPostedFileBase postedFile)
        {

            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];
            string CmdUpload = Request.Form["CmdUpload"];
            string cmdsubmit= Request.Form["CmdSelStore"]; 
            int ScanFlag = 0;

            if (Mymodel.PostFlag == 3) // post flag done
            {
                if (Mymodel.ReaderType == 2) // when user using  seacrh by barcode
                {
                    if (!string.IsNullOrEmpty(Mymodel.Barcode))
                    {
                        long SelStoreId = Mymodel.SelectedItem.StoreId.GetValueOrDefault();
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
                        bool GenerationItemFlag = false;
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
                            GenerationItemFlag = true;
                            
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
                                CostPrice = existingStock.MainCostPrice.GetValueOrDefault();
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
                            Mymodel.Barcode = "";
                            if (StoreId == Mymodel.SelectedItem.StoreId) // check select inventory
                            {
                                List<OutOrdersDetails> RowsOutOrdersDetials = new List<OutOrdersDetails>();
                                if (Mymodel.OutOrdersDetails != null)
                                {
                                    
                                    RowsOutOrdersDetials = Mymodel.OutOrdersDetails.Where(c => c.StockId == StockId && c.StockDetId==StockDetId).ToList();
                                }
                                else
                                {
                                    Mymodel.OutOrdersDetails = new List<OutOrdersDetails>();
                                }
                                if (RowsOutOrdersDetials.Count == 0)
                                {
                                    OutOrdersDetails NewItem = new OutOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Item_BarCode = Barcode;
                                    NewItem.Qty = 1;
                                    NewItem.StoreId = StoreId;
                                    NewItem.CostPrice = CostPrice;
                                    NewItem.InStoreQty = InStoreQty;
                                    NewItem.StockDetId = StockDetId;
                                    NewItem.InStoreItemQty = InStoreItemQty;
                                    Mymodel.OutOrdersDetails.Add(NewItem);

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
            else if (Mymodel.PostFlag == 6 || Mymodel.PostFlag==5)
            {
                ScanFlag = 1;
            }
            else if (CmdUpload == "CmdUpload")
            {
                string NewFilename = "";
                if (postedFile != null)
                {
                    string CurfileName = Path.GetFileName(postedFile.FileName);
                    string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                    string FolderName = "~" + ConfigurationManager.AppSettings["OutOrdersPath"].ToString();
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
                bool SucessFlag = true;
                if (Mymodel.EditMode == DataModel.EditMode) // chech date range when edit order
                {
                    SucessFlag = CheckDateRange(Mymodel.CurOrderDate.Value, Mymodel.SelectedItem.OutOrderDate.Value);
                }
                //DateTime min = Mymodel.CurOrderDate.Value.AddDays(-DataModel.DayRange);
                //DateTime max = Mymodel.CurOrderDate.Value.AddDays(DataModel.DayRange);
                //int Minres = DateTime.Compare(min, Mymodel.SelectedItem.OutOrderDate.Value);
                //int Maxres = DateTime.Compare(max, Mymodel.SelectedItem.OutOrderDate.Value);

                if (SucessFlag) // check date in date range 
                {

                    if (SaveOutOrder(Mymodel))
                    {
                        //return GetData(Mymodel, "", "", "" ,0);
                        return RedirectToAction("AddEditOutOrder", new { id = Mymodel.SelectedItem.OutOrderId, ScanFlag=0 });
                    }
                    else
                    {
                        return GetData(Mymodel, "", "", "", 0,0);
                        //return RedirectToAction("AddEditOutOrder", Mymodel);
                    }
                }
                else
                {
                    Mymodel.SelectedItem.OutOrderDate = Mymodel.CurOrderDate;
                    TempData["warningMessage"] = Resources.DefaultResource.CheckDateRange;
                }
            }
            // if make scan item from scan device
            else if (ScanValue == "Scan")
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
                      
                        if (StoreId == Mymodel.SelectedItem.StoreId)
                        {
                            List<OutOrdersDetails> RowsOutOrdersDetials = new List<OutOrdersDetails>();
                            if (Mymodel.OutOrdersDetails != null)
                            {
                                RowsOutOrdersDetials = Mymodel.OutOrdersDetails.Where(c => c.StockId == StockId).ToList();
                            }
                            else
                            {
                                Mymodel.OutOrdersDetails = new List<OutOrdersDetails>();
                            }
                            if (RowsOutOrdersDetials.Count == 0)
                            {
                                if (Mymodel.ScanItems[i].ItemQty > 0)
                                {
                                    OutOrdersDetails NewItem = new OutOrdersDetails();
                                    NewItem.ItemId = ItemId;
                                    NewItem.Item_RFID = ItemRFID;
                                    NewItem.StockId = StockId;
                                    NewItem.Qty = 1;
                                    //NewItem.CostPrice = 1;
                                    NewItem.StoreId = StoreId;
                                    NewItem.Item_BarCode = BarCode;
                                    NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;
                                    NewItem.InStoreItemQty  = Mymodel.ScanItems[i].ItemQty;

                                    Mymodel.OutOrdersDetails.Add(NewItem);
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
                    if (Mymodel.OutOrdersDetails != null)
                    {
                        Boolean DelFalg = true;
                        long OutOrderDetId = Mymodel.OutOrdersDetails[index].OutOrderDetId;
                        long ItemId = Mymodel.OutOrdersDetails[index].ItemId.GetValueOrDefault();
                        string Item_RFID = Mymodel.OutOrdersDetails[index].Item_RFID;

                        OutOrdersDetails existing = unitWork.OutOrdersDetailsManager.GetByOrderDetId(OutOrderDetId);
                        if (existing != null)
                        {
                            if (CheckItemDelete(existing.OutOrderDetId))
                            {
                                unitWork.OutOrdersDetailsManager.Delete(existing);
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.DeleteItemInOrderMsg;
                                DelFalg = false;
                            }
                        }
                        if (DelFalg)
                        {
                            Mymodel.OutOrdersDetails.RemoveAt(index);
                        }


                    }

                }

            }

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, ScanFlag));
        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(OutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No,0));
        }
        private bool SaveOutOrder(OutOrdersViewModel Mymodel)
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

                    if (Mymodel.SelectedItem.OrganizedFlag != null)
                    {
                        int OrganizedFlag = Mymodel.SelectedItem.OrganizedFlag.GetValueOrDefault();

                        if (OrganizedFlag == 1) // check choose personal asset
                        {
                            if (Mymodel.SelectedItem.EmpId != null && Mymodel.SelectedItem.DeptId != null && Mymodel.SelectedItem.RoomId != null)
                            {
                                TempData["warningMessage"] = null;
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.SelectedEmpAndDepartMsg;
                                ret = false; // end flag
                            }
                        }
                        else if (OrganizedFlag == 2) // check choose organizational asset
                        {
                            if (Mymodel.SelectedItem.DeptId != null && Mymodel.SelectedItem.RoomId != null)
                            {
                                TempData["warningMessage"] = null;
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.SelectedEmpAndDepartMsg;
                                ret = false; // end flag
                            }
                        }
                    }
                    if (Mymodel.OutOrdersDetails.Count == 0)
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                        TempData["Message"] = null;
                    }
                    if (ret)
                    {
                        if (ModelState.IsValid)
                        {
                            // Upload attachment file

                            if (Mymodel.SelectedItem.OutOrderId == 0)
                            {

                                // insert new record
                                OutOrders OutOrdersbj = unitWork.OutOrdersManager.Add(Mymodel.SelectedItem);
                                //unitWork.RoomsManager.SaveLog("OutOrders", int.Parse(Mymodel.SelectedItem.OutOrderId.ToString()), "Insert", SesssionUser.GetCurrentUserId());

                                if (OutOrdersbj != null)
                                {
                                    Mymodel.OutOrdersDetails = Mymodel.OutOrdersDetails;
                                    TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
                                }
                                else
                                {
                                    TempData["Message"] = null;
                                }
                                //ModelState.Clear();
                            }
                            else
                            {

                                bool bret = unitWork.OutOrdersManager.Update(Mymodel.SelectedItem);

                                // update record
                            }

                            if (SaveOrderItems(Mymodel))
                            {
                                TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
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
        private bool CheckEnoughQty(OutOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.OutOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                OutOrdersDetails OutOrdersDetailsObj = unWork.OutOrdersDetailsManager.GetById(item.OutOrderDetId);
                if (OutOrdersDetailsObj != null)
                {
                    OldQty = OutOrdersDetailsObj.Qty.GetValueOrDefault();
                }

                tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                if (ItemObj.CatSub_tbl.GenerateBarcodeFlag && item.StockDetId!=0)
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
        private bool SaveOrderItems(OutOrdersViewModel Mymodel)
        {
         
            bool ret = false;
            try
            {
                foreach (var item in Mymodel.OutOrdersDetails)
                {
                    item.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                    if (!item.tbl_ItemsStock.Item_tbl.CountableFlag)
                    {
                        item.Qty = 1;
                    }
                    if (item.OutOrderDetId == 0)
                    {
                        // insert new record
                        item.OutOrderId = Mymodel.SelectedItem.OutOrderId;

                        OutOrdersDetails OutOrderItemsbj = unitWork.OutOrdersDetailsManager.Add(item);
                        if (OutOrderItemsbj != null)
                        {
                            //Mymodel.OutOrderDetails = Mymodel.OutOrderDetails;
                            TempData["Message"] = Resources.DefaultResource.SaveMessage; //"Success";
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
                        item.OutOrderId = Mymodel.SelectedItem.OutOrderId;
                        bool bret = unitWork.OutOrdersDetailsManager.Update(item);
                        // update record
                        if (bret)
                        {
                            TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
                            ret = true;
                        }
                        else
                        {
                            TempData["Message"] = null;
                            ret = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            return ret;

        }
        
     
        private bool CheckDateRange(DateTime CurOrderDate, DateTime OutOrderDate)
        {
            DateTime min = CurOrderDate.AddDays(-DataModel.DayRange);
            DateTime max = CurOrderDate.AddDays(DataModel.DayRange);
            int Minres = DateTime.Compare(min, OutOrderDate);
            int Maxres = DateTime.Compare(max, OutOrderDate);

            if (Minres < 0 && Maxres >= 0) // check date in date range 
            {
                return true;
            }
            else
            {
                return   false;
            }
         }
        public ActionResult GetData(OutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, int? ScanFlag)
        {

            OutOrdersViewModel model = new OutOrdersViewModel();
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
         
            if (Mymodel.SelectedItem.OrganizedFlag == 0) // action to do in form for assets
            {
                Mymodel.SelectedItem.OrganizedFlag = 1;
            }

            if (Mymodel.ReaderType == 0 ) // action to do in form for assets
            {
                model.ReaderType = 1;
            }
            else
            {
                model.ReaderType = Mymodel.ReaderType;
            }
            model.SelectedItem = Mymodel.SelectedItem;
            model.Barcode = Mymodel.Barcode;
            model.CurOrderDate = Mymodel.CurOrderDate;

            model.EditMode  = Mymodel.EditMode;

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            Mymodel.searchType= Mymodel.SelectedItem.OrganizedFlag.GetValueOrDefault();
            model.searchType = Mymodel.SelectedItem.OrganizedFlag.GetValueOrDefault();
            model.DisplayMode = Mymodel.DisplayMode;
            int userId = SesssionUser.GetCurrentUserId();
            model.Inventories = unitWork.RoomsManager.GetUserInventories(userId).Where(m => m.StoreFlag == true).ToList().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

            //model.ItemsPopulateList(model.FromEmpId, null, 1);
            //model.ActionType = Mymodel.ActionType;
            //model.ItemsPopulateList(Mymodel.FromEmpId, null);
            switch (Mymodel.searchType)
            {
                case 1:
                    if (Mymodel.SelectedItem.EmpId != null)
                    {
                        //Boolean ChangeRoom = false;
                        //if (model.SelectedItem.RoomId == null && model.FromRoomId == null)
                        //{
                        //    ChangeRoom = true;
                        //}
                         model.setDropDrownList("FromEmpId", Mymodel.SelectedItem.EmpId.GetValueOrDefault(), 1);
                        //model.SelectedItem.RoomId = model.FromRoomId;
                        // if (ChangeRoom) 
                        //{ 
                        //    model.SelectedItem.RoomId = model.FromRoomId;
                        //}
                        // else
                        //{
                        //    model.SelectedItem.RoomId = model.SelectedItem.RoomId;
                        //}

                        //if (model.SelectedItem.DeptId == null)
                        //{
                        //    model.SelectedItem.DeptId = model.FromDeptId;
                        //}
                    }
                    else
                    {
                        model.ItemsPopulateList(0, null, 1);
                    }
                  
                    break;
                case 2:
                    if (Mymodel.SelectedItem.RoomId != null)
                    {
                        model.FromDeptId = Mymodel.SelectedItem.DeptId.GetValueOrDefault();
                        model.FromEmpId  = Mymodel.SelectedItem.EmpId.GetValueOrDefault();
                        model.setDropDrownList("FromRoomId", Mymodel.SelectedItem.RoomId.GetValueOrDefault(), 1);
                    }
                    else
                    {
                        model.ItemsPopulateList(null, 0, 1);
                    }

                    break;
            }
            //check inventory
            if (model.Inventories.Count() > 0 && Mymodel.SelectedItem.StoreId == null) 
            {
                //model.SelectedItem.StoreId = long.Parse(model.Inventories.First().Value);
            }
            // if user choose inventory connecto to reader
            if (Mymodel.SelectedItem.StoreId != null)
            {
                long? StoreId = Mymodel.SelectedItem.StoreId;
                Room_tbl Roomobj = new Room_tbl();
                Roomobj = unitWork.RoomsManager.GetById(StoreId);
                if (Roomobj != null)
                {
                    if (Roomobj.ReaderId != null)
                    {
                        if (Mymodel.PostFlag != 4 && Mymodel.PostFlag != 3) // if postback reader nothing
                        {
                            if (Mymodel.DisplayMode == "StoreId")
                            {
                                model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                            }
                        }
                    }
                    if (model.ReaderType == 1 && ScanFlag == 1)
                    {
                        model.IPAddress = Roomobj.IPAddress;
                        model.TcpFlag = Roomobj.TcpFlag.GetValueOrDefault();
                       
                        List<tbl_ItemsStock> ScanItemsList = ScanReader(model.IPAddress, model.TcpFlag, StoreId);
                        ScanItemsList.OrderBy(c => c.StockId);

                        model.ScanItems = ScanItemsList;//.ToPagedList(No_Of_Page, Size_Of_Page);
                        List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                        //for (int i= 0; i< ScanItemsList.Count; i++ )

                        foreach (var item in ScanItemsList)
                        {
                            //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                            bool IsSelected = false;
                            CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString() };
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

            foreach (var item in Mymodel.OutOrdersDetails)
            {
                item.tbl_ItemsStock  = unitWork.ItemsStockManager.GetById(item.StockId);
            }
            model.OutOrdersDetails = Mymodel.OutOrdersDetails;
            ModelState.Clear();
            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);
            
        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditOutOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid
        }
        private List<tbl_ItemsStock> ScanReader(string IpAddress, bool TcpFlag, long ? StoreId)
        {
            GlobalsCls globalClass = new GlobalsCls();
            //mReader = new clsReader();
            //mReaderInfo = mReader.ReaderSettings;
            string ReaderStatus = "";
            bool ret = globalClass.Connect( IpAddress,  TcpFlag,ref ReaderStatus);
            if (!ret)
            {
                TempData["ScannerMessage"] =Resources.DefaultResource.UnSuccessConnect;
            }
            else
            {
                TempData["ScannerMessage"] = null;
            }
            List<tbl_ItemsStock> ItemsStockListAll;
            ItemsStockListAll= globalClass.ParsTages(StoreId);
            globalClass.DisconnectReader();
            return ItemsStockListAll;
            //timer1.Start();
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            OutOrders existing = unitWork.OutOrdersManager.GetByOrderId (Id);

            if (existing != null)
            {
                if (existing.ReturnOutOrders.Count == 0)
                {
                    if (CheckDetailsBeforeDelete(Id))
                    {
                        unitWork.OutOrdersManager.Delete(existing);
                        DeleteDetails(Id); // delete order items details

                        OutOrdersViewModel model = new OutOrdersViewModel();
                        model.OutOrders = unitWork.OutOrdersManager.GetNotDelAll().OrderByDescending(m => m.OutOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                        model.SelectedItem = null;
                        model.DisplayMode = "";
                        TempData["warningMessage"] = null;
                    }
                    else
                    {
                        TempData["warningMessage"] = Resources.DefaultResource.DeleteInOrderMsg;
                    }
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteOutOrderMsg;
                  
                }
            }
           
            return RedirectToAction("OutOrderList");
        }
      
        private void DeleteDetails(int OrderId)
        {
            List<OutOrdersDetails> OutOrdersDetailsList = unitWork.OutOrdersDetailsManager.GetByOrderId(OrderId);

            foreach (var item in OutOrdersDetailsList)
            {
                OutOrdersDetails existing = unitWork.OutOrdersDetailsManager.GetById(item.OutOrderDetId);
                if (existing != null)
                {
                    
                        unitWork.OutOrdersDetailsManager.Delete(existing);

                }
            }
        }
        private Boolean CheckItemDelete(long InOrderDetId)
        {
            Boolean ret = true;
            OutOrdersDetails existing = unitWork.OutOrdersDetailsManager.GetByOrderDetId  (InOrderDetId);
            if (existing != null)
            {
                int Difference = existing.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;

                }

            }
            if (existing.ReturnOutOrdersDetails.Count > 0)
            {
                ret = false;
            }
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(long OrderId)
        {
            List<OutOrdersDetails> InOrdersDetailsList = unitWork.OutOrdersDetailsManager.GetByOrderId(OrderId);
            Boolean ret = true;
            foreach (var item in InOrdersDetailsList)
            {
                ret = CheckItemDelete(item.OutOrderDetId);
                if (!ret)
                {
                    ret = false;
                    break;
                }

            }
            return ret;
        }

        [HttpGet]
        [ActionName("ShowOrderDetails")]
        public ActionResult ShowOrderDetails(string id = null)
        {

            OutOrdersDetialsViewModel  model = new OutOrdersDetialsViewModel();
            if (String.IsNullOrEmpty(id))
            {

                //OutOrdersDetails OutOrdersDetailsRecord = new OutOrdersDetails();
                //model.SelectedUnit = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("OutOrderModal", model.OutOrdersDetials);
            }
            else
            {
                // Edit record (view in Edit mode)
                long OrderId = long.Parse(id);

                int Size_Of_Page = 30;
                              
                model.OutOrdersDetials = unitWork.OutOrdersDetailsManager.GetByOrderId(OrderId).OrderBy(m => m.OutOrderId).ToPagedList(No_Of_Page, Size_Of_Page);

                ViewBag.RoomName = model.OutOrdersDetials.FirstOrDefault().OutOrders.Room_tbl.Room_Name;
                if (!String.IsNullOrEmpty(model.OutOrdersDetials.FirstOrDefault().OutOrders.Remarks))
                {
                    ViewBag.Notes = model.OutOrdersDetials.FirstOrDefault().OutOrders.Remarks;
                }
                else
                {
                    ViewBag.Notes = "";
                }
                //ViewBag.EmpName = model.OutOrdersDetials.FirstOrDefault().OutOrders.VmEmployees.FULL_NAME_AR;
                //ViewBag.DeptName = model.OutOrdersDetials.FirstOrDefault().OutOrders.VmDepartments.Name;
                //model.OutOrders = OutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);

                //model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                model.DisplayMode = "ReadOnly";
                //if (model.SelectedUnit == null) { return View("_error"); }
                // ...
                return PartialView("OutOrderModal", model);
            }

        }
       


    }
}