using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class ChangeQuantityOrdersDetailsManager : Repository<ChangeQuantityOrdersDetails>
    {

        public ChangeQuantityOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(ChangeQuantityOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            ChangeQuantityOrdersDetails st = GetById(id);

            return Update(st);
        }
        public List<ChangeQuantityOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<ChangeQuantityOrdersDetails> GetByOrderId(long ChangeOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.ChangeOrderId == ChangeOrderId).ToList();

        }
        public List<ChangeQuantityOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}