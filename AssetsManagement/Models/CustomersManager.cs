using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

    public class CustomersManager : Repository<cust>
    {
        public CustomersManager (CARACCOUNTWebEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(cust entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            cust st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }

    public List<cust> GetCastByCust(string CustNo)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.civilid.ToUpper().Contains(CustNo.ToUpper()) || c.custname.ToUpper().Contains(CustNo.ToUpper())).ToList();
    }
}
