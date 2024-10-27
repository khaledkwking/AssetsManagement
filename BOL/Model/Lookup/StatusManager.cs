using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class StatusManager : Repository<Status>
    {

        public StatusManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(Status entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            Status st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Status> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.AName.ToUpper().Contains(UnitSearch.ToUpper()) || c.EName.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
    }
}