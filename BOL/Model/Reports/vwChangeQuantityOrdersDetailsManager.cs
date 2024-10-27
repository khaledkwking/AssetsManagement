using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwChangeQuantityOrdersDetailsManager : Repository<vwChangeQuantityOrdersDetails>
    {

        public vwChangeQuantityOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        
        public List<vwChangeQuantityOrdersDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate
            , long? StoreId, long? ItemId, int? CatMain_Id)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwChangeQuantityOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            if (FromDate != null && Todate != null)
            {
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.ChangeOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.ChangeOrderDate) <= DbFunctions.TruncateTime(Todate))
             && ((c.StoreId_From == StoreId || StoreId == null)
             && (c.ItemId == ItemId || ItemId == null)
             && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
                return List;
            }
            else
            {
                List = GetAll().Where(c => ((c.StoreId_From == StoreId || StoreId == null)
            && (c.ItemId == ItemId || ItemId == null)
            && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
            && (c.IsDeleted == false || c.IsDeleted == null))
           ).ToList();
                return List;
            }
            //return GetAllDetails(List);
        }
      
        public List<vwChangeQuantityOrdersDetails> GetNotDelByOrderId(long? ChangeOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwChangeQuantityOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.ChangeOrderId  == ChangeOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            return List;
            //return GetAllDetails(List);
        }

    }
}