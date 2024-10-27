using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwTransferOrdersDetailsManager : Repository<vwTransferOrdersDetails>
    {

        public vwTransferOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
                
        public List<vwTransferOrdersDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate
            , long? StoreId,long? StoreToId, long? ItemId, int? CatMain_Id,int userId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwTransferOrdersDetails> List;
                if (FromDate != null && Todate != null)
                {
                    //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
                    List = GetAll().Where(c => (DbFunctions.TruncateTime(c.TransferOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.TransferOrderDate) <= DbFunctions.TruncateTime(Todate))
                 && ((c.StoreId_From == StoreId || StoreId == null)
                 && (c.StoreId_To == StoreToId || StoreToId == null)
                 && (c.ItemId == ItemId || ItemId == null)
                 && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
                 && (c.IsDeleted == false || c.IsDeleted == null))
                ).ToList();
            }
            else
            {
                List = GetAll().Where(c=> ((c.StoreId_From == StoreId || StoreId == null)
                                && (c.StoreId_To == StoreToId || StoreToId == null)
                                && (c.ItemId == ItemId || ItemId == null)
                                && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
                                && (c.IsDeleted == false || c.IsDeleted == null))
                               ).ToList();
            }
            UnitOfWork UWork = new UnitOfWork();
            List<Room_tbl> Inventories = UWork.RoomsManager.GetUserInventories(userId).ToList();
            var listOfRoomId = Inventories.Select(r => r.Room_Id).ToList();

            List<vwTransferOrdersDetails> ListNew = List.Where(c => listOfRoomId.Contains(c.StoreId_From.GetValueOrDefault())).ToList();

            return ListNew;
            //return GetAllDetails(List);
        }
      
        public List<vwTransferOrdersDetails> GetNotDelByOrderId(long? DestroyOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwTransferOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.TransferOrderId  == DestroyOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            return List;
            //return GetAllDetails(List);
        }

    }
}