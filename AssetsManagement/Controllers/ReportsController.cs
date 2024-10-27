using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Collections;
using System.Data;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Sdk.Comm;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Zebra.Sdk.Graphics;

namespace AssetsManagement.Controllers
{


    public class ReportsController : Controller
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        string CurrentUser { get; set; }
        public static string QRCode;

        private UnitOfWork unitWork = new UnitOfWork();

        public ActionResult ShowReport(string ReportName, string DataSetName, int? Flag, string Param, string ItemName)
        {
            if (TempData["list"] != null)
            {

                //List<IEnumerable> cm = TempData["list"] as List<IEnumerable>;
                //var cm = TempData["list"] as DataSet;
                //List<object> products = JsonSerializer.Deserialize<List<object>>(data);
                var profileData = Session["UserProfile"] as SesssionUser;
                if (profileData != null)
                {
                    CurrentUser = profileData.LoginName; ;

                    //DataTable cm = TempData["list"] as DataTable;
                    LocalReport lr = new LocalReport();
                    string path = Path.Combine(Server.MapPath("~/Reports"), ReportName);
                    if (System.IO.File.Exists(path))
                    {
                        lr.ReportPath = path;
                    }
                    else
                    {
                        return View();
                    }

                    ReportDataSource rd = new ReportDataSource(DataSetName, TempData["list"]);
                    lr.DataSources.Add(rd);

                    var reportViewer = new ReportViewer();
                    reportViewer.LocalReport.ReportPath = path;
                    // Server.MapPath("~/Reports/Reception/PatientMoneyReceipt.rdlc");
                    ReportParameter[] parameters;

                    switch (Flag)
                    {
                        case 1:
                            parameters = new ReportParameter[2];
                            byte[] QRBytes = GlobalsCls.GetQRCodeBytes(QRCode);
                            //byte[] QRBytes = GlobalsCls.GetBarcodeImage(QRCode);
                            string base64String = Convert.ToBase64String(QRBytes);
                            parameters[0] = new ReportParameter("ReportUserName", CurrentUser, true);
                            parameters[1] = new ReportParameter("ReportQRCode", base64String, true);
                            break;
                        case 2:
                            parameters = new ReportParameter[3];
                            string ShowRFID = bool.FalseString;
                            if (Param.Length > 0)
                            {
                                ShowRFID = Param;
                            }
                            parameters[0] = new ReportParameter("ReportUserName", CurrentUser, true);
                            parameters[1] = new ReportParameter("ShowRFID", ShowRFID, true);
                            parameters[2] = new ReportParameter("ItemName", ItemName, true);
                            break;
                        case 3:
                            parameters = new ReportParameter[2];
                            //byte[] QRBytes = GlobalsCls.GetQRCodeBytes(QRCode);
                            byte[] QRBytes1 = GlobalsCls.GetBarcodeImage(QRCode);
                            string base64String1 = Convert.ToBase64String(QRBytes1);
                            parameters[0] = new ReportParameter("ReportUserName", CurrentUser, true);
                            parameters[1] = new ReportParameter("ReportQRCode", base64String1, true);
                            break;
                        default:
                            parameters = new ReportParameter[1];
                            parameters[0] = new ReportParameter("ReportUserName", CurrentUser, true);
                            break;

                    }

                    reportViewer.LocalReport.EnableExternalImages = true;
                    reportViewer.LocalReport.SetParameters(parameters);

                    reportViewer.LocalReport.DataSources.Clear();
                    reportViewer.LocalReport.DataSources.Add(rd);
                    reportViewer.LocalReport.Refresh();
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.AsyncRendering = false;
                    reportViewer.SizeToReportContent = true;
                    reportViewer.ZoomMode = ZoomMode.FullPage;
                    ViewBag.ReportViewer = reportViewer;

                    ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;
                    return View();
                }
                else
                {
                    //return RedirectToAction();
                    return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
                    //return View(System.Web.HttpContext.Current.Request.UrlReferrer);
                }
            }
            else
            {
                if (System.Web.HttpContext.Current.Request.UrlReferrer.LocalPath == "/Reports/ShowReport")
                {
                    return Redirect("/home/index");
                }
                else
                {
                    return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());
                }
            }


        }

        public ActionResult OutOrderItemReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.searchType = 1;
            var allDeptsList = unitWork.DepartmentManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            var defaultDeptId = allDeptsList.Select(m => m.Id).FirstOrDefault();
            model.Departments = new SelectList(allDeptsList, "Id", "Name", "");
            //get all Categories according to defaultCountryId

            var allEmployeesList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == defaultDeptId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Employees = new SelectList(allEmployeesList, "Id", "FULL_NAME_AR", "");

            // get user inventories


            /*List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();*/
            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);

            var allRoomsList = unitWork.RoomsManager.GetOnlyRoomsAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Rooms = new SelectList(allRoomsList, "Room_Id", "Room_Name", model.RoomId);

            var allItemsList = unitWork.ItemsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", model.ItemId);


            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

            //List<OutOrdersDetails> cm = unitWork.OutOrdersDetailsManager.GetNotDelAll().ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            ////string ReportName,string DataSetName, IEnumerable dataSourceValue
            //TempData["list"] = cm;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }


        [HttpPost]
        public ActionResult OutOrderItemReport(ReportViewModel model)
        {

            long? RoomId = model.RoomId;
            //long? StoreId = model.StoreId;
            long[] StoresIds = model.Inventors;
            long? ItemId = model.ItemId;
            int? DeptId = model.DeptId;
            int? EmpId = model.EmpId;
            int? searchType = model.searchType;

            //DateTime ? FromDate= model.FromDate==null ? DateTime.Today: DateTime .Parse(model.FromDate);
            //DateTime ? ToDate= model.ToDate == null ? DateTime.Today : DateTime.Parse(model.ToDate);
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (model.FromDate != null)
            {
                FromDate = DateTime.Parse(model.FromDate);
            }

            if (model.ToDate != null)
            {
                ToDate = DateTime.Parse(model.ToDate);
            }
            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();
           
            List<vwOutOrderDetails> cm = unitWork.VwOutOrderDetailsManager.GetNotDelAllByParam
         (FromDate, ToDate, RoomId, DeptId, EmpId, StoresIds, ItemId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "OutOrderItemsRpt.rdlc";
            switch (searchType)
            {
                case 1:
                    ReportName = "OutOrderItemsRpt.rdlc";
                    break;
                case 2:
                    ReportName = "OutOrderTotalItemsRpt.rdlc";
                    break;
            }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });

        }



        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {

            ReportViewModel model = new ReportViewModel();
            DepartementsViewModel DeptModel = new DepartementsViewModel();
            if (type == "DeptId") { type = "DeptId"; }

            var defaultSubCatId = 0;

            switch (type)
            {
                case "DeptId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.Employees = DeptModel.vwEmployees;
                    break;
                case "FloorId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.Rooms = DeptModel.vwRooms;
                    break;
                case "VendorId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.VwContracts = DeptModel.vwContracts;
                    break;
                case "MainCatId":
                    var CategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id == value).ToList();
                    var defaultCatId = 0;// allCategoryList.Select(m => m.Cat_Id).FirstOrDefault();
                    SelectList Category = new SelectList(CategoryList, "Cat_Id", "Cat_Name", Resources.DefaultResource.ListChoose);
                    model.Category = Category;

                    var SubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCatId).ToList();
                    var SubCategory = new SelectList(SubCategoryList, "CatSub_Id", "CatSub_Name", Resources.DefaultResource.ListChoose);
           
                    model.SubCategory = SubCategory;

                    var ItemsList = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId).ToList();
                    SelectList Items = new SelectList(ItemsList, "Item_Id", "Item_Name", Resources.DefaultResource.ListChoose);
                    model.Items = Items;

                    break;
                case "Cat_Id":
                    var SubCategoryList1 = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id  == value).ToList();
                    SelectList SubCategory1 = new SelectList(SubCategoryList1, "CatSub_Id", "CatSub_Name", Resources.DefaultResource.ListChoose);
                    model.SubCategory  = SubCategory1;

                    var ItemsList1 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == defaultSubCatId).ToList();
                    SelectList Items1 = new SelectList(ItemsList1, "Item_Id", "Item_Name", Resources.DefaultResource.ListChoose);
                    model.Items = Items1;
                    break;
                case "CatSub_Id":
                    var  ItemsList2 = unitWork.ItemsManager.GetNotDelAll().Where(m => m.CatSub_Id == value).ToList();
                    SelectList Items2 = new SelectList(ItemsList2, "Item_Id", "Item_Name", Resources.DefaultResource.ListChoose);
                    model.Items = Items2;
                    break;
            }


            return Json(model);
        }


        public ActionResult ShowItemStockReport(long? ItemId, int? DeptId, long? RoomId, int? EmpId, int? ContractId,int? Flag, string ZeroFlag)
        {
            List<tbl_ItemsStock> cm = unitWork.ItemsStockManager.GetByParam(ItemId, DeptId, RoomId, EmpId, ContractId).ToList();

            bool ZeroFlagVaule = false;
            if (!String.IsNullOrEmpty(ZeroFlag)) // check qty of zero value
            {
                ZeroFlagVaule = bool.Parse(ZeroFlag);
                
            }
            else
            {
                
                ZeroFlagVaule = false;
            }
            if (ZeroFlagVaule)
            {
                cm = cm.Where(c => c.ItemQty > 0).ToList();
            }

            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string DefaultReport = "ItemsStockRpt.rdlc";
            if (Flag == 1)
            {
                DefaultReport = "ItemsStockContractRpt.rdlc";
            }
                return RedirectToAction("ShowReport", "Reports", new { ReportName = DefaultReport, DataSetName = "ItemStockDataSet" });
            

        }


        public ActionResult ShowOutOrderReport(long? orderId)
        {

            List<vwOutOrderDetails> cm = unitWork.VwOutOrderDetailsManager.GetNotDelByOrderId(orderId);
            if (cm.Count > 0)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                CurrentUser = profileData.LoginName; ;
                QRCode = "Date: " + DateTime.Now.ToString() + "\n" + "\n" + "Printed by: " +
                CurrentUser + "\n" + "OrderId: " +
                cm[0].OutOrderId.ToString() + "\n" + "Order Date :" +
                cm[0].OutOrderDate.Value.ToShortDateString();

            }
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderRpt.rdlc", DataSetName = "ItemStockDataSet", Flag = 1 });


            //Image1.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String(QRBytes);
        }


        public ActionResult EmptyOutOrderItemReport()
        {
            //List<vwOutOrderDetails> cm = new List<vwOutOrderDetails>();
            List<vwOutOrderDetails> cm = unitWork.VwOutOrderDetailsManager.GetNotDelByOrderId(57);
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderEmptyRpt.rdlc", DataSetName = "ItemStockDataSet" });

        }

        public ActionResult ItemsOutOfLimitReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.searchType = 1;
            var allMainCatList = unitWork.CatMainManager.GetNotDelAll().OrderByDescending(m => m.CatMain_Id).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.CatMain = new SelectList(allMainCatList, "CatMain_Id", "CatMain_Name", model.MainCatId);
            //get all Categories according to defaultCountryId


            //var allStoresList = unitWork.RoomsManager.GetInventoriesAll().ToList();
            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);

            var allItemsList = unitWork.ItemsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", model.ItemId);

            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }

        [HttpPost]
        public ActionResult ItemsOutOfLimitReport(ReportViewModel model)
        {

            long? MainCatId = model.MainCatId;
            long? StoreId = model.StoreId;
            long? ItemId = model.ItemId;

            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

            List<tbl_ItemsStock> cm = unitWork.ItemsStockManager.GetByLessThanLimit(ItemId, StoreId, MainCatId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "ItemsStockRpt.rdlc";
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });

        }
        public ActionResult AllOrderItemsReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.searchType = 1;
            var allDeptsList = unitWork.DepartmentManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            var defaultDeptId = allDeptsList.Select(m => m.Id).FirstOrDefault();
            model.Departments = new SelectList(allDeptsList, "Id", "Name", model.DeptId);
            //get all Categories according to defaultCountryId

            var allEmployeesList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == defaultDeptId).ToList();
            var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Employees = new SelectList(allEmployeesList, "Id", "FULL_NAME_AR", model.EmpId);


            //var allStoresList = unitWork.RoomsManager.GetInventoriesAll().ToList();
            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);


            var allItemsList = unitWork.ItemsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", model.ItemId);


            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();


            //List<OutOrdersDetails> cm = unitWork.OutOrdersDetailsManager.GetNotDelAll().ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            ////string ReportName,string DataSetName, IEnumerable dataSourceValue
            //TempData["list"] = cm;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }

        [HttpPost]
        public ActionResult AllOrderItemsReport(ReportViewModel model)
        {
            long? StoreId = model.StoreId;
            long? ItemId = model.ItemId;
            int? DeptId = model.DeptId;
            int? EmpId = model.EmpId;
            int? searchType = model.searchType;
            //DateTime? FromDate = model.FromDate == null ? DateTime.Today : DateTime.Parse(model.FromDate);
            //DateTime? ToDate = model.ToDate == null ? DateTime.Today : DateTime.Parse(model.ToDate);
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (model.FromDate != null)
            {
                FromDate = DateTime.Parse(model.FromDate);
            }

            if (model.ToDate != null)
            {
                ToDate = DateTime.Parse(model.ToDate);
            }
            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

            List<vwAllOrders> cm = unitWork.vwAllOrdersManager.GetNotDelAllByParam(FromDate, ToDate, StoreId, ItemId, EmpId, DeptId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "AllOrdersItemsRpt.rdlc";
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });

        }

        public ActionResult ShowQRcodeReport(long? InOrderDetId)
        {
            List<InOrdersDetails> cm = unitWork.InOrdersDetailsManager.GetNotDelAll().Where(c => c.InOrderDetId == InOrderDetId).ToList();
            //InOrdersDetails cm = unitWork.InOrdersDetailsManager.GetById(InOrderDetId);
            if (cm.Count > 0)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                CurrentUser = profileData.LoginName;
                QRCode = cm[0].Item_BarCode;

                QRCode = "Date: " + DateTime.Now.ToString() + "\n" + "\n" + "Printed by: " +
                CurrentUser + "\n" + "OrderId: " +
                cm[0].InOrderDetId.ToString() + "\n" + "Order Date :" +
                cm[0].InOrders.InOrderDate.Value.ToShortDateString();

            }
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "QrCodeRpt.rdlc", DataSetName = "ItemStockDataSet", Flag = 1 });


            //Image1.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String(QRBytes);
        }
        public ActionResult ShowBarcodeReport(long? InOrderDetId)
        {
            List<InOrdersDetails> cm = unitWork.InOrdersDetailsManager.GetNotDelAll().Where(c => c.InOrderDetId == InOrderDetId).ToList();
            //InOrdersDetails cm = unitWork.InOrdersDetailsManager.GetById(InOrderDetId);
            if (cm.Count > 0)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                CurrentUser = profileData.LoginName;
                QRCode = cm[0].Item_BarCode;

                //QRCode = "Date: " + DateTime.Now.ToString() + "\n" + "\n" + "Printed by: " +
                //CurrentUser + "\n" + "OrderId: " +
                //cm[0].OutOrderId.ToString() + "\n" + "Order Date :" +
                //cm[0].OutOrderDate.Value.ToShortDateString();

            }
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "QrCodeRpt.rdlc", DataSetName = "ItemStockDataSet", Flag = 3 });
            //Image1.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String(QRBytes);
        }
        public ActionResult PrintBarcode(long InOrderDetId)
        {
            ReportViewModel model = new ReportViewModel();
            InOrdersDetails InOrdersDetailsObj = unitWork.InOrdersDetailsManager.GetById(InOrderDetId);
            string ItemBarCode = "";
            string ItemName = "";
            string ItemPrice = "";
            string OrderDate = "";
            string OrderNo = "";
            if (InOrdersDetailsObj != null)
            {
                OrderDate = InOrdersDetailsObj.InOrders.InOrderDate.GetValueOrDefault().ToShortDateString();
                OrderNo = InOrdersDetailsObj.InOrders.PurchaseNo;
                ItemBarCode = InOrdersDetailsObj.Item_BarCode;
                ItemPrice = InOrdersDetailsObj.CostPrice.ToString();

                long StockId = InOrdersDetailsObj.StockId.GetValueOrDefault();
                //tbl_ItemsStock tbl_ItemsStockObj = unitWork.ItemsStockManager.GetById(StockId);
                if (InOrdersDetailsObj.tbl_ItemsStock != null)
                {
                    ItemName = InOrdersDetailsObj.tbl_ItemsStock.Item_tbl.Item_Name;
                    //ItemName = InOrdersDetailsObj.tbl_ItemsStock.Item_tbl.Item_Name; ;
                }
            }
            var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            var defaultPrinterId = allSetupList.Select(m => m.Id).FirstOrDefault();
            model.PrinterName = allSetupList.Select(m => m.PrinterName).FirstOrDefault();
            var defaultNoOfPaper = allSetupList.Select(m => m.NofPaper).FirstOrDefault();
            model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", defaultPrinterId);

            model.NofPapers = defaultNoOfPaper.Value;
            model.ItemName = ItemName;
            model.ItemBarcode = ItemBarCode;
            model.ItemPrice = ItemPrice;

            model.OrderNo = OrderNo;
            model.OrderDate = OrderDate;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\

            string zpl_string =
                "^XA"//^LH230,15"
                + "^FO10,10^BY2"
                + "^BCN,100,Y,N,N"
                 + "^FD" + ItemBarCode + "^FS"
                + "^CI28^FT50,150^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                + "^A0N,20,20" // Font Size
                + "^FO15,165^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                + "^XZ";
            model.ZplCode = zpl_string;
            string ZPLQrCode =
                "^XA"
                 //+ "^FO35,20^BY2"
                 + "^FO2,10"
               + "^FO150,3^BQN,2,5^FDQA," + ItemBarCode + "^FS" // set QRcode
                + "^CI28^FT50,147^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                + "^A0N,20,20" // Font Size
                + "^FO15,160^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                + "^XZ";
            model.ZplQRCode = ZPLQrCode;
            return View(model);
        }

        [HttpPost]
        public ActionResult PrintBarcode(ReportViewModel model)
        {
            Connection connection = null;
            string error = "";
            try
            {
                var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
                model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", model.PrinterId);
                //Print(model);
            //    int No = model.NofPapers;
            //    string ItemName = model.ItemName;
            //    string ItemBarcode = model.ItemBarcode;
            //    string ItemPrice = model.ItemPrice;
            //    string PrinterMName = model.PrinterName;// form1["PrinterId"].ToString();
            //    string OrderNo = model.OrderNo;
            //    string OrderDate = model.OrderDate;
            //    //List<DiscoveredPrinter> printerList = GetUSBPrinters();
            //    //string zpl_string =
            //    //"^XA"
            //    //+ "^LL3000"
            //    //+ "^XGMyFormat^FS"
            //    //+ "^FO20,20^AE^FDTesting^FS"
            //    //+ "^XZ";

                //    string zpl_string =
                //    "^XA"

                //    //+ "^BY2,2,95"

                //    //+ "^FO1,30^BC^FD"+ ItemBarcode+"^FS"
                //    //+ "^FO10,150^FD"+ ItemName+"^FS"

                //    + "^FO245,20^BY2"
                //    + "^BCN,100,Y,N,N"
                //     + "^FD" + ItemBarcode + "^FS"
                //    //+ "^A0N,15,15"
                //    //+ "^FO20,150^FD" + ItemName + "^FS"
                //    + "^CI28^FT280,160^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                //    //+ @"^FPH,3^FT250,160^A@N,20,20,TT0003M_^FH\^CI28^FD" + ItemName + "^FS^CI27"
                //    + "^A0N,20,20" // Font Size
                //    + "^FO270,180^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                //    + "^XZ";
                //    //if (printerList.Count > 0)
                //    //{


                //    //error = "before";

                //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(zpl_string);
                //    var encodedString = System.Convert.ToBase64String(plainTextBytes);
                //    model.EditMode = encodedString;
                //    connection = new DriverPrinterConnection(PrinterMName);
                //    //PrintFile(model);
                //    error = "before";
                //    if (connection != null)
                //    {
                //        connection.Open();
                //        // in this case, we arbitrarily are printing to the first found printer  
                //        //DiscoveredPrinter discoveredPrinter = printerList[0];
                //        //Connection connection = discoveredPrinter.GetConnection();
                //        //connection.Open();
                //        error = "after open";
                //        for (int i = 0; i < No; i++)
                //        {
                //            connection.Write(Encoding.UTF8.GetBytes(zpl_string));
                //        }
                //        connection.Close();
                //        TempData["Message"] = null;
                //    }
            }
            catch
            {
                //if (connection != null)
                //{
                //    connection.Close();
                //    TempData["Message"] = "<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + Resources.DefaultResource.Error + " " + error + "  :  " + Resources.DefaultResource.PrintingError + "</h6></div>";
                //}
            }


            return View(model);
        }
        private void PrintLocal(ReportViewModel model)
        {
            Connection connection = null;
            string error = "";
            try
            {
                var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
                model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", model.PrinterId);

                int No = model.NofPapers;
                string ItemName = model.ItemName;
                string ItemBarcode = model.ItemBarcode;
                string ItemPrice = model.ItemPrice;
                string PrinterMName = model.PrinterName;// form1["PrinterId"].ToString();
                string OrderNo = model.OrderNo;
                string OrderDate = model.OrderDate;
                //List<DiscoveredPrinter> printerList = GetUSBPrinters();
                //string zpl_string =
                //"^XA"
                //+ "^LL3000"
                //+ "^XGMyFormat^FS"
                //+ "^FO20,20^AE^FDTesting^FS"
                //+ "^XZ";

                string zpl_string =
                "^XA"

                //+ "^BY2,2,95"

                //+ "^FO1,30^BC^FD"+ ItemBarcode+"^FS"
                //+ "^FO10,150^FD"+ ItemName+"^FS"

                + "^FO245,20^BY2"
                + "^BCN,100,Y,N,N"
                 + "^FD" + ItemBarcode + "^FS"
                //+ "^A0N,15,15"
                //+ "^FO20,150^FD" + ItemName + "^FS"
                + "^CI28^FT280,160^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                //+ @"^FPH,3^FT250,160^A@N,20,20,TT0003M_^FH\^CI28^FD" + ItemName + "^FS^CI27"
                + "^A0N,20,20" // Font Size
                + "^FO270,180^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                + "^XZ";
                //if (printerList.Count > 0)
                //{


                //error = "before";

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(zpl_string);
                var encodedString = System.Convert.ToBase64String(plainTextBytes);
                model.EditMode = encodedString;
                connection = new DriverPrinterConnection(PrinterMName);
                //PrintFile(model);
                //ZebraImageI image = ZebraImageFactory.GetImage();

                error = "before";
                if (connection != null)
                {
                    connection.Open();
                    // in this case, we arbitrarily are printing to the first found printer  
                    //DiscoveredPrinter discoveredPrinter = printerList[0];
                    //Connection connection = discoveredPrinter.GetConnection();
                    //connection.Open();
                    error = "after open";
                    for (int i = 0; i < No; i++)
                    {
                        connection.Write(Encoding.UTF8.GetBytes(zpl_string));
                    }
                    connection.Close();
                    TempData["Message"] = null;
                }
            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                    TempData["Message"] = "<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + Resources.DefaultResource.Error + " " + error + "  :  " + Resources.DefaultResource.PrintingError + "</h6></div>";
                }
            }
        }
        private void Print(ReportViewModel model)
        {
            var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", model.PrinterId);

            int No = model.NofPapers;
            string ItemName = model.ItemName;
            string ItemBarcode = model.ItemBarcode;
            string ItemPrice = model.ItemPrice;
            string PrinterMName = model.PrinterName;// form1["PrinterId"].ToString();

            string OrderNo = model.OrderNo;
            string OrderDate = model.OrderDate;
            string s =
                "^XA"

                + "^FO245,20^BY2"
                + "^BCN,100,Y,N,N"
                 + "^FD" + ItemBarcode + "^FS"
                //+ "^A0N,15,15"
                //+ "^FO20,150^FD" + ItemName + "^FS"
                + "^CI28^FT280,160^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                //+ @"^FPH,3^FT250,160^A@N,20,20,TT0003M_^FH\^CI28^FD" + ItemName + "^FS^CI27"
                + "^A0N,20,20" // Font Size
                + "^FO270,180^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                + "^XZ";


            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            if (DialogResult.OK == pd.ShowDialog())
            {
                for (int i = 0; i < No; i++)
                {
                    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
                    TempData["Message"] = "<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + No.ToString() + " : " + Resources.DefaultResource.PrintingError + "</h6></div>";
                }
            }

        }

        private void PrintFile(ReportViewModel model)
        {
            var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", model.PrinterId);

            int No = model.NofPapers;
            string ItemName = model.ItemName;
            string ItemBarcode = model.ItemBarcode;
            string ItemPrice = model.ItemPrice;
            string PrinterMName = model.PrinterName;// form1["PrinterId"].ToString();

            string OrderNo = model.OrderNo;
            string OrderDate = model.OrderDate;
            string s =
                "^XA"

                + "^FO245,20^BY2"
                + "^BCN,100,Y,N,N"
                 + "^FD" + ItemBarcode + "^FS"
                //+ "^A0N,15,15"
                //+ "^FO20,150^FD" + ItemName + "^FS"
                + "^CI28^FT280,160^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                //+ @"^FPH,3^FT250,160^A@N,20,20,TT0003M_^FH\^CI28^FD" + ItemName + "^FS^CI27"
                + "^A0N,20,20" // Font Size
                + "^FO270,180^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                + "^XZ";
            string command = s;
            Byte[] buffer = new byte[command.Length];
            buffer = System.Text.Encoding.ASCII.GetBytes(command);
            // Use the CreateFile external func to connect to the LPT1 port
            SafeFileHandle printer = CreateFile(PrinterMName, FileAccess.ReadWrite, 0, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            // Aqui verifico se a impressora é válida
            if (printer.IsInvalid == true)
            {
                return;
            }

            // Open the filestream to the lpt1 port and send the command
            FileStream lpt1 = new FileStream(printer, FileAccess.ReadWrite);
            lpt1.Write(buffer, 0, buffer.Length);
            // Close the FileStream connection
            lpt1.Close();


            //for (int i = 0; i < No; i++)
            //{
            //    RawPrinterHelper.SendStringToPrinter(PrinterMName, s);
            //    TempData["Message"] = "<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + No.ToString() + " : " + Resources.DefaultResource.PrintingError + "</h6></div>";
            //}
            //}

        }


        public ActionResult ItemsStockTotalReport()
        {
            ReportViewModel model = new ReportViewModel();
          

            /*List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();*/
            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);


            var allMainCatList = unitWork.CatMainManager.GetNotDelAll().ToList();
            var defaultMainCatId = 0;// allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.CatMain  = new SelectList(allMainCatList, "CatMain_Id", "CatMain_Name", Resources.DefaultResource.ListChoose);
                     

            //get all Categories according to defaultCountryId
            var allCategoryList = unitWork.CategoryManager.GetNotDelAll().Where(m => m.CatMain_Id  == defaultMainCatId).ToList();
            var defaultCatId = 0;// allCategoryList.Select(m => m.Cat_Id).FirstOrDefault();
            model.Category = new SelectList(allCategoryList, "Cat_Id", "Cat_Name", Resources.DefaultResource.ListChoose);

            var allSubCategoryList = unitWork.CatSubManager.GetNotDelAll().Where(m => m.Cat_Id == defaultCatId).ToList();
            var defaultSubCatId = 0;// allSubCategoryList.Select(m => m.CatSub_Id).FirstOrDefault();
            model.SubCategory  = new SelectList(allSubCategoryList, "CatSub_Id", "CatSub_Name", Resources.DefaultResource.ListChoose);

            var allItemsList = unitWork.ItemsManager.GetNotDelAll();//.Where(m => m.CatSub_Id == defaultSubCatId).ToList();
           
           model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", Resources.DefaultResource.ListChoose);


            //var allItemsList = unitWork.ItemsManager.GetNotDelAll().ToList();

            //model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", model.ItemId);

            return View(model);
        }


        [HttpPost]
        public ActionResult ItemsStockTotalReport(ReportViewModel model, int ? DetailsFlag)
        {

            //long? StoreId = model.StoreId;
            long[] StoresIds = model.Inventors;
            long? ItemId = model.ItemId;
            long? CatId = model.Cat_Id;
            long? SubCatId = model.CatSub_Id;
            long? MainCatId = model.MainCatId ;
            List<tbl_ItemsStock> cm;
            if (StoresIds == null) { StoresIds = new long[1]; StoresIds[0] = -1; }
               cm = unitWork.ItemsStockManager.GetByInventoryParam(ItemId, StoresIds,MainCatId,CatId,SubCatId).ToList();
                //string ReportName,string DataSetName, IEnumerable dataSourceValue
                TempData["list"] = cm;
                string ReportName = "ItemsStockTotalRpt.rdlc";
                if (DetailsFlag !=null)
                {
                    ReportName= "ItemsStockTotalDetailsRpt.rdlc";
                }
               

                return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });
           
        }




        public ActionResult PrintJs()
        {

            return View();
        }

        public ActionResult PrintStockBarcode(long StockId)
        {
            ReportViewModel model = new ReportViewModel();
            tbl_ItemsStock StocksObj = unitWork.ItemsStockManager.GetById(StockId);
            string ItemBarCode = "";
            string ItemName = "";
            string ItemPrice = "";
            string OrderDate = "";
            string OrderNo = "";
            string Qty = "";
            if (StocksObj != null)
            {
                //OrderDate = InOrdersDetailsObj.InOrders.InOrderDate.GetValueOrDefault().ToShortDateString();
                //OrderNo = InOrdersDetailsObj.InOrders.PurchaseNo;
                ItemBarCode = StocksObj.Item_BarCode;
                //ItemPrice = InOrdersDetailsObj.CostPrice.ToString();

                 //= InOrdersDetailsObj.StockId.GetValueOrDefault();
                //tbl_ItemsStock tbl_ItemsStockObj = unitWork.ItemsStockManager.GetById(StockId);
                if (StocksObj != null)
                {
                    ItemName = StocksObj.Item_tbl.Item_Name;
                    if (StocksObj.ItemQty != null)
                    {
                        Qty = StocksObj.ItemQty.ToString();
                    }
                    //ItemName = InOrdersDetailsObj.tbl_ItemsStock.Item_tbl.Item_Name; ;
                }
            }
            var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            var defaultPrinterId = allSetupList.Select(m => m.Id).FirstOrDefault();
            model.PrinterName = allSetupList.Select(m => m.PrinterName).FirstOrDefault();
            var defaultNoOfPaper = allSetupList.Select(m => m.NofPaper).FirstOrDefault();
            model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", defaultPrinterId);

            model.NofPapers = defaultNoOfPaper.Value;
            model.ItemName = ItemName;
            model.ItemBarcode = ItemBarCode;
            model.ItemPrice = ItemPrice;

            model.OrderNo = OrderNo;
            model.OrderDate = OrderDate;
            model.Qty = Qty;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\

            string zpl_string =
                "^XA"//^LH230,15"
                + "^FO10,10^BY2"
                + "^BCN,100,Y,N,N"
                 + "^FD" + ItemBarCode + "^FS"
                + "^CI28^FT50,150^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                + "^A0N,20,20" // Font Size
                + "^FO15,165^FD" + "Qty:" + Qty /*+ "kd -" + OrderDate + "- " + OrderNo*/ + "^FS"
                + "^XZ";
            model.ZplCode = zpl_string;
            string ZPLQrCode =
                "^XA"
                 //+ "^FO35,20^BY2"
                 + "^FO2,10"
               + "^FO150,3^BQN,2,5^FDQA," + ItemBarCode + "^FS" // set QRcode
                + "^CI28^FT50,147^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                + "^A0N,20,20" // Font Size
                + "^FO15,160^FD" + "Qty:" + Qty /*+ "kd -" + OrderDate + "- " + OrderNo*/ + "^FS"
                + "^XZ";
            model.ZplQRCode = ZPLQrCode;
            return View(model);
        }

        [HttpPost]
        public ActionResult PrintStockBarcode(ReportViewModel model)
        {
            Connection connection = null;
            string error = "";
            try
            {
                var allSetupList = unitWork.SetupManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
                model.PrinterNames = new SelectList(allSetupList, "Id", "PrinterName", model.PrinterId);
                //Print(model);
                //    int No = model.NofPapers;
                //    string ItemName = model.ItemName;
                //    string ItemBarcode = model.ItemBarcode;
                //    string ItemPrice = model.ItemPrice;
                //    string PrinterMName = model.PrinterName;// form1["PrinterId"].ToString();
                //    string OrderNo = model.OrderNo;
                //    string OrderDate = model.OrderDate;
                //    //List<DiscoveredPrinter> printerList = GetUSBPrinters();
                //    //string zpl_string =
                //    //"^XA"
                //    //+ "^LL3000"
                //    //+ "^XGMyFormat^FS"
                //    //+ "^FO20,20^AE^FDTesting^FS"
                //    //+ "^XZ";

                //    string zpl_string =
                //    "^XA"

                //    //+ "^BY2,2,95"

                //    //+ "^FO1,30^BC^FD"+ ItemBarcode+"^FS"
                //    //+ "^FO10,150^FD"+ ItemName+"^FS"

                //    + "^FO245,20^BY2"
                //    + "^BCN,100,Y,N,N"
                //     + "^FD" + ItemBarcode + "^FS"
                //    //+ "^A0N,15,15"
                //    //+ "^FO20,150^FD" + ItemName + "^FS"
                //    + "^CI28^FT280,160^A@N,20,20,E:TT0003M_.FNT^PA1,1,1,1^FH^FD" + ItemName + "^FS"
                //    //+ @"^FPH,3^FT250,160^A@N,20,20,TT0003M_^FH\^CI28^FD" + ItemName + "^FS^CI27"
                //    + "^A0N,20,20" // Font Size
                //    + "^FO270,180^FD" + "Price:" + ItemPrice + "kd -" + OrderDate + "- " + OrderNo + "^FS"
                //    + "^XZ";
                //    //if (printerList.Count > 0)
                //    //{


                //    //error = "before";

                //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(zpl_string);
                //    var encodedString = System.Convert.ToBase64String(plainTextBytes);
                //    model.EditMode = encodedString;
                //    connection = new DriverPrinterConnection(PrinterMName);
                //    //PrintFile(model);
                //    error = "before";
                //    if (connection != null)
                //    {
                //        connection.Open();
                //        // in this case, we arbitrarily are printing to the first found printer  
                //        //DiscoveredPrinter discoveredPrinter = printerList[0];
                //        //Connection connection = discoveredPrinter.GetConnection();
                //        //connection.Open();
                //        error = "after open";
                //        for (int i = 0; i < No; i++)
                //        {
                //            connection.Write(Encoding.UTF8.GetBytes(zpl_string));
                //        }
                //        connection.Close();
                //        TempData["Message"] = null;
                //    }
            }
            catch
            {
                //if (connection != null)
                //{
                //    connection.Close();
                //    TempData["Message"] = "<div class=\"alert alert-warning alert-dismissable\"> <h6><i class=\"fa fa-times-circle\"></i> " + Resources.DefaultResource.Error + " " + error + "  :  " + Resources.DefaultResource.PrintingError + "</h6></div>";
                //}
            }


            return View(model);
        }

    }
        

}

