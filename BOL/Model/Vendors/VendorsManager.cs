using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class VendorsManager : Repository<Vendors>
    {

      

        public VendorsManager(GPFAssetsEntities ctx) : base(ctx)
        {


        }
        public override bool Delete(Vendors entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            Vendors st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Vendors> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.VendorName .ToUpper().Contains(UnitSearch.ToUpper()) || c.VendorNameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Vendors> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

    }
}