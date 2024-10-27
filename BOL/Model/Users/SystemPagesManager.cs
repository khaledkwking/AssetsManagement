using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class SystemPagesManager : Repository<tbSystemPages>
    {

        public SystemPagesManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(tbSystemPages entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            tbSystemPages st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<tbSystemPages> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.PageName.ToUpper().Contains(UnitSearch.ToUpper()) || c.PageNameAr.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<tbSystemPages> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}