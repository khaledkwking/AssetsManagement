using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using System.Data.Entity;
using BOL;

public class UsersDeptsManager : Repository<tbUsersDepts>
{

    public UsersDeptsManager(GPFAssetsEntities ctx) : base(ctx)
    {

    }


    public override bool Delete(tbUsersDepts entity)
    {
        return base.Delete(entity);
    }
   
    public bool update(int id)
    {
        tbUsersDepts st = GetById(id);
        //st.StudentName = Name;
        return Update(st);
    }
    public List<tbUsersDepts> GetCastByUnitName(string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.tbUsers.FullName.ToUpper().Contains(UnitSearch.ToUpper())).ToList();
    }
    public List<tbUsersDepts> GetNotDelAll()
    {
        List<tbUsersDepts> Items = GetAll().Where(c => c.IsDeleted == false || c.IsDeleted == null).ToList();
        UnitOfWork UWork = new UnitOfWork();
        List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
        foreach (tbUsersDepts obj in Items)
        {
            obj.VmDepartments = DeptList.Where(c => c.Id == obj.DeptID).FirstOrDefault();
        }
        return Items;
    }

    public List<tbUsersDepts> GetByValue(int Value)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetAll().Where(c => c.Id == Value).ToList();
    }
    public List<tbUsersDepts> GetByUserId(int UserId)
    {
        List<tbUsersDepts> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        UnitOfWork UWork = new UnitOfWork();
        List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();
        foreach (tbUsersDepts obj in Items)
        {
            obj.VmDepartments = DeptList.Where(c => c.Id == obj.DeptID).FirstOrDefault();
        }
        return Items;
        
    }
    public tbUsersDepts GetByUserIdAndDeptId(int UserId, int DeptId)
    {
        List<tbUsersDepts> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId && c.DeptID == DeptId).ToList();
        
        return Items.FirstOrDefault();
    }
    public List<tbUsersDepts> GetCastByName(int UserId, string UnitSearch)
    {
        //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));

        return GetNotDelAll().Where(c => (c.UserID == UserId) && (c.tbUsers.FullName.ToUpper().Contains(UnitSearch.ToUpper()))).ToList();
    }
    public List<tbUsersDepts> CheckByUserId(int UserId, int PageId)
    {
        List<tbUsersDepts> Items = GetAll().Where(c => (c.IsDeleted == false || c.IsDeleted == null) && c.UserID == UserId).ToList();
        foreach (tbUsersDepts obj in Items)
        {

        }
        return Items;
    }
}


