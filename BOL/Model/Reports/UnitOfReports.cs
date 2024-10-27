using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace BOL.Model
{

    public  class UnitOfReports
    {
        // Added properties:
        private UnitOfWork u;
        public UnitOfReports()
        {
             u = new UnitOfWork();
        }
        public static List<Building_tbl> GetAllBuilding()
        {
           
            var entities = new List<Building_tbl>();
            return entities.ToList();
        }
        public static List<tbl_ItemsStock> GetAllItemStock()
        {
            
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<tbl_ItemsStock>();
            return entities.ToList();
        }
        public static List<vwOutOrderDetails> GetOutOrderItems()
        {
            UnitOfWork UWork = new UnitOfWork();
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwOutOrderDetails>();
            return entities.ToList();
        }
        public static List<vwInOrderDetails> GetInOrderItems()
        {
           
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwInOrderDetails>();
            return entities.ToList();
        }
        public static List<vwDestroyOrdersDetails> GetDestoryOrderItems()
        {
            
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwDestroyOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwHandOverOrdersDetails> GetHandOverOrderItems()
        {
            
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwHandOverOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwTransferOrdersDetails> GetTransferOrderItems()
        {
           
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwTransferOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwChangeQuantityOrdersDetails> GetChangeQuantityOrderItems()
        {
            
            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwChangeQuantityOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwAllOrders> GetAllOrderItems()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwAllOrders>();
            return entities.ToList();
        }
        public static List<vwReturnInOrdersDetails> GetReturnInOrderItems()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwReturnInOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwReturnOutOrdersDetails> GetReturnOutOrderItems()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwReturnOutOrdersDetails>();
            return entities.ToList();
        }
        public static List<vwItemsStock> GetvwItemsStock()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwItemsStock>();
            return entities.ToList();
        }
        public static List<vwEmpRooms> vwEmpRooms()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<vwEmpRooms>();
            return entities.ToList();
        }
        public static List<InOrdersDetails> GetInOrderDetials()
        {

            //var entities = UWork.ItemsStockManager.GetNotDelAll();
            var entities = new List<InOrdersDetails >();
            return entities.ToList();
        }
    }
    
}
