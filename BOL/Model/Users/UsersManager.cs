using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using BOL;

public class UsersManager : Repository<tbUsers>
{
   
    public UsersManager(GPFAssetsEntities ctx) : base(ctx)
    {

    }

    public override bool Delete(tbUsers entity)
    {
        return base.Delete(entity);
    }
    public tbUsers GetEmpByPassord(string UserName, string Password)
    {
        //a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault()

        return GetAll().Where(c => c.UserName.ToUpper().Equals(UserName.ToUpper()) && c.Password.ToUpper().Equals(Password.ToUpper())).FirstOrDefault();

    }
    public tbUsers GetByUserName(string UserName)
    {
        //a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault()
        string CurUserName = UserName.ToUpper();
        return GetAll().Where(c => c.UserName.ToUpper().Equals(CurUserName.ToUpper())).FirstOrDefault();

    }
    public bool update(int id, string Name, string address, int genderId)
    {
        tbUsers st = GetById(id);
        //st.StudentName = Name;
        return Update(st);
    }
    public List<tbUsers> GetNotDelAll()
    {
        return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
    }
    public List<tbUsers> GetCastByName(string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetNotDelAll().Where(c => c.FullName.ToUpper().Contains(UnitSearch.ToUpper()) || c.UserName.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
    }
    public List<vwEmployees> GetEmployee(int EmpId)
    {
        UnitOfWork UWork = new UnitOfWork();
        List<vwEmployees> Emp = new List<vwEmployees>();
        Emp = UWork.EmployeesManager.GetEmployeeByEmpId(EmpId );
        return Emp;
    }
    public tbUsers GetByUserId(int UserId)
    { 

        tbUsers Item = GetById(UserId);
        List<vwEmployees> Employee = GetEmployee(Item.EmpId.GetValueOrDefault ());
        if (Item !=null)
        {
            Item.VmEmployees = Employee.FirstOrDefault();
        }
        return Item;

    }

    public tbUsers GetUserByEmpId(int EmpId)
    {
        tbUsers UserObj = GetNotDelAll().Where(c => c.EmpId ==EmpId).FirstOrDefault();

        return UserObj;
        
    }
    public bool HasPermission(string AllowFeature ,string requiredPermission,int UserId)
    {
        UnitOfWork UWork = new UnitOfWork();
        bool bFound = false;
        //tbUsers UserObj = GetById(UserId);
        if (UserId > 0)
        {
            List<tbUsersPages> UsersPages = UWork.UsersPagesManager.GetByUserPermission(UserId, AllowFeature).ToList();
          


            //UsersPages= UsersPages.Where(p => p.tbSystemPages.PageName.ToLower() == AllowFeature.ToLower()).ToList();
            if (UsersPages != null)
            {
                if (UsersPages.Count > 0)
                {
                    tbUsersPages Obj = UsersPages.FirstOrDefault();
                    switch (requiredPermission)
                    {
                        case "Accessing":
                            bFound = Obj.Accessing.GetValueOrDefault();
                            break;
                        case "Adding":
                            bFound = Obj.Adding.GetValueOrDefault();
                            break;
                        case "Updating":
                            bFound = Obj.Updating.GetValueOrDefault();
                            break;
                        case "Deleting":
                            bFound = Obj.Deleting.GetValueOrDefault();
                            break;
                        case "Importing":
                            bFound = Obj.Importing.GetValueOrDefault();
                            break;
                    }

                }
            }
        }
        return bFound;
                 

    }

    public tbUsersPages UserPermission(int UserId, string AllowFeature)
    {
        UnitOfWork UWork = new UnitOfWork();
       
        tbUsersPages UsersPages = UWork.UsersPagesManager.GetByUserPermission(UserId, AllowFeature).FirstOrDefault();
        if (UsersPages !=null)
        {
            tbUsers requestingUser = UWork.UsersManager.GetById(UserId);
            if (requestingUser.RoleID == BOL.DataModel.AdminRoleId)
            {
                UsersPages.Adding = true;
                UsersPages.Updating  = true;
                UsersPages.Deleting = true;
                UsersPages.Importing = true;
                UsersPages.Accessing = true;
            }
        }
         return UsersPages;
    }

}


