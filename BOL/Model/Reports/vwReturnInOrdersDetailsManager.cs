using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwReturnInOrdersDetailsManager : Repository<vwReturnInOrdersDetails>
    {

        public vwReturnInOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }


        public List<vwReturnInOrdersDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate
         , long? StoreId, long? ItemId, int? CatMain_Id)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwReturnInOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            if (FromDate != null && Todate != null)
            {
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.ReturnOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.ReturnOrderDate) <= DbFunctions.TruncateTime(Todate))
             && ((c.StoreId == StoreId || StoreId == null)
             && (c.Item_Id == ItemId || ItemId == null)
             && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            }
            else
            {
                List = GetAll().Where(c => ((c.StoreId == StoreId || StoreId == null)
            && (c.Item_Id == ItemId || ItemId == null)
            && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
            && (c.IsDeleted == false || c.IsDeleted == null))
           ).ToList();
            }


            return List;
            //return GetAllDetails(List);
        }

        public List<vwReturnInOrdersDetails> GetNotDelByOrderId(long? ReturnOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwReturnInOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c => (c.ReturnOrderId == ReturnOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            return List;
            //return GetAllDetails(List);
        }
        //public List<vwReturnInOrdersDetails> GetAllDetails(List<vwReturnInOrdersDetails> List)
        //{
        //    UnitOfWork UWork = new UnitOfWork();
        //    List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
        //    List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

        //    foreach (var Item in List)
        //    {
        //        if (Item.EmpId != null)
        //        {
        //            int EmpId = int.Parse(Item.EmpId.ToString());
        //            vwEmployees CurList = new vwEmployees();
        //            CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
        //            Item.EmpName = CurList.FULL_NAME_AR;
        //            //Item.VmEmployees = CurList;
        //        }
        //        if (Item.DeptId != null)
        //        {
        //            int DeptId = int.Parse(Item.DeptId.ToString());
        //            vwDepartments CurDeptList = new vwDepartments();
        //            CurDeptList = DeptList.Where(c => c.Id == DeptId).FirstOrDefault();
        //            Item.DeptName = CurDeptList.Name;
        //            //Item.VmDepartments = CurDeptList;
        //        }

        //    }
        //    return List;
        //}

    }
}