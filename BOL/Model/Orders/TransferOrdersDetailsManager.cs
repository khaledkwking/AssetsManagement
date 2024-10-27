using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class TransferOrdersDetailsManager : Repository<TransferOrdersDetails>
    {

        public TransferOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(TransferOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            TransferOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<TransferOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<TransferOrdersDetails> GetByOrderId(int OrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.TransferOrderId == OrderId).ToList();

        }
        public List<TransferOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}