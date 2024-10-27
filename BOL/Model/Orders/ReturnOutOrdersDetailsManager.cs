using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ReturnOutOrdersDetailsManager : Repository<ReturnOutOrdersDetails>
    {

        public ReturnOutOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(ReturnOutOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            ReturnOutOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<ReturnOutOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.StockId.ToString().ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<ReturnOutOrdersDetails> GetByOrderId(long ReturnOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            UnitOfWork UWork = new UnitOfWork();
            List<ReturnOutOrdersDetails> List;
            List = GetNotDelAll().Where(c => c.ReturnOrderId == ReturnOrderId).ToList();

            foreach (var Item in List)
            {
                if (Item.OutOrdersDetails != null)
                {

                    int StockId = int.Parse(Item.OutOrdersDetails.StockId.ToString());
                    tbl_ItemsStock StockItem = UWork.ItemsStockManager.GetById(StockId);
                    Item.OutOrdersDetails.tbl_ItemsStock = StockItem;
                }

            }
            return List;

        }
        public List<ReturnOutOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}