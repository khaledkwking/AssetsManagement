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
    public class ReportsInOrderController : Controller
    {
        string CurrentUser { get; set; }
        public static string QRCode;
        
        private UnitOfWork unitWork = new UnitOfWork();
        
        public ActionResult InOrderItemReport()
        {
            ReportViewModel model = new ReportViewModel();
            model.searchType = 1;


            //var allStoresList = unitWork.RoomsManager.GetInventoriesAll().ToList();
            int userId = SesssionUser.GetCurrentUserId();
            var allStoresList = unitWork.RoomsManager.GetUserInventories(userId).ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Inventories = new SelectList(allStoresList, "Room_Id", "Room_Name", model.StoreId);

            var allSupplierList = unitWork.SuppliersManager.GetNotDelAll ().ToList();
            //var defaultEmpId = allEmployeesList.Select(m => m.Id).FirstOrDefault();
            model.Suppliers = new SelectList(allSupplierList, "Sup_code", "Sup_Name", model.RoomId);

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
        public ActionResult InOrderItemReport(ReportViewModel model)
        {

         int? SupplierId=model.supplierId ;
         long? StoreId=model.StoreId ;
         long? ItemId= model.ItemId ;
         
         int? searchType = model.searchType;

            DateTime? FromDate=null;
            DateTime? ToDate=null;
            if (model.FromDate != null)
            {
                FromDate=DateTime.Parse(model.FromDate);
            }

            if (model.ToDate != null)
            {
                ToDate=DateTime.Parse(model.ToDate);
            }
            //DateTime ? ToDate= model.ToDate == null ? DateTime.Today : DateTime.Parse(model.ToDate);

            //model.ToDate = DateTime.Today.ToShortDateString();
            //model.FromDate = DateTime.Today.ToShortDateString();

         List<vwInOrderDetails> cm = unitWork.VwInOrderDetailsManager.GetNotDelAllByParam
         (FromDate, ToDate,SupplierId,StoreId,ItemId ).ToList();// (ItemId, DeptId, RoomId, EmpId).ToList();
            //string ReportName,string DataSetName, IEnumerable dataSourceValue
            TempData["list"] = cm;
            string ReportName = "InOrderItemsRpt.rdlc";
           switch (searchType)
            {
                case 1:
                    ReportName = "InOrderItemsRpt.rdlc";
                    break;
                case 2:
                    ReportName = "InOrderTotalItemsRpt.rdlc";
                    break;
            }
            return RedirectToAction("ShowReport", "Reports", new { ReportName = ReportName, DataSetName = "ItemStockDataSet" });
            
        }
     
               
        public ActionResult ShowInOrderReport(long? orderId)
        {
         
           List<vwOutOrderDetails> cm = unitWork.VwOutOrderDetailsManager.GetNotDelByOrderId(orderId);
           if (cm.Count >0)
            {
                var profileData = Session["UserProfile"] as SesssionUser;
                CurrentUser = profileData.LoginName; ;
                QRCode = "Date: " + DateTime.Now.ToString() + "\n" +  "\n" + "Printed by: " +
                CurrentUser + "\n" + "OrderId: " + 
                cm[0].OutOrderId.ToString() + "\n" + "Order Date :"+
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

      

    }

}

