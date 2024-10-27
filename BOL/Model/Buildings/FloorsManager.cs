using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class FloorsManager : Repository<Floor_tbl>
    {

        public FloorsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(Floor_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Floor_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Floor_tbl> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Floor_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Floor_NameEn.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Floor_tbl> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}