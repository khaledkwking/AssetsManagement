using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using BOL;

public class UsersStoresManager : Repository<tbUsersStores>
{

    public UsersStoresManager(GPFAssetsEntities ctx) : base(ctx)
    {

    }


    public override bool Delete(tbUsersStores entity)
    {
        return base.Delete(entity);
    }
   
    public bool update(int id)
    {
        tbUsersStores st = GetById(id);
        //st.StudentName = Name;
        return Update(st);
    }
    public List<tbUsersStores> GetCastByUnitName(string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.Room_tbl.Room_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.tbUsers.FullName.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
    }
    public List<tbUsersStores> GetNotDelAll()
    {
        return GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
    }

    public List<tbUsersStores> GetByValue(int Value)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.Id == Value).ToList();
    }
    public List<tbUsersStores> GetByUserId(int UserId)
    {
        List<tbUsersStores> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        //List<vwEmployees> Employee = GetEmployee(Item.EmpId);
        //if (Item !=null)
        //{
        //    Item.VmEmployees = Employee.FirstOrDefault();
        //}
        return Items;
    }
    public tbUsersStores GetByUserIdAndStoreId(int UserId, long StoreId)
    {
        List<tbUsersStores> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId && c.StoreID==StoreId).ToList();
        
        return Items.FirstOrDefault();
    }
    public List<tbUsersStores> GetCastByName(int UserId, string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetNotDelAll().Where(c => (c.UserID == UserId) && (c.Room_tbl.Room_Name.ToUpper().Contains(UnitSearch.ToUpper()) || c.Room_tbl.Room_NameEn.ToUpper().Contains(UnitSearch.ToUpper()))).ToList();
    }
    public List<tbUsersStores> CheckByUserId(int UserId, int PageId)
    {
        List<tbUsersStores> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        foreach (tbUsersStores obj in Items)
        {

        }
        return Items;
    }
}


