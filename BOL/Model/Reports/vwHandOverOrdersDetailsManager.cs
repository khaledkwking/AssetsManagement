using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwHandOverOrdersDetailsManager : Repository<vwHandOverOrdersDetails>
    {

        public vwHandOverOrdersDetailsManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }

        
        public List<vwHandOverOrdersDetails> GetNotDelAllByParam(DateTime? FromDate, DateTime? Todate, int? ReasonId
            , long? StoreId, long? ItemId, int? CatMain_Id ,int? EmpId,int? DeptId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwHandOverOrdersDetails> List;
            if (FromDate != null && Todate != null)
            {
                //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.OverOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OverOrderDate) <= DbFunctions.TruncateTime(Todate))
             && ((c.StoreId == StoreId || StoreId == null)
             && (c.ReasonId == ReasonId || ReasonId == null)
             && (c.ItemId == ItemId || ItemId == null)
                 && (c.DeptId == DeptId || DeptId == null)
                  && (c.EmpId == EmpId || EmpId == null)
               && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            }
            else
            {
                List = GetAll().Where(c=> ((c.StoreId == StoreId || StoreId == null)
        && (c.ReasonId == ReasonId || ReasonId == null)
        && (c.ItemId == ItemId || ItemId == null)
            && (c.DeptId == DeptId || DeptId == null)
             && (c.EmpId == EmpId || EmpId == null)
          && (c.CatMain_Id == CatMain_Id || CatMain_Id == null)
        && (c.IsDeleted == false || c.IsDeleted == null))
       ).ToList();
            }
            //return List;
            return GetAllDetails(List);
        }
       
        public List<vwHandOverOrdersDetails> GetNotDelByOrderId(long? OverOrderId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwHandOverOrdersDetails> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            List = GetAll().Where((c =>(c.OverOrderId  == OverOrderId)
             && (c.IsDeleted == false || c.IsDeleted == null))
            ).ToList();
            //return List;
            return GetAllDetails(List);
        }
        public List<vwHandOverOrdersDetails> GetAllDetails(List<vwHandOverOrdersDetails> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetAll().ToList();
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
                    Item.DeptName = CurDeptList.Name;
                    //Item.VmDepartments = CurDeptList;
                }

            }
            return List;
        }

    }
}