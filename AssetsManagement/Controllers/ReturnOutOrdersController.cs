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
    public class ReturnOutOrdersController : Controller
    {
        // reader variables

        //-------------------------------end of reader variables---------------------------------------
        public int Qty = 0;
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        private UnitOfWork CurUnitWork = new UnitOfWork();
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReturnOutOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string OrderId = null)
        {

            //List<Car> carList = null;
            if (!string.IsNullOrEmpty(OrderId))
            {
                ReturnOutOrdersViewModel model = new ReturnOutOrdersViewModel();
                long CurOrderId = long.Parse(OrderId);
                List<ReturnOutOrders> ReturnOutOrderList = unitWork.ReturnOutOrdersManager.GetNotDelAllByOrderId(CurOrderId).OrderByDescending(m => m.ReturnOrderId).ToList();
                model.SelectedItem = null;
                //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
                ViewBag.CurrentSortOrder = Sorting_Order;
                ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "ReturnOrderId" : "";
                ViewBag.SortingModel = Sorting_Order == "ReturnOrderId" ? "ReturnOrderId" : "ReturnOrderDate";

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
                    ReturnOutOrderList = unitWork.ReturnOutOrdersManager.GetCastByName(Search_Data);
                    //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
                }
                switch (Sorting_Order)
                {
                    case "ReturnOrderId":
                        ReturnOutOrderList = ReturnOutOrderList.OrderByDescending(stu => stu.ReturnOrderId).ToList();
                        break;
                    case "ReturnOrderDate":
                        ReturnOutOrderList = ReturnOutOrderList.OrderBy(stu => stu.ReturnOrderDate).ToList();
                        break;

                    default:
                        ReturnOutOrderList = ReturnOutOrderList.OrderByDescending(stu => stu.ReturnOrderId).ToList();
                        //carList =
                        break;
                }


                int Size_Of_Page = 15;
                int No_Of_Page = (Page_No ?? 1);
                model.ReturnOutOrders = ReturnOutOrderList.ToPagedList(No_Of_Page, Size_Of_Page);
                if (ReturnOutOrderList.Any())
                {
                    return View(model);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditReturnOutOrder")]
        public ActionResult AddEditReturnOutOrder(string OutOrderId, string Id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            ReturnOutOrdersViewModel model = new ReturnOutOrdersViewModel();

            //model.ItemsPopulateList(model.FromEmpId, null,1);


            if (String.IsNullOrEmpty(Id) || Id=="0")
            {
                ModelState.Clear();
                ReturnOutOrders UnitRecord = new ReturnOutOrders();
                UnitRecord.ReturnOrderDate = DateTime.Today;
                model.SelectedItem = UnitRecord;
                model.SelectedItem.OutOrderId = long.Parse(OutOrderId);

                model.EditMode = "Add";
                model.DisplayMode = "ReadOnly";
                OutOrders OutOrdersList = unitWork.OutOrdersManager.GetByOrderId(long.Parse(OutOrderId));
                model.SelectedItem.OutOrders = OutOrdersList;


                List<ReturnOutOrdersDetails> ReturnOutOrdersDetailsList = new List<ReturnOutOrdersDetails>();
                model.ReturnOutOrdersDetails = ReturnOutOrdersDetailsList;
                //model.SelectedItem.TransferFlag = 1; // personal ReturnOutOrder
                //return View(model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int OrderId = int.Parse(Id);

                //model.Buildings = unitWork.ReturnOutOrdersManager.GetNotDelAll().OrderBy(m => m.ReturnOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.ReturnOutOrdersManager.GetById(OrderId);

                if (model.SelectedItem == null)
                {
                    return RedirectToAction("ReturnOutOrderList", new { OrderId = OrderId });
                }

                model.DisplayMode = "ReadWrite";
                model.EditMode = "Edit";

                OutOrders OutOrdersList = unitWork.OutOrdersManager.GetByOrderId(long.Parse(OutOrderId));
                model.SelectedItem.OutOrders = OutOrdersList;

                List<ReturnOutOrdersDetails> ReturnOutOrdersDetailsList = unitWork.ReturnOutOrdersDetailsManager.GetByOrderId(OrderId);
                model.ReturnOutOrdersDetails = ReturnOutOrdersDetailsList;



                // ...
                //return View( model);
            }

            return GetData(model, "", "", "", 0);

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(ReturnOutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }
        private bool SaveReturnOutOrder(ReturnOutOrdersViewModel Mymodel)
        {
            bool ret = true;
          
            string ItemName = "";
            if (!CheckEnoughQty(Mymodel, ref ItemName))
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.InSufficientReturnQtyMsg + " " + ItemName; ;
                TempData["Message"] = null;

            }
            if (Mymodel.ReturnOutOrdersDetails.Count == 0)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                TempData["Message"] = null;
            }
            if (ret)
            {
                //if (ModelState.IsValid)
                //{
                if (Mymodel.SelectedItem.ReturnOrderId == 0)
                {
                    // insert new record
                    ReturnOutOrders ReturnOutOrdersbj = unitWork.ReturnOutOrdersManager.Add(Mymodel.SelectedItem);
                    if (ReturnOutOrdersbj != null)
                    {
                        Mymodel.ReturnOutOrdersDetails = Mymodel.ReturnOutOrdersDetails;
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
                    bool bret = unitWork.ReturnOutOrdersManager.Update(Mymodel.SelectedItem);
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
            //}
            //else
            //{
            //    TempData["warningMessage"] = Resources.DefaultResource.InSufficientQtyMsg;
            //    ret = false;

            //}
            return ret;
        }
        // check enough quantity to In
        private bool CheckEnoughQty(ReturnOutOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.ReturnOutOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                ReturnOutOrdersDetails ReturnOutOrdersDetailsObj = unWork.ReturnOutOrdersDetailsManager.GetById(item.ReturnOrderDetId);
                if (ReturnOutOrdersDetailsObj != null)
                {
                    OldQty = ReturnOutOrdersDetailsObj.Qty.GetValueOrDefault();
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
                    TotalQty = item.InStoreQty.GetValueOrDefault();
                }
                // get value of quantity in current inorder
                OutOrdersDetails OutOrdersDetailsObj = unWork.OutOrdersDetailsManager.GetById(item.OutOrderDetId);
                if (item.Qty.GetValueOrDefault() > OutOrdersDetailsObj.Qty.GetValueOrDefault()) // القيمه اكبر من الكمية الموجود اصلا فى السند
                {
                    ret = false;
                }
                else
                {
                    int OutOrderQty = OutOrdersDetailsObj.Qty.GetValueOrDefault();
                    int DiffQty = (OutOrderQty + OldQty) - item.Qty.GetValueOrDefault(); // نتاكد ان الكمية الحالية لا تصبح سالب
                    if (DiffQty < 0)
                    {
                        ret = false;
                    }
                  
                }
                if (!ret)
                {
                    ItemName = StockObj.ItemName;
                    break;
                }
            }
            return ret;
        }
        private bool SaveOrderItems(ReturnOutOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.ReturnOutOrdersDetails)
            {

                item.OutOrdersDetails = unitWork.OutOrdersDetailsManager.GetById(item.OutOrderDetId);
                if (item.ReturnOrderDetId == 0)
                {
                    // insert new record

                    item.ReturnOrderId = Mymodel.SelectedItem.ReturnOrderId;
                    ReturnOutOrdersDetails ReturnOutOrderItemsbj = unitWork.ReturnOutOrdersDetailsManager.Add(item);
                    if (ReturnOutOrderItemsbj != null)
                    {
                        //Mymodel.ReturnOutOrdersDetails = Mymodel.ReturnOutOrdersDetails;
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
                    item.ReturnOrderId = Mymodel.SelectedItem.ReturnOrderId;
                    bool bret = unitWork.ReturnOutOrdersDetailsManager.Update(item);
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
        public ActionResult AddEditReturnOutOrder(ReturnOutOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            bool ScanFlag = false;
            string SaveValue = Request.Form["CmdSave"];
            //string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];


            if (Mymodel.PostFlag == 3)
            {
                List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                foreach (var item in Mymodel.SelectedItem.OutOrders.OutOrdersDetails)
                {
                    //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                    bool IsSelected = false;
                    CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.OutOrderDetId, Name = "Item" + item.OutOrderDetId.ToString(), ItemId = item.OutOrderDetId };
                    ItemsCheckBoxList.Add(CheckItem);
                }
                Mymodel.ItemsScanCheckList = ItemsCheckBoxList;

            }
            else if (SaveValue == "CmdSave")
            {

                if (SaveReturnOutOrder(Mymodel))
                {

                    return RedirectToAction("AddEditReturnOutOrder", new { OutOrderId = Mymodel.SelectedItem.OutOrderId, id = Mymodel.SelectedItem.ReturnOrderId });
                }
                else
                {
                    //return GetData(Mymodel, "", "", "", 0);
                    return RedirectToAction("AddEditReturnOutOrder", new { OutOrderId = Mymodel.SelectedItem.OutOrderId, id = Mymodel.SelectedItem.ReturnOrderId });
                }

            }
            // if make scan item from scan device

            else if (AddValue == "AddItem")
            {
                int i = 0;
                ScanFlag = false;
                if (Mymodel.SelectedItem.OutOrders == null)
                {
                    OutOrders OutOrdersList = unitWork.OutOrdersManager.GetByOrderId(Mymodel.SelectedItem.OutOrderId.GetValueOrDefault());
                    Mymodel.SelectedItem.OutOrders = OutOrdersList;
                }
                List<OutOrdersDetails> OutOrdersDetailsList = Mymodel.SelectedItem.OutOrders.OutOrdersDetails.ToList();
                foreach (var item in Mymodel.ItemsScanCheckList)
                {
                    if (item.IsSelected)
                    {
                        long ItemId = OutOrdersDetailsList[i].ItemId.GetValueOrDefault();
                        string ItemRFID = OutOrdersDetailsList[i].Item_RFID;
                        long StoreId = OutOrdersDetailsList[i].StoreId.GetValueOrDefault();
                        long StockId = OutOrdersDetailsList[i].StockId.GetValueOrDefault();
                        string BarCode = OutOrdersDetailsList[i].Item_BarCode;

                        List<ReturnOutOrdersDetails> RowsReturnOutOrdersDetails = new List<ReturnOutOrdersDetails>();
                        if (Mymodel.ReturnOutOrdersDetails != null)
                        {
                            RowsReturnOutOrdersDetails = Mymodel.ReturnOutOrdersDetails.Where(c => c.StockId == StockId).ToList();
                        }
                        else
                        {
                            Mymodel.ReturnOutOrdersDetails = new List<ReturnOutOrdersDetails>();
                        }
                       
                        if (RowsReturnOutOrdersDetails.Count == 0)
                        {
                            if (OutOrdersDetailsList[i].Qty > 0)
                            {
                                ReturnOutOrdersDetails NewItem = new ReturnOutOrdersDetails();
                                //NewItem.ItemId = ItemId;

                                OutOrdersDetails RowsOutOrdersDetails = unitWork.OutOrdersDetailsManager.GetById(OutOrdersDetailsList[i].OutOrderDetId);
                                NewItem.OutOrdersDetails = RowsOutOrdersDetails;
                                NewItem.OutOrderDetId = OutOrdersDetailsList[i].OutOrderDetId;
                                NewItem.StockId = StockId;
                                NewItem.Qty = 1;
                                List<tbl_ItemsStockDetails> ItemsStockDetailsList = unitWork.ItemsStockDetailsManager.GetByStockDetId (OutOrdersDetailsList[i].StockDetId.GetValueOrDefault() );
                                if (ItemsStockDetailsList.Count > 0)
                                {
                                    NewItem.InStoreItemQty = ItemsStockDetailsList.FirstOrDefault().Qty;
                                    NewItem.StockDetId = OutOrdersDetailsList[i].StockDetId;
                                }

                                //NewItem.Item_BarCode = BarCode;
                                NewItem.InStoreQty = OutOrdersDetailsList[i].tbl_ItemsStock.ItemQty;//.InStoreQty;
                                NewItem.StoreId = StoreId;
                                Mymodel.ReturnOutOrdersDetails.Add(NewItem);
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.QtyLimitMsg;
                            }
                        }

                    }
                    i = ++i;
                }


            }
            else if (DelValue == "CmdDel")
            {
                if (Id != null)
                {
                    Boolean DelFalg = true;
                    int index = Id.GetValueOrDefault();
                    if (Mymodel.ReturnOutOrdersDetails != null)
                    {
                        long ReturnOrderDetId = Mymodel.ReturnOutOrdersDetails[index].ReturnOrderDetId;
                        //long ItemId = Mymodel.ReturnOutOrdersDetails[index].ItemId.GetValueOrDefault();
                        //string Item_RFID = Mymodel.ReturnOutOrdersDetails[index].Item_RFID;

                        ReturnOutOrdersDetails existing = unitWork.ReturnOutOrdersDetailsManager.GetById(ReturnOrderDetId);
                        if (existing != null)
                        {
                            if (CheckItemDelete(existing.ReturnOrderDetId))
                            {
                                unitWork.ReturnOutOrdersDetailsManager.Delete(existing);
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.DeleteItemInOrderMsg;
                                DelFalg = false;
                            }
                        }
                        Mymodel.ReturnOutOrdersDetails.RemoveAt(index);

                    }
                    if (Mymodel.SelectedItem.OutOrders == null)
                    {
                        Mymodel.SelectedItem.OutOrders = unitWork.OutOrdersManager.GetByOrderId(Mymodel.SelectedItem.OutOrderId.GetValueOrDefault());
                    }
                    Mymodel.ReturnOutOrdersDetails = unitWork.ReturnOutOrdersDetailsManager.GetByOrderId(Mymodel.SelectedItem.ReturnOrderId);
                    ScanFlag = true;
                }

            }
            //else if (Mymodel.ReaderType == 1)
            //{
            //    ScanFlag = true;
            //}
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }

        public ActionResult GetData(ReturnOutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            ReturnOutOrdersViewModel model = new ReturnOutOrdersViewModel();
            model = Mymodel;
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();




            //model.ReturnOutOrdersDetails = Mymodel.ReturnOutOrdersDetails ;
            //model.SelectedItem.OutOrders = Mymodel.SelectedItem.OutOrders;



            //model.FromStoreId = Mymodel.SelectedItem.OutOrderId;
            //model.Barcode = Mymodel.Barcode;
            model.SelectedItem = Mymodel.SelectedItem;
            model.EditMode = Mymodel.EditMode;

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            //Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
            //model.FromStores = unitWork.RoomsManager.GetInventoriesAll().ToList().Select(option => new SelectListItem
            //{
            //    Text = option.Room_Name,
            //    Value = option.Room_Id.ToString()
            //});


            //if (model.SelectedItem.OutOrders == null)
            //{
            //    model.SelectedItem.OutOrders = unitWork.OutOrdersManager.GetByOrderId(Mymodel.SelectedItem.OutOrderId.GetValueOrDefault());
            //}
            List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
            foreach (var item in Mymodel.SelectedItem.OutOrders.OutOrdersDetails)
            {
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                List<tbl_ItemsStockDetails> ItemsStockDetails = unitWork.ItemsStockDetailsManager.GetByStockDetId(item.StockDetId.GetValueOrDefault());
                if (ItemsStockDetails.Count > 0)
                {
                    item.ItemInStoreQty = ItemsStockDetails.FirstOrDefault().Qty;
                }
                bool IsSelected = false;
                CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.OutOrderDetId, Name = "Item" + item.OutOrderDetId.ToString(), ItemId = item.OutOrderDetId };
                ItemsCheckBoxList.Add(CheckItem);
            }

            //Mymodel.ItemsScanCheckList = ItemsCheckBoxList;
            model.ItemsScanCheckList = ItemsCheckBoxList;


            //else
            //{

            //    model.ScanItems = Mymodel.ScanItems;
            //    int j = 0;
            //    if (Mymodel.ScanItems != null)
            //    {
            //        foreach (var Scanitem in Mymodel.ScanItems)
            //        {
            //            if (Mymodel.ItemsScanCheckList[j].ItemId != 0)
            //            {

            //            }
            //            Scanitem.Item_tbl = unitWork.ItemsManager.GetById(Scanitem.Item_Id);
            //            Scanitem.Room_tbl = unitWork.RoomsManager.GetById(Scanitem.Room_Id);
            //            j++;

            //        }
            //        model.ItemsScanCheckList = Mymodel.ItemsScanCheckList;

            //        if (Mymodel.ReaderType == 2 && string.IsNullOrEmpty(Mymodel.Barcode))
            //        {
            //            model.ScanItems.Clear();
            //            model.ItemsScanCheckList.Clear();
            //        }
            //    }
            //}


            foreach (var item in Mymodel.ReturnOutOrdersDetails)
            {

                //OutOrdersDetails RowsOutOrdersDetails1 = unitWork.OutOrdersDetailsManager.GetById(item.OutOrderDetId);
                //item.OutOrdersDetails = RowsOutOrdersDetails1;
                if (item.OutOrdersDetails.tbl_ItemsStock == null)
                {
                    item.OutOrdersDetails.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                }

                //item.tbl_ItemsStock.Item_tbl= unitWork.ItemsManager.GetById(item.ItemId);
            }
            //model.SelectedItem.OutOrders= Mymodel.SelectedItem.OutOrders;
            model.ReturnOutOrdersDetails = Mymodel.ReturnOutOrdersDetails;

            //ModelState.Clear();

            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditReturnOutOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid

        }



        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            long CurOrderId = 0;
            ReturnOutOrders existing = unitWork.ReturnOutOrdersManager.GetById(Id);
            if (existing != null)
            {
                 CurOrderId = existing.OutOrderId.GetValueOrDefault() ;
                if (CheckDetailsBeforeDelete(Id))
                {
                    unitWork.ReturnOutOrdersManager.Delete(existing);
                    DeleteDetails(Id); // delete order items

                    ReturnOutOrdersViewModel model = new ReturnOutOrdersViewModel();
                    model.ReturnOutOrders = unitWork.ReturnOutOrdersManager.GetNotDelAll().OrderByDescending(m => m.ReturnOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                    model.SelectedItem = null;
                    model.DisplayMode = "";
                    TempData["warningMessage"] = null;
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteOutOrderMsg;
                }
               
            }
           
            return RedirectToAction("ReturnOutOrderList", new { OrderId = CurOrderId });
        }
        private void DeleteDetails(int OrderId)
        {
            UnitOfWork DelReturnUnitWork = new UnitOfWork();
            List<ReturnOutOrdersDetails> ReturnOutOrdersDetailsList = unitWork.ReturnOutOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in ReturnOutOrdersDetailsList)
            {
                ReturnOutOrdersDetails existing = DelReturnUnitWork.ReturnOutOrdersDetailsManager.GetById(item.ReturnOrderDetId);
                if (existing != null)
                {
                    DelReturnUnitWork.ReturnOutOrdersDetailsManager.Delete(existing);

                }
            }
        }

        private Boolean CheckItemDelete(long ReturnOrderDetId)
        {
            Boolean ret = true;
          UnitOfWork CurReturnUnitWork = new UnitOfWork();
          ReturnOutOrdersDetails existing = CurReturnUnitWork.ReturnOutOrdersDetailsManager.GetById(ReturnOrderDetId);
            if (existing != null)
            {
                int Difference = existing.OutOrdersDetails.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;

                }

            }
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(long ReturnOrderId)
        {
           
            List<ReturnOutOrdersDetails> ReturnOutOrdersDetailsList = CurUnitWork.ReturnOutOrdersDetailsManager.GetByOrderId(ReturnOrderId);
            Boolean ret = true;
            foreach (var item in ReturnOutOrdersDetailsList)
            {
                ret = CheckItemDelete(item.ReturnOrderDetId);
                if (!ret)
                {
                    ret = false;
                    break;
                }

            }
            return ret;
        }



    }

}