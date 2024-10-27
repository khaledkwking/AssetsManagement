using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class SuppliersManager : Repository<Suppliers_tbl>
    {

        public SuppliersManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(Suppliers_tbl entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            Suppliers_tbl st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<Suppliers_tbl> GetCastByName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Sup_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Sup_Ename.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<Suppliers_tbl> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}