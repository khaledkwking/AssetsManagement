using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class InOrdersDetailsManager : Repository<InOrdersDetails>
    {

        public InOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(InOrdersDetails entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            InOrdersDetails st = GetById(id);

            return Update(st);
        }
        public InOrdersDetails GetByOrderDetId(long Id)
        {
            InOrdersDetails st = GetById(Id);
            st.ReturnInOrdersDetails = st.ReturnInOrdersDetails.Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
           
            return st; 
        }
        public List<InOrdersDetails> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Item_RFID.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<InOrdersDetails> GetByOrderId(long OrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.InOrderId == OrderId).ToList();

        }
        public List<InOrdersDetails> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

       

        
    }
}