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
    public class InOrdersController : Controller
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
        [UserPermissionAttribute(AllowFeature = "InOrders", AllowPermission = "Accessing")]
        public ActionResult InOrderList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            InOrdersViewModel model = new InOrdersViewModel();
            int userId = SesssionUser.GetCurrentUserId();
            List<InOrders> InOrdersList = unitWork.InOrdersManager.GetNotDelAllByUserId(userId).OrderByDescending (m => m.InOrderId).ToList();
            
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "InOrderId" : "";
            ViewBag.SortingModel = Sorting_Order == "InOrderId" ? "InOrderDate" : "StoreId";

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
                InOrdersList = unitWork.InOrdersManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "InOrderId":
                    InOrdersList = InOrdersList.OrderByDescending(stu => stu.InOrderId).ToList();
                    break;
                case "InOrderDate":
                    InOrdersList = InOrdersList.OrderBy(stu => stu.InOrderDate).ToList();
                    break;
                case "StoreId":
                    InOrdersList = InOrdersList.OrderByDescending(stu => stu.StoreId_To).ToList();
                    break;
                default:
                    InOrdersList = InOrdersList.OrderByDescending(stu => stu.InOrderId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            ViewBag.Page_No = No_Of_Page;
            model.InOrders = InOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);
            if (InOrdersList.Any())
            {
                return View(model);
            }
            else
            {
                return View();
            }

        }
        [HttpGet]
        [ActionName("AddEditInOrder")]
        public ActionResult AddEditInOrder(string Id = null, int ScanFlag = 0)
        {
             
            //ViewBag.DeleteCar = TempData["Del"];
            //var CurList = unitWork.modelmanager.GetAll();
            //var CurStatusList = unitWork.StatusManager.GetAll();
            //ViewBag.CarModelId = new SelectList(CurList, "ModelId", "ModelDesc");
            //ViewBag.CarStatusId = new SelectList(CurStatusList, "CarStausId", "CarStatusDesc");
            InOrdersViewModel model = new InOrdersViewModel();
            try
            {
                //model.ItemsPopulateList(model.FromEmpId, null,1);


                if (String.IsNullOrEmpty(Id))
                {
                    if (this.HasPermission("InOrders", "Adding"))
                    {
                        ModelState.Clear();
                        InOrders UnitRecord = new InOrders();
                        UnitRecord.InOrderDate = DateTime.Today;
                        model.SelectedItem = UnitRecord;
                        model.DisplayMode = "ReadOnly";
                        model.EditMode = "Add";
                        List<InOrdersDetails> InOrdersDetailsList = new List<InOrdersDetails>();
                        model.InOrdersDetails = InOrdersDetailsList;
                        model.SelectedItem.TransferFlag = 1; // personal Inorder
                                                             //return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Unauthorised");

                    }
                }
                else
                {
                    if (this.HasPermission("InOrders", "Updating"))
                    {
                        // Edit record (view in Edit mode)
                        int OrderId = int.Parse(Id);

                        //model.Buildings = unitWork.InOrdersManager.GetNotDelAll().OrderBy(m => m.InOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page);
                        model.SelectedItem = unitWork.InOrdersManager.GetByOrderId(OrderId);
                        model.DisplayMode = "ReadWrite";
                        model.EditMode = "Edit";
                        List<InOrdersDetails> InOrdersDetailsList = unitWork.InOrdersDetailsManager.GetByOrderId(OrderId);
                        model.InOrdersDetails = InOrdersDetailsList;
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
            return GetData(model, "", "", "", 0,ScanFlag);

        }
        public void AddUploadItemsData (InOrdersViewModel Mymodel, HttpPostedFileBase ExcelFileUploader)
        {
           
                long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
           string FolderName = "~" + ConfigurationManager.AppSettings["ImportsPath"].ToString();
            
            string excelPath = Server.MapPath(FolderName) + DateTime.Now.ToOADate().ToString() + Path.GetFileName(ExcelFileUploader.FileName);
            
                ExcelFileUploader.SaveAs(excelPath);
                       
            //string Str = "Pass";
            if (excelPath != "")
                {
                    try
                    {
                        //Upload and save the file
                        string conString = string.Empty;
                        //Connection String to Excel Workbook
                        string extension = Path.GetExtension(ExcelFileUploader.FileName);
                        switch (extension)
                        {
                            case ".xls": //Excel 97-03
                                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                break;
                            case ".xlsx": //Excel 07 or higher
                                conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                                break;
                            case ".csv":
                                conString = ConfigurationManager.ConnectionStrings["CSV07+ConString"].ConnectionString;
                                break;


                        }
                     //Str = "Passxls";
                    if (extension != ".csv")
                    {
                        DataSet ds = new DataSet();
                      
                        conString = string.Format(conString, excelPath);
                            string query = "SELECT * FROM [Sheet1$]";
                            OleDbConnection conn = new OleDbConnection(conString);
                       
                        if (conn.State == ConnectionState.Closed)
                                conn.Open();

                       // Str = "Passxls222";

                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(ds);

                        string SerialNo = "";
                        string ItemId = "";
                        string RFID = "";

                        int i = 0;
                       
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            //your code here
                            if (dr["SerialNo"] != null)
                            {
                                SerialNo = dr["SerialNo"].ToString();
                               
                                i++;
                            }
                            else
                            {
                                SerialNo = "";
                            }
                            if (dr["ItemId"] != null)
                            {
                                ItemId = dr["ItemId"].ToString();
                               
                                i++;
                            }
                            else
                            {
                                ItemId = "";
                            }


                            if (dr["RFID"] != null)
                            {
                                RFID = dr["RFID"].ToString();
                              
                                i++;
                            }
                            else
                            {
                                RFID = "";
                            }
                            if (!String.IsNullOrEmpty(ItemId)) // check user select ItemName
                            {
                                int CurQty = 0;
                                Item_tbl obj = unitWork.ItemsManager.GetById(long.Parse(ItemId));
                                bool CountableFlag = false;
                                string ItemName = "";
                                if (obj != null)
                                {
                                    tbl_ItemsStock objStock = unitWork.ItemsStockManager.GetByRFID(RFID, null);
                                    if (objStock == null)
                                    {
                                        CountableFlag = obj.CountableFlag;
                                        ItemName = obj.Item_Name;
                                        if (!CountableFlag)
                                        {
                                            //Mymodel.ScanItems[i].ItemQty = 1;
                                            CurQty = 1;
                                        }

                                        //Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                                        string ItemRFID = RFID;
                                        long StockId = 0;
                                        //string BarCode = Item_BarCode;
                                        InOrdersDetails NewItem = new InOrdersDetails();
                                        NewItem.ItemId = long.Parse(ItemId);
                                        NewItem.Item_RFID = ItemRFID;
                                        NewItem.StockId = StockId;
                                        NewItem.Item_BarCode = SerialNo;
                                        if (Mymodel.SelectedItem.ContractId != null)
                                        {
                                            NewItem.ContractId = Mymodel.SelectedItem.ContractId;
                                        }
                                        NewItem.Qty = CurQty;
                                        NewItem.ItemName = ItemName;
                                        //NewItem.Item_BarCode = BarCode;
                                        NewItem.ReaderTypeId = Mymodel.ReaderType;
                                        NewItem.StoreId = StoreId;
                                        NewItem.InStoreQty = 1;
                                        NewItem.CountableFlag = CountableFlag;
                                        Mymodel.InOrdersDetails.Add(NewItem);
                                    }
                                    else
                                    {
                                        TempData["warningMessage"] = Resources.DefaultResource.CheckStockIdMsg + " " + RFID;
                                        //break;
                                    }
                                }
                                else
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.CheckItemIdMsg + " " + RFID;
                                    //break;
                                }
                            }
                            else
                            {
                                TempData["warningMessage"] = Resources.DefaultResource.CheckItemIdMsg + " " + RFID;
                                break;
                            }
                        }

                            da.Dispose();
                            conn.Close();
                            conn.Dispose();
                        }
                        else
                        {
                             ImportItemCSV(excelPath, Mymodel);
                        }

                    }
                    catch (Exception ex)
                    {
                        //ErrorDiv.InnerHtml = "<div class=\"alert alert-danger alert-dismissable\"> <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">×</button><h4><i class=\"fa fa-times-circle\"></i> Error</h4>" + Resources.DefaultResource.ExcelErrorMessage + "</div>";
                        TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgErrorImportItem ;
                    }

                }

            
            
        }
        private void ImportItemCSV(string filePath, InOrdersViewModel Mymodel)
        {
            long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
            string csvData = System.IO.File.ReadAllText(filePath);
            DataTable dt = new DataTable();
            bool firstRow = true;
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (firstRow)
                        {
                            foreach (string cell in row.Split(','))
                            {
                                dt.Columns.Add(cell.Trim());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (string cell in row.Split(','))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Trim();

                                i++;
                            }
                        }
                    }
                }


            }
            string SerialNo = "";
            string ItemId = "";
            string RFID = "";

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["SerialNo"] != null)
                {
                    SerialNo = dr["SerialNo"].ToString();
                }
                else
                {
                    SerialNo = "";
                }
                               
                if (dr["ItemId"] != null)
                {
                    ItemId = dr["ItemId"].ToString();
                }
                else
                {
                    ItemId = "";
                }

                if (dr["RFID"] != null)
                {
                    RFID = dr["RFID"].ToString();

                   
                }
                else
                {
                    RFID = "";
                }
                if (!String.IsNullOrEmpty(ItemId)) // check user select ItemName
                {
                    int CurQty = 0;
                    Item_tbl obj = unitWork.ItemsManager.GetById(long.Parse(ItemId));
                    bool CountableFlag = false;
                    string ItemName = "";
                    if (obj != null)
                    {
                        tbl_ItemsStock objStock = unitWork.ItemsStockManager.GetByRFID(RFID, null);
                        if (objStock == null)
                        {
                            CountableFlag = obj.CountableFlag;
                            ItemName = obj.Item_Name;
                            if (!CountableFlag)
                            {
                                //Mymodel.ScanItems[i].ItemQty = 1;
                                CurQty = 1;
                            }

                            //Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                            string ItemRFID = RFID;
                            long StockId = 0;
                            //string BarCode = Item_BarCode;
                            InOrdersDetails NewItem = new InOrdersDetails();
                            NewItem.ItemId = long.Parse(ItemId);
                            NewItem.Item_RFID = ItemRFID;
                            NewItem.StockId = StockId;
                            NewItem.Item_BarCode = SerialNo;
                            if (Mymodel.SelectedItem.ContractId != null)
                            {
                                NewItem.ContractId = Mymodel.SelectedItem.ContractId;
                            }
                            NewItem.Qty = CurQty;
                            NewItem.ItemName = ItemName;
                            //NewItem.Item_BarCode = BarCode;
                            NewItem.ReaderTypeId = Mymodel.ReaderType;
                            NewItem.StoreId = StoreId;
                            NewItem.InStoreQty = 1;
                            NewItem.CountableFlag = CountableFlag;
                            Mymodel.InOrdersDetails.Add(NewItem);
                        }
                        else
                        {
                            TempData["warningMessage"] = Resources.DefaultResource.CheckStockIdMsg + " " + RFID;
                            //break;
                        }
                    }
                    else
                    {
                        TempData["warningMessage"] = Resources.DefaultResource.CheckItemIdMsg + " " + RFID;
                        //break;
                    }
                }
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.CheckItemIdMsg + " " + RFID;
                    break;
                }
            }
            if (Mymodel.InOrdersDetails.Count >0)
            {
                TempData["ScannerMessage"] = Resources.DefaultResource.UploadSucessMsg;
            }
                       

        }
        [HttpPost]
        public ActionResult AddEditInOrder(InOrdersViewModel Mymodel, int? Id, int? Flag, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, HttpPostedFileBase postedFile, HttpPostedFileBase ExcelFileUploader)
        {
            int ScanFlag = 0;
            string SaveValue = Request.Form["CmdSave"];
            string ScanValue = Request.Form["CmdScan"];
            string AddValue = Request.Form["CmdAdd"];
            string DelValue = Request.Form["CmdDel"];
            string CmdUpload = Request.Form["CmdUpload"];
            string UploadItems = Request.Form["UploadItems"];
            string SelNewItem = Request.Form["SelNewItem"];
            string UploadItemsData = Request.Form["UploadItemsData"];
            
            if (Mymodel.PostFlag == 3 ||  SelNewItem == "SelNewItem")
            {
                if (Mymodel.ReaderType == 2 || Mymodel.ReaderType == 3) // select barcode choice
                {
                    long SelStoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
                    //if (!string.IsNullOrEmpty(Mymodel.Barcode))
                    tbl_ItemsStock existing = null;
                    if (Mymodel.SelItemId != null && Mymodel.ReaderType == 2 && SelNewItem == "SelNewItem")
                    {
                        long SelItemId = Mymodel.SelItemId.GetValueOrDefault();

                        existing = unitWork.ItemsStockManager.GetByItemId(SelItemId, SelStoreId);
                    }
                    else if (Mymodel.ReaderType == 3 && !string.IsNullOrEmpty(Mymodel.Barcode))
                    {
                        //existing = unitWork.ItemsStockManager.GetByRFID(Mymodel.Barcode, SelStoreId);
                        existing = unitWork.ItemsStockManager.GetByBarcode(Mymodel.Barcode, SelStoreId);
                    }
                    if (Mymodel.ScanItems == null)
                    {
                        Mymodel.ScanItems = new List<tbl_ItemsStock>();
                    }

                    if (existing != null) // item barcode is existing
                    {
                        //long ItemId = existing.Item_Id;
                        long StoreId = existing.Room_Id.GetValueOrDefault();
                        //long StockId = existing.StockId;
                        //string Barcode = existing.Item_BarCode;
                        //int InStoreQty = existing.ItemQty.GetValueOrDefault();
                        if (StoreId == Mymodel.SelectedItem.StoreId_To)
                        {
                            Mymodel.ScanItems.Add(existing);

                        }

                    }
                    else
                    {
                        tbl_ItemsStock NewStock = new tbl_ItemsStock();
                        NewStock.Item_BarCode = Mymodel.Barcode;
                        NewStock.Room_Id = SelStoreId;
                        NewStock.Item_Id = 0;
                        Mymodel.ScanItems.Add(NewStock);
                        
                    }
                    Mymodel.Barcode = "";
                    List<CheckBoxListItem> ItemsCheckBoxList = new List<CheckBoxListItem>();
                    foreach (var item in Mymodel.ScanItems)
                    {
                        //string stockId = ScanItemsList[i].StockId.ToString() + i.ToString();
                        bool IsSelected = false;
                        CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId, Name = "Item" + item.StockId.ToString(), ItemId = item.Item_Id };
                        ItemsCheckBoxList.Add(CheckItem);
                    }
                    Mymodel.ItemsScanCheckList = ItemsCheckBoxList;

                }
            }

            else if (Mymodel.PostFlag == 6 || Mymodel.PostFlag == 5)
            {
                ScanFlag = 1;
            }
            // UpLoading order image as pdf
            else if (CmdUpload == "CmdUpload")
            {
                string NewFilename = "";
                if (postedFile != null)
                {
                    string CurfileName = Path.GetFileName(postedFile.FileName);
                    string exten = Path.GetExtension(Server.MapPath(postedFile.FileName));
                    string FolderName = "~" + ConfigurationManager.AppSettings["InOrdersPath"].ToString();
                    string path = Server.MapPath(FolderName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    NewFilename = DateTime.Now.ToOADate().ToString() + exten;
                    postedFile.SaveAs(path + NewFilename);
                    if (NewFilename != "")
                    {
                        Mymodel.SelectedItem.PictureName = NewFilename;
                        TempData["warningMessage"] = Resources.DefaultResource.UploadSucessMsg;
                    }

                }


            }
            // UpLoading From Excel file
            else if (UploadItems == "UploadItems")
            {
             
                if (ExcelFileUploader != null)
                {
                    //string CurfileName = Path.GetFileName(ExcelFileUploader.FileName);
                    //string exten = Path.GetExtension(Server.MapPath(ExcelFileUploader.FileName));
                    string FolderName = "~" + ConfigurationManager.AppSettings["ImportsPath"].ToString();

                    string excelPath = Server.MapPath(FolderName) + DateTime.Now.ToOADate().ToString() + Path.GetFileName(ExcelFileUploader.FileName);

                    ExcelFileUploader.SaveAs(excelPath);
                   
                    //ExcelFileUploader.SaveAs(path + NewFilename);
                    if (excelPath != "")
                    {
                        try
                        {
                            //Upload and save the file
                            string conString = string.Empty;
                            //Connection String to Excel Workbook
                            string extension = Path.GetExtension(ExcelFileUploader.FileName);
                            switch (extension)
                            {
                                case ".xls": //Excel 97-03
                                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                                    break;
                                case ".xlsx": //Excel 07 or higher
                                    conString = ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString;
                                    break;
                                case ".csv":
                                    conString = ConfigurationManager.ConnectionStrings["CSV07+ConString"].ConnectionString;
                                    break;


                            }
                            if (extension != ".csv")
                            {
                                DataSet ds = new DataSet();
                                conString = string.Format(conString, excelPath);
                                string query = "SELECT * FROM [Sheet1$]";
                                OleDbConnection conn = new OleDbConnection(conString);
                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();
                                OleDbCommand cmd = new OleDbCommand(query, conn);
                                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                da.Fill(ds);

                                string SerialNo = "";
                                if (Mymodel.ScanItems.Count > 0)
                                {
                                    if (Mymodel.ScanItems.Count >= ds.Tables[0].Rows.Count)
                                    {
                                        int i = 0;
                                        foreach (DataRow dr in ds.Tables[0].Rows)
                                        {
                                            //your code here
                                            if (dr["SerialNo"] != null)
                                            {
                                                SerialNo = dr["SerialNo"].ToString();
                                                Mymodel.ScanItems[i].Item_BarCode = SerialNo;
                                                i++;
                                            }
                                            else
                                            {
                                                SerialNo = "";
                                            }
                                        }

                                        if (Mymodel.ScanItems.Count != ds.Tables[0].Rows.Count)
                                        {
                                            TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgCountNumbers;
                                        }
                                        else
                                        {
                                            TempData["ScannerMessage"] = Resources.DefaultResource.UploadSucessMsg;
                                        }
                                    }
                                    else
                                    {
                                        TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgCountNumbers;
                                    }

                                }

                                da.Dispose();
                                conn.Close();
                                conn.Dispose();
                            }
                            else
                            {
                                ImportCSV(excelPath, Mymodel);
                            }

                        }
                        catch (Exception ex)
                        {
                            //ErrorDiv.InnerHtml = "<div class=\"alert alert-danger alert-dismissable\"> <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">×</button><h4><i class=\"fa fa-times-circle\"></i> Error</h4>" + Resources.DefaultResource.ExcelErrorMessage + "</div>";
                            TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgErrorImportItem;
                        }

                    }

                }


            }
            // scan from reader
            // UpLoading From Excel file data items
            else if (UploadItemsData == "UploadItemsData")
            {
               
                if (ExcelFileUploader != null)
                {

                    AddUploadItemsData(Mymodel, ExcelFileUploader);

                }

            }
            // scan from reader
            else if (SaveValue == "CmdSave")
            {

                bool ret = SaveInOrder(Mymodel);
                if (ret)
                {
                    return RedirectToAction("AddEditInOrder", new { id = Mymodel.SelectedItem.InOrderId, ScanFlag = 0 });
                }
                else
                {
                    return GetData(Mymodel, "", "", "", 0, ScanFlag);
                    //return RedirectToAction("AddEditInOrder", Mymodel);
                }

            }
            // if make scan item from scan device
            else if (ScanValue == "Scan" || Mymodel.DisplayMode == "StoreId_To")
            {

                ScanFlag = 1;
            }
            else if (AddValue == "AddItem") // add item button to inser item in right list
            {
                
                int i = 0;
                foreach (var item in Mymodel.ItemsScanCheckList)
                {
                    if (item.IsSelected)
                    {

                        bool CountableFlag = false;
                        string ItemName = "";
                        // check new item add to Inventory
                        if (Mymodel.ScanItems[i].Item_Id == 0 && Mymodel.SelectedItem.StoreId_To != null)
                        {
                            if (Mymodel.ItemsScanCheckList[i].ItemId > 0) // check user select ItemName
                            {
                                Item_tbl obj = unitWork.ItemsManager.GetById(Mymodel.ItemsScanCheckList[i].ItemId);
                                if (obj != null)
                                {
                                    CountableFlag = obj.CountableFlag;
                                    ItemName = obj.Item_Name;
                                    if (!CountableFlag) { Mymodel.ScanItems[i].ItemQty = 1; 
                                 }
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
                                CountableFlag = obj.CountableFlag;
                                ItemName = obj.Item_Name;
                                long curStoreId = Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                                if (!CountableFlag) // وجود صنف غير نثري واضافته مرة اخري
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.DuplicateUncountableItemInstock + " " + Mymodel.ScanItems[i].Item_RFID;
                                    break;
                                }
                                else if (curStoreId != Mymodel.SelectedItem.StoreId_To) // التاكد من ان الصنف نثري و موجود فى نفس المخزن
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.CheckItemsInventoryMsg + " " + Mymodel.ScanItems[i].Item_RFID;
                                    break;
                                }
                            }

                        }
                        long ItemId;
                        if (Mymodel.ReaderType==2 && Mymodel.ScanItems[i].Item_Id!=0) // read 
                        {
                            ItemId= Mymodel.ScanItems[i].Item_Id;
                        }
                        else
                        { // scan barcode of old items
                            if (Mymodel.ScanItems[i].Item_Id == 0) // 
                            {
                                ItemId = Mymodel.ItemsScanCheckList[i].ItemId; // new item not found in inventory
                            }
                            else
                            {
                                ItemId = Mymodel.ScanItems[i].Item_Id; // set if an old item in inventory
                            }
                        }
                        //long ItemId = Mymodel.ItemsScanCheckList[i].ItemId; // 
                        long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault(); //Mymodel.ScanItems[i].Room_Id.GetValueOrDefault();
                        string ItemRFID = Mymodel.ScanItems[i].Item_RFID;
                        long StockId = Mymodel.ScanItems[i].StockId;
                        string BarCode = Mymodel.ScanItems[i].Item_BarCode;

                        List<InOrdersDetails> RowsInOrdersDetails = new List<InOrdersDetails>();
                        if (Mymodel.InOrdersDetails != null)
                        {
                            //case of scanning by RFID reader
                            if (Mymodel.ReaderType == 1)
                            {
                                RowsInOrdersDetails = Mymodel.InOrdersDetails.Where(c => c.StockId == StockId && c.ItemId == ItemId && c.Item_RFID == ItemRFID).ToList();
                            }
                            else if (Mymodel.ReaderType == 3)
                            {
                                RowsInOrdersDetails = Mymodel.InOrdersDetails.Where(c => c.StockId == StockId && c.ItemId == ItemId && c.Item_BarCode == BarCode).ToList();
                            }
                            else
                            {
                                RowsInOrdersDetails = Mymodel.InOrdersDetails.Where(c => c.StockId == StockId && c.ItemId == ItemId).ToList();
                            }
                        }
                        else
                        {
                            Mymodel.InOrdersDetails = new List<InOrdersDetails>();
                        }
                        if (RowsInOrdersDetails.Count == 0)
                        {
                            
                            InOrdersDetails NewItem = new InOrdersDetails();
                            NewItem.ItemId = ItemId;
                            NewItem.Item_RFID = ItemRFID;
                            NewItem.StockId = StockId;
                            if (Mymodel.SelectedItem.ContractId != null)
                            {
                                NewItem.ContractId = Mymodel.SelectedItem.ContractId;
                            }
                            NewItem.Qty = 1;
                            NewItem.ItemName = ItemName;
                            NewItem.Item_BarCode = BarCode;
                            NewItem.ReaderTypeId  = Mymodel.ReaderType;
                            NewItem.StoreId = StoreId;
                            NewItem.InStoreQty = Mymodel.ScanItems[i].ItemQty;
                            NewItem.CountableFlag = CountableFlag;
                            Mymodel.InOrdersDetails.Add(NewItem);

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
                    if (Mymodel.SelectedItem.IsLock.GetValueOrDefault())
                    {

                        TempData["warningMessage"] = Resources.DefaultResource.InorderIslockMsg;
                        TempData["Message"] = null;
                    }
                    else
                    {
                        int index = Id.GetValueOrDefault();
                        Boolean DelFalg = true;
                        if (Mymodel.InOrdersDetails != null)
                        {
                            long InOrderDetId = Mymodel.InOrdersDetails[index].InOrderDetId;
                            long ItemId = Mymodel.InOrdersDetails[index].ItemId.GetValueOrDefault();
                            string Item_RFID = Mymodel.InOrdersDetails[index].Item_RFID;


                            InOrdersDetails existing = unitWork.InOrdersDetailsManager.GetByOrderDetId(InOrderDetId);
                            if (existing != null)
                            {
                                //tbl_ItemsStock StockObj =  unitWork.ItemsStockManager.GetById(existing.StockId);
                                //if (StockObj.Room_Id == Mymodel.SelectedItem.StoreId_To)
                                //{
                                if (CheckItemDelete(existing.InOrderDetId))
                                {
                                    unitWork.InOrdersDetailsManager.Delete(existing);
                                }
                                else
                                {
                                    TempData["warningMessage"] = Resources.DefaultResource.DeleteItemInOrderMsg;
                                    DelFalg = false;
                                }

                                //}
                            }
                            if (DelFalg)
                            {
                                Mymodel.InOrdersDetails.RemoveAt(index);
                            }
                        }
                    }
                   
                }

            }
            else if (Mymodel.ReaderType == 1 && ScanFlag == 1)
            {
                ScanFlag = 1;
            }

            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No, ScanFlag));
        }
        [HttpPost]
        public ActionResult DeleteOrderItemsOrder(InOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
            return (GetData(Mymodel, Sorting_Order, Search_Data, Filter_Value, Page_No,0));
        }
        private bool SaveInOrder(InOrdersViewModel Mymodel)
        {
            bool ret = true;
            try
            {
                if (CheckExpiredDate(Mymodel))
                {
                    //checked user select invetory
                    if (Mymodel.SelectedItem.StoreId_To != null)
                    {
                        long StoreId = Mymodel.SelectedItem.StoreId_To.GetValueOrDefault();
                    }
                    else
                    {
                        ret = false; // end flag
                    }

                    if (Mymodel.InOrdersDetails.Count == 0) // check if user add details items
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.RequiredItemsMsg;
                        TempData["Message"] = null;
                    }
                    // check in order not lock for update
                    if (Mymodel.SelectedItem.IsLock.GetValueOrDefault())
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.InorderIslockMsg;
                        TempData["Message"] = null;
                        unitWork.InOrdersManager.Update(Mymodel.SelectedItem);
                    }
                    // check date is more than Today
                    if (Mymodel.SelectedItem.InOrderId == 0 && Mymodel.SelectedItem.InOrderDate.GetValueOrDefault() < DateTime.Today)
                    {
                        ret = false;
                        TempData["warningMessage"] = Resources.DefaultResource.DateMorThanToday;
                        TempData["Message"] = null;
                    }
                    if (ret)
                    {
                        //if (ModelState.IsValid)
                        //{
                        // insert new In order
                        if (Mymodel.SelectedItem.InOrderId == 0)
                        {

                            // insert new record
                            Mymodel.SelectedItem.IsLock = false;
                            InOrders InOrdersbj = unitWork.InOrdersManager.Add(Mymodel.SelectedItem);
                            if (InOrdersbj != null)
                            {
                                Mymodel.InOrdersDetails = Mymodel.InOrdersDetails;
                                TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
                            }
                            else
                            {
                                TempData["Message"] = null;
                            }


                            //ModelState.Clear();
                        }
                        else // update old in order
                        {

                            bool bret = unitWork.InOrdersManager.Update(Mymodel.SelectedItem);

                            // update record
                        }

                        if (SaveOrderItems(Mymodel))
                        {
                            TempData["Message"] = Resources.DefaultResource.SaveMessage;
                        }
                        else
                        {
                            TempData["Message"] = null;
                        }
                        //}
                    }
                }
                else
                {
                    ret = false;
                    TempData["warningMessage"] = Resources.DefaultResource.ExpiredDateInvalidMsg;
                    TempData["Message"] = null;
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
        private bool CheckExpiredDate(InOrdersViewModel Mymodel)
        {
            bool ret = true;
            foreach (var item in Mymodel.InOrdersDetails)
            {
                if (item.ExpiredDate != null)
                {
                    DateTime ExpiredDate = item.ExpiredDate.Value;
                    if (ExpiredDate != null)
                    {
                        if (ExpiredDate < DateTime.Today)
                        {
                            ret = false;
                            break;
                        }
                    }
                }
            }
            return ret;
        }
        private bool CheckEnoughQty(InOrdersViewModel Mymodel, ref string ItemName)
        {
            UnitOfWork unWork = new UnitOfWork();
            bool ret = true;
            ItemName = "";
            foreach (var item in Mymodel.InOrdersDetails)
            {

                int OldQty = 0;
                int TotalQty = 0;
                // get old quantity of item
                InOrdersDetails InOrdersDetailsObj = unWork.InOrdersDetailsManager.GetById(item.InOrderDetId);
                if (InOrdersDetailsObj != null)
                {
                    OldQty = InOrdersDetailsObj.Qty.GetValueOrDefault();
                }
                tbl_ItemsStock StockObj = unWork.ItemsStockManager.GetById(item.StockId);
                Item_tbl ItemObj = unWork.ItemsManager.GetById(StockObj.Item_Id); // main stock
                if (ItemObj.CatSub_tbl.GenerateBarcodeFlag)
                {
                    if (ItemObj.CatSub_tbl.GenerateBarcodeFlag) // get current Item Quantity in inventory
                    {
                        List<tbl_ItemsStockDetails> ItemsStockDetailsList = unWork.ItemsStockDetailsManager.GetByInOrderDetId(item.InOrderDetId);
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
                int DiffQty = TotalQty + (item.Qty.GetValueOrDefault()-OldQty) ; // نتاكد ان الكمية الحالية لا تصبح سالب
                if (DiffQty < 0)
                {
                    ret = false;
                    ItemName = StockObj.ItemName;
                    break;
                }
                
                
            }
            return ret;
        }
        private bool SaveOrderItems(InOrdersViewModel Mymodel)
        {
            bool ret = false;
            int i = 0;
            foreach (var item in Mymodel.InOrdersDetails)
            {
                if (item.StockId ==0 ) // save stock item if new
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
                            StockObj.Item_BarCode = item.Item_BarCode;
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
                    item.tbl_ItemsStock.Room_Id = Mymodel.SelectedItem.StoreId_To;
                   
                }
                if (!item.tbl_ItemsStock.Item_tbl.CountableFlag )
                {
                    item.Qty = 1;
                }
                if (item.InOrderDetId == 0) // add new record of InorderDetails
                {
                    // insert new record
                    item.InOrderId = Mymodel.SelectedItem.InOrderId;

                    if (item.ContractId == null)
                    {
                        item.ContractId = Mymodel.SelectedItem.ContractId;
                    }
                    InOrdersDetails InOrderItemsbj = unitWork.InOrdersDetailsManager.Add(item);
                    if (InOrderItemsbj != null)
                    {
                        //Mymodel.InOrdersDetails = Mymodel.InOrdersDetails;
                        TempData["Message"] = Resources.DefaultResource.SaveMessage;// "Success";
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
                    item.InOrderId = Mymodel.SelectedItem.InOrderId;
                    if (item.ContractId == null)
                    {
                        item.ContractId = Mymodel.SelectedItem.ContractId;
                    }
                    bool bret = unitWork.InOrdersDetailsManager.Update(item);
                    // update record
                    if (bret)
                    {
                        TempData["Message"] = Resources.DefaultResource.SaveMessage; //"Success";
                        TempData["warningMessage"] = null;
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
      
        private void  ImportCSV(string filePath, InOrdersViewModel Mymodel)
        {
            string csvData = System.IO.File.ReadAllText(filePath);
            DataTable dt = new DataTable();
            bool firstRow = true;
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (firstRow)
                        {
                            foreach (string cell in row.Split(','))
                            {
                                dt.Columns.Add(cell.Trim());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (string cell in row.Split(','))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Trim();
                               
                                i++;
                            }
                        }
                    }
                }


            }

            string SerialNo = "";
            if (Mymodel.ScanItems.Count > 0)
            {
                if (Mymodel.ScanItems.Count >= dt.Rows.Count)
                {
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        //your code here
                        if (dr["SerialNo"] != null)
                        {
                            SerialNo = dr["SerialNo"].ToString();
                            Mymodel.ScanItems[i].Item_BarCode = SerialNo;
                            i++;
                        }
                        else
                        {
                            SerialNo = "";
                        }
                    }

                    if (Mymodel.ScanItems.Count != dt.Rows.Count)
                    {
                        TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgCountNumbers;
                    }
                    else
                    {
                        TempData["ScannerMessage"] = Resources.DefaultResource.UploadSucessMsg;
                    }
                }
                else
                {
                    TempData["ScannerMessage"] = Resources.DefaultResource.WariningMsgCountNumbers;
                }

            }
           
        }
        public ActionResult GetData(InOrdersViewModel Mymodel, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No,  int? ScanFlag)
        {

            InOrdersViewModel model = new InOrdersViewModel();
            //List<tbl_ItemsStock> ItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();
            //List<tbl_ItemsStock> ToItemsList = unitWork.ItemsStockManager.GetNotDelAll().OrderByDescending(m => m.Item_Id).ToList();

            if (Mymodel.SelectedItem.TransferFlag == 0) // action to do in form for assets
            {
                Mymodel.SelectedItem.TransferFlag = 1;
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
            model.SelItemId = Mymodel.SelItemId;
            model.EditMode = Mymodel.EditMode;
            model.NewItemFlag = Mymodel.NewItemFlag;

            //model.SelectedItem.OrganizedFlag= Mymodel.SelectedItem.OrganizedFlag;
            Mymodel.searchType = Mymodel.SelectedItem.TransferFlag.GetValueOrDefault();
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

            model.Suppliers = unitWork.SuppliersManager.GetNotDelAll().ToList().Select(option => new SelectListItem
            {
                Text = option.Sup_Name,
                Value = option.Sup_code.ToString()
            });

            model.Contracts = unitWork.VendorContractsManager.GetNotDelAll().ToList().Select(option => new SelectListItem
            {
                Text = option.ContractName,
                Value = option.ContractId.ToString()
            });


            List<Item_tbl>  Items = new List<Item_tbl>();
            if (model.ReaderType == 2)
            {
                Items = unitWork.ItemsManager.GetNotDelAllNewBarcode().ToList();
            }
            else
            {
                Items = unitWork.ItemsManager.GetNotDelAll().ToList();
            }

           

            if (ScanFlag==1)
            {             
                
                if (model.Suppliers.Count() > 0 && Mymodel.SelectedItem.SupplierId_From == null)
                {
                    model.SelectedItem.SupplierId_From = int.Parse(model.Suppliers.First().Value);
                }
                // if user choose inventory connecto to reader
                if (Mymodel.SelectedItem.StoreId_To != null)
                {
                    Room_tbl Roomobj = new Room_tbl();
                    Roomobj = unitWork.RoomsManager.GetById(Mymodel.SelectedItem.StoreId_To);
                    if (Roomobj != null)
                    {
                        if (Roomobj.ReaderId != null)
                        {
                            if (Mymodel.DisplayMode == "StoreId_To")
                            {
                                model.ReaderType = Roomobj.ReaderId.GetValueOrDefault();
                            }
                        }
                        if (model.ReaderType == 1)
                        {
                            model.IPAddress = Roomobj.IPAddress;
                            model.TcpFlag = Roomobj.TcpFlag.GetValueOrDefault();
                            // scan from reader
                            List<tbl_ItemsStock> ScanItemsList = ScanReader(model.IPAddress, model.TcpFlag,null, model.NewItemFlag);
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
                            if (model.ReaderType == 2)
                            {
                                Items = unitWork.ItemsManager.GetNotDelAllNewBarcode().ToList();
                            }
                            else
                            {
                                Items = unitWork.ItemsManager.GetNotDelAll().ToList();
                            }
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
                if (Mymodel.PostFlag != 7)
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

                        //if (Mymodel.ReaderType == 2 && string.IsNullOrEmpty(Mymodel.Barcode))
                        //{
                        //    model.ScanItems.Clear();
                        //    model.ItemsScanCheckList.Clear();
                        //}
                    }
                }

               
            }
            model.Items = Items.Select(option => new SelectListItem
            {
                Text = option.Item_Name,
                Value = option.Item_Id.ToString()
            });

            //List<CheckBoxListItem> ItemsSelectList = new List<CheckBoxListItem>();

            foreach (var item in Mymodel.InOrdersDetails)
            {
                item.tbl_ItemsStock  = unitWork.ItemsStockManager.GetById(item.StockId);

                //bool IsSelected = false;
                //if (item.ContractId != null)
                //{
                //    IsSelected = true;
                //}
                //CheckBoxListItem CheckItem = new CheckBoxListItem() { IsSelected = IsSelected, Id = item.StockId.GetValueOrDefault(), Name = "Item" + item.StockId.GetValueOrDefault() };
                //ItemsSelectList.Add(CheckItem);
            }
            //model.ContractSelectList = ItemsSelectList;


            model.InOrdersDetails = Mymodel.InOrdersDetails;
            ModelState.Clear();
            //ModelState.Remove("ItemsCheckBoxList");
            return View(model);

        }
        [HttpPost]
        public ActionResult LoadData(ItemsStockViewModel Mymodel)
        {
            // When the ModelState is not valid, I'd like to redirect the user
            return RedirectToAction("AddEditInOrder", "Orders", Mymodel);
            //// Will save the data to the DB after if ModelState is valid

        }

        private List<tbl_ItemsStock> ScanReader(string IpAddress, bool TcpFlag,long ? StoreId, bool NewItemOnly)
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
            ItemsStockListAll = globalClass.ParsInOrderTages(StoreId, NewItemOnly);
            globalClass.DisconnectReader();
            return ItemsStockListAll;
            //timer1.Start();
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            int Id = int.Parse(id);
            InOrders existing = unitWork.InOrdersManager.GetByOrderId (Id); //GetByOrderId(Id);
            if (existing != null)
            {
                if (existing.ReturnInOrders.Count == 0)
                {
                    if (CheckDetailsBeforeDelete(Id))
                    {
                        unitWork.InOrdersManager.Delete(existing);
                        DeleteDetails(Id); // delete order items

                        InOrdersViewModel model = new InOrdersViewModel();
                        model.InOrders = unitWork.InOrdersManager.GetNotDelAll().OrderByDescending(m => m.InOrderId).Take(Size_Of_Page).ToPagedList(No_Of_Page, Size_Of_Page); ;

                        model.SelectedItem = null;
                        model.DisplayMode = "";
                        TempData["warningMessage"] = null;
                       // return RedirectToAction("InOrderList");
                    }
                    else
                    {
                        TempData["warningMessage"] = Resources.DefaultResource.DeleteInOrderMsg;
                        
                    }
                }
               
                else
                {
                    TempData["warningMessage"] = Resources.DefaultResource.DeleteOutOrderMsg;
                    //return RedirectToAction("InOrderList");

                }
            }
            
            return RedirectToAction("InOrderList");
        }
        private Boolean CheckItemDelete(long InOrderDetId)
        {
            Boolean ret = true;
            InOrdersDetails existing = unitWork.InOrdersDetailsManager.GetByOrderDetId(InOrderDetId);
            if (existing != null)
            {
                int Difference = existing.tbl_ItemsStock.ItemQty.GetValueOrDefault() - existing.Qty.GetValueOrDefault();
                if (Difference < 0)
                {
                    ret = false;
                   
                }
                //List <ReturnInOrdersDetails> ReturnInOrdersDetail = unitWork.ReturnInOrdersDetailsManager.GetOrderByDetId(InOrderDetId);
                if (existing.ReturnInOrdersDetails.Count > 0)
                {
                    ret = false;
                }
            }
            
            return ret;
        }
        private Boolean CheckDetailsBeforeDelete(long OrderId)
        {
            List<InOrdersDetails> InOrdersDetailsList = unitWork.InOrdersDetailsManager.GetByOrderId(OrderId);
            Boolean ret=true;
            foreach (var item in InOrdersDetailsList)
            {
                ret=CheckItemDelete(item.InOrderDetId);
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
            List<InOrdersDetails> InOrdersDetailsList = unitWork.InOrdersDetailsManager.GetByOrderId(OrderId);
            
            foreach (var item in InOrdersDetailsList)
            {
                InOrdersDetails existing = unitWork.InOrdersDetailsManager.GetByOrderDetId (item.InOrderDetId);
                if (existing != null)
                {
                    unitWork.InOrdersDetailsManager.Delete(existing);

                }
            }
        }
        [HttpGet]
        [ActionName("ShowOrderDetails")]
        public ActionResult ShowOrderDetails(string id = null)
        {

            InOrdersDetialsViewModel model = new InOrdersDetialsViewModel();
            if (String.IsNullOrEmpty(id))
            {

                //OutOrdersDetails OutOrdersDetailsRecord = new OutOrdersDetails();
                //model.SelectedUnit = UnitRecord;
                model.DisplayMode = "ReadOnly";
                return PartialView("InOrderModal", model.InOrdersDetails);
            }
            else
            {
                // Edit record (view in Edit mode)
                long OrderId = long.Parse(id);
                int Size_Of_Page = 30;
                model.InOrdersDetails = unitWork.InOrdersDetailsManager.GetByOrderId(OrderId).OrderBy(m => m.InOrderId).ToPagedList(No_Of_Page, Size_Of_Page);

                //model.OutOrders = OutOrdersList.ToPagedList(No_Of_Page, Size_Of_Page);

                //model.SelectedUnit = unitWork.UnitsManager.GetById(ModelId);
                model.DisplayMode = "ReadOnly";
                //if (model.SelectedUnit == null) { return View("_error"); }
                // ...
                return PartialView("InOrderModal", model);
            }

        }

    }

}