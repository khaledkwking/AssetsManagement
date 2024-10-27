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
    public class ReportsHandOverOrdersController : Controller
    {
        string CurrentUser { get; set; }
        public static string QRCode;
        
        private UnitOfWork unitWork = new UnitOfWork();
        
        public ActionResult HandOverOrdersItemReport()
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

            var allMainCatList = unitWork.CatMainManager.GetNotDelAll().OrderByDescending(m => m.CatMain_Id).ToList();
            //var defaultDeptId = allMainCatList.Select(m => m.CatMain_Id).FirstOrDefault();
            model.CatMain = new SelectList(allMainCatList, "CatMain_Id", "CatMain_Name", model.MainCatId);

            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            //var allStoresList = unitWork.RoomsManager.GetInventoriesAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);

            var allItemsList = unitWork.ItemsManager.GetNotDelAll().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Items = new SelectList(allItemsList, "Item_Id", "Item_Name", model.ItemId);

            var allIReasonsList = unitWork.tbLookupsManager.GetByValue(1).ToList();
            model.HandOverReasons = new SelectList(allIReasonsList, "LookupID", "LookupStringAr", model.ItemId);
           
            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

           
            //return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderItemsRpt.rdlc", DataSetName = "ItemStockDataSet" });\
            return View(model);
        }

       
        [HttpPost]
        public ActionResult HandOverOrdersItemReport(ReportViewModel model)
        {

        
         long? StoreId=model.StoreId ;
         long? ItemId= model.ItemId ;
         int? CatMain_Id = model.MainCatId;
         int? searchType = model.searchType;
         int? ReasonId = model.ReasonId;
         int? DeptId = model.DeptId;
         int? EmpId = model.EmpId;
            //   DateTime ? FromDate= model.FromDate==null ? DateTime.Today: DateTime .Parse(model.FromDate);
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

            List<vwHandOverOrdersDetails> cm = unitWork.vwHandOverOrdersDetailsManager.GetNotDelAllByParam
         (FromDate, ToDate,ReasonId,StoreId,ItemId, CatMain_Id, EmpId,DeptId).ToList();// (ItemId, ).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "HandOverOrderItemsRpt.rdlc";
           switch (searchType)
            {
                case 1:
                    ReportName = "HandOverOrderItemsRpt.rdlc";
                    break;
                case 2:
                    ReportName = "HandOverOrderTotalItemsRpt.rdlc";
                    break;
            }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });
            
        }
     
               
        public ActionResult ShowDestroyOrderReport(long? orderId)
        {
         
           List<vwHandOverOrdersDetails> cm = unitWork.vwHandOverOrdersDetailsManager.GetNotDelByOrderId(orderId);
           if (cm.Count >0)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                CurrentUser = profileData.LoginName; ;
                QRCode = "Date: " + DateTime.Now.ToString() + "\n" +  "\n" + "Printed by: " +
                CurrentUser + "\n" + "OrderId: " + 
                cm[0].OverOrderId .ToString() + "\n" + "Order Date :"+
                 cm[0].OverOrderDate.Value.ToShortDateString();
                
            }
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderRpt.rdlc", DataSetName = "ItemStockDataSet", Flag = 1 });

           
            //Image1.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String(QRBytes);
        }
       

        public ActionResult EmptyOutOrderItemReport()
        {
            //List<vwOutOrderDetails> cm = new List<vwOutOrderDetails>();
            List<vwHandOverOrdersDetails> cm = unitWork.vwHandOverOrdersDetailsManager.GetNotDelByOrderId(57);
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;

            return RedirectToAction("ShowReport", "Reports", new { ReportName = "OutOrderEmptyRpt.rdlc", DataSetName = "ItemStockDataSet" });

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
            }


            return Json(model);
        }



    }

}

