using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class UnitsManager : Repository<Unit_tbl>
    {

        public UnitsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(Unit_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Unit_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Unit_tbl> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Unit_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Unit_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Unit_tbl> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

    }
}