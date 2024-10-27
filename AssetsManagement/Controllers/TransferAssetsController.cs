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
    public class TransferAssetsController : Controller
    {
        // reader variables
        
        //-------------------------------end of reader variables---------------------------------------
        public int Qty = 0;
        public int Size_Of_Page = 15;
        public int No_Of_Page = 1;
        private UnitOfWork unitWork = new UnitOfWork();
        // GET: Orders
        
        [UserPermissionAttribute(AllowFeature = "TransferAssets", AllowPermission = "Accessing")]
        public ActionResult TransferAssetsList(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            //List<Car> carList = null;
            TransferAssetsViewModel model = new TransferAssetsViewModel();
          
            //model.FromDate = DateTime.Today.ToShortDateString();
            //model.ToDate = DateTime.Today.ToShortDateString();
            return (GetData(model, Sorting_Order, Search_Data, Filter_Value, Page_No));
          
        }

        [HttpPost]
        [UserPermissionAttribute(AllowFeature = "TransferAssets", AllowPermission = "Accessing")]
        public ActionResult TransferAssetsList(TransferAssetsViewModel model,string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
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
            return (GetData(model, Sorting_Order, Search_Data, Filter_Value, Page_No));
        }

        public ActionResult GetData(TransferAssetsViewModel model, string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {
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
            List<TransferAssets> TransferAssetsList = unitWork.TransferAssetsManager.GetNotDelAllByParam(FromDate, ToDate, Search_Data).OrderByDescending(m => m.TansId).ToList();
            model.SelectedItem = null;
            //List<Unit_tbl> UnitList = unitWork.BuildingsManager.GetNotDelAll().ToList();
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "TansId" : "";
            ViewBag.SortingModel = Sorting_Order == "TansId" ? "TansDate" : "TansId";

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
                TransferAssetsList = unitWork.TransferAssetsManager.GetCastByName(Search_Data);
                //carList = carList.Where(stu => stu.CarNo.Contains(Search_Data)).ToList();
            }
            switch (Sorting_Order)
            {
                case "TansId":
                    TransferAssetsList = TransferAssetsList.OrderByDescending(stu => stu.TansId).ToList();
                    break;
                case "TansDate":
                    TransferAssetsList = TransferAssetsList.OrderBy(stu => stu.TransDate).ToList();
                    break;

                default:
                    TransferAssetsList = TransferAssetsList.OrderByDescending(stu => stu.TansId).ToList();
                    //carList =
                    break;
            }


            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            model.TransferAssets = TransferAssetsList.ToPagedList(No_Of_Page, Size_Of_Page);
           
               return View(model);
           

        }

        public ActionResult AssetTransferListReport(long? TansId)
        //public ActionResult AssetTransferListReport(ReportViewModel model)
        {

            //long? RoomId=model.RoomId ;
            ////long? StoreId=model.StoreId ;
            //int? FloorId = model.FloorId ;
            //int? DeptId=model.DeptId  ;
            //int? EmpId= model.EmpId  ;
            int searchType = 1;// model.searchType;


            List<vwTransferAssests> cm = unitWork.vwTransferAssestsManager.GetNotDelAllById(TansId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "TransferAssestsRpt.rdlc";
            switch (searchType)
            {
                case 1:
                    ReportName = "TransferAssestsRpt.rdlc";
                    break;
                case 2:
                    ReportName = "TransferAssestsRpt.rdlc";
                    break;
            }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });

        }


    }

    }