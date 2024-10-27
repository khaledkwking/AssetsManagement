using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ReturnInOrdersDetailsManager : Repository<ReturnInOrdersDetails>
    {

        public ReturnInOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(ReturnInOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            ReturnInOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<ReturnInOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.StockId.ToString().ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<ReturnInOrdersDetails> GetByOrderId(long  ReturnOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            UnitOfWork UWork = new UnitOfWork();
            List<ReturnInOrdersDetails> List;
            List = GetNotDelAll().Where(c => c.ReturnOrderId == ReturnOrderId).ToList();

            foreach (var Item in List)
            {
                if (Item.InOrdersDetails != null)
                {

                    int StockId = int.Parse(Item.InOrdersDetails.StockId.ToString());
                    tbl_ItemsStock StockItem = UWork.ItemsStockManager.GetById(StockId);
                    Item.InOrdersDetails.tbl_ItemsStock = StockItem;
                }

            }
            return List;

        }
        public List<ReturnInOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
        public List<ReturnInOrdersDetails> GetOrderByDetId(long OrderDetId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.InOrderDetId== OrderDetId).ToList();
        }
    }
}