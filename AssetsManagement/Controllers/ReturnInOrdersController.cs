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
    public class ReturnInOrdersController : Controller
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
        public ActionResult ReturnInOrderList( string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string OrderId = null)
        {

            //List<Car> carList = null;
            if (!string.IsNullOrEmpty(OrderId))
            {
                ReturnInOrdersViewModel model = new ReturnInOrdersViewModel();
                long CurOrderId = long.Parse(OrderId);
                List<ReturnInOrders> ReturnInOrderList = unitWork.ReturnInOrdersManager.GetNotDelAllByOrderId(CurOrderId).OrderByDescending(m => m.ReturnOrderId).ToList();
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
                    ReturnInOrderList = unitWork.ReturnInOrdersManager.GetCastByName(Search_Data);
                    //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
                }
                switch (Sorting_Order)
                {
                    case "ReturnOrderId":
                        ReturnInOrderList = ReturnInOrderList.OrderByDescending(stu => stu.ReturnOrderId).ToList();
                        break;
                    case "ReturnOrderDate":
                        ReturnInOrderList = ReturnInOrderList.OrderBy(stu => stu.ReturnOrderDate).ToList();
                        break;

                    default:
                        ReturnInOrderList = ReturnInOrderList.OrderByDescending(stu => stu.ReturnOrderId).ToList();
                        //carList =
                        break;
                }


                int Size_Of_Page = 15;
                int No_Of_Page = (Page_No ?? 1);
                model.ReturnInOrders = ReturnInOrderList.ToPagedList(No_Of_Page, Size_Of_Page);
                if (ReturnInOrderList.Any())
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
        [ActionName("AddEditReturnInOrder")]
        public ActionResult AddEditReturnInOrder(string InOrderId, string Id = null)
        {
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            ReturnInOrdersViewModel model = new ReturnInOrdersViewModel();

            //model.ItemsPopulateList(model.FromEmpId, null,1);


            if (String.IsNullOrEmpty(Id) || Id=="0")
            {
                ModelState.Clear();
                ReturnInOrders UnitRecord = new ReturnInOrders();
                UnitRecord.ReturnOrderDate = DateTime.Today;
                model.SelectedItem = UnitRecord;
                model.SelectedItem.InOrderId  =long.Parse(InOrderId);

                model.EditMode = "Add";
                model.DisplayMode = "ReadOnly";
                InOrders InOrdersList = unitWork.InOrdersManager.GetByOrderId(long.Parse(InOrderId));
                model.SelectedItem.InOrders = InOrdersList;
               

                List<ReturnInOrdersDetails> ReturnInOrdersDetailsList = new List<ReturnInOrdersDetails>();
               
                model.ReturnInOrdersDetails = ReturnInOrdersDetailsList;
                //model.SelectedItem.TransferFlag = 1; // personal ReturnInOrder
                //return View(model);
            }
            else
            {
                // Edit record (view in Edit mode)
                int OrderId = int.Parse(Id);

                //model.Buildings = unitWork.ReturnInOrdersManager.GetNotDelAll().OrderBy(m => m.ReturnOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                model.SelectedItem = unitWork.ReturnInOrdersManager.GetById(OrderId);

                if (model.SelectedItem == null) {
                    return RedirectToAction("ReturnInOrderList", new { OrderId = OrderId});
                }

                model.DisplayMode = "ReadWrite";
                model.EditMode = "Edit";

                InOrders InOrdersList = unitWork.InOrdersManager.GetByOrderId(long.Parse(InOrderId));
                model.SelectedItem.InOrders = InOrdersList;

                List<ReturnInOrdersDetails> ReturnInOrdersDetailsList = unitWork.ReturnInOrdersDetailsManager.GetByOrderId(OrderId);
                
                model.ReturnInOrdersDetails = ReturnInOrdersDetailsList;
                


                // ...
                //return View( model);
            }

            return GetData(model, "", "", "", 0);

        }

        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(ReturnInOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }
        private bool CheckEnoughQty(ReturnInOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.ReturnInOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                ReturnInOrdersDetails ReturnInOrdersDetailsObj = unWork.ReturnInOrdersDetailsManager.GetById(item.ReturnOrderDetId);
                if (ReturnInOrdersDetailsObj != null)
                {
                    OldQty = ReturnInOrdersDetailsObj.Qty.GetValueOrDefault();
                }

                tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                if (ItemObj.CatSub_tbl.GenerateBarcodeFlag)
                {
                    if (ItemObj.CatSub_tbl.GenerateBarcodeFlag) // get current Item Quantity in inventory
                    {
                        List<tbl_ItemsStockDetails> ItemsStockDetailsList = unWork.ItemsStockDetailsManager.GetByInOrderDetId(item.InOrderDetId.GetValueOrDefault());
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
                InOrdersDetails InOrdersDetailsObj = unWork.InOrdersDetailsManager.GetById(item.InOrderDetId);
                if (item.Qty.GetValueOrDefault() > InOrdersDetailsObj.Qty.GetValueOrDefault()) // القيمه اكبر من الكمية الموجود اصلا فى السند
                {
                    ret = false;
                }
                else
                {
                    int DiffQty = (TotalQty + OldQty)-item.Qty.GetValueOrDefault(); // نتاكد ان الكمية الحالية لا تصبح سالب
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
        private bool SaveReturnInOrder(ReturnInOrdersViewModel Mymodel)
        {
            bool ret = true;
            string ItemName = "";
            if (!CheckEnoughQty(Mymodel,ref ItemName))
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.InSufficientReturnQtyMsg + " " + ItemName; ;
                TempData["Message"] = null;

            }
            //checked user select invetory
            //if (Mymodel.SelectedItem.StoreId_To != null)
            //{
            //    long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
            //}
            //else
            //{
            //    ret = false; // end flag
            //}
            if (Mymodel.ReturnInOrdersDetails.Count == 0)
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
                    ReturnInOrders ReturnInOrdersbj = unitWork.ReturnInOrdersManager.Add(Mymodel.SelectedItem);
                    if (ReturnInOrdersbj != null)
                    {
                        Mymodel.ReturnInOrdersDetails = Mymodel.ReturnInOrdersDetails;
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
                    bool bret = unitWork.ReturnInOrdersManager.Update(Mymodel.SelectedItem);
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

        private bool SaveOrderItems(ReturnInOrdersViewModel Mymodel)
        {
            bool ret = false;
            foreach (var item in Mymodel.ReturnInOrdersDetails)
            {

                item.InOrdersDetails  = unitWork.InOrdersDetailsManager.GetById(item.InOrderDetId);
                if (item.ReturnOrderDetId == 0)
                {
                    // insert new record

                    item.ReturnOrderId = Mymodel.SelectedItem.ReturnOrderId;
                    ReturnInOrdersDetails ReturnInOrderItemsbj = unitWork.ReturnInOrdersDetailsManager.Add(item);
                    if (ReturnInOrderItemsbj != null)
                    {
                        //Mymodel.ReturnInOrdersDetails = Mymodel.ReturnInOrdersDetails;
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
                    bool bret = unitWork.ReturnInOrdersDetailsManager.Update(item);
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
        public ActionResult AddEditReturnInOrder(ReturnInOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            bool ScanFlag = false;
            string SaveValue = Request.Form["CmdSave"];
            //string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];
            

            if (Mymodel.PostFlag == 3)
            {
                List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                foreach (var item in Mymodel.SelectedItem.InOrders.InOrdersDetails)
                {
                    //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                    bool IsSelected = false;
                    CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.InOrderDetId, Name = "Item" + item.InOrderDetId.ToString(), ItemId = item.InOrderDetId };
                    ItemsCheckBoxList.Add(CheckItem);
                }
                Mymodel.ItemsScanCheckList = ItemsCheckBoxList;

            }
            else if (SaveValue == "CmdSave")
            {

                if (SaveReturnInOrder(Mymodel))
                {
                   
                        return RedirectToAction("AddEditReturnInOrder", new { InOrderId= Mymodel.SelectedItem.InOrderId, id = Mymodel.SelectedItem.ReturnOrderId });
                }
                else
                {
                    //return View(Mymodel);
                    //return GetData(Mymodel, "", "", "", 0);
                    return RedirectToAction("AddEditReturnInOrder", new { InOrderId = Mymodel.SelectedItem.InOrderId, id = Mymodel.SelectedItem.ReturnOrderId });
                }
              
            }
            // if make scan item from scan device
            
            else if (AddValue == "AddItem")
            {
                int i = 0;
                ScanFlag = false;
                if (Mymodel.SelectedItem.InOrders == null)
                {
                    InOrders InOrdersList = unitWork.InOrdersManager.GetByOrderId(Mymodel.SelectedItem.InOrderId.GetValueOrDefault());
                    Mymodel.SelectedItem.InOrders = InOrdersList;
                }
                List<InOrdersDetails> InOrdersDetailsList = Mymodel.SelectedItem.InOrders.InOrdersDetails.ToList();
                foreach (var item in Mymodel.ItemsScanCheckList)
                {
                    if (item.IsSelected)
                    {
                        long ItemId = InOrdersDetailsList[i].ItemId.GetValueOrDefault();
                        string ItemRFID = InOrdersDetailsList[i].Item_RFID;
                        long StoreId = InOrdersDetailsList[i].StoreId.GetValueOrDefault();
                        long StockId = InOrdersDetailsList[i].StockId.GetValueOrDefault ();
                        string BarCode = InOrdersDetailsList[i].Item_BarCode;
                       
                            List<ReturnInOrdersDetails> RowsReturnInOrdersDetails = new List<ReturnInOrdersDetails>();
                            if (Mymodel.ReturnInOrdersDetails != null)
                            {
                                RowsReturnInOrdersDetails = Mymodel.ReturnInOrdersDetails.Where(c => c.StockId == StockId).ToList();
                            }
                            else
                            {
                                Mymodel.ReturnInOrdersDetails = new List<ReturnInOrdersDetails>();
                            }
                            if (RowsReturnInOrdersDetails.Count == 0)
                            {
                                if (InOrdersDetailsList[i].Qty > 0)
                                {
                                    ReturnInOrdersDetails NewItem = new ReturnInOrdersDetails();
                                //NewItem.ItemId = ItemId;
                              
                                    InOrdersDetails RowsInOrdersDetails = unitWork.InOrdersDetailsManager.GetById (InOrdersDetailsList[i].InOrderDetId);
                                    NewItem.InOrdersDetails = RowsInOrdersDetails;
                                    NewItem.InOrderDetId = InOrdersDetailsList[i].InOrderDetId;
                                
                                    NewItem.StockId = StockId;
                                    NewItem.Qty = 1;
                                //NewItem.Item_BarCode = BarCode;
                                List <tbl_ItemsStockDetails> ItemsStockDetailsList = unitWork.ItemsStockDetailsManager.GetByInOrderDetId(InOrdersDetailsList[i].InOrderDetId);
                                if (ItemsStockDetailsList.Count > 0)
                                {
                                    NewItem.InStoreItemQty = ItemsStockDetailsList.FirstOrDefault().Qty;
                                }
                                NewItem.InStoreQty = InOrdersDetailsList[i].tbl_ItemsStock.ItemQty;//.InStoreQty;
                                    
                                    NewItem.StoreId = StoreId;
                                    Mymodel.ReturnInOrdersDetails.Add(NewItem);
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
                    if (Mymodel.ReturnInOrdersDetails != null)
                    {
                        long ReturnOrderDetId = Mymodel.ReturnInOrdersDetails[index].ReturnOrderDetId;
                        //long ItemId = Mymodel.ReturnInOrdersDetails[index].ItemId.GetValueOrDefault();
                        //string Item_RFID = Mymodel.ReturnInOrdersDetails[index].Item_RFID;

                        ReturnInOrdersDetails existing = unitWork.ReturnInOrdersDetailsManager.GetById(ReturnOrderDetId);
                        if (existing != null)
                        {
                            if (CheckItemDelete(existing))//.ReturnOrderDetId))
                            {
                                unitWork.ReturnInOrdersDetailsManager.Delete(existing);
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.DeleteItemInOrderMsg;
                                DelFalg = false;
                            }
                        }
                        Mymodel.ReturnInOrdersDetails.RemoveAt(index);

                    }
                    if (Mymodel.SelectedItem.InOrders == null)
                    {
                        Mymodel.SelectedItem.InOrders = unitWork.InOrdersManager.GetByOrderId(Mymodel.SelectedItem.InOrderId.GetValueOrDefault());
                        
                    }
                    Mymodel.ReturnInOrdersDetails = unitWork.ReturnInOrdersDetailsManager.GetByOrderId(Mymodel.SelectedItem.ReturnOrderId);
                   
                }

            }
            //else if (Mymodel.ReaderType == 1)
            //{
            //    ScanFlag = true;
            //}
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }

        public ActionResult GetData(ReturnInOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            ReturnInOrdersViewModel model = new ReturnInOrdersViewModel();
            model = Mymodel;
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();

            //model.ReturnInOrdersDetails = Mymodel.ReturnInOrdersDetails ;
            //model.SelectedItem.InOrders = Mymodel.SelectedItem.InOrders;

            //model.FromStoreId = Mymodel.SelectedItem.InOrderId;
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
   

            //if (model.SelectedItem.InOrders == null)
            //{
            //    model.SelectedItem.InOrders = unitWork.InOrdersManager.GetByOrderId(Mymodel.SelectedItem.InOrderId.GetValueOrDefault());
            //}
           
                List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                foreach (var item in Mymodel.SelectedItem.InOrders.InOrdersDetails)
                {
                List <tbl_ItemsStockDetails> ItemsStockDetails =unitWork.ItemsStockDetailsManager.GetByInOrderDetId(item.InOrderDetId);
                if (ItemsStockDetails.Count  > 0)
                {
                    item.ItemInStoreQty = ItemsStockDetails.FirstOrDefault().Qty;
                }
                //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                bool IsSelected = false;
                    CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.InOrderDetId, Name = "Item" + item.InOrderDetId.ToString(), ItemId = item.InOrderDetId };
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
            

            foreach (var item in Mymodel.ReturnInOrdersDetails)
            {

                //InOrdersDetails RowsInOrdersDetails1 = unitWork.InOrdersDetailsManager.GetById(item.InOrderDetId);
                //item.InOrdersDetails = RowsInOrdersDetails1;
                if (item.InOrdersDetails.tbl_ItemsStock == null)
                {
                    item.InOrdersDetails.tbl_ItemsStock = unitWork.ItemsStockManager.GetById(item.StockId);
                }
                
                //item.tbl_ItemsStock.Item_tbl= unitWork.ItemsManager.GetById(item.ItemId);
            }
           

            //model.SelectedItem.InOrders= Mymodel.SelectedItem.InOrders;
            model.ReturnInOrdersDetails = Mymodel.ReturnInOrdersDetails;
           
            //ModelState.Clear();

            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditReturnInOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid

        }

      

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            ReturnInOrders existing = unitWork.ReturnInOrdersManager.GetById(Id);
            if (existing != null)
            {
                Boolean ret = true;
                if (existing.ReturnInOrdersDetails != null)
                {
                    ret = CheckDetailsBeforeDelete(existing.ReturnInOrdersDetails.ToList());
                }
                if (ret)
                {

                    //ReturnInOrders Newexisting = unitWork.ReturnInOrdersManager.GetById(Id);
                    if (existing.ReturnInOrdersDetails != null)
                    {
                        DeleteDetails(existing.ReturnInOrdersDetails.ToList());//Id); // delete order items
                    }
                    unitWork.ReturnInOrdersManager.Delete(existing);
                    

                    ReturnInOrdersViewModel model = new ReturnInOrdersViewModel();
                    model.ReturnInOrders = unitWork.ReturnInOrdersManager.GetNotDelAll().OrderByDescending(m => m.ReturnOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                    model.SelectedItem = null;
                    model.DisplayMode = "";
                    TempData["warningMessage"] = null;
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteInOrderMsg;
                }
                return RedirectToAction("ReturnInOrderList", new { OrderId = id });
            }
            else
            {
                return RedirectToAction("ReturnInOrderList", new { OrderId = id });
            }
        }
        private void DeleteDetails(List<ReturnInOrdersDetails> ReturnInOrdersDetailsList)//int OrderId)
        {
            //List<ReturnInOrdersDetails> ReturnInOrdersDetailsList = unitWork.ReturnInOrdersDetailsManager.GetByOrderId(OrderId);
            foreach (var item in ReturnInOrdersDetailsList)
            {
                ReturnInOrdersDetails existing = unitWork.ReturnInOrdersDetailsManager.GetById(item.ReturnOrderDetId);
                if (existing != null)
                {
                    unitWork.ReturnInOrdersDetailsManager.Delete(existing);

                }
            }
        }

        private Boolean CheckItemDelete(ReturnInOrdersDetails existing)//long ReturnOrderDetId)
        {
            Boolean ret = true;
            //ReturnInOrdersDetails existing = unitWork.ReturnInOrdersDetailsManager.GetById(ReturnOrderDetId);
            if (existing != null)
            {
                int Difference = existing.InOrdersDetails.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;

                }

            }
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(List<ReturnInOrdersDetails> ReturnInOrdersDetailsList) //long ReturnOrderId)
        {
            //List<ReturnInOrdersDetails> ReturnInOrdersDetailsList = unitWork.ReturnInOrdersDetailsManager.GetByOrderId(ReturnOrderId);
            Boolean ret = true;
            foreach (var item in ReturnInOrdersDetailsList)
            {
                ret = CheckItemDelete(item);//.ReturnOrderDetId);
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