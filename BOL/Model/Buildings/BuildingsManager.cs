using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class BuildingsManager : Repository<Building_tbl>
    {

      

        public BuildingsManager(GPFAssetsEntities ctx) : base(ctx)
        {


        }
        public override bool Delete(Building_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Building_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Building_tbl> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Building_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Building_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Building_tbl> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }

    }
}