using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class EmployeesManager : Repository_HR<vwEmployees>
    {

        public EmployeesManager(GPFEmployeesEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(vwEmployees entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            vwEmployees st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<vwEmployees> GetCastByUnitName(string UnitSearch, Boolean AcativeFlag)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwEmployees> List = null;
            if (AcativeFlag)
            {
                List = GetDelAll().Where(c => c.FULL_NAME_AR.ToUpper().Contains(UnitSearch.ToUpper())
                                || c.FULL_NAME_En.ToUpper().Contains(UnitSearch.ToUpper())
                                //|| c.DeptTitle.ToUpper().Contains(UnitSearch.ToUpper())
                                || c.JobTitle.ToUpper().Contains(UnitSearch.ToUpper())
                                || c.Fingerprint_Id.ToUpper().Contains(UnitSearch.ToUpper())
                                ).ToList();
            }
            else
            {
                List = GetNotDelAll().Where(c => c.FULL_NAME_AR.ToUpper().Contains(UnitSearch.ToUpper())
                || c.FULL_NAME_En.ToUpper().Contains(UnitSearch.ToUpper())
                //|| c.DeptTitle.ToUpper().Contains(UnitSearch.ToUpper())
                || c.JobTitle.ToUpper().Contains(UnitSearch.ToUpper())
                || c.Fingerprint_Id.ToUpper().Contains(UnitSearch.ToUpper())
                || c.Email.ToUpper().Contains(UnitSearch.ToUpper())
                ).ToList();
            }
            //if (AcativeFlag)
            //{
            //    List = List.Where(c => c.Deleted_at != null).ToList();
            //}

            return List;
        }
        public List<vwEmployees> GetNotDelAll()
        {
            return GetAll().Where(c => c.Deleted_at == null).ToList();
        }
        public List<vwEmployees> GetEmployeeByEmpId(int EmpId)
        {
            return GetAll().Where(c => c.Id == EmpId ).ToList();
        }
        public List<vwEmployees> GetDelAll()
        {
            List < vwEmployees > EmpList= GetAll().Where(c => c.Deleted_at != null).ToList();
            return EmpList;
        }
    }
}