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
using System.Data.OleDb;
using System.Data;

namespace AssetsManagement.Controllers
{
    public class RequestOutOrdersController : Controller
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
        [UserPermissionAttribute(AllowFeature = "RequestOutOrders", AllowPermission = "Accessing")]
        public ActionResult RequestOutOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            int? ReqStatusId = null;
            int? SecondReqStatusId = null;
            //List<Car> carList = null;
            if (Session["RoleID"] != null)
            {
                //public int SuperAdminRole = 1;
                //public int SecretaryRole = 4;
                //public int Inventory_ManagerRole = 5;
                //public int SupervisorRole = 6;
                //public int StorekeeperRole = 7;
                int RoleId = int.Parse(Session["RoleID"].ToString());
                switch (RoleId)
                {
                    case BOL.DataModel.SecretaryRole:
                        ReqStatusId = BOL.DataModel.RequestLevel1;
                    break;
                    case BOL.DataModel.StorekeeperRole:
                        ReqStatusId = BOL.DataModel.RequestLevel1;
                        SecondReqStatusId= BOL.DataModel.RequestLevel4;
                        break;
                    case BOL.DataModel.Inventory_ManagerRole:
                        ReqStatusId = BOL.DataModel.RequestLevel2;
                        break;
                    case BOL.DataModel.SupervisorRole:
                        ReqStatusId = BOL.DataModel.RequestLevel3;
                        break;
                    
                    case BOL.DataModel.SuperAdminRole :
                        ReqStatusId = null;
                        break;
                }
            }
            RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            //List<RequestOutOrders> RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending (m => m.ReqOrderId).ToList();
            List<RequestOutOrders> RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAllByUserId(userId, ReqStatusId, SecondReqStatusId).OrderByDescending(m => m.ReqOrderId).ToList();
                      
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "ReqOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "ReqOrderId" ? "InOrderDate" : "ReqOrderId";

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
                //Buildings.Find()
                RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAllByUserId(userId, ReqStatusId, SecondReqStatusId, Search_Data).OrderByDescending(m => m.ReqOrderId).ToList(); 
            }
            switch (Sorting_Order)
            {
                case "InOrderId":
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.ReqOrderId).ToList();
                    break;
                case "ReqOrderDate":
                    RequestOutOrdersList = RequestOutOrdersList.OrderBy(stu => stu.ReqOrderDate).ToList();
                    break;
                case "DeptId":
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.DeptId).ToList();
                    break;
                default:
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.ReqOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.Page_No = No_Of_Page;
            model.RequestOutOrders = RequestOutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (RequestOutOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditRequestOutOrder")]
        public ActionResult AddEditRequestOutOrder(string Id = null)
        {
             
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();
            try
            {
                //model.ItemsPopulateList(model.FromEmpId, null,1);
                               
                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("RequestOutOrders", "Adding"))
                    {
                        ModelState.Clear();
                        RequestOutOrders UnitRecord = new RequestOutOrders();
                        UnitRecord.ReqOrderDate = DateTime.Today;
                        UnitRecord.ReqStatusId = BOL.DataModel.RequestLevel1;
                        model.SelectedItem = UnitRecord;
                        model.DisplayMode = "ReadOnly";
                        model.EditMode = "Add";
                        List<RequestOrdersDetails> RequestOutOrdersDetailsList = new List<RequestOrdersDetails>();
                        model.RequestOrdersDetails = RequestOutOrdersDetailsList;
                       
                                                             //return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");

                    }
                }
                else
                {
                    if (this.HasPermission("RequestOutOrders", "Updating"))
                    {
                        // Edit record (view in Edit mode)
                        int OrderId = int.Parse(Id);

                        //model.Buildings = unitWork.RequestOutOrdersManager.GetNotDelAll().OrderBy(m => m.InOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.RequestOutOrdersManager.GetByOrderId(OrderId);
                        model.DisplayMode = "ReadWrite";
                        model.EditMode = "Edit";
                        List<RequestOrdersDetails> RequestOutOrdersDetailsList = unitWork.RequestOrdersDetailsManager.GetByOrderId_MasterItem(OrderId);
                        model.RequestOrdersDetails = RequestOutOrdersDetailsList;
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
            return GetData(model, "", "", "", 0);

        }
     
        [HttpPost]
        public ActionResult AddEditRequestOutOrder(RequestOutOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, HttpPostedFileBase postedFile, HttpPostedFileBase ExcelFileUploader)
        {
            int ScanFlag = 0;
            string SaveValue = Request.Form["CmdSave"];
            string ManagerValue = Request.Form["CmdManager"];
            string SupervisorValue = Request.Form["CmdSupervisor"];
            string KeeperValue = Request.Form["CmdSKeeper"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];  
            string SelNewItem = Request.Form["SelNewItem"];
            string Reject = Request.Form["CmdReject"];

            if (Mymodel.PostFlag == 3 || SelNewItem == "SelNewItem")
            {


                if (Mymodel.SelItemId != null && SelNewItem == "SelNewItem" && Mymodel.SelQty != null)
                {
                    long SelItemId = Mymodel.SelItemId.GetValueOrDefault();
                    int SelQty = Mymodel.SelQty.GetValueOrDefault();
                    Item_tbl obj = unitWork.ItemsManager.GetById(Mymodel.SelItemId);

                    List<RequestOrdersDetails> RowsOutOrdersDetials = new List<RequestOrdersDetails>();
                    if (Mymodel.RequestOrdersDetails != null)
                    {

                        RowsOutOrdersDetials = Mymodel.RequestOrdersDetails.Where(c => c.ItemId == SelItemId).ToList();
                    }
                    else
                    {
                        Mymodel.RequestOrdersDetails = new List<RequestOrdersDetails>();
                    }

                    if (RowsOutOrdersDetials.Count == 0)
                    {
                        RequestOrdersDetails NewItem = new RequestOrdersDetails();
                        NewItem.CountableFlag = obj.CountableFlag;
                        NewItem.ItemId = obj.Item_Id;
                        NewItem.Qty = SelQty;

                        Mymodel.RequestOrdersDetails.Add(NewItem);
                        Mymodel.SelItemId = null;  
                        Mymodel.SelQty = null;
                    }
                    else
                    {
                        TempData["ScannerMessage"] = Resources.DefaultResource.DuplicateItemMsg;
                    }

                }
                else
                {
                    TempData["ScannerMessage"] = Resources.DefaultResource.SelectItemAndQtyMsg;
                }


            }

          
            // save  
            else if (SaveValue == "CmdSave")
            {
                if (Mymodel.SelectedItem.StoreId != null)
                {

                }

                bool ret = SaveReqOrder(Mymodel);

                if (ret)
                {
                    return RedirectToAction("AddEditRequestOutOrder", new { id = Mymodel.SelectedItem.ReqOrderId });
                }
                else
                {
                    return GetData(Mymodel, "", "", "", 0);
                    //return RedirectToAction("AddEditInOrder", Mymodel);
                }

            }
            // if make scan item from scan device
            else if (ManagerValue == "CmdManager")
            {

                bool ret = ManagerSave(Mymodel);
                if (ret)
                {
                    Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RequestLevel3;
                    bool retSave = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                    TempData["Message"] = Resources.DefaultResource.ApprovedRequestMessage;
                    TempData["warningMessage"] = null;
                    TempData["ScannerMessage"] = null;
                    
                    return RedirectToAction("RequestOutOrderList");
                }
                else
                {
                    return GetData(Mymodel, "", "", "", 0);
                }

            }
            else if (SupervisorValue == "CmdSupervisor")
            {
                Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RequestLevel4;
                bool retSave = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                TempData["Message"] = Resources.DefaultResource.ApprovedRequestMessage;
                TempData["warningMessage"] = null;
                TempData["ScannerMessage"] = null;
                return RedirectToAction("RequestOutOrderList");
            }
            else if (Reject == "CmdReject")
            {
                Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RejectStaus;
                bool retSave = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                TempData["Message"] = Resources.DefaultResource.RejectedRequestMessage;
                TempData["warningMessage"] = null;
                TempData["ScannerMessage"] = null;
                return RedirectToAction("RequestOutOrderList");
            }
            
            else if (KeeperValue == "CmdSKeeper")
            {
                bool ret = ManagerSave(Mymodel);
                if (ret)
                {
                    Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RequestLevel2;
                    bool retSave = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                    if (retSave )
                    {
                        List<RequestOrdersDetails>  List = unitWork.RequestOrdersDetailsManager.GetByOrderId(Mymodel.SelectedItem.ReqOrderId);
                        foreach (RequestOrdersDetails objItem in List )
                        {
                            unitWork.RequestOrdersDetailsManager.Update(objItem);
                        }
                    }
                    TempData["Message"] = Resources.DefaultResource.ApprovedRequestMessage;
                    TempData["warningMessage"] = null;
                    TempData["ScannerMessage"] = null;
                    return RedirectToAction("RequestOutOrderList");
                }
                else
                {
                    return GetData(Mymodel, "", "", "", 0);
                }
            }
            else if (DelValue == "CmdDel")
            {

                if (Id != null)
                {
                    //if (Mymodel.SelectedItem.IsLock.GetValueOrDefault())
                    //{

                    //    TempData["warningMessage"] = Resources.DefaultResource.InorderIslockMsg;
                    //    TempData["Message"] = null;
                    //}
                    //else
                    //{
                    int index = Id.GetValueOrDefault();
                    Boolean DelFalg = true;
                    if (Mymodel.RequestOrdersDetails != null)
                    {
                        long ReqOrderId = Mymodel.RequestOrdersDetails[index].ReqOrderId.GetValueOrDefault();

                        long InOrderDetId = Mymodel.RequestOrdersDetails[index].ReqOrderDetId;
                        long ItemId = Mymodel.RequestOrdersDetails[index].ItemId.GetValueOrDefault();
                        //delete Master

                        RequestOrdersDetails existing = unitWork.RequestOrdersDetailsManager.GetByOrderDetId(InOrderDetId);
                        if (existing != null)
                        {
                  
                            unitWork.RequestOrdersDetailsManager.Delete(existing);
                           List <RequestOrdersDetails> List = unitWork.RequestOrdersDetailsManager.GetByItemId(ItemId, ReqOrderId);
                            foreach (RequestOrdersDetails item in List)
                            {
                                unitWork.RequestOrdersDetailsManager.Delete(item);
                            }

                        }
                        if (DelFalg)
                        {
                            Mymodel.RequestOrdersDetails.RemoveAt(index);
                        }
                    }
                    // }

                }

            }
            else if (Mymodel.DisplayMode == "StoreId")
            {
                bool bret = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                long StoreId = Mymodel.SelectedItem.StoreId.GetValueOrDefault();

                List<RequestOrdersDetails> List = unitWork.RequestOrdersDetailsManager.GetByOrderId(Mymodel.SelectedItem.ReqOrderId);
                foreach (RequestOrdersDetails item in List)
                {

                    bool CountableFlag = false;
                    if (item.CountableFlag != null)
                    {
                        CountableFlag = item.CountableFlag.GetValueOrDefault();
                    }
                    if (CountableFlag)
                    {
                        tbl_ItemsStock Obj = unitWork.ItemsStockManager.GetByItemId(item.ItemId.GetValueOrDefault(), StoreId);
                        if (Obj != null)
                        {
                            item.StockId = Obj.StockId;
                        }
                    }
                    item.StoreId = StoreId;
                    unitWork.RequestOrdersDetailsManager.Update(item);
                }
                Mymodel.RequestOrdersDetails = List.Where(c => c.MasterItemFalg == true).ToList();
                Mymodel.DisplayMode = "";
            }

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }
        [HttpPost]
        public ActionResult DeleteItemsOrder(RequestOutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }
        private bool SaveReqOrder(RequestOutOrdersViewModel Mymodel)
        {
            bool ret = true;
            try
            {
                //if (CheckExpiredDate(Mymodel))
                //{
                    //checked user select invetory
                    if (Mymodel.SelectedItem.DeptId != null)
                    {
                        long DeptId = Mymodel.SelectedItem.DeptId.GetValueOrDefault();
                    }
                    else
                    {
                        ret = false; // end flag
                    }

                    if (Mymodel.RequestOrdersDetails.Count == 0) // check if user add details items
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                        TempData["Message"] = null;
                    TempData["ScannerMessage"] = null;
                }
                    
                   
                    if (ret)
                    {
                        //if (ModelState.IsValid)
                        //{
                        // insert new In order
                        if (Mymodel.SelectedItem.ReqOrderId == 0)
                        {

                        // insert new record
                        //Mymodel.SelectedItem.IsLock = false;
                        Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RequestLevel1;
                        RequestOutOrders RequestOutOrdersbj = unitWork.RequestOutOrdersManager.Add(Mymodel.SelectedItem);

                            if (RequestOutOrdersbj != null)
                            {
                                Mymodel.RequestOrdersDetails = Mymodel.RequestOrdersDetails;
                                TempData["Message"] = Resources.DefaultResource.SaveMessage; //"Success";
                                TempData["warningMessage"] = null;
                               TempData["ScannerMessage"] = null;
                        }
                            else
                            {
                                TempData["Message"] = null;
                            }


                            //ModelState.Clear();
                        }
                        else // update old in order
                        {

                            bool bret = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);

                            // update record
                        }

                        if (SaveOrderItems(Mymodel))
                        {
                            TempData["Message"] = Resources.DefaultResource.SaveMessage;
                            TempData["ScannerMessage"] = null;
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
                //    ret = false;
                //    TempData["warningMessage"] = Resources.DefaultResource.ExpiredDateInvalidMsg;
                //    TempData["Message"] = null;
                //}
            }
            catch (Exception ex)
            {
                ret = false;
                TempData["warningMessage"] = Resources.DefaultResource.ErrorMessage;

            }
            return ret;
        }
        // check enough quantity to In

        private bool ManagerSave(RequestOutOrdersViewModel Mymodel)
        {
            bool ret = true;
            try
            {
                long StoreId = 0;
                if (Mymodel.SelectedItem.StoreId != null)
                {
                    StoreId = Mymodel.SelectedItem.DeptId.GetValueOrDefault();
                    bool bret = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                    if (SaveOrderItems(Mymodel))
                    {
                        TempData["Message"] = Resources.DefaultResource.SaveMessage;
                        TempData["ScannerMessage"] = null;
                    }
                    else
                    {
                        TempData["Message"] = null;
                        ret = false;
                    }
                }
                else
                {
                    ret = false; // end flag
                    TempData["warningMessage"] = Resources.DefaultResource.ChooseInventoryMsg;
                    TempData["Message"] = null;
                    TempData["ScannerMessage"] = null;
                }
               
                if (ret)
                {
                    // insert new In order
                    if (Mymodel.SelectedItem.ReqOrderId > 0)
                    {                        
                        if (Mymodel.SelectedItem != null)
                        {
                            string ItemName = "";
                            string StockQty = "";
                            ret = CheckEnoughQty(Mymodel, ref ItemName,ref StockQty);
                            if (ret)
                            {
                                //Mymodel.SelectedItem.ReqStatusId = BOL.DataModel.RequestLevel2;
                                //bool bret = unitWork.RequestOutOrdersManager.Update(Mymodel.SelectedItem);
                                //TempData["Message"] = Resources.DefaultResource.ApprovedRequestMessage; //"Success";
                            }
                            else
                            {
                                TempData["warningMessage"] =Resources.DefaultResource.LessQtyInStock +" "+ ItemName+" "+ Resources.DefaultResource.CurrentItemQuantity+"="+ StockQty;
                                TempData["Message"] = null;
                                TempData["ScannerMessage"] = null;
                            }
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.ChooseAssetItemsMessage;
                            TempData["Message"] = null;
                            TempData["ScannerMessage"] = null;
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
        private bool CheckEnoughQty(RequestOutOrdersViewModel Mymodel, ref string ItemName, ref string StockQty)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            long  StoreId = Mymodel.SelectedItem.StoreId.GetValueOrDefault();
            foreach (var item in Mymodel.RequestOrdersDetails) //countable item
            {
                int OldQty = 0;
                int TotalQty = 0;
                int Qty = 0;
                // get old quantity of item
                RequestOrdersDetails RequestOutOrdersDetailsObj = unWork.RequestOrdersDetailsManager.GetById(item.ReqOrderDetId);
                long ItemId = item.ItemId.GetValueOrDefault();
                OldQty = RequestOutOrdersDetailsObj.Qty.GetValueOrDefault();
                List<tbl_ItemsStock> StockList = unWork.ItemsStockManager.GetByAllItemId(ItemId, StoreId);
                //tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                if (StockList.Count > 0)
                {
                    tbl_ItemsStock StockObj = StockList.FirstOrDefault();
                    Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                    bool CountableFlag = ItemObj.CountableFlag;
                    if (CountableFlag) // لو كان الصنف نثري
                    {
                        if (ItemObj.CatSub_tbl.GenerateBarcodeFlag)
                        {
                            if (ItemObj.CatSub_tbl.GenerateBarcodeFlag) // get current Item Quantity in inventory
                            {
                                List<tbl_ItemsStockDetails> ItemsStockDetailsList = unWork.ItemsStockDetailsManager.GetByInOrderDetId(item.ReqOrderDetId);
                                if (ItemsStockDetailsList.Count > 0)
                                {
                                    TotalQty = ItemsStockDetailsList.FirstOrDefault().Qty.GetValueOrDefault();
                                }
                            }
                        }
                        else // get current Quantity in inventory
                        {
                            TotalQty = StockObj.ItemQty.GetValueOrDefault();
                        }
                        //get value of quantity in current inorder
                        int DiffQty = TotalQty - (item.Qty.GetValueOrDefault());// - OldQty); // نتاكد ان الكمية الحالية لا تصبح سالب
                        if (DiffQty < 0)
                        {

                            ret = false;
                            StockQty = TotalQty.ToString ();
                            ItemName = StockObj.ItemName;
                            break;
                        }
                    }
                    else // لو كان الصنف غير نثري
                    {
                        if (item.StockId == null)
                        {
                            ret = false;
                            ItemName = Resources.DefaultResource.DefineAssetItemsMessage;
                            break;
                        }
                        else
                        {
                            List < RequestOrdersDetails> RequestOrdersDetailsList=  unWork.RequestOrdersDetailsManager.GetByAllItemId(item.ItemId.GetValueOrDefault(),item.ReqOrderId.GetValueOrDefault());
                            foreach (RequestOrdersDetails CItem in RequestOrdersDetailsList)
                            {
                                if (CItem.StockId == null)
                                {
                                    ret = false;
                                    ItemName = Resources.DefaultResource.DefineAssetItemsMessage; 
                                    break;
                                }
                                else
                                {
                                    int TotCount = RequestOrdersDetailsList.Count;
                                    if (StockList.Count < TotCount )
                                    {
                                        ret = false;
                                        ItemName = Resources.DefaultResource.QtyinsufficientMessage;
                                        break;
                                    }
                                    bool isValid = RequestOrdersDetailsList.Select(c => c.StockId.ToString()).Distinct().Count() == TotCount ;

                                    if (!isValid)
                                    {
                                        ret = false;
                                        ItemName = Resources.DefaultResource.BarcodedifferenceMessage;
                                        break;
                                    }
                                }
                            }
                            if (!ret)
                            {
                                break;
                            }
                        }
                    }

                }
            }
            return ret;
        }
        private bool SaveOrderItems(RequestOutOrdersViewModel Mymodel)
        {
            bool ret = false;
            int i = 0;
            foreach (var item in Mymodel.RequestOrdersDetails)
            {
               
                Item_tbl ItemObj = unitWork.ItemsManager.GetById(item.ItemId); // main stock
                bool CountableFlag = true;
                if (ItemObj !=null)
                {
                    CountableFlag = ItemObj.CountableFlag;
                }
              
                if (item.ReqOrderDetId == 0) // add new record of InorderDetails
                {
                    // insert new record
                    RequestOrdersDetails InOrderItemsbj=null;
                    item.ReqOrderId = Mymodel.SelectedItem.ReqOrderId ;
                    item.ItemQty = item.Qty;
                    if (!CountableFlag)
                    {
                        
                        item.MasterItemFalg = true;
                        int No = item.Qty.GetValueOrDefault();
                        item.ItemQty = 1;
                        InOrderItemsbj = unitWork.RequestOrdersDetailsManager.Add(item);

                        int j = 0;
                        for (j=0; j< No-1; j++)
                        {
                            item.MasterItemFalg = false;
                            InOrderItemsbj = unitWork.RequestOrdersDetailsManager.Add(item);
                        }
                    }
                    else
                    {
                        item.MasterItemFalg = true;
                       
                        InOrderItemsbj = unitWork.RequestOrdersDetailsManager.Add(item);

                    }
                    if (InOrderItemsbj != null)
                    {
                        //Mymodel.RequestOutOrdersDetails = Mymodel.RequestOutOrdersDetails;
                        TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
                        TempData["ScannerMessage"] = null;
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
                    bool bret = false;
                    item.ReqOrderId = Mymodel.SelectedItem.ReqOrderId;
                    if (!CountableFlag && item.MasterItemFalg.GetValueOrDefault())
                    {
                        List<RequestOrdersDetails>  DetailsOrdersList = unitWork.RequestOrdersDetailsManager.GetByItemId(item.ItemId.GetValueOrDefault(), item.ReqOrderId.GetValueOrDefault()).ToList();
                        if (DetailsOrdersList.Count == item.Qty-1)
                        {

                        }
                        else
                        {
                            int dif = (item.Qty.GetValueOrDefault()-1)-DetailsOrdersList.Count;
                            if (dif>0)//add item
                            {
                                for (int j = 0; j < dif; j++)
                                {
                                    item.MasterItemFalg = false;
                           
                                    unitWork.RequestOrdersDetailsManager.Add(item);
                                }
                            }
                            else // removeitem
                            {
                                dif = -1 * dif;
                                for (int j = 0; j < dif; j++)
                                {
                                    RequestOrdersDetails OldItem = DetailsOrdersList[j];
                                    unitWork.RequestOrdersDetailsManager.DeleteOld(OldItem);
                                }
                            }
                        }
                        RequestOrdersDetails DetailsOrdersObj = unitWork.RequestOrdersDetailsManager.GetByOrderDetId(item.ReqOrderDetId);
                        DetailsOrdersObj.Qty = item.Qty;
                        DetailsOrdersObj.ItemQty = 1;
                        DetailsOrdersObj.CountableFlag = CountableFlag;
                        bret = unitWork.RequestOrdersDetailsManager.Update(DetailsOrdersObj);
                        

                        
                    }
                    else
                    {
                    
                      
                        RequestOrdersDetails DetailsOrdersObj = unitWork.RequestOrdersDetailsManager.GetByOrderDetId (item.ReqOrderDetId);
                        DetailsOrdersObj.CountableFlag = CountableFlag;
                        if (!CountableFlag)
                        {
                            DetailsOrdersObj.ItemQty = 1;
                        }
                        else
                        {
                            DetailsOrdersObj.ItemQty = item.Qty;
                            DetailsOrdersObj.Qty = item.Qty;
                        }
                        bret = unitWork.RequestOrdersDetailsManager.Update(DetailsOrdersObj);
                    }
                    // update record
                    if (bret)
                    {
                        TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
                        TempData["warningMessage"] = null;
                        TempData["ScannerMessage"] = null;
                        ret = true;
                    }
                    else
                    {
                        TempData["Message"] = null;
                        ret = false;
                    }
                }
                i++;
            }
            return ret;

        }
      
    
        public ActionResult GetData(RequestOutOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            if (Mymodel != null)
            {
                model = Mymodel;
            }
            List<CatMain_tbl> MainCats = unitWork.CatMainManager.GetUserMainCats(userId).ToList();
            model.Items = unitWork.ItemsManager.GetNotDelAllByMainCat(MainCats).Select(option => new SelectListItem
            {
                Text = option.Item_Name,
                Value = option.Item_Id.ToString()
            });

            model.LocationsList = unitWork.BuildingsManager.GetNotDelAll().Select(option => new SelectListItem
            {
                Text = option.Building_Name ,
                Value = option.Building_Id.ToString()
            });

            model.StoresList = unitWork.RoomsManager.GetInventoriesAllNoEmp().Select(option => new SelectListItem
            {
                Text = option.Room_Name,
                Value = option.Room_Id.ToString()
            });

           
            model.DeptsList = unitWork.DepartmentManager.GetUserDepts(userId).Select(option => new SelectListItem
            {
                Text = option.Name,
                Value = option.Id.ToString()
            });


            model.SelectedItem = Mymodel.SelectedItem;
            model.SelQty  = Mymodel.SelQty;
            model.SelItemId = Mymodel.SelItemId;
            
            model.EditMode = Mymodel.EditMode;
         

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
          
            //List<CheckBoxListItem> ItemsSelectList = new List<CheckBoxListItem>();

            foreach (var item in Mymodel.RequestOrdersDetails)
            {
                item.Item_tbl = unitWork.ItemsManager.GetById(item.ItemId);

                
            }
          
            model.RequestOrdersDetails = Mymodel.RequestOrdersDetails;
            ModelState.Clear();
            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
       
      
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            RequestOutOrders existing = unitWork.RequestOutOrdersManager.GetByOrderId (Id); //GetByOrderId(Id);
            if (existing != null)
            {
                //if (existing.RequestOrdersDetails.Count == 0)
                //{
                    //if (CheckDetailsBeforeDelete(Id))
                    //{
                        unitWork.RequestOutOrdersManager.Delete(existing);
                        DeleteDetails(Id); // delete order items

                        RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();
                        model.RequestOutOrders = unitWork.RequestOutOrdersManager.GetNotDelAll().OrderByDescending(m => m.ReqOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                        model.SelectedItem = null;
                        model.DisplayMode = "";
                        TempData["warningMessage"] = null;
                       // return RedirectToAction("InOrderList");
                    //}
                    //else
                    //{
                    //    TempData["warningMessage"] = Resources.DefaultResource.DeleteInOrderMsg;
                        
                    //}
              //  }
               
                //else
                //{
                //    TempData["warningMessage"] = Resources.DefaultResource.DeleteOutOrderMsg;
                //    //return RedirectToAction("InOrderList");

                //}
            }
            
            return RedirectToAction("RequestOutOrderList");
        }
        private Boolean CheckItemDelete(long InOrderDetId)
        {
            Boolean ret = true;
            RequestOrdersDetails existing = unitWork.RequestOrdersDetailsManager.GetByOrderDetId(InOrderDetId);
            if (existing != null)
            {
                //int Difference = existing.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                //if (Difference < 0)
                //{
                //    ret = false;
                   
                //}
                //List <ReturnRequestOutOrdersDetails> ReturnRequestOutOrdersDetail = unitWork.ReturnRequestOutOrdersDetailsManager.GetOrderByDetId(InOrderDetId);
                //if (existing.ReturnRequestOutOrdersDetails.Count > 0)
                //{
                //    ret = false;
                //}
            }
            
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(long OrderId)
        {
            List<RequestOrdersDetails> RequestOutOrdersDetailsList = unitWork.RequestOrdersDetailsManager.GetByOrderId(OrderId);
            Boolean ret=true;
            foreach (var item in RequestOutOrdersDetailsList)
            {
                ret=CheckItemDelete(item.ReqOrderDetId);
                if (!ret)
                {
                    ret = false;
                    break;
                }
                
            }
            return ret;
        }
        private void DeleteDetails(int OrderId)
        {
            List<RequestOrdersDetails> RequestOutOrdersDetailsList = unitWork.RequestOrdersDetailsManager.GetByOrderId(OrderId);
            
            foreach (var item in RequestOutOrdersDetailsList)
            {
                RequestOrdersDetails existing = unitWork.RequestOrdersDetailsManager.GetByOrderDetId (item.ReqOrderDetId);
                if (existing != null)
                {
                    unitWork.RequestOrdersDetailsManager.Delete(existing);

                }
            }
        }
        [HttpGet]
        [ActionName("ShowOrderDetails")]
        public ActionResult ShowOrderDetails(string id = null, string StoreId=null)
        {

            RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();

                
            if (String.IsNullOrEmpty(id))
            {

                //OutOrdersDetails OutOrdersDetailsRecord = new OutOrdersDetails();
                //model.SelectedUnit = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("RequestDetailsOrderModal", model.RequestOrdersDetails);
            }
            else
            {
                int Size_Of_Page = 30;
                // Edit record (view in Edit mode)
                long OrderDetId = long.Parse(id);
                long CurStoreId = long.Parse(StoreId);
                RequestOrdersDetails Item = unitWork.RequestOrdersDetailsManager.GetByOrderDetId(OrderDetId);
                if (Item != null)
                {
                    model.StockBarCodeList = unitWork.ItemsStockManager.GetByAllItemId(Item.ItemId.GetValueOrDefault(), CurStoreId).Select(option => new SelectListItem
                    {
                        Text = option.Item_BarCode,
                        Value = option.StockId.ToString()
                    });

                    model.RequestOrdersDetails = unitWork.RequestOrdersDetailsManager.GetByAllItemId(Item.ItemId.GetValueOrDefault(), Item.ReqOrderId.GetValueOrDefault()).OrderBy(m => m.ReqOrderDetId).ToList();
                }
                //model.OutOrders = OutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);

                //model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                model.DisplayMode = "ReadOnly";
                //if (model.SelectedUnit == null) { return View("_error"); }
                // ...
                return PartialView("RequestDetailsOrderModal", model);
            }

        }

        [HttpPost]
        [ActionName("ShowOrderDetails")]
        public ActionResult ShowOrderDetails(RequestOutOrdersViewModel model)
        {
            long ReqOrderId = 0;
            if (model.RequestOrdersDetails != null)
            {
                foreach (RequestOrdersDetails item in model.RequestOrdersDetails)
                {
                    unitWork.RequestOrdersDetailsManager.Update(item);
                    ReqOrderId = item.ReqOrderId.GetValueOrDefault();
                }
            }
            
             return RedirectToAction("AddEditRequestOutOrder", "RequestOutOrders", new { id = ReqOrderId });

        }
        [UserPermissionAttribute(AllowFeature = "RequestOutOrders", AllowPermission = "Accessing")]
        public ActionResult RequestOutOrderFollowList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
                       
            RequestOutOrdersViewModel model = new RequestOutOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            //List<RequestOutOrders> RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending (m => m.ReqOrderId).ToList();
            List<RequestOutOrders> RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAll().OrderByDescending(m => m.ReqOrderId).ToList();


            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "ReqOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "ReqOrderId" ? "InOrderDate" : "ReqOrderId";

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
            //if (!String.IsNullOrEmpty(Search_Data))
            //{
            //    //carList = Buildings.Where(stu => stu.Carid == 61);
            //    //carList = carList.Where(stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            //    //Buildings.Find()
            //    RequestOutOrdersList = unitWork.RequestOutOrdersManager.GetNotDelAllByUserId(userId, ReqStatusId, Search_Data).OrderByDescending(m => m.ReqOrderId).ToList();
            //    //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            //}
            switch (Sorting_Order)
            {
                case "InOrderId":
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.ReqOrderId).ToList();
                    break;
                case "ReqOrderDate":
                    RequestOutOrdersList = RequestOutOrdersList.OrderBy(stu => stu.ReqOrderDate).ToList();
                    break;
                case "DeptId":
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.DeptId).ToList();
                    break;
                default:
                    RequestOutOrdersList = RequestOutOrdersList.OrderByDescending(stu => stu.ReqOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.Page_No = No_Of_Page;
            model.RequestOutOrders = RequestOutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (RequestOutOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("ShowRequestDetails")]
        public ActionResult ShowRequestDetails(string id = null)
        {

            RequestOrdersDetailsViewModel model = new RequestOrdersDetailsViewModel();
            if (String.IsNullOrEmpty(id))
            {

                //OutOrdersDetails OutOrdersDetailsRecord = new OutOrdersDetails();
                //model.SelectedUnit = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("RequestDetailsOrderModal", model.RequestOrdersDetails);
            }
            else
            {
                // Edit record (view in Edit mode)
                long OrderId = long.Parse(id);

                int Size_Of_Page = 30;

                model.RequestOrdersDetails = unitWork.RequestOrdersDetailsManager.GetByOrderId(OrderId).OrderBy(m => m.ReqOrderDetId).ToPagedList(No_Of_Page, Size_Of_Page);

                //model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                model.DisplayMode = "ReadOnly";
                //if (model.SelectedUnit == null) { return View("_error"); }
                // ...
                return PartialView("RequestDetailsOrderModal", model);
            }

        }
    }

}