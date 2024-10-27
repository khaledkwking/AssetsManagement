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
using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;

namespace AssetsManagement.Controllers
{
    public class ReportsEmpRoomsController : Controller
    {
        string CurrentUser { get; set; }
        public static string QRCode;
        
        private UnitOfWork unitWork = new UnitOfWork();
        
        //public ActionResult EmptyRoomsReport()
        //{
        //    ReportViewModel model = new ReportViewModel();
        //    model.searchType = 1;

        //    var allBuildingList = unitWork.BuildingsManager.GetNotDelAll().OrderByDescending(m => m.Building_Id).ToList();
        //    //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
        //    model.Buildings = new SelectList(allBuildingList, "Building_Id", "Building_Name", model.BuildingId );

        //    var allFloorsList = unitWork.FloorsManager.GetNotDelAll().ToList();
        //    //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
        //    model.Floors = new SelectList(allFloorsList, "Floor_Id", "Floor_Name", model.FloorId );

        //    var allRoomTypesList = unitWork.tbLookupsManager.GetByValue(3).ToList();
        //    //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
        //    model.RoomTypes = new SelectList(allRoomTypesList, "LookupID", "LookupStringAr", model.RoomTypeId);

        //    //model.ToDate = DateTime.Today.ToShortDateString();
        //    //model.FromDate = DateTime.Today.ToShortDateString();

        //    //List<OutOrdersDetails> cm = unitWork.OutOrdersDetailsManager.GetNotDelAll().ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
        //    ////string ReportName,string DataSetName, IEnumerable dataSourceValue
        //    //TempData["list"] = cm;
        //    //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
        //    return View(model);
        //}

       
        //[HttpPost]
        //public ActionResult EmptyRoomsReport(ReportViewModel model)
        //{

        // int? BuildingId=model.BuildingId  ;
        // int? FloorId=model.FloorId  ;
        //    int [] RoomTypeId= model.members;// = model.RoomTypeId;
        //    //long? ItemId= model.ItemId ;
        //    //   int? CatMain_Id = model.MainCatId;
        //    //   int? searchType = model.searchType;

        //    //DateTime ? FromDate= model.FromDate==null ? DateTime.Today: DateTime .Parse(model.FromDate);
        //    //DateTime ? ToDate= model.ToDate == null ? DateTime.Today : DateTime.Parse(model.ToDate);

        //    //model.ToDate = DateTime.Today.ToShortDateString();
        //    //model.FromDate = DateTime.Today.ToShortDateString();

        //    List<vwEmpRooms> cm = unitWork.vwEmpRoomsManager.GetNotDelAllByParam(BuildingId, FloorId, RoomTypeId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
        //    //string ReportName,string DataSetName, IEnumerable dataSourceValue
        //    TempData["list"] = cm;
        //    string ReportName = "EmptyRoomsListRpt.rdlc";
        //   //switch (searchType)
        //   // {
        //   //     case 1:
        //   //         ReportName = "DestoryOrderItemsRpt.rdlc";
        //   //         break;
        //   //     case 2:
        //   //         ReportName = "DestoryOrderTotalItemsRpt.rdlc";
        //   //         break;
        //   // }
        //    return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });
            
        //}

        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            ReportViewModel model = new ReportViewModel();
            BuildingsViewModel BuildingsModel = new BuildingsViewModel();
            if (type == "BuildingId") { type = "BuildingId"; }
            switch (type)
            {
                case "BuildingId":
                    BuildingsModel.setDropDrownList(type, value);
                    model.Floors = BuildingsModel.Floors;
                    break;
            }


            return Json(model);
        }
        public ActionResult EmptyRoomsReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.searchType = 1;

            var allBuildingList = unitWork.BuildingsManager.GetNotDelAll().OrderByDescending(m => m.Building_Id).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.Buildings = new SelectList(allBuildingList, "Building_Id", "Building_Name", model.BuildingId);

            var allFloorsList = unitWork.FloorsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Floors = new SelectList(allFloorsList, "Floor_Id", "Floor_Name", model.FloorId);

            var allRoomTypesList = unitWork.tbLookupsManager.GetByValue(3).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.RoomTypes = new SelectList(allRoomTypesList, "LookupID", "LookupStringAr", model.RoomTypeId);

            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

            //List<OutOrdersDetails> cm = unitWork.OutOrdersDetailsManager.GetNotDelAll().ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            ////string ReportName,string DataSetName, IEnumerable dataSourceValue
            //TempData["list"] = cm;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }


        [HttpPost]
        public ActionResult EmptyRoomsReport(ReportViewModel model)
        {

            int? BuildingId = model.BuildingId;
            int? FloorId = model.FloorId;
            int? RoomTypeId = model.RoomTypeId;
            int [] RoomTypeIds = model.members;
            //long? ItemId= model.ItemId ;
            //   int? CatMain_Id = model.MainCatId;
            //   int? searchType = model.searchType;

            //DateTime ? FromDate= model.FromDate==null ? DateTime.Today: DateTime .Parse(model.FromDate);
            //DateTime ? ToDate= model.ToDate == null ? DateTime.Today : DateTime.Parse(model.ToDate);

            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

            List<vwEmpRooms> cm = unitWork.vwEmpRoomsManager.GetNotDelAllByParam(BuildingId, FloorId, RoomTypeIds).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "EmptyRoomsListRpt.rdlc";
            //switch (searchType)
            // {
            //     case 1:
            //         ReportName = "DestoryOrderItemsRpt.rdlc";
            //         break;
            //     case 2:
            //         ReportName = "DestoryOrderTotalItemsRpt.rdlc";
            //         break;
            // }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });

        }
        public ActionResult test()
        {
           
            return View();
        }




    }

}

