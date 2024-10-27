using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DAL;
namespace BOL
{
    public class vwItemsStockManager : Repository<vwItemsStock>
    {

        public vwItemsStockManager(GPFAssetsEntities ctx) : base(ctx)
        {

        }


        public List<vwItemsStock> GetNotDelAllByParam(long? RoomId, int? FloorId, int? EmpId, int? DeptId, int searchType)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwItemsStock> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            switch (searchType)
            {
                case 1:
                    if (EmpId != null)
                    {
                        List = GetAll().Where(c => (c.Depart_Id == DeptId)
                                && (c.Emp_Id == EmpId) && (c.Emp_Id != 0) && (c.Emp_Id != null)
                                && (c.CountableFlag == false)).ToList();
                    }
                    else
                    {
                        List = GetAll().Where(c => (c.Depart_Id == DeptId || DeptId==null)
                             && (c.Emp_Id != 0) && (c.Emp_Id != null) 
                             && (c.Floor_Id == FloorId || FloorId == null)
                             && (c.Room_Id == RoomId || RoomId == null)
                              && (c.CountableFlag == false)).ToList();
                    }
                    break;
           
                case 2: // display room without emp
                    List = GetAll().Where(c => c.Room_Id == RoomId || RoomId == null
                        && (c.Floor_Id  == FloorId || FloorId == null)
                            && (c.Depart_Id == DeptId || DeptId == null)
                             && (c.Emp_Id  == 0 || c.Emp_Id == null) && (c.CountableFlag == false)).ToList();
                    break;
                default :
                        List = GetAll().Where(c => c.Room_Id == RoomId || RoomId == null
                         && (c.Floor_Id  == FloorId || FloorId == null)
                             && (c.Depart_Id == DeptId || DeptId == null)
                              && (c.Emp_Id == EmpId || EmpId == null) && (c.CountableFlag == false)).ToList();
                    break;
            //return List;
        }
            return GetAllDetails(List);
        }
       
       
        public List<vwItemsStock> GetAllDetails(List<vwItemsStock> List)
        {
            UnitOfWork UWork = new UnitOfWork();
            List<vwEmployees> empList = UWork.EmployeesManager.GetNotDelAll().ToList();
            List<vwDepartments> DeptList = UWork.DepartmentManager.GetNotDelAll().ToList();

            foreach (var Item in List)
            {
                if (Item.Emp_Id != null)
                {
                    int EmpId = int.Parse(Item.Emp_Id.ToString());
                    vwEmployees CurList = new vwEmployees();
                    CurList = empList.Where(c => c.Id == EmpId).FirstOrDefault();
                    if (CurList != null)
                    {
                        Item.EmpName = CurList.FULL_NAME_AR;
                        Item.EmpTitle = CurList.JobTitle;
                    }
                    //Item.VmEmployees = CurList;
                }
                if (Item.Depart_Id != null)
                {
                    int DeptId = int.Parse(Item.Depart_Id.ToString());
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

        public List<vwItemsStock> GetNotDelExpiredDateByParam(DateTime? FromDate, DateTime? Todate, long? ItemId)
        {
            //stu => stu.CarNo.ToUpper().Contains(Search_Data.ToUpper()) || stu.CarType.ToUpper().Contains(Search_Data.ToUpper()));
            List<vwItemsStock> List;
            //DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.OutOrders.OutOrderDate) <= DbFunctions.TruncateTime(Todate)
            if (FromDate != null && Todate != null)
            {
                List = GetAll().Where(c => (DbFunctions.TruncateTime(c.Item_Expire) >= DbFunctions.TruncateTime(FromDate) && DbFunctions.TruncateTime(c.Item_Expire) <= DbFunctions.TruncateTime(Todate))
                && (c.Item_Id == ItemId || ItemId == null)
                && (c.CountableFlag == false)).ToList();
            }
            else
            {
                List = GetAll().Where(c =>(c.Item_Id == ItemId || ItemId == null)
                && (c.CountableFlag == false)).ToList();
            }

            //List = GetAll().Where(c => c.Room_Id == RoomId || RoomId == null
            // && (c.Item_Id == ItemId || ItemId == null)
            //     && (c.Depart_Id == DeptId || DeptId == null)
            //      && (c.Emp_Id == EmpId || EmpId == null) && (c.CountableFlag == false)).ToList();
            //return List;
            return GetAllDetails(List);
        }


    }
}