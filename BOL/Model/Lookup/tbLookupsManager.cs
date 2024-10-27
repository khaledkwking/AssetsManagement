using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class tbLookupsManager : Repository<tbLookups>
    {

        public tbLookupsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(tbLookups entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            tbLookups st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<tbLookups> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.LookupString.ToUpper().Contains(UnitSearch.ToUpper()) || c.LookupStringAr.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<tbLookups> GetByValue(int Value)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Value == Value).ToList();
        }
    }
}