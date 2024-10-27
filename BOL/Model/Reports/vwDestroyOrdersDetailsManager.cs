using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwDestroyOrdersDetailsManager : Repository<vwDestroyOrdersDetails>
    {

        public vwDestroyOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        
        public List<vwDestroyOrdersDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate
            , long? StoreId, long? ItemId, int? CatMain_Id)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwDestroyOrdersDetails> List;
            if (FromDate != null && Todate != null)
            {
                //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
             List = GetAll().Where(c => (DbFunctions.TruncateTime(c.DestroyOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.DestroyOrderDate) <= DbFunctions.TruncateTime(Todate))
             && ((c.StoreId_From == StoreId || StoreId == null)
             && (c.ItemId == ItemId || ItemId == null)
             && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            }
            else
            {
                List = GetAll().Where(c =>((c.StoreId_From == StoreId || StoreId == null)
             && (c.ItemId == ItemId || ItemId == null)
             && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            }
            //return List;
            return GetAllDetails(List);
        }
      
        public List<vwDestroyOrdersDetails> GetNotDelByOrderId(long? DestroyOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwDestroyOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.DestroyOrderId  == DestroyOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            return List;
            //return GetAllDetails(List);
        }
        public List<vwDestroyOrdersDetails> GetAllDetails(List<vwDestroyOrdersDetails> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            foreach (var Item in List)
            {
                if (Item.EmpId != null)
                {
                    int EmpId = int.Parse(Item.EmpId.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpName = CurList.FULL_NAME_AR;
                    }
                    //Item.VmEmployees = CurList;
                }
                if (Item.DeptId != null)
                {
                    int DeptId = int.Parse(Item.DeptId.ToString());
                    vwDepartments CurDeptList = new vwDepartments();
                    CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
                    if (CurDeptList != null)
                    {
                        Item.DeptName = CurDeptList.Name;
                    }
                    //Item.VmDepartments = CurDeptList;
                }

            }
            return List;
        }

    }
}