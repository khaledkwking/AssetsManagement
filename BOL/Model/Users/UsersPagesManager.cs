using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using BOL;

public class UsersPagesManager : Repository<tbUsersPages>
{

    public UsersPagesManager(GPFAssetsEntities ctx) : base(ctx)
    {

    }

    public override bool Delete(tbUsersPages entity)
    {
        return base.Delete(entity);
    }
    
    
    public bool update(int id)
    {
        tbUsersPages st = GetById(id);
        //st.StudentName = Name;
        return Update(st);
    }
    public List<tbUsersPages> GetNotDelAll()
    {
        return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
    }
   
   
    public List<tbUsersPages> GetByUserId(int UserId)
    {
        List < tbUsersPages> Items =   GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID ==UserId  ).ToList();
        //List<vwEmployees> Employee = GetEmployee(Item.EmpId);
        //if (Item !=null)
        //{
        //    Item.VmEmployees = Employee.FirstOrDefault();
        //}
        return Items;
    }
    public List<tbUsersPages> GetByUserPermission(int UserId,string AllowFeature)
    {
        List<tbUsersPages> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId
        && c.tbSystemPages.PageName.ToLower() == AllowFeature.ToLower()).ToList();
      
        return Items;
    }
   
    public List<tbUsersPages> GetCastByName(int UserId,string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetNotDelAll().Where(c => (c.UserID ==UserId) && (c.tbSystemPages.PageName.ToUpper().Contains(UnitSearch.ToUpper()) || c.tbSystemPages.PageNameAr.ToUpper().Contains(UnitSearch.ToUpper()))).ToList();
    }

    public List<tbUsersPages> CheckByUserId(int UserId)
    {
        List<tbUsersPages> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();

        
        return Items;
    }



}


