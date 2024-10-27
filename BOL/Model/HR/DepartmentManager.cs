using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class DepartmentManager : Repository_HR<vwDepartments>
    {

        public DepartmentManager(GPFEmployeesEntities ctx) : base(ctx)
        {

        }
        public override bool Delete(vwDepartments entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id, string Name, string address, int genderId)
        {
            vwDepartments st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<vwDepartments> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetNotDelAll().Where(c => c.Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.ManagerFullNameAr.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<vwDepartments> GetParentId(int ParentId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.Deleted_at == null && c.Parent_Id == ParentId).ToList();
        }
        public List<vwDepartments> GetNotDelAll()
        {
            return GetAll().Where(c => c.Deleted_at == null).ToList();
        }
        public List<vwDepartments> GetDeptById(int DeptId)
        {
            return GetAll().Where(c => c.Id == DeptId).ToList();
        }
        public List<vwDepartments> GetUserDepts(int CuruserId)
        {
            UnitOfWork unitWork = new UnitOfWork();
            List<vwDepartments> allDepts = GetNotDelAll().ToList();
            //int userId = SesssionUser.GetCurrentUserId();

            List<tbUsersDepts> UsersDepts = unitWork.UsersDeptsManager.GetAll().Where(m => m.UserID == CuruserId && m.Accessing == true).ToList();

            var UserInvetories = (from p in allDepts // get Rooms table as p
                                  join e in UsersDepts // implement join as e in Emp_rooms table
                                    on p.Id equals e.DeptID //implement join on rows where p.RoomId == e.RoomId
                                  select p).ToList();
            return UserInvetories;
        }
    }
}