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


namespace AssetsManagement.Controllers
{
    public class ReportsItemsStockController : Controller
    {
        string CurrentUser { get; set; }
                
        private UnitOfWork unitWork = new UnitOfWork();
       
        
        public ActionResult ItemsStockCardReport()
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
           
            var allFloorsList = unitWork.FloorsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Floors = new SelectList(allFloorsList, "Floor_Id", "Floor_Name", model.FloorId);

            var allRoomsList = unitWork.RoomsManager.GetOnlyRoomsAll().OrderByDescending(m => m.Room_Id).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.Rooms = new SelectList(allRoomsList, "Room_Id", "Room_Name", model.RoomId);



            var allVendorsList = unitWork.VendorsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            var defaultVendorId = allDeptsList.Select(m => m.Id).FirstOrDefault();
            model.vWVendors  = new SelectList(allVendorsList, "VendorId", "VendorName", model.VendorId);
          

            var allContractsList = unitWork.VendorContractsManager.GetNotDelAll().Where(m => m.VendorId == defaultVendorId).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.VwContracts  = new SelectList(allContractsList, "ContractId", "ContractName", model.ContractId );


            //List<OutOrdersDetails> cm = unitWork.OutOrdersDetailsManager.GetNotDelAll().ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            ////string ReportName,string DataSetName, IEnumerable dataSourceValue
            //TempData["list"] = cm;
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }

       
        [HttpPost]
        public ActionResult ItemsStockCardReport(ReportViewModel model)
        {

         long? RoomId=model.RoomId ;
         //long? StoreId=model.StoreId ;
         int? FloorId = model.FloorId ;
         int? DeptId=model.DeptId  ;
         int? EmpId= model.EmpId  ;
         int searchType = model.searchType;
         string PrintCardValue = Request.Form["CmdPrint"];
        
                //model.ToDate = DateTime.Today.ToShortDateString();
                //model.FromDate = DateTime.Today.ToShortDateString();

                List<vwItemsStock> cm = unitWork.vwItemsStockManager.GetNotDelAllByParam 
         (RoomId, FloorId, EmpId,DeptId , searchType).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "AssetItemsCardRpt.rdlc";
            if (PrintCardValue == "PrintCard")
            {
                ReportName = "AssetItemsMalyaCardRpt.rdlc";
            }
            //switch (searchType)
            // {
            //     case 1:
            //         ReportName = "AssetItemsCardRpt.rdlc";
            //         break;
            //     case 2:
            //         ReportName = "AssetItemsCardRpt.rdlc";
            //         break;
            // }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });
            
        }


        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
           
            ReportViewModel model = new ReportViewModel();
            DepartementsViewModel DeptModel = new DepartementsViewModel();
            if (type == "DeptId") { type = "DeptId"; }
            switch (type)
            {
                case "DeptId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.Employees = DeptModel.vwEmployees;
                    break;
                case "FloorId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.Rooms  = DeptModel.vwRooms;
                    break;
                case "VendorId":
                    DeptModel.setDropDrownList(type, value, Resources.DefaultResource.ListChoose);
                    model.VwContracts   = DeptModel.vwContracts ;
                    break;
            }


            return Json(model);
        }
    
     
        public ActionResult ItemsStockListReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.RFIDFlag  = false;
            //var allDeptsList = unitWork.DepartmentManager.GetNotDelAll().OrderByDescending(m => m.Id).ToList();
            //var defaultDeptId = allDeptsList.Select(m => m.Id).FirstOrDefault();
            //model.Departments = new SelectList(allDeptsList, "Id", "Name", model.DeptId);
            //get all Categories according to defaultCountryId

            //var allEmployeesList = unitWork.EmployeesManager.GetNotDelAll().Where(m => m.Department_Id == defaultDeptId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            //model.Employees = new SelectList(allEmployeesList, "Id", "FULL_NAME_AR", model.EmpId);
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
        public ActionResult ItemsStockListReport(ReportViewModel model)
        {

            //long? RoomId = model.RoomId;
            //long? StoreId=model.StoreId ;
            long? ItemId = model.ItemId;
            //int? DeptId = model.DeptId;
            //int? EmpId = model.EmpId;
            int? searchType = model.searchType;
            string RFIDFlag = bool.FalseString;
            if (model.RFIDFlag)
            {
                RFIDFlag=bool.TrueString;
            }
                      
            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();
            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (model.FromDate !=null)  FromDate = DateTime.Parse(model.FromDate);
            if (model.ToDate != null) ToDate = DateTime.Parse(model.ToDate);



            string ItemName =" ";
            List<vwItemsStock> cm = unitWork.vwItemsStockManager.GetNotDelExpiredDateByParam 
            (FromDate, ToDate, ItemId).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            if (ItemId !=null)
            {
                Item_tbl obj = unitWork.ItemsManager.GetById(ItemId);
                ItemName = obj.Item_Name;
            }
            TempData["list"] = cm;
            string ReportName = "ItemsListRpt.rdlc";
            switch (searchType)
            {
                case 1:
                    ReportName = "ItemsListRpt.rdlc";
                    break;
                case 2:
                    ReportName = "ItemsListRpt.rdlc";
                    break;
            }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet",Flag=2, Param= RFIDFlag,ItemName= ItemName });

        }

    }

}

