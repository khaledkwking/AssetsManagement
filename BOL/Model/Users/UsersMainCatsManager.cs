using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using BOL;

public class UsersMainCatsManager : Repository<tbUsersMainCats>
{

    public UsersMainCatsManager(GPFAssetsEntities ctx) : base(ctx)
    {

    }


    public override bool Delete(tbUsersMainCats entity)
    {
        return base.Delete(entity);
    }
   
    public bool update(int id)
    {
        tbUsersMainCats st = GetById(id);
        //st.StudentName = Name;
        return Update(st);
    }
    public List<tbUsersMainCats> GetCastByUnitName(string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.CatMain_tbl.CatMain_Name .ToUpper().Contains(UnitSearch.ToUpper()) || c.tbUsers.FullName.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
    }
    public List<tbUsersMainCats> GetNotDelAll()
    {
        return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
    }

    public List<tbUsersMainCats> GetByValue(int Value)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.Id == Value).ToList();
    }
    public List<tbUsersMainCats> GetByUserId(int UserId)
    {
        List<tbUsersMainCats> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        //List<vwEmployees> Employee = GetEmployee(Item.EmpId);
        //if (Item !=null)
        //{
        //    Item.VmEmployees = Employee.FirstOrDefault();
        //}
        return Items;
    }
    public tbUsersMainCats GetByUserIdAndCatMainId(int UserId, int? CatMain_Id)
    {
        List<tbUsersMainCats> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId && c.CatMain_Id == CatMain_Id).ToList();
        
        return Items.FirstOrDefault();
    }
    public List<tbUsersMainCats> GetCastByName(int UserId, string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetNotDelAll().Where(c => (c.UserID == UserId) && (c.CatMain_tbl.CatMain_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.CatMain_tbl.CatMain_Name.ToUpper().Contains(UnitSearch.ToUpper()))).ToList();
    }
    public List<tbUsersMainCats> CheckByUserId(int UserId, int PageId)
    {
        List<tbUsersMainCats> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        foreach (tbUsersMainCats obj in Items)
        {

        }
        return Items;
    }
}


