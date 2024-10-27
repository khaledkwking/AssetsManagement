using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class UsersRolesManager : Repository<tbUsersRoles>
    {

        public UsersRolesManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        public override bool Delete(tbUsersRoles entity)
        {
            return base.Delete(entity);
        }
        //public List <Car> GetStudentByGender(int genderId)
        //{
        //    return GetAll().Where(c => c.genderId == genderId).ToList();
        //}
        public bool update(int id)
        {
            tbUsersRoles st = GetById(id);
            //st.StudentName = Name;
            return Update(st);
        }
        public List<tbUsersRoles> GetCastByUnitName(string UnitSearch)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

            return GetAll().Where(c => c.RoleName.ToUpper().Contains(UnitSearch.ToUpper()) || c.RoleNameAr.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
        }
        public List<tbUsersRoles> GetNotDelAll()
        {
            return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        }
    }
}